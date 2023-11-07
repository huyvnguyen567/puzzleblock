using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class Tetromino : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private Vector3 offset = new Vector3(0, 3, 0);
    private Vector3 initialPosition;
    private int height;
    private int width;
    public int id;
    public int Height => height;
    public int Width => width;

    private float offsetScreen_x = 0.5f;
    private float offsetScreen_y = 2;

    private Camera mainCamera;
    private float xMax, xMin, yMin, yMax;
    Toggle toggle;
    private Vector3[] initialTilePositions;
    private bool rotated = false;

    private bool canRotate = true; 
    private void Awake()
    {
        initialPosition = transform.position;
       

        toggle = FindObjectOfType<Toggle>();

    }

    private void Start()
    {
        mainCamera = Camera.main;
        CalculateCameraBounds();
        initialTilePositions = new Vector3[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            initialTilePositions[i] = transform.GetChild(i).position;
        }

    }
    private void Update()
    {
        int maxX = int.MinValue;
        int maxY = int.MinValue;

        foreach (Transform tile in transform)
        {
            int x = Mathf.RoundToInt(tile.localPosition.x);
            int y = Mathf.RoundToInt(tile.localPosition.y);

            if (x > maxX)
            {
                maxX = x;
            }

            if (y > maxY)
            {
                maxY = y;
            }
        }

        width = maxX + 1;
        height = maxY + 1;
        // Lấy vị trí hiện tại của Tetromino
        Vector3 currentPosition = transform.position;

        // Kiểm tra giới hạn và ngăn Tetromino ra khỏi giới hạn
        currentPosition.x = Mathf.Clamp(currentPosition.x, xMin + mainCamera.transform.position.x, xMax + mainCamera.transform.position.x - offsetScreen_x);
        currentPosition.y = Mathf.Clamp(currentPosition.y, yMin + mainCamera.transform.position.y, yMax + mainCamera.transform.position.x - offsetScreen_y);

        // Cập nhật vị trí của Tetromino
        transform.position = currentPosition;
        if (DataManager.Instance.RotateQuantity <= 0)
        {
            toggle.isOn = false;
        }

    }
    private void CalculateCameraBounds()
    {
        float orthographicSize = mainCamera.orthographicSize;
        float aspect = mainCamera.aspect;

        // Tính toán giới hạn của Tetromino
        xMax = orthographicSize * aspect;
        xMin = -xMax;
        yMin = -orthographicSize;
        yMax = -yMin;
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        if (!GameController.Instance.IsPlaced(transform.gameObject) && !GameController.Instance.GameOver && !GameController.Instance.GamePause)
        {
            var target = Camera.main.ScreenToWorldPoint(eventData.position);
            target.z = -1;
            if (toggle.isOn)
            {
                //Khong lam gi
            }
            else
            {
                target += offset;
                transform.position = target;
                transform.localScale = new Vector3(1, 1, 1);
            }

            DataManager.Instance.ScoreAmount = 0;
            SoundManager.Instance.PlaySfx(SfxType.TetrominoClick);
        }

    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!GameController.Instance.IsPlaced(transform.gameObject) && !GameController.Instance.GameOver && !GameController.Instance.GamePause)
        {
            canRotate = false;
            var target = Camera.main.ScreenToWorldPoint(eventData.position);
            target += offset;
            transform.localScale = new Vector3(1, 1, 1);
            target.z = -2;
            transform.position = target;
            HighLightColor();
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!GameController.Instance.IsPlaced(transform.gameObject))
        {
            if (gameObject.CompareTag("Square"))
            {
                canRotate = false;
            }
            if (toggle.isOn && canRotate && DataManager.Instance.RotateQuantity > 0) 
            {
                GameController.Instance.RotateTetromino(gameObject);
                rotated = true;         
            }
            else
            {
                canRotate = true;
            }
        }
        
        foreach (Transform tile in GameController.Instance.gridContainer)
        {
            tile.GetComponent<Tile>().ResetColor();
        }
        if (!GameController.Instance.GameOver && !GameController.Instance.GamePause) 
        {
            //Kiểm tra và đặt Tetromino vào lưới grid
            StartCoroutine(SnapAndCheckGameOver());
        }
        else
        {
            transform.localScale = new Vector3(0.4f, 0.4f, 1);
            transform.position = initialPosition;
        }
       
    }

    public void HighLightColor()
    {
        
        if (CanSnap())
        {
            foreach (Transform tile in transform)
            {
                var origin = tile.position;
                RaycastHit2D hit = Physics2D.Raycast(origin, transform.forward, 10, layerMask);
                if (hit.collider == null || !GameController.Instance.IsGridCellEmpty(tile.position))
                {
                    
                }
                else
                {
                    hit.collider.GetComponent<Tile>().HighlightTile();
                }
            }
        }
        else
        {
            foreach (Transform tile in GameController.Instance.gridContainer)
            {
                tile.GetComponent<Tile>().ResetColor();
            }
        }
    }

    public bool CanSnap()
    {
        foreach (Transform tile in transform)
        {
            var origin = tile.position;
            RaycastHit2D hit = Physics2D.Raycast(origin, transform.forward, 10, layerMask);
            if (hit.collider == null || !GameController.Instance.IsGridCellEmpty(tile.position))
            {
                return false;
            }
        }
        return true;
    }
    private IEnumerator SnapAndCheckGameOver()
    {
        SnapToValidGridCell();
        yield return new WaitForSeconds(0.1f);
        
        if (rotated && !GameController.Instance.currentTetrominos.Contains(gameObject))
        {
            if (DataManager.Instance.RotateQuantity > 0)
            {
                DataManager.Instance.RotateQuantity--;
                DataManager.Instance.SaveRotateQuantity();
                toggle.GetComponent<SwitchToggle>().UpdateRotateCount();
            }

        }
        // Chờ cho tất cả các hàm khác trong SnapToValidGridCell hoàn thành
        yield return new WaitForSeconds(1f);
      
        GameController.Instance.CheckGameOver();
    }
    private void SnapToValidGridCell()
    {  
        if (CanSnap())
        {
            SoundManager.Instance.PlaySfx(SfxType.OnBoard); 
            GameController.Instance.IncreaseScore(transform.childCount);
            GameController.Instance.SnapTetrominoToGrid(transform);
            GameController.Instance.TetrominoUsed(transform.gameObject);
  

            GameController.Instance.CheckAndClearFullColumns();
            GameController.Instance.CheckAndClearFullRows();
            for (int x = 0; x < GameController.Instance.Width; x += 3)
            {
                for (int y = 0; y < GameController.Instance.Height; y += 3)
                {
                    GameController.Instance.CheckAndClearSquare(x, y);
                }
            }
            DataManager.Instance.ScoreAmount += transform.childCount;
            UIController.Instance.gamePlayWindow.GetComponent<GameplayWindow>().UpdateRewardProcess(DataManager.Instance.ScoreAmount);

            if (GameController.Instance.CheckDestroyed)
            {
                ScorePopup.Create(transform.position, DataManager.Instance.ScoreAmount);
                GameController.Instance.CheckDestroyed = false;
            }
        }
        else
        {
            if (!GameController.Instance.IsPlaced(transform.gameObject))
            {
                transform.localScale = new Vector3(0.4f, 0.4f, 1);
                transform.position = initialPosition;
            }
        } 
    }

    public void ResetTilePosition()
    {
        rotated = false;
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).position = initialTilePositions[i];
        }
    }
}
