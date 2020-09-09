using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*座標を表す構造体*/
public struct Point
{
    public int x, y, z; //これらの座標はUnityの座標系に準拠する

    public Point(int x, int y, int z) {
        this.x = x;
        this.y = y;
        this.z = z;
    }
}

/*プレイヤーor鬼or脱出扉が向いている方向を表す列挙型*/
public enum Direction { PositiveX, NegativeX, PositiveZ, NegativeZ }

public class StageManager : MonoBehaviour
{
    public GameObject whiteOgre, blueOgre, greenOgre, redOgre, door, bridgePrefab; // 3種の鬼、脱出用のドア, 橋
    private GameObject player;

    private PlayerController playerController;
    private UIManager uiManager;

    private AudioSource　audioSource;

    /*行けるマスをtrue、行けないマスをfalseにする*/
    /*第1要素がx座標、第2要素がy座標、第3要素がz座標を表している*/
    /*座標軸はUnityの座標軸に準拠する*/
    /*鬼が進めるマスをogreSquares, プレイヤーが進めるマスをplayerSquaresで表している*/
    public bool[,,] ogreSquares, playerSquares;

    public Dictionary<Point, GameObject> bridges; //橋を管理するディクショナリー
    private List<GameObject> ogres;
    private Point goal_square = new Point(); //脱出扉があるマス
    private float crowScreamTime; //時間管理のための変数
    private float noCrowSoundTime, crowSoundTime; //カラスの鳴き声がない時間、カラスの鳴き声がある時間
    private bool crowExit; //鬼がいるステージならtrueになる, カラスの鳴き声がある間はtrueになる

    public Point GOAL_SQUARE {
        get { return goal_square; } //goalの値を他のクラスからは変更できないようにする
    }

    private Direction goal_direction = new Direction(); //脱出扉がマスに対してどの方向についているか
    public Direction GOAL_DIRECTION {
        get { return goal_direction; }
    }

    private void Awake() {
        ogreSquares = new bool[15, 2, 15];
        playerSquares = new bool[15, 2, 15];
        crowScreamTime = 0f;

        player = GameObject.Find("Player");
        playerController = player.GetComponent<PlayerController>();
        uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
        audioSource = GetComponent<AudioSource>();

        InitializeSquares();
        bridges = new Dictionary<Point, GameObject>();
        ogres = new List<GameObject>();

        UIManager.bridgeNumber = 0;
        uiManager.firstBridge.gameObject.SetActive(false);
        uiManager.secondBridge.gameObject.SetActive(false);

        SelectCourse(ScreenUIManager.courseNumber);

        noCrowSoundTime = UnityEngine.Random.Range(2f, 5f);
        crowSoundTime = UnityEngine.Random.Range(2f, 8f);
        crowExit = false;
    }

    // Update is called once per frame
    void Update() {
        crowScreamTime += Time.deltaTime;

        //if (ogreWalkingTime > 1f && ogreExit && uiManager.IS_GOING_ON) {
        //    audioSources[0].Play();
        //    ogreWalkingTime = 0f;
        //}

        PlayCrowSoundsAtIrregularIntervals();
    }

    public void SelectCourse(int number) {
        Invoke("Course" + number.ToString(), 0f);
    }

    /*スタートとゴールがともに1階にある*/
    private void Course1() {
        InitializeDoor(11, 0, 12);
        playerController.Initialize(4, 0, 2, Direction.PositiveZ);
    }

    /*スタートとゴールがともに1階にある*/
    private void Course2() {
        InitializeDoor(9, 0, 6);
        playerController.Initialize(0, 0, 8, Direction.PositiveX);
    }

    /*スタートとゴールがともに1階にある*/
    private void Course3() {
        InitializeDoor(3, 0, 2);
        playerController.Initialize(2, 0, 12, Direction.PositiveX);
    }

    /*スタートが1階、ゴールが2階にある*/
    private void Course4() {
        InitializeDoor(3, 1, 9);
        playerController.Initialize(8, 0, 5, Direction.NegativeZ);
    }

    /*スタートが2階、ゴールが1階にある*/
    private void Course5() {
        InitializeDoor(6, 0, 6);
        playerController.Initialize(7, 1, 5, Direction.PositiveZ);
    }

    /*スタートもゴールも2階にある*/
    private void Course6() {
        InitializeDoor(1, 1, 4);
        playerController.Initialize(13, 1, 9, Direction.NegativeZ);
    }

    /*スタートもゴールも2階にある, 橋を使う*/
    private void Course7() {
        InitializeDoor(11, 1, 11);
        CreateOneBridge(9, 1, 8, Direction.PositiveZ);
        playerController.Initialize(7, 1, 7, Direction.PositiveX);
    }

    /*スタートもゴールも2階にある, 橋を使う*/
    private void Course8() {
        InitializeDoor(3, 1, 3);
        CreateOneBridge(8, 1, 13, Direction.PositiveX);
        playerController.Initialize(9, 1, 13, Direction.PositiveX);
    }

    /*スタートが1階、ゴールが2階, 橋を使う, 橋を1本手持ち*/
    private void Course9() {
        UIManager.bridgeNumber = 1;
        uiManager.firstBridge.gameObject.SetActive(true);
        InitializeDoor(5, 1, 5);
        playerController.Initialize(4, 0, 2, Direction.PositiveZ);
    }

    /*スタートが1階、ゴールが2階, 橋を使う, 橋を1本手持ち*/
    private void Course10() {
        UIManager.bridgeNumber = 1;
        uiManager.firstBridge.gameObject.SetActive(true);
        InitializeDoor(9, 1, 11);
        playerController.Initialize(10, 0, 4, Direction.NegativeX);
    }

    /*スタートが2階、ゴールが1階, 橋を使う*/
    private void Course11() {
        InitializeDoor(14, 0, 3);
        CreateOneBridge(6, 1, 9, Direction.PositiveX);
        playerController.Initialize(9, 1, 9, Direction.NegativeX);
    }

    /*スタートもゴールも1階, 壁を使う*/
    private void Course12() {
        InitializeDoor(12, 0, 3);
        CreateOneBridge(9, 0, 10, Direction.PositiveZ);
        CreateOneBridge(14, 0, 9, Direction.PositiveX);
        CreateOneBridge(9, 0, 8, Direction.PositiveZ);
        CreateOneBridge(7, 0, 4, Direction.PositiveZ);
        CreateOneBridge(14, 0, 1, Direction.PositiveX);
        playerController.Initialize(4, 0, 10, Direction.PositiveX);
    }

    /*スタートもゴールも1階, 緑鬼が1体いる*/
    private void Course13() {
        InitializeDoor(3, 0, 4);
        CreateOneOgre(greenOgre, 6, 0, 9, Direction.NegativeZ);
        playerController.Initialize(11, 0, 8, Direction.NegativeX);
    }

    /*スタートもゴールも2階、青鬼が2体いる、青鬼の後をつけてゴールにたどり着く*/
    private void Course14() {
        InitializeDoor(7, 1, 5);
        CreateOneBridge(5, 1, 6, Direction.PositiveZ);
        CreateOneBridge(6, 1, 5, Direction.PositiveX);

        Behaviour[] b = new Behaviour[12];
        for (int i = 0; i < 12; i++) {
            if (i == 2 || i == 5 || i == 8 || i == 11) {
                b[i] = Behaviour.TurnToTheRight;
            }
            else {
                b[i] = Behaviour.GoForward;
            }
        }
        CreateOneOgre(blueOgre, 5, 1, 7, Direction.PositiveX, b);
        CreateOneOgre(blueOgre, 7, 1, 5, Direction.NegativeX, b);

        playerController.Initialize(3, 1, 9, Direction.NegativeZ);
    }

    /*スタートもゴールも1階、緑鬼が3体いる、壁を取り払う*/
    private void Course15() {
        InitializeDoor(2, 0, 12);

        CreateOneBridge(9, 0, 14, Direction.PositiveZ);
        CreateOneBridge(9, 0, 10, Direction.PositiveZ);
        CreateOneBridge(9, 0, 8, Direction.PositiveZ);
        CreateOneBridge(5, 0, 6, Direction.PositiveZ);
        CreateOneBridge(0, 0, 5, Direction.PositiveX);
        CreateOneBridge(2, 0, 5, Direction.PositiveX);
        CreateOneBridge(4, 0, 5, Direction.PositiveX);

        CreateOneOgre(greenOgre, 0, 0, 14, Direction.NegativeZ);
        CreateOneOgre(greenOgre, 6, 0, 11, Direction.PositiveZ);
        CreateOneOgre(greenOgre, 3, 0, 6, Direction.PositiveZ);

        playerController.Initialize(13, 1, 7, Direction.NegativeX);
    }

