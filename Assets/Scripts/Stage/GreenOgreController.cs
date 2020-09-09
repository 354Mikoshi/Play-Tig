using System.Collections.Generic;
using UnityEngine;

public class GreenOgreController : RedOgreController
{
    private Behaviour lastBehaviour;

    private void Awake() {
        stageManager = GameObject.Find("StageManager").GetComponent<StageManager>();
        uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();

        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        isLeft = Random.Range(0, 2) == 0; //初めに上げる足はランダムに決める
    }

    // Start is called before the first frame update
    void Start() {
        isChasing = false;
        lastBehaviour = Behaviour.GoForward;

        playerController = GameObject.Find("Player").GetComponent<PlayerController>();
        //Debug.Log("鬼はランダム行動中です " + "現在の座標" + "(" + point.x + ", " + point.y + ", " + point.z + ")");
    }

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
            else {
                DecideNextRandomBehaviour();
            }
            //TryLocatingPlayer();
            //Debug.Log("destination = (" + destination.x + ", " + destination.y + ", " + destination.z + ")");
            time = 0f;
        }
    }

    /*プレイヤーを追跡していないときに、次の鬼の行動を決める*/
    private void DecideNextRandomBehaviour() {
        List<Behaviour> nextBehaviours = new List<Behaviour>();

        switch (currentDirection) {
            case Direction.PositiveX:
                /*右*/
                if (point.z - 1 >= 0) {
                    if (stageManager.ogreSquares[point.x, point.y, point.z - 1]) {
                        nextBehaviours.Add(Behaviour.TurnToTheRight);
                        //Debug.Log("ランダム行動決め: PositiveX-右");
                    }
                }
                /*前*/
                if (point.x + 1 <= 14) {
                    if (stageManager.ogreSquares[point.x + 1, point.y, point.z]) {
                        nextBehaviours.Add(Behaviour.GoForward);
                        //Debug.Log("ランダム行動決め: PositiveX-前");
                    }
                }
                /*左*/
                if (point.z + 1 <= 14) {
                    if (stageManager.ogreSquares[point.x, point.y, point.z + 1]) {
                        nextBehaviours.Add(Behaviour.TurnToTheLeft);
                        //Debug.Log("ランダム行動決め: PositiveX-左");
                    }
                }
                //Debug.Log("現在の向きはPositiveXです");
                break;
            case Direction.PositiveZ:
                /*右*/
                if (point.x + 1 <= 14) {
                    if (stageManager.ogreSquares[point.x + 1, point.y, point.z]) {
                        nextBehaviours.Add(Behaviour.TurnToTheRight);
                        //Debug.Log("ランダム行動決め: PositiveZ-右");
                    }
                }
                /*前*/
                if (point.z + 1 <= 14) {
                    if (stageManager.ogreSquares[point.x, point.y, point.z + 1]) {
                        nextBehaviours.Add(Behaviour.GoForward);
                        //Debug.Log("ランダム行動決め: PositiveZ-前");
                    }
                }
                /*左*/
                if (point.x - 1 >= 0) {
                    if (stageManager.ogreSquares[point.x - 1, point.y, point.z]) {
                        nextBehaviours.Add(Behaviour.TurnToTheLeft);
                        //Debug.Log("ランダム行動決め: PositiveZ-左");
                    }
                }
                //Debug.Log("現在の向きはPositiveZです");
                break;
            case Direction.NegativeX:
                /*右*/
                if (point.z + 1 <= 14) {
                    if (stageManager.ogreSquares[point.x, point.y, point.z + 1]) {
                        nextBehaviours.Add(Behaviour.TurnToTheRight);
                        //Debug.Log("ランダム行動決め: NegativeX-右");
                    }
                }
                /*前*/
                if (point.x - 1 >= 0) {
                    if (stageManager.ogreSquares[point.x - 1, point.y, point.z]) {
                        nextBehaviours.Add(Behaviour.GoForward);
                        //Debug.Log("ランダム行動決め: NegativeX-前");
                    }
                }
                /*左*/
                if (point.z - 1 >= 0) {
                    if (stageManager.ogreSquares[point.x, point.y, point.z - 1]) {
                        nextBehaviours.Add(Behaviour.TurnToTheLeft);
                        //Debug.Log("ランダム行動決め: NegativeX-左");
                    }
                }
                //Debug.Log("現在の向きはNegativeXです");
                break;
            case Direction.NegativeZ:
                /*右*/
                if (point.x - 1 >= 0) {
                    if (stageManager.ogreSquares[point.x - 1, point.y, point.z]) {
                        nextBehaviours.Add(Behaviour.TurnToTheRight);
                        //Debug.Log("ランダム行動決め: NegativeZ-右");
                    }
                }
                /*前*/
                if (point.z - 1 >= 0) {
                    if (stageManager.ogreSquares[point.x, point.y, point.z - 1]) {
                        nextBehaviours.Add(Behaviour.GoForward);
                        //Debug.Log("ランダム行動決め: NegativeZ-前");
                    }
                }
                /*左*/
                if (point.x + 1 <= 14) {
                    if (stageManager.ogreSquares[point.x + 1, point.y, point.z]) {
                        nextBehaviours.Add(Behaviour.TurnToTheLeft);
                        //Debug.Log("ランダム行動決め: NegativeZ-左");
                    }
                }
                //Debug.Log("現在の向きはNegativeZです");
                break;
        }

        if (nextBehaviours.Count == 0) { //行き止まりに突き当たった場合
            StartCoroutine(Rotate(Behaviour.TurnToTheRight));
            lastBehaviour = Behaviour.GoForward;
            //Debug.Log("行き止まりなので右を向いた");
        }
        else { //行き止まりではない場合
            /*最後にとった行動が「前進する」だった場合*/
            if (lastBehaviour == Behaviour.GoForward) {
                Behaviour nextBehaviour = nextBehaviours[Random.Range(0, nextBehaviours.Count)]; // 次の行動をランダムに決定

                /*右を向く*/
                if (nextBehaviour == Behaviour.TurnToTheRight) {
                    StartCoroutine(Rotate(Behaviour.TurnToTheRight));
                    lastBehaviour = Behaviour.TurnToTheRight;
                    //Debug.Log("ランダム行動中: 右を向いた");
                }
                /*前進する*/
                else if (nextBehaviour == Behaviour.GoForward) {
                    lastBehaviour = Behaviour.GoForward;
                    switch (currentDirection) {
                        case Direction.PositiveX:
                            StartCoroutine(Proceed(Direction.PositiveX));
                            //Debug.Log("ランダム行動中: PositiveXに向かって前進した");
                            break;
                        case Direction.PositiveZ:
                            StartCoroutine(Proceed(Direction.PositiveZ));
                            //Debug.Log("ランダム行動中: PositiveZに向かって前進した");
                            break;
                        case Direction.NegativeX:
                            StartCoroutine(Proceed(Direction.NegativeX));
                            //Debug.Log("ランダム行動中: NegativeXに向かって前進した");
                            break;
                        case Direction.NegativeZ:
                            StartCoroutine(Proceed(Direction.NegativeZ));
                            //Debug.Log("ランダム行動中: NegativeZに向かって前進した");
                            break;
                    }
                }
                /*左を向く*/
                else if (nextBehaviour == Behaviour.TurnToTheLeft) {
                    StartCoroutine(Rotate(Behaviour.TurnToTheLeft));
                    lastBehaviour = Behaviour.TurnToTheLeft;
                    //Debug.Log("ランダム行動中: 左を向いた");
                }

                lastBehaviour = nextBehaviour; //最後にとった行動を記録する
            }
            else if (currentDirection == Direction.PositiveX && point.x <= 13) {
                if (stageManager.ogreSquares[point.x + 1, point.y, point.z]) {
                    StartCoroutine(Proceed(Direction.PositiveX));
                    //Debug.Log("ランダム行動中: 最後にとった行動が方向転換だったのでPositiveXに向かって前進した");
                }
                lastBehaviour = Behaviour.GoForward;
            }
            else if (currentDirection == Direction.PositiveZ && point.z <= 13) {
                if (stageManager.ogreSquares[point.x, point.y, point.z + 1]) {
                    StartCoroutine(Proceed(Direction.PositiveZ));
                    //Debug.Log("ランダム行動中: 最後にとった行動が方向転換だったのでPositiveZに向かって前進した");
                }
                lastBehaviour = Behaviour.GoForward;
            }
            else if (currentDirection == Direction.NegativeX && point.x >= 1) {
                if (stageManager.ogreSquares[point.x - 1, point.y, point.z]) {
                    StartCoroutine(Proceed(Direction.NegativeX));
                    //Debug.Log("ランダム行動中: 最後にとった行動が方向転換だったのでNegativeXに向かって前進した");
                }
                lastBehaviour = Behaviour.GoForward;
            }
            else if (currentDirection == Direction.NegativeZ && point.z >= 1) {
                if (stageManager.ogreSquares[point.x, point.y, point.z - 1]) {
                    StartCoroutine(Proceed(Direction.NegativeZ));
                    //Debug.Log("ランダム行動中: 最後にとった行動が方向転換だったのでNegativeZに向かって前進した");
                }
                lastBehaviour = Behaviour.GoForward;
            }
            /*最後にとった行動が「左を向く」or「右を向く」だった場合*/
        }
        //Debug.Log("<color=blue>DecideNextRandomBehaviourが実行されました</color>");

        //Debug.Log("鬼はランダム行動中です " + "現在の座標" + "(" + point.x + ", " + point.y + ", " + point.z + ")");
    }
}
