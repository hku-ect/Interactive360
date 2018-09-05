using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Video;

[CustomEditor(typeof(VideoPlayer))]
public class DisplayFrameStatistics : Editor {
    
    VideoPlayer vpTarget;
    Rect windowRect = new Rect(2,20,200,100);
    const string FRAME = "Frame: ";
    const string DIV = " / ";
    float frameSlider = 0;

    void OnEnable() {
        vpTarget = target as VideoPlayer;
        vpTarget.seekCompleted += SeekCompleted;
        UpdateSlider();
    }

    void OnDisable() {
         vpTarget.seekCompleted -= SeekCompleted;
    }

    void OnSceneGUI() {
        GUI.Window(0, windowRect, SceneWindow, "Preview Controls");
    }

    void SceneWindow(int id) {
        //TODO: use images
        GUILayout.BeginHorizontal();
        if ( !vpTarget.isPlaying ) {
            if ( GUILayout.Button(EditorGUIUtility.IconContent("PlayButton"))) {
                vpTarget.skipOnDrop = true;
                vpTarget.Play();
            }
        }
        else {
            UpdateSlider();
            if ( GUILayout.Button(EditorGUIUtility.IconContent("PauseButton"))) {
                vpTarget.Pause();
            }
        }
        if ( GUILayout.Button(EditorGUIUtility.IconContent("StepButton"))) {
            vpTarget.StepForward();
            UpdateSlider();
        }
        if ( GUILayout.Button("<<", GUILayout.Height(25))) {
            vpTarget.Stop();
            UpdateSlider();
        }
        GUILayout.EndHorizontal();

        EditorGUI.BeginChangeCheck();
        frameSlider = GUILayout.HorizontalSlider( frameSlider, 0, 1 );
        if ( EditorGUI.EndChangeCheck() ) {
            vpTarget.skipOnDrop = false;
            vpTarget.time = vpTarget.clip.length * frameSlider;
        }

        ulong fCount = vpTarget.frameCount;
        long curFrame = vpTarget.frame;
        GUILayout.Label( FRAME + curFrame + DIV + fCount );
    }

    void SeekCompleted( VideoPlayer source ) {
        vpTarget.Play();
        vpTarget.Pause();
        UpdateSlider();
    }

    void UpdateSlider() {
        if (vpTarget.clip == null) return;
        
        frameSlider = (float) ( vpTarget.time / vpTarget.clip.length );
    }
}
