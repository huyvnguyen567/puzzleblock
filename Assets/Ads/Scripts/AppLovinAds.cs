//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.Events;

//public class AppLovinAds : MonoBehaviour
//{
//    public string ID_BANNER;
//    public string ID_FULL;
//    public string ID_REWARD;

//    int retryAttempt = 7;

//    private UnityAction<bool> videoCallback;

//    [HideInInspector]
//    public bool bannerLoaded = false;

//    private void Awake()
//    {
//        MaxSdkCallbacks.OnSdkInitializedEvent += (MaxSdkBase.SdkConfiguration sdkConfiguration) => {
//            Debug.Log("AppLovin SDK is initialized, start loading ads");

//            InitAds();
//        };

//        MaxSdk.InitializeSdk();
//    }

//    public void InitAds()
//    {
//        InitializeBannerAds();
//        InitializeInterstitialAds();
//        InitializeRewardedAds();
//        RequestInterstitial();
//        RequestRewardVideo();
//    }

//    static int GetSDKInt()
//    {
//        using (var version = new AndroidJavaClass("android.os.Build$VERSION"))
//        {
//            return version.GetStatic<int>("SDK_INT");
//        }
//    }

//    #region banner
//    public void ShowBanner()
//    {
//        MaxSdk.CreateBanner(ID_BANNER, MaxSdkBase.BannerPosition.BottomCenter);
//    }

//    public void HideBanner()
//    {
//        MaxSdk.HideBanner(ID_BANNER);
//    }

//    public void InitializeBannerAds()
//    {
//        // Banners are automatically sized to 320×50 on phones and 728×90 on tablets
//        // You may call the utility method MaxSdkUtils.isTablet() to help with view sizing adjustments

//        // Set background or background color for banners to be fully functional
//        MaxSdk.SetBannerBackgroundColor(ID_BANNER, new Color(1, 1, 1, 1));

//        MaxSdkCallbacks.Banner.OnAdLoadedEvent += OnBannerAdLoadedEvent;
//        MaxSdkCallbacks.Banner.OnAdLoadFailedEvent += OnBannerAdLoadFailedEvent;
//        MaxSdkCallbacks.Banner.OnAdClickedEvent += OnBannerAdClickedEvent;
//        MaxSdkCallbacks.Banner.OnAdRevenuePaidEvent += OnBannerAdRevenuePaidEvent;
//        MaxSdkCallbacks.Banner.OnAdExpandedEvent += OnBannerAdExpandedEvent;
//        MaxSdkCallbacks.Banner.OnAdCollapsedEvent += OnBannerAdCollapsedEvent;
//    }

//    private void OnBannerAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { bannerLoaded = true; }

//    private void OnBannerAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo) { bannerLoaded = false; }

//    private void OnBannerAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

//    private void OnBannerAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

//    private void OnBannerAdExpandedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

//    private void OnBannerAdCollapsedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }
//    #endregion
//    #region showfull
//    public void ShowInterstitial()
//    {
//        if (GetSDKInt() < 31)
//        {
//            MaxSdk.ShowInterstitial(ID_FULL);
//        }
//    }

//    public bool CanShowInterstitial()
//    {
//        return MaxSdk.IsInterstitialReady(ID_FULL);
//    }

//    public void InitializeInterstitialAds()
//    {
//        // Attach callback
//        MaxSdkCallbacks.Interstitial.OnAdLoadedEvent += OnInterstitialLoadedEvent;
//        MaxSdkCallbacks.Interstitial.OnAdLoadFailedEvent += OnInterstitialLoadFailedEvent;
//        MaxSdkCallbacks.Interstitial.OnAdDisplayedEvent += OnInterstitialDisplayedEvent;
//        MaxSdkCallbacks.Interstitial.OnAdClickedEvent += OnInterstitialClickedEvent;
//        MaxSdkCallbacks.Interstitial.OnAdHiddenEvent += OnInterstitialHiddenEvent;
//        MaxSdkCallbacks.Interstitial.OnAdDisplayFailedEvent += OnInterstitialAdFailedToDisplayEvent;
//    }

//    private void RequestInterstitial()
//    {
//        MaxSdk.LoadInterstitial(ID_FULL);
//    }

//    private IEnumerator RequestInterstitialCoroutine()
//    {
//        yield return new WaitForSecondsRealtime(5);
//        RequestInterstitial();
//    }

//    private void OnInterstitialLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
//    {
//        // Interstitial ad is ready for you to show. MaxSdk.IsInterstitialReady(adUnitId) now returns 'true'

