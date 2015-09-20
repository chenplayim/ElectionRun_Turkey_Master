using UnityEngine;
using System.Collections;

public class FoliageController : MonoBehaviour {

	public Transform _transform;
	public float hwidth;
	public float width;
	public float height;

	void Awake() {
		_transform = this.transform;

		Vector2 size = GetComponent<SpriteRenderer>().bounds.size;
		hwidth = size.x * 0.5f;
		width = size.x;
		height = size.y;
	}
}
