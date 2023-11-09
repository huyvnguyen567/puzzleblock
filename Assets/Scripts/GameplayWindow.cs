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
    [SerializeField] private TMP_Text changeCountText;
    [SerializeField] private Button btnPause;
    [SerializeField] private Slider rewardProcessSlider;
    //[SerializeField] private Text rewardText;
    [SerializeField] private Image starImage;
    [SerializeField] private Sprite swapSprite;
    [SerializeField] private Sprite rotateSprite;
    [SerializeField] private Sprite goldSprite;
    [SerializeField] private Sprite starSprite;

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
    void ChangeSpriteWithDotween(Sprite newSprite)
    {
        // Sử dụng DOTween để thay đổi sprite
        starImage.DOFade(0f, 0.5f) // Phai hoàn toàn hình ảnh hiện tại
             .OnComplete(() =>
             {
                 starImage.sprite = newSprite; // Thay đổi sprite thành sprite mới
                 starImage.rectTransform.sizeDelta = new Vector2(110, 110);
                 starImage.DOFade(1f, 0.5f); // Hiển thị hình ảnh mới
             });
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
        scoreText.text = $"{DataManager.Instance.Score}";
        highScoreText.text = $"{DataManager.Instance.HighScore}";
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
            SoundManager.Instance.PlaySfx(SfxType.Complete);
            PlayerPrefs.SetFloat("ProcessReward", 0);
            int randomReward = Random.Range(0, 3);
            HandleReward(randomReward);
            StartCoroutine(ResetRewardProcess());
        }
    }
    public void OnClickPause()
    {
        SoundManager.Instance.PlaySfx(SfxType.ButtonClick);
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
                //rewardText.text = "+ 1 Lượt Xoay";
                ChangeSpriteWithDotween(rotateSprite);
                DataManager.Instance.RotateQuantity++;
                DataManager.Instance.SaveRotateQuantity();
                var toggle = FindObjectOfType<Toggle>().GetComponent<SwitchToggle>();
                toggle.UpdateRotateCount();
                break;
            case 1:
                //rewardText.text = "+ 1 Lượt Đổi";
                ChangeSpriteWithDotween(swapSprite);
                DataManager.Instance.SwapQuantity++;
                DataManager.Instance.SaveSwapQuantity();
                UpdateChangeCount();
                break;
            case 2:
                //rewardText.text = "+ 100 Vàng";
                ChangeSpriteWithDotween(goldSprite);
                DataManager.Instance.Gold += 100;
                DataManager.Instance.SaveGold();
                break;
        }
    }
    private IEnumerator ResetRewardProcess()
    {
        yield return new WaitForSeconds(3);
        rewardProcessSlider.DOValue(0, 1);
        starImage.sprite = starSprite;
        starImage.SetNativeSize();
        //rewardText.text = "Quà";
    }
}
