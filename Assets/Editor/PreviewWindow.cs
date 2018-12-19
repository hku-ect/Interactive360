using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine.Video;

public class PreviewWindow : EditorWindow {

	[MenuItem("Window/360-Tool/Preview")]
	public static void Create() {
		EditorWindow.GetWindow<PreviewWindow>();
	}

	Camera cam;
	RenderTexture rt;

	Rect r = new Rect(0,0,100,100);

	void OnEnable() {
		cam = Camera.main;
		EditorSceneManager.activeSceneChangedInEditMode += UpdateCamera;
		EditorApplication.update += Repaint;
		Selection.selectionChanged += UpdateSelection;
	}

	void OnDisable() {
		EditorSceneManager.activeSceneChangedInEditMode -= UpdateCamera;
		EditorApplication.update -= Repaint;
	}

	void UpdateSelection() {
		if ( Selection.activeGameObject != null ) {
			HotspotPositionCalc hpc = Selection.activeGameObject.GetComponent<HotspotPositionCalc>();
			if ( hpc ) {
				cam.transform.LookAt(hpc.transform.position);
			}
		}
	}

	void UpdateCamera( Scene oldScene, Scene newScene ) {
		cam = Camera.main;
		if ( cam == null ) {
			//check if there's a preview camera
			PreviewCamera pcam = FindObjectOfType<PreviewCamera>();
			if ( pcam ) {
				cam = pcam.GetComponent<Camera>();
			}
		}
	}

	void OnGUI() {
		if ( cam != null ) {
			rt = RenderTexture.GetTemporary((int)position.width, (int)position.height, 24);
			
			cam.targetTexture = rt;
			cam.Render();
			cam.targetTexture = null;

			r.width = position.width;
			r.height = position.height;

			GUI.DrawTexture(r, rt);

			RenderTexture.ReleaseTemporary(rt);

			BeginWindows();
			if ( DisplayFrameStatistics.instance ) {
				DisplayFrameStatistics.instance.windowRect = GUI.Window(0, DisplayFrameStatistics.instance.windowRect, DisplayFrameStatistics.instance.SceneWindow, "Preview Controls" );
			}
			if ( HotspotEditor.instance ) {
				HotspotEditor.instance.rect = GUI.Window(0, HotspotEditor.instance.rect, HotspotEditor.instance.GUIWindow, "Hotspot Controls" );
			}
			EndWindows();

			if ( Event.current.type == EventType.MouseDrag ) {
				float yaw = Event.current.delta.x * .2f;
				float pitch = Event.current.delta.y * .2f;
				Vector3 euler = cam.transform.eulerAngles;
				euler.x -= pitch;
				euler.y -= yaw;
				euler.z = 0;
				cam.transform.rotation = Quaternion.Euler(euler);
			}
		}
		else {
			cam = Camera.main;
		}
	}

	void Window(int id) {
		GUILayout.Button("AAAAAA");
		GUI.DragWindow();
	}
}
