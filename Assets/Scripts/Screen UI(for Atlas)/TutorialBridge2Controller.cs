using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class TutorialBridge2Controller : MonoBehaviour
{
    [SerializeField]
    private SpriteAtlas UISpriteAtlas;

    void Start() {
        gameObject.GetComponent<Image>().sprite = UISpriteAtlas.GetSprite("bridge2");
    }
}
