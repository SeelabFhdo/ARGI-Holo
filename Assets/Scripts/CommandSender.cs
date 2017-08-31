using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandSender : MonoBehaviour {

    public string Command;

    void OnSelect()
    {
        SendMessageUpwards("OnCommand", Command);
    }
}
