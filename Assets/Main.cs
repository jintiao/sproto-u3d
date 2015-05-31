using UnityEngine;
using System.Collections;
using System.IO;
using System;

public class Main : MonoBehaviour {
	private Client mClient = null;
	private string mText = "";

	void Start () {
		new TestUnit ().Run ();
		new Test ().Run ();
		new TestAll ().Run ();
		new TestRpc ().Run ();
	}

	void OnGUI () {
		if (GUI.Button (new Rect (10, 10, 100, 100), "Benchmark")){
			new Benchmark().Run ();
		}

		if (mClient != null) {

			mText = GUI.TextField (new Rect(120, 150, 200, 40), mText, 25);
			if (GUI.Button (new Rect (330, 150, 80, 40), "Get")){
				mClient.SendGet (mText);
				mText = "";
			}

			GUI.enabled = false;
		}

		if (GUI.Button (new Rect (10, 150, 100, 100), "Client")){
			mClient = new Client();
			mClient.Run ();
		}

		GUI.enabled = true;
	}
}
