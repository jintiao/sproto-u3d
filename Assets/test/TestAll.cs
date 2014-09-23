using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;

public class TestAll {
	public void Run () {
		LoadProto ();

		SpObject obj = CreateObject ();
		Debug.Log (DumpObject (obj));

		MemoryStream encode_stream = new MemoryStream ();
		SpCodec.Encode ("foobar", obj, encode_stream);

		encode_stream.Position = 0;
		Debug.Log (DumpStream (encode_stream));
		
		encode_stream.Position = 0;
		MemoryStream pack_stream = new MemoryStream ();
		SpPacker.Pack (encode_stream, pack_stream);
		
		pack_stream.Position = 0;
		Debug.Log (DumpStream (pack_stream));

		pack_stream.Position = 0;
		MemoryStream unpack_stream = new MemoryStream ();
		SpPacker.Unpack (pack_stream, unpack_stream);
		
		unpack_stream.Position = 0;
		Debug.Log (DumpStream (unpack_stream));
		
		unpack_stream.Position = 0;
		SpObject newObj = SpCodec.Decode ("foobar", unpack_stream);
		Debug.Log (DumpObject (newObj));
	}

	private SpObject CreateObject () {
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

		return obj;
	}

	public void LoadProto () {
		string path = Application.dataPath + "/foobar.sproto";
		
		using (FileStream stream = new FileStream (path, FileMode.Open)) {
			SpTypeManager.Import (stream);
		}
	}

	public static string DumpStream (Stream stream) {
		string str = "";

		byte[] buf = new byte[16];
		int count;
		
		while ((count = stream.Read (buf, 0, buf.Length)) > 0) {
			str += DumpLine (buf, count);
		}

		return str;
	}
	
	private static string DumpLine (byte[] buf, int count) {
		string str = "";
	
		for (int i = 0; i < count; i++) {
			str += ((i < count) ? String.Format ("{0:X2}", buf[i]) : "  ");
			str += ((i > 0) && (i < count - 1) && ((i + 1) % 8 == 0) ? " - " : " ");
		}
		str += "\n";
		
		return str;
	}

	public static string DumpObject (SpObject obj) {
		return DumpObject (obj, 0);
	}
	
	private static string DumpObject (SpObject obj, int tab) {
		string str = "";
		
		if (obj.IsTable ()) {
			str = GetTab (tab) + "<table>\n";
			foreach (KeyValuePair<string, SpObject> entry in obj.ToTable ()) {
				str += GetTab (tab + 1) + "<key : " + entry.Key + ">\n";
				str += DumpObject (entry.Value, tab + 1);
			}
		}
		else if (obj.IsArray ()) {
			str = GetTab (tab) + "<array>\n";
			foreach (SpObject o in obj.ToArray ()) {
				str += DumpObject (o, tab + 1);
			}
		}
		else if (obj.IsBuildinType ()) {
			str = GetTab (tab) + obj.Value.ToString () + "\n";
		}
		
		return str;
	}
	
	private static string GetTab(int n) {
		string str = "";
		for (int i = 0; i < n; i++)
			str += "\t";
		return str;
	}
}
