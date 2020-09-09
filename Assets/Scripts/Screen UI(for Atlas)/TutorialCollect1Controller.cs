using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class TutorialCollect1Controller : MonoBehaviour
{
    [SerializeField]
    private SpriteAtlas UISpriteAtlas;

    void Start() {
        gameObject.GetComponent<Image>().sprite = UISpriteAtlas.GetSprite("collect1");
    }
}
