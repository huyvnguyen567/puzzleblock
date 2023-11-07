using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

public class SwitchToggle : MonoBehaviour
{
    [SerializeField]  RectTransform uiHandleRectTransform;
    [SerializeField] Color backgroundActiveColor;
    [SerializeField] Color handleActiveColor;
    public TMP_Text rotateCountText;

    Image backgroundImage, handleImage;
    Color backgrounDefaultColor, handleDefaultColor;
    Toggle toggle;
    Vector2 handlePosition;
    void Awake()
    {
        toggle = GetComponent<Toggle>();

        handlePosition = uiHandleRectTransform.anchoredPosition;

        backgroundImage = uiHandleRectTransform.parent.GetComponent<Image>();
        handleImage = uiHandleRectTransform.GetComponent<Image>();

        backgrounDefaultColor = backgroundImage.color;
        handleDefaultColor = handleImage.color;

        toggle.onValueChanged.AddListener(OnSwitch);
        if (toggle.isOn)
        {
            OnSwitch(true);
        }
        UpdateRotateCount();
    }
   
    void OnSwitch(bool on)
    {
        //uiHandleRectTransform.anchoredPosition = on ? handlePosition * -1 : handlePosition;
        uiHandleRectTransform.DOAnchorPos(on ? handlePosition * -1 : handlePosition, .4f).SetEase(Ease.InOutBack);
        //backgroundImage.color = on ? backgroundActiveColor : backgrounDefaultColor;
        backgroundImage.DOColor(on ? backgroundActiveColor : backgrounDefaultColor, .6f);
        //handleImage.color = on ? handleActiveColor : handleDefaultColor;
        handleImage.DOColor(on ? handleActiveColor : handleDefaultColor, .6f);
    }
    public void ResetTile()
    {
        if (!toggle.isOn)
        {
            foreach(var tetromino in GameController.Instance.currentTetrominos)
            {
                tetromino.GetComponent<Tetromino>().ResetTilePosition();
            }
        }
    }

    public void UpdateRotateCount()
    {
        rotateCountText.text = DataManager.Instance.RotateQuantity.ToString();
    }
    void OnDestroy()
    {
        toggle.onValueChanged.RemoveListener(OnSwitch);
    }
}
