using UnityEngine;

public class RectTransformSizeController : MonoBehaviour
{
    void Start() {
        /*親のCanvasのRectTransformの大きさに合わせる*/
        GetComponent<RectTransform>().sizeDelta = transform.parent.gameObject.GetComponent<RectTransform>().sizeDelta;
    }
}
