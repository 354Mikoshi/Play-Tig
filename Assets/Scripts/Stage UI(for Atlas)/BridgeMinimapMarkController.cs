using UnityEngine;
using UnityEngine.U2D;

public class BridgeMinimapMarkController : MonoBehaviour
{
    [SerializeField]
    private SpriteAtlas UISpriteAtlas;

    void Start() {
        gameObject.GetComponent<SpriteRenderer>().sprite = UISpriteAtlas.GetSprite("bridge_mark");
    }
}
