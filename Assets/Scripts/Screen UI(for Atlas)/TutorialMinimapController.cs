using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class TutorialMinimapController : MonoBehaviour
{
    [SerializeField]
    private SpriteAtlas UISpriteAtlas;

    void Start() {
        gameObject.GetComponent<Image>().sprite = UISpriteAtlas.GetSprite("minimap");
    }
}
