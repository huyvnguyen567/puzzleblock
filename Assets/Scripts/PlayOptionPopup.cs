using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using UnityEngine.UI;

public class PlayOptionPopup : MonoBehaviour
{
    [SerializeField] private RectTransform backgroundRect;
    private void OnEnable()
    {
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
    public void OnContinueClick()
    {
        SoundManager.Instance.PlaySfx(SfxType.ButtonClick);
        SceneManager.LoadScene("Game Play");
    }
    public void OnClosePopup()
    {
        SoundManager.Instance.PlaySfx(SfxType.ButtonClick);
        // Đặt một scale nhỏ hơn để thu nhỏ popup
        Vector3 smallScale = new Vector3(0.8f, 0.8f, 0.8f);

        // Sử dụng DOTween để tạo tween để thu nhỏ popup
        backgroundRect.transform.DOScale(smallScale, 0.3f).OnComplete(() => { UIController.Instance.ShowPopup(PopupType.PlayOption, false); });
    }
    public void ActivePopup(bool isActive)
    {
        gameObject.SetActive(isActive);
    }
}
