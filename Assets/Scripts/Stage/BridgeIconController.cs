using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(Image))]
public class BridgeIconController : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler/*, IPointerEnterHandler, IPointerExitHandler, IDeselectHandler*/
{
    private PlayerController playerController;
    private UIManager uiManager;

    private Vector2 previousPosition;

    void Start() {
        playerController = Camera.main.transform.root.gameObject.GetComponent<PlayerController>();
        uiManager = GameObject.Find("UIManager").GetComponent<UIManager>();
    }

    public void OnBeginDrag(PointerEventData eventData) {
        previousPosition = transform.position;
    }

    public void OnDrag(PointerEventData eventData) {
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData) {
        var raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventData, raycastResults);

        foreach (RaycastResult hit in raycastResults) {
            /*橋を正しい場所に投下し、橋がすでに設置されていないマスだったら橋を新たに設置できる*/
            if (hit.gameObject.CompareTag("DropBridge") && playerController.DropBridge()) {
                //gameObject.SetActive(false);
                uiManager.ChangeCollectBridgeOrWallButton(true);
            }
        }

        transform.position = previousPosition;
    }
}
