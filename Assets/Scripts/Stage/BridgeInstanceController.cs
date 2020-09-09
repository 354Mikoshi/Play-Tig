using UnityEngine;

public class BridgeInstanceController : MonoBehaviour
{
    private Transform wallIcon;

    void Start() {
        wallIcon = transform.Find("Wall Icon");
        wallIcon.rotation = Quaternion.Euler(90f, 90f, 0f);
    }

    public void DestroyThis() {
        Destroy(gameObject);
    }
}
