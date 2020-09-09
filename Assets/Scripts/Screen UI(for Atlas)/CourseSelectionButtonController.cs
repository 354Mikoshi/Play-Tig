﻿using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class CourseSelectionButtonController : MonoBehaviour
{
    [SerializeField]
    private SpriteAtlas UISpriteAtlas;

    void Start() {
        gameObject.GetComponent<Image>().sprite = UISpriteAtlas.GetSprite("CourseSelectionButton");
    }
}
