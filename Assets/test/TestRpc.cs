using System.Collections;
using System.IO;
using UnityEngine;

public class TestRpc {
	public void Run () {
		LoadProto ();

        SpRpc server = SpRpc.Create ("package");
        SpRpc client = SpRpc.Create ("package");

        TestFoobar (server, client);
        TestFoo (server, client);
        TestBar (server, client);
        TestBlackhole (server, client);
	}

    private void TestFoobar (SpRpc server, SpRpc client) {
        Debug.Log ("client request foobar");

        SpObject foobar_request = new SpObject ();
        foobar_request.Insert ("what", "foo");
        Stream req = client.Request ("foobar", foobar_request, 1);

        Debug.Log ("request foobar size = " + req.Length);

        req.Position = 0;
        SpRpcDispatchInfo dispatched_req = server.Dispatch (req);
        Debug.Log (Util.DumpObject (dispatched_req.Object));

        Debug.Log ("server response");

        SpObject foobar_response = new SpObject ();
        foobar_response.Insert ("ok", true);
        Stream resp = server.Response (dispatched_req.Session, foobar_response);

        Debug.Log ("response package size = " + resp.Length);

        Debug.Log ("client dispatch");

        resp.Position = 0;
        SpRpcDispatchInfo dispatched_resp = client.Dispatch (resp);
        Debug.Log (Util.DumpObject (dispatched_resp.Object));
    }

    private void TestFoo (SpRpc server, SpRpc client) {
        Stream req = client.Request ("foo", null, 2);

		Debug.Log ("request foo size = " + req.Length);

        req.Position = 0;
        SpRpcDispatchInfo dispatched_req = server.Dispatch (req);

        SpObject foobar_response = new SpObject ();
        foobar_response.Insert ("ok", false);
        Stream resp = server.Response (dispatched_req.Session, foobar_response);

		Debug.Log ("response package size = " + resp.Length);

		Debug.Log ("client dispatch");

        resp.Position = 0;
        SpRpcDispatchInfo dispatched_resp = client.Dispatch (resp);
        Debug.Log (Util.DumpObject (dispatched_resp.Object));
    }

    private void TestBar (SpRpc server, SpRpc client) {
        Stream req = client.Request ("bar");

		Debug.Log ("request bar size = " + req.Length);

        req.Position = 0;
        server.Dispatch (req);
    }

    private void TestBlackhole (SpRpc server, SpRpc client) {
        Stream req = client.Request ("blackhole");
		Debug.Log ("request blackhole size = " + req.Length);
    }
	
	private void LoadProto () {
		string path = Application.dataPath +  "/protocol.sproto";
		
		using (FileStream stream = new FileStream (path, FileMode.Open)) {
			SpTypeManager.Import (stream);
		}
	}
}
