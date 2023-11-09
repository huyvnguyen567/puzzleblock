//using System.Collections;
//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine;
//using UnityEngine.SceneManagement;

//public class MainMenuWindow : MonoBehaviour
//{
//    [SerializeField] private RectTransform[] scaleTransforms;

//    private void Awake()
//    {
//        SoundManager.Instance.PlayMusic(MusicType.MainMenu);
//        DataManager.Instance.LoadGold();
//        DataManager.Instance.LoadSwapQuantity();
//        DataManager.Instance.LoadRotateQuantity();
//        DataManager.Instance.LoadScore();
//    }

//    private void Start()
//    {
//        foreach (RectTransform scaleTransform in scaleTransforms)
//        {
//            TweenManagerUI.Instance.OnScaleButton(scaleTransform);
//        }
//    }
//    public void OnStartClick()
//    {
//        SoundManager.Instance.PlaySfx(SfxType.ButtonClick);
//        if (PlayerPrefs.HasKey("tile_data"))
//        {
//            string loadedJson = PlayerPrefs.GetString("tile_data");
//            DataManager.Instance.savedTileList = JsonHelper.FromJson<Vector3>(loadedJson);
//            if (DataManager.Instance.savedTileList.Count == 0)
//            {
//                SceneManager.LoadScene("Game Play");
//            }
//            else
//            {
//                UIController.Instance.ShowPopup(PopupType.PlayOption, true);
//            }
//        }
//        else
//        {
//            SceneManager.LoadScene("Game Play");
//        }

//    }
//    public void OnQuitClick()
//    {
//        SoundManager.Instance.PlaySfx(SfxType.ButtonClick);
//        Debug.Log("Thoat game");
//        Application.Quit();
//    }
//    public  void OnShopClick()
//    {
//        UIController.Instance.ShowPopup(PopupType.Shop, true);
//    }
//    public void ActiveMainMenuWindow(bool isActive)
//    {
//        gameObject.SetActive(isActive);
//    }
//}
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenuWindow : MonoBehaviour
{
    [SerializeField] private RectTransform[] scaleTransforms;
    [SerializeField] private TMP_Text highScoreText;
    [SerializeField] private TMP_Text goldText;

    private void Awake()
    {
        SoundManager.Instance.PlayMusic(MusicType.MainMenu);
        DataManager.Instance.LoadGold();
        DataManager.Instance.LoadSwapQuantity();
        DataManager.Instance.LoadRotateQuantity();
        DataManager.Instance.LoadScore();
        DataManager.Instance.LoadHighScore();
    }

    private void OnEnable()
    {
        highScoreText.text = "High Score: " + DataManager.Instance.HighScore;
        UpdateGoldText();

    }
    private void Start()
    {
        foreach (RectTransform scaleTransform in scaleTransforms)
        {
            TweenManagerUI.Instance.OnScaleButton(scaleTransform);
        }
    }
    private void Update()
    {
        UpdateGoldText();
    }
    public void UpdateGoldText()
    {
        goldText.text = $"{DataManager.Instance.Gold}";
    }
    public void OnStartClick()
    {
        SoundManager.Instance.PlaySfx(SfxType.ButtonClick);
        if (PlayerPrefs.HasKey("tile_data"))
        {
            string loadedJson = PlayerPrefs.GetString("tile_data");
            DataManager.Instance.savedTileList = JsonHelper.FromJson<Vector3>(loadedJson);
            if (DataManager.Instance.savedTileList.Count == 0)
            {

                SceneManager.LoadScene("Game Play");
            }
            else
            {
                UIController.Instance.ShowPopup(PopupType.PlayOption, true);
            }
        }
        else
        {

            SceneManager.LoadScene("Game Play");
        }

    }
    public void OnQuitClick()
    {
        SoundManager.Instance.PlaySfx(SfxType.ButtonClick);
        Debug.Log("Thoat game");
        Application.Quit();
    }
    public void OnShopClick()
    {
        SoundManager.Instance.PlaySfx(SfxType.ButtonClick);
        UIController.Instance.ShowPopup(PopupType.Shop, true);
    }

    public void OnSettingClick()
    {
        SoundManager.Instance.PlaySfx(SfxType.ButtonClick);
        UIController.Instance.ShowPopup(PopupType.Setting, true);
    }
    public void ActiveMainMenuWindow(bool isActive)
    {
        gameObject.SetActive(isActive);
    }
}
