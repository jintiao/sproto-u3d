using System.Collections;
using System.IO;

public class TestUnit {

	SpTypeManager manager;

	private void TestStr (string s) {
		SpObject obj = new SpObject (SpObject.ArgType.Table, "a", s);
		
		Util.Log ("------------------TestStr----------------------------");
		Util.Log (s);
		
		Util.Log ("Encode");
		SpStream encode_stream = new SpStream (2);
		bool ret = manager.Codec.Encode ("ss", obj, encode_stream);
		Util.Assert (ret == false);
		encode_stream = new SpStream (encode_stream.Position);
		ret = manager.Codec.Encode ("ss", obj, encode_stream);
		Util.Assert (ret == true);
		encode_stream.Position = 0;
		Util.DumpStream (encode_stream);

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
		SpObject dobj = manager.Codec.Decode ("ss", unpack_stream);
		string ds = dobj["a"].AsString ();
		Util.Log (ds);
		Util.Assert (s == ds);
	}

	private void TestNest () {
		SpObject clist = new SpObject (SpObject.ArgType.Table,
				"character",
				new SpObject (SpObject.ArgType.Array, 
						new SpObject (SpObject.ArgType.Table,
								"id", 
		              			1,
								"attribute", 
		              			new SpObject (SpObject.ArgType.Table,
		                 				"level", 
		              					1,
		                 				"templateId", 
		              					1001,
		                 				"ability", 
		              					new SpObject (SpObject.ArgType.Table,
		                 						"hp", 2530,
		                 						"att", 2310
		                 				)
		                 		)
						),
						new SpObject (SpObject.ArgType.Table,
								"id", 
		              			2,
		              			"attribute", 
		              			new SpObject (SpObject.ArgType.Table,
		                 				"level", 
		              					1,
		                 				"templateId", 
		              					1002,
		                 				"ability", 
		              					new SpObject (SpObject.ArgType.Table,
		                 						"hp", 1320,
		                 						"att", 2090
		                 				)
		                 		)
						)
				)
		);
		
		Util.Log ("------------------TEST CHARACTER----------------------------");
		Util.DumpObject (clist);
		
		Util.Log ("Encode");
		SpStream encode_stream = new SpStream ();
		manager.Codec.Encode ("clist", clist, encode_stream);
		encode_stream.Position = 0;
		Util.DumpStream (encode_stream);
		
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
		SpObject dobj = manager.Codec.Decode ("clist", unpack_stream);
		Util.DumpObject (dobj);
	}

	public void Run () {
		
		string proto = @"
			.ss {
				a 0 : string
			}

			.ability {
				hp 0 : integer
				att 1 : integer
			}

			.attribute {
				templateId 0 : integer
				level 1 : integer
				ability 2 : ability
			}

			.character {
				id 0 : integer
				attribute 1 : attribute
			}

			.clist {
				character 0 : *character(id)
			}
			
		";
		manager = SpTypeManager.Import (proto);


		
		TestStr ("12345678");/*
		TestNest ();
		
		TestStr ("");
		TestStr ("123");
		TestStr ("123456");
		TestStr ("12345678");
		TestStr ("12345678123");
		TestStr ("12345678123456");
		TestStr ("1234567812345678");
		TestStr ("12345678123456781234567812345678");
		TestStr ("123456781234567812345678123456781");
		TestStr ("123456781234567812345678123456781234567"); */
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