    /*スタートもゴールも1階、白鬼が1体、緑鬼が3体、赤鬼が1体いる、緑鬼たちは一定の領域内しか移動できない*/
    private void Course16() {
        InitializeDoor(8, 0, 0);
        CreateOneOgre(whiteOgre, 0, 0, 5, Direction.PositiveZ);
        CreateOneOgre(greenOgre, 11, 0, 10, Direction.NegativeZ);
        CreateOneOgre(greenOgre, 5, 0, 3, Direction.PositiveZ);
        CreateOneOgre(greenOgre, 10, 0, 4, Direction.PositiveZ);
        CreateOneOgre(redOgre, 14, 0, 5, Direction.PositiveZ);

        /*15箇所の鬼だけの行き止まり*/
        ogreSquares[10, 0, 13] = false;
        ogreSquares[9, 0, 10] = false;
        ogreSquares[13, 0, 10] = false;
        ogreSquares[2, 0, 9] = false;
        ogreSquares[4, 0, 9] = false;
        ogreSquares[1, 0, 8] = false;
        ogreSquares[5, 0, 8] = false;
        ogreSquares[9, 0, 8] = false;
        ogreSquares[1, 0, 6] = false;
        ogreSquares[7, 0, 4] = false;
        ogreSquares[13, 0, 4] = false;
        ogreSquares[7, 0, 2] = false;
        ogreSquares[13, 0, 2] = false;
        ogreSquares[2, 0, 1] = false;
        ogreSquares[6, 0, 1] = false;

        playerController.Initialize(8, 0, 14, Direction.NegativeZ);
    }

    /*スタートもゴールも1階、赤鬼が2体いてゴールを取り囲んでいる*/
    private void Course17() {
        InitializeDoor(0, 0, 14);
        CreateOneOgre(redOgre, 1, 0, 14, Direction.PositiveX);
        CreateOneOgre(redOgre, 0, 0, 13, Direction.NegativeZ);
        playerController.Initialize(6, 0, 0, Direction.PositiveZ);
    }

    /*スタートが1階、ゴールが2階、白鬼が1体、緑鬼が2体いる、橋を使う、急いで緑鬼の進撃を防ぐ*/
    private void Course18() {
        InitializeDoor(5, 1, 9);
        CreateOneBridge(14, 0, 1, Direction.PositiveX);
        CreateOneOgre(whiteOgre, 14, 0, 2, Direction.NegativeZ);
        CreateOneOgre(greenOgre, 8, 0, 0, Direction.PositiveX);
        CreateOneOgre(greenOgre, 6, 1, 13, Direction.NegativeX);
        playerController.Initialize(11, 0, 0, Direction.NegativeX);
    }

    /*スタートもゴールも2階、白鬼が4体、青鬼が1体、緑鬼が1体、赤鬼が1体、ゴールと同じマスにいる緑鬼をうまくどかす*/
    private void Course19() {
        InitializeDoor(3, 1, 5);
        CreateOneBridge(1, 1, 8, Direction.PositiveZ);
        CreateOneBridge(2, 1, 9, Direction.PositiveX);
        CreateOneOgre(whiteOgre, 7, 1, 13, Direction.NegativeX);
        CreateOneOgre(whiteOgre, 2, 0, 12, Direction.NegativeZ);
        CreateOneOgre(whiteOgre, 5, 1, 7, Direction.PositiveZ);
        CreateOneOgre(whiteOgre, 1, 1, 5, Direction.PositiveZ);

        Behaviour[] b = new Behaviour[8];
        for (int i = 0; i < 8; i++) {
            if (i == 2 || i == 3 || i == 6 || i == 7) {
                b[i] = Behaviour.TurnToTheLeft;
            }
            else {
                b[i] = Behaviour.GoForward;
            }
        }
        CreateOneOgre(blueOgre, 7, 1, 9, Direction.PositiveX, b);

        CreateOneOgre(greenOgre, 3, 1, 5, Direction.PositiveX);
        CreateOneOgre(redOgre, 5, 1, 11, Direction.NegativeX);
        playerController.Initialize(1, 1, 7, Direction.PositiveZ);
    }

    /*スタートもゴールも1階、赤鬼が4体いる*/
    private void Course20() {
        InitializeDoor(6, 0, 4);
        CreateOneOgre(redOgre, 6, 0, 5, Direction.PositiveZ);
        CreateOneOgre(redOgre, 6, 0, 3, Direction.NegativeZ);
        CreateOneOgre(redOgre, 7, 0, 4, Direction.PositiveX);
        CreateOneOgre(redOgre, 5, 0, 4, Direction.NegativeX);
        playerController.Initialize(2, 0, 8, Direction.NegativeZ);
    }

    /*スタートもゴールも1階、赤鬼が4体いる*/
    private void Course21() {
        InitializeDoor(2, 0, 12);
        CreateOneOgre(redOgre, 8, 0, 14, Direction.NegativeZ);
        CreateOneOgre(redOgre, 6, 0, 12, Direction.NegativeZ);
        CreateOneOgre(redOgre, 4, 0, 12, Direction.PositiveX);
        playerController.Initialize(4, 0, 6, Direction.PositiveZ);
    }

    /*スタートもゴールも1階、白鬼が3体いる、壁が11本ある*/
    private void Course22() {
        InitializeDoor(14, 0, 0);
        CreateOneOgre(whiteOgre, 9, 0, 14, Direction.PositiveX);
        CreateOneOgre(whiteOgre, 14, 0, 5, Direction.PositiveZ);
        CreateOneOgre(whiteOgre, 8, 0, 0, Direction.PositiveZ);
        
        for (int x = 7; x <= 9; x++) {
            CreateOneBridge(x, 0, 8, Direction.PositiveZ);
            CreateOneBridge(x, 0, 10, Direction.PositiveZ);
        }
        CreateOneBridge(4, 0, 9, Direction.PositiveX);
        CreateOneBridge(5, 0, 8, Direction.PositiveZ);
        CreateOneBridge(7, 0, 4, Direction.PositiveZ);
        CreateOneBridge(9, 0, 6, Direction.PositiveZ);
        CreateOneBridge(9, 0, 4, Direction.PositiveZ);
        CreateOneBridge(12, 0, 5, Direction.PositiveX);
        CreateOneBridge(13, 0, 4, Direction.PositiveZ);
        CreateOneBridge(11, 0, 2, Direction.PositiveZ);
        CreateOneBridge(13, 0, 2, Direction.PositiveZ);
        CreateOneBridge(14, 0, 1, Direction.PositiveX);

        playerController.Initialize(14, 0, 14, Direction.NegativeZ);
    }

    /*スタートが1階、ゴールが2階、緑鬼が3体、赤鬼が2体いる、橋を使う*/
    private void Course23() {
        InitializeDoor(10, 1, 7);
        CreateOneBridge(6, 1, 11, Direction.PositiveX);
        CreateOneOgre(greenOgre, 14, 0, 14, Direction.NegativeX);
        CreateOneOgre(greenOgre, 6, 0, 8, Direction.PositiveZ);
        CreateOneOgre(greenOgre, 12, 0, 8, Direction.PositiveZ);
        CreateOneOgre(redOgre, 9, 1, 7, Direction.NegativeX);
        CreateOneOgre(redOgre, 11, 1, 7, Direction.PositiveX);
        playerController.Initialize(0, 0, 14, Direction.PositiveX);
    }

    /*スタートは2階、ゴールは1階、白鬼が1体、青鬼が1体、緑鬼が4体いる*/
    private void Course24() {
        InitializeDoor(13, 1, 11);
        CreateOneOgre(whiteOgre, 8, 0, 8, Direction.NegativeX);
        CreateOneOgre(greenOgre, 10, 0, 10, Direction.NegativeX);
        CreateOneOgre(greenOgre, 4, 0, 6, Direction.NegativeZ);
        CreateOneOgre(greenOgre, 14, 0, 5, Direction.PositiveZ);
        CreateOneOgre(greenOgre, 13, 1, 1, Direction.NegativeX);

        Behaviour[] b = new Behaviour[16];
        for (int i = 0; i < 16; i++) {
            if (i == 3 || i == 4 || i == 11 || i == 12) {
                b[i] = Behaviour.TurnToTheLeft;
            }
            else {
                b[i] = Behaviour.GoForward;
            }
        }
        CreateOneOgre(blueOgre, 8, 0, 14, Direction.PositiveX, b);

        playerController.Initialize(10, 1, 1, Direction.NegativeX);
    }

    /*スタートもゴールも2階、橋を1本手持ち、青鬼が1体いる*/
    private void Course25() {
        UIManager.bridgeNumber = 1;
        uiManager.firstBridge.gameObject.SetActive(true);
        InitializeDoor(9, 1, 9);
        Behaviour[] ogreBehaviours = new Behaviour[8];
        for (int i = 0; i < 8; i++) {
            if (i == 2 || i == 3 || i == 6 || i == 7) {
                ogreBehaviours[i] = Behaviour.TurnToTheRight;
            }
            else {
                ogreBehaviours[i] = Behaviour.GoForward;
            }
        }
        CreateOneOgre(blueOgre, 7, 1, 9, Direction.PositiveX, ogreBehaviours); ;
        playerController.Initialize(7, 1, 5, Direction.PositiveZ);
    }

