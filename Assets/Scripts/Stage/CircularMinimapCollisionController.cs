using UnityEngine;
using UnityEngine.UI;

public class CircularMinimapCollisionController : Button, ICanvasRaycastFilter
{
	public float radius = 100f;

	public bool IsRaycastLocationValid(Vector2 sp, Camera eventCamera) {
		return Vector2.Distance(sp, transform.position) < radius;
	}
}
