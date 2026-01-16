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
    public Sprite puzzleImage; // Dra din 256x256 bild hit i Unity

    [Header("UI ELEMENTS")]
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI statusText;
    public Image previewImage;
    public GameObject winScreen;
    public GameObject loseScreen;

    [Header("GAME SETTINGS")]
    public float totalTime = 240f; // 4 minuter
    public int gridSize = 4; // 4x4 pussel

    // PRIVATA VARIABLER
    private List<GameObject> tiles = new List<GameObject>();
    private Dictionary<Vector2Int, GameObject> tileAtPosition = new Dictionary<Vector2Int, GameObject>();
    private Vector2Int emptyGridPos = new Vector2Int(3, 3); // Nedre högra hörnet

    private float currentTime;
    private bool gameActive = false;
    private bool isShowingPreview = true;
    private Vector2[,] gridPositions;

    // ========== START ==========
    void Start()
    {
        SetupGridPositions();
        CreatePuzzleTiles();
        StartCoroutine(GameIntroduction());
    }

    // ========== UPDATE ==========
    void Update()
    {
        if (!gameActive || isShowingPreview) return;

        // Timer
        currentTime -= Time.deltaTime;
        UpdateTimerDisplay();

        if (currentTime <= 0)
        {
            GameOver();
            return;
        }

        // Klickhantering
        if (Input.GetMouseButtonDown(0))
        {
            HandleTileClick();
        }
    }

    // ========== SETUP GRID POSITIONS ==========
    void SetupGridPositions()
    {
        gridPositions = new Vector2[gridSize, gridSize];
        float spacing = 3.5f;
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

    // ========== CREATE PUZZLE TILES ==========
    void CreatePuzzleTiles()
    {
        // Rensa gamla brickor
        foreach (Transform child in gridContainer)
        {
            Destroy(child.gameObject);
        }
        tiles.Clear();
        tileAtPosition.Clear();

        int tileNumber = 1;
        for (int row = 0; row < gridSize; row++)
        {
            for (int col = 0; col < gridSize; col++)
            {
                // Hoppa över den tomma positionen (nedre högra hörnet)
                if (row == gridSize - 1 && col == gridSize - 1)
                    continue;

                // Skapa ny bricka
                GameObject tile = Instantiate(tilePrefab, gridContainer);

                // Placera brickan
                Vector3 position = new Vector3(
                    gridPositions[row, col].x,
                    gridPositions[row, col].y,
                    0
                );

                tile.transform.localPosition = position;
                tile.transform.localScale = Vector3.one * 1.8f;

                // Hämta SpriteRenderer
                SpriteRenderer renderer = tile.GetComponent<SpriteRenderer>();

                // ANVÄND PUZZELBILDEN OM DEN FINNS
                if (puzzleImage != null)
                {
                    // Skapa sprite från rätt del av bilden
                    SetTileSpriteFromImage(renderer, row, col, tileNumber);
                }
                else
                {
                    // Fallback: använd färger om ingen bild
                    float hue = (float)tileNumber / 16f;
                    renderer.color = Color.HSVToRGB(hue, 0.8f, 1f);
                }

                // Lägg till TileInfo
                TileInfo info = tile.GetComponent<TileInfo>();
                if (info == null) info = tile.AddComponent<TileInfo>();
                info.correctRow = row;
                info.correctCol = col;
                info.tileNumber = tileNumber;

                // Spara brickan i listorna
                tiles.Add(tile);
                Vector2Int gridPos = new Vector2Int(col, row);
                tileAtPosition[gridPos] = tile;

                tileNumber++;
            }
        }

        // Sätt den tomma positionen
        emptyGridPos = new Vector2Int(gridSize - 1, gridSize - 1);

        Debug.Log($"Skapade {tiles.Count} brickor");
        Debug.Log($"Tom position: ({emptyGridPos.x}, {emptyGridPos.y})");
    }

    // ========== SET TILE SPRITE FROM IMAGE ==========
    void SetTileSpriteFromImage(SpriteRenderer renderer, int row, int col, int tileNumber)
    {
        if (puzzleImage == null)
        {
            Debug.LogError("Ingen pusselbild tilldelad!");
            return;
        }

        // Hämta originaltexturen från bilden
        Texture2D originalTexture = puzzleImage.texture;

        // DEBUG: Visa bildstorlek
        Debug.Log($"Pusselbild: {originalTexture.width}x{originalTexture.height} pixlar");

        // Beräkna storlek på varje bricka (256 / 4 = 64 pixlar)
        int tileWidth = originalTexture.width / 4;
        int tileHeight = originalTexture.height / 4;

        // Beräkna vilken del av bilden denna bricka ska visa
        // Kolumn bestämmer X, rad bestämmer Y
        int x = col * tileWidth;
        int y = (3 - row) * tileHeight; // Vänd Y-axeln (börja uppifrån)

        // DEBUG info
        Debug.Log($"Bricka {tileNumber} på position ({row},{col}):");
        Debug.Log($"  Använder pixlar: X:{x}-{x + tileWidth}, Y:{y}-{y + tileHeight}");

        // Skapa en ny sprite från just denna del av bilden
        Sprite tileSprite = Sprite.Create(
            originalTexture,                    // Originaltexturen
            new Rect(x, y, tileWidth, tileHeight), // Vilken del att använda
            new Vector2(0.5f, 0.5f),           // Pivot i mitten
            100f,                               // Pixels Per Unit
            0,                                  // Extra pixels
            SpriteMeshType.Tight,               // Mesh typ
            Vector4.zero,                       // Border
            false                               // Generate fallback physics shape
        );

        // Applicera spriten på brickans renderer
        renderer.sprite = tileSprite;

        Debug.Log($"Bricka {tileNumber} sprite skapad!");
    }

    // ========== GAME INTRODUCTION ==========
    IEnumerator GameIntroduction()
    {
        isShowingPreview = true;
        statusText.text = "MEMORIZE THE IMAGE!";

        // Visa förhandsvisningen om vi har en bild
        if (previewImage.sprite != null)
        {
            previewImage.gameObject.SetActive(true);
        }

        // Vänta 3 sekunder
        yield return new WaitForSeconds(3f);

        // Dölj förhandsvisning och starta spelet
        previewImage.gameObject.SetActive(false);
        statusText.text = "REPAIR THE COMMS SYSTEM!";
        isShowingPreview = false;

        // Blanda pusslet
        ShufflePuzzle();

        // Starta spelet
        gameActive = true;
        currentTime = totalTime;
        UpdateTimerDisplay();
    }

    // ========== SHUFFLE PUZZLE ==========
    void ShufflePuzzle()
    {
        Debug.Log("Blandar pusslet...");

        // 1. Skapa lista med alla positioner
        List<Vector2Int> availablePositions = new List<Vector2Int>();
        for (int row = 0; row < gridSize; row++)
        {
            for (int col = 0; col < gridSize; col++)
            {
                if (!(row == gridSize - 1 && col == gridSize - 1))
                {
                    availablePositions.Add(new Vector2Int(col, row));
                }
            }
        }

        // 2. Blanda positionerna
        for (int i = 0; i < availablePositions.Count; i++)
        {
            int randomIndex = Random.Range(i, availablePositions.Count);
            Vector2Int temp = availablePositions[i];
            availablePositions[i] = availablePositions[randomIndex];
            availablePositions[randomIndex] = temp;
        }

        // 3. Placera brickor på unika positioner
        tileAtPosition.Clear();

        for (int i = 0; i < tiles.Count; i++)
        {
            if (i < availablePositions.Count)
            {
                Vector2Int pos = availablePositions[i];
                Vector3 worldPos = new Vector3(
                    gridPositions[pos.y, pos.x].x,
                    gridPositions[pos.y, pos.x].y,
                    0
                );

                tiles[i].transform.localPosition = worldPos;
                tileAtPosition[pos] = tiles[i];
            }
        }

        // 4. Sätt tom position
        emptyGridPos = new Vector2Int(gridSize - 1, gridSize - 1);

        Debug.Log($"Pussel blandat! Tom position: ({emptyGridPos.x},{emptyGridPos.y})");

        // 5. Gör några slumpmässiga drag för att verkligen blanda
        MakeSomeRandomMoves();
    }

    void MakeSomeRandomMoves()
    {
        for (int i = 0; i < 20; i++)
        {
            List<GameObject> movableTiles = GetMovableTiles();
            if (movableTiles.Count > 0)
            {
                GameObject randomTile = movableTiles[Random.Range(0, movableTiles.Count)];
                MoveTile(randomTile);
            }
        }
    }

    // ========== HANDLE TILE CLICK ==========
    void HandleTileClick()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        if (hit.collider != null)
        {
            GameObject clickedTile = hit.collider.gameObject;

            if (CanTileMove(clickedTile))
            {
                MoveTile(clickedTile);

                // Kolla om pusslet är löst
                if (IsPuzzleSolved())
                {
                    WinGame();
                }
            }
        }
    }

    // ========== CAN TILE MOVE ==========
    bool CanTileMove(GameObject tile)
    {
        Vector2Int tilePos = FindTilePosition(tile);

        if (tilePos.x == -1) return false;

        // En bricka kan bara flyttas om den är bredvid den tomma positionen
        bool canMove = (tilePos.x == emptyGridPos.x && Mathf.Abs(tilePos.y - emptyGridPos.y) == 1) ||
                       (tilePos.y == emptyGridPos.y && Mathf.Abs(tilePos.x - emptyGridPos.x) == 1);

        return canMove;
    }

    // ========== FIND TILE POSITION ==========
    Vector2Int FindTilePosition(GameObject tile)
    {
        foreach (var entry in tileAtPosition)
        {
            if (entry.Value == tile)
            {
                return entry.Key;
            }
        }
        return new Vector2Int(-1, -1);
    }

    // ========== MOVE TILE ==========
    void MoveTile(GameObject tile)
    {
        // Hitta brickans nuvarande position
        Vector2Int tilePos = FindTilePosition(tile);

        if (tilePos.x == -1)
        {
            Debug.LogError("Kunde inte hitta brickans position!");
            return;
        }

        // Beräkna var brickan ska flyttas (till den tomma positionen)
        Vector3 newWorldPos = new Vector3(
            gridPositions[emptyGridPos.y, emptyGridPos.x].x,
            gridPositions[emptyGridPos.y, emptyGridPos.x].y,
            0
        );

        // Flytta brickan visuellt
        tile.transform.localPosition = newWorldPos;

        // Uppdatera vårt positionssystem
        tileAtPosition.Remove(tilePos);
        tileAtPosition[emptyGridPos] = tile;
        emptyGridPos = tilePos;
    }

    // ========== GET MOVABLE TILES ==========
    List<GameObject> GetMovableTiles()
    {
        List<GameObject> movable = new List<GameObject>();

        // Kolla alla 4 positioner bredvid den tomma
        Vector2Int[] directions = {
            new Vector2Int(1, 0),   // Höger
            new Vector2Int(-1, 0),  // Vänster
            new Vector2Int(0, 1),   // Upp
            new Vector2Int(0, -1)   // Ner
        };

        foreach (Vector2Int dir in directions)
        {
            Vector2Int checkPos = emptyGridPos + dir;

            // Kolla om positionen är inom gridet
            if (checkPos.x >= 0 && checkPos.x < gridSize &&
                checkPos.y >= 0 && checkPos.y < gridSize)
            {
                // Kolla om det finns en bricka på denna position
                if (tileAtPosition.ContainsKey(checkPos))
                {
                    movable.Add(tileAtPosition[checkPos]);
                }
            }
        }

        return movable;
    }

    // ========== IS PUZZLE SOLVED ==========
    bool IsPuzzleSolved()
    {
        foreach (GameObject tile in tiles)
        {
            TileInfo info = tile.GetComponent<TileInfo>();
            Vector2Int currentPos = FindTilePosition(tile);

            // En bricka är på rätt plats om dess nuvarande position
            // matchar dess korrekta position (lagrad i TileInfo)
            if (currentPos.x != info.correctCol || currentPos.y != info.correctRow)
            {
                return false;
            }
        }
        return true;
    }

    // ========== UPDATE TIMER DISPLAY ==========
    void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(currentTime / 60);
        int seconds = Mathf.FloorToInt(currentTime % 60);
        timerText.text = $"TIME: {minutes:00}:{seconds:00}";

        // Färgändring baserat på tid kvar
        if (currentTime <= 60f) // Mindre än 1 minut kvar
        {
            float flash = Mathf.Sin(Time.time * 10f) * 0.5f + 0.5f;
            timerText.color = Color.Lerp(Color.white, Color.red, flash);
        }
        else if (currentTime <= 120f) // 1-2 minuter kvar
        {
            timerText.color = Color.yellow;
        }
        else // Mer än 2 minuter kvar
        {
            timerText.color = Color.white;
        }
    }

    // ========== WIN GAME ==========
    void WinGame()
    {
        gameActive = false;
        statusText.text = "COMMUNICATION RESTORED!";
        statusText.color = Color.green;
        winScreen.SetActive(true);
        Invoke("LoadNextLevel", 3f);
    }

    // ========== GAME OVER ==========
    void GameOver()
    {
        gameActive = false;
        statusText.text = "TRANSMISSION FAILED!";
        statusText.color = Color.red;
        loseScreen.SetActive(true);
    }

    // ========== SCENE MANAGEMENT ==========
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