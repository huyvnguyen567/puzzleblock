using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScorePopup : MonoBehaviour
{
    private TextMeshPro textMesh;
    private float disappearTimer;
    private Color textColor;

    public static ScorePopup Create(Vector3 position, int scoreAmount)
    {
        Transform scorePopupTransform = Instantiate(UIController.Instance.scorePopupPrefab.transform, position, Quaternion.identity);
        ScorePopup scorePopup = scorePopupTransform.GetComponent<ScorePopup>();
        scorePopup.Setup(scoreAmount);
        return scorePopup;
    }
    private void Awake()
    {
        textMesh = transform.GetComponent<TextMeshPro>();
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
            textMesh.color = textColor;
            if (textColor.a < 0)
            {
                Destroy(gameObject);
            }
        }
    }
    public void Setup(int scoreAmount)
    {
        textColor = textMesh.color;
        if (scoreAmount > 54)
        {
            textMesh.text = "AMAZING!" + "\n+" + scoreAmount;
        }
        else if (scoreAmount > 36)
        {
            textMesh.text = "GREAT!" + "\n+" + scoreAmount;
        }
        else
        {
            textMesh.text = "+" + scoreAmount;
        }
    }
  
}
