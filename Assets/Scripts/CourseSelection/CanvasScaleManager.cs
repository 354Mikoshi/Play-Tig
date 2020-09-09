using UnityEngine;
using UnityEngine.UI;

public class CanvasScaleManager : MonoBehaviour
{
    [SerializeField]
    private float idealRatio = 2f;

    private float actualRatio;
    private CanvasScaler canvasScaler;

    void Start() {
        canvasScaler = GetComponent<CanvasScaler>();
        Debug.Log("Height = " + Screen.height + ", Width = " + Screen.width);
        actualRatio = (float)Screen.width / Screen.height;

        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.referenceResolution = new Vector2(1000f, 500f);
        canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        
        if (actualRatio > idealRatio) {
            canvasScaler.matchWidthOrHeight = 1f;
            Debug.Log("This is HUAWEI P30 lite.");
        }
        else {
            canvasScaler.matchWidthOrHeight = 0f;
            Debug.Log("This is iPhone7.");
        }
        
        canvasScaler.referencePixelsPerUnit = 100f;
    }

    void Update() {

    }
}
