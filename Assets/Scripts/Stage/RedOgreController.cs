using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedOgreController : OgreController
{
    protected PlayerController playerController;
    protected bool isChasing; //isChasing: プレイヤーを追っているときはtrueになる, isLeft: 左足を前に出すように歩くときはtrueになる
    protected Point destination = new Point(); //プレイヤーを最後に視界に入れたマスの座標

    private void Awake() {
        stageManager = GameObject.Find("StageManager").GetComponent<StageManager>();
        uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();

        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        isLeft = UnityEngine.Random.Range(0, 2) == 0; //初めに上げる足はランダムに決める
    }

    // Start is called before the first frame update
    void Start() {
        //s = GameObject.Find("Script").GetComponent<StageManager>();
        //gameOverText = GameObject.Find("Canvas/GameOverText").GetComponent<Text>();
        //gameOverText.gameObject.SetActive(false);

        isChasing = false;
        //lastBehaviour = OgreBehaviour.GoForward;

        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        //Debug.Log("鬼はランダム行動中です " + "現在の座標" + "(" + point.x + ", " + point.y + ", " + point.z + ")");
    }

    // Update is called once per frame
    void Update() {
        if (uiManager.IS_GOING_ON) {
            time += Time.deltaTime;
        }

        /*1秒ごとに次の行動を決める*/
        if (time > 1f) {
            TryLocatingPlayer();
            if (isChasing) {
                GoToDestination();
            }
            //TryLocatingPlayer();
            //Debug.Log("destination = (" + destination.x + ", " + destination.y + ", " + destination.z + ")");
            time = 0f;
        }
    }

    /*もしプレイヤーを見つけたら目的地となる座標を設定し、追跡を開始する*/
    /*目的地まで行く途中に現在の目的地とは異なるマスでプレイヤーを見つけたら、目的地の座標を更新する*/
    /*目的地であるマスに到達したときにプレイヤーを見失ったら、追跡を終了する*/
    protected void TryLocatingPlayer() {
        Point p_point; //現在のプレイヤーの座標を取得
        p_point.x = playerController.point.x; p_point.y = playerController.point.y; p_point.z = playerController.point.z;

        Point o_point = point; //現在の鬼の座標を取得

        switch (currentDirection) {
            case Direction.PositiveX:
                /*右方向の探索*/
                SearchNegativeZDirection();
                /*正面方向の探索*/
                SearchPositiveXDirection();
                /*左方向の探索*/
                SearchPositiveZDirection();
                break;
            case Direction.PositiveZ:
                /*右方向の探索*/
                SearchPositiveXDirection();
                /*正面方向の探索*/
                SearchPositiveZDirection();
                /*左方向の探索*/
                SearchNegativeXDirection();
                break;
            case Direction.NegativeX:
                /*右方向の探索*/
                SearchPositiveZDirection();
                /*正面方向の探索*/
                SearchNegativeXDirection();
                /*左方向の探索*/
                SearchNegativeZDirection();
                break;
            case Direction.NegativeZ:
                /*右方向の探索*/
                SearchNegativeXDirection();
                /*正面方向の探索*/
                SearchNegativeZDirection();
                /*左方向の探索*/
                SearchPositiveXDirection();
                break;
        }

        void SearchPositiveXDirection() {
            for (int x = o_point.x + 1; x <= 14; x++) {
                if (!stageManager.ogreSquares[x, o_point.y, o_point.z]) {
                    break; //壁があるマスから先のマスは探索しない
                }
                if (p_point.x == x && p_point.y == o_point.y && p_point.z == o_point.z) { //視線の先にプレイヤーがいたら
                    destination.x = x; destination.y = o_point.y; destination.z = o_point.z; //プレイヤーがいるマスを目的地に設定する
                    isChasing = true; //プレイヤーを見つけたら追跡を開始する
                    //Debug.Log("<color=red>プレイヤーを発見</color>");
                }
            }
        }

        void SearchPositiveZDirection() {
            for (int z = o_point.z + 1; z <= 14; z++) {
                if (!stageManager.ogreSquares[o_point.x, o_point.y, z]) {
                    break; //壁があるマスから先のマスは探索しない
                }
                if (p_point.x == o_point.x && p_point.y == o_point.y && p_point.z == z) {
                    destination.x = o_point.x; destination.y = o_point.y; destination.z = z;
                    isChasing = true;　//プレイヤーを見つけたら追跡を開始する
                    //Debug.Log("<color=red>プレイヤーを発見</color>");
                }
            }
        }

        void SearchNegativeXDirection() {
            for (int x = o_point.x - 1; x >= 0; x--) {
                if (!stageManager.ogreSquares[x, o_point.y, o_point.z]) {
                    break; //壁があるマスから先のマスは探索しない
                }
                if (p_point.x == x && p_point.y == o_point.y && p_point.z == o_point.z) {
                    destination.x = x; destination.y = o_point.y; destination.z = o_point.z;
                    isChasing = true;　//プレイヤーを見つけたら追跡を開始する
                    //Debug.Log("<color=red>プレイヤーを発見</color>");
                }
            }
        }

        void SearchNegativeZDirection() {
            for (int z = o_point.z - 1; z >= 0; z--) {
                if (!stageManager.ogreSquares[o_point.x, o_point.y, z]) {
                    break; //壁があるマスから先のマスは探索しない
                }
                if (p_point.x == o_point.x && p_point.y == o_point.y && p_point.z == z) {
                    destination.x = o_point.x; destination.y = o_point.y; destination.z = z;
                    isChasing = true;　//プレイヤーを見つけたら追跡を開始する
                    //Debug.Log("<color=red>プレイヤーを発見</color>");
                }
            }
        }
    }

    /*もしプレイヤーを追跡中なら、目的地として設定した座標まで直線的に向かう*/
    protected void GoToDestination() {
        //lastBehaviour = OgreBehaviour.GoForward; //追跡を開始したらランダム行動中の最後にとった行動はなかったことにする

        int x = destination.x - point.x;
        int z = destination.z - point.z;

        if (x > 0) {
            if (currentDirection == Direction.PositiveZ) {
                StartCoroutine(Rotate(Behaviour.TurnToTheRight));
                //Debug.Log("追跡中: PositiveXを向いた");
            }
            else if (currentDirection == Direction.NegativeZ) {
                StartCoroutine(Rotate(Behaviour.TurnToTheLeft));
                //Debug.Log("追跡中: PositiveXを向いた");
            }
            else if (currentDirection == Direction.PositiveX) {
                if (stageManager.ogreSquares[point.x + 1, point.y, point.z]) {
                    StartCoroutine(Proceed(Direction.PositiveX));
                    //Debug.Log("追跡中: PositiveXに向かって移動した");
                    //point.x++;
                }
                /*壁が進路上に置かれていたら追跡を中止する*/
                else if (!stageManager.playerSquares[point.x + 1, point.y, point.z]) {
                    isChasing = false;
                }
            }
            //Debug.Log("鬼は追跡中です: +xに進む " + "現在の座標" + "(" + point.x + ", " + point.y + ", " + point.z + ")");
        }
        else if (x < 0) {
            if (currentDirection == Direction.NegativeZ) {
                StartCoroutine(Rotate(Behaviour.TurnToTheRight));
                //Debug.Log("追跡中: NegativeXを向いた");
            }
            else if (currentDirection == Direction.PositiveZ) {
                StartCoroutine(Rotate(Behaviour.TurnToTheLeft));
                //Debug.Log("追跡中: NegativeXを向いた");
            }
            else if (currentDirection == Direction.NegativeX) {
                if (stageManager.ogreSquares[point.x - 1, point.y, point.z]) {
                    StartCoroutine(Proceed(Direction.NegativeX));
                    //Debug.Log("追跡中: NegativeXに向かって移動した");
                    //point.x--;
                }
                /*壁が進路上に置かれていたら追跡を中止する*/
                else if (!stageManager.playerSquares[point.x - 1, point.y, point.z]) {
                    isChasing = false;
                }
            }
            //Debug.Log("鬼は追跡中です: -xに進む " + "現在の座標" + "(" + point.x + ", " + point.y + ", " + point.z + ")");
        }
        else if (z > 0) {
            if (currentDirection == Direction.NegativeX) {
                StartCoroutine(Rotate(Behaviour.TurnToTheRight));
                //Debug.Log("追跡中: PositiveZを向いた");
            }
            else if (currentDirection == Direction.PositiveX) {
                StartCoroutine(Rotate(Behaviour.TurnToTheLeft));
                //Debug.Log("追跡中: PositiveZを向いた");
            }
            else if (currentDirection == Direction.PositiveZ) {
                if (stageManager.ogreSquares[point.x, point.y, point.z + 1]) {
                    StartCoroutine(Proceed(Direction.PositiveZ));
                    //Debug.Log("追跡中: PositiveZに向かって移動した");
                    //point.z++;
                }
                /*壁が進路上に置かれていたら追跡を中止する*/
                else if (!stageManager.playerSquares[point.x, point.y, point.z + 1]) {
                    isChasing = false;
                }
            }
            //Debug.Log("鬼は追跡中です: +zに進む " + "現在の座標" + "(" + point.x + ", " + point.y + ", " + point.z + ")");
        }
        else if (z < 0) {
            if (currentDirection == Direction.PositiveX) {
                StartCoroutine(Rotate(Behaviour.TurnToTheRight));
                //Debug.Log("追跡中: NegativeZを向いた");
            }
            else if (currentDirection == Direction.NegativeX) {
                StartCoroutine(Rotate(Behaviour.TurnToTheLeft));
                //Debug.Log("追跡中: NegativeZを向いた");
            }
            else if (currentDirection == Direction.NegativeZ) {
                if (stageManager.ogreSquares[point.x, point.y, point.z - 1]) {
                    StartCoroutine(Proceed(Direction.NegativeZ));
                    //Debug.Log("追跡中: NegativeZに向かって移動した");
                    //point.z--;
                }
                /*壁が進路上に置かれていたら追跡を中止する*/
                else if (!stageManager.playerSquares[point.x, point.y, point.z - 1]) {
                    isChasing = false;
                }
            }
            //Debug.Log("鬼は追跡中です: -zに進む " + "現在の座標" + "(" + point.x + ", " + point.y + ", " + point.z + ")");
        }
        /*目的地に到着した場合*/
        else {
            isChasing = false;
            //Debug.Log("<color=red>目的地に到着した</color>");
        }
        //Debug.Log("<color=blue>GoToDestinationが実行されました</color>");
    }
}
