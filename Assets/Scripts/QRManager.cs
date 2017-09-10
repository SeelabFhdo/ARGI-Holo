using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class QRManager : MonoBehaviour {
    public GameObject textMeshObject;
    public PersistenceManager persistence;

    private void Start()
    {
        this.textMesh = this.textMeshObject.GetComponent<TextMesh>();
        this.OnReset();
    }
    public void OnScan()
    {
        this.textMeshObject.SetActive(true);
        this.textMesh.text = "scanning...";

#if !UNITY_EDITOR
    MediaFrameQrProcessing.Wrappers.ZXingQrCodeScanner.ScanFirstCameraForQrCode(
        result =>
        {
          UnityEngine.WSA.Application.InvokeOnAppThread(() =>
          {
            this.textMesh.text = result ?? "not found";
            this.textMeshObject.SetActive(false);
            if(result != null) {
             persistence.initObjectFromJson(result);
            }
          }, 
          false);
        },
        TimeSpan.FromSeconds(30));
#endif
    }
    public void OnReset()
    {
        this.textMesh.text = "say scan to start";
    }
    TextMesh textMesh;
}
