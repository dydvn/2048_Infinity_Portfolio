using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using GoogleMobileAds.Api;

public class Manager_Admob : MonoBehaviour
{
    static private Manager_Admob instance;

    private BannerView bannerView;
    private InterstitialAd interstitialAd;
    private RewardedAd rewardedAd;
    private AdRequest adRequest;
    private string bannerAdsKey;
    private string interstitialAdsKey;
    private string rewardedAdsKey;

    public static Manager_Admob Instance
    {
        get
        {
            if (instance == null)
                return null;

            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    void Start()
    {
#if UNITY_EDITOR
        //Test ID
        Debug.Log("테스트 아이디 사용");
        bannerAdsKey = "ca-app-pub-****************/**********";
        interstitialAdsKey = "ca-app-pub-****************/**********";
        rewardedAdsKey = "ca-app-pub-****************/**********";
#elif UNITY_ANDROID
        // 내부 테스트용 테스트 아이디 사용
        // bannerAdsKey = "ca-app-pub-****************/**********";
        // interstitialAdsKey = "ca-app-pub-****************/**********";
        // rewardedAdsKey = "ca-app-pub-****************/**********";

        // 아래가 찐 아이디
        bannerAdsKey = "ca-app-pub-****************/**********";
        interstitialAdsKey = "ca-app-pub-****************/**********";
        rewardedAdsKey = "ca-app-pub-****************/**********";
#elif UNITY_IPHONE

#else
#endif



        //광고 초기화
        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize((InitializationStatus initStatus) =>
        {
            // This callback is called once the MobileAds SDK is initialized.
        });

        adRequest = new AdRequest();

        LoadBannerAd();
        LoadInterstitialAd();
        LoadRewardedAd();
    }


    // Banner

    public void LoadBannerAd()
    {
        // create an instance of a banner view first.
        if (bannerView == null)
        {
            CreateBannerView();
        }
        // create our request used to load the ad.

        // send the request to load the ad.
        Debug.Log("Loading banner ad.");
        bannerView.LoadAd(adRequest);
    }
    public void CreateBannerView()
    {
        Debug.Log("Creating banner view");

        // If we already have a banner, destroy the old one.
        if (bannerView != null)
        {
            DestroyAd();
        }

        // Create a 320x50 banner at top of the screen
        bannerView = new BannerView(bannerAdsKey, AdSize.Banner, AdPosition.Top);
    }

    private void ListenToAdEvents()
    {
        bannerView.OnBannerAdLoaded += () =>
        {
            Debug.Log("Banner view loaded an ad with response : "
                + bannerView.GetResponseInfo());
        };
        bannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
        {
            Debug.LogError("Banner view failed to load an ad with error : "
                + error);
        };
        bannerView.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(string.Format("Banner view paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        bannerView.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Banner view recorded an impression.");
        };
        bannerView.OnAdClicked += () =>
        {
            Debug.Log("Banner view was clicked.");
        };
        bannerView.OnAdFullScreenContentOpened += (null);
        {
            Debug.Log("Banner view full screen content opened.");
        };
        bannerView.OnAdFullScreenContentClosed += (null);
        {
            Debug.Log("Banner view full screen content closed.");
        };
    }

    public void DestroyAd()
    {
        if (bannerView != null)
        {
            bannerView.Destroy();
            bannerView = null;
        }
    }




    // front

    public void LoadInterstitialAd() //광고 로드
    {
        if (interstitialAd != null)
        {
            interstitialAd.Destroy();
            interstitialAd = null;
        }

        InterstitialAd.Load(interstitialAdsKey, adRequest,
            (InterstitialAd ad, LoadAdError error) =>
            {
                if (error != null || ad == null)
                {
                    Debug.LogError("interstitial ad failed to load an ad " +
                                   "with error : " + error);
                    return;
                }

                Debug.Log("Interstitial ad loaded with response : "
                          + ad.GetResponseInfo());

                interstitialAd = ad;
            });
        RegisterEventHandlers(interstitialAd); //이벤트 등록
    }

    public void ShowInterstitialAd() //광고 보기
    {
        if (CanShowInterstitialAd())
        {
            interstitialAd.Show();

            // RegisterReloadHandler(interstitialAd);
        }
        else
        {
            Debug.LogError("Interstitial ad is not ready yet.");
        }
    }

    private void RegisterEventHandlers(InterstitialAd ad) //광고 이벤트
    {
        ad.OnAdPaid += (AdValue adValue) =>
        {
    
            //보상 주기
        };
        ad.OnAdImpressionRecorded += () =>
        {
        };
        ad.OnAdClicked += () =>
        {
        };
        ad.OnAdFullScreenContentOpened += () =>
        {
        };
        ad.OnAdFullScreenContentClosed += () =>
        {
            LoadInterstitialAd(); //닫기 버튼 누를때 광고 재로드
        };
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Interstitial ad failed to open full screen content " +
                           "with error : " + error);
            LoadInterstitialAd();
        };
    }

    private void RegisterReloadHandler(InterstitialAd ad) //광고 재로드
    {
        ad.OnAdFullScreenContentClosed += (null);
        {
            Debug.Log("Interstitial Ad full screen content closed.");
    
            LoadInterstitialAd();
        };
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Interstitial ad failed to open full screen content " +
                           "with error : " + error);
    
            LoadInterstitialAd();
        };
    }




    // Reward

    public void LoadRewardedAd() //광고 로드 하기
    {
        // Clean up the old ad before loading a new one.
        if (rewardedAd != null)
        {
            rewardedAd.Destroy();
            rewardedAd = null;
        }

        // create our request used to load the ad.

        // send the request to load the ad.
        RewardedAd.Load(rewardedAdsKey, adRequest,
            (RewardedAd ad, LoadAdError error) =>
            {
                // if error is not null, the load request failed.
                if (error != null || ad == null)
                {
                    Debug.LogError("Rewarded ad failed to load an ad " +
                                   "with error : " + error);
                    LoadRewardedAd();
                    return;
                }

                Debug.Log("Rewarded ad loaded with response : "
                          + ad.GetResponseInfo());

                rewardedAd = ad;
            });
    }

    public void ShowRewardedAd(Func<bool> rewardFunc) //광고 보기
    {
        // const string rewardMsg =
        //     "Rewarded ad rewarded the user. Type: {0}, amount: {1}.";

        if (CanShowRewardedAd())
        {
            rewardedAd.Show((Reward reward) =>
            {
                //보상 획득하기
                rewardFunc();
            });
        }
        else
        {
            LoadRewardedAd();
        }    
    }

    private void RegisterReloadHandler(RewardedAd ad) //광고 재로드
    {
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += (null);
        {
            Debug.Log("Rewarded Ad full screen content closed.");

            // Reload the ad so that we can show another as soon as possible.
            LoadRewardedAd();
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Rewarded ad failed to open full screen content " +
                           "with error : " + error);

            // Reload the ad so that we can show another as soon as possible.
            LoadRewardedAd();
        };
    }

    // custom Func
    public bool CanShowInterstitialAd()
    {
        return interstitialAd != null && interstitialAd.CanShowAd();
    }


    public bool CanShowRewardedAd()
    {
        return rewardedAd != null && rewardedAd.CanShowAd();
    }
}
