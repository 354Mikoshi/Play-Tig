using System.Collections;
using UnityEngine;

public enum Behaviour { TurnToTheRight, GoForward, TurnToTheLeft }

public class OgreController : MonoBehaviour
{
    protected Point point = new Point(); //現在鬼がいる座標
    protected Direction currentDirection; //現在鬼が向いている方向
    protected StageManager stageManager;
    protected UIManager uiManager;
    protected Animator animator;
    protected AudioSource audioSource;
    protected bool isLeft; //次に左足を上げるアニメーションをするならtrueになる
    protected float time; //何秒に一回行動するか決めるときに使う

    void Start() {
    }

    void Update() {

    }

    public void Initialize(int x, int y, int z, Direction initialDirection) {
        point = new Point(x, y, z);
        currentDirection = initialDirection;
        stageManager.ogreSquares[x, y, z] = false;
    }

    protected IEnumerator Proceed(Direction travellingDirection) {

        Vector3 v = new Vector3();
        if (travellingDirection == Direction.PositiveX) {
            v = new Vector3(1f, 0f, 0f);
            stageManager.ogreSquares[point.x + 1, point.y, point.z] = false; //これから進む先のマスに他の鬼が侵入できないようにする
        }
        else if (travellingDirection == Direction.PositiveZ) {
            v = new Vector3(0f, 0f, 1f);
            stageManager.ogreSquares[point.x, point.y, point.z + 1] = false; //これから進む先のマスに他の鬼が侵入できないようにする
        }
        else if (travellingDirection == Direction.NegativeX) {
            v = new Vector3(-1f, 0f, 0f);
            stageManager.ogreSquares[point.x - 1, point.y, point.z] = false; //これから進む先のマスに他の鬼が侵入できないようにする
        }
        else if (travellingDirection == Direction.NegativeZ) {
            v = new Vector3(0f, 0f, -1f);
            stageManager.ogreSquares[point.x, point.y, point.z - 1] = false; //これから進む先のマスに他の鬼が侵入できないようにする
        }
        stageManager.ogreSquares[point.x, point.y, point.z] = true; //もといたマスに他の鬼が侵入できるようにする

        /*座標を変更する*/
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

        /*アニメーション*/
        if (isLeft) {
            animator.SetTrigger("StartWalkingLeft");
            isLeft = false;
        }
        else {
            animator.SetTrigger("StartWalkingRight");
            isLeft = true;
        }

        /*足音を再生する*/
        audioSource.Play();

        /*実際に動く*/
        for (int i = 0; i < 10; i++) {
            gameObject.transform.Translate(v / 10f, Space.World);
            yield return new WaitForSeconds(0.025f);
        }
    }

    protected IEnumerator Rotate(Behaviour behaviour) {

        /*アニメーション*/
        if (isLeft) {
            animator.SetTrigger("StartWalkingLeft");
            isLeft = false;
        }
        else {
            animator.SetTrigger("StartWalkingRight");
            isLeft = true;
        }

        /*足音を再生する*/
        audioSource.Play();

        if (behaviour == Behaviour.TurnToTheLeft) {
            for (int i = 0; i < 10; i++) {
                gameObject.transform.Rotate(new Vector3(0f, -90f, 0f) / 10f, Space.World);
                yield return new WaitForSeconds(0.018f);
            }
            if (currentDirection == Direction.PositiveZ) {
                currentDirection = Direction.NegativeX;
            }else if (currentDirection == Direction.PositiveX) {
                currentDirection = Direction.PositiveZ;
            }else if (currentDirection == Direction.NegativeZ) {
                currentDirection = Direction.PositiveX;
            }else if (currentDirection == Direction.NegativeX) {
                currentDirection = Direction.NegativeZ;
            }
        }
        else if (behaviour == Behaviour.TurnToTheRight) {
            for (int i = 0; i < 10; i++) {
                gameObject.transform.Rotate(new Vector3(0f, 90f, 0f) / 10f, Space.World);
                yield return new WaitForSeconds(0.018f);
            }
            if (currentDirection == Direction.PositiveZ) {
                currentDirection = Direction.PositiveX;
            }
            else if (currentDirection == Direction.PositiveX) {
                currentDirection = Direction.NegativeZ;
            }
            else if (currentDirection == Direction.NegativeZ) {
                currentDirection = Direction.NegativeX;
            }
            else if (currentDirection == Direction.NegativeX) {
                currentDirection = Direction.PositiveZ;
            }
        }
    }
}
