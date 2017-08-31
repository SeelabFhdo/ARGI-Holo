using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorItemHandler : BasicItemHandler {

    private Color color;
	// Use this for initialization
	void OnColorCommand(Color c)
    {
        color = c;
        sendColor(c);
        calculateText(c);
    }

    private void sendColor(Color c)
    {
        float h;
        float s;
        float v;
        Color.RGBToHSV(c, out h, out s, out v);
        h *= 360;
        s *= 100;
        v *= 100;
        OnCommand(string.Format("{0},{1},{2}", h, s, v));
        
    }

    private void calculateText(Color c )
    {
        float h;
        float s;
        float v;
        Color.RGBToHSV(c, out h, out s, out v);
        BroadcastMessage("SetText", v > 0 ? "ON" : "OFF");

    }

    private void OnToggleCommand()
    {
        if(color == null)
        {
            color = Color.white;
        } else if(color.r > 0 || color.g > 0 || color.b > 0)
        {
            color = Color.black;
        } else
        {
            color = Color.white;
        }
        sendColor(color);
        calculateText(color);
    }

    public override void OnUpdate(string state)
    {
        if(state == null || state.Equals("UNDEFINED"))
        {
            return;
        }

        string[] hsvString = state.Split(',');
        Color c = Color.HSVToRGB(float.Parse(hsvString[0]) / 360.0f, float.Parse(hsvString[1]) / 100.0f, float.Parse(hsvString[2]) / 100.0f);
        this.color = c;
        calculateText(c);
        BroadcastMessage("SetColor", c);
    }
}
