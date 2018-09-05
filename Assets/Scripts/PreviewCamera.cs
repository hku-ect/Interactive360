using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreviewCamera : MonoBehaviour {
	static GameObject sceneCam;
	Camera cam;

	void Awake() {
		gameObject.SetActive(false);
	}

	void OnDrawGizmos() {
		if ( sceneCam == null ) {
			sceneCam = GameObject.Find("SceneCamera");
		}
		if ( cam == null ) {
			cam = GetComponent<Camera>();
		}
		if ( cam && sceneCam ) {
			cam.transform.rotation  = sceneCam.transform.rotation;
		}
	}
}