    /*スタートが2階、ゴールが1階、青鬼が4体いる*/
    private void Course26() {
        InitializeDoor(3, 0, 7);

        Behaviour[] ogreBehaviours = new Behaviour[12];
        for (int i = 0; i < 12; i++) {
            if (i == 2 || i == 5 || i == 8 || i == 11) {
                ogreBehaviours[i] = Behaviour.TurnToTheRight;
            }
            else {
                ogreBehaviours[i] = Behaviour.GoForward;
            }
        }
        CreateOneOgre(blueOgre, 2, 0, 6, Direction.PositiveZ, ogreBehaviours);
        CreateOneOgre(blueOgre, 2, 0, 8, Direction.PositiveX, ogreBehaviours);
        CreateOneOgre(blueOgre, 4, 0, 8, Direction.NegativeZ, ogreBehaviours);
        CreateOneOgre(blueOgre, 4, 0, 6, Direction.NegativeX, ogreBehaviours);

        playerController.Initialize(1, 1, 5, Direction.PositiveZ);
    }

    /*スタートもゴールも1階、白鬼が2体、青鬼が3体、赤鬼が1体いる*/
    private void Course27() {
        InitializeDoor(9, 0, 4);
        CreateOneOgre(whiteOgre, 7, 0, 4, Direction.PositiveX);
        CreateOneOgre(whiteOgre, 14, 0, 0, Direction.NegativeX);

        Behaviour[] b = new Behaviour[12];
        for (int i = 0; i < 12; i++) {
            if (i == 1 || i == 4 || i == 7 || i == 10) {
                b[i] = Behaviour.TurnToTheLeft;
            }
            else {
                b[i] = Behaviour.GoForward;
            }
        }
        CreateOneOgre(blueOgre, 11, 0, 10, Direction.NegativeX, b);
        CreateOneOgre(blueOgre, 3, 0, 8, Direction.NegativeX, b);
        CreateOneOgre(blueOgre, 13, 0, 4, Direction.NegativeX, b);

        CreateOneOgre(redOgre, 9, 0, 8, Direction.NegativeX);
        playerController.Initialize(2, 0, 4, Direction.PositiveX);
    }

    /*スタートもゴールも1階、白鬼が2体、青鬼が6体いる、うまくタイミングを見て青鬼を躱す*/
    private void Course28() {
        InitializeDoor(4, 0, 4);
        CreateOneOgre(whiteOgre, 14, 0, 10, Direction.NegativeZ);
        CreateOneOgre(whiteOgre, 14, 0, 0, Direction.PositiveZ);

        Behaviour[] b1 = new Behaviour[12];
        Behaviour[] b2 = new Behaviour[12];
        Behaviour[] b3 = new Behaviour[16];
        Behaviour[] b4 = new Behaviour[12];

        for (int i = 0; i < 12; i++) {
            if (i == 2 || i == 5 || i == 8 || i == 11) {
                b1[i] = Behaviour.TurnToTheRight;
                b2[i] = Behaviour.TurnToTheLeft;
            }
            else {
                b1[i] = Behaviour.GoForward;
                b2[i] = Behaviour.GoForward;
            }
        }

        for (int i = 0; i < 16; i++) {
            if (i == 4 || i == 7 || i == 12 || i == 15) {
                b3[i] = Behaviour.TurnToTheLeft;
            }
            else {
                b3[i] = Behaviour.GoForward;
            }
        }

        for (int i = 0; i < 12; i++) {
            if (i == 1 || i == 4 || i == 7 || i == 10) {
                b4[i] = Behaviour.TurnToTheRight;
            }
            else {
                b4[i] = Behaviour.GoForward;
            }
        }

        CreateOneOgre(blueOgre, 12, 0, 2, Direction.PositiveZ, b1);
        CreateOneOgre(blueOgre, 10, 0, 6, Direction.NegativeZ, b3);
        CreateOneOgre(blueOgre, 8, 0, 6, Direction.PositiveX, b1);
        CreateOneOgre(blueOgre, 4, 0, 4, Direction.PositiveX, b2);
        CreateOneOgre(blueOgre, 2, 0, 4, Direction.PositiveX, b2);
        CreateOneOgre(blueOgre, 3, 0, 2, Direction.NegativeX, b4);

        playerController.Initialize(14, 0, 8, Direction.NegativeZ);
    }

    /*スタートは1階、ゴールは2階、白鬼が6体、青鬼が1体、緑鬼が1体、緑鬼を密閉空間で躱した後、青鬼を躱す*/
    private void Course29() {
        InitializeDoor(13, 1, 9);
        CreateOneOgre(whiteOgre, 13, 1, 11, Direction.PositiveZ);
        CreateOneOgre(whiteOgre, 4, 0, 6, Direction.NegativeZ);
        CreateOneOgre(whiteOgre, 3, 0, 4, Direction.PositiveX);
        CreateOneOgre(whiteOgre, 3, 0, 2, Direction.PositiveX);
        CreateOneOgre(whiteOgre, 7, 0, 2, Direction.NegativeX);
        CreateOneOgre(whiteOgre, 6, 0, 1, Direction.PositiveZ);

        Behaviour[] b = new Behaviour[12];
        for (int i = 0; i < 12; i++) {
            if (i == 4 || i == 5 || i == 10 || i == 11) {
                b[i] = Behaviour.TurnToTheRight;
            }
            else {
                b[i] = Behaviour.GoForward;
            }
        }
        CreateOneOgre(blueOgre, 13, 1, 5, Direction.PositiveZ, b);

        CreateOneOgre(greenOgre, 8, 0, 4, Direction.NegativeX);
        playerController.Initialize(4, 0, 4, Direction.PositiveX);
    }

    /*スタートは1階、ゴールは2階、青鬼が1体いる、プレイヤーは壁に囲まれた状態でスタートする、青鬼がゴールを警護している*/
    private void Course30() {
        InitializeDoor(9, 1, 11);
        CreateOneBridge(2, 0, 5, Direction.PositiveX);
        CreateOneBridge(4, 0, 5, Direction.PositiveX);
        CreateOneBridge(6, 0, 5, Direction.PositiveX);
        CreateOneBridge(7, 0, 4, Direction.PositiveZ);
        CreateOneBridge(7, 0, 2, Direction.PositiveZ);
        CreateOneBridge(2, 0, 1, Direction.PositiveX);
        CreateOneBridge(6, 0, 1, Direction.PositiveX);

        Behaviour[] b = new Behaviour[14];
        for (int i = 0; i < 14; i++) {
            if (i == 2 || i == 5 || i == 6) {
                b[i] = Behaviour.TurnToTheRight;
            }
            else if (i == 9 || i == 12 || i == 13) {
                b[i] = Behaviour.TurnToTheLeft;
            }
            else {
                b[i] = Behaviour.GoForward;
            }
        }
        CreateOneOgre(blueOgre, 9, 0, 13, Direction.NegativeZ, b);

        playerController.Initialize(4, 0, 2, Direction.PositiveZ);
    }

    /*スタートもゴールも2階、白鬼が2体、緑鬼が1体、緑鬼の配置をうまく変えてクリアする*/
    private void Course31() {
        CreateOneBridge(6, 1, 5, Direction.PositiveX);
        CreateOneBridge(2, 1, 3, Direction.PositiveX);
        InitializeDoor(1, 1, 9);
        CreateOneOgre(whiteOgre, 1, 1, 11, Direction.PositiveZ);
        CreateOneOgre(whiteOgre, 3, 1, 9, Direction.NegativeZ);
        CreateOneOgre(greenOgre, 1, 1, 7, Direction.PositiveX);
        playerController.Initialize(9, 1, 7, Direction.NegativeX);
    }

    /*スタートが1階、ゴールが2階、白鬼が10体いる、橋を使う*/
    private void Course32() {
        InitializeDoor(11, 1, 5);
        CreateOneBridge(1, 1, 6, Direction.PositiveZ);
        CreateOneOgre(whiteOgre, 5, 1, 11, Direction.NegativeX);
        CreateOneOgre(whiteOgre, 5, 1, 9, Direction.PositiveX);
        CreateOneOgre(whiteOgre, 8, 1, 7, Direction.PositiveX);
        CreateOneOgre(whiteOgre, 13, 1, 7, Direction.PositiveZ);
        CreateOneOgre(whiteOgre, 5, 1, 5, Direction.PositiveZ);
        CreateOneOgre(whiteOgre, 9, 1, 5, Direction.PositiveZ);
        CreateOneOgre(whiteOgre, 6, 0, 4, Direction.PositiveX);
        CreateOneOgre(whiteOgre, 9, 1, 2, Direction.NegativeX);
        CreateOneOgre(whiteOgre, 14, 0, 2, Direction.NegativeZ);
        CreateOneOgre(whiteOgre, 5, 1, 1, Direction.NegativeX);
        playerController.Initialize(14, 0, 0, Direction.NegativeX);
    }

