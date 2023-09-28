using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class GameOverPopup : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    public Image blurImage;
    public RectTransform popup;

    private void OnEnable()
    {
        UpdateScoreText();
        DataManager.Instance.ResetScore();
        DataManager.Instance.ResetTile();
        DataManager.Instance.ResetTetrominoData();
    }
    public void OnReplayClick()
    {
        DataManager.Instance.ResetScore();
        DataManager.Instance.ResetTile();
        DataManager.Instance.ResetTetrominoData();
        GameController.Instance.ReplayGame();
    }
    public void OnMainMenuClick()
    {
        DataManager.Instance.ResetScore();
        DataManager.Instance.ResetTile();
        DataManager.Instance.ResetTetrominoData();
        GameController.Instance.LoadMainMenu();
    }

    public void ActiveGameOverPopup(bool isActive)
    {
        gameObject.SetActive(isActive);
        if (isActive)
        {
            SoundManager.Instance.PlaySfx(SfxType.GameOver);
            TweenManagerUI.Instance.MoveYPopup(popup);
            TweenManagerUI.Instance.FadeUI(blurImage);
        }
            
    }
    public void UpdateScoreText()
    {
        scoreText.text = "Score: " + DataManager.Instance.Score;
    }

}
