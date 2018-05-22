using UnityEngine;



/// <summary>
/// GestureAction performs custom actions based on
/// which gesture is being performed.
/// </summary>
public class PositioningManager : MonoBehaviour
{

    public bool RotationEnabled;

    private Vector3 manipulationPreviousPosition;

    private float rotationFactor;

    public string ObjectAnchorStoreName;


    UnityEngine.XR.WSA.Persistence.WorldAnchorStore anchorStore;


    void Start()
    {
        UnityEngine.XR.WSA.Persistence.WorldAnchorStore.GetAsync(AnchorStoreReady);
    }

    void AnchorStoreReady(UnityEngine.XR.WSA.Persistence.WorldAnchorStore store)
    {
        anchorStore = store;
        Debug.Log("looking for " + ObjectAnchorStoreName);
        anchorStore.Load(ObjectAnchorStoreName, gameObject);
        UnityEngine.XR.WSA.WorldAnchor anchor = gameObject.GetComponent<UnityEngine.XR.WSA.WorldAnchor>();
        if (anchor != null)
        {
            Destroy(anchor);
        }


    }

    void Update()
    {
        if(RotationEnabled)
            PerformRotation();
    }


    void PerformRotation()
    {
        Vector3 toTarget = (transform.position - Camera.main.transform.position).normalized;
        toTarget.y = 0;
        transform.rotation = Quaternion.LookRotation(toTarget);
    }

    void PerformManipulationStop()
    {
        UnityEngine.XR.WSA.WorldAnchor attachingAnchor = gameObject.AddComponent<UnityEngine.XR.WSA.WorldAnchor>();
        if (attachingAnchor.isLocated)
        {
            Debug.Log("Saving persisted position immediately");
            bool saved = anchorStore.Save(ObjectAnchorStoreName, attachingAnchor);
            Debug.Log("saved: " + saved);
            DestroyImmediate(attachingAnchor);
        }
        else
        {
            attachingAnchor.OnTrackingChanged += AttachingAnchor_OnTrackingChanged;
        }
    }

    void PerformManipulationStart(Vector3 position)
    {
        manipulationPreviousPosition = position;
        UnityEngine.XR.WSA.WorldAnchor anchor = gameObject.GetComponent<UnityEngine.XR.WSA.WorldAnchor>();
        if (anchor != null)
        {
            DestroyImmediate(anchor);
        }
        anchorStore.Delete(ObjectAnchorStoreName);
    }

    void PerformManipulationUpdate(Vector3 position) {

        
     
        
        Vector3 moveVector = Vector3.zero;

            // 4.a: Calculate the moveVector as position - manipulationPreviousPosition.
            moveVector = position - manipulationPreviousPosition;

            // 4.a: Update the manipulationPreviousPosition with the current position.
            manipulationPreviousPosition = position;

        // 4.a: Increment this transform's position by the moveVector.
        transform.position += moveVector * 3;
        PerformRotation();;
        
    }


    private void AttachingAnchor_OnTrackingChanged(UnityEngine.XR.WSA.WorldAnchor self, bool located)
    {
        if (located)
        {
            Debug.Log("Saving persisted position in callback");
            bool saved = anchorStore.Save(ObjectAnchorStoreName, self);
            Debug.Log("saved: " + saved);
            self.OnTrackingChanged -= AttachingAnchor_OnTrackingChanged;
            Destroy(gameObject.GetComponent<UnityEngine.XR.WSA.WorldAnchor>());
        }
    }
}