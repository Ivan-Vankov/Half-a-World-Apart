using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Mathf;

public class FpsCounter : MonoBehaviour {
	
	private readonly float waitTime = 0.2f;
	private string oldResult;
	private float countDown = 0;

	private void OnGUI() {
		countDown -= Time.deltaTime;

		if (countDown < 0) {
			countDown = waitTime;
			oldResult = Floor(1 / Time.deltaTime).ToString();
		}

		GUI.Label(new Rect(120, 20, 40, 40), oldResult);
	}
}
