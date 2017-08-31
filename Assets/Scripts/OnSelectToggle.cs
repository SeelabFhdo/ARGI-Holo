using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnSelectToggle : MonoBehaviour {

	void OnSelect()
    {
        SendMessageUpwards("OnToggleCommand");
    }
}
