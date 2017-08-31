using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class BasicItemHandler : MonoBehaviour {

    public string ItemUrl;

	// Use this for initialization
	public void OnCommand(string command)
    {
        var data = (Encoding.UTF8.GetBytes(command));
        WWWForm form = new WWWForm(); // Just fot getting the headers;
        Dictionary<string, string> headers = form.headers;
        headers["Content-Type"] = "text/plain";
        var www = new WWW(ItemUrl, data, headers);
        StartCoroutine(WaitForRequest(www));
    }

    IEnumerator WaitForRequest(WWW www)
    {
        yield return www;

        // check for errors
        if (www.error == null)
        {
            Debug.Log("WWW Ok!: " + www.text);
        }
        else
        {
            Debug.Log("WWW Error: " + www.error);
        }
    }
    public virtual void OnUpdate(string state)
    {
        BroadcastMessage("SetText", state);
    }
}
