using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class OnSelectSwitch : MonoBehaviour {
    public bool on { get; private set; }
    public string ItemUrl;
    void OnSelect()
      {
        System.Diagnostics.Debug.WriteLine("Im selected");
        var command = on ? "OFF" : "ON";
        var data = (Encoding.UTF8.GetBytes(command));
        WWWForm form = new WWWForm(); // Just fot getting the headers;
        Dictionary<string, string> headers = form.headers;
        headers["Content-Type"] = "text/plain";
        var www = new WWW(ItemUrl, data, headers);
        on = !on;
        BroadcastMessage("SetText", on ? "ON" : "OFF");
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

    public void OnUpdate(string text)
    {
        if (text.Equals("UNDEFINED") || text.Equals("OFF") || text.Equals("0"))
        {
            on = false;
            text = "OFF";
        } else
        {
            on = true;
        }
        BroadcastMessage("SetText", text);
    }
}
