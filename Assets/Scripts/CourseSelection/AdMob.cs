using UnityEngine;
using GoogleMobileAds.Api;

public class AdMob : MonoBehaviour
{
    private BannerView bannerView;

    public void Start() {

        #if UNITY_ANDROID
            string appId = "ca-app-pub-5831891803553766~4022954650";
        #elif UNITY_IPHONE
            string appId = "ca-app-pub-5831891803553766~8519670658";
        #else
            string appId = "unexpected_platform";
        #endif

        // Initialize the Google Mobile Ads SDK.
        MobileAds.Initialize(appId);
    }

    public void RequestBanner() {

        // 本番用
        
        #if UNITY_ANDROID
            string adUnitId = "ca-app-pub-5831891803553766/5391984444";
        #elif UNITY_IPHONE
            string adUnitId = "ca-app-pub-5831891803553766/1762690616";
        #else
            string adUnitId = "unexpected_platform";
        #endif
        

        //テスト用
        /*
        #if UNITY_ANDROID
            string adUnitId = "ca-app-pub-3940256099942544/6300978111";
        #elif UNITY_IPHONE
            string adUnitId = "ca-app-pub-3940256099942544/2934735716";
        #else
            string adUnitId = "unexpected_platform";
        #endif
        */

        // Create a 320x50 banner at the top of the screen.
        this.bannerView = new BannerView(adUnitId, AdSize.Banner, AdPosition.Top);

        // Create an empty ad request.
        AdRequest request = new AdRequest.Builder().Build();

        // Load the banner with the request.
        this.bannerView.LoadAd(request);
    }

    /*バナー広告を非表示にする*/
    public void HideBanner() {
        bannerView.Hide();
    }

    /*バナー広告を表示する*/
    public void ShowBanner() {
        bannerView.Show();
    }

    
}