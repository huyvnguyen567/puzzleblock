using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.Events;

public class UnityAds : MonoBehaviour, IUnityAdsInitializationListener
{
    public string gameId;

    public string ID_BANNER;
    public string ID_FULL;
    public string ID_REWARD;

    public InterstitialUnity interstitial;
    public RewardedVideoUnity rewardedVideo;

    void Awake()
    {
        InitializeAds();
    }

    public void InitializeAds()
    {
        Advertisement.Initialize(gameId, false, this);
    }

    public void OnInitializationComplete()
    {
        Debug.Log("Unity Ads initialization complete.");
        Request();
    }

    public void OnInitializationFailed(UnityAdsInitializationError error, string message)
    {
        Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
    }

    private void Request()
    {
#if USE_UNITY_ADS
        rewardedVideo = new RewardedVideoUnity();
        interstitial = new InterstitialUnity();
        rewardedVideo.onAdsFailedToShow += () => { StartCoroutine(RequestRewardVideoCoroutine()); };
        interstitial.onAdsFailedToShow += () => { StartCoroutine(RequestInterstitialCoroutine()); };
        RequestRewardVideo();
        RequestInterstitial();
#endif
    }

    #region video
    public void RequestRewardVideo()
    {
        rewardedVideo.LoadAd(ID_REWARD, () => {
            StartCoroutine(RequestRewardVideoCoroutine());
        });
    }

    IEnumerator RequestRewardVideoCoroutine()
    {
        yield return new WaitForSecondsRealtime(5);
        RequestRewardVideo();
    }

    public void ShowRewardedVideo(UnityAction<bool> callback)
    {
        if (IsRewardVideoAvailable())
        {
            this.rewardedVideo.ShowAd(ID_REWARD, callback);
        }
    }

    public bool IsRewardVideoAvailable()
    {
        if (rewardedVideo != null)
        {
            return rewardedVideo.CanShowAd();
        }
        return false;

    }
    #endregion
    #region showfull

    public void RequestInterstitial()
    {
        interstitial.LoadAd(ID_FULL, () => {
            StartCoroutine(RequestInterstitialCoroutine());
        });
    }

    public void ShowInterstitial()
    {
        if (IsInterstitialAvailable())
        {
            interstitial.ShowAd(ID_FULL);
        }
    }

    public bool IsInterstitialAvailable()
    {
        if (interstitial != null)
        {
            return interstitial.CanShowAd();
        }
        return false;

    }

    private IEnumerator RequestInterstitialCoroutine()
    {
        yield return new WaitForSecondsRealtime(5);
        RequestInterstitial();
    }
    #endregion
    public class InterstitialUnity : IUnityAdsLoadListener, IUnityAdsShowListener
    {
        private bool canShow = false;
        private UnityAction onAdsFailedToLoad;
        public UnityAction onAdsFailedToShow;
        public void LoadAd(string adUnitId, UnityAction _onAdsFailedToLoad)
        {
            Debug.Log("Loading Ad: " + adUnitId);
            onAdsFailedToLoad = _onAdsFailedToLoad;
            Advertisement.Load(adUnitId, this);
        }

        public void ShowAd(string adUnitId)
        {
            Debug.Log("Showing Ad: " + adUnitId);
            canShow = false;
            Advertisement.Show(adUnitId, this);
        }

        public bool CanShowAd()
        {
            return canShow;
        }

        public void OnUnityAdsAdLoaded(string adUnitId)
        {
            // Optionally execute code if the Ad Unit successfully loads content.
            canShow = true;
        }

        public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
        {
            Debug.Log($"Error loading Ad Unit: {adUnitId} - {error.ToString()} - {message}");
            // Optionally execute code if the Ad Unit fails to load, such as attempting to try again.
            onAdsFailedToLoad.Invoke();
            canShow = false;
        }

        public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
        {
            Debug.Log($"Error showing Ad Unit {adUnitId}: {error.ToString()} - {message}");
            // Optionally execute code if the Ad Unit fails to show, such as loading another ad.
            canShow = false;
            onAdsFailedToShow.Invoke();
        }