    /*スタートもゴールも1階、白鬼が4体、青鬼が1体、赤鬼が1体、壁が6個、壁がたくさんあるのでいくつかは捨てていく、橋を使う、赤鬼の位置をずらしてゴールにたどり着く*/
    private void Course33() {
        InitializeDoor(8, 0, 8);
        CreateOneBridge(9, 0, 6, Direction.PositiveZ);
        CreateOneBridge(8, 0, 5, Direction.PositiveX);
        CreateOneBridge(10, 0, 5, Direction.PositiveX);
        CreateOneBridge(12, 0, 5, Direction.PositiveX);
        CreateOneBridge(9, 0, 4, Direction.PositiveZ);
        CreateOneBridge(11, 0, 2, Direction.PositiveZ);
        CreateOneOgre(whiteOgre, 10, 0, 14, Direction.NegativeZ);
        CreateOneOgre(whiteOgre, 8, 0, 10, Direction.PositiveX);
        CreateOneOgre(whiteOgre, 7, 0, 4, Direction.PositiveX);
        CreateOneOgre(whiteOgre, 14, 0, 1, Direction.PositiveZ);

        Behaviour[] b = new Behaviour[12];
        for (int i = 0; i < 12; i++) {
            if (i == 2 || i == 5 || i == 8 || i == 11) {
                b[i] = Behaviour.TurnToTheRight;
            }
            else {
                b[i] = Behaviour.GoForward;
            }
        }
        CreateOneOgre(blueOgre, 4, 0, 10, Direction.PositiveX, b);

        CreateOneOgre(redOgre, 9, 0, 8, Direction.NegativeX);
        playerController.Initialize(8, 0, 6, Direction.NegativeZ);
    }

    /*スタートもゴールも2階、白鬼が4体、青鬼が6体、赤鬼が1体いる、2体の青鬼のグルグルを躱す*/
    private void Course34() {
        InitializeDoor(13, 1, 1);

        CreateOneOgre(whiteOgre, 8, 0, 14, Direction.PositiveX);
        CreateOneOgre(whiteOgre, 7, 0, 4, Direction.PositiveX);
        CreateOneOgre(whiteOgre, 14, 0, 1, Direction.PositiveZ);
        CreateOneOgre(whiteOgre, 2, 0, 0, Direction.PositiveZ);

        Behaviour[] b = new Behaviour[8];
        for (int i = 0; i < 8; i++) {
            if (i == 1 || i == 3 || i == 5 || i == 7) {
                b[i] = Behaviour.TurnToTheRight;
            }
            else {
                b[i] = Behaviour.GoForward;
            }
        }
        CreateOneOgre(blueOgre, 3, 0, 7, Direction.NegativeZ, b);
        CreateOneOgre(blueOgre, 4, 0, 8, Direction.NegativeZ, b);

        for (int i = 0; i < 8; i++) {
            if (i == 1 || i == 3 || i == 5 || i == 7) {
                b[i] = Behaviour.TurnToTheLeft;
            }
            else {
                b[i] = Behaviour.GoForward;
            }
        }
        CreateOneOgre(blueOgre, 12, 0, 9, Direction.PositiveZ, b);
        CreateOneOgre(blueOgre, 11, 0, 8, Direction.PositiveZ, b);
        CreateOneOgre(blueOgre, 5, 0, 3, Direction.PositiveZ, b);
        CreateOneOgre(blueOgre, 6, 0, 2, Direction.PositiveZ, b);

        CreateOneOgre(redOgre, 14, 0, 9, Direction.NegativeZ);

        playerController.Initialize(5, 1, 7, Direction.NegativeX);
    }

    /*スタートもゴールも1階、白鬼が10体いる、橋を使う、橋を2本手持ち*/
    private void Course35() {
        UIManager.bridgeNumber = 2;
        uiManager.firstBridge.gameObject.SetActive(true);
        uiManager.secondBridge.gameObject.SetActive(true);
        InitializeDoor(4, 0, 7);
        CreateOneOgre(whiteOgre, 8, 0, 14, Direction.NegativeX);
        CreateOneOgre(whiteOgre, 5, 0, 12, Direction.NegativeX);
        CreateOneOgre(whiteOgre, 9, 1, 11, Direction.NegativeZ);
        CreateOneOgre(whiteOgre, 13, 1, 11, Direction.NegativeX);
        CreateOneOgre(whiteOgre, 1, 1, 9, Direction.PositiveX);
        CreateOneOgre(whiteOgre, 14, 0, 9, Direction.NegativeZ);
        CreateOneOgre(whiteOgre, 2, 0, 8, Direction.NegativeX);
        CreateOneOgre(whiteOgre, 2, 0, 6, Direction.NegativeX);
        CreateOneOgre(whiteOgre, 4, 0, 6, Direction.NegativeZ);
        CreateOneOgre(whiteOgre, 8, 0, 4, Direction.PositiveX);
        playerController.Initialize(0, 0, 14, Direction.NegativeZ);
    }

    /*スタートもゴールも2階、青鬼が1体、緑鬼が2体、赤鬼が3体いる、ゴールに緑鬼が立っていてそれを赤鬼と青鬼が取り囲んでいる*/
    private void Course36() {
        InitializeDoor(5, 1, 9);

        CreateOneBridge(10, 1, 13, Direction.PositiveX);
        CreateOneBridge(11, 1, 12, Direction.PositiveZ);

        Behaviour[] b = new Behaviour[8];
        for (int i = 0; i < 8; i++) {
            if (i == 2 || i == 3 || i == 6 || i == 7) {
                b[i] = Behaviour.TurnToTheLeft;
            }
            else {
                b[i] = Behaviour.GoForward;
            }
        }
        CreateOneOgre(blueOgre, 9, 1, 9, Direction.NegativeX, b);

        CreateOneOgre(greenOgre, 7, 1, 11, Direction.PositiveX);
        CreateOneOgre(greenOgre, 5, 1, 9, Direction.PositiveZ);
        CreateOneOgre(redOgre, 5, 1, 11, Direction.NegativeZ);
        CreateOneOgre(redOgre, 3, 1, 9, Direction.PositiveX);
        CreateOneOgre(redOgre, 5, 1, 7, Direction.PositiveZ);

        playerController.Initialize(13, 1, 11, Direction.NegativeX);
    }

    /*スタートもゴールも1階、白鬼が5体、青鬼が2体、橋を使う、壁で青鬼の進路をうまくふさぐ*/
    private void Course37() {
        InitializeDoor(6, 0, 8);

        CreateOneBridge(11, 1, 12, Direction.PositiveZ);
        CreateOneBridge(12, 1, 11, Direction.PositiveX);
        CreateOneBridge(6, 0, 14, Direction.PositiveZ);
        CreateOneBridge(8, 0, 13, Direction.PositiveX);

        CreateOneOgre(whiteOgre, 9, 1, 13, Direction.PositiveX);
        CreateOneOgre(whiteOgre, 9, 1, 11, Direction.PositiveX);
        CreateOneOgre(whiteOgre, 14, 0, 10, Direction.PositiveZ);
        CreateOneOgre(whiteOgre, 13, 1, 9, Direction.PositiveZ);
        CreateOneOgre(whiteOgre, 10, 0, 13, Direction.PositiveZ);

        CreateOneOgre(greenOgre, 12, 0, 14, Direction.PositiveX);
        CreateOneOgre(greenOgre, 10, 0, 14, Direction.NegativeX);
        playerController.Initialize(11, 1, 11, Direction.PositiveZ);
    }

    /*スタートが1階、ゴールが2階、青鬼が4体、赤鬼が2体、橋を使う、うまく橋を取り払ってから置く*/
    private void Course38() {
        InitializeDoor(7, 1, 9);
        CreateOneOgre(redOgre, 6, 0, 4, Direction.NegativeZ);
        CreateOneOgre(redOgre, 12, 0, 6, Direction.NegativeX);

        CreateOneBridge(6, 1, 11, Direction.PositiveX);
        CreateOneBridge(5, 1, 10, Direction.PositiveZ);
        CreateOneBridge(5, 1, 8, Direction.PositiveZ);
        CreateOneBridge(9, 1, 10, Direction.PositiveZ);
        CreateOneBridge(9, 1, 8, Direction.PositiveZ);

        Behaviour[] b1 = new Behaviour[20];
        for (int i = 0; i < 20; i++) {
            if (i == 4 || i == 9 || i == 14 || i == 19) {
                b1[i] = Behaviour.TurnToTheRight;
            }
            else {
                b1[i] = Behaviour.GoForward;
            }
        }
        CreateOneOgre(blueOgre, 5, 1, 7, Direction.PositiveZ, b1);
        CreateOneOgre(blueOgre, 5, 1, 11, Direction.PositiveX, b1);
        CreateOneOgre(blueOgre, 9, 1, 11, Direction.NegativeZ, b1);
        CreateOneOgre(blueOgre, 9, 1, 7, Direction.NegativeX, b1);

        playerController.Initialize(9, 1, 1, Direction.PositiveZ);
    }

