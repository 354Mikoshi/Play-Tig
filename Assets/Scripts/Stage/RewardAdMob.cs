using GoogleMobileAds.Api;
using System;
using UnityEngine;
using UnityEngine.UI;

public class RewardAdMob : MonoBehaviour
{
    private UIManager uiManager;
    private AudioSource[] audioSources; //シーン内にあるすべてのAudioSource
    
    private RewardedAd rewardedAd;

    private void Awake() {
        MainThreadEventExecutor.Initialize();
    }

    void Start() {
        uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
        audioSources = new AudioSource[]
        {
            GameObject.Find("Player").GetComponent<AudioSource>(),
            GameObject.Find("StageManager").GetComponent<AudioSource>(),
            GameObject.Find("UIManager").GetComponent<AudioSource>()
        };

        CreateAndLoadRewardedAd();
    }

    public void CreateAndLoadRewardedAd() {
        // 本番用
        /*
        #if UNITY_ANDROID
            string adUnitId = "ca-app-pub-5831891803553766/3121044321";
        #elif UNITY_IPHONE
            string adUnitId = "ca-app-pub-5831891803553766/8575510972";
        #else
            string adUnitId = "unexpected_platform";
        #endif
        */

        // テスト用
        
#if UNITY_ANDROID
        string adUnitId = "ca-app-pub-3940256099942544/5224354917";
#elif UNITY_IPHONE
            string adUnitId = "ca-app-pub-3940256099942544/1712485313";
#else
            string adUnitId = "unexpected_platform";
#endif
        

        this.rewardedAd = new RewardedAd(adUnitId);

        // Called when an ad request has successfully loaded.
        this.rewardedAd.OnAdLoaded += HandleRewardedAdLoaded;
        // Called when an ad request failed to load.
        this.rewardedAd.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
        // Called when an ad is shown.
        this.rewardedAd.OnAdOpening += HandleRewardedAdOpening;
        // Called when an ad request failed to show.
        this.rewardedAd.OnAdFailedToShow += HandleRewardedAdFailedToShow;
        // Called when the user should be rewarded for interacting with the ad.
        this.rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
        // Called when the ad is closed.
        this.rewardedAd.OnAdClosed += HandleRewardedAdClosed;

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();
        // Load the rewarded ad with the request.
        this.rewardedAd.LoadAd(request);
    }

    /*リワード広告を視聴する*/
    public void UserChoseToWatchAd() {
        if (this.rewardedAd.IsLoaded()) {
            this.rewardedAd.Show();
        }
        else {
            uiManager.OpenWholeMap(); //全体マップを見せる
            uiManager.loadFailedText.gameObject.SetActive(true);
            this.CreateAndLoadRewardedAd(); //新たなリワード広告を読み込んでおく
        }
    }

    private void HandleRewardedAdFailedToShow(object sender, AdErrorEventArgs e) {
        CreateAndLoadRewardedAd(); //新たなリワード広告を読み込んでおく
    }

    /*リワード広告が開くときに実行される*/
    private void HandleRewardedAdOpening(object sender, EventArgs e) {
        MainThreadEventExecutor.Execute(() =>
        {
            uiManager.IS_GOING_ON = false; //ゲームを停止する

            /*リワード広告視聴中はアプリの音が聞こえないようにする*/
            foreach (AudioSource audioSource in audioSources) {
                audioSource.mute = true;
            }
        });
    }

    private void HandleRewardedAdFailedToLoad(object sender, AdErrorEventArgs e) {
    }

    private void HandleRewardedAdClosed(object sender, EventArgs e) {
        MainThreadEventExecutor.Execute(() =>
        {
            uiManager.CloseWatchRewardVideoScreen(); //リワード広告を見るかどうか聞くパネルを閉じる

            uiManager.IS_GOING_ON = true; //ゲームを再開させる

            /*シーン内の音を復活させる*/
            foreach (AudioSource audioSource in audioSources) {
                audioSource.mute = false;
            }

            CreateAndLoadRewardedAd(); //新たなリワード広告を読み込んでおく
        });
    }

    /*動画を視聴したユーザーに報酬を付与するときに実行される*/
    private void HandleUserEarnedReward(object sender, Reward e) {
        MainThreadEventExecutor.Execute(() =>
        {
            uiManager.OpenWholeMap(); //全体マップを見せる
        });
    }

    private void HandleRewardedAdLoaded(object sender, EventArgs e) {
    }

    void Update() {

    }
}
