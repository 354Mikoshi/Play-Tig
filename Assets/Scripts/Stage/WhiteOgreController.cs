using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhiteOgreController : OgreController
{
    void Awake() {
        stageManager = GameObject.Find("StageManager").GetComponent<StageManager>();
        uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
    }

    void Update() {

    }
}
