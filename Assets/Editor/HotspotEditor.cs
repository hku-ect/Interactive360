using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Interactive360;

[CustomEditor(typeof(HotspotButtonGaze))]
public class HotspotEditor : Editor {
	internal static HotspotEditor instance;
	internal Rect rect = new Rect(20,20,300,400);

	HotspotButtonGaze mTarget;
	HotspotPositionCalc posCalc;
	SerializedObject posCalcSO;
	Vector2 scrollPos = Vector2.zero;

	bool posOpen = true;
	bool visibilityOpen = true;
	bool actionsOpen = true;

	void OnEnable() {
		instance = this;
		mTarget = target as HotspotButtonGaze;
		posCalc = mTarget.GetComponent<HotspotPositionCalc>();
		posCalcSO = new SerializedObject(posCalc);
	}

	void OnDisable() {
		instance = null;
	}

	internal void GUIWindow( int id ) {
		Position();

		GUILayout.Box("", GUILayout.Height(1), GUILayout.Width(290));

		Visibility();

		GUILayout.Box("", GUILayout.Height(1), GUILayout.Width(290));

		Interactions();
		
		GUI.DragWindow();
	}

	void Position() {
		if ( GUILayout.Button("Position" + ( posOpen ? "" : " (click to expand)" ), EditorStyles.boldLabel) ) {
			posOpen = !posOpen;
		}

		if ( !posOpen ) {
			return;
		}

		EditorGUI.BeginChangeCheck();

		if ( posCalcSO == null ) {
			posCalc = mTarget.GetComponent<HotspotPositionCalc>();
			posCalcSO = new SerializedObject(posCalc);
		}

		posCalcSO.Update();
		posCalc.Apply();

		SerializedProperty distance = posCalcSO.FindProperty("distance");
		SerializedProperty pitch = posCalcSO.FindProperty("pitch");
		SerializedProperty yaw = posCalcSO.FindProperty("yaw");

		EditorGUI.indentLevel++;

		GUILayout.BeginHorizontal();
		GUILayout.Label("Distance: ", GUILayout.Width(65));
		distance.floatValue = GUILayout.HorizontalSlider(distance.floatValue, 1, 500);
		GUILayout.Label(distance.floatValue.ToString("F1"), GUILayout.Width(40));
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		GUILayout.Label("Pitch: ", GUILayout.Width(65));
		pitch.floatValue = GUILayout.HorizontalSlider(pitch.floatValue, 0, 180);
		GUILayout.Label(pitch.floatValue.ToString("F1"), GUILayout.Width(40));
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		GUILayout.Label("Yaw: ", GUILayout.Width(65));
		yaw.floatValue = GUILayout.HorizontalSlider(yaw.floatValue, -180, 180);
		GUILayout.Label(yaw.floatValue.ToString("F1"), GUILayout.Width(40));
		GUILayout.EndHorizontal();

		EditorGUI.indentLevel--;

		if ( EditorGUI.EndChangeCheck() ) {
			Undo.RecordObject(posCalcSO.targetObject, "Hotspot Position");
			posCalcSO.ApplyModifiedProperties();
			Repaint();
		}
	}

	void Visibility() {
		if ( GUILayout.Button("Visibility" + ( visibilityOpen ? "" : " (click to expand)" ), EditorStyles.boldLabel) ) {
			visibilityOpen = !visibilityOpen;
		}

		if ( !visibilityOpen ) {
			return;
		}
		
		EditorGUI.BeginChangeCheck();
		serializedObject.Update();

		//m_GazeTime, visible, permanent, startFrame, endFrame
		SerializedProperty m_GazeTime = serializedObject.FindProperty("m_GazeTime");
		EditorGUILayout.PropertyField(m_GazeTime);

		SerializedProperty visible = serializedObject.FindProperty("visible");
		EditorGUILayout.PropertyField(visible);
		
		SerializedProperty permanent = serializedObject.FindProperty("permanent");
		EditorGUILayout.PropertyField(permanent);

		if ( !permanent.boolValue ) {
			SerializedProperty startFrame = serializedObject.FindProperty("startFrame");
			EditorGUILayout.PropertyField(startFrame);
			SerializedProperty endFrame = serializedObject.FindProperty("endFrame");
			EditorGUILayout.PropertyField(endFrame);
		}

		if ( EditorGUI.EndChangeCheck() ) {
			Undo.RecordObject(posCalcSO.targetObject, "Hotspot Visibility");
			serializedObject.ApplyModifiedProperties();
		}
	}

