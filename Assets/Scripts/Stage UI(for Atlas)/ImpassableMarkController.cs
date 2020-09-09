using UnityEngine;
using UnityEngine.U2D;

public class ImpassableMarkController : MonoBehaviour
{
    [SerializeField]
    private SpriteAtlas UISpriteAtlas;

    void Start() {
        gameObject.GetComponent<SpriteRenderer>().sprite = UISpriteAtlas.GetSprite("impassable_mark");
    }
}
