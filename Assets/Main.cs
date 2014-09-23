using UnityEngine;
using System.Collections;
using System.IO;
using System;

public class Main : MonoBehaviour {
	void Start () {
		TestAll test = new TestAll ();
		test.Run ();
	}
}
