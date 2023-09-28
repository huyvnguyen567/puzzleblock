using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GamePausePopup : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;

    private void OnEnable()
    {
        UpdateScoreText();
    }
    public void OnReplayClick()
    {
        DataManager.Instance.ResetScore();
        DataManager.Instance.ResetTile();
        DataManager.Instance.ResetTetrominoData();
        SceneManager.LoadScene("Game Play");
    }
    public void OnMainMenuClick()
    {
        Debug.Log(11);
        GameController.Instance.LoadMainMenu();
    }
    public void OnClosePopup()
    {
        UIController.Instance.ShowPopup(PopupType.GamePause, false);
        GameController.Instance.GamePause = false;
    }
    public void ActiveGamePausePopup(bool isActive)
    {
        gameObject.SetActive(isActive);
    }
    public void UpdateScoreText()
    {
        scoreText.text = "Score: " + DataManager.Instance.Score;
    }
}
