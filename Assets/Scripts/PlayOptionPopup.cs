using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayOptionPopup : MonoBehaviour
{

   
    public void OnReplayClick()
    {
        
        DataManager.Instance.ResetScore();
        DataManager.Instance.ResetTile();
        DataManager.Instance.ResetTetrominoData();
        SceneManager.LoadScene("Game Play");

    }
    public void OnContinueClick()
    {
        SceneManager.LoadScene("Game Play");
    }
    public void OnClosePopup()
    {
        UIController.Instance.ShowPopup(PopupType.PlayOption, false);
    }
    public void ActivePopup(bool isActive)
    {
        gameObject.SetActive(isActive);
    }
}
