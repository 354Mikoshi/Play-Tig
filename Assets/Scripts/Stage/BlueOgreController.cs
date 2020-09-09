using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BlueOgreController : OgreController
{
    private Behaviour[] ogreBehaviours;
    private int index; //行動順序を決める数字

    void Awake() {
        ogreBehaviours = new Behaviour[0];
        time = 0f;
        index = 0;

        stageManager = GameObject.Find("StageManager").GetComponent<StageManager>();
        uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();

        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        isLeft = UnityEngine.Random.Range(0, 2) == 0; //初めに上げる足はランダムに決める
    }

    void Update() {
        if (uiManager.IS_GOING_ON) {
            time += Time.deltaTime;
        }

        if (time > 1f) {
            CirclePathway();
            time = 0f;
        }
    }

    /*青鬼が周回する経路を設定する*/
    public void SetPathway(Behaviour[] ogreBehaviours) {
        this.ogreBehaviours = new Behaviour[ogreBehaviours.Length];
        Array.Copy(ogreBehaviours, this.ogreBehaviours, ogreBehaviours.Length);
    }

    /*青鬼が設定された経路上を周回する*/
    private void CirclePathway() {
        if (ogreBehaviours.Length > 0) {
            if (ogreBehaviours[index] == Behaviour.TurnToTheRight) {
                StartCoroutine(Rotate(Behaviour.TurnToTheRight));
                index = (index + 1) % ogreBehaviours.Count();
            }
            else if (ogreBehaviours[index] == Behaviour.GoForward) {
                if (currentDirection == Direction.PositiveZ) {
                    if (point.z <= 13) {
                        if (stageManager.ogreSquares[point.x, point.y, point.z + 1]) {
                            StartCoroutine(Proceed(currentDirection));
                            index = (index + 1) % ogreBehaviours.Count();
                        }
                    }
                }
                else if (currentDirection == Direction.PositiveX) {
                    if (point.x <= 13) {
                        if (stageManager.ogreSquares[point.x + 1, point.y, point.z]) {
                            StartCoroutine(Proceed(currentDirection));
                            index = (index + 1) % ogreBehaviours.Count();
                        }
                    }
                }
                else if (currentDirection == Direction.NegativeZ) {
                    if (point.z >= 1) {
                        if (stageManager.ogreSquares[point.x, point.y, point.z - 1]) {
                            StartCoroutine(Proceed(currentDirection));
                            index = (index + 1) % ogreBehaviours.Count();
                        }
                    }
                }
                else if (currentDirection == Direction.NegativeX) {
                    if (point.x >= 1) {
                        if (stageManager.ogreSquares[point.x - 1, point.y, point.z]) {
                            StartCoroutine(Proceed(currentDirection));
                            index = (index + 1) % ogreBehaviours.Count();
                        }
                    }
                }
            }
            else if (ogreBehaviours[index] == Behaviour.TurnToTheLeft) {
                StartCoroutine(Rotate(Behaviour.TurnToTheLeft));
                index = (index + 1) % ogreBehaviours.Count();
            }
        }
    }
}
