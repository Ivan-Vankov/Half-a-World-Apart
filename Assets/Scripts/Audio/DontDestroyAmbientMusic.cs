using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroyAmbientMusic : MonoBehaviour {

    void Start() {
		DontDestroyOnLoad(gameObject);
    }

}