        public void OnUnityAdsShowStart(string adUnitId) { }
        public void OnUnityAdsShowClick(string adUnitId) { }
        public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState) { }
    }

    public class RewardedVideoUnity : IUnityAdsLoadListener, IUnityAdsShowListener
    {
        private bool canShow = false;
        private UnityAction<bool> callback;
        private UnityAction onAdsFailedToLoad;
        public UnityAction onAdsFailedToShow;
        public void LoadAd(string adUnitId, UnityAction _onAdsFailedToLoad)
        {
            // IMPORTANT! Only load content AFTER initialization (in this example, initialization is handled in a different script).
            Debug.Log("Loading Ad: " + adUnitId);
            onAdsFailedToLoad = _onAdsFailedToLoad;
            Advertisement.Load(adUnitId, this);
        }

        public void ShowAd(string adUnitId, UnityAction<bool> _callback)
        {
            Debug.Log("Showing Ad: " + adUnitId);
            callback = _callback;
            Advertisement.Show(adUnitId, this);
        }

        // If the ad successfully loads, add a listener to the button and enable it:
        public void OnUnityAdsAdLoaded(string adUnitId)
        {
            Debug.Log("Ad Loaded: " + adUnitId);
            canShow = true;
        }

        // Implement the Show Listener's OnUnityAdsShowComplete callback method to determine if the user gets a reward:
        public void OnUnityAdsShowComplete(string adUnitId, UnityAdsShowCompletionState showCompletionState)
        {
            Debug.Log("Unity Ads Rewarded Ad Completed");
            canShow = false;
            callback(true);
        }

        // Implement Load and Show Listener error callbacks:
        public void OnUnityAdsFailedToLoad(string adUnitId, UnityAdsLoadError error, string message)
        {
            Debug.Log($"Error loading Ad Unit {adUnitId}: {error.ToString()} - {message}");
            canShow = false;
            onAdsFailedToLoad.Invoke();
            // Use the error details to determine whether to try to load another ad.
        }

        public void OnUnityAdsShowFailure(string adUnitId, UnityAdsShowError error, string message)
        {
            Debug.Log($"Error showing Ad Unit {adUnitId}: {error.ToString()} - {message}");
            // Use the error details to determine whether to try to load another ad.
            canShow = false;
            onAdsFailedToShow();
        }

        public bool CanShowAd()
        {
            return canShow;
        }

        public void OnUnityAdsShowStart(string adUnitId) { }
        public void OnUnityAdsShowClick(string adUnitId) { }
    }

    //public class BannerUnity
    //{
    //    public bool bannerLoaded = false;
    //    public void SetPosition(BannerPosition position)
    //    {
    //        Advertisement.Banner.SetPosition(position);
    //    }

    //    public void RequestBanner(string adUnitId)
    //    {
    //        // Set up options to notify the SDK of load events:
    //        BannerLoadOptions options = new BannerLoadOptions
    //        {
    //            loadCallback = OnBannerLoaded,
    //            errorCallback = OnBannerError
    //        };

    //        // Load the Ad Unit with banner content:
    //        Advertisement.Banner.Load(adUnitId, options);
    //    }

    //    public bool BannerLoaded()
    //    {
    //        return bannerLoaded;
    //    }

    //    // Implement code to execute when the loadCallback event triggers:
    //    void OnBannerLoaded()
    //    {
    //        Debug.Log("Banner loaded");
    //        bannerLoaded = true;
    //    }

    //    // Implement code to execute when the load errorCallback event triggers:
    //    void OnBannerError(string message)
    //    {
    //        Debug.Log($"Banner Error: {message}");
    //        // Optionally execute additional code, such as attempting to load another ad.
    //        bannerLoaded = false;
    //    }

    //    // Implement a method to call when the Show Banner button is clicked:
    //    public void ShowBanner(string adUnitId)
    //    {
    //        // Set up options to notify the SDK of show events:
    //        BannerOptions options = new BannerOptions
    //        {
    //            clickCallback = OnBannerClicked,
    //            hideCallback = OnBannerHidden,
    //            showCallback = OnBannerShown
    //        };

    //        // Show the loaded Banner Ad Unit:
    //        Advertisement.Banner.Show(adUnitId, options);
    //    }

    //    public void HideBanner()
    //    {
    //        Advertisement.Banner.Hide(true);
    //    }

    //    // Implement a method to call when the Hide Banner button is clicked:
    //    void HideBannerAd()
    //    {
    //        // Hide the banner:
    //        Advertisement.Banner.Hide();
    //    }

    //    void OnBannerClicked() { }
    //    void OnBannerShown() { }
    //    void OnBannerHidden() { }
    //}
}
