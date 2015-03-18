using System.Collections;
using System.IO;

public class Test {
	public void Run () {
        SpTypeManager manager = LoadProto ();
		
		SpObject obj = CreateObject ();
        CheckObj (obj);

		Util.Log ("Encode");
		SpStream encode_stream = new SpStream ();
		manager.Codec.Encode ("AddressBook", obj, encode_stream);
		
		encode_stream.Position = 0;
        Util.DumpStream (encode_stream);
		
		Util.Log ("Decode");
		encode_stream.Position = 0;
		SpObject newObj = manager.Codec.Decode ("AddressBook", encode_stream);
		CheckObj (newObj);
		
		Util.Log ("Pack");
		encode_stream.Position = 0;
		SpStream pack_stream = new SpStream ();
		SpPacker.Pack (encode_stream, pack_stream);
		
		pack_stream.Position = 0;
        Util.DumpStream (pack_stream);

		Util.Log ("Unpack");
		pack_stream.Position = 0;
		SpStream unpack_stream = new SpStream ();
		SpPacker.Unpack (pack_stream, unpack_stream);
		
		unpack_stream.Position = 0;
        Util.DumpStream (unpack_stream);

		Util.Log ("Decode");
		unpack_stream.Position = 0;
        newObj = manager.Codec.Decode ("AddressBook", unpack_stream);
        CheckObj (newObj);
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

			person.Insert (p["id"].AsInt (), p);
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
			
			person.Insert (p["id"].AsInt (), p);
		}

        obj.Insert ("person", person);
		
		return obj;
	}

    private SpTypeManager LoadProto () {
        SpTypeManager tm = null;
        string path = Util.GetFullPath ("AddressBook.sproto");
        using (FileStream stream = new FileStream (path, FileMode.Open)) {
            tm = SpTypeManager.Import (stream);
        }
        return tm;
    }

	private void CheckObj (SpObject obj) {
		Util.DumpObject (obj);
        Util.Assert (obj["person"][10000]["id"].AsInt () == 10000);
        Util.Assert (obj["person"][10000]["name"].AsString ().Equals ("Alice"));
        Util.Assert (obj["person"][10000]["phone"][0]["type"].AsInt () == 1);
        Util.Assert (obj["person"][10000]["phone"][0]["number"].AsString ().Equals ("123456789"));
        Util.Assert (obj["person"][10000]["phone"][1]["type"].AsInt () == 2);
        Util.Assert (obj["person"][10000]["phone"][1]["number"].AsString ().Equals ("87654321"));
        Util.Assert (obj["person"][20000]["id"].AsInt () == 20000);
        Util.Assert (obj["person"][20000]["name"].AsString ().Equals ("Bob"));
        Util.Assert (obj["person"][20000]["phone"][0]["type"].AsInt () == 3);
        Util.Assert (obj["person"][20000]["phone"][0]["number"].AsString ().Equals ("01234567890"));
    }
}
