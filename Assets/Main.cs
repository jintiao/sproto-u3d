using UnityEngine;
using System.Collections;
using System.IO;
using System;

public class Main : MonoBehaviour {
	void Start () {
		TestAll testall = new TestAll ();
		testall.Run ();

		Test test = new Test ();
		test.Run ();

		TestRpc testrpc = new TestRpc ();
		testrpc.Run ();
	}
}
