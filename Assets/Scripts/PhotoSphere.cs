using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhotoSphere : MonoBehaviour {
	
	[ContextMenu("GetWebcames")]
	void GetWebcams() {
		webCamNames = new string[WebCamTexture.devices.Length];
		int index = 0;
		foreach( WebCamDevice d in WebCamTexture.devices ) {
			webCamNames[index++] = d.name;
		}
	}

	public string[] webCamNames = {};
	public bool useWebCam = false;
	public string webCamName = "";
	
	MeshRenderer rend;
	WebCamTexture camTex;
	Transform _t;

	// Use this for initialization
	void Awake () {
		rend = GetComponent<MeshRenderer>();
		Hide();
		_t = transform;
	}
	
	public void Show() {
		if ( useWebCam && camTex == null ) {
			camTex = new WebCamTexture(webCamName);
			rend.material.shader = Shader.Find("UI/Default");
		}

		rend.material.mainTexture = camTex;
		camTex.Play();
		
		rend.enabled = true;
	}

	public void Hide() {
		rend.enabled = false;

		if ( camTex != null ) {
			camTex.Stop();
		}
	}

	void Update()
    {
        //TODO: optimize
        _t.position = Camera.main.transform.position;
    }
}
