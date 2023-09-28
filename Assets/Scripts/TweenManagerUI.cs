using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class TweenManagerUI : MonoBehaviour
{
    public static TweenManagerUI Instance;
    [SerializeField] private float scaleDuration = 1;
    [SerializeField] private float moveDuration = 3;
    [SerializeField] private float fadeDuration = 3;
    [SerializeField] private float targetAlpha = 0.5f;

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
    public void OnScaleButton(RectTransform recTransform)
    {
        recTransform.DOScale(Vector3.zero, scaleDuration).From();
    }

    public void MoveYPopup(RectTransform rectTransform)
    {
        rectTransform.DOAnchorPosY(0, moveDuration, false);
    }
    public void FadeUI(Image image)
    {
        image.DOFade(targetAlpha, fadeDuration);
    }
 
 
}
