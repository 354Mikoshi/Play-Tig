using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

public class FenceController : MonoBehaviour
{
    [SerializeField]
    private SpriteAtlas UISpriteAtlas;

    void Start() {
        gameObject.GetComponent<SpriteRenderer>().sprite = UISpriteAtlas.GetSprite("Fence");
    }
}
