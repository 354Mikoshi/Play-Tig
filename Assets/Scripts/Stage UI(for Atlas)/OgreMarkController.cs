using UnityEngine;
using UnityEngine.U2D;

public class OgreMarkController : MonoBehaviour
{
    [SerializeField]
    private SpriteAtlas UISpriteAtlas;

    void Start() {
        gameObject.GetComponent<SpriteRenderer>().sprite = UISpriteAtlas.GetSprite("ogre_mark");
    }
}
