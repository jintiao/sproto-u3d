using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;

public class TestAll {
    public void Run () {
        LoadProto ();

        SpObject obj = CreateObject2 ();
        Debug.Log (Util.DumpObject (obj));

        MemoryStream encode_stream = new MemoryStream ();
        MemoryStream decode_stream = new MemoryStream ();
        MemoryStream pack_stream = new MemoryStream ();

        SpCodec.Encode ("foobar", obj, encode_stream);

        encode_stream.Position = 0;
        Debug.Log (Util.DumpStream (encode_stream));

        encode_stream.Position = 0;
        SpPacker.Pack (encode_stream, pack_stream);

        pack_stream.Position = 0;
        Debug.Log (Util.DumpStream (pack_stream));

        pack_stream.Position = 0;
        SpPacker.Unpack (pack_stream, decode_stream);

        decode_stream.Position = 0;
        Debug.Log (Util.DumpStream (decode_stream));

        decode_stream.Position = 0;
        SpObject newObj = SpCodec.Decode ("foobar", decode_stream);
        Debug.Log (Util.DumpObject (newObj));
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
        e.Append ("ABC");
        e.Append ("def");
        obj.Insert ("e", e);

        SpObject f = new SpObject ();
        f.Append (-3);
        f.Append (-2);
        f.Append (-1);
        f.Append (0);
        f.Append (1);
        f.Append (2);
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

    private SpObject CreateObject2 () {
        SpObject obj = new SpObject ();

        obj.Insert ("a", "hello");
        obj.Insert ("b", 1000000);
        obj.Insert ("c", true);

        SpObject d = new SpObject ();
        d.Insert ("a", "world");
        d.Insert ("c", -1);
        obj.Insert ("d", d);

        obj.Insert ("e", new SpObject ("ABC", "def"));
        obj.Insert ("f", new SpObject (-3, -2, -1, 0, 1, 2));
        obj.Insert ("g", new SpObject (true, false, true));

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

    private void LoadProto () {
        string path = Application.dataPath +  "/foobar.sproto";
        using (FileStream stream = new FileStream (path, FileMode.Open)) {
            SpTypeManager.Import (stream);
        }
    }
}

