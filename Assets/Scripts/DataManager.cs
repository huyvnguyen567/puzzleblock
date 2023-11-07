using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;
    [SerializeField] private int score = 0;
    [SerializeField] private int scorePerBlock = 2;
    [SerializeField] private int highScore = 0;
    [SerializeField] private int scoreAmount = 0;
    [SerializeField] private int gold = 500;
    [SerializeField] private int swapQuantity = 0;
    [SerializeField] private int rotateQuantity = 0;
    public List<global::Tetromino> tetrominoPrefab;
    public Tile tilePrefab;
    public List<Vector3> savedTileList = new List<Vector3>();
    public List<TetrominoData> savedTetrominoList = new List<TetrominoData>();

    public int SwapQuantity
    {
        get { return swapQuantity; }
        set
        {
            swapQuantity = value;
        }
    }
    public int RotateQuantity
    {
        get { return rotateQuantity; }
        set
        {
            rotateQuantity = value;
        }
    }
    public int Score
    {
        get { return score; }
        set
        {
            score = value;
        }
    }
    public int ScorePerBlock
    {
        get { return scorePerBlock; }
        set
        {
            scorePerBlock = value;
        }
    }

    public int HighScore
    {
        get { return highScore; }
        set
        {
            highScore = value;
        }
    }
    public int ScoreAmount
    {
        get { return scoreAmount; }
        set
        {
            scoreAmount = value;
        }
    }
    public int Gold
    {
        get { return gold; }
        set
        {
            gold = value;
        }
    }


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    //private void Start()
    //{
    //    highScore = PlayerPrefs.GetInt("HighScore", 0);
    //}
    public void LoadHighScore()
    {
        highScore = PlayerPrefs.GetInt("HighScore", 0);
    }

    public void SaveHighScore()
    {
        PlayerPrefs.SetInt("HighScore", highScore);
        PlayerPrefs.Save();
    }
    public void SaveScore(int score)
    {
        PlayerPrefs.SetInt("score_data", score);
        PlayerPrefs.Save();
    }

    public void ResetScore()
    {
        PlayerPrefs.DeleteKey("score_data");
        PlayerPrefs.Save();
    }
    public void LoadScore()
    {
        score = PlayerPrefs.GetInt("score_data", 0); 
    }

    public void SaveTile()
    {
        string json = JsonHelper.ToJson(savedTileList, true);
        PlayerPrefs.SetString("tile_data", json);
    }

    public void RemoveTile(Tile tile)
    {
        savedTileList.Remove(tile.transform.position);
        SaveTile();
    }

    public void ResetTile()
    {
        if (savedTileList != null)
        {
            savedTileList.Clear();
            SaveTile();
        }
    
    }
    public void LoadTile()
    {
        if (!PlayerPrefs.HasKey("tile_data"))
        {
            SaveTile();
        }
        string loadedJson = PlayerPrefs.GetString("tile_data");
        savedTileList = JsonHelper.FromJson<Vector3>(loadedJson);
        foreach(var tile in savedTileList)
        {
            if(tile != null)
            {
                //Debug.Log("position: " + tile);
                Tile tileObject = Instantiate(tilePrefab, tile, Quaternion.identity);
                GameController.Instance.grid[(int)tile.x, (int)tile.y] = tileObject.transform;
            }
         
        }

    }

    public void SaveGold()
    {
        PlayerPrefs.SetInt("gold", gold);
    }

    public void LoadGold()
    {
        gold = PlayerPrefs.GetInt("gold", 500);
    }

    public void SaveRotateQuantity()
    {
        PlayerPrefs.SetInt("rotate", rotateQuantity);
    }
    public void LoadRotateQuantity()
    {
        rotateQuantity = PlayerPrefs.GetInt("rotate", 5);
    }
    public void SaveSwapQuantity()
    {
        PlayerPrefs.SetInt("swap", swapQuantity);
    }
    public void LoadSwapQuantity()
    {
        swapQuantity = PlayerPrefs.GetInt("swap", 5);
    }

  
    [System.Serializable]
    public class TetrominoData
    {
        public int id;
        public string name;
        public Vector3 position;
    }
    // Hàm để lưu trạng thái TetrominoData vào một tệp JSON
    public void SaveTetrominoData()
    {
        string jsonData = JsonHelper.ToJson(savedTetrominoList,true);
        //System.IO.File.WriteAllText("tetromino_data.json", jsonData);
        PlayerPrefs.SetString("tetromino_data", jsonData);
    }

    public void ResetTetrominoData()
    {
        if (savedTetrominoList != null)
        {
            savedTetrominoList.Clear();
            SaveTetrominoData();
        } 
    }
    // Hàm để tải dữ liệu từ tệp JSON và cập nhật tetrominoDataList
    public void LoadTetrominoData()
{
        if (!PlayerPrefs.HasKey("tetromino_data"))
        {
            SaveTetrominoData();
        }
        string jsonData = PlayerPrefs.GetString("tetromino_data");
        savedTetrominoList = JsonHelper.FromJson<TetrominoData>(jsonData);
        foreach(var tetromino in savedTetrominoList)
        {
            foreach(var item in tetrominoPrefab)
            {
                if (tetromino.id == item.id)
                {
                    Tetromino prefab = Instantiate(item, tetromino.position, Quaternion.identity);
                    GameController.Instance.tetrominoList.Add(prefab);
                }
            }
        }
    }
}
