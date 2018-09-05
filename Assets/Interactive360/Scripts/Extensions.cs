using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Extensions {
	public static bool FindInScene<T>( this Scene sc, out T vp ) where T : Component {
        vp = null;
        GameObject[] objects = sc.GetRootGameObjects();
        foreach( GameObject g in objects ) {
            vp = g.GetComponent<T>();
            if ( vp ) return true;
        }
        return false;
    }

    public static bool IsInScene( this Scene sc, Transform objTransform ) {
        Transform root = objTransform.root;
        foreach( GameObject g in sc.GetRootGameObjects() ) {
            if ( g.transform == root ) return true;
        }
        return false;
    }
}
