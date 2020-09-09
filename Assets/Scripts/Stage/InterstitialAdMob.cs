using GoogleMobileAds.Api;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InterstitialAdMob : MonoBehaviour
{
    private AudioSource[] audioSources; //シーン内にあるすべてのAudioSource
    private bool nextIsRetry; //インタースティシャル広告を閉じた後にリトライするのならtrue, コース選択画面に戻るならfalseになる

    private InterstitialAd interstitial;

    private void Awake() {
        MainThreadEventExecutor.Initialize();
    }

    private void Start() {
        audioSources = new AudioSource[]
        {
            GameObject.Find("Player").GetComponent<AudioSource>(),
            GameObject.Find("StageManager").GetComponent<AudioSource>(),
            GameObject.Find("UIManager").GetComponent<AudioSource>()
        };

        RequestInterstitial();
    }

    private void RequestInterstitial() {

        // 本番用
        /*
        #if UNITY_ANDROID
            string adUnitId = "ca-app-pub-5831891803553766/7635004402";
        #elif UNITY_IPHONE
            string adUnitId = "ca-app-pub-5831891803553766/7900739460";
        #else
            string adUnitId = "unexpected_platform";
        #endif
        */

        // テスト用
        
#if UNITY_ANDROID
        string adUnitId = "ca-app-pub-3940256099942544/1033173712";
#elif UNITY_IPHONE
            string adUnitId = "ca-app-pub-3940256099942544/4411468910";
#else
            string adUnitId = "unexpected_platform";
#endif
        


        // Initialize an InterstitialAd.
        this.interstitial = new InterstitialAd(adUnitId);

        // Called when an ad request has successfully loaded.
        this.interstitial.OnAdLoaded += HandleOnAdLoaded;
        // Called when an ad request failed to load.
        this.interstitial.OnAdFailedToLoad += HandleOnAdFailedToLoad;
        // Called when an ad is shown.
        this.interstitial.OnAdOpening += HandleOnAdOpened;
        // Called when the ad is closed.
        this.interstitial.OnAdClosed += HandleOnAdClosed;
        // Called when the ad click caused the user to leave the application.
        this.interstitial.OnAdLeavingApplication += HandleOnAdLeavingApplication;

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();

        // Load the interstitial with the request.
        this.interstitial.LoadAd(request);
    }

    public void ShowInterstitialAd(bool retry) {
        if (retry) {
            nextIsRetry = true;
        }
        else {
            nextIsRetry = false;
        }

        if (this.interstitial.IsLoaded()) {
            this.interstitial.Show();
        }
        else {
            if (nextIsRetry) {
                FadeManager.Instance.LoadScene("Stage", 1f, 0f, 1f);
            }
            else {
                FadeManager.Instance.LoadScene("CourseSelection", 1f, 0f, 1f);
            }
        }
    }

    public void HandleOnAdLoaded(object sender, EventArgs args) {
    }

    public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args) {
    }

    /*インタースティシャル広告が開くときに実行される*/
    public void HandleOnAdOpened(object sender, EventArgs args) {
        MainThreadEventExecutor.Execute(() =>
        {
            /*リワード広告視聴中はアプリの音が聞こえないようにする*/
            foreach (AudioSource audioSource in audioSources) {
                audioSource.mute = true;
            }
        });
    }

    /*インタースティシャル広告が閉じるときに実行される*/
    public void HandleOnAdClosed(object sender, EventArgs args) {
        MainThreadEventExecutor.Execute(() =>
        {
            if (nextIsRetry) {
                FadeManager.Instance.LoadScene("Stage", 0f, 0f, 1f);
            }
            else {
                SceneManager.LoadScene("CourseSelection");
            }
        });

        //RequestInterstitial();
    }

    public void HandleOnAdLeavingApplication(object sender, EventArgs args) {
    }
}
