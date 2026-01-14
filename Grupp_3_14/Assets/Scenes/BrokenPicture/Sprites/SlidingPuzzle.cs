using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class SlidingPuzzle : MonoBehaviour
{
    [Header("PUZZLE SETTINGS")]
    public GameObject tilePrefab;
    public Transform gridContainer;
    public Sprite puzzleImage;

    [Header("UI ELEMENTS")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI statusText;
    public Image previewImage;
    public GameObject winScreen;
    public GameObject loseScreen;

    [Header("GAME SETTINGS")]
    public float totalTime = 120f;
    public int gridSize = 4;

    private List<GameObject> tiles = new List<GameObject>();
    private int emptyIndex = 15;
    private float currentTime;
    private bool gameActive = false;
    private bool isShowingPreview = true;
    private Vector2[,] gridPositions;

    void Start()
    {
        SetupGridPositions();
        CreatePuzzleTiles();
        StartCoroutine(GameIntroduction());
    }

    void Update()
    {
        if (!gameActive || isShowingPreview) return;

        currentTime -= Time.deltaTime;
        UpdateTimerDisplay();

        if (currentTime <= 0)
        {
            GameOver();
            return;
        }

        if (Input.GetMouseButtonDown(0))
        {
            HandleTileClick();
        }
    }

    void SetupGridPositions()
    {
        gridPositions = new Vector2[gridSize, gridSize];
        float spacing = 3.5f; // CHANGED FROM 2.0f to 3.5f (BIGGER!)
        float offsetX = -((gridSize - 1) * spacing) / 2f;
        float offsetY = ((gridSize - 1) * spacing) / 2f;

        for (int row = 0; row < gridSize; row++)
        {
            for (int col = 0; col < gridSize; col++)
            {
                float x = offsetX + (col * spacing);
                float y = offsetY - (row * spacing);
                gridPositions[row, col] = new Vector2(x, y);
            }
        }
    }

    void CreatePuzzleTiles()
    {
        foreach (Transform child in gridContainer)
        {
            Destroy(child.gameObject);
        }
        tiles.Clear();

        int tileNumber = 1;
        for (int row = 0; row < gridSize; row++)
        {
            for (int col = 0; col < gridSize; col++)
            {
                if (row == gridSize - 1 && col == gridSize - 1)
                    continue;

                GameObject tile = Instantiate(tilePrefab, gridContainer);
                tile.transform.localPosition = new Vector3(
                    gridPositions[row, col].x,
                    gridPositions[row, col].y,
                    0
                );

                // ADDED: Scale the tile to make it bigger
                tile.transform.localScale = Vector3.one * 1.8f; // 1.8x bigger

                SpriteRenderer renderer = tile.GetComponent<SpriteRenderer>();
                float hue = (float)tileNumber / 16f;
                Color color = Color.HSVToRGB(hue, 0.8f, 1f);
                renderer.color = color;

                TileInfo info = tile.GetComponent<TileInfo>();
                if (info == null) info = tile.AddComponent<TileInfo>();
                info.correctRow = row;
                info.correctCol = col;
                info.tileNumber = tileNumber;

                CreateNumberText(tile, tileNumber);
                tiles.Add(tile);
                tileNumber++;
            }
        }

        // DEBUG: Log tile positions
        Debug.Log($"Created {tiles.Count} tiles. First tile at: {tiles[0].transform.position}");
    }

    void CreateNumberText(GameObject tile, int number)
    {
        GameObject textObj = new GameObject("TileNumber");
        textObj.transform.SetParent(tile.transform);
        textObj.transform.localPosition = Vector3.zero;

        TextMesh text = textObj.AddComponent<TextMesh>();
        text.text = number.ToString();
        text.fontSize = 80; // INCREASED FROM 50 to 80 (bigger numbers)
        text.characterSize = 0.08f; // ADJUSTED for scaling
        text.anchor = TextAnchor.MiddleCenter;
        text.color = Color.black;
        text.alignment = TextAlignment.Center;
    }

    IEnumerator GameIntroduction()
    {
        isShowingPreview = true;
        statusText.text = "MEMORIZE THE DISPLAY!";
        if (previewImage.sprite != null)
        {
            previewImage.gameObject.SetActive(true);
        }

        yield return new WaitForSeconds(3f);

        previewImage.gameObject.SetActive(false);
        statusText.text = "REPAIR THE COMMS SYSTEM!";
        isShowingPreview = false;

        ShufflePuzzle();
        gameActive = true;
        currentTime = totalTime;
        UpdateTimerDisplay();
    }

    void ShufflePuzzle()
    {
        int shuffleCount = 100;
        for (int i = 0; i < shuffleCount; i++)
        {
            List<GameObject> movableTiles = GetMovableTiles();
            if (movableTiles.Count > 0)
            {
                GameObject randomTile = movableTiles[Random.Range(0, movableTiles.Count)];
                MoveTile(randomTile);
            }
        }
    }

    void HandleTileClick()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Debug.Log($"Mouse clicked at world position: {mousePos}"); // DEBUG

        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        if (hit.collider != null)
        {
            Debug.Log($"Hit: {hit.collider.gameObject.name}"); // DEBUG
            GameObject clickedTile = hit.collider.gameObject;

            // Visual feedback - flash the tile
            StartCoroutine(FlashTile(clickedTile));

            if (CanTileMove(clickedTile))
            {
                MoveTile(clickedTile);
                if (IsPuzzleSolved())
                {
                    WinGame();
                }
            }
            else
            {
                Debug.Log("Tile cannot move (not adjacent to empty space)"); // DEBUG
            }
        }
        else
        {
            Debug.Log("No collider hit! Check Box Collider 2D on tiles."); // DEBUG
        }
    }

    IEnumerator FlashTile(GameObject tile)
    {
        SpriteRenderer renderer = tile.GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            Color original = renderer.color;
            renderer.color = Color.white;
            yield return new WaitForSeconds(0.1f);
            renderer.color = original;
        }
    }

    bool CanTileMove(GameObject tile)
    {
        Vector3 tilePos = tile.transform.localPosition;
        Vector3 emptyPos = GetEmptyPosition();
        float distance = Vector3.Distance(tilePos, emptyPos);

        // ADJUSTED for bigger tiles: 3.5f * 1.2f ≈ 4.2f
        float maxDistance = 3.5f * 1.2f;
        return distance < maxDistance;
    }

    Vector3 GetEmptyPosition()
    {
        int emptyRow = emptyIndex / gridSize;
        int emptyCol = emptyIndex % gridSize;
        return new Vector3(
            gridPositions[emptyRow, emptyCol].x,
            gridPositions[emptyRow, emptyCol].y,
            0
        );
    }

    void MoveTile(GameObject tile)
    {
        Vector3 emptyPos = GetEmptyPosition();
        tile.transform.localPosition = emptyPos;

        for (int i = 0; i < tiles.Count; i++)
        {
            if (tiles[i] == tile)
            {
                emptyIndex = i;
                break;
            }
        }

        // Play sound effect (you can add this later)
        // AudioManager.PlaySound("slide");
    }

    List<GameObject> GetMovableTiles()
    {
        List<GameObject> movable = new List<GameObject>();
        Vector3 emptyPos = GetEmptyPosition();

        foreach (GameObject tile in tiles)
        {
            if (CanTileMove(tile))
            {
                movable.Add(tile);
            }
        }
        return movable;
    }

    bool IsPuzzleSolved()
    {
        foreach (GameObject tile in tiles)
        {
            TileInfo info = tile.GetComponent<TileInfo>();
            Vector3 currentPos = tile.transform.localPosition;
            Vector3 correctPos = new Vector3(
                gridPositions[info.correctRow, info.correctCol].x,
                gridPositions[info.correctRow, info.correctCol].y,
                0
            );

            if (Vector3.Distance(currentPos, correctPos) > 0.5f) // INCREASED tolerance
            {
                return false;
            }
        }
        return true;
    }

    void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);
        timerText.text = $"TIME: {minutes:00}:{seconds:00}";

        if (currentTime <= 30f)
        {
            float flash = Mathf.Sin(Time.time * 10f) * 0.5f + 0.5f;
            timerText.color = Color.Lerp(Color.white, Color.red, flash);
        }
        else if (currentTime <= 60f)
        {
            timerText.color = Color.yellow;
        }
        else
        {
            timerText.color = Color.white;
        }
    }

    void WinGame()
    {
        gameActive = false;
        statusText.text = "COMMUNICATION RESTORED!";
        statusText.color = Color.green;
        winScreen.SetActive(true);
        Invoke("LoadNextLevel", 3f);
    }

    void GameOver()
    {
        gameActive = false;
        statusText.text = "TRANSMISSION FAILED!";
        statusText.color = Color.red;
        loseScreen.SetActive(true);
    }

    void LoadNextLevel()
    {
        SceneManager.LoadScene("EndingScene");
    }

    public void RestartPuzzle()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}