using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private Color originalColor; // Màu gốc của tile
    [SerializeField] private Color highlightColor; // Màu khi tile được highlight

    private void Start()
    {
        originalColor = GetComponent<SpriteRenderer>().color;
    }

    public void HighlightTile()
    {
        Color newColor = Color.white;
        newColor.a = 0.2f;
        highlightColor = newColor;
        GetComponent<SpriteRenderer>().color = highlightColor;
    }

    public void ResetColor()
    { 
        GetComponent<SpriteRenderer>().color = originalColor;
    }
}
