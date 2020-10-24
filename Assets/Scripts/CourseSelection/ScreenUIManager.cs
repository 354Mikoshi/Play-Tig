using System;
using System.Collections;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class ScreenUIManager : MonoBehaviour
{
    public GameObject titleScreen, courseSelectionScreen, tutorialScreen, settingScreen, reviewScreen; //各種画面
    public AudioClip courseSelecionButtonSound, decideCourseSelectionOrTutorialSound, clickSound, bellSound; //各種サウンド

    /*コース選択画面のUI*/
    public GameObject buttonPrototype; //ボタンのプレハブ
    public Text difficultyText, explanationText, starText, courseText;
    public Button proceedButton;
    public RectTransform contentRectTransform; //ボタンを配置するための長方形領域

    /*設定画面のUI*/
    public GameObject reset, medal, opening;
    public Text questionSentenceText, dataErasedText, clearedNumberText;
    public Button eraseDataButton;
    public Image[] medals; //メダルたち

    /*チュートリアル画面のUI*/
    public GameObject tutorialButtonPrefab; //ボタンのプレハブ
    public RectTransform tutorialContentRectTransform; //ボタンを配置するための長方形領域
    public Text tutorialTitleText;
    public GameObject rulePanel, howToMovePanel, stairPanel, itemStoragePanel, bridgePanel, wallPanel,
        collectPanel, distanceDisplayPanel/*, minimapPanel*/, ogrePanel;

    /*トロフィー画面のUI*/
    public Image trophyScreen;
    public Text clearText, sentenceText, numberText;

    /*アプリ起動時に流れる動画*/
    private VideoPlayer videoPlayer;
    public Image initialMovieBackground;
    public RawImage initialMovie;
    private static bool moviePlay = true; //コースから戻ってきたとき

    /*BGM*/
    private AudioSource BGM;

    /*エンディングのUI*/
    public Image finalMessageImage;
    public Text finalMessageText;

    private const int theNumberOfCourses = 50; //コースの数
    private int clearedCourses; //クリア済みのコースの数
    public static int courseNumber; //コース番号
    private GameObject[] courseButtons; //コースボタン(クリア済みのコースの右上にチェックマークを出すときに必要)

    private AudioSource audioSource;
    private AdMob admob;
    private static bool admobBannerExist;
    private bool canFinishEnding; //trueだったらエンディングを終了できる
    public static bool canClick; //「行く」ボタンを押した瞬間にfalseになり、画面のボタンを押せなくなる

    private enum Screen { title, courseSelection, tutorial, setting }
    private static Screen lastScreen;

    void Start() {
        audioSource = GetComponent<AudioSource>();
        admob = GameObject.Find("AdMob").GetComponent<AdMob>();
        videoPlayer = GameObject.Find("Video Player").GetComponent<VideoPlayer>();
        BGM = GameObject.Find("BGM").GetComponent<AudioSource>();

        courseButtons = new GameObject[theNumberOfCourses];
        admobBannerExist = false;
        canClick = true; //初めはボタンをクリックできる状態にしておく

        /*バナー広告を最初の1回だけリクエストする*/
        if (!admobBannerExist) {
            admob.RequestBanner();
            admobBannerExist = true;
        }

        /*アプリ起動時のみオープニング動画を再生する*/
        if (moviePlay) {

            lastScreen = Screen.title;
            PlayOpeningMovie();
            moviePlay = false;

            /*デバッグ用*/
            PlayerPrefs.SetInt("Course1".ToString(), 0);
            for (int i = 1; i <= 15; i++) {
                /*15コースをクリア済みにしておく*/
                PlayerPrefs.SetInt("Course" + (i + 1).ToString(), 1);
            }

        }
        else {
            BGM.mute = false;
            //BGM.Play();
            videoPlayer.Pause();
            initialMovieBackground.gameObject.SetActive(false);
        }

        Initialize();
    }

    private void Initialize() {
        CreateButtons();
        CreateTutorialButtons();
        trophyScreen.gameObject.SetActive(false); //トロフィー画面を一旦非表示にする
        reviewScreen.gameObject.SetActive(false); //レビュー要請画面を非表示にする
        finalMessageImage.gameObject.SetActive(false); //エンディング画面を非表示にする

        /*最後に開いていた画面に応じてUIを表示する*/
        if (lastScreen == Screen.tutorial) {
            difficultyText.gameObject.SetActive(false);
            proceedButton.gameObject.SetActive(false);
            titleScreen.gameObject.SetActive(false);
            courseText.gameObject.SetActive(false);
            courseSelectionScreen.gameObject.SetActive(false);
            tutorialScreen.gameObject.SetActive(true);
            settingScreen.gameObject.SetActive(false);

            admob.ShowBanner();
        }
        else if (lastScreen == Screen.courseSelection) {

            difficultyText.gameObject.SetActive(false);
            proceedButton.gameObject.SetActive(false);
            courseText.gameObject.SetActive(false);
            titleScreen.gameObject.SetActive(false);
            courseSelectionScreen.gameObject.SetActive(true);
            tutorialScreen.gameObject.SetActive(false);
            settingScreen.gameObject.SetActive(false);

            admob.ShowBanner();

            /*クリア済みのコース数を取得する*/
            int tmp = 0;
            for (int i = 0; i < theNumberOfCourses; i++) {
                /*クリア済みのコースにはチェックマークを表示する*/
                if (PlayerPrefs.GetInt("Course" + (i + 1).ToString(), 0) == 1) {
                    courseButtons[i].gameObject.transform.GetChild(1).transform.gameObject.SetActive(true);
                    tmp++;
                }
            }
            clearedCourses = tmp;

            DisplayTrophyScreen(); //トロフィー画面を表示する

            if (clearedCourses == 16 || clearedCourses == 31) {
                RequestReview(); //ユーザーにアプリストアでのレビューを頼む
            }

        }
        else if (lastScreen == Screen.title) {
            difficultyText.gameObject.SetActive(false);
            proceedButton.gameObject.SetActive(false);
            courseText.gameObject.SetActive(false);
            titleScreen.gameObject.SetActive(true);
            courseSelectionScreen.gameObject.SetActive(false);
            tutorialScreen.gameObject.SetActive(false);
            settingScreen.gameObject.SetActive(false);

            admob.HideBanner();
        }
    }

    //コース選択画面に進む
    public void MoveToCourseSelectionScreen() {
        audioSource.PlayOneShot(decideCourseSelectionOrTutorialSound);
        titleScreen.gameObject.SetActive(false);
        courseSelectionScreen.gameObject.SetActive(true);

        for (int i = 0; i < theNumberOfCourses; i++) {
            /*クリア済みのコースにはチェックマークを表示する*/
            if (PlayerPrefs.GetInt("Course" + (i + 1).ToString(), 0) == 1) {
                courseButtons[i].gameObject.transform.GetChild(1).transform.gameObject.SetActive(true);
            }
            else {
                courseButtons[i].gameObject.transform.GetChild(1).transform.gameObject.SetActive(false);
            }
        }

        admob.ShowBanner();
        lastScreen = Screen.courseSelection;
    }

    //チュートリアル画面に進む
    public void MoveToTutorialScreen() {
        audioSource.PlayOneShot(decideCourseSelectionOrTutorialSound);
        titleScreen.gameObject.SetActive(false);
        tutorialScreen.gameObject.SetActive(true);

        /*一旦すべての説明テキストを非アクティブにする*/
        tutorialTitleText.text = "";
        rulePanel.gameObject.SetActive(false);
        howToMovePanel.gameObject.SetActive(false);
        stairPanel.gameObject.SetActive(false);
        itemStoragePanel.gameObject.SetActive(false);
        bridgePanel.gameObject.SetActive(false);
        wallPanel.gameObject.SetActive(false);
        collectPanel.gameObject.SetActive(false);
        distanceDisplayPanel.gameObject.SetActive(false);
        //minimapPanel.gameObject.SetActive(false);
        ogrePanel.gameObject.SetActive(false);

        admob.ShowBanner();
        lastScreen = Screen.tutorial;
    }

    /*設定画面に進む*/
    public void MoveToSettingScreen() {
        audioSource.PlayOneShot(decideCourseSelectionOrTutorialSound);
        titleScreen.gameObject.SetActive(false);
        courseSelectionScreen.gameObject.SetActive(false);
        tutorialScreen.gameObject.SetActive(false);
        settingScreen.gameObject.SetActive(true);

        opening.SetActive(false); //「オープニング」の要素を非表示にする
        reset.SetActive(false); //「リセット」の要素を非表示にする
        medal.SetActive(false); //「メダル」の要素を非表示にする

        admob.ShowBanner();
        lastScreen = Screen.setting;
    }

    /*設定画面のリセットボタンを押すと、このメソッドが実行される*/
    public void OpenResetElements() {
        audioSource.PlayOneShot(clickSound);
        dataErasedText.gameObject.SetActive(false);

        /*リセット要素だけアクティブにする*/
        medal.SetActive(false);
        opening.SetActive(false);
        reset.SetActive(true);
    }

    /*設定画面のメダルボタンを押すと、このメソッドが実行される*/
    public void OpenMedalElements() {

        /*クリア済みのコース数を取得する*/
        int tmp = 0;
        for (int i = 0; i < theNumberOfCourses; i++) {
            /*クリア済みのコースにはチェックマークを表示する*/
            if (PlayerPrefs.GetInt("Course" + (i + 1).ToString(), 0) == 1) {
                tmp++;
            }
        }
        clearedCourses = tmp;

        audioSource.PlayOneShot(clickSound);
        clearedNumberText.text = clearedCourses.ToString();
        foreach (Image image in medals) {
            image.gameObject.SetActive(false); //一旦すべてのメダルの画像を非表示にする
        }
        if (clearedCourses >= 1) {
            medals[0].gameObject.SetActive(true);
        }
        if (clearedCourses >= 5) {
            medals[1].gameObject.SetActive(true);
        }
        if (clearedCourses >= 10) {
            medals[2].gameObject.SetActive(true);
        }
        if (clearedCourses >= 15) {
            medals[3].gameObject.SetActive(true);
        }
        if (clearedCourses >= 20) {
            medals[4].gameObject.SetActive(true);
        }
        if (clearedCourses >= 25) {
            medals[5].gameObject.SetActive(true);
        }
        if (clearedCourses >= 30) {
            medals[6].gameObject.SetActive(true);
        }
        if (clearedCourses >= 40) {
            medals[7].gameObject.SetActive(true);
        }
        if (clearedCourses >= 45) {
            medals[8].gameObject.SetActive(true);
        }
        if (clearedCourses >= 50) {
            medals[9].gameObject.SetActive(true);
        }

        /*メダル要素だけアクティブにする*/
        reset.SetActive(false);
        opening.SetActive(false);
        medal.SetActive(true);

    }

    /*設定画面でオープニングボタンを押すと、このメソッドが実行される*/
    public void OpenOpeningElements() {
        audioSource.PlayOneShot(clickSound);

        /*オープニング要素だけアクティブにする*/
        medal.SetActive(false);
        reset.SetActive(false);
        opening.SetActive(true);
    }

    /*タイトル画面に戻る*/
    public void BackToTitleScreen() {
        if (canClick) {
            audioSource.PlayOneShot(bellSound);

            courseSelectionScreen.gameObject.SetActive(false);
            tutorialScreen.gameObject.SetActive(false);
            settingScreen.gameObject.SetActive(false);
            titleScreen.gameObject.SetActive(true);

            difficultyText.gameObject.SetActive(false);
            proceedButton.gameObject.SetActive(false);
            courseText.gameObject.SetActive(false);
            explanationText.text = "";
            starText.text = "";

            admob.HideBanner();
            lastScreen = Screen.title;
        }
    }

    /*説明テキストと星の数のテキストを更新する*/
    public void SetExplainingCourseTexts(int courseNumber) {
        difficultyText.gameObject.SetActive(true);
        proceedButton.gameObject.SetActive(true);
        courseText.gameObject.SetActive(true);
        ScreenUIManager.courseNumber = courseNumber;

        courseText.text = "コース" + courseNumber.ToString();
        if (1 <= courseNumber && courseNumber <= 3) {
            explanationText.text = "君ハ脱出口ヲ\n見ツケラレルカ？";
            starText.text = "★";
        }
        else if (4 <= courseNumber && courseNumber <= 6) {
            explanationText.text = "階段ヲ使エ.";
            starText.text = "★";
        }
        else if (7 <= courseNumber && courseNumber <= 11) {
            explanationText.text = "橋ヲ使エバ\n道ハ開ク.";
            starText.text = "★★";
        }
        else if (courseNumber == 12) {
            explanationText.text = "壁ガ行ク手ヲ阻ム.";
            starText.text = "★★";
        }
        else if (courseNumber == 13) {
            explanationText.text = "鬼ガ現レル.";
            starText.text = "★";
        }
        else if (courseNumber == 14) {
            explanationText.text = "青鬼ハ巡回シテイル.";
            starText.text = "★★";
        }
        else if (courseNumber == 15) {
            explanationText.text = "壁ヲ越エタ先ニ緑鬼.";
            starText.text = "★★";
        }
        else if (courseNumber == 16) {
            explanationText.text = "緑鬼タチハ\n楽シソウニ\n散歩シテイル.";
            starText.text = "★★★";
        }
        else if (courseNumber == 17) {
            explanationText.text = "赤鬼ヲドカセ.";
            starText.text = "★★★";
        }
        else if (courseNumber == 18) {
            explanationText.text = "緑鬼ノ進撃ヲ防ゲ.";
            starText.text = "★★★★";
        }
        else if (courseNumber == 19) {
            explanationText.text = "オイデ...\n緑鬼ハ君ヲ\n迎エテクレル.";
            starText.text = "★★★★";
        }
        else if (courseNumber == 20) {
            explanationText.text = "赤鬼タチノ団欒ニ\n君モ入ロウヨ.";
            starText.text = "★★";
        }
        else if (courseNumber == 21) {
            explanationText.text = "赤鬼タチハ\n君ヲ脱出口ニ通サナイ.";
            starText.text = "★★★★";
        }
        else if (courseNumber == 22) {
            explanationText.text = "数多ノ壁ガ\n置カレテイル.";
            starText.text = "★★";
        }
        else if (courseNumber == 23) {
            explanationText.text = "赤鬼ヲオビキ寄セロ.";
            starText.text = "★★★";
        }
        else if (courseNumber == 24) {
            explanationText.text = "後ロカラ失礼.";
            starText.text = "★★";
        }
        else if (courseNumber == 25) {
            explanationText.text = "警備員ハ\n青鬼1体ダケダ.";
            starText.text = "★★";
        }
        else if (courseNumber == 26) {
            explanationText.text = "警備員ハ\n青鬼4体ダ.";
            starText.text = "★★★";
        }
        else if (courseNumber == 27) {
            explanationText.text = "白鬼ノ\n背後ニ見エル脱出口.";
            starText.text = "★★★";
        }
        else if (courseNumber == 28) {
            explanationText.text = "青鬼タチハ\n協力シテイル.";
            starText.text = "★★★";
        }
        else if (courseNumber == 29) {
            explanationText.text = "白鬼ガ行ク手ヲ塞ギ\n緑鬼ハ君ヲ追ウ.\n青鬼ハ脱出口ヲ守ル.";
            starText.text = "★★★";
        }
        else if (courseNumber == 30) {
            explanationText.text = "忘レ物ハナイカ？";
            starText.text = "★★";
        }
        else if (courseNumber == 31) {
            explanationText.text = "緑鬼トノ一騎打チ.";
            starText.text = "★★★★";
        }
        else if (courseNumber == 32) {
            explanationText.text = "道ハ限ラレテイル.";
            starText.text = "★★★";
        }
        else if (courseNumber == 33) {
            explanationText.text = "急ガバ回レ.";
            starText.text = "★★★★";
        }
        else if (courseNumber == 34) {
            explanationText.text = "青鬼タチハ\nコンビヲ組ンデイル.";
            starText.text = "★★★★";
        }
        else if (courseNumber == 35) {
            explanationText.text = "道ハ限ラレテイル.";
            starText.text = "★★★★";
        }
        else if (courseNumber == 36) {
            explanationText.text = "緑鬼ガ祀ラレテイル.";
            starText.text = "★★★★★";
        }
        else if (courseNumber == 37) {
            explanationText.text = "緑鬼タチノ連携ヲ\n崩セルカ？";
            starText.text = "★★★★★";
        }
        else if (courseNumber == 38) {
            explanationText.text = "青鬼ノ隙ヲツイテ\n橋ヲトレ.";
            starText.text = "★★★★";
        }
        else if (courseNumber == 39) {
            explanationText.text = "赤鬼ニ\n急カサレル.";
            starText.text = "★★★★";
        }
        else if (courseNumber == 40) {
            explanationText.text = "速ヤカナ行動ヲ.";
            starText.text = "★★★★";
        }
        else if (courseNumber == 41) {
            explanationText.text = "壁ハ赤鬼ニ\n守ラレテイル.";
            starText.text = "★★★";
        }
        else if (courseNumber == 42) {
            explanationText.text = "緑鬼ト赤鬼ハ\n君ノ来訪ヲ歓迎スル.";
            starText.text = "★★★★★";
        }
        else if (courseNumber == 43) {
            explanationText.text = "ドウヤッテ逃ゲル？";
            starText.text = "★★★★★";
        }
        else if (courseNumber == 44) {
            explanationText.text = "青鬼ト\n呼吸ヲ\n合ワセロ.";
            starText.text = "★★★";
        }
        else if (courseNumber == 45) {
            explanationText.text = "青鬼ト一緒ニ\n回ロウ.";
            starText.text = "★★★★";
        }
        else if (courseNumber == 46) {
            explanationText.text = "青鬼ト\n戯レルカハ\n君次第ダ.";
            starText.text = "★★★★";
        }
        else if (courseNumber == 47) {
            explanationText.text = "青鬼ト緑鬼ノ\n呼吸ヲ見ロ.";
            starText.text = "★★★★★";
        }
        else if (courseNumber == 48) {
            explanationText.text = "赤鬼ガ来ル前ニ\n逃ゲラレルカ？\nイヤ、逃ゲラレマイ.";
            starText.text = "★★★★★";
        }
        else if (courseNumber == 49) {
            explanationText.text = "鬼タチガ\nヒシメキ合ッテイル.";
            starText.text = "★★★★★";
        }
        else if (courseNumber == 50) {
            explanationText.text = "青鬼タチガ\n行列ヲ作ッテイル.";
            starText.text = "★★★★★";
        }
        else {
            courseText.text = "";
            explanationText.text = "";
            starText.text = "";
        }
    }

    /*チュートリアル画面でボタンを押すと、対応する説明文が表示される*/
    public void SetExplainingTutorialTexts(string text) {

        /*一旦すべての説明テキストを非アクティブにする*/
        rulePanel.gameObject.SetActive(false);
        howToMovePanel.gameObject.SetActive(false);
        stairPanel.gameObject.SetActive(false);
        itemStoragePanel.gameObject.SetActive(false);
        bridgePanel.gameObject.SetActive(false);
        wallPanel.gameObject.SetActive(false);
        collectPanel.gameObject.SetActive(false);
        distanceDisplayPanel.gameObject.SetActive(false);
        //minimapPanel.gameObject.SetActive(false);
        ogrePanel.gameObject.SetActive(false);

        tutorialTitleText.text = text; //説明テキストのタイトルを表示する

        if (text == "ルール") {
            rulePanel.gameObject.SetActive(true);
        }
        else if (text == "プレイヤーの動かし方") {
            howToMovePanel.gameObject.SetActive(true);
        }
        else if (text == "階段の上り下り") {
            stairPanel.gameObject.SetActive(true);
        }
        else if (text == "アイテム欄") {
            itemStoragePanel.gameObject.SetActive(true);
        }
        else if (text == "壁の設置") {
            wallPanel.gameObject.SetActive(true);
        }
        else if (text == "橋の設置") {
            bridgePanel.gameObject.SetActive(true);
        }
        else if (text == "壁や橋の回収") {
            collectPanel.gameObject.SetActive(true);
        }
        else if (text == "脱出口までの直線距離") {
            distanceDisplayPanel.gameObject.SetActive(true);
        }
        //else if (text == "マップ") {
        //    minimapPanel.gameObject.SetActive(true);
        //}
        else if (text == "鬼") {
            ogrePanel.gameObject.SetActive(true);
        }
    }

    /*実際にコースに出発する*/
    public void LeaveForCourse() {
        proceedButton.interactable = false; //複数回「行く」ボタンが押されることを防ぐ
        canClick = false; //コース選択ボタン, 戻るボタンを押せないようにする
        audioSource.PlayOneShot(courseSelecionButtonSound);
        admob.HideBanner();
        FadeManager.Instance.LoadScene("Stage", 1f, 2f, 1f);
    }

    /*コースのクリア履歴を消去する*/
    public void EraseClearData() {
        audioSource.PlayOneShot(clickSound);
        for (int i = 0; i < theNumberOfCourses; i++) {
            PlayerPrefs.SetInt("Course" + (i + 1).ToString(), 0);
        }
        clearedCourses = 0; //クリア済みのコースクリア済みのコース数を0にする
        PlayerPrefs.SetInt("showTrophy", 0); //トロフィー画面を見られるようにする
        dataErasedText.gameObject.SetActive(true);
    }

    /*コースを選択するボタンを生成する*/
    private void CreateButtons() {
        for (int i = 0; i < theNumberOfCourses; i++) {
            courseButtons[i] = Instantiate(buttonPrototype, contentRectTransform);
            courseButtons[i].gameObject.transform.GetChild(0).transform.GetChild(0).GetComponent<Text>().text = (i + 1).ToString(); //コース番号を入力
            courseButtons[i].gameObject.transform.GetChild(1).transform.gameObject.SetActive(false); //初めはチェックマークを非アクティブにしておく
        }
    }

    /*オープニング動画を再生する*/
    public void PlayOpeningMovie() {
        StopAllCoroutines(); //動画を停止させるコルーチンを停止する
        StartCoroutine(StartOpeningMovie());
    }

    /*動画をタッチする*/
    public void SkipMovie() {
        initialMovie.raycastTarget = false; //2回タッチできないようにする
        StartCoroutine(FadeOutMovie());
    }

    /*トロフィー画面を表示させる*/
    /*showTrophyという変数をローカルに保存しておき、showTrophy==0ならトロフィー画面を映し、showTrophy==1なら映さない*/
    private void DisplayTrophyScreen() {
        if (clearedCourses == 1) {
            if (PlayerPrefs.GetInt("showTrophy", 0) == 0) {
                trophyScreen.gameObject.SetActive(true);
                numberText.text = "1";
                clearText.text = "1ステージクリア";
                sentenceText.text = "脱出への道程が今始まった。";
                PlayerPrefs.SetInt("showTrophy", 1);
            }
        }
        else if (clearedCourses == 5) {
            if (PlayerPrefs.GetInt("showTrophy", 0) == 0) {
                trophyScreen.gameObject.SetActive(true);
                numberText.text = "5";
                clearText.text = "5ステージクリア";
                sentenceText.text = "さあ、どんどん進もう。";
                PlayerPrefs.SetInt("showTrophy", 1);
            }
        }
        else if (clearedCourses == 10) {
            if (PlayerPrefs.GetInt("showTrophy", 0) == 0) {
                trophyScreen.gameObject.SetActive(true);
                numberText.text = "10";
                clearText.text = "10ステージクリア";
                PlayerPrefs.SetInt("showTrophy", 1);
                sentenceText.text = "操作には慣れたかい？";
            }
        }
        else if (clearedCourses == 15) {
            if (PlayerPrefs.GetInt("showTrophy", 0) == 0) {
                trophyScreen.gameObject.SetActive(true);
                numberText.text = "15";
                clearText.text = "15ステージクリア";
                PlayerPrefs.SetInt("showTrophy", 1);
                sentenceText.text = "まだまだ道は険しい。";
                PlayerPrefs.SetInt("showTrophy", 1);
            }
        }
        else if (clearedCourses == 20) {
            if (PlayerPrefs.GetInt("showTrophy", 0) == 0) {
                trophyScreen.gameObject.SetActive(true);
                numberText.text = "20";
                clearText.text = "20ステージクリア";
                sentenceText.text = "このゲームを遊んでくれて\nありがとう。";
                PlayerPrefs.SetInt("showTrophy", 1);
            }
        }
        else if (clearedCourses == 25) {
            if (PlayerPrefs.GetInt("showTrophy", 0) == 0) {
                trophyScreen.gameObject.SetActive(true);
                numberText.text = "25";
                clearText.text = "25ステージクリア";
                sentenceText.text = "折り返し地点だ。";
                PlayerPrefs.SetInt("showTrophy", 1);
            }
        }
        else if (clearedCourses == 30) {
            if (PlayerPrefs.GetInt("showTrophy", 0) == 0) {
                trophyScreen.gameObject.SetActive(true);
                numberText.text = "30";
                clearText.text = "30ステージクリア";
                sentenceText.text = "鬼と戯れる気分は\nどうだい？";
                PlayerPrefs.SetInt("showTrophy", 1);
            }
        }
        else if (clearedCourses == 40) {
            if (PlayerPrefs.GetInt("showTrophy", 0) == 0) {
                trophyScreen.gameObject.SetActive(true);
                numberText.text = "40";
                clearText.text = "40ステージクリア";
                sentenceText.text = "脱出への光が見えてきた。";
                PlayerPrefs.SetInt("showTrophy", 1);
            }
        }
        else if (clearedCourses == 45) {
            if (PlayerPrefs.GetInt("showTrophy", 0) == 0) {
                trophyScreen.gameObject.SetActive(true);
                numberText.text = "45";
                clearText.text = "45ステージクリア";
                sentenceText.text = "あともうひと踏ん張りだ。";
                PlayerPrefs.SetInt("showTrophy", 1);
            }
        }
        else if (clearedCourses == 50) {
            if (PlayerPrefs.GetInt("showTrophy", 0) == 0) {
                trophyScreen.gameObject.SetActive(true);
                numberText.text = "50";
                clearText.text = "50ステージクリア";
                sentenceText.text = "おめでとう。\nよく成し遂げた。";
                PlayerPrefs.SetInt("showTrophy", 1);
            }
        }
        else {
            PlayerPrefs.SetInt("showTrophy", 0);
        }
    }

    /*トロフィー画面を閉じる*/
    public void CloseTrophyScreen() {
        trophyScreen.gameObject.SetActive(false);
        audioSource.PlayOneShot(bellSound);

        /*全ステージをクリアしていたらエンディングを見せる*/
        if (clearedCourses == 50) {
            StartCoroutine(ShowFinalMessages());
        }
    }

    /*エンディング画面を閉じる*/
    public void CloseEnding() {
        if (canFinishEnding) {
            BGM.time = 0f;
            BGM.Play();
            BGM.mute = false;
            finalMessageImage.gameObject.SetActive(false);
            admob.ShowBanner();

            RequestReview(); //レビューを要請する
        }
    }

    /*チュートリアル左画面のボタンを生成する*/
    private void CreateTutorialButtons() {
        GameObject button = Instantiate(tutorialButtonPrefab, tutorialContentRectTransform);
        button.transform.GetChild(0).GetComponent<Text>().text = "ルール";

        button = Instantiate(tutorialButtonPrefab, tutorialContentRectTransform);
        button.transform.GetChild(0).GetComponent<Text>().text = "プレイヤーの動かし方";

        button = Instantiate(tutorialButtonPrefab, tutorialContentRectTransform);
        button.transform.GetChild(0).GetComponent<Text>().text = "階段の上り下り";

        button = Instantiate(tutorialButtonPrefab, tutorialContentRectTransform);
        button.transform.GetChild(0).GetComponent<Text>().text = "アイテム欄";

        button = Instantiate(tutorialButtonPrefab, tutorialContentRectTransform);
        button.transform.GetChild(0).GetComponent<Text>().text = "壁の設置";

        button = Instantiate(tutorialButtonPrefab, tutorialContentRectTransform);
        button.transform.GetChild(0).GetComponent<Text>().text = "橋の設置";

        button = Instantiate(tutorialButtonPrefab, tutorialContentRectTransform);
        button.transform.GetChild(0).GetComponent<Text>().text = "壁や橋の回収";

        button = Instantiate(tutorialButtonPrefab, tutorialContentRectTransform);
        button.transform.GetChild(0).GetComponent<Text>().text = "脱出口までの直線距離";

        //button = Instantiate(tutorialButtonPrefab, tutorialContentRectTransform);
        //button.transform.GetChild(0).GetComponent<Text>().text = "マップ";

        button = Instantiate(tutorialButtonPrefab, tutorialContentRectTransform);
        button.transform.GetChild(0).GetComponent<Text>().text = "鬼";
    }

    /*SNSでアプリを共有する*/
    public void ShareWithSNS() {
        StartCoroutine(TrySharingWithSNS("Feature Image.png"));
    }

    /*ユーザーにレビューを要請する*/
    private void RequestReview() {

        #if UNITY_ANDROID
            reviewScreen.gameObject.SetActive(true);

        #elif UNITY_IPHONE
            UnityEngine.iOS.Device.RequestStoreReview();
        #endif

    }

    /*レビューをする(Android)*/
    public void WriteReview() {
        audioSource.PlayOneShot(clickSound);
        Application.OpenURL("https://play.google.com/store/apps/details?id=com.Mikoshi.Onikimi");
        CloseReviewScreen();
    }

    /*レビューをしない(Android)*/
    public void CloseReviewScreen() {
        audioSource.PlayOneShot(clickSound);
        reviewScreen.gameObject.SetActive(false);
    }

    /*オープニング動画を再生する*/
    private IEnumerator StartOpeningMovie() {
        admob.HideBanner();
        //initialMovie.GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 1f);
        BGM.mute = true; //動画が流れている間はBGMをミュートにする
        BGM.Stop(); //BGMを止める
        videoPlayer.time = 0f; //再生位置を最初に戻す
        initialMovie.gameObject.SetActive(false);
        initialMovieBackground.gameObject.SetActive(true);

        //while (videoPlayer.time == 0f) {
        //    yield return null;
        //}
        yield return new WaitForSeconds(1f); //再生位置を最初に戻す操作をしてから実際に画面が最初に戻るまでにタイムラグがある

        videoPlayer.Play();
        initialMovie.GetComponent<RawImage>().color = new Color(1f, 1f, 1f, 1f);
        initialMovie.raycastTarget = true; //タッチできるようにする
        initialMovie.gameObject.SetActive(true);
        StartCoroutine(DelayMethod((float)videoPlayer.length + 1f, () =>
        {
            videoPlayer.Pause();
            initialMovieBackground.gameObject.SetActive(false);
            //videoPlayer.time = 0f; //再生位置を最初に戻す
            BGM.time = 0f; //BGMを最初から再生する
            BGM.mute = false;
            BGM.Play();
            if (lastScreen != Screen.title) {
                admob.ShowBanner();
            }
        }));
    }

    /*動画をフェードアウトさせる*/
    private IEnumerator FadeOutMovie() {
        for (float value = 1f; value >= 0f; value -= 0.1f) {
            initialMovie.GetComponent<RawImage>().color = new Color(value, value, value, 1f);
            yield return new WaitForSeconds(0.05f);
        }
        videoPlayer.Pause();
        yield return new WaitForSeconds(1f);
        initialMovieBackground.gameObject.SetActive(false);
        //videoPlayer.time = 0f; //再生位置を最初に戻す
        BGM.time = 0f; //BGMを最初から再生する
        BGM.Play();
        BGM.mute = false;
        if (lastScreen != Screen.title) {
            admob.ShowBanner();
        }
        StopAllCoroutines(); //オープニング映像が途中で止まるのを防ぐ
    }

    /*エンディングのメッセージを流す*/
    private IEnumerator ShowFinalMessages() {

        admob.HideBanner();
        BGM.mute = true;
        BGM.Stop();
        canFinishEnding = false; //タッチしてもエンディングが終了しないようにする
        finalMessageText.text = "";
        finalMessageImage.gameObject.SetActive(true);

        yield return new WaitForSeconds(1f);
        finalMessageText.text = "あなたは無事\nすべてのステージをクリアできた。";
        yield return new WaitForSeconds(4f);
        finalMessageText.text = "";
        yield return new WaitForSeconds(0.2f);
        finalMessageText.text = "ついに\n現実世界へ戻る扉が開いた。";
        yield return new WaitForSeconds(4f);
        finalMessageText.text = "";
        yield return new WaitForSeconds(0.2f);
        finalMessageText.text = "しかし\n扉をまたいだ先にあったのは";
        yield return new WaitForSeconds(4f);
        finalMessageText.text = "";
        yield return new WaitForSeconds(4f);
        finalMessageText.text = "続く。";
        yield return new WaitForSeconds(1f);

        canFinishEnding = true; //タッチしたらエンディングを終了できるようにする
    }

    /*SNSでアプリを共有するためのコルーチン*/
    private IEnumerator TrySharingWithSNS(string imgName) {

        StringBuilder message = new StringBuilder("【スマホゲーム】\n#鬼ハ追イカケ君ハ逃ゲル\n\n", 140);
        message.Append("さあ、あなたも鬼と戯れながら密閉空間から脱出しよう。\n\n");
        if (lastScreen == Screen.courseSelection) { //トロフィー画面から
            message.Append(clearText.text).Append("！\n").Append(sentenceText.text).Append("\n\n");
        }
        message.Append("Android: play.google.com/store/apps/details?id=com.Mikoshi.Onikimi\n\n");
        message.Append("iOS: apps.apple.com/jp/app/id1529844082");

        string imgPath = "";

        /*画像のパスを指定する*/
#if UNITY_ANDROID
        imgPath = "jar:file://" + Application.dataPath + "!/assets/" + imgName;
        WWW www = new WWW(imgPath);
        yield return www;
        imgPath = Application.persistentDataPath + "/" + imgName;
        File.WriteAllBytes(imgPath, www.bytes);
# elif UNITY_IPHONE
        imgPath = Application.dataPath + "/Raw/" + imgName;
#endif

        SocialConnector.SocialConnector.Share(message.ToString(), "", imgPath);
        yield break;
    }

    private IEnumerator DelayMethod(float waitTime, Action action) {
        yield return new WaitForSeconds(waitTime);
        action();
    }
}
