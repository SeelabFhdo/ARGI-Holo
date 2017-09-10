using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

public class PersistenceManager : MonoBehaviour {

    // Use this for initialization

    public string ItemURL;

    private Dictionary<string, GameObject> itemObjects;

    public GameObject switchPrefab;

    public GameObject rollershutterPrefab;
    public GameObject ColorPrefab;
    public bool fetchFromUrl;
    private string persistfile;
    private Item[] items;
    void Start () {
        persistfile = Application.persistentDataPath + "/items.json";
        itemObjects = new Dictionary<string, GameObject>();
        if (fetchFromUrl)
        {
            WWWForm form = new WWWForm(); // Just fot getting the headers;
            Dictionary<string, string> headers = form.headers;
            headers["Accept"] = "application/json";
            var www = new WWW(ItemURL);
            StartCoroutine(WaitForRequest(www));
        } else
        {
            items = new Item[0];
            load();
            Vector3 initialPos = new Vector3(0, 0.5f, 2);
            initialPos += transform.position;
            foreach (Item i in items) {
                initItem(i, initialPos);
                initialPos.x += 0.3f;
            }
        }


    }

    private void save()
    {
        if (File.Exists(persistfile))
        {
            File.Delete(persistfile);
        }
        File.WriteAllText(persistfile, JsonConvert.SerializeObject(items));
    }
    private void load()
    {
        if (File.Exists(persistfile))
        {
            string json = File.ReadAllText(persistfile);
            items = JsonConvert.DeserializeObject<Item[]>(json);
        } else
        {
            Debug.Log("File not existing");
        }

        //DEBUG
        foreach (Item i in items)
        {
            if(i == null)
            {
                Debug.Log("NULL ITEM");
                continue;
            }
            Debug.Log("LOADED" + i.name);
        }
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
        foreach (Item i in items)
        {
            foreach (string tag in i.tags)
            {
                if (tag.Equals("holo"))
                {
                    initItem(i, initialPos);
                    initialPos.x += 0.3f;
                    break;
                }

            }
            //updateItems();
        }
    }


    private void initItem(Item i, Vector3 initialPos)
    {
        //TODO: Polymorphie beutzen ansonsten unnötig, doppelter code 
        Debug.Log("add item");
        Debug.Log(i.name);
        if (i.type.Equals("Switch") || i.type.Equals("Dimmer"))
        {
            GameObject newSwitch = Instantiate(switchPrefab, initialPos, Quaternion.identity);
            PositioningManager pm = newSwitch.GetComponent<PositioningManager>();
            OnSelectSwitch oss = newSwitch.GetComponent<OnSelectSwitch>();
            oss.ItemUrl = i.link;
            pm.ObjectAnchorStoreName = i.name;
            itemObjects.Add(i.name, newSwitch);
            newSwitch.BroadcastMessage("OnUpdate", i.state);
        }
        else if (i.type.Equals("Rollershutter"))
        {
            GameObject newRollerShutter = Instantiate(rollershutterPrefab, initialPos, Quaternion.identity);
            PositioningManager pm = newRollerShutter.GetComponent<PositioningManager>();
            BasicItemHandler bih = newRollerShutter.GetComponent<BasicItemHandler>();
            bih.ItemUrl = i.link;
            pm.ObjectAnchorStoreName = i.name;
            itemObjects.Add(i.name, newRollerShutter);
            newRollerShutter.BroadcastMessage("OnUpdate", i.state);
        }
        else if (i.type.Equals("Color"))
        {
            GameObject newColor = Instantiate(ColorPrefab, initialPos, Quaternion.identity);
            PositioningManager pm = newColor.GetComponent<PositioningManager>();
            BasicItemHandler bih = newColor.GetComponent<BasicItemHandler>();
            bih.ItemUrl = i.link;
            pm.ObjectAnchorStoreName = i.name;
            itemObjects.Add(i.name, newColor);
            newColor.BroadcastMessage("OnUpdate", i.state);
        }

    
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

    public void initObjectFromJson(string json)
    {
        Item iQR = JsonConvert.DeserializeObject<Item>(json);
        itemObjects = new Dictionary<string, GameObject>();
        WWWForm form = new WWWForm(); // Just fot getting the headers;
        Dictionary<string, string> headers = form.headers;
        headers["Accept"] = "application/json";
        var www = new WWW(iQR.link);
        StartCoroutine(WaitForSingleItemRequest(www));

    }

    private void handleSingleItemResponse(string json)
    {
        Item item = JsonConvert.DeserializeObject<Item>(json);
        foreach(Item icmp in items)
        {
            if(icmp.link.Equals(item.link))
            {
                return;
            }
        }
        var headPosition = Camera.main.transform.position;
        initItem(item, headPosition);
        Item[] itemsTmp = new Item[items.Length + 1];
        for (int i = 0; i < items.Length; i++) 
        {
            itemsTmp[i] = items[i];
        }
        itemsTmp[items.Length] = item;
        items = itemsTmp;
        save();
    }

    IEnumerator WaitForSingleItemRequest(WWW www)
    {
        yield return www;

        // check for errors
        if (string.IsNullOrEmpty(www.error))
        {
            Debug.Log("WWW Ok!: " + www.text);
            handleSingleItemResponse(www.text);
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
