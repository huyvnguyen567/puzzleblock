using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class GameplayWindow : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI highScoreText;
    [SerializeField] private Text changeCountText;
    [SerializeField] private Button btnPause;
    [SerializeField] private Slider rewardProcessSlider;
    [SerializeField] private Text rewardText;
    private void Awake()
    {
        SoundManager.Instance.PlayMusic(MusicType.GamePlay);
        UpdateChangeCount();
    }
    private void OnEnable()
    {
        UpdateScoreText();
        
        DataManager.Instance.LoadTile();
        DataManager.Instance.LoadTetrominoData();
        rewardProcessSlider.value = PlayerPrefs.GetFloat("ProcessReward", 0);

    }
    private void Update()
    {

    }
    public void ActiveGamePlayWindow(bool isActive)
    {
        gameObject.SetActive(isActive);
    }
    public void UpdateScoreText()
    {
        DataManager.Instance.LoadScore();
        //// Sử dụng DOTween để tạo hiệu ứng tweening cho giá trị Text
        //DOTween.To(() => DataManager.Instance.Score, x => {
        //    DataManager.Instance.Score = x;
        //    scoreText.text = "Score: " + DataManager.Instance.Score;
        //}, DataManager.Instance.Score, 0.5f);
        scoreText.text = "Score: " + DataManager.Instance.Score;
        highScoreText.text = "High Score: " +DataManager.Instance.HighScore;
    }

    public void UpdateChangeCount()
    {
        changeCountText.text = DataManager.Instance.SwapQuantity.ToString();
    }

    public void UpdateRewardProcess(int scoreIncrease)
    {
        float targetValue = Mathf.Clamp01(rewardProcessSlider.value + ((float)scoreIncrease / 500));
        rewardProcessSlider.DOValue(targetValue, 0.7f);
        
        PlayerPrefs.SetFloat("ProcessReward", rewardProcessSlider.value);
    
        if (targetValue >= 1)
        {
            PlayerPrefs.SetFloat("ProcessReward", 0);
            int randomReward = Random.Range(0, 3);
            HandleReward(randomReward);
            StartCoroutine(ResetRewardProcess());
        }
    }
    public void OnClickPause()
    {
        if (!GameController.Instance.GameOver)
        {
            UIController.Instance.ShowPopup(PopupType.GamePause, true);
            GameController.Instance.GamePause = true;
        }
    }
    public void OnClickChangeTetromino()
    {
        if(DataManager.Instance.SwapQuantity > 0)
        {
            DataManager.Instance.SwapQuantity--;
            DataManager.Instance.SaveSwapQuantity();
            UpdateChangeCount();
            StartCoroutine(ChangeAndCheckGameOver());
        }
        else
        {
            Debug.Log("Bạn hết lượt đổi");
        }
    }
    private IEnumerator ChangeAndCheckGameOver()
    {
        GameController.Instance.ChangeTetromino();

        yield return new WaitForSeconds(1f);
        GameController.Instance.CheckGameOver();

    }
    private void HandleReward(int rewardType)
    {

        switch (rewardType)
        {
            case 0:
                rewardText.text = "+ 1 Lượt Xoay";
                DataManager.Instance.RotateQuantity++;
                DataManager.Instance.SaveRotateQuantity();
                var toggle = FindObjectOfType<Toggle>().GetComponent<SwitchToggle>();
                toggle.UpdateRotateCount();
                break;
            case 1:
                rewardText.text = "+ 1 Lượt Đổi";
                DataManager.Instance.SwapQuantity++;
                DataManager.Instance.SaveSwapQuantity();
                UpdateChangeCount();
                break;
            case 2:
                rewardText.text = "+ 100 Vàng";
                DataManager.Instance.Gold += 100;
                DataManager.Instance.SaveGold();
                break;
        }
    }
    private IEnumerator ResetRewardProcess()
    {
        yield return new WaitForSeconds(3);
        rewardProcessSlider.DOValue(0, 1);
        rewardText.text = "Quà";
    }
}