//        // Reset retry attempt
//        retryAttempt = 0;
//    }

//    private void OnInterstitialLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
//    {
//        // Interstitial ad failed to load 
//        // AppLovin recommends that you retry with exponentially higher delays, up to a maximum delay (in this case 64 seconds)

//        retryAttempt++;
//        double retryDelay = Math.Pow(2, Math.Min(6, retryAttempt));

//        Invoke("LoadInterstitial", (float)retryDelay);
//    }

//    private void OnInterstitialDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

//    private void OnInterstitialAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
//    {
//        // Interstitial ad failed to display. AppLovin recommends that you load the next ad.
//        StartCoroutine(RequestInterstitialCoroutine());
//    }

//    private void OnInterstitialClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

//    private void OnInterstitialHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
//    {
//        // Interstitial ad is hidden. Pre-load the next ad.
//        StartCoroutine(RequestInterstitialCoroutine());
//    }
//    #endregion
//    #region video
//    public void ShowVideo(UnityAction<bool> _callback)
//    {
//        if (GetSDKInt() < 31)
//        {
//            videoCallback = _callback;
//            MaxSdk.ShowRewardedAd(ID_REWARD);
//        }
//    }

//    public bool IsRewardVideoAvailable()
//    {
//        return MaxSdk.IsRewardedAdReady(ID_REWARD);
//    }
//    public void InitializeRewardedAds()
//    {
//        // Attach callback
//        MaxSdkCallbacks.Rewarded.OnAdLoadedEvent += OnRewardedAdLoadedEvent;
//        MaxSdkCallbacks.Rewarded.OnAdLoadFailedEvent += OnRewardedAdLoadFailedEvent;
//        MaxSdkCallbacks.Rewarded.OnAdDisplayedEvent += OnRewardedAdDisplayedEvent;
//        MaxSdkCallbacks.Rewarded.OnAdClickedEvent += OnRewardedAdClickedEvent;
//        MaxSdkCallbacks.Rewarded.OnAdRevenuePaidEvent += OnRewardedAdRevenuePaidEvent;
//        MaxSdkCallbacks.Rewarded.OnAdHiddenEvent += OnRewardedAdHiddenEvent;
//        MaxSdkCallbacks.Rewarded.OnAdDisplayFailedEvent += OnRewardedAdFailedToDisplayEvent;
//        MaxSdkCallbacks.Rewarded.OnAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;
//    }

//    public void RequestRewardVideo()
//    {
//        MaxSdk.LoadRewardedAd(ID_REWARD);
//    }

//    IEnumerator RequestRewardVideoCoroutine()
//    {
//        yield return new WaitForSecondsRealtime(5);
//        RequestRewardVideo();
//    }

//    private void OnRewardedAdLoadedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
//    {
//        // Rewarded ad is ready for you to show. MaxSdk.IsRewardedAdReady(adUnitId) now returns 'true'.

//        // Reset retry attempt
//        retryAttempt = 0;
//    }

//    private void OnRewardedAdLoadFailedEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo)
//    {
//        // Rewarded ad failed to load 
//        // AppLovin recommends that you retry with exponentially higher delays, up to a maximum delay (in this case 64 seconds).

//        retryAttempt++;
//        double retryDelay = Math.Pow(2, Math.Min(6, retryAttempt));

//        Invoke("LoadRewardedAd", (float)retryDelay);
//    }

//    private void OnRewardedAdDisplayedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

//    private void OnRewardedAdFailedToDisplayEvent(string adUnitId, MaxSdkBase.ErrorInfo errorInfo, MaxSdkBase.AdInfo adInfo)
//    {
//        // Rewarded ad failed to display. AppLovin recommends that you load the next ad.
//        StartCoroutine(RequestRewardVideoCoroutine());
//    }

//    private void OnRewardedAdClickedEvent(string adUnitId, MaxSdkBase.AdInfo adInfo) { }

//    private void OnRewardedAdHiddenEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
//    {
//        // Rewarded ad is hidden. Pre-load the next ad
//        RequestRewardVideo();
//    }

//    private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward, MaxSdkBase.AdInfo adInfo)
//    {
//        // The rewarded ad displayed and the user should receive the reward.
//        if(adUnitId == ID_REWARD)
//        {
//            videoCallback(true);
//            RequestRewardVideo();
//        }
//    }

//    private void OnRewardedAdRevenuePaidEvent(string adUnitId, MaxSdkBase.AdInfo adInfo)
//    {
//        // Ad revenue paid. Use this callback to track user revenue.
//    }
//    #endregion
//}
