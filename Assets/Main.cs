using UnityEngine;
using System.Collections;
using System.IO;
using System;

public class Main : MonoBehaviour {
	void Start () {
		new Test ().Run ();
		new TestAll ().Run ();
		new TestRpc ().Run ();

		//new Benchmark().Run ();
	}
}