    /*スタートもゴールも1階、青鬼が7体、赤鬼が3体いる、青鬼で進路が限定されており、赤鬼が追いかけてくる*/
    private void Course39() {
        InitializeDoor(6, 0, 2);
        CreateOneOgre(whiteOgre, 2, 0, 10, Direction.NegativeZ);
        CreateOneOgre(whiteOgre, 6, 0, 10, Direction.NegativeZ);
        CreateOneOgre(whiteOgre, 0, 0, 8, Direction.PositiveX);
        CreateOneOgre(whiteOgre, 10, 0, 8, Direction.NegativeX);
        CreateOneOgre(whiteOgre, 0, 0, 6, Direction.PositiveX);
        CreateOneOgre(whiteOgre, 6, 0, 4, Direction.NegativeX);
        CreateOneOgre(whiteOgre, 2, 0, 0, Direction.PositiveZ);
        CreateOneOgre(redOgre, 9, 0, 8, Direction.NegativeX);
        CreateOneOgre(redOgre, 2, 0, 6, Direction.PositiveZ);
        CreateOneOgre(redOgre, 5, 0, 3, Direction.NegativeZ);
        playerController.Initialize(6, 0, 8, Direction.PositiveX);
    }

    /*スタートもゴールも2階、白鬼が10体、緑鬼が1体、赤鬼が1体いる、スタート時に緑鬼からうまく逃げて最後は赤鬼の配置をうまく変えてクリアする*/
    private void Course40() {
        InitializeDoor(11, 1, 4);
        CreateOneBridge(8, 1, 8, Direction.PositiveZ);
        CreateOneOgre(whiteOgre, 13, 1, 7, Direction.NegativeX);
        CreateOneOgre(whiteOgre, 11, 0, 6, Direction.PositiveZ);
        CreateOneOgre(whiteOgre, 10, 0, 3, Direction.NegativeX);
        CreateOneOgre(whiteOgre, 10, 1, 3, Direction.NegativeX);
        CreateOneOgre(whiteOgre, 12, 0, 3, Direction.PositiveX);
        CreateOneOgre(whiteOgre, 12, 1, 3, Direction.PositiveX);
        CreateOneOgre(greenOgre, 5, 1, 7, Direction.PositiveX);
        CreateOneOgre(redOgre, 11, 1, 5, Direction.NegativeZ);
        playerController.Initialize(9, 1, 7, Direction.NegativeX);
    }

    /*スタートが2階、ゴールが1階、白鬼が6体、緑鬼が1体、赤鬼が2体、橋を使う、赤鬼に囲まれた橋を取得する*/
    private void Course41() {
        InitializeDoor(0, 0, 5);
        CreateOneBridge(3, 0, 4, Direction.PositiveZ);
        CreateOneOgre(whiteOgre, 1, 1, 9, Direction.PositiveZ);
        CreateOneOgre(whiteOgre, 0, 0, 6, Direction.PositiveX);
        CreateOneOgre(whiteOgre, 7, 1, 5, Direction.NegativeZ);
        CreateOneOgre(whiteOgre, 1, 0, 0, Direction.PositiveX);
        CreateOneOgre(whiteOgre, 7, 1, 7, Direction.PositiveZ);
        CreateOneOgre(greenOgre, 5, 1, 13, Direction.NegativeX);
        CreateOneOgre(redOgre, 2, 0, 4, Direction.NegativeX);
        CreateOneOgre(redOgre, 4, 0, 4, Direction.PositiveX);

        Behaviour[] b = new Behaviour[8];
        for (int i = 0; i < 8; i++) {
            if (i == 2 || i == 3 || i == 6 || i == 7) {
                b[i] = Behaviour.TurnToTheLeft;
            }
            else {
                b[i] = Behaviour.GoForward;
            }
        }
        CreateOneOgre(blueOgre, 7, 1, 11, Direction.PositiveX, b);

        playerController.Initialize(3, 1, 9, Direction.PositiveZ);
    }

    /*スタートは2階、ゴールは1階、白鬼が7体、緑鬼が1体、赤鬼が1体いる、橋を使う、プレイヤーが必ず通らなければいけないところに
    緑鬼と赤鬼がいる、緑鬼はマスをfalseにされることで同じ領域から出られない*/
    private void Course42() {
        InitializeDoor(10, 0, 4);
        CreateOneBridge(12, 1, 5, Direction.PositiveX);
        CreateOneOgre(whiteOgre, 9, 1, 11, Direction.NegativeZ);
        CreateOneOgre(whiteOgre, 13, 1, 11, Direction.NegativeZ);
        CreateOneOgre(whiteOgre, 7, 1, 7, Direction.PositiveX);
        CreateOneOgre(whiteOgre, 11, 0, 6, Direction.PositiveX);
        CreateOneOgre(whiteOgre, 7, 0, 4, Direction.NegativeX);
        CreateOneOgre(whiteOgre, 8, 0, 2, Direction.NegativeZ);
        CreateOneOgre(whiteOgre, 1, 1, 1, Direction.PositiveZ);

        ogreSquares[9, 0, 14] = false;
        ogreSquares[9, 0, 10] = false;
        ogreSquares[9, 0, 8] = false;
        ogreSquares[13, 0, 4] = false;
        CreateOneOgre(greenOgre, 11, 0, 8, Direction.PositiveZ);
        CreateOneOgre(redOgre, 14, 0, 2, Direction.PositiveZ);
        playerController.Initialize(13, 1, 9, Direction.NegativeZ);
    }

    /*白鬼が6体、緑鬼が4体、赤鬼が2体いる、橋を使う、赤鬼をおびき寄せて下に落とす*/
    private void Course43() {
        InitializeDoor(12, 0, 4);

        CreateOneBridge(8, 1, 5, Direction.PositiveX);
        CreateOneBridge(9, 1, 6, Direction.PositiveZ);
        CreateOneBridge(7, 0, 4, Direction.PositiveZ);
        CreateOneBridge(10, 1, 3, Direction.PositiveX);
        CreateOneBridge(12, 1, 3, Direction.PositiveX);

        CreateOneOgre(whiteOgre, 13, 1, 13, Direction.NegativeZ);
        CreateOneOgre(whiteOgre, 11, 1, 7, Direction.NegativeX);
        CreateOneOgre(whiteOgre, 13, 0, 4, Direction.PositiveX);
        CreateOneOgre(whiteOgre, 13, 0, 2, Direction.PositiveX);
        CreateOneOgre(whiteOgre, 9, 1, 2, Direction.PositiveZ);
        CreateOneOgre(whiteOgre, 1, 1, 1, Direction.PositiveZ);
        CreateOneOgre(whiteOgre, 5, 1, 11, Direction.NegativeZ);

        CreateOneOgre(greenOgre, 1, 1, 9, Direction.NegativeZ);
        CreateOneOgre(greenOgre, 8, 1, 7, Direction.NegativeX);
        CreateOneOgre(greenOgre, 12, 0, 6, Direction.NegativeX);
        CreateOneOgre(greenOgre, 12, 0, 2, Direction.PositiveZ);

        CreateOneOgre(redOgre, 1, 1, 11, Direction.NegativeZ);
        CreateOneOgre(redOgre, 3, 1, 9, Direction.NegativeX);

        playerController.Initialize(7, 1, 5, Direction.PositiveZ);
    }

    /*白鬼が5体、青鬼が8体いる*/
    private void Course44() {
        InitializeDoor(11, 0, 0);

        CreateOneOgre(whiteOgre, 9, 0, 10, Direction.PositiveX);
        CreateOneOgre(whiteOgre, 0, 0, 6, Direction.NegativeZ);
        CreateOneOgre(whiteOgre, 6, 0, 4, Direction.PositiveX);
        CreateOneOgre(whiteOgre, 14, 0, 1, Direction.PositiveZ);
        CreateOneOgre(whiteOgre, 2, 0, 2, Direction.PositiveZ);

        Behaviour[] b = new Behaviour[12];
        for (int i = 0; i < 12; i++) {
            if (i == 2 || i == 5 || i == 8 || i == 11) {
                b[i] = Behaviour.TurnToTheRight;
            }
            else {
                b[i] = Behaviour.GoForward;
            }
        }
        CreateOneOgre(blueOgre, 2, 0, 6, Direction.PositiveX, b);
        CreateOneOgre(blueOgre, 4, 0, 6, Direction.NegativeZ, b);
        CreateOneOgre(blueOgre, 2, 0, 4, Direction.PositiveZ, b);
        CreateOneOgre(blueOgre, 4, 0, 4, Direction.NegativeX, b);

        for (int i = 0; i < 12; i++) {
            if (i == 2 || i == 5 || i == 8 || i == 11) {
                b[i] = Behaviour.TurnToTheLeft;
            }
            else {
                b[i] = Behaviour.GoForward;
            }
        }
        CreateOneOgre(blueOgre, 4, 0, 10, Direction.NegativeZ, b);
        CreateOneOgre(blueOgre, 6, 0, 8, Direction.PositiveZ, b);

        playerController.Initialize(7, 1, 5, Direction.PositiveZ);
    }

