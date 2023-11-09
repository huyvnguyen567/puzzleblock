using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class GameController : MonoBehaviour
{

    public static GameController Instance;
    public Transform gridContainer; // Container chứa các ô trong lưới grid
    private Transform spawnPoints;
    private Transform board;

    [SerializeField] private GameObject background;

    [SerializeField] private ObjectPooling pool;
    [SerializeField] private GameObject gridContainPrefab;
    [SerializeField] private GameObject spawnPointsPrefab;
    [SerializeField] private GameObject boardPrefab;

    [SerializeField] private GameObject tilePrefab; // Prefab của ô (tile)
    [SerializeField] private int width = 9;
    [SerializeField] private int height = 9;
    public List<GameObject> tetrominoPrefabs;
    [SerializeField] private LayerMask layerMask;

    [SerializeField] private Vector3 scaleTile = new Vector3(0.3f, 0.3f, 1);
    [SerializeField] private float scaleTileTimer = 0.3f;
    [SerializeField] private float timeDestroyTile = 0.3f;

    public List<GameObject> currentTetrominos = new List<GameObject>();
    public List<Tetromino> tetrominoList = new List<Tetromino>();

    private List<Tile> listTiles = new List<Tile>();
    public Transform[,] grid; // Lưới grid lưu trữ thông tin về ô
    private bool gameOver = false;
    private bool gamePause = false;
    private bool checkDestroyed = false;
    public bool CheckDestroyed
    {
        get { return checkDestroyed; }
        set
        {
            checkDestroyed = value;
        }
    }
    public bool GameOver
    {
        get { return gameOver; }
        set
        {
            gameOver = value;
        }
    }
    public bool GamePause
    {
        get { return gamePause; }
        set
        {
            gamePause = value;
        }
    }

    public int Width => width;
    public int Height => height;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        SpawnGridContainAndSpawnPoints();
        // Khởi tạo lưới grid và các thông tin liên quan
        InitializeGrid();
        // Sinh ngẫu nhiên các khối tetromino
        gridContainer.localScale = new Vector3(0.8f, 0.8f, 1);
        board.localScale *= 0.8f;
        board.position *= 0.8f;
        background.transform.position *= 0.8f;
        //spawnPoints.localScale = new Vector3(0.8f, 0.8f, 1);

        UIController.Instance.ShowWindow(WindowType.MainMenu, false);
        UIController.Instance.ShowWindow(WindowType.GamePlay, true);

        if (DataManager.Instance.savedTetrominoList.Count == 0)
        {
            SpawnInitialTetrominos();
        }
        else
        {

            foreach (var item in tetrominoList)
            {
                currentTetrominos.Add(item.gameObject);
            }

        }
    }

    private void SpawnGridContainAndSpawnPoints()
    {
        gridContainer = Instantiate(gridContainPrefab.transform);
        spawnPoints = Instantiate(spawnPointsPrefab.transform);
        board = Instantiate(boardPrefab.transform);
    }
    public void IncreaseScore(int amount)
    {
        DataManager.Instance.Score += amount;
        DataManager.Instance.SaveScore(DataManager.Instance.Score);
        if (DataManager.Instance.Score > DataManager.Instance.HighScore)
        {
            DataManager.Instance.HighScore = DataManager.Instance.Score;
            PlayerPrefs.SetInt("HighScore", DataManager.Instance.HighScore);
        }
        UIController.Instance.gamePlayWindow.GetComponent<GameplayWindow>().UpdateScoreText();
    }
    private void InitializeGrid()
    {
        grid = new Transform[width, height]; // Khởi tạo lưới grid có kích thước width * height

        // Lặp qua tất cả các ô trong lưới grid để khởi tạo các ô
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                // Tạo GameObject từ prefab của ô (tile)
                //GameObject newTile = Instantiate(tilePrefab, new Vector3(x, y, 0), Quaternion.identity);
                GameObject newTile = pool.GetObjectFromPool().gameObject;
                newTile.transform.position = new Vector3(x, y, 0);

                newTile.name = $"Tile {x} {y}";
                newTile.transform.SetParent(gridContainer);
                listTiles.Add(newTile.GetComponent<Tile>());

                // Lưu trữ transform của ô trong lưới grid
                grid[x, y] = newTile.transform;
            }
        }

        ChangeColorTileGrid(0, 3);
        ChangeColorTileGrid(3, 0);
        ChangeColorTileGrid(6, 3);
        ChangeColorTileGrid(3, 6);

        Camera.main.transform.position = new Vector3((float)width / 2 - 0.5f, (float)height / 2 - 2.08f, -10) * 0.8f;
    }
    public void ChangeColorTileGrid(int startX, int startY)
    {
        for (int x = startX; x < startX + 3; x++)
        {
            for (int y = startY; y < startY + 3; y++)
            {
                Transform tile = grid[x, y];
                Color currentColor = tile.GetComponent<SpriteRenderer>().color;
                currentColor.a = 0.8f;
                tile.GetComponent<SpriteRenderer>().color = currentColor;
            }
        }
    }

    public Vector3 CalculateGridPosition(Vector3 position)
    {
        return new Vector3(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y), 0);
    }

    public bool IsGridCellEmpty(Vector3 gridPosition)
    {
        int x = (int)Mathf.Round(gridPosition.x);
        int y = (int)Mathf.Round(gridPosition.y);

        // Kiểm tra xem x và y có nằm trong phạm vi hợp lệ của mảng không
        if (x >= 0 && x < grid.GetLength(0) && y >= 0 && y < grid.GetLength(1))
        {
            if (grid[x, y] != null)
            {
                if (grid[x, y].gameObject.CompareTag("TileShape"))
                {
                    return false;
                }
            }
        }
        else
        {
            return false; // Nếu nằm ngoài phạm vi mảng, coi như ô không trống
        }
        return true;
    }

    public void PlaceTileInGrid(Vector3 gridPosition, Transform tileTransform)
    {
        int x = (int)Mathf.Round(gridPosition.x);
        int y = (int)Mathf.Round(gridPosition.y);

        if (x >= 0 && x < width && y >= 0 && y < height)
        {
            grid[x, y] = tileTransform;

        }
    }

    public void SnapTetrominoToGrid(Transform tetromino)
    {
        foreach (Transform tile in tetromino)
        {
            //Debug.Log(tile.position);
            
           
            var origin = tile.position;
            RaycastHit2D hit = Physics2D.Raycast(origin, transform.forward, 10, layerMask);
            if (hit.collider == null)
            {
                Debug.Log("hi");
            }
            else
            {
                Vector3 gridPosition = CalculateGridPosition(hit.collider.transform.position / 0.8f);
                gridPosition.z = -1;
                PlaceTileInGrid(gridPosition, tile);
                tile.position = hit.collider.transform.position;
                DataManager.Instance.savedTileList.Add(tile.position);
                DataManager.Instance.SaveTile();
            }

            //Debug.Log(tile.position);

            
        }
        //tetromino.localScale *= 0.8f;

    }
    private void SpawnInitialTetrominos()
    {
        List<int> usedIndices = new List<int>(); // Danh sách để theo dõi các prefab đã sử dụng

        // Đảm bảo bạn có ít nhất 3 prefab để chọn
        if (tetrominoPrefabs.Count < 3)
        {
            Debug.LogWarning("Cần ít nhất 3 prefab tetromino.");
            return;
        }

        for (int i = 0; i < 3; i++)
        {
            int randomIndex;

            // Lựa chọn một prefab chưa được sử dụng trước đó
            do
            {
                if (DataManager.Instance.Score < 500)
                {
                    randomIndex = Random.Range(0, tetrominoPrefabs.Count - 5);
                }
                else
                {
                    randomIndex = Random.Range(0, tetrominoPrefabs.Count);
                }
            } while (usedIndices.Contains(randomIndex));

            usedIndices.Add(randomIndex); // Đánh dấu prefab đã được sử dụng

            GameObject tetrominoPrefab = tetrominoPrefabs[randomIndex];
            Transform spawnPosition = spawnPoints.GetChild(i);
            GameObject tetromino = Instantiate(tetrominoPrefab, spawnPosition.position, Quaternion.identity);
            tetromino.transform.SetParent(spawnPosition);
            currentTetrominos.Add(tetromino);

            DataManager.TetrominoData tetrominoData = new DataManager.TetrominoData();
            tetrominoData.id = tetromino.GetComponent<Tetromino>().id;
            tetrominoData.name = tetromino.name;
            tetrominoData.position = tetromino.transform.position;
            DataManager.Instance.savedTetrominoList.Add(tetrominoData);
            DataManager.Instance.SaveTetrominoData();
        }
    }


    public void ChangeTetromino()
    {
        DataManager.Instance.ResetTetrominoData();
        List<int> usedIndices = new List<int>(); // Danh sách để theo dõi các prefab đã sử dụng
        // Xóa Tetromino hiện tại
        foreach (GameObject tetromino in currentTetrominos)
        {
            Destroy(tetromino);
        }
        currentTetrominos.Clear();

        // Lựa chọn ngẫu nhiên 3 Tetromino từ danh sách tất cả Tetromino
        List<GameObject> newTetrominos = new List<GameObject>();
        for (int i = 0; i < 3; i++)
        {
            int randomIndex;

            // Lựa chọn một prefab chưa được sử dụng trước đó
            do
            {
                if (DataManager.Instance.Score < 500)
                {
                    randomIndex = Random.Range(0, tetrominoPrefabs.Count - 5);
                }
                else
                {
                    randomIndex = Random.Range(0, tetrominoPrefabs.Count);
                }
            } while (usedIndices.Contains(randomIndex));

            usedIndices.Add(randomIndex); // Đánh dấu prefab đã được sử dụng

            GameObject tetrominoPrefab = tetrominoPrefabs[randomIndex];
            Transform spawnPosition = spawnPoints.GetChild(i);
            GameObject tetromino = Instantiate(tetrominoPrefab, spawnPosition.position, Quaternion.identity);
            tetromino.transform.SetParent(spawnPosition);
            newTetrominos.Add(tetromino);

            DataManager.TetrominoData tetrominoData = new DataManager.TetrominoData();
            tetrominoData.id = tetromino.GetComponent<Tetromino>().id;
            tetrominoData.name = tetromino.name;
            tetrominoData.position = tetromino.transform.position;
            DataManager.Instance.savedTetrominoList.Add(tetrominoData);
            DataManager.Instance.SaveTetrominoData();
        }
        // Thêm các Tetromino mới vào danh sách hiện tại
        currentTetrominos.AddRange(newTetrominos);

    }
    // Gọi hàm này khi Tetromino đã được drop
    public void TetrominoUsed(GameObject tetromino)
    {
        currentTetrominos.Remove(tetromino);
        DataManager.Instance.savedTetrominoList.RemoveAll(tetrominoData => tetrominoData.name == tetromino.name);
        DataManager.Instance.SaveTetrominoData();
        if (currentTetrominos.Count == 0)
        {
            // Sinh ngẫu nhiễn các tetromino mới
            SpawnInitialTetrominos();

        }
    }

    public bool IsPlaced(GameObject tetromino)
    {
        if (currentTetrominos.Contains(tetromino))
        {
            return false;
        }
        return true;
    }

    public void RotateTetromino(GameObject tetromino)
    {
        // Lấy điểm neo của Tetromino (trong trường hợp này, giả sử điểm neo nằm ở trung tâm Tetromino)
        Vector3 pivot = new Vector3(0, 0, tetromino.transform.position.z);

        // Khởi tạo biến để lưu trữ tile có vị trí x và y nhỏ nhất
        Vector3 minXTile = new Vector3(0, 0, 0);
        Vector3 minYTile = new Vector3(0, 0, 0);



        // Lặp qua từng tile trong Tetromino và tính toán vị trí mới sau khi xoay
        foreach (Transform tile in tetromino.transform)
        {
            // Lấy vị trí ban đầu của tile
            Vector3 oldPosition = tile.localPosition;


            // Tính toán vị trí mới sau khi xoay bằng cách sử dụng công thức xoay
            Vector3 newPosition = new Vector3(
                pivot.x - oldPosition.y + pivot.y,
                pivot.y + oldPosition.x - pivot.x,
                oldPosition.z
            );
            //Debug.Log(newPosition.x);
            //Debug.Log(newPosition.y);



            // Kiểm tra nếu tile hiện tại có tọa độ x hoặc y nhỏ nhất
            if (newPosition.x < minXTile.x)
            {
                minXTile = newPosition;
            }

            if (newPosition.y < minYTile.y)
            {
                minYTile = newPosition;
            }

        }
        //Debug.Log(minXTile.x);
        //Debug.Log(minYTile.y);

        foreach (Transform tile in tetromino.transform)
        {
            // Lấy vị trí ban đầu của tile
            Vector3 oldPosition = tile.localPosition;

            // Tính toán vị trí mới sau khi xoay bằng cách sử dụng công thức xoay
            Vector3 newPosition = new Vector3(
                pivot.x - oldPosition.y + pivot.y,
                pivot.y + oldPosition.x - pivot.x,
                oldPosition.z
            );
            if (minXTile.x < 0)
            {
                newPosition.x += Mathf.Abs(minXTile.x);
            }
            if (minYTile.y < 0)
            {
                newPosition.y += Mathf.Abs(minYTile.y);
            }
            tile.localPosition = newPosition;
        }


    }

    public void CheckAndClearFullRows()
    {
        for (int y = 0; y < height; y++)
        {
            bool isRowFull = true;
            for (int x = 0; x < width; x++)
            {
                Transform tile = grid[x, y];
                if (tile == null || !tile.CompareTag("TileShape"))
                {
                    isRowFull = false;
                    break;
                }
            }
            if (isRowFull)
            {
                float offset = 0;
                //Debug.Log("Full dòng " +y);
                //Xóa dòng
                for (int x = 0; x < width; x++)
                {
                    offset += 0.03f;
                    Transform tile = grid[x, y];
                    if (tile != null && tile.CompareTag("TileShape"))
                    {
                        IncreaseScore(DataManager.Instance.ScorePerBlock);
                        DataManager.Instance.ScoreAmount += DataManager.Instance.ScorePerBlock;
                        tile.DOScale(scaleTile + new Vector3(offset, offset, 0), scaleTileTimer);
                        tile.DORotate(tile.localRotation.eulerAngles + new Vector3(0, 0, -180), scaleTileTimer);
                        Destroy(tile.gameObject, timeDestroyTile);
                        DataManager.Instance.RemoveTile(tile.GetComponent<Tile>());

                    }
                }
                checkDestroyed = true;

            }
        }
    }
    public void CheckAndClearFullColumns()
    {
        for (int x = 0; x < width; x++)
        {
            bool isColumnFull = true;
            for (int y = 0; y < height; y++)
            {
                Transform tile = grid[x, y];
                if (tile == null || !tile.CompareTag("TileShape"))
                {
                    isColumnFull = false;
                    break;
                }
            }
            if (isColumnFull)
            {
                float offset = 0;

                //Debug.Log("Full cột " + x);

                //Xóa cột
                for (int y = 0; y < height; y++)
                {
                    offset += 0.03f;
                    Transform tile = grid[x, y];
                    if (tile != null && tile.CompareTag("TileShape"))
                    {
                        IncreaseScore(DataManager.Instance.ScorePerBlock);
                        DataManager.Instance.ScoreAmount += DataManager.Instance.ScorePerBlock;
                        tile.DOScale(scaleTile + new Vector3(offset, offset, 0), scaleTileTimer);
                        tile.DORotate(tile.localRotation.eulerAngles + new Vector3(0, 0, -180), scaleTileTimer);
                        Destroy(tile.gameObject, timeDestroyTile);
                        DataManager.Instance.RemoveTile(tile.GetComponent<Tile>());
                    }
                }
                checkDestroyed = true;

            }
        }
    }
    public void CheckAndClearSquare(int startX, int startY)
    {
        bool isSquareFull = true;

        for (int x = startX; x < startX + 3; x++)
        {
            for (int y = startY; y < startY + 3; y++)
            {
                Transform tile = grid[x, y];
                if (tile == null || !tile.CompareTag("TileShape"))
                {
                    isSquareFull = false;
                    break;
                }
            }
        }
        if (isSquareFull)
        {
            float offset = 0;
            for (int x = startX; x < startX + 3; x++)
            {
                for (int y = startY; y < startY + 3; y++)
                {
                    offset += 0.03f;
                    Transform tile = grid[x, y];
                    if (tile != null || tile.CompareTag("TileShape"))
                    {
                        IncreaseScore(DataManager.Instance.ScorePerBlock);
                        DataManager.Instance.ScoreAmount += DataManager.Instance.ScorePerBlock;
                        tile.DOScale(scaleTile + new Vector3(offset, offset, 0), scaleTileTimer);
                        tile.DORotate(tile.localRotation.eulerAngles + new Vector3(0, 0, -180), scaleTileTimer);
                        Destroy(tile.gameObject, timeDestroyTile);
                        DataManager.Instance.RemoveTile(tile.GetComponent<Tile>());
                    }
                }
            }
            checkDestroyed = true;

        }
    }
    //public void InvokeGameOVer()
    //{
    //    Invoke("CheckGameOver", 0.5f);
    //}
    public void CheckGameOver()
    {
        bool canAdd = false;

        foreach (GameObject tetromino in currentTetrominos)
        {
            if (tetromino != null)
            {
                for (int i = 0; i < width + 1 - tetromino.GetComponent<Tetromino>().Width; i++)
                {
                    for (int j = 0; j < height + 1 - tetromino.GetComponent<Tetromino>().Height; j++)
                    {
                        bool isEmpty = true;
                        foreach (Transform tile in tetromino.transform)
                        {
                            int x = (int)Mathf.Round(tile.localPosition.x) + i;
                            int y = (int)Mathf.Round(tile.localPosition.y) + j;

                            if ((x >= 0 && x < grid.GetLength(0) && y >= 0 && y < grid.GetLength(1)))
                            {
                                Transform gridCell = grid[x, y];
                                if (gridCell != null && gridCell.CompareTag("TileShape"))
                                {
                                    isEmpty = false;
                                    break;
                                }
                            }
                        }
                        if (isEmpty)
                        {
                            canAdd = true;
                            break;
                        }
                    }
                }
            }
        }
        if (canAdd)
        {
            //Debug.Log("Tiếp tục chơi!");
        }
        else
        {
            gameOver = true;
            UIController.Instance.ShowPopup(PopupType.GameOver, true);

            Debug.Log("Thua! Không thể đặt tetromino nào vào grid.");
        }

    }
    public void ReplayGame()
    {
        SceneManager.LoadScene("Game Play");
    }
    public void LoadMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }


}