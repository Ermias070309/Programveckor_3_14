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
    public float totalTime = 180f; // ÄNDRAT FRÅN 120f TILL 180f (3 MINUTER)
    public int gridSize = 4;

    // VARIABLER FÖR ATT HÅLLA KOLL PÅ BRICKOR OCH POSITIONER
    private List<GameObject> tiles = new List<GameObject>();
    private Dictionary<Vector2Int, GameObject> tileAtPosition = new Dictionary<Vector2Int, GameObject>();
    private Vector2Int emptyGridPos = new Vector2Int(3, 3); // Nedre högra hörnet (kolumn, rad)

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

    void CreatePuzzleTiles()
    {
        // RENSA ALLT FÖRST
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
                // SKAPA BARA BRICKOR FÖR POSITIONER SOM INTE ÄR TOM
                if (!(row == gridSize - 1 && col == gridSize - 1))
                {
                    GameObject tile = Instantiate(tilePrefab, gridContainer);

                    // Använd gridPositions för att placera brickan
                    Vector3 position = new Vector3(
                        gridPositions[row, col].x,
                        gridPositions[row, col].y,
                        0
                    );

                    tile.transform.localPosition = position;
                    tile.transform.localScale = Vector3.one * 1.8f;

                    // Sätt färg
                    SpriteRenderer renderer = tile.GetComponent<SpriteRenderer>();
                    float hue = (float)tileNumber / 16f;
                    renderer.color = Color.HSVToRGB(hue, 0.8f, 1f);

                    // Lägg till TileInfo
                    TileInfo info = tile.GetComponent<TileInfo>();
                    if (info == null) info = tile.AddComponent<TileInfo>();
                    info.correctRow = row;
                    info.correctCol = col;
                    info.tileNumber = tileNumber;

                    // Lägg till nummertext
                    CreateNumberText(tile, tileNumber);

                    // Spara brickan i listan
                    tiles.Add(tile);

                    // Spara positionen i dictionary
                    Vector2Int gridPos = new Vector2Int(col, row); // x = kolumn, y = rad
                    tileAtPosition[gridPos] = tile;

                    tileNumber++;
                }
            }
        }

        // Sätt den tomma positionen
        emptyGridPos = new Vector2Int(gridSize - 1, gridSize - 1);

        Debug.Log($"Skapade {tiles.Count} brickor");
        Debug.Log($"Tom position: ({emptyGridPos.x}, {emptyGridPos.y})");
    }

    void CreateNumberText(GameObject tile, int number)
    {
        GameObject textObj = new GameObject("TileNumber");
        textObj.transform.SetParent(tile.transform);
        textObj.transform.localPosition = Vector3.zero;

        TextMesh text = textObj.AddComponent<TextMesh>();
        text.text = number.ToString();
        text.fontSize = 80;
        text.characterSize = 0.08f;
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

    // ENKEL SHUFFLE SOM INTE ÖVERLAPPAR BRICKOR
    void ShufflePuzzle()
    {
        Debug.Log("Startar shuffle utan överlappning...");

        // 1. SKAPA EN LISTA MED ALLA POSITIONER SOM KAN HA BRICKOR
        List<Vector2Int> availablePositions = new List<Vector2Int>();
        for (int row = 0; row < gridSize; row++)
        {
            for (int col = 0; col < gridSize; col++)
            {
                // Lägg till alla positioner UTOM den tomma
                if (!(row == gridSize - 1 && col == gridSize - 1))
                {
                    availablePositions.Add(new Vector2Int(col, row));
                }
            }
        }

        // 2. BLANDA LISTAN
        for (int i = 0; i < availablePositions.Count; i++)
        {
            int randomIndex = Random.Range(i, availablePositions.Count);
            Vector2Int temp = availablePositions[i];
            availablePositions[i] = availablePositions[randomIndex];
            availablePositions[randomIndex] = temp;
        }

        // 3. PLACERA VARJE BRICKA PÅ EN UNIK POSITION
        tileAtPosition.Clear();

        for (int i = 0; i < tiles.Count; i++)
        {
            if (i < availablePositions.Count)
            {
                Vector2Int pos = availablePositions[i];
                Vector3 worldPos = new Vector3(
                    gridPositions[pos.y, pos.x].x,  // row = y, col = x
                    gridPositions[pos.y, pos.x].y,
                    0
                );

                tiles[i].transform.localPosition = worldPos;
                tileAtPosition[pos] = tiles[i];

                Debug.Log($"Placerade bricka {i + 1} på position ({pos.x}, {pos.y})");
            }
        }

        // 4. DEN TOMMA POSITIONEN ÄR ALLTID NEDRE HÖGRA HÖRNET
        emptyGridPos = new Vector2Int(gridSize - 1, gridSize - 1);

        Debug.Log($"Shuffle klar! Tom position: ({emptyGridPos.x}, {emptyGridPos.y})");

        // 5. GÖR NÅGRA ENKLA DRAG FÖR ATT VERKLIGEN BLANDA
        MakeSomeRandomMoves();
    }

    void MakeSomeRandomMoves()
    {
        // Gör 20 slumpmässiga men giltiga drag
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

    void HandleTileClick()
    {
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

        if (hit.collider != null)
        {
            GameObject clickedTile = hit.collider.gameObject;

            // Kolla om brickan kan flyttas
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
                Debug.Log("Brickan kan inte flyttas (inte bredvid tom position)");
            }
        }
    }

    // KONTROLLERA OM EN BRICKA KAN FLYTTAS
    bool CanTileMove(GameObject tile)
    {
        // HITTA BRICKANS POSITION
        Vector2Int tilePos = FindTilePosition(tile);

        if (tilePos.x == -1)
        {
            Debug.Log("Kunde inte hitta brickans position");
            return false;
        }

        // EN BRICKA KAN BARA FLYTTAS OM DEN ÄR BREDVID DEN TOMMA POSITIONEN
        // Det betyder: samma rad och kolumn skillnad 1, ELLER samma kolumn och rad skillnad 1
        bool canMove = (tilePos.x == emptyGridPos.x && Mathf.Abs(tilePos.y - emptyGridPos.y) == 1) ||
                       (tilePos.y == emptyGridPos.y && Mathf.Abs(tilePos.x - emptyGridPos.x) == 1);

        Debug.Log($"Bricka på ({tilePos.x},{tilePos.y}), tom på ({emptyGridPos.x},{emptyGridPos.y}) - Kan flyttas: {canMove}");
        return canMove;
    }

    // HITTA EN BRICKAS POSITION I VÅRT SYSTEM
    Vector2Int FindTilePosition(GameObject tile)
    {
        foreach (var entry in tileAtPosition)
        {
            if (entry.Value == tile)
            {
                return entry.Key;
            }
        }
        return new Vector2Int(-1, -1); // Hittades inte
    }

    // HÄMTA DEN TOMMA POSITIONENS VÄRLDSKOORDINATER
    Vector3 GetEmptyPosition()
    {
        return new Vector3(
            gridPositions[emptyGridPos.y, emptyGridPos.x].x,
            gridPositions[emptyGridPos.y, emptyGridPos.x].y,
            0
        );
    }

    // FLYTTA EN BRICKA
    void MoveTile(GameObject tile)
    {
        // HITTA BRICKANS NUvarande POSITION
        Vector2Int tilePos = FindTilePosition(tile);

        if (tilePos.x == -1)
        {
            Debug.LogError("Kunde inte hitta brickans position!");
            return;
        }

        // BERÄKNA VAR BRICKAN SKA FLYTTAS (till den tomma positionen)
        Vector3 newWorldPos = GetEmptyPosition();

        // FLYTTA BRICKAN VISUELLT
        tile.transform.localPosition = newWorldPos;

        // UPPDATERA VÅRT SYSTEM:
        // 1. Ta bort brickan från dess gamla position
        tileAtPosition.Remove(tilePos);

        // 2. Lägg till brickan på den nya positionen (där den tomma var)
        tileAtPosition[emptyGridPos] = tile;

        // 3. Uppdatera emptyGridPos till där brickan KOM FRÅN
        emptyGridPos = tilePos;

        Debug.Log($"Flyttade bricka från ({tilePos.x},{tilePos.y}) till ({emptyGridPos.x},{emptyGridPos.y})");
    }

    // HÄMTA ALLA BRICKOR SOM KAN FLYTTAS
    List<GameObject> GetMovableTiles()
    {
        List<GameObject> movable = new List<GameObject>();

        // KONTROLLERA ALLA 4 POSITIONER BREDVID DEN TOMMA
        Vector2Int[] directions = {
            new Vector2Int(1, 0),   // Höger
            new Vector2Int(-1, 0),  // Vänster
            new Vector2Int(0, 1),   // Upp
            new Vector2Int(0, -1)   // Ner
        };

        foreach (Vector2Int dir in directions)
        {
            Vector2Int checkPos = emptyGridPos + dir;

            // KONTROLLERA OM POSITIONEN ÄR INOM GRIDET
            if (checkPos.x >= 0 && checkPos.x < gridSize &&
                checkPos.y >= 0 && checkPos.y < gridSize)
            {
                // KONTROLLERA OM DET FINNS EN BRICKA PÅ DENNA POSITION
                if (tileAtPosition.ContainsKey(checkPos))
                {
                    movable.Add(tileAtPosition[checkPos]);
                }
            }
        }

        return movable;
    }

    // KONTROLLERA OM PUZZLET ÄR LÖST
    bool IsPuzzleSolved()
    {
        foreach (GameObject tile in tiles)
        {
            TileInfo info = tile.GetComponent<TileInfo>();
            Vector2Int currentPos = FindTilePosition(tile);

            // En bricka är på rätt plats om:
            // currentPos.x == info.correctCol OCH currentPos.y == info.correctRow
            if (currentPos.x != info.correctCol || currentPos.y != info.correctRow)
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

        // BLINKA RÖD NÄR DET ÄR MINDRE ÄN 1 MINUT KVAR
        if (currentTime <= 60f)
        {
            float flash = Mathf.Sin(Time.time * 10f) * 0.5f + 0.5f;
            timerText.color = Color.Lerp(Color.white, Color.red, flash);
        }
        else if (currentTime <= 120f) // GUL VID 2 MINUTER KVAR
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