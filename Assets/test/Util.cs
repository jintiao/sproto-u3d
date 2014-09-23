using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;

public class Util {
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
