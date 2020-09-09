using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class TutorialItemStorageController : MonoBehaviour
{
    [SerializeField]
    private SpriteAtlas UISpriteAtlas;

    void Start() {
        gameObject.GetComponent<Image>().sprite = UISpriteAtlas.GetSprite("item storage");
    }
}
