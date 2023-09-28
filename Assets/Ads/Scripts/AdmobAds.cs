using GoogleMobileAds.Api;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AdmobAds : MonoBehaviour
{
    public string ID_BANNER;
    public string ID_FULL;
    public string ID_REWARD;

    public BannerView bannerView;
    public InterstitialAd interstitial;
    public RewardedAd rewardBasedVideo;

    [HideInInspector]
    public bool bannerLoaded = false;

    private void Start()
    {
#if USE_ADMOB
        RequestRewardVideo();
        RequestBanner();
        RequestInterstitial();
#endif
    }

    #region banner
    public void RequestBanner()
    {
        if (bannerView != null)
        {
            bannerView.Destroy();
        }
        AdSize adSize = AdSize.Banner;
        // Create a 320x50 banner at the top of the screen.

        this.bannerView = new BannerView(ID_BANNER, adSize, AdPosition.Bottom);
        bannerView.OnBannerAdLoaded += BannerLoadSuccess;
        bannerView.OnBannerAdLoadFailed += BannerLoadFailed;
        // Load a banner ad.
        AdRequest request = new AdRequest.Builder().Build();
        this.bannerView.LoadAd(request);

    }

    public void ShowBanner()
    {
        bannerLoaded = false;
        if (bannerView != null)
        {
            bannerLoaded = true;
            bannerView.Show();
        }
        else
        {
            RequestBanner();
        }
    }

    public void HideBanner()
    {
        if (bannerView != null)
        {
            bannerView.Hide();
            RequestBanner();
        }
    }

    private void BannerLoadSuccess()
    {
        bannerLoaded = true;

        if (ManagerAds.Ins.isStartBanner)
        {
            bannerView.Show();
        }
    }

    private void BannerLoadFailed(LoadAdError error)
    {
        Debug.Log("Failed to load banner with error: " + error.GetMessage());
        bannerLoaded = false;
    }
    #endregion

    #region video
    public void RequestRewardVideo()
    {
        AdRequest request = new AdRequest.Builder().AddExtra("npa", "1").AddExtra("is_designed_for_families", "false").Build();
        RewardedAd.Load(ID_REWARD, request, (RewardedAd ad, LoadAdError err) => {
            if (err != null || ad == null)
            {
                Debug.Log("rewarded ad failed to load an ad with error : " + err.GetMessage());
                StartCoroutine(RequestRewardVideoCoroutine());
            }
            else
            {
                rewardBasedVideo = ad;
                rewardBasedVideo.OnAdFullScreenContentClosed += OnVideoClose;
                rewardBasedVideo.OnAdFullScreenContentFailed += OnVideoFailedToShow;
            }
        });
    }

    public void ShowRewardedVideo(UnityAction<bool> callback)
    {
        if (IsRewardVideoAvailable())
        {
            this.rewardBasedVideo.Show((Reward reward) => {
                callback(true);
            });
        }
    }

    public bool IsRewardVideoAvailable()
    {

        if (rewardBasedVideo != null)
        {
            return rewardBasedVideo.CanShowAd();
        }
        return false;

    }

    public void OnVideoClose()
    {
        Debug.Log("Close rewarded video");
        RequestRewardVideo();
    }

    public void OnVideoFailedToShow(AdError error)
    {
        Debug.Log("Failed to show rewarded video with error: " + error.GetMessage());
        RequestRewardVideo();
    }

    public IEnumerator RequestRewardVideoCoroutine()
    {
        yield return new WaitForSecondsRealtime(5);
        RequestRewardVideo();
    }
#endregion

    #region Interstitial
    public void RequestInterstitial()
    {
        if (interstitial != null)
        {
            interstitial.Destroy();
        }

        // Load an interstitial ad.
        AdRequest request = new AdRequest.Builder().Build();

        InterstitialAd.Load(ID_FULL, request, (InterstitialAd ad, LoadAdError err) =>
        {
            if (err != null || ad == null)
            {
                Debug.Log("interstitial ad failed to load an ad with error : " + err.GetMessage());
                StartCoroutine(RequestInterstitialCoroutine());
            }
            else
            {
                interstitial = ad;
                interstitial.OnAdFullScreenContentFailed += HandleInterstitialFailedToShow;
                interstitial.OnAdFullScreenContentClosed += HandleInterstitialClosed;
            }
        });
    }

    public IEnumerator RequestInterstitialCoroutine()
    {
        yield return new WaitForSecondsRealtime(5);
        RequestInterstitial();
    }

    public void HandleInterstitialFailedToShow(AdError error)
    {
        Debug.Log("Failed to show interstitial with error: " + error.GetMessage());
        RequestInterstitial();
    }

    public void HandleInterstitialClosed()
    {
        Debug.Log("Close interstitial");
        RequestInterstitial();
    }
#endregion
}
