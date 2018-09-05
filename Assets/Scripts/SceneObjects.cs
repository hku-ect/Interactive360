using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneObjects : MonoBehaviour {

    public bool stickToCamera = true;
    Transform _t;

    void Awake()
    {
        _t = transform;
    }

	public void Hide() {
		foreach( Transform t in transform ) {
			t.gameObject.SetActive(false);
            AudioSource asrc = t.gameObject.GetComponent<AudioSource>();
            if (asrc) asrc.Stop();
        }
	}

	public void Show() {
		foreach( Transform t in transform ) {
			t.gameObject.SetActive(true);
            AudioSource asrc = t.gameObject.GetComponent<AudioSource>();
            if (asrc) asrc.Play();
		}
	}

    void Update()
    {
        //TODO: optimize
        if (stickToCamera) _t.position = Camera.main.transform.position;
    }
}
