using System.Collections;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject bridgePrefab;
    public AudioClip[] footSound;
    private StageManager stageManager;
    private UIManager uiManager;

    private AudioSource[] audioSources;

    private Vector2 touchStartPos, touchEndPos; //タッチを始めたときの座標、タッチが終わった時の座標
    float movingAmountX, movingAmountY; // x軸方向のフリックの移動量, y軸方向のフリックの移動量

    public Point point = new Point(); //インスタンスを生成
    public Point oneSquareAhead = new Point(); //今プレイヤーが見ている方向で現在地の1マス前の座標

    private Direction currentDirection; //現在プレイヤーが向いている方向

    private bool is_dragginig; //橋のアイコンをドラッグ中はtrueになる
    private bool canClear; //クリア可能ならtrueになる

    // Start is called before the first frame update
    void Start() {
        stageManager = GameObject.Find("StageManager").GetComponent<StageManager>();
        uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
        audioSources = GetComponents<AudioSource>(); //audioSources[0]には風の音、audioSources[1]にはプレイヤーの足音が入っている

        is_dragginig = false;
        canClear = false;
        audioSources[0].volume = 0f;
        //UnityEngine.Debug.Log("PlayerController.Start: 脱出扉の座標 = (" + stageManager.GOAL_SQUARE.x + ", " + stageManager.GOAL_SQUARE.y + ", " + stageManager.GOAL_SQUARE.z + ")");
        //UnityEngine.Debug.Log("PlayerController.Start: プレイヤーの座標 = (" + point.x + ", " + point.y + ", " + point.z + ")");
    }

    public void Initialize(int x, int y, int z, Direction initialDirection) {
        gameObject.transform.position = new Vector3(x, (float)(y + 0.5), z);
        point.x = x; point.y = y; point.z = z;
        currentDirection = initialDirection;
        CollectBridgeOrWall();

        float xRotation = 10f;
        transform.Find("MinimapCamera").rotation = Quaternion.Euler(90f - xRotation, 0f, 0f);
        if (initialDirection == Direction.PositiveZ) {
            gameObject.transform.rotation = Quaternion.Euler(xRotation, 0f, 0f);
        }
        else if (initialDirection == Direction.PositiveX) {
            gameObject.transform.rotation = Quaternion.Euler(xRotation, 90f, 0f);
        }
        else if (initialDirection == Direction.NegativeZ) {
            gameObject.transform.rotation = Quaternion.Euler(xRotation, 180f, 0f);
        }
        else if (initialDirection == Direction.NegativeX) {
            gameObject.transform.rotation = Quaternion.Euler(xRotation, 270f, 0f);
        }

        CalculateDistanceFromPlayerToGoal();

        if (point.y == 1) {
            StartCoroutine(WindSoundFadeInOrFadeOut(true, 5f));
        }
    }


    public void StartFlicking() {
        touchStartPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        is_dragginig = true;
    }

    public void EndFlicking() {
        if (is_dragginig && uiManager.IS_GOING_ON) {
            touchEndPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
            ChangeDirection();
        }
        is_dragginig = false;
    }

    private void ChangeDirection() {
        movingAmountX = touchEndPos.x - touchStartPos.x; // x軸方向の移動量
        movingAmountY = touchEndPos.y - touchStartPos.y; // y軸方向の移動量
        //UnityEngine.Debug.Log(movingAmountX + " " + movingAmountY);

        if (Mathf.Abs(movingAmountX) >= Mathf.Abs(movingAmountY)) {
            /*右を向く*/
            if (movingAmountX > 0) {
                StartCoroutine(Rotate(new Vector3(0f, 90f, 0f)));
                switch (currentDirection) {
                    case Direction.PositiveX:
                        currentDirection = Direction.NegativeZ;
                        break;
                    case Direction.NegativeX:
                        currentDirection = Direction.PositiveZ;
                        break;
                    case Direction.PositiveZ:
                        currentDirection = Direction.PositiveX;
                        break;
                    case Direction.NegativeZ:
                        currentDirection = Direction.NegativeX;
                        break;
                }
            }
            /*左を向く*/
            else if (movingAmountX < 0) {
                StartCoroutine(Rotate(new Vector3(0f, -90f, 0f)));
                switch (currentDirection) {
                    case Direction.PositiveX:
                        currentDirection = Direction.PositiveZ;
                        break;
                    case Direction.NegativeX:
                        currentDirection = Direction.NegativeZ;
                        break;
                    case Direction.PositiveZ:
                        currentDirection = Direction.NegativeX;
                        break;
                    case Direction.NegativeZ:
                        currentDirection = Direction.PositiveX;
                        break;
                }
            }
        }
        else if (Mathf.Abs(movingAmountX) < Mathf.Abs(movingAmountY)) {
            /*前に進む*/
            if (movingAmountY > 0) {
                Walk();
            }
            /*後ろを向く*/
            else if (movingAmountY < 0) {
                StartCoroutine(Rotate(new Vector3(0f, 180f, 0f)));
                switch (currentDirection) {
                    case Direction.PositiveX:
                        currentDirection = Direction.NegativeX;
                        break;
                    case Direction.NegativeX:
                        currentDirection = Direction.PositiveX;
                        break;
                    case Direction.PositiveZ:
                        currentDirection = Direction.NegativeZ;
                        break;
                    case Direction.NegativeZ:
                        currentDirection = Direction.PositiveZ;
                        break;
                }
            }
        }
    }

    private void Walk() {
        /*普通に歩く*/
        switch (currentDirection) {
            case Direction.PositiveX:
                if (0 <= point.x && point.x <= 13) {
                    /*例外が出ないようにif分を入れ子にした*/
                    if (stageManager.playerSquares[point.x + 1, point.y, point.z]) {
                        StartCoroutine(Proceed(Direction.PositiveX));
                        //point.x++;
                        //CalculateDistanceFromPlayerToGoal();
                        return;
                    }
                }
                break;
            case Direction.NegativeX:
                if (1 <= point.x && point.x <= 14) {
                    if (stageManager.playerSquares[point.x - 1, point.y, point.z]) {
                        StartCoroutine(Proceed(Direction.NegativeX));
                        //point.x--;
                        //CalculateDistanceFromPlayerToGoal();
                        return;
                    }
                }
                break;
            case Direction.PositiveZ:
                if (0 <= point.z && point.z <= 13) {
                    if (stageManager.playerSquares[point.x, point.y, point.z + 1]) {
                        StartCoroutine(Proceed(Direction.PositiveZ));
                        //point.z++;
                        //CalculateDistanceFromPlayerToGoal();
                        return;
                    }
                }
                break;
            case Direction.NegativeZ:
                if (1 <= point.z && point.z <= 14) {
                    if (stageManager.playerSquares[point.x, point.y, point.z - 1]) {
                        StartCoroutine(Proceed(Direction.NegativeZ));
                        //point.z--;
                        //CalculateDistanceFromPlayerToGoal();
                        return;
                    }
                }
                break;
        }

        /*階段を上る*/
        if (point.x == 0 && point.y == 0 && point.z == 1 && currentDirection == Direction.PositiveX
                    || point.x == 8 && point.y == 0 && point.z == 2 && currentDirection == Direction.PositiveX
                    || point.x == 11 && point.y == 0 && point.z == 6 && currentDirection == Direction.PositiveZ
                    || point.x == 2 && point.y == 0 && point.z == 12 && currentDirection == Direction.NegativeZ
                    || point.x == 13 && point.y == 0 && point.z == 14 && currentDirection == Direction.NegativeZ) {
            ClimbUpLadder();
            return;
        }

        /*階段を下る*/
        else if (point.x == 1 && point.y == 1 && point.z == 1 && currentDirection == Direction.NegativeX
                    || point.x == 9 && point.y == 1 && point.z == 2 && currentDirection == Direction.NegativeX
                    || point.x == 11 && point.y == 1 && point.z == 7 && currentDirection == Direction.NegativeZ
                    || point.x == 2 && point.y == 1 && point.z == 11 && currentDirection == Direction.PositiveZ
                    || point.x == 13 && point.y == 1 && point.z == 13 && currentDirection == Direction.PositiveZ) {
            ClimbDownLadder();
        }
        //UnityEngine.Debug.Log("プレイヤーの座標 = (" + point.x + ", " + point.y + ", " + point.z + "), プレイヤーの方向 = " + currentDirection);
    }

    /*ハシゴを上る*/
    private void ClimbUpLadder() {
        /*ハシゴ1, ハシゴ2*/
        if (point.x == 0 && point.y == 0 && point.z == 1 && currentDirection == Direction.PositiveX
            || point.x == 8 && point.y == 0 && point.z == 2 && currentDirection == Direction.PositiveX) {
            Vector3 movement = new Vector3(1f, 0f, 0f);
            StartCoroutine(ClimbUp(movement));
            point.x++; point.y++;
            //UnityEngine.Debug.Log("プレイヤーの座標 = (" + point.x + ", " + point.y + ", " + point.z + "), プレイヤーの方向 = " + currentDirection);
            //UnityEngine.Debug.Log("ハシゴ1orハシゴ2を上った");
        }
        /*ハシゴ3*/
        else if (point.x == 11 && point.y == 0 && point.z == 6 && currentDirection == Direction.PositiveZ) {
            Vector3 movement = new Vector3(0f, 0f, 1f);
            StartCoroutine(ClimbUp(movement));
            point.y++; point.z++;
            //UnityEngine.Debug.Log("ハシゴ3を上った");
            //UnityEngine.Debug.Log("プレイヤーの座標 = (" + point.x + ", " + point.y + ", " + point.z + "), プレイヤーの方向 = " + currentDirection);
        }
        /*ハシゴ4, ハシゴ5*/
        else if (point.x == 2 && point.y == 0 && point.z == 12 && currentDirection == Direction.NegativeZ
            || point.x == 13 && point.y == 0 && point.z == 14 && currentDirection == Direction.NegativeZ) {
            Vector3 movement = new Vector3(0f, 0f, -1f);
            StartCoroutine(ClimbUp(movement));
            point.y++; point.z--;
            //UnityEngine.Debug.Log("プレイヤーの座標 = (" + point.x + ", " + point.y + ", " + point.z + "), プレイヤーの方向 = " + currentDirection);
            //UnityEngine.Debug.Log("ハシゴ4orハシゴ5を上った");
        }
    }

    private void ClimbDownLadder() {
        /*ハシゴ1, ハシゴ2*/
        if (point.x == 1 && point.y == 1 && point.z == 1 && currentDirection == Direction.NegativeX
            || point.x == 9 && point.y == 1 && point.z == 2 && currentDirection == Direction.NegativeX) {
            StartCoroutine(ClimbDown(new Vector3(-1f, 0f, 0f)));
            point.x--; point.y--;
            //currentDirection = Direction.PositiveX;
            //UnityEngine.Debug.Log("ハシゴ1orハシゴ2を下った");
            //UnityEngine.Debug.Log("プレイヤーの座標 = (" + point.x + ", " + point.y + ", " + point.z + "), プレイヤーの方向 = " + currentDirection);
        }
        /*ハシゴ3*/
        else if (point.x == 11 && point.y == 1 && point.z == 7 && currentDirection == Direction.NegativeZ) {
            StartCoroutine(ClimbDown(new Vector3(0f, 0f, -1f)));
            point.y--; point.z--;
            //currentDirection = Direction.PositiveZ;
            //UnityEngine.Debug.Log("ハシゴ3を下った");
            //UnityEngine.Debug.Log("プレイヤーの座標 = (" + point.x + ", " + point.y + ", " + point.z + "), プレイヤーの方向 = " + currentDirection);
        }
        /*ハシゴ4, ハシゴ5*/
        else if (point.x == 2 && point.y == 1 && point.z == 11 && currentDirection == Direction.PositiveZ
            || point.x == 13 && point.y == 1 && point.z == 13 && currentDirection == Direction.PositiveZ) {
            StartCoroutine(ClimbDown(new Vector3(0f, 0f, 1f)));
            point.y--; point.z++;
            //currentDirection = Direction.NegativeZ;
            //UnityEngine.Debug.Log("ハシゴ4orハシゴ5を下った");
            //UnityEngine.Debug.Log("プレイヤーの座標 = (" + point.x + ", " + point.y + ", " + point.z + "), プレイヤーの方向 = " + currentDirection);
        }
    }

    /*クリア処理*/
    private void Clear() {
        Point square = stageManager.GOAL_SQUARE;
        if (point.x == square.x && point.y == square.y && point.z == square.z 
            && uiManager.IS_GOING_ON) {
            uiManager.Clear();
            canClear = true;
        }
    }

    /*プレイヤーから脱出扉までの距離を計算する*/
    public void CalculateDistanceFromPlayerToGoal() { //vは移動するベクトル量
        Point goal = stageManager.GOAL_SQUARE;
        float distance = Vector3.Magnitude(new Vector3(goal.x - point.x, goal.y - point.y, goal.z - point.z));
        uiManager.UpdateMeterText(distance);
    }

    /*壁or橋を設置できるならtrueを返して実際に壁or橋を実体化させる*/
    /*壁or橋を設置できないのならfalseを返す*/
    /*橋のアイコンが0, 壁のアイコンが1*/
    public bool DropBridge() {
        /*プレイヤーがいるマスの1マス前が空いているとき*/
        if (!stageManager.bridges.ContainsKey(oneSquareAhead)) {
            /*プレイヤーが1階にいるとき*/
            if (point.y == 0) {
                switch (currentDirection) {
                    case Direction.PositiveX:
                        if (point.x <= 13) {
                            if (stageManager.playerSquares[point.x + 1, point.y, point.z] 
                                && stageManager.ogreSquares[point.x + 1, point.y, point.z]) {

                                if (point.z == 0 && !stageManager.ogreSquares[point.x + 1, point.y, point.z + 1]
                                    || 1 <= point.z && point.z <= 13 && !stageManager.ogreSquares[point.x + 1, point.y, point.z + 1] && !stageManager.ogreSquares[point.x + 1, point.y, point.z - 1]
                                    || point.z == 14 && !stageManager.ogreSquares[point.x + 1, point.y, point.z - 1]) {

                                    stageManager.playerSquares[point.x + 1, point.y, point.z] = false;
                                    stageManager.ogreSquares[point.x + 1, point.y, point.z] = false;
                                    GameObject wall = Instantiate(bridgePrefab, new Vector3(point.x + 1, transform.position.y, point.z), Quaternion.Euler(45f, 0f, 0f));
                                    wall.transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
                                    wall.transform.GetChild(0).gameObject.SetActive(false); //壁のアイコンのみアクティブにする
                                    stageManager.bridges.Add(new Point(point.x + 1, point.y, point.z), wall); //ディクショナリーに追加
                                    uiManager.UpdateBridgeImages(false); //アイテム欄内の橋のアイコンを減らす
                                    //UnityEngine.Debug.Log("PositiveX方向に壁を落としました" + "(" + (point.x + 1) + ", " + point.y + ", " + point.z + ")");
                                    return true;

                                }

                            }
                        }
                        break;
                    case Direction.PositiveZ:
                        if (point.z <= 13) {
                            if (stageManager.playerSquares[point.x, point.y, point.z + 1] 
                                && stageManager.ogreSquares[point.x, point.y, point.z + 1]) {

                                if (point.x == 0 && !stageManager.ogreSquares[point.x + 1, point.y, point.z + 1]
                                    || 1 <= point.x && point.x <= 13 && !stageManager.ogreSquares[point.x - 1, point.y, point.z + 1] && !stageManager.ogreSquares[point.x + 1, point.y, point.z + 1]
                                    || point.x == 14 && !stageManager.ogreSquares[point.x - 1, point.y, point.z + 1]) {

                                    stageManager.playerSquares[point.x, point.y, point.z + 1] = false;
                                    stageManager.ogreSquares[point.x, point.y, point.z + 1] = false;
                                    GameObject wall = Instantiate(bridgePrefab, new Vector3(point.x, transform.position.y, point.z + 1), Quaternion.Euler(-45f, 90f, 0f));
                                    wall.transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
                                    wall.transform.GetChild(0).gameObject.SetActive(false); //壁のアイコンのみアクティブにする
                                    stageManager.bridges.Add(new Point(point.x, point.y, point.z + 1), wall); //ディクショナリーに追加
                                    uiManager.UpdateBridgeImages(false); //アイテム欄内の橋のアイコンを減らす
                                    //UnityEngine.Debug.Log("PositiveZ方向に壁を落としました" + "(" + (point.x) + ", " + point.y + ", " + (point.z + 1) + ")");
                                    return true;

                                }

                            }
                        }
                        break;
                    case Direction.NegativeX:
                        if (point.x >= 1) {
                            if (stageManager.playerSquares[point.x - 1, point.y, point.z] 
                                && stageManager.ogreSquares[point.x - 1, point.y, point.z]) {

                                if (point.z == 0 && !stageManager.ogreSquares[point.x - 1, point.y, point.z + 1]
                                    || 1 <= point.z && point.z <= 13 && !stageManager.ogreSquares[point.x - 1, point.y, point.z + 1] && !stageManager.ogreSquares[point.x - 1, point.y, point.z - 1]
                                    || point.z == 14 && !stageManager.ogreSquares[point.x - 1, point.y, point.z - 1]) {

                                    stageManager.playerSquares[point.x - 1, point.y, point.z] = false;
                                    stageManager.ogreSquares[point.x - 1, point.y, point.z] = false;
                                    GameObject wall = Instantiate(bridgePrefab, new Vector3(point.x - 1, transform.position.y, point.z), Quaternion.Euler(-45f, 0f, 0f));
                                    wall.transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
                                    wall.transform.GetChild(0).gameObject.SetActive(false); //壁のアイコンのみアクティブにする
                                    stageManager.bridges.Add(new Point(point.x - 1, point.y, point.z), wall); //ディクショナリーに追加
                                    uiManager.UpdateBridgeImages(false); //アイテム欄内の橋のアイコンを減らす
                                    //UnityEngine.Debug.Log("NegativeX方向に壁を落としました" + "(" + (point.x - 1) + ", " + point.y + ", " + point.z + ")");
                                    return true;

                                }

                            }
                        }
                        break;
                    case Direction.NegativeZ:
                        if (point.z >= 1) {
                            if (stageManager.playerSquares[point.x, point.y, point.z - 1] 
                                && stageManager.ogreSquares[point.x, point.y, point.z - 1]) {

                                if (point.x == 0 && !stageManager.ogreSquares[point.x + 1, point.y, point.z - 1]
                                    || 1 <= point.x && point.x <= 13 && !stageManager.ogreSquares[point.x - 1, point.y, point.z - 1] && !stageManager.ogreSquares[point.x + 1, point.y, point.z - 1]
                                    || point.x == 14 && !stageManager.ogreSquares[point.x - 1, point.y, point.z - 1]) {

                                    stageManager.playerSquares[point.x, point.y, point.z - 1] = false;
                                    stageManager.ogreSquares[point.x, point.y, point.z - 1] = false;
                                    GameObject wall = Instantiate(bridgePrefab, new Vector3(point.x, transform.position.y, point.z - 1), Quaternion.Euler(45f, 90f, 0f));
                                    wall.transform.localScale = new Vector3(1.3f, 1.3f, 1.3f);
                                    wall.transform.GetChild(0).gameObject.SetActive(false); //壁のアイコンのみアクティブにする
                                    stageManager.bridges.Add(new Point(point.x, point.y, point.z - 1), wall); //ディクショナリーに追加
                                    uiManager.UpdateBridgeImages(false); //アイテム欄内の橋のアイコンを減らす
                                    //UnityEngine.Debug.Log("NegativeZ方向に壁を落としました" + "(" + (point.x) + ", " + point.y + ", " + (point.z - 1) + ")");
                                    return true;

                                }

                            }
                        }
                        break;
                }
                return false; //壁を設置できる条件が整っていなかったら設置できない
            }
            /*プレイヤーが2階にいるとき*/
            else if (point.y == 1) {
                switch (currentDirection) {
                    case Direction.PositiveX:
                        if (point.x <= 12) {
                            if (!stageManager.playerSquares[point.x + 1, point.y, point.z]
                                && stageManager.playerSquares[point.x + 2, point.y, point.z]) {
                                stageManager.playerSquares[point.x + 1, point.y, point.z] = true;
                                stageManager.ogreSquares[point.x + 1, point.y, point.z] = true;
                                GameObject bridge = Instantiate(bridgePrefab, new Vector3(point.x + 1, point.y, point.z), Quaternion.Euler(0f, 90f, 0f));
                                bridge.transform.GetChild(1).gameObject.SetActive(false); //橋のアイコンのみアクティブにする
                                stageManager.bridges.Add(new Point(point.x + 1, point.y, point.z), bridge); //ディクショナリーに追加
                                uiManager.UpdateBridgeImages(false); //アイテム欄内の橋のアイコンを減らす
                                //UnityEngine.Debug.Log("PositiveX方向に橋を落としました" + "(" + (point.x+1) + ", " + point.y + ", " + point.z + ")");
                                return true;
                            }
                        }
                        break;
                    case Direction.PositiveZ:
                        if (point.z <= 12) {
                            if (!stageManager.playerSquares[point.x, point.y, point.z + 1]
                                && stageManager.playerSquares[point.x, point.y, point.z + 2]
                                && !(point.x == 2 && point.z == 11)
                                && !(point.x == 11 && point.z == 5)) {
                                stageManager.playerSquares[point.x, point.y, point.z + 1] = true;
                                stageManager.ogreSquares[point.x, point.y, point.z + 1] = true;
                                GameObject bridge = Instantiate(bridgePrefab, new Vector3(point.x, point.y, point.z + 1), Quaternion.Euler(0f, 0f, 0f));
                                bridge.transform.GetChild(1).gameObject.SetActive(false); //橋のアイコンのみアクティブにする
                                stageManager.bridges.Add(new Point(point.x, point.y, point.z + 1), bridge);
                                uiManager.UpdateBridgeImages(false); //アイテム欄内の橋のアイコンを減らす
                                //UnityEngine.Debug.Log("PositiveZ方向に橋を落としました" + "(" + point.x + ", " + point.y + ", " + (point.z+1) + ")");
                                return true;
                            }
                        }
                        break;
                    case Direction.NegativeX:
                        if (point.x >= 2) {
                            if (!stageManager.playerSquares[point.x - 1, point.y, point.z]
                                && stageManager.playerSquares[point.x - 2, point.y, point.z]) {
                                stageManager.playerSquares[point.x - 1, point.y, point.z] = true;
                                stageManager.ogreSquares[point.x - 1, point.y, point.z] = true;
                                GameObject bridge = Instantiate(bridgePrefab, new Vector3(point.x - 1, point.y, point.z), Quaternion.Euler(0f, 90f, 0f));
                                bridge.transform.GetChild(1).gameObject.SetActive(false); //橋のアイコンのみアクティブにする
                                stageManager.bridges.Add(new Point(point.x - 1, point.y, point.z), bridge);
                                uiManager.UpdateBridgeImages(false); //アイテム欄内の橋のアイコンを減らす
                                //UnityEngine.Debug.Log("NegativeX方向に橋を落としました" + "(" + (point.x-1) + ", " + point.y + ", " + point.z + ")");
                                return true;
                            }
                        }
                        break;
                    case Direction.NegativeZ:
                        if (point.z >= 2) {
                            if (!stageManager.playerSquares[point.x, point.y, point.z - 1]
                                && stageManager.playerSquares[point.x, point.y, point.z - 2]
                                && !(point.x == 2 && point.z == 13)
                                && !(point.x == 11 && point.z == 7)) {
                                stageManager.playerSquares[point.x, point.y, point.z - 1] = true;
                                stageManager.ogreSquares[point.x, point.y, point.z - 1] = true;
                                GameObject bridge = Instantiate(bridgePrefab, new Vector3(point.x, point.y, point.z - 1), Quaternion.Euler(0f, 0f, 0f));
                                bridge.transform.GetChild(1).gameObject.SetActive(false); //橋のアイコンのみアクティブにする
                                stageManager.bridges.Add(new Point(point.x, point.y, point.z - 1), bridge);
                                uiManager.UpdateBridgeImages(false); //アイテム欄内の橋のアイコンを減らす
                                //UnityEngine.Debug.Log("NegativeZ方向に橋を落としました" + "(" + point.x + ", " + point.y + ", " + (point.z-1) + ")");
                                return true;
                            }
                        }
                        break;
                }
            }
            return false; //橋を設置できる条件が整っていなかったら設置できない
        }
        /*プレイヤーがいるマスの1マス前が空いているとき*/
        else {
            return false;
        }
    }

    /*橋を回収する*/
    public void CollectBridgeOrWall() {
        oneSquareAhead = point;
        switch (currentDirection) {
            case Direction.PositiveX:
                oneSquareAhead.x++;
                break;
            case Direction.PositiveZ:
                oneSquareAhead.z++;
                break;
            case Direction.NegativeX:
                oneSquareAhead.x--;
                break;
            case Direction.NegativeZ:
                oneSquareAhead.z--;
                break;
        }
        //UnityEngine.Debug.Log("一マス先の座標 = (" + oneSquareAhead.x + ", " + oneSquareAhead.y + ", " + oneSquareAhead.z + ")");
        if (stageManager.bridges.ContainsKey(oneSquareAhead)) { //視線方向で1マス先に橋があったら
            uiManager.ChangeCollectBridgeOrWallButton(true);　//「回収する」ボタンをアクティブにする
            //UnityEngine.Debug.Log("現在、橋を回収できるマスにいます" + Time.realtimeSinceStartup);
        }
        else { //視線方向で1マス先に橋がなかったら
            uiManager.ChangeCollectBridgeOrWallButton(false); //「回収する」ボタンを非アクティブにする
            //UnityEngine.Debug.Log("橋を回収できませんでした");
        }
    }

    private void PlayFootSound() {
        int number = Random.Range(0, footSound.Length);
        audioSources[1].PlayOneShot(footSound[number]);
    }

    private IEnumerator Proceed(Direction travellingDirection) {
        uiManager.ChangeCollectBridgeOrWallButton(false); //前進中は「回収する」ボタンを無効にする

        if (travellingDirection == Direction.PositiveX) {
            point.x++;
        }
        else if (travellingDirection == Direction.PositiveZ) {
            point.z++;
        }
        else if (travellingDirection == Direction.NegativeX) {
            point.x--;
        }
        else if (travellingDirection == Direction.NegativeZ) {
            point.z--;
        }

        Clear(); //クリアの条件を満たしているかチェックする

        Vector3 v = new Vector3();
        if (travellingDirection == Direction.PositiveX) {
            v = new Vector3(1f, 0f, 0f);
        }
        else if (travellingDirection == Direction.PositiveZ) {
            v = new Vector3(0f, 0f, 1f);
        }
        else if (travellingDirection == Direction.NegativeX) {
            v = new Vector3(-1f, 0f, 0f);
        }
        else if (travellingDirection == Direction.NegativeZ) {
            v = new Vector3(0f, 0f, -1f);
        }

        PlayFootSound();
        for (int i = 0; i < 10; i++) {
            gameObject.transform.Translate(v / 10f, Space.World);
            yield return new WaitForSeconds(0.025f);
        }

        CollectBridgeOrWall();
        CalculateDistanceFromPlayerToGoal();
        if (canClear) {
            StartCoroutine(uiManager.DisplayClearBelt());
        }
    }

    private IEnumerator Rotate(Vector3 v) {
        uiManager.ChangeCollectBridgeOrWallButton(false); //回転中は「回収する」のボタンを無効にする

        PlayFootSound();
        for (int i = 0; i < 10; i++) {
            gameObject.transform.Rotate(v / 10f, Space.World);
            yield return new WaitForSeconds(0.018f);
        }
        CollectBridgeOrWall();
        //UnityEngine.Debug.Log("プレイヤーの座標 = (" + point.x + ", " + point.y + ", " + point.z + "), プレイヤーの方向 = " + currentDirection);
    }

    private IEnumerator ClimbUp(Vector3 v) {
        PlayFootSound();
        StartCoroutine(WindSoundFadeInOrFadeOut(true, 5f));
        for (int i = 0; i < 20; i++) {
            gameObject.transform.Translate(0.05f * new Vector3(0f, 1f, 0f) + 0.05f * v, Space.World);
            yield return new WaitForSeconds(0.02f);
        }
    }

    private IEnumerator ClimbDown(Vector3 v) {
        PlayFootSound();
        StartCoroutine(WindSoundFadeInOrFadeOut(false, 5f));
        for (int i = 0; i < 20; i++) {
            gameObject.transform.Translate(0.05f * v + new Vector3(0f, -0.05f, 0f), Space.World);
            yield return new WaitForSeconds(0.02f);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "Ogre") {
            canClear = false;
            uiManager.GameOver();
        }
    }

    private IEnumerator WindSoundFadeInOrFadeOut(bool fadeIn, float fadeTime) {
        if (fadeIn) {
            for (int i = 0; i < 10; i++) {
                audioSources[0].volume += 0.1f;
                yield return new WaitForSeconds(fadeTime / 10f);
            }
        }
        else {
            for (int i = 0; i < 10; i++) {
                audioSources[0].volume -= 0.1f;
                yield return new WaitForSeconds(fadeTime / 10f);
            }
        }
    }
}