    /*スタートは1階、ゴールは2階、白鬼が4体、青鬼が3体、赤鬼が1体、青鬼と赤鬼をうまくかわす、ステージを上下上方向に動く*/
    private void Course45() {
        CreateOneBridge(13, 1, 4, Direction.PositiveZ);
        InitializeDoor(7, 1, 1);
        CreateOneOgre(whiteOgre, 7, 1, 5, Direction.PositiveZ);
        CreateOneOgre(whiteOgre, 9, 1, 3, Direction.PositiveZ);
        CreateOneOgre(whiteOgre, 3, 1, 1, Direction.NegativeX);
        CreateOneOgre(whiteOgre, 11, 1, 1, Direction.PositiveX);

        Behaviour[] b1 = new Behaviour[12];
        for (int i = 0; i < 12; i++) {
            if (i == 2 || i == 5 || i == 8 || i == 11) {
                b1[i] = Behaviour.TurnToTheLeft;
            }else {
                b1[i] = Behaviour.GoForward;
            }
        }
        CreateOneOgre(blueOgre, 6, 0, 0, Direction.PositiveX, b1);
        CreateOneOgre(blueOgre, 8, 0, 2, Direction.NegativeX, b1);

        Behaviour[] b2 = new Behaviour[12];
        for (int i = 0; i < 12; i++) {
            if (i == 4 || i == 5 || i == 10 || i == 11) {
                b2[i] = Behaviour.TurnToTheRight;
            }
            else {
                b2[i] = Behaviour.GoForward;
            }
        }
        CreateOneOgre(blueOgre, 8, 0, 6, Direction.PositiveX, b2);

        CreateOneOgre(redOgre, 13, 1, 9, Direction.NegativeZ);
        playerController.Initialize(2, 0, 7, Direction.PositiveX);
    }

    /*スタートは1階、ゴールは2階、白鬼が10体、青鬼が2体、橋を1本手持ち、ステージ全体を逆S字に歩き回る*/
    private void Course46() {
        uiManager.firstBridge.gameObject.SetActive(true);
        UIManager.bridgeNumber = 1;
        InitializeDoor(4, 1, 11);
        CreateOneOgre(whiteOgre, 7, 1, 13, Direction.NegativeX);
        CreateOneOgre(whiteOgre, 7, 1, 11, Direction.PositiveX);
        CreateOneOgre(whiteOgre, 0, 0, 10, Direction.NegativeZ);
        CreateOneOgre(whiteOgre, 14, 0, 10, Direction.PositiveZ);
        CreateOneOgre(whiteOgre, 5, 1, 9, Direction.NegativeZ);
        CreateOneOgre(whiteOgre, 4, 0, 8, Direction.PositiveX);
        CreateOneOgre(whiteOgre, 10, 0, 6, Direction.PositiveX);
        CreateOneOgre(whiteOgre, 1, 1, 5, Direction.PositiveX);
        CreateOneOgre(whiteOgre, 12, 0, 5, Direction.PositiveZ);
        CreateOneOgre(whiteOgre, 8, 0, 2, Direction.NegativeX);

        Behaviour[] b1 = new Behaviour[8];
        for (int i = 0; i < 8; i++) {
            if (i == 1 || i == 2 || i == 5 || i == 6) {
                b1[i] = Behaviour.TurnToTheLeft;
            }
            else {
                b1[i] = Behaviour.GoForward;
            }
        }
        CreateOneOgre(blueOgre, 8, 1, 9, Direction.NegativeX, b1);
        CreateOneOgre(blueOgre, 4, 1, 1, Direction.NegativeX, b1);

        playerController.Initialize(10, 0, 2, Direction.PositiveZ);
    }

    /*スタートもゴールも2階、白鬼が14体、青鬼が2体、緑鬼が1体、赤鬼が1体いる、
    スタートとゴールがすくそばにある、青鬼と緑鬼の隙をついてブロックを橋で渡っていく、橋を使う、橋を1本手持ち*/
    private void Course47() {
        UIManager.bridgeNumber = 1;
        uiManager.firstBridge.gameObject.SetActive(true);
        InitializeDoor(8, 1, 3);
        CreateOneOgre(whiteOgre, 11, 1, 13, Direction.PositiveX);
        CreateOneOgre(whiteOgre, 5, 0, 12, Direction.PositiveX);
        CreateOneOgre(whiteOgre, 5, 1, 11, Direction.PositiveX);
        CreateOneOgre(whiteOgre, 11, 1, 11, Direction.NegativeX);
        CreateOneOgre(whiteOgre, 1, 1, 9, Direction.PositiveZ);
        CreateOneOgre(whiteOgre, 6, 0, 9, Direction.PositiveX);
        CreateOneOgre(whiteOgre, 6, 1, 9, Direction.PositiveX);
        CreateOneOgre(whiteOgre, 6, 1, 7, Direction.PositiveX);
        CreateOneOgre(whiteOgre, 7, 1, 6, Direction.PositiveZ);
        CreateOneOgre(whiteOgre, 14, 0, 6, Direction.PositiveZ);
        CreateOneOgre(whiteOgre, 3, 1, 5, Direction.NegativeX);
        CreateOneOgre(whiteOgre, 9, 1, 5, Direction.NegativeZ);
        CreateOneOgre(whiteOgre, 9, 1, 3, Direction.NegativeZ);
        CreateOneOgre(whiteOgre, 7, 1, 1, Direction.PositiveZ);

        Behaviour[] b1 = new Behaviour[12];
        for (int i = 0; i < 10; i++) {
            if (i == 4 || i == 5 || i == 10 || i == 11) {
                b1[i] = Behaviour.TurnToTheRight;
            }
            else {
                b1[i] = Behaviour.GoForward;
            }
        }
        CreateOneOgre(blueOgre, 13, 1, 5, Direction.PositiveZ, b1);

        Behaviour[] b2 = new Behaviour[8];
        for (int i = 0; i < 8; i++) {
            if (i == 2 || i == 3 || i == 6 || i == 7) {
                b2[i] = Behaviour.TurnToTheRight;
            }
            else {
                b2[i] = Behaviour.GoForward;
            }
        }
        CreateOneOgre(blueOgre, 9, 1, 9, Direction.NegativeX, b2);

        CreateOneOgre(greenOgre, 9, 1, 13, Direction.NegativeZ);
        CreateOneOgre(redOgre, 14, 0, 14, Direction.NegativeX);

        playerController.Initialize(13, 1, 1, Direction.NegativeX);
    }

    /*スタートは1階、ゴールは2階、白鬼が7体、緑鬼が1体、赤鬼が3体いる、赤鬼にはさまれる、緑鬼を一旦ゴールからどかして1階に降りてまた2階に上がる、橋を使う*/
    private void Course48() {
        InitializeDoor(5, 1, 9);
        CreateOneBridge(7, 0, 4, Direction.PositiveZ);
        CreateOneOgre(whiteOgre, 4, 0, 5, Direction.NegativeZ);
        CreateOneOgre(whiteOgre, 6, 0, 5, Direction.NegativeZ);
        CreateOneOgre(whiteOgre, 4, 0, 3, Direction.PositiveZ);
        CreateOneOgre(whiteOgre, 5, 0, 3, Direction.PositiveZ);
        CreateOneOgre(whiteOgre, 6, 0, 3, Direction.PositiveZ);
        CreateOneOgre(whiteOgre, 8, 0, 5, Direction.PositiveZ);
        CreateOneOgre(whiteOgre, 9, 0, 4, Direction.PositiveX);
        CreateOneOgre(greenOgre, 5, 1, 9, Direction.PositiveZ);
        CreateOneOgre(redOgre, 2, 0, 10, Direction.NegativeZ);
        CreateOneOgre(redOgre, 2, 0, 0, Direction.PositiveZ);
        CreateOneOgre(redOgre, 8, 0, 4, Direction.NegativeX);
        playerController.Initialize(7, 0, 10, Direction.PositiveX);
    }

