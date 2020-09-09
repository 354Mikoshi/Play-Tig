using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class TutorialOgreIconController : MonoBehaviour
{
    [SerializeField]
    private SpriteAtlas UISpriteAtlas;

    void Start() {
        gameObject.GetComponent<Image>().sprite = UISpriteAtlas.GetSprite("ogre_mark");
    }
}
