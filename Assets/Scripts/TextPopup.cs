using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TextPopup : MonoBehaviour
{
    private TextMeshPro text;
    private float disappearTimer;
    private Color textColor;

    public static TextPopup Create(Vector3 positon, string text)
    {
        Transform textPopupTransform = Instantiate(UIController.Instance.textPopupPrefab.transform, positon, Quaternion.identity);
        TextPopup textPopup = textPopupTransform.GetComponent<TextPopup>();
        textPopup.Setup(text);
        return textPopup;
    }
    private void Awake()
    {
        text = transform.GetComponent<TextMeshPro>();
    }
    void Update()
    {
        float moveYSpeed = 2f;
        transform.position += new Vector3(0, moveYSpeed) * Time.deltaTime;
        disappearTimer -= Time.deltaTime;
        if (disappearTimer < 0)
        {
            float disappearSpeed = 0.45f;
            textColor.a -= disappearSpeed * Time.deltaTime;
            text.color = textColor;
            if (textColor.a < 0)
            {
                Destroy(gameObject);
            }
        }
    }
    public void Setup(string textInput)
    {
        textColor = text.color;
        text.text = textInput;
    }
}
