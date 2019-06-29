using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadGame : MonoBehaviour {
    void Update() {
        if (Input.anyKey) {
			UnityEngine.SceneManagement.SceneManager.LoadScene("MainScene");
		}
    }
}
