using System.Collections;
using System.IO;
using UnityEngine;

public class Test {
	public void Run () {
		LoadProto ();
		
		SpObject obj = CreateObject ();
        Debug.Log (Util.DumpObject (obj));
		
		MemoryStream encode_stream = new MemoryStream ();
		SpCodec.Encode ("AddressBook", obj, encode_stream);
		
		encode_stream.Position = 0;
        Debug.Log (Util.DumpStream (encode_stream));
		
		encode_stream.Position = 0;
		MemoryStream pack_stream = new MemoryStream ();
		SpPacker.Pack (encode_stream, pack_stream);
		
		pack_stream.Position = 0;
        Debug.Log (Util.DumpStream (pack_stream));
		
		pack_stream.Position = 0;
		MemoryStream unpack_stream = new MemoryStream ();
		SpPacker.Unpack (pack_stream, unpack_stream);
		
		unpack_stream.Position = 0;
        Debug.Log (Util.DumpStream (unpack_stream));
		
		unpack_stream.Position = 0;
		SpObject newObj = SpCodec.Decode ("AddressBook", unpack_stream);
        Debug.Log (Util.DumpObject (newObj));
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
}
