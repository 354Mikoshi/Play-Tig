using UnityEngine;
using UnityEngine.U2D;

public class StairMarkController : MonoBehaviour
{
    [SerializeField]
    private SpriteAtlas UISpriteAtlas;

    void Start() {
        gameObject.GetComponent<SpriteRenderer>().sprite = UISpriteAtlas.GetSprite("stair_mark");
    }
}
