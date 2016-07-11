using UnityEngine;
using System.Collections;

public class PinchZoom : MonoBehaviour {

	public float perspectiveZoomSpeed = 0.5f;        // The rate of change of the field of view in perspective mode.
	public float orthoZoomSpeed = 0.5f;        // The rate of change of the orthographic size in orthographic mode.


	void Update()
	{
		// If there are two touches on the device...
		if (Input.touchCount == 2)
		{
			// Store both touches.
			Touch touchZero = Input.GetTouch(0);
			Touch touchOne = Input.GetTouch(1);

			// Find the position in the previous frame of each touch.
			Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
			Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

			// Find the magnitude of the vector (the distance) between the touches in each frame.
			float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
			float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

			// Find the difference in the distances between each frame.
			float deltaMagnitudeDiff = prevTouchDeltaMag - touchDeltaMag;
			zoom (deltaMagnitudeDiff);
		}

		if (Input.GetKey (KeyCode.Z)) {
			Debug.Log ("Z key was pressed.");
			zoom (-.1f);
		}

		if (Input.GetKey (KeyCode.X)) {
			Debug.Log ("X key was pressed.");
			zoom (.1f);
		}
	}

	private void zoom(float magnitude){

		Camera cam = GetComponent<Camera>();

		if (cam.orthographic)
		{
			cam.orthographicSize += magnitude * orthoZoomSpeed;
			cam.orthographicSize = Mathf.Max(cam.orthographicSize, 0.1f);
		}
		else
		{
			cam.fieldOfView += magnitude * perspectiveZoomSpeed;
			cam.fieldOfView = Mathf.Clamp(cam.fieldOfView, 0.1f, 179.9f);
		}
	}
}
