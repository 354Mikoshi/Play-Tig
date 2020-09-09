using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class CircleFrameController : MonoBehaviour
{
    [SerializeField]
    private SpriteAtlas UISpriteAtlas;

    void Start() {
        gameObject.GetComponent<Image>().sprite = UISpriteAtlas.GetSprite("frame");
    }
}
