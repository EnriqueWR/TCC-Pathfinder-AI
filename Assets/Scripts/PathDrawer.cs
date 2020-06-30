using UnityEngine;

public class PathDrawer : MonoBehaviour {
	public LineRenderer lRenderer;

	public void SetParent(GameObject parent) {
		if (parent == null) {
			return;
		}

		Vector3[] positions = { transform.position, parent.transform.position };
		lRenderer.SetPositions(positions);
	}
}