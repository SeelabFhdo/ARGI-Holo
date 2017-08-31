using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if !UNITY_EDITOR
using Newtonsoft.Json;
#endif
#if UNITY_EDITOR
using Newtonsoft.Json;
#endif


public class PersistenceManager : MonoBehaviour {

    // Use this for initialization

    public string ItemURL;

    private Dictionary<string, GameObject> itemObjects;

    public GameObject switchPrefab;
    public GameObject rollershutterPrefab;
    public GameObject ColorPrefab;
    void Start () {
        itemObjects = new Dictionary<string, GameObject>();

        WWWForm form = new WWWForm(); // Just fot getting the headers;
        Dictionary<string, string> headers = form.headers;
        headers["Accept"] = "application/json";
        var www = new WWW(ItemURL);
        StartCoroutine(WaitForRequest(www));

    }

    private void updateItems()
    {
        WWWForm form = new WWWForm(); // Just fot getting the headers;
        Dictionary<string, string> headers = form.headers;
        headers["Accept"] = "application/json";
        var www = new WWW(ItemURL);
        StartCoroutine(WaitForUpdateRequest(www));
    }

    IEnumerator WaitForUpdateRequest(WWW www)
    {
        yield return www;

        // check for errors
        if (string.IsNullOrEmpty(www.error))
        {
            Debug.Log("WWW Ok!: " + www.text);
            handleUpdateResponse(www.text);
        }
        else
        {
            Debug.Log("WWW Error: " + www.error);
        }
    }

    private void handleUpdateResponse(string json)
    {
        Item[] items = JsonConvert.DeserializeObject<Item[]>(json);
        foreach (Item i in items)
        {
            if(itemObjects.ContainsKey(i.name))
            {
                itemObjects[i.name].BroadcastMessage("OnUpdate", i.state);
            }
        }
    }

    private void handleInitResponse(string json)
    {

        Item[] items = JsonConvert.DeserializeObject<Item[]>(json);
        Vector3 initialPos = new Vector3(0, 0.5f, 2);
        foreach(Item i in items)
        {
            foreach(string tag in i.tags)
            {
                if(tag.Equals("holo"))
                {
                    //TODO: Polymorphie beutzen ansonsten unnötig, doppelter code 
                    Debug.Log("add item");
                    Debug.Log(i.name);
                    if (i.type.Equals("Switch") || i.type.Equals("Dimmer"))
                    {
                        GameObject newSwitch = Instantiate(switchPrefab, transform.position + initialPos, Quaternion.identity);
                        PositioningManager pm = newSwitch.GetComponent<PositioningManager>();
                        OnSelectSwitch oss = newSwitch.GetComponent<OnSelectSwitch>();
                        oss.ItemUrl = i.link;
                        pm.ObjectAnchorStoreName = i.name;
                        itemObjects.Add(i.name, newSwitch);
                    } else if(i.type.Equals("Rollershutter"))
                    {
                        GameObject newRollerShutter = Instantiate(rollershutterPrefab, transform.position + initialPos, Quaternion.identity);
                        PositioningManager pm = newRollerShutter.GetComponent<PositioningManager>();
                        BasicItemHandler bih = newRollerShutter.GetComponent<BasicItemHandler>();
                        bih.ItemUrl = i.link;
                        pm.ObjectAnchorStoreName = i.name;
                        itemObjects.Add(i.name, newRollerShutter);
                    }
                    else if (i.type.Equals("Color"))
                    {
                        GameObject newColor = Instantiate(ColorPrefab, transform.position + initialPos, Quaternion.identity);
                        PositioningManager pm = newColor.GetComponent<PositioningManager>();
                        BasicItemHandler bih = newColor.GetComponent<BasicItemHandler>();
                        bih.ItemUrl = i.link;
                        pm.ObjectAnchorStoreName = i.name;
                        itemObjects.Add(i.name, newColor);
                    }
                    initialPos.x += 0.3f;
                    break;
                }
            }
            
        }
        updateItems();
    }


    IEnumerator WaitForRequest(WWW www)
    {
        yield return www;

        // check for errors
        if (string.IsNullOrEmpty(www.error))
        {
            Debug.Log("WWW Ok!: " + www.text);
            handleInitResponse(www.text);
        }
        else
        {
            Debug.Log("WWW Error: " + www.error);
        }
    }

    // Update is called once per frame
    void Update () {
		
	}

}
