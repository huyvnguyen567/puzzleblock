using System;
using UnityEngine;
using System.Collections;
#if USE_ADMOB
using GoogleMobileAds;
using GoogleMobileAds.Api;
#endif
#if USE_UNITY_ADS
using UnityEngine.Advertisements;
#endif
using UnityEngine.Events;

public class ManagerAds : MonoBehaviour
{
    public static ManagerAds Ins;

    [Space(2)]
    [Header("Info App")]
    public string ID_MORE;

    public bool isUseAdmob;
    public bool isUseUnityAds;
    public bool isUseAppLovinAds;
    public bool isUseFireBase;
    public bool isStartBanner;

    public AdmobAds admob;
    public UnityAds unityAds;
    //public AppLovinAds appLovinAds;

    public string USE_ADMOB = "USE_ADMOB";
    public string USE_UNITY_ADS = "USE_UNITY_ADS";
    public string USE_FIREBASE = "USE_FIREBASE";
    public string USE_APPLOVIN_ADS = "USE_APPLOVIN_ADS";
    private const string URL_RATE = "https://play.google.com/store/apps/details?id=";
    private const string URL_MORE = "https://play.google.com/store/apps/dev?id=";
    private bool triggerCompleteMethod = false;
    private const string REMOVE_ADS = "RemoveAds";

    private void Awake()
    {
        if (Ins == null)
        {
            Ins = this;
            DontDestroyOnLoad(this);
        }
        else if (Ins != this)
        {
            Destroy(gameObject);
        }
    }

    public void LogEvent(string name)
    {
#if USE_FIREBASE
        Firebase.Analytics.FirebaseAnalytics.LogEvent(name);
#endif
    }

    public void RemoveAds()
    {
        PlayerPrefs.SetInt(REMOVE_ADS, 1);
        HideBanner();
    }

    public bool CanShowAds()
    {
        return PlayerPrefs.GetInt(REMOVE_ADS, 0) == 0;
    }


    public void Start()
    {
        if (isStartBanner)
        {
            ShowBanner();
        }
    }

    public IEnumerator BannerOnStartCoroutine()
    {
        while (true)
        {

#if USE_ADMOB
            if (admob.bannerLoaded)
            {
                ShowBanner();
                break;
            }
#elif USE_APPLOVIN_ADS
            if (appLovinAds.bannerLoaded)
            {
                ShowBanner();
                break;
            }
#endif
            yield return new WaitForSecondsRealtime(1);
        }
    }

#region Share More Rate
    public void RateApp()
    {
        Application.OpenURL(URL_RATE + Application.identifier);
    }

    public void MoreGame()
    {
        Application.OpenURL(URL_MORE + ID_MORE);
    }
    public void ShareApp()
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
            AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");

            intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
            //AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
            //AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("parse", "file://" + imagePath);
            //intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_STREAM"), uriObject);
            //intentObject.Call<AndroidJavaObject>("setType", "image/png");

            intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"),
                URL_RATE + Application.identifier);
            intentObject.Call<AndroidJavaObject>("setType", "text/plain");
            AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");

            AndroidJavaObject jChooser = intentClass.CallStatic<AndroidJavaObject>("createChooser", intentObject, "");
            currentActivity.Call("startActivity", jChooser);
        }

    }
#endregion

#region Ads

    // Returns an ad request with custom ad targeting.
    public void ShowInterstitial()
    {
        if (!CanShowAds())
        {
            return;
        }
        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            return;
        }
#if USE_ADMOB
        if (admob.interstitial.CanShowAd())
        {
            admob.interstitial.Show();
        }
#endif
#if USE_UNITY_ADS
        else if (unityAds.interstitial.CanShowAd())
        {
            unityAds.ShowInterstitial();
        }
#endif
//#if USE_APPLOVIN_ADS
//        else if (appLovinAds.CanShowInterstitial())
//        {
//            appLovinAds.ShowInterstitial();
//        }
//#endif
    }

    public void ShowRewardedVideo(UnityAction<bool> callback)
    {
        triggerCompleteMethod = true;
        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            if (callback != null)
            {
                callback(true);
            }
            return;
        }

#if USE_ADMOB
        else if (admob.IsRewardVideoAvailable())
        {
            admob.rewardBasedVideo.Show((Reward reward) => {
                callback(true);
            });
        }
#endif
#if USE_UNITY_ADS
        else if (unityAds.IsRewardVideoAvailable())
        {
            unityAds.ShowRewardedVideo(callback);
        }
#endif
//#if USE_APPLOVIN_ADS
//        else if(appLovinAds.IsRewardVideoAvailable())
//        {
//            appLovinAds.ShowVideo(callback);
//        }
//#endif
    }

    public void ShowBanner()
    {
        if (!CanShowAds())
        {
            return;
        }

#if USE_ADMOB
        if (admob.bannerLoaded)
        {
            admob.ShowBanner();
        }
#elif USE_APPLOVIN_ADS
        if (appLovinAds.bannerLoaded)
        {
            appLovinAds.ShowBanner();
        }
#endif
    }

    public void HideBanner()
    {
#if USE_ADMOB
        admob.HideBanner();
#elif USE_APPLOVIN_ADS
        appLovinAds.HideBanner();
#endif
    }
#endregion
}
