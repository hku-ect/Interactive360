using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PositionHack : MonoBehaviour {

	// Use this for initialization
	void Start () {
        //UnityEngine.XR.InputTracking.disablePositionalTracking = true;
       // transform.position = Vector3.zero;
    }
	
#if UNITY_EDITOR
    static GameObject sceneCam;
    Camera cam;

    void OnDrawGizmos() {
        if (!UnityEditor.PlayerSettings.virtualRealitySupported)
        {
            if (sceneCam == null)
            {
                sceneCam = GameObject.Find("SceneCamera");
            }
            if (cam == null)
            {
                cam = GetComponent<Camera>();
            }
            if (cam && sceneCam)
            {
                cam.transform.rotation = sceneCam.transform.rotation;
            }
        }
    }
#endif
}
