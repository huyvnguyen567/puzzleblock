using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;


public class ShopPopup : MonoBehaviour
{
    [SerializeField] private Text goldText;
    [SerializeField] private Button freeButton;
    [SerializeField] private Color colorFreeButton;
    private UnityAction<bool> x;
    private Vector2 centerPosition;

    private void Awake()
    {
         Canvas canvas = GetComponent<Canvas>();

        if (canvas != null)
        {
            // Lấy vị trí chính giữa của Canvas
            centerPosition = new Vector2(canvas.pixelRect.width * 0.5f, canvas.pixelRect.height * 0.5f);
        }

    }
    void OnEnable()
    {
        UpdateGoldText();
        freeButton.GetComponent<Image>().color = Color.red;
    }

    public void ChangeColorFreeButton()
    {
        freeButton.GetComponent<Image>().color = colorFreeButton;
    }
   
    public void UpdateGoldText()
    {
        //goldText.text = "Gold: " + DataManager.Instance.Gold;
    }
    public void BuyRotateItem(int price)
    {
        if (DataManager.Instance.Gold >= price)
        {
            DataManager.Instance.Gold -= price;
            DataManager.Instance.SaveGold();
            DataManager.Instance.RotateQuantity++;
            DataManager.Instance.SaveRotateQuantity();
            UpdateGoldText();
        }
        else
        {
            Debug.Log("Bạn không đủ vàng");
            TextPopup.Create(centerPosition, "Bạn không đủ vàng");
        }
    }
    public void BuySwapItem(int price)
    {
        if (DataManager.Instance.Gold >= price)
        {
            DataManager.Instance.Gold -= price;
            DataManager.Instance.SaveGold();
            DataManager.Instance.SwapQuantity++;
            DataManager.Instance.SaveSwapQuantity();
            UpdateGoldText();
        }
        else
        {
            Debug.Log("Bạn không đủ vàng");
            TextPopup.Create(centerPosition, "Bạn không đủ vàng");

        }
    }
    public void ActivePopup(bool isActive)
    {
        gameObject.SetActive(isActive);
    }
    public void OnClosePopup()
    {
        UIController.Instance.ShowPopup(PopupType.Shop, false);
    }

    public void ShowAds(string name)
    {
        Debug.Log("show");
        ManagerAds.Ins.ShowRewardedVideo((x) =>
            {
            if (x)
            {
                    switch (name)
                    {
                        case "swap":
                            Debug.Log("swap");
                            DataManager.Instance.SwapQuantity++;
                            DataManager.Instance.SaveSwapQuantity();
                            break;
                        case "rotate":
                            Debug.Log("rotate");
                            DataManager.Instance.RotateQuantity++;
                            DataManager.Instance.SaveRotateQuantity();
                            break;
                        case "gold":
                            Debug.Log("gold");
                            DataManager.Instance.Gold += 100;
                            DataManager.Instance.SaveGold();
                            UpdateGoldText();
                            break;
                    }
            }
        });
       
    }
 
}
