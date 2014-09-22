using UnityEngine;
using System.Collections;
using System.IO;
using System;

public class Main : MonoBehaviour {
	void Start () {
		string path = Application.dataPath + "/foobar.sproto";
		
		using (FileStream stream = new FileStream (path, FileMode.Open)) {
			SpTypeManager.Import (stream);
		}
		
		SpObject obj = new SpObject ();
		obj.Insert ("a", "hello");
		obj.Insert ("b", 1000000);
		obj.Insert ("c", true);
		
		SpObject d = new SpObject ();
		d.Insert ("a", "world");
		d.Insert ("c", -1);
		obj.Insert ("d", d);
		
		SpObject e = new SpObject ();
		e.Append("ABC");
		e.Append("def");
		obj.Insert ("e", e);
		
		SpObject f = new SpObject ();
		f.Append(-3);
		f.Append(-2);
		f.Append(-1);
		f.Append(0);
		f.Append(1);
		f.Append(2);
		obj.Insert ("f", f);
		
		SpObject g = new SpObject ();
		g.Append (true);
		g.Append (false);
		g.Append (true);
		obj.Insert ("g", g);
		
		SpObject h = new SpObject ();
		{
			SpObject t = new SpObject ();
			t.Insert ("b", 100);
			h.Append (t);
		}
		{
			SpObject t = new SpObject ();
			h.Append (t);
		}
		{
			SpObject t = new SpObject ();
			t.Insert ("b", -100);
			t.Insert ("c", false);
			h.Append (t);
		}
		{
			SpObject t = new SpObject ();
			t.Insert ("b", 0);
			
			SpObject he = new SpObject ();
			he.Append ("test");
			t.Insert ("e", he);
			h.Append (t);
		}
		obj.Insert ("h", h);

		//Debug.Log (obj.Dump ());
		
		using (MemoryStream stream = new MemoryStream ()) {
			long pos = stream.Position;
			SpCodec.Encode ("foobar", obj, stream);

			stream.Position = pos;
			SpObject newObj = SpCodec.Decode ("foobar", stream);
			if (newObj == null ) {
				Debug.LogWarning ("Decode failed!");
				return;
			}

			Debug.Log (newObj.Dump ());
			if (newObj.Match (obj) == false) {
				Debug.LogWarning ("Encode/Decode not match!");
				return;
			}
		}
	}
}
