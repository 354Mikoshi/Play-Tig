using UnityEngine;

public class CameraScaleManager : MonoBehaviour
{
    private Camera cam;

    // 固定したい表示サイズ
    private float width = 1000f;
    private float height = 500f;

    // 画像のPixel Per Unit
    private float pixelPerUnit = 100f;

    //カメラのSize設定が height / 2 / picelParUnit である必要がある
    //picelParUnitが 200 で height が 1920 なら カメラのサイズは 4.8になる。

    void Awake() {
        float aspect = (float)Screen.height / (float)Screen.width; //表示画面のアスペクト比
        float bgAcpect = height / width; //理想とするアスペクト比

        // カメラコンポーネントを取得します
        cam = GetComponent<Camera>();

        // カメラのorthographicSizeを設定
        cam.orthographicSize = (height / 2f / pixelPerUnit);

        if (bgAcpect > aspect) {
            //画面が横に広いとき
            // 倍率
            float bgScale = height / Screen.height;
            // viewport rectの幅
            float camWidth = width / (Screen.width * bgScale);
            // viewportRectを設定
            cam.rect = new Rect((1f - camWidth) / 2f, 0f, camWidth, 1f);

        }
        else {
            //画面が縦に長い
            //想定しているアスペクト比とどれだけ差があるかを出す
            float bgScale = aspect / bgAcpect;

            // カメラのorthographicSizeを縦の長さに合わせて設定しなおす
            cam.orthographicSize *= bgScale;

            // viewportRectを設定
            cam.rect = new Rect(0f, 0f, 1f, 1f);
        }
    }
}