using System.Collections;
using System.IO;
using System;

public class Benchmark {
	private SpObject obj;
	MemoryStream encode_stream = new MemoryStream ();
	MemoryStream pack_stream = new MemoryStream ();
	MemoryStream unpack_stream = new MemoryStream ();
	
	public Benchmark () {
		LoadProto ();
		
		obj = CreateObject ();
		SpCodec.Encode ("AddressBook", obj, encode_stream);
		encode_stream.Position = 0;
		SpPacker.Pack (encode_stream, pack_stream);
		pack_stream.Position = 0;
		SpPacker.Unpack (pack_stream, unpack_stream);
	}

	public void Run () {
		Encode ();
		Pack ();
		EncodeAndPack ();
		Unpack ();
		Decode ();
		UnpackAndDecode ();
	}

	public void Encode () {
		double begin = GetMs ();
		
		for (int i = 0; i < 1000000; i++) {
			encode_stream.Position = 0;
			pack_stream.Position = 0;
			unpack_stream.Position = 0;
			
			SpCodec.Encode ("AddressBook", obj, encode_stream);
			//SpPacker.Pack (encode_stream, pack_stream);
			//SpPacker.Unpack (pack_stream, unpack_stream);
			unpack_stream.Position = 0;
			//SpCodec.Decode ("AddressBook", unpack_stream);
		}
		
		double end = GetMs ();
		Util.Log ("Encode: " + (end - begin)/1000  +" s");
	}
	
	public void Pack () {
		double begin = GetMs ();
		
		for (int i = 0; i < 1000000; i++) {
			encode_stream.Position = 0;
			pack_stream.Position = 0;
			unpack_stream.Position = 0;
			
			//SpCodec.Encode ("AddressBook", obj, encode_stream);
			SpPacker.Pack (encode_stream, pack_stream);
			//SpPacker.Unpack (pack_stream, unpack_stream);
			unpack_stream.Position = 0;
			//SpCodec.Decode ("AddressBook", unpack_stream);
		}
		
		double end = GetMs ();
		Util.Log ("Pack: " + (end - begin)/1000  +" s");
	}
	
	public void EncodeAndPack () {
		double begin = GetMs ();
		
		for (int i = 0; i < 1000000; i++) {
			encode_stream.Position = 0;
			pack_stream.Position = 0;
			unpack_stream.Position = 0;
			
			SpCodec.Encode ("AddressBook", obj, encode_stream);
			SpPacker.Pack (encode_stream, pack_stream);
			//SpPacker.Unpack (pack_stream, unpack_stream);
			unpack_stream.Position = 0;
			//SpCodec.Decode ("AddressBook", unpack_stream);
		}
		
		double end = GetMs ();
		Util.Log ("EncodeAndPack: " + (end - begin)/1000  +" s");
	}
	
	public void Unpack () {
		double begin = GetMs ();
		
		for (int i = 0; i < 1000000; i++) {
			encode_stream.Position = 0;
			pack_stream.Position = 0;
			unpack_stream.Position = 0;
			
			//SpCodec.Encode ("AddressBook", obj, encode_stream);
			//SpPacker.Pack (encode_stream, pack_stream);
			SpPacker.Unpack (pack_stream, unpack_stream);
			unpack_stream.Position = 0;
			//SpCodec.Decode ("AddressBook", unpack_stream);
		}
		
		double end = GetMs ();
		Util.Log ("Unpack: " + (end - begin)/1000  +" s");
	}
	
	public void Decode () {
		double begin = GetMs ();
		
		for (int i = 0; i < 1000000; i++) {
			encode_stream.Position = 0;
			pack_stream.Position = 0;
			unpack_stream.Position = 0;
			
			//SpCodec.Encode ("AddressBook", obj, encode_stream);
			//SpPacker.Pack (encode_stream, pack_stream);
			//SpPacker.Unpack (pack_stream, unpack_stream);
			unpack_stream.Position = 0;
			SpCodec.Decode ("AddressBook", unpack_stream);
		}
		
		double end = GetMs ();
		Util.Log ("Decode: " + (end - begin)/1000  +" s");
	}
	
	public void UnpackAndDecode () {
		double begin = GetMs ();
		
		for (int i = 0; i < 1000000; i++) {
			encode_stream.Position = 0;
			pack_stream.Position = 0;
			unpack_stream.Position = 0;
			
			//SpCodec.Encode ("AddressBook", obj, encode_stream);
			//SpPacker.Pack (encode_stream, pack_stream);
			SpPacker.Unpack (pack_stream, unpack_stream);
			unpack_stream.Position = 0;
			SpCodec.Decode ("AddressBook", unpack_stream);
		}
		
		double end = GetMs ();
		Util.Log ("UnpackAndDecode: " + (end - begin)/1000  +" s");
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
        string path = Util.GetFullPath ("AddressBook.sproto");
		
		using (FileStream stream = new FileStream (path, FileMode.Open)) {
			SpTypeManager.Import (stream);
		}
	}

	double GetMs() {
		TimeSpan ts = DateTime.Now - new DateTime(1960, 1, 1);
		return ts.TotalMilliseconds;
	}
}