    /*プレイヤーは2階、ゴールは1階、白鬼が2体、緑鬼が2体、赤鬼が2体いる、壁を使う、密です*/
    private void Course49() {
        InitializeDoor(3, 0, 14);
        CreateOneBridge(6, 0, 11, Direction.PositiveX);
        CreateOneBridge(8, 0, 13, Direction.PositiveX);
        CreateOneOgre(whiteOgre, 12, 0, 14, Direction.PositiveX);
        CreateOneOgre(whiteOgre, 10, 0, 13, Direction.NegativeZ);
        CreateOneBridge(0, 0, 11, Direction.PositiveX);
        CreateOneOgre(greenOgre, 9, 0, 14, Direction.NegativeX);
        CreateOneOgre(greenOgre, 0, 0, 12, Direction.PositiveZ);
        CreateOneOgre(redOgre, 1, 0, 14, Direction.PositiveX);
        CreateOneOgre(redOgre, 2, 0, 12, Direction.PositiveX);
        playerController.Initialize(11, 1, 13, Direction.NegativeX);
    }

    /*スタートもゴールも2階、白鬼が18体、青鬼が3体、緑鬼が1体、赤鬼が3体、落ちている橋は7本、青鬼の行列を躱す*/
    private void Course50() {
        InitializeDoor(9, 1, 11);

        CreateOneBridge(13, 1, 10, Direction.PositiveZ);
        CreateOneBridge(6, 1, 9, Direction.PositiveX);
        CreateOneBridge(7, 1, 8, Direction.PositiveZ);
        CreateOneBridge(8, 1, 8, Direction.PositiveZ);
        CreateOneBridge(9, 1, 8, Direction.PositiveZ);

        CreateOneOgre(whiteOgre, 7, 1, 13, Direction.NegativeX);
        CreateOneOgre(whiteOgre, 9, 1, 12, Direction.PositiveZ);
        CreateOneOgre(whiteOgre, 5, 1, 11, Direction.NegativeZ);
        CreateOneOgre(whiteOgre, 11, 1, 11, Direction.PositiveX);
        CreateOneOgre(whiteOgre, 10, 1, 7, Direction.PositiveX);

        Behaviour[] b = new Behaviour[8];
        for (int i = 0; i < 8; i++) {
            if (i == 0 || i == 1 || i == 4 || i == 5) {
                b[i] = Behaviour.TurnToTheLeft;
            }
            else {
                b[i] = Behaviour.GoForward;
            }
        }
        CreateOneOgre(blueOgre, 7, 1, 9, Direction.PositiveZ, b);
        CreateOneOgre(blueOgre, 8, 1, 7, Direction.NegativeZ, b);
        CreateOneOgre(blueOgre, 9, 1, 9, Direction.PositiveZ, b);

        CreateOneOgre(greenOgre, 13, 1, 11, Direction.NegativeX);
        CreateOneOgre(redOgre, 5, 1, 9, Direction.PositiveX);

        playerController.Initialize(8, 0, 4, Direction.PositiveX);
    }

    private void InitializeSquares() {
        for (int i = 0; i < 15; i++) {
            for (int j = 0; j < 15; j++) {
                ogreSquares[i, 0, j] = true;
                ogreSquares[i, 1, j] = false;
                playerSquares[i, 0, j] = true;
                playerSquares[i, 1, j] = false;
            }
        }

        /*ブロック1*/
        for (int j = 1; j <= 5; j++) {
            ogreSquares[1, 0, j] = false;
            ogreSquares[1, 1, j] = true;
            playerSquares[1, 0, j] = false;
            playerSquares[1, 1, j] = true;
        }

        /*ブロック2*/
        for (int i = 3; i <= 5; i++) {
            ogreSquares[i, 0, 1] = false;
            ogreSquares[i, 1, 1] = true;
            playerSquares[i, 0, 1] = false;
            playerSquares[i, 1, 1] = true;
        }

        /*ブロック3*/
        ogreSquares[7, 0, 1] = false;
        ogreSquares[7, 1, 1] = true;
        playerSquares[7, 0, 1] = false;
        playerSquares[7, 1, 1] = true;

        /*ブロック4*/
        ogreSquares[3, 0, 3] = false;
        ogreSquares[3, 1, 3] = true;
        playerSquares[3, 0, 3] = false;
        playerSquares[3, 1, 3] = true;

        /*ブロック5*/
        //ogreSquares[5, 0, 3] = false;
        //ogreSquares[5, 1, 3] = true;
        //playerSquares[5, 0, 3] = false;
        //playerSquares[5, 1, 3] = true;

        /*ブロック6*/
        for (int i = 7; i <= 9; i++) {
            ogreSquares[i, 0, 3] = false;
            ogreSquares[i, 1, 3] = true;
            playerSquares[i, 0, 3] = false;
            playerSquares[i, 1, 3] = true;
        }
        ogreSquares[9, 0, 2] = false;
        ogreSquares[9, 1, 2] = true;
        playerSquares[9, 0, 2] = false;
        playerSquares[9, 1, 2] = true;
        for (int i = 9; i <= 13; i++) {
            ogreSquares[i, 0, 1] = false;
            ogreSquares[i, 1, 1] = true;
            playerSquares[i, 0, 1] = false;
            playerSquares[i, 1, 1] = true;
        }

        /*ブロック7*/
        ogreSquares[3, 0, 5] = false;
        ogreSquares[3, 1, 5] = true;
        playerSquares[3, 0, 5] = false;
        playerSquares[3, 1, 5] = true;

        /*ブロック8*/
        ogreSquares[5, 0, 5] = false;
        ogreSquares[5, 1, 5] = true;
        playerSquares[5, 0, 5] = false;
        playerSquares[5, 1, 5] = true;

        /*ブロック9*/
        ogreSquares[9, 0, 5] = false;
        ogreSquares[9, 1, 5] = true;
        playerSquares[9, 0, 5] = false;
        playerSquares[9, 1, 5] = true;

        /*ブロック10*/
        for (int j = 3; j <= 5; j++) {
            ogreSquares[11, 0, j] = false;
            ogreSquares[11, 1, j] = true;
            playerSquares[11, 0, j] = false;
            playerSquares[11, 1, j] = true;
        }

        /*ブロック11*/
        ogreSquares[13, 0, 3] = false;
        ogreSquares[13, 1, 3] = true;
        playerSquares[13, 0, 3] = false;
        playerSquares[13, 1, 3] = true;

        /*ブロック12*/
        ogreSquares[1, 0, 7] = false;
        ogreSquares[1, 1, 7] = true;
        playerSquares[1, 0, 7] = false;
        playerSquares[1, 1, 7] = true;

        /*ブロック13*/
        //ogreSquares[3, 0, 7] = false;
        //ogreSquares[3, 1, 7] = true;
        //playerSquares[3, 0, 7] = false;
        //playerSquares[3, 1, 7] = true;

        /*ブロック14*/
        for (int i = 5; i <= 12; i++) {
            ogreSquares[i, 0, 7] = false;
            ogreSquares[i, 1, 7] = true;
            playerSquares[i, 0, 7] = false;
            playerSquares[i, 1, 7] = true;
        }
        for (int j = 5; j <= 6; j++) {
            ogreSquares[7, 0, j] = false;
            ogreSquares[7, 1, j] = true;
            playerSquares[7, 0, j] = false;
            playerSquares[7, 1, j] = true;
        }
        for (int j = 5; j <= 9; j++) {
            ogreSquares[13, 0, j] = false;
            ogreSquares[13, 1, j] = true;
            playerSquares[13, 0, j] = false;
            playerSquares[13, 1, j] = true;
        }

        /*ブロック15*/
        ogreSquares[1, 0, 9] = false;
        ogreSquares[1, 1, 9] = true;
        playerSquares[1, 0, 9] = false;
        playerSquares[1, 1, 9] = true;

        /*ブロック16*/
        ogreSquares[5, 0, 9] = false;
        ogreSquares[5, 1, 9] = true;
        playerSquares[5, 0, 9] = false;
        playerSquares[5, 1, 9] = true;

        /*ブロック17*/
        for (int i = 7; i <= 9; i++) {
            ogreSquares[i, 0, 9] = false;
            ogreSquares[i, 1, 9] = true;
            playerSquares[i, 0, 9] = false;
            playerSquares[i, 1, 9] = true;
        }

        /*ブロック18*/
        //ogreSquares[11, 0, 9] = false;
        //ogreSquares[11, 1, 9] = true;
        //playerSquares[11, 0, 9] = false;
        //playerSquares[11, 1, 9] = true;

        /*ブロック19*/
        for (int i = 1; i <= 7; i++) {
            ogreSquares[i, 0, 13] = false;
            ogreSquares[i, 1, 13] = true;
            playerSquares[i, 0, 13] = false;
            playerSquares[i, 1, 13] = true;
        }
        for (int j = 11; j <= 12; j++) {
            ogreSquares[1, 0, j] = false;
            ogreSquares[1, 1, j] = true;
            playerSquares[1, 0, j] = false;
            playerSquares[1, 1, j] = true;
        }
        for (int i = 2; i <= 5; i++) {
            ogreSquares[i, 0, 11] = false;
            ogreSquares[i, 1, 11] = true;
            playerSquares[i, 0, 11] = false;
            playerSquares[i, 1, 11] = true;
        }
        for (int j = 9; j <= 10; j++) {
            ogreSquares[3, 0, j] = false;
            ogreSquares[3, 1, j] = true;
            playerSquares[3, 0, j] = false;
            playerSquares[3, 1, j] = true;
        }

        /*ブロック20*/
        for (int i = 7; i <= 9; i++) {
            ogreSquares[i, 0, 11] = false;
            ogreSquares[i, 1, 11] = true;
            playerSquares[i, 0, 11] = false;
            playerSquares[i, 1, 11] = true;
        }
        for (int j = 12; j <= 13; j++) {
            ogreSquares[9, 0, j] = false;
            ogreSquares[9, 1, j] = true;
            playerSquares[9, 0, j] = false;
            playerSquares[9, 1, j] = true;
        }

        /*ブロック21*/
        ogreSquares[11, 0, 11] = false;
        ogreSquares[11, 1, 11] = true;
        playerSquares[11, 0, 11] = false;
        playerSquares[11, 1, 11] = true;

        /*ブロック22*/
        for (int i = 11; i <= 13; i++) {
            ogreSquares[i, 0, 13] = false;
            ogreSquares[i, 1, 13] = true;
            playerSquares[i, 0, 13] = false;
            playerSquares[i, 1, 13] = true;
        }
        for (int j = 11; j <= 12; j++) {
            ogreSquares[13, 0, j] = false;
            ogreSquares[13, 1, j] = true;
            playerSquares[13, 0, j] = false;
            playerSquares[13, 1, j] = true;
        }
    }

