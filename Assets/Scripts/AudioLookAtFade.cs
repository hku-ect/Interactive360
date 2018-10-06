using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioLookAtFade : MonoBehaviour {
	public enum FALLOFF_TYPE {
		Linear,
		Exponential
	}

	[Range(0,1)]
	public float minVolume = 0;
	[Range(0,1)]
	public float maxVolume = 1;
	[Range(0,180)]
	public float fallOffDegrees = 180;
	public FALLOFF_TYPE fallOffType = FALLOFF_TYPE.Linear;
	
	AudioSource source;

	Transform sourceTransform;
	Transform cameraTransform;

	// Use this for initialization
	void Start () {
		source = GetComponent<AudioSource>();
		sourceTransform = transform;
	}
	
	// Update is called once per frame
	void Update () {
		if ( Camera.main == null ) return;

		if ( cameraTransform == null ) cameraTransform = Camera.main.transform;
		//calculate angle between view direction of main camera and audio source relative position vector
		Vector3 audioVector = ( sourceTransform.position - cameraTransform.position ).normalized;
		Vector3 lookVector = cameraTransform.forward;
		float dot = Vector3.Dot(audioVector, lookVector);
		float angle = Mathf.Acos(dot);

		//convert to [0,1] based on falloff
		if ( fallOffDegrees == 0 ) angle = 1;
		else angle /= ( fallOffDegrees * Mathf.Deg2Rad );

		//make sure to clamp to [0,1]
		angle = 1 - Mathf.Clamp01(angle);

		if ( fallOffType == FALLOFF_TYPE.Exponential ) angle *= angle;

		//apply to source volume based on mix/max variables
		float volume = Mathf.Lerp(minVolume, maxVolume, angle);
		source.volume = volume;
	}

	void OnDrawGizmos() {
		Gizmos.DrawWireSphere(transform.position, 1f);
	}
}
