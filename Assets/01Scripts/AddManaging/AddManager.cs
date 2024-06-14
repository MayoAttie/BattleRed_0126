using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GoogleMobileAds.Api;
using GoogleMobileAds.Common;
using System;
using UnityEngine.UI;
using TMPro;
using static UI_UseToolClass;
using static UseTool;
public class AddManager : Singleton<AddManager>
{
    // These ad units are configured to always serve test ads.
#if UNITY_ANDROID
    private string _adUnitId = "ca-app-pub-3940256099942544/5224354917";
#elif UNITY_IPHONE
  private string _adUnitId = "ca-app-pub-3940256099942544/1712485313";
#else
  private string _adUnitId = "unused";
#endif
    
    private RewardedAd _rewardedAd;
    List<ItemClass> rewardData;                         // reward 아이템 데이터
    Transform rewardPrintObj;                           // reward 데이터 출력 오브젝트

    private void Awake()
    {
    }

    public void Start()
    {
        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize((InitializationStatus initStatus) =>
        {
            LoadRewardedAd();
        });
    }

    public void LoadRewardedAd()
    {
        // Clean up the old ad before loading a new one.
        if (_rewardedAd != null)
        {
            _rewardedAd.Destroy();
            _rewardedAd = null;
        }

        Debug.Log("Loading the rewarded ad.");

        // create our request used to load the ad.
        var adRequest = new AdRequest();

        // send the request to load the ad.
        RewardedAd.Load(_adUnitId, adRequest,
            (RewardedAd ad, LoadAdError error) =>
            {
              // if error is not null, the load request failed.
              if (error != null || ad == null)
                {
                    Debug.LogError("Rewarded ad failed to load an ad " +
                                   "with error : " + error);
                    return;
                }

                Debug.Log("Rewarded ad loaded with response : "
                          + ad.GetResponseInfo());

                _rewardedAd = ad;

                // Register event handlers after ad is loaded
                RegisterEventHandlers(ad);
                RegisterReloadHandler(ad);
            });
    }

    private void RegisterEventHandlers(RewardedAd ad)
    {
        // Raised when the ad is estimated to have earned money.
        ad.OnAdPaid += (AdValue adValue) =>
        {
            Debug.Log(String.Format("Rewarded ad paid {0} {1}.",
                adValue.Value,
                adValue.CurrencyCode));
        };
        // Raised when an impression is recorded for an ad.
        ad.OnAdImpressionRecorded += () =>
        {
            Debug.Log("Rewarded ad recorded an impression.");
        };
        // Raised when a click is recorded for an ad.
        ad.OnAdClicked += () =>
        {
            Debug.Log("Rewarded ad was clicked.");
        };
        // Raised when an ad opened full screen content.
        ad.OnAdFullScreenContentOpened += () =>
        {
            Debug.Log("Rewarded ad full screen content opened.");
        };
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
        {
            Debug.Log("Rewarded ad full screen content closed.");
        };
        // Raised when the ad failed to open full screen content.
        ad.OnAdFullScreenContentFailed += (AdError error) =>
        {
            Debug.LogError("Rewarded ad failed to open full screen content " +
                           "with error : " + error);
        };
    }


    private void RegisterReloadHandler(RewardedAd ad)
    {
        // Raised when the ad closed full screen content.
        ad.OnAdFullScreenContentClosed += () =>
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

    private void HandleUserEarnedReward(Reward reward)
    {
        ProcessReward();
    }
    private void ProcessReward()
    {
        rewardPrintObj.gameObject.SetActive(true);

        Image img1 = rewardPrintObj.GetChild(1).GetComponent<Image>();
        TextMeshProUGUI txt1 = img1.GetComponentInChildren<TextMeshProUGUI>();
        Image img2 = rewardPrintObj.GetChild(2).GetComponent<Image>();    
        TextMeshProUGUI txt2 = img2.GetComponentInChildren<TextMeshProUGUI>();
        Image img3 = rewardPrintObj.GetChild(3).GetComponent<Image>();    
        TextMeshProUGUI txt3 = img3.GetComponentInChildren<TextMeshProUGUI>();

        ItemClass data1 = new ItemClass();
        data1.CopyFrom(rewardData[0]);

        WeaponAndEquipCls data2 = new WeaponAndEquipCls();
        data2.CopyFrom(rewardData[1]);

        var instance = GameManager.Instance.GetUserClass();

        if (data1.GetTag() == "광물")
        {
            GemKindDivider(data1, img1);
            txt1.text = data1.GetName();
            ItemDataInsert_excludingEquipment(data1);
        }
        else if(data1.GetTag() == "음식")
        {
            FoodKindDivider(data1, img1);
            txt1.text = data1.GetName();
            ItemDataInsert_excludingEquipment(data1);
        }
        else
        {
            Sprite spr = WeaponAndEquipLimitBreak_UI_Dvider(data1);
            txt1.text = data1.GetName();
            img1.sprite = spr;
            ItemDataInsert_excludingEquipment(data1);
        }

        if (data2.GetTag() == "무기")
        {
            WeaponKindDivider(data2, img2);
            txt2.text = data2.GetName();
            GameManager.Instance.Item_Id_Generator(data2);
            instance.GetHadWeaponList().Add(data2);
        }
        else
        {
            EquipmentKindDivider(data2, img2);
            txt2.text = data2.GetName();
            GameManager.Instance.EquipStatusRandomSelector(data2);
            GameManager.Instance.EquipItemStatusSet(data2);
            GameManager.Instance.Item_Id_Generator(data2);
            instance.GetHadEquipmentList().Add(data2);
        }

        img3.sprite = ItemSpritesSaver.Instance.SpritesSet[3];
        txt3.text = "10000";
        int mora = instance.GetMora();
        instance.SetMora(mora + 10000);

        Invoke("CloseRewardWindow", 2.5f);
    }

    void CloseRewardWindow()
    {
        rewardPrintObj.gameObject.SetActive(false);
    }

    public void ShowRewardedAd(Transform rewardPrintObj, List<ItemClass> rewardData)
    {
        if (_rewardedAd != null)
        {
            _rewardedAd.Show(HandleUserEarnedReward);
            this.rewardData = rewardData;
            this.rewardPrintObj = rewardPrintObj;
        }
        else
        {
            Debug.LogWarning("Rewarded ad is not ready to be shown yet.");
        }
    }
}
