using System.Collections;
using System.IO;
using System;

public class Benchmark {
	private SpObject obj;
	SpStream encode_stream = new SpStream (1024);
	SpStream pack_stream = new SpStream (1024);
	SpStream unpack_stream = new SpStream (1024);
	const int BENCHUMARK_RUN_TIMES = 100000;
    SpTypeManager manager;
	
	public Benchmark () {
        manager = LoadProto ();
		
		obj = CreateObject ();
		manager.Codec.Encode ("AddressBook", obj, encode_stream);
		encode_stream.Position = 0;
		SpPacker.Pack (encode_stream, pack_stream);
		pack_stream.Position = 0;
		SpPacker.Unpack (pack_stream, unpack_stream);
	}

	public void Run () {
		double encode = Encode ();
		double pack = Pack ();
		double encodeAndPack = EncodeAndPack ();
		double unpack = Unpack ();
		double decode = Decode ();
		double unpackAndDecode = UnpackAndDecode ();

		Util.Log ("Encode:\t" + encode +" s\n" + 
		          "Pack:\t" + pack +" s\n" + 
		          "EncodeAndPack:\t" + encodeAndPack +" s\n" + 
		          "Unpack:\t" + unpack +" s\n" + 
		          "Decode:\t" + decode +" s\n" + 
		          "UnpackAndDecode:\t" + unpackAndDecode +" s\n"
		          );
	}

	public double Encode () {
		double begin = GetMs ();
		
		for (int i = 0; i < BENCHUMARK_RUN_TIMES; i++) {
			encode_stream.Position = 0;
			pack_stream.Position = 0;
			unpack_stream.Position = 0;
			
			manager.Codec.Encode ("AddressBook", obj, encode_stream);
			//SpPacker.Pack (encode_stream, pack_stream);
			//SpPacker.Unpack (pack_stream, unpack_stream);
			unpack_stream.Position = 0;
			//manager.Codec.Decode ("AddressBook", unpack_stream);
		}
		
		double end = GetMs ();
		return (end - begin)/1000;
	}
	
	public double Pack () {
		double begin = GetMs ();
		
		for (int i = 0; i < BENCHUMARK_RUN_TIMES; i++) {
			encode_stream.Position = 0;
			pack_stream.Position = 0;
			unpack_stream.Position = 0;
			
			//manager.Codec.Encode ("AddressBook", obj, encode_stream);
			SpPacker.Pack (encode_stream, pack_stream);
			//SpPacker.Unpack (pack_stream, unpack_stream);
			unpack_stream.Position = 0;
			//manager.Codec.Decode ("AddressBook", unpack_stream);
		}
		
		double end = GetMs ();
		return (end - begin)/1000;
	}
	
	public double EncodeAndPack () {
		double begin = GetMs ();
		
		for (int i = 0; i < BENCHUMARK_RUN_TIMES; i++) {
			encode_stream.Position = 0;
			pack_stream.Position = 0;
			unpack_stream.Position = 0;
			
			manager.Codec.Encode ("AddressBook", obj, encode_stream);
			SpPacker.Pack (encode_stream, pack_stream);
			//SpPacker.Unpack (pack_stream, unpack_stream);
			unpack_stream.Position = 0;
			//manager.Codec.Decode ("AddressBook", unpack_stream);
		}
		
		double end = GetMs ();
		return (end - begin)/1000;
	}
	
	public double Unpack () {
		double begin = GetMs ();
		
		for (int i = 0; i < BENCHUMARK_RUN_TIMES; i++) {
			encode_stream.Position = 0;
			pack_stream.Position = 0;
			unpack_stream.Position = 0;
			
			//manager.Codec.Encode ("AddressBook", obj, encode_stream);
			//SpPacker.Pack (encode_stream, pack_stream);
			SpPacker.Unpack (pack_stream, unpack_stream);
			unpack_stream.Position = 0;
			//manager.Codec.Decode ("AddressBook", unpack_stream);
		}
		
		double end = GetMs ();
		return (end - begin)/1000;
	}
	
	public double Decode () {
		double begin = GetMs ();
		
		for (int i = 0; i < BENCHUMARK_RUN_TIMES; i++) {
			encode_stream.Position = 0;
			pack_stream.Position = 0;
			unpack_stream.Position = 0;
			
			//manager.Codec.Encode ("AddressBook", obj, encode_stream);
			//SpPacker.Pack (encode_stream, pack_stream);
			//SpPacker.Unpack (pack_stream, unpack_stream);
			unpack_stream.Position = 0;
			manager.Codec.Decode ("AddressBook", unpack_stream);
		}
		
		double end = GetMs ();
		return (end - begin)/1000;
	}
	
	public double UnpackAndDecode () {
		double begin = GetMs ();
		
		for (int i = 0; i < BENCHUMARK_RUN_TIMES; i++) {
			encode_stream.Position = 0;
			pack_stream.Position = 0;
			unpack_stream.Position = 0;
			
			//manager.Codec.Encode ("AddressBook", obj, encode_stream);
			//SpPacker.Pack (encode_stream, pack_stream);
			SpPacker.Unpack (pack_stream, unpack_stream);
			unpack_stream.Position = 0;
			manager.Codec.Decode ("AddressBook", unpack_stream);
		}
		
		double end = GetMs ();
		return (end - begin)/1000;
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

    private SpTypeManager LoadProto () {
        SpTypeManager tm = null;
        string path = Util.GetFullPath ("AddressBook.sproto");
        using (FileStream stream = new FileStream (path, FileMode.Open)) {
            tm = SpTypeManager.Import (stream);
        }
        return tm;
    }

	double GetMs() {
		TimeSpan ts = DateTime.Now - new DateTime(1960, 1, 1);
		return ts.TotalMilliseconds;
	}
}