    private void CreateOneBridge(int x, int y, int z, Direction initialDirection) {
        /*橋のアイコンが0, 壁のアイコンが1*/
        if (y == 0) {
            playerSquares[x, y, z] = false;
            ogreSquares[x, y, z] = false;
            if (initialDirection == Direction.PositiveZ || initialDirection == Direction.NegativeZ) {
                GameObject bridge = Instantiate(bridgePrefab, new Vector3(x, y + 0.5f, z), Quaternion.Euler(45f, 0f, 0f));
                bridge.gameObject.transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
                bridge.transform.GetChild(0).gameObject.SetActive(false);
                bridges.Add(new Point(x, y, z), bridge);
            }
            else if (initialDirection == Direction.PositiveX || initialDirection == Direction.NegativeX) {
                GameObject bridge = Instantiate(bridgePrefab, new Vector3(x, y + 0.5f, z), Quaternion.Euler(45f, 90f, 0f));
                bridge.gameObject.transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
                bridge.transform.GetChild(0).gameObject.SetActive(false);
                bridges.Add(new Point(x, y, z), bridge);
            }
        }
        else if (y == 1) {
            playerSquares[x, y, z] = true;
            ogreSquares[x, y, z] = true;
            if (initialDirection == Direction.PositiveZ || initialDirection == Direction.NegativeZ) {
                GameObject bridge = Instantiate(bridgePrefab, new Vector3(x, y, z), Quaternion.Euler(0f, 0f, 0f));
                bridge.transform.GetChild(1).gameObject.SetActive(false);
                bridges.Add(new Point(x, y, z), bridge);
            }
            else if (initialDirection == Direction.PositiveX || initialDirection == Direction.NegativeX) {
                GameObject bridge = Instantiate(bridgePrefab, new Vector3(x, y, z), Quaternion.Euler(0f, 90f, 0f));
                bridge.transform.GetChild(1).gameObject.SetActive(false);
                bridges.Add(new Point(x, y, z), bridge);
            }
        }
    }

    public void DestroyBridges(Point bridgePosition) {
        //Debug.Log(bridges[bridgePosition]);
        bridges[bridgePosition].GetComponent<BridgeInstanceController>().DestroyThis();
        bridges.Remove(bridgePosition);
        if (bridgePosition.y == 1) {
            playerSquares[bridgePosition.x, bridgePosition.y, bridgePosition.z] = false;
            ogreSquares[bridgePosition.x, bridgePosition.y, bridgePosition.z] = false;
        }
        else if (bridgePosition.y == 0) {
            playerSquares[bridgePosition.x, bridgePosition.y, bridgePosition.z] = true;
            ogreSquares[bridgePosition.x, bridgePosition.y, bridgePosition.z] = true;
        }
    }

    private void InitializeDoor(int x, int y, int z) {
        goal_square = new Point(x, y, z);
        Instantiate(door, new Vector3(x, y + 0.01f, z), Quaternion.Euler(90f, 0f, 0f));
        //Debug.Log("ドアの座標: " + goal_square.x + " " + goal_square.y + " " + goal_square.z);
    }

    private void CreateOneOgre(GameObject ogre, int x, int y, int z, Direction initialDirection) {
        GameObject thisOgre = null;
        if (initialDirection == Direction.PositiveZ) {
            thisOgre = Instantiate(ogre, new Vector3(x, y, z), Quaternion.identity);
        }
        else if (initialDirection == Direction.PositiveX) {
            thisOgre = Instantiate(ogre, new Vector3(x, y, z), Quaternion.Euler(0f, 90f, 0f));
        }
        else if (initialDirection == Direction.NegativeZ) {
            thisOgre = Instantiate(ogre, new Vector3(x, y, z), Quaternion.Euler(0f, 180f, 0f));
        }
        else if (initialDirection == Direction.NegativeX) {
            thisOgre = Instantiate(ogre, new Vector3(x, y, z), Quaternion.Euler(0f, 270f, 0f));
        }
        thisOgre.GetComponent<OgreController>().Initialize(x, y, z, initialDirection);
        ogres.Add(thisOgre);
    }

    private void CreateOneOgre(GameObject ogre, int x, int y, int z, Direction initialDirection, Behaviour[] ogreBehaviours) {
        GameObject thisOgre = null;
        if (initialDirection == Direction.PositiveZ) {
            thisOgre = Instantiate(ogre, new Vector3(x, y, z), Quaternion.identity);
        }
        else if (initialDirection == Direction.PositiveX) {
            thisOgre = Instantiate(ogre, new Vector3(x, y, z), Quaternion.Euler(0f, 90f, 0f));
        }
        else if (initialDirection == Direction.NegativeZ) {
            thisOgre = Instantiate(ogre, new Vector3(x, y, z), Quaternion.Euler(0f, 180f, 0f));
        }
        else if (initialDirection == Direction.NegativeX) {
            thisOgre = Instantiate(ogre, new Vector3(x, y, z), Quaternion.Euler(0f, 270f, 0f));
        }
        thisOgre.GetComponent<OgreController>().Initialize(x, y, z, initialDirection);
        thisOgre.GetComponent<BlueOgreController>().SetPathway(ogreBehaviours); //青鬼のみに対する操作
        ogres.Add(thisOgre);
    }

    /*不定期に効果音を鳴らす*/
    private void PlayCrowSoundsAtIrregularIntervals() {
        if (crowExit) {
            if (crowScreamTime > crowSoundTime) {
                StartCoroutine(CrowSoundEffectFadeInOrFadeOut(false, 5f)); //フェードアウトする
            }
        }
        else {
            if (crowScreamTime > noCrowSoundTime) {
                StartCoroutine(CrowSoundEffectFadeInOrFadeOut(true, 5f)); //フェードインする
            }
        }
    }

    /*コースクリア時にそのコースがクリア済みであることをローカルに記録しておく*/
    public void RecordClearCourseNumber() {
        PlayerPrefs.SetInt("Course" + ScreenUIManager.courseNumber, 1);
        PlayerPrefs.Save();
    }
    
    public void MakeOgresActive(bool getActive) {
        if (getActive) {
            foreach (GameObject ogre in ogres) {
                ogre.transform.GetChild(0).gameObject.SetActive(true);
            }
        }
        else {
            foreach (GameObject ogre in ogres) {
                ogre.transform.GetChild(0).gameObject.SetActive(false);
            }
        }
    }

    /*フェードインならtrue, フェードアウトならfalseを渡す*/
    private IEnumerator CrowSoundEffectFadeInOrFadeOut(bool fadeIn, float fadeTime) {
        crowScreamTime = 0f;
        if (fadeIn) {
            crowExit = true;
            for (int i = 0; i < 10; i++) {
                audioSource.volume += 0.1f;
                yield return new WaitForSeconds(fadeTime / 10f);
            }
            noCrowSoundTime = UnityEngine.Random.Range(10f, 20f);
        }
        else {
            crowExit = false;
            for (int i = 0; i < 10; i++) {
                audioSource.volume -= 0.1f;
                yield return new WaitForSeconds(fadeTime / 10f);
            }
            crowSoundTime = UnityEngine.Random.Range(3f, 7f);
        }
    }
}
