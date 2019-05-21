using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour {

	public static AudioManager instance = null;
	
	public AudioSource throwSound;
	public AudioSource explosionSound;
	public AudioSource stepsSound;

    void Start() {
        if (instance == null) {
			instance = this;
		}
    }

	public void PlayThrowSound() {
		throwSound.Play();
	}

    public void PlayExplosionSound() {
		explosionSound.Play();
	}

	public void PlayStepsSound() {
		stepsSound.loop = true;
		stepsSound.Play();
	}

	public void StopStepsSound() {
		stepsSound.loop = false;
	}
}