	void Interactions() {
		if ( GUILayout.Button("Interactions" + ( actionsOpen ? "" : " (click to expand)" ), EditorStyles.boldLabel) ) {
			actionsOpen = !actionsOpen;
		}

		if ( !actionsOpen ) {
			return;
		}

		EditorGUI.BeginChangeCheck();
		scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

		SerializedProperty data = serializedObject.FindProperty("interactionData");
		for( int i = 0; i < data.arraySize; ++i ) {
			SerializedProperty elem = data.GetArrayElementAtIndex(i);
			SerializedProperty interaction = elem.FindPropertyRelative("interaction");
			string actionName = ((HotspotInteraction)interaction.intValue).ToString();
			actionName = actionName.Split('_')[1];
			bool open = EditorGUILayout.PropertyField(elem, new GUIContent("Action "+(i+1)+ ": "+actionName), GUILayout.Width(215));
			if ( open ) {
				//render delete box
				Rect r = GUILayoutUtility.GetLastRect();
				r.x += r.width;
				r.width = 50;
				if ( GUI.Button( r, "Delete") ) {
					data.DeleteArrayElementAtIndex(i);
					break;	//exit for loop when changing array
				}

				EditorGUI.indentLevel++;
				if ( RenderHotspotData(elem) ) {
					SerializedProperty transition = serializedObject.FindProperty("transition");
					EditorGUILayout.PropertyField(transition);
					if ((SceneTransitionStyle)transition.intValue == SceneTransitionStyle.START_FROM_FRAME) {
						SerializedProperty targetFrame = serializedObject.FindProperty("targetFrame");
						EditorGUILayout.PropertyField(targetFrame);
					}
				}
				EditorGUI.indentLevel--;
			}
		}

		EditorGUILayout.EndScrollView();

		if (GUILayout.Button("Add", GUILayout.Width(50))) {
			data.arraySize += 1;
		}

		if ( EditorGUI.EndChangeCheck() ) {
			Undo.RecordObject(posCalcSO.targetObject, "Hotspot Interaction");
			serializedObject.ApplyModifiedProperties();
		}
	}

	bool RenderHotspotData( SerializedProperty elem ) {
		SerializedProperty interaction = elem.FindPropertyRelative("interaction");
		EditorGUILayout.PropertyField(interaction, GUIContent.none, GUILayout.Width(125));

		SerializedProperty delay = elem.FindPropertyRelative("delay");
		EditorGUILayout.PropertyField(delay);
		
		bool containsScene = false;

		switch( (HotspotInteraction)interaction.intValue ) {
			case HotspotInteraction.LOAD_SCENE:
				SerializedProperty targetScene = elem.FindPropertyRelative("targetScene");
				EditorGUILayout.PropertyField(targetScene);
				containsScene = true;
			break;
			case HotspotInteraction.PLAY_ANIMATION:
				SerializedProperty targetAnimator = elem.FindPropertyRelative("targetAnimator");
				EditorGUILayout.PropertyField(targetAnimator);
				SerializedProperty animationName = elem.FindPropertyRelative("animationName");
				EditorGUILayout.PropertyField(animationName);
			break;
			case HotspotInteraction.PLAY_SOUND:
				SerializedProperty targetSource = elem.FindPropertyRelative("targetSource");
				EditorGUILayout.PropertyField(targetSource);
			break;
		}

		return containsScene;
	}
}
