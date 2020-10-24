using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    /*クリア画面にあるオブジェクト*/
    public Image clearBeltImage;

    /*ゲームオーバー画面にあるオブジェクト*/
    public Image gameOverBackgroundImage, backUnderlineImage, retryUnderlineImage;
    public Button backToCourseSelectionButton, retryButton;
    public Text gameOverText, backText, retryText;

    /*リワード広告を見る画面にあるオブジェクト*/
    //public Image watchRewardVideoImage;

    /*戻る矢印のボタン*/
    public Image backArrowImage;

    /*ミニマップの画像*/
    //public RawImage minimap;

    /*フリックの情報を受け取るためのImage*/
    public Image flickImage1, flickImage2;

    public Text meterText, countDownText, collectText, yesText, noText, loadFailedText;
    public Image firstBridge, secondBridge, wholeMap;
    public Button collectButton;
    public GameObject wholeMapCamera, backToCourseSelectionScreen;
    public AudioClip clearSound, gameOverShoutSound, dropBridgeSound, collectBridgeSound, mouseClickSound, bellSound;

    /*リトライ時にフェードイン、フェードアウトさせる画像*/
    public Image retryBackgroundImage;

    /*アドモブ*/
    private InterstitialAdMob interstitialAdMob;

    private StageManager stageManager;
    private PlayerController playerController;

    private AudioSource audioSource;

    private static bool isGoingOn; //ゲームが進行中だったらtrueになる
    public bool IS_GOING_ON {
        get { return isGoingOn; }
        set { isGoingOn = value; }
    }

    public static int bridgeNumber; //アイテム欄に入っている橋の数

    void Start() {
        stageManager = GameObject.Find("StageManager").GetComponent<StageManager>();
        interstitialAdMob = GameObject.Find("AdMob").GetComponent<InterstitialAdMob>();
        playerController = Camera.main.transform.root.gameObject.GetComponent<PlayerController>();
        audioSource = GetComponent<AudioSource>();

        /*ゲームオーバー画面のオブジェクト*/
        gameOverBackgroundImage.gameObject.SetActive(false);

        /*クリア画面のオブジェクト*/
        clearBeltImage.gameObject.SetActive(false);

        /*リトライ背景のオブジェクト*/
        retryBackgroundImage.gameObject.SetActive(false);

        /*リワード広告を見るための画面*/
        //watchRewardVideoImage.gameObject.SetActive(false);

        /*リワード広告の読み込みに失敗したテキスト*/
        loadFailedText.gameObject.SetActive(false);

        /*初め、プレイヤーは移動や回転ができない*/
        flickImage1.gameObject.SetActive(false);
        flickImage2.gameObject.SetActive(false);

        countDownText.gameObject.SetActive(false);
        backToCourseSelectionScreen.gameObject.SetActive(false);

        wholeMap.gameObject.SetActive(false);
        wholeMapCamera.gameObject.SetActive(false);

        IS_GOING_ON = false;

        bridgeNumber = 0;
        firstBridge.gameObject.SetActive(false);
        secondBridge.gameObject.SetActive(false);

        ChangeCollectBridgeOrWallButton(false);

        StartCoroutine(CountDown());
    }

    // Update is called once per frame
    void Update() {
    }

    public void UpdateMeterText(float distance) {
        meterText.text = distance.ToString("F2");
    }

    public void Clear() {
        flickImage1.gameObject.SetActive(false);
        flickImage2.gameObject.SetActive(false);
    }

    public void GameOver() {
        flickImage1.gameObject.SetActive(false);
        flickImage2.gameObject.SetActive(false);
        IS_GOING_ON = false;

        gameOverBackgroundImage.gameObject.SetActive(true);
        audioSource.PlayOneShot(gameOverShoutSound);

        StartCoroutine(GameOverFadeIn());

        backToCourseSelectionButton.gameObject.SetActive(true);
        retryButton.gameObject.SetActive(true);
    }

    /*「回収する」ボタンを押すと引数にtrueを渡してこのメソッドが呼ばれる*/
    public void UpdateBridgeImages(bool increase) { //increaseがtrueのとき、アイテム欄に表示させる橋のアイコンを1つ増やす
        if (IS_GOING_ON) {
            if (increase) { //アイテム欄に表示させる橋のアイコン数を増やせるなら増やす

                if (playerController.oneSquareAhead.y == 1
                    && stageManager.ogreSquares[playerController.oneSquareAhead.x, playerController.oneSquareAhead.y, playerController.oneSquareAhead.z]
                    || playerController.oneSquareAhead.y == 0) {
                    if (bridgeNumber == 0) { //アイテム欄の橋の数が0→1になる
                        firstBridge.gameObject.SetActive(true);
                        //Debug.Log("橋のアイコン: 0→1");
                    }
                    else if (bridgeNumber == 1) { //1→2になる
                        firstBridge.gameObject.SetActive(true);
                        secondBridge.gameObject.SetActive(true);
                        //Debug.Log("橋のアイコン: 1→2");
                    }
                    if (bridgeNumber < 2) {
                        bridgeNumber++;
                        stageManager.DestroyBridges(playerController.oneSquareAhead);
                        audioSource.PlayOneShot(collectBridgeSound);
                        ChangeCollectBridgeOrWallButton(false);
                        //Debug.Log("橋を回収しました");
                    }
                }

            }
            else { //アイテム欄に表示させる橋のアイコンを減らせるなら減らす
                if (bridgeNumber == 1) { //1→0になる
                    firstBridge.gameObject.SetActive(false);
                    secondBridge.gameObject.SetActive(false);
                    //Debug.Log("橋のアイコン: 1→0");
                }
                else if (bridgeNumber == 2) { //2→1になる
                    firstBridge.gameObject.SetActive(true);
                    secondBridge.gameObject.SetActive(false);
                    //Debug.Log("橋のアイコン: 2→1");
                }
                if (bridgeNumber > 0) {
                    bridgeNumber--;
                    audioSource.PlayOneShot(dropBridgeSound);
                    //Debug.Log("手持ちの橋が減りました");
                }
            }
            //Debug.Log("橋の数 = " + bridgeNumber);
        }
    }

    public void ChangeCollectBridgeOrWallButton(bool canCollect) {
        if (bridgeNumber < 2 && canCollect) { //ボタンを有効にし、ボタンとテキストを濃くする
            collectButton.GetComponent<Button>().enabled = true;
            collectButton.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
            collectText.GetComponent<Text>().color = new Color(0f, 0f, 0f, 1f);
            //Debug.Log("「回収する」のボタンを有効にしました");
        }
        else { //ボタンを無効にし、ボタンをテキストを薄くする
            collectButton.GetComponent<Button>().enabled = false;
            collectButton.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.3f);
            collectText.GetComponent<Text>().color = new Color(0f, 0f, 0f, 0.3f);
            //Debug.Log("「回収する」のボタンを無効にしました");
        }
    }

    //画面左上にある戻るボタンを押すとコース選択画面に戻るための画面が開く
    public void OpenBackToCourseSelectionScreen() {
        if (IS_GOING_ON) {
            audioSource.PlayOneShot(bellSound);
            flickImage1.gameObject.SetActive(false);
            flickImage2.gameObject.SetActive(false);
            backToCourseSelectionScreen.gameObject.SetActive(true);
        }
    }

    //コース選択画面に戻る
    public void MoveToCourseSelectionScreen() {
        audioSource.PlayOneShot(mouseClickSound);
        IS_GOING_ON = false;
        yesText.raycastTarget = false; //複数回「はい」ボタンが押されることを防止する
        noText.raycastTarget = false; //「いいえ」を押せないようにする
        retryButton.interactable = false; //「もう一度」ボタンを押せないようにする
        backToCourseSelectionButton.interactable = false; //複数回「コース選択画面に戻る」ボタンが押されることを防止する
        interstitialAdMob.ShowInterstitialAd(false);
        //FadeManager.Instance.LoadScene("CourseSelection", 1f, 0f, 0f);
    }

    //「いいえ」ボタンを押すとコース選択画面に戻るための画面を閉じる
    public void CloseBackToCourseSelectionScreen() {
        audioSource.PlayOneShot(mouseClickSound);
        backToCourseSelectionScreen.gameObject.SetActive(false);
        flickImage1.gameObject.SetActive(true);
        flickImage2.gameObject.SetActive(true);
    }

    /*ゲームオーバーしたときにリトライする*/
    public void Retry() {
        audioSource.PlayOneShot(mouseClickSound);
        retryButton.interactable = false; //複数回「もう一度」ボタンが押されることを防止する
        backToCourseSelectionButton.interactable = false; //「コース選択に戻る」ボタンを押せないようにする
        retryBackgroundImage.gameObject.SetActive(true);
        retryBackgroundImage.raycastTarget = true;
        interstitialAdMob.ShowInterstitialAd(true);
        //FadeManager.Instance.LoadScene("Stage", 1f, 0f, 0f);
        //retryBackgroundImage.raycastTarget = false;
        //retryBackgroundImage.gameObject.SetActive(false);
    }

    /*全体マップを開く*/
    public void OpenWholeMap() {
        stageManager.MakeOgresActive(false); //鬼を非表示にする
        wholeMapCamera.gameObject.SetActive(true);
        wholeMap.gameObject.SetActive(true);
    }

    /*全体マップを閉じる*/
    public void CloseWholeMap() {
        wholeMap.gameObject.SetActive(false);
        wholeMapCamera.gameObject.SetActive(false);
        //watchRewardVideoImage.gameObject.SetActive(false);
        loadFailedText.gameObject.SetActive(false);
        stageManager.MakeOgresActive(true); //鬼を表示する

        flickImage1.gameObject.SetActive(true);
        flickImage2.gameObject.SetActive(true);
    }

    /*リワード広告を見るか聞くパネルを開く*/
    public void OpenWatchRewardVideoScreen() {
        if (IS_GOING_ON) {
            flickImage1.gameObject.SetActive(false);
            flickImage2.gameObject.SetActive(false);
            //watchRewardVideoImage.gameObject.SetActive(true);
        }
    }

    /*リワード広告を見るか聞くパネルを閉じる*/
    public void CloseWatchRewardVideoScreen() {
        audioSource.PlayOneShot(mouseClickSound);
        //watchRewardVideoImage.gameObject.SetActive(false);
        flickImage1.gameObject.SetActive(true);
        flickImage2.gameObject.SetActive(true);
    }

    /*カウントダウン処理*/
    private IEnumerator CountDown() {
        yield return new WaitForSeconds(3f);
        countDownText.gameObject.SetActive(true);
        countDownText.text = "2";
        yield return new WaitForSeconds(1f);
        countDownText.text = "1";
        yield return new WaitForSeconds(1f);
        countDownText.text = "Go!";
        IS_GOING_ON = true;
        flickImage1.gameObject.SetActive(true);
        flickImage2.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        countDownText.gameObject.SetActive(false);
    }

    private IEnumerator GameOverFadeIn() {
        float alpha = 0.1f;
        for (int i = 0; i < 10; i++) {
            gameOverBackgroundImage.color = new Color(gameOverBackgroundImage.color.r, gameOverBackgroundImage.color.g,
                gameOverBackgroundImage.color.b, alpha * (i + 1));
            backUnderlineImage.color = new Color(backUnderlineImage.color.r, backUnderlineImage.color.g, backUnderlineImage.color.b,
                alpha * (i + 1));
            retryUnderlineImage.color = new Color(retryUnderlineImage.color.r, retryUnderlineImage.color.g, retryUnderlineImage.color.b,
                alpha * (i + 1));
            gameOverText.color = new Color(gameOverText.color.r, gameOverText.color.g, gameOverText.color.b, alpha * (i + 1));
            backText.color = new Color(backText.color.r, backText.color.g, backText.color.b, alpha * (i + 1));
            retryText.color = new Color(retryText.color.r, retryText.color.g, retryText.color.b, alpha * (i + 1));
            yield return new WaitForSeconds(0.05f);
        }
    }

    public IEnumerator DisplayClearBelt() {

        if (IS_GOING_ON) {
            IS_GOING_ON = false;
            backArrowImage.raycastTarget = false;
            //minimap.raycastTarget = false;
            clearBeltImage.gameObject.SetActive(true);
            audioSource.PlayOneShot(clearSound);
            stageManager.RecordClearCourseNumber(); //クリアしたことを保存する

            float alpha = 0.1f;
            for (int i = 0; i < 10; i++) {
                clearBeltImage.color = new Color(clearBeltImage.color.r, clearBeltImage.color.g, clearBeltImage.color.b, alpha * (i + 1));
                yield return new WaitForSeconds(0.05f);
            }

            yield return new WaitForSeconds(2f);
            interstitialAdMob.ShowInterstitialAd(false); //インタースティシャル広告を見せる
        }

    }
}
