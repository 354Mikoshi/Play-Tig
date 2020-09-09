using UnityEngine;
using UnityEngine.U2D;

public class PlayerMarkController : MonoBehaviour
{
    [SerializeField]
    private SpriteAtlas UISpriteAtlas;

    void Start() {
        gameObject.GetComponent<SpriteRenderer>().sprite = UISpriteAtlas.GetSprite("player_mark");
    }
}
