using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class HotspotPositionCalc : MonoBehaviour {

	[Range(1,500)]
	public float distance = 1f;
	[Range(0,180)]
	public float pitch = 0;
	[Range(-180,180)]
	public float yaw = 0;

	Transform hotspotCenter;

	public void Apply() {
		Vector3 pos;
		pos.x = Mathf.Cos(Mathf.Deg2Rad * yaw) * Mathf.Sin(Mathf.Deg2Rad * pitch);
		pos.y = Mathf.Cos(Mathf.Deg2Rad * pitch);
		pos.z = Mathf.Sin(Mathf.Deg2Rad * yaw) * Mathf.Sin(Mathf.Deg2Rad * pitch);
		pos *= distance;

		transform.position = pos;
        if (hotspotCenter == null)
        {
            hotspotCenter = transform.parent;
        }
		transform.LookAt(hotspotCenter.position);
	}
}

[CustomEditor(typeof(HotspotPositionCalc))]
public class HotSpotInspector : Editor {

	float dist, p, y;
	HotspotPositionCalc cTarg;

	void OnEnable() {
		cTarg = target as HotspotPositionCalc;
		dist = cTarg.distance;
		p = cTarg.pitch;
		y = cTarg.yaw;
	}

	public override void OnInspectorGUI() {
		base.OnInspectorGUI();

		if ( dist != cTarg.distance || p != cTarg.pitch || y != cTarg.yaw ) {
			cTarg.Apply();
		}
	} 
}
