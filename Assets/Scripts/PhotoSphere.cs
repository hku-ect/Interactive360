using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotoSphere : MonoBehaviour {
	MeshRenderer rend;
    
    Transform _t;

    // Use this for initialization
    void Awake () {
		rend = GetComponent<MeshRenderer>();
		Hide();
        _t = transform;

    }
	
	public void Show() {
		rend.enabled = true;
	}

	public void Hide() {
		rend.enabled = false;
	}

    void Update()
    {
        //TODO: optimize
        _t.position = Camera.main.transform.position;
    }
}
