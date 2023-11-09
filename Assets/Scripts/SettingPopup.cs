using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class SettingPopup : MonoBehaviour
{
    [SerializeField] private RectTransform backgroundRect;
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Slider musicSlider;
    private void OnEnable()
    {
        backgroundRect.localScale = Vector3.one;
        backgroundRect.DOScale(new Vector3(0.8f, 0.8f, 0.8f), 0.2f).From();
        sfxSlider.value = DataManager.Instance.LoadSFXVolume();
        musicSlider.value = DataManager.Instance.LoadMusicVolume();
    }
    private void Start()
    {
        sfxSlider.onValueChanged.AddListener(delegate { SoundManager.Instance.ChangeSoundVolume(sfxSlider.value); });
        musicSlider.onValueChanged.AddListener(delegate { SoundManager.Instance.ChangeMusicVolume(musicSlider.value); });
    }

    public void OnClosePopup()
    {
        SoundManager.Instance.PlaySfx(SfxType.ButtonClick);
        // Đặt một scale nhỏ hơn để thu nhỏ popup
        Vector3 smallScale = new Vector3(0.8f, 0.8f, 0.8f);

        // Sử dụng DOTween để tạo tween để thu nhỏ popup
        backgroundRect.transform.DOScale(smallScale, 0.3f).OnComplete(() => { UIController.Instance.ShowPopup(PopupType.Setting, false); });
    }
    public void ActivePopup(bool isActive)
    {
        gameObject.SetActive(isActive);
    }
}
