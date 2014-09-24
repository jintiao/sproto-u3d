using System.Collections;
using System.IO;
using UnityEngine;
using System;

public class Benchmark {
	public void Run () {
		LoadProto ();
		
		SpObject obj = CreateObject ();
		
		MemoryStream encode_stream = new MemoryStream (1024);
		MemoryStream pack_stream = new MemoryStream (1024);
		MemoryStream unpack_stream = new MemoryStream (1024);

		
		SpCodec.Encode ("AddressBook", obj, encode_stream);
		encode_stream.Position = 0;
		SpPacker.Pack (encode_stream, pack_stream);
		pack_stream.Position = 0;
		SpPacker.Unpack (pack_stream, unpack_stream);

		double begin = GetMs ();

		for (int i = 0; i < 100; i++) {
			encode_stream.Position = 0;
			pack_stream.Position = 0;
			unpack_stream.Position = 0;

			SpCodec.Encode ("AddressBook", obj, encode_stream);
			//SpPacker.Pack (encode_stream, pack_stream);
			//SpPacker.Unpack (pack_stream, unpack_stream);
			//SpCodec.Decode ("AddressBook", unpack_stream)
		}

		double end = GetMs ();
		
		encode_stream.Position = 0;
		Debug.Log ("total: " + (end - begin)/1000  +"s");
	}
	
	private SpObject CreateObject () {
		SpObject obj = new SpObject ();

		SpObject person = new SpObject ();

		{
			SpObject p = new SpObject ();
			p.Insert ("name", "Alice");
			p.Insert ("id", 10000);

			SpObject phone = new SpObject ();
			{
				SpObject p1 = new SpObject ();
				p1.Insert ("number", "123456789");
				p1.Insert ("type", 1);
				phone.Append (p1);
			}
			{
				SpObject p1 = new SpObject ();
				p1.Insert ("number", "87654321");
				p1.Insert ("type", 2);
				phone.Append (p1);
			}
			p.Insert ("phone", phone);

			person.Append (p);
		}
		{
			SpObject p = new SpObject ();
			p.Insert ("name", "Bob");
			p.Insert ("id", 20000);
			
			SpObject phone = new SpObject ();
			{
				SpObject p1 = new SpObject ();
				p1.Insert ("number", "01234567890");
				p1.Insert ("type", 3);
				phone.Append (p1);
			}
			p.Insert ("phone", phone);
			
			person.Append (p);
		}

        obj.Insert ("person", person);
		
		return obj;
	}
	
	public void LoadProto () {
		string path = Application.dataPath +  "/AddressBook.sproto";
		
		using (FileStream stream = new FileStream (path, FileMode.Open)) {
			SpTypeManager.Import (stream);
		}
	}

	double GetMs() {
		TimeSpan ts = DateTime.Now - new DateTime(1960, 1, 1);
		return ts.TotalMilliseconds;
	}
}
