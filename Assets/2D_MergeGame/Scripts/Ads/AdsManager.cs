using GoogleMobileAds.Api;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AdsManager : MonoBehaviour
{
    public static AdsManager instance;

    BannerView _bannerView;
    private string _adBannerUnitId = "ca-app-pub-6878793099358997/6003891940";

    private InterstitialAd _interstitialAd;
    private string _adInterstitialUnitId = "ca-app-pub-6878793099358997/7309767588";

    private RewardedAd _rewardedAd;
    private string _adRewardUnitId = "ca-app-pub-6878793099358997/3191393328";

    private bool isBannerHide;

    // Günlük limit ile ilgili deðiþkenler
    private const string RewardAdLimitKey = "RewardAdDailyLimit";
    private const string LastRewardAdDateKey = "LastRewardAdDate";
    private const int DailyRewardAdLimit = 5;
    private int currentRewardAdCount;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        MobileAds.Initialize((InitializationStatus initStatus) =>
        {
        });

        LoadInterstitialAd();
        LoadRewardedAd();

        LoadRewardAdLimit();
    }

    private void LoadRewardAdLimit()
    {
        string lastDate = PlayerPrefs.GetString(LastRewardAdDateKey, "");
        if (lastDate != DateTime.Now.ToString("yyyy-MM-dd"))
        {
            PlayerPrefs.SetString(LastRewardAdDateKey, DateTime.Now.ToString("yyyy-MM-dd"));
            PlayerPrefs.SetInt(RewardAdLimitKey, 0);
        }
        currentRewardAdCount = PlayerPrefs.GetInt(RewardAdLimitKey, 0);
    }

    private void SaveRewardAdLimit()
    {
        PlayerPrefs.SetInt(RewardAdLimitKey, currentRewardAdCount);
    }

    #region Banner
    public void CreateBannerView()
    {
        Debug.Log("Creating banner view");

        if (_bannerView != null)
        {
            DestroyBannerView();
        }

        AdSize adaptiveSize =
                AdSize.GetCurrentOrientationAnchoredAdaptiveBannerAdSizeWithWidth(AdSize.FullWidth);

        _bannerView = new BannerView(_adBannerUnitId, adaptiveSize, AdPosition.Bottom);
    }

    public void DestroyBannerView()
    {
        if (_bannerView != null)
        {
            Debug.Log("Destroying banner view.");
            _bannerView.Destroy();
            _bannerView = null;
        }
    }

    public void LoadBannerAd()
    {
        if (_bannerView == null)
        {
            CreateBannerView();
        }

        var adRequest = new AdRequest();

        Debug.Log("Loading banner ad.");
        _bannerView.LoadAd(adRequest);
    }

    public void HideBanner()
    {
        isBannerHide = !isBannerHide;
        _bannerView.Hide();
        if (isBannerHide)
        {
            _bannerView.Show();
        }
        else
        {
            _bannerView.Hide();
        }

    }

    private void ListenToAdEvents()
    {
        _bannerView.OnBannerAdLoaded += () =>
        {
            Debug.Log("Banner view loaded an ad with response : "
                + _bannerView.GetResponseInfo());
        };
        _bannerView.OnBannerAdLoadFailed += (LoadAdError error) =>
        {
            Debug.LogError("Banner view failed to load an ad with error : "
                + error);
        };
        _bannerView.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(String.Format("Banner view paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        _bannerView.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Banner view recorded an impression.");
        };
        _bannerView.OnAdClicked += () =>
        {
            Debug.Log("Banner view was clicked.");
        };
        _bannerView.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Banner view full screen content opened.");
        };
        _bannerView.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Banner view full screen content closed.");
        };
    }
    #endregion

    #region GecisReklam
    public void LoadInterstitialAd()
    {
        if (_interstitialAd != null)
        {
            _interstitialAd.Destroy();
            _interstitialAd = null;
        }

        Debug.Log("Loading the interstitial ad.");

        var adRequest = new AdRequest();

        InterstitialAd.Load(_adInterstitialUnitId, adRequest,
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

                _interstitialAd = ad;
            });
    }

    public void ShowInterstitialAd()
    {
        if (_interstitialAd != null && _interstitialAd.CanShowAd())
        {
            Debug.Log("Showing interstitial ad.");
            _interstitialAd.Show();
            RegisterReloadHandler(_interstitialAd);
        }
        else
        {
            Debug.LogError("Interstitial ad is not ready yet.");
        }
    }

    private void RegisterEventHandlers(InterstitialAd interstitialAd)
    {
        interstitialAd.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(String.Format("Interstitial ad paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        interstitialAd.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Interstitial ad recorded an impression.");
        };
        interstitialAd.OnAdClicked += () =>
        {
            Debug.Log("Interstitial ad was clicked.");
        };
        interstitialAd.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Interstitial ad full screen content opened.");
        };
        interstitialAd.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Interstitial ad full screen content closed.");
        };
        interstitialAd.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Interstitial ad failed to open full screen content " +
                           "with error : " + error);
        };
    }

    private void RegisterReloadHandler(InterstitialAd interstitialAd)
    {
        interstitialAd.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Interstitial Ad full screen content closed.");

            LoadInterstitialAd();
        };
        interstitialAd.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Interstitial ad failed to open full screen content " +
                           "with error : " + error);

            LoadInterstitialAd();
        };
    }
    #endregion

    #region RewardedAd
    public void LoadRewardedAd()
    {
        if (_rewardedAd != null)
        {
            _rewardedAd.Destroy();
            _rewardedAd = null;
        }

        Debug.Log("Loading the rewarded ad.");

        var adRequest = new AdRequest();

        RewardedAd.Load(_adRewardUnitId, adRequest,
            (RewardedAd ad, LoadAdError error) =>
            {
                if (error != null || ad == null)
                {
                    Debug.LogError("Rewarded ad failed to load an ad " +
                                   "with error : " + error);
                    return;
                }

                Debug.Log("Rewarded ad loaded with response : "
                          + ad.GetResponseInfo());

                _rewardedAd = ad;
            });
    }

    public void ShowRewardedAd()
    {
        if (currentRewardAdCount >= DailyRewardAdLimit)
        {
            Debug.LogError("Daily limit reached for rewarded ads.");
            ShopManager.instance.ShowLimitReachedPanel(); // ShopManager'daki paneli aç.
            return;
        }

        const string rewardMsg =
            "Rewarded ad rewarded the user. Type: {0}, amount: {1}.";

        if (_rewardedAd != null && _rewardedAd.CanShowAd())
        {
            _rewardedAd.Show((Reward reward) =>
            {
                Debug.Log(String.Format(rewardMsg, reward.Type, reward.Amount));

                CoinSpawnerManager manager = FindObjectOfType<CoinSpawnerManager>();
                manager.SpawnCoins();

                CoinManager.instance.AddCoins(100);

                currentRewardAdCount++;
                SaveRewardAdLimit();
            });
            RegisterReloadHandler(_rewardedAd);
        }
    }

    private void RegisterReloadHandler(RewardedAd ad)
    {
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Rewarded Ad full screen content closed.");

            LoadRewardedAd();
        };
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Rewarded ad failed to open full screen content " +
                           "with error : " + error);

            LoadRewardedAd();
        };
    }
    #endregion

}
