﻿using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class TutorialWhiteOgreController : MonoBehaviour
{
    [SerializeField]
    private SpriteAtlas UISpriteAtlas;

    void Start() {
        gameObject.GetComponent<Image>().sprite = UISpriteAtlas.GetSprite("white ogre");
    }
}
