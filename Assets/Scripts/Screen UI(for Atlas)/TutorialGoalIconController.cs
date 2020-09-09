using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class TutorialGoalIconController : MonoBehaviour
{
    [SerializeField]
    private SpriteAtlas UISpriteAtlas;

    void Start() {
        gameObject.GetComponent<Image>().sprite = UISpriteAtlas.GetSprite("target_4_spritesheet_0");
    }
}
