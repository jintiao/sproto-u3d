using System.Collections;
using System.IO;

public class TestRpc {
    private const string server_proto = @"
        .package {
	        type 0 : integer
	        session 1 : integer
        }

        foobar 1 {
	        request {
		        what 0 : string
	        }
	        response {
		        ok 0 : boolean
	        }
        }

        foo 2 {
	        response {
		        ok 0 : boolean
	        }
        }

        bar 3 {}

        blackhole 4 {
	        request {}
        }
        ";

    const string client_proto = @"
        .package {
	        type 0 : integer
	        session 1 : integer
        }
        ";

    private SpRpc server;
    private SpRpc client;

	public void Run () {
        SpTypeManager server_tm = SpTypeManager.Import (server_proto);

        server = SpRpc.Create (server_tm, "package");

        client = SpRpc.Create (client_proto, "package");
        client.Attach (server_tm);

        TestFoobar ();
        TestFoo ();
        TestBar ();
        TestBlackhole ();
	}

    private void TestFoobar () {
        Util.Log ("client request foobar");

        SpObject foobar_request = new SpObject ();
        foobar_request.Insert ("what", "foo");
        SpStream req = client.Request ("foobar", foobar_request, 1);

        Util.Assert (req.Length == 11);
        Util.Log ("request foobar size = " + req.Length);

        req.Position = 0;
		SpRpcResult dispatch_result = server.Dispatch (req);
		Util.Assert (dispatch_result.Arg["what"].AsString ().Equals ("foo"));
		Util.DumpObject (dispatch_result.Arg);

        Util.Log ("server response");

        SpObject foobar_response = new SpObject ();
        foobar_response.Insert ("ok", true);
		SpStream resp = server.Response (dispatch_result.Session, foobar_response);

        Util.Assert (resp.Length == 7);
        Util.Log ("response package size = " + resp.Length);

        Util.Log ("client dispatch");

        resp.Position = 0;
		dispatch_result = client.Dispatch (resp);
        Util.Assert (dispatch_result.Arg["ok"].AsBoolean () == true);
        Util.DumpObject (dispatch_result.Arg);
    }

    private void TestFoo () {
        SpStream req = client.Request ("foo", null, 2);

        Util.Assert (req.Length == 4);
		Util.Log ("request foo size = " + req.Length);

        req.Position = 0;
        SpRpcResult dispatch_result = server.Dispatch (req);

        SpObject foobar_response = new SpObject ();
        foobar_response.Insert ("ok", false);
		SpStream resp = server.Response (dispatch_result.Session, foobar_response);

        Util.Assert (resp.Length == 7);
		Util.Log ("response package size = " + resp.Length);

		Util.Log ("client dispatch");

        resp.Position = 0;
        dispatch_result = client.Dispatch (resp);
        Util.Assert (dispatch_result.Arg["ok"].AsBoolean () == false);
        Util.DumpObject (dispatch_result.Arg);
    }

    private void TestBar () {
        SpStream req = client.Request ("bar");

        Util.Assert (req.Length == 3);
		Util.Log ("request bar size = " + req.Length);

        req.Position = 0;
        server.Dispatch (req);
    }

    private void TestBlackhole () {
        SpStream req = client.Request ("blackhole");
        Util.Assert (req.Length == 3);
		Util.Log ("request blackhole size = " + req.Length);
    }
}
