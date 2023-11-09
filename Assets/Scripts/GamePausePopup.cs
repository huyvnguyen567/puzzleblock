using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class GamePausePopup : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private RectTransform backgroundRect;
    private void OnEnable()
    {
        UpdateScoreText();
        backgroundRect.localScale = Vector3.one;
        backgroundRect.DOScale(new Vector3(0.8f, 0.8f, 0.8f), 0.2f).From();
    }
    public void OnReplayClick()
    {
        SoundManager.Instance.PlaySfx(SfxType.ButtonClick);
        DataManager.Instance.ResetScore();
        DataManager.Instance.ResetTile();
        DataManager.Instance.ResetTetrominoData();
        SceneManager.LoadScene("Game Play");
    }
    public void OnMainMenuClick()
    {
        SoundManager.Instance.PlaySfx(SfxType.ButtonClick);
        GameController.Instance.LoadMainMenu();
    }
    public void OnClosePopup()
    {
        SoundManager.Instance.PlaySfx(SfxType.ButtonClick);
        // Đặt một scale nhỏ hơn để thu nhỏ popup
        Vector3 smallScale = new Vector3(0.8f, 0.8f, 0.8f);

        // Sử dụng DOTween để tạo tween để thu nhỏ popup
        backgroundRect.transform.DOScale(smallScale, 0.3f).OnComplete(() => { UIController.Instance.ShowPopup(PopupType.GamePause, false); });
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
