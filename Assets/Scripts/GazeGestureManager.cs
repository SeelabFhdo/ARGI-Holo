using UnityEngine;
using UnityEngine.VR.WSA.Input;

public class GazeGestureManager : MonoBehaviour
{
    public static GazeGestureManager Instance { get; private set; }

    // Represents the hologram that is currently being gazed at.
    public GameObject FocusedObject { get; private set; }
    public GameObject ManipulatingObject { get; private set; }

    public AudioClip ClickSound;
    private AudioSource audiosource;

    private bool IsManipulating;
    Vector3 manipulationStartingPosition;

    GestureRecognizer TapRecognizer;
    GestureRecognizer ManipulationRecognizer;

    // Use this for initialization
    void Awake()
    {
        Instance = this;
        audiosource = GetComponent<AudioSource>();
        // Set up a GestureRecognizer to detect Select gestures.
        TapRecognizer = new GestureRecognizer();
        ManipulationRecognizer = new GestureRecognizer();
        TapRecognizer.TappedEvent += (source, tapCount, ray) =>
        {
            // Send an OnSelect message to the focused object and its ancestors.
            if (FocusedObject != null)
            {   
                FocusedObject.SendMessageUpwards("OnSelect");
                Debug.Log("On select called: " + FocusedObject.name);

            } else
            {
                Debug.Log("Focused object is null");
            }
        };
        ManipulationRecognizer.SetRecognizableGestures(
                GestureSettings.ManipulationTranslate);

        ManipulationRecognizer.ManipulationStartedEvent += ManipulationRecognizer_ManipulationStartedEvent;
        ManipulationRecognizer.ManipulationUpdatedEvent += ManipulationRecognizer_ManipulationUpdatedEvent;
        ManipulationRecognizer.ManipulationCompletedEvent += ManipulationRecognizer_ManipulationCompletedEvent;
        ManipulationRecognizer.ManipulationCanceledEvent += ManipulationRecognizer_ManipulationCanceledEvent;

        TapRecognizer.StartCapturingGestures();
        ManipulationRecognizer.StartCapturingGestures();
    }

    private void ManipulationRecognizer_ManipulationStartedEvent(InteractionSourceKind source, Vector3 position, Ray ray)
    {
        if(FocusedObject != null)
        {
            Debug.Log("Started");
            manipulationStartingPosition = position;
            ManipulatingObject = FocusedObject.gameObject;
            ManipulatingObject.SendMessageUpwards("PerformManipulationStart", position);
            audiosource.PlayOneShot(ClickSound, 1);
            
        }
        
    }

    private void ManipulationRecognizer_ManipulationUpdatedEvent(InteractionSourceKind source, Vector3 position, Ray ray)
    {
        if(!IsManipulating)
        {
            if((position - manipulationStartingPosition).magnitude > 0.05)
            {
                IsManipulating = true;
            }
        }
        if (IsManipulating && ManipulatingObject != null)
        {
            Debug.Log("Updated");
            ManipulatingObject.SendMessageUpwards("PerformManipulationUpdate", position);
        }
    }

    private void ManipulationRecognizer_ManipulationCompletedEvent(InteractionSourceKind source, Vector3 position, Ray ray)
    {
        Debug.Log("Completed");
        if (ManipulatingObject != null)
        {
            ManipulatingObject.SendMessageUpwards("PerformManipulationStop");
        }
        IsManipulating = false;
        ManipulatingObject = null;
    }

    private void ManipulationRecognizer_ManipulationCanceledEvent(InteractionSourceKind source, Vector3 position, Ray ray)
    {
        Debug.Log("Canceled");
        if (ManipulatingObject != null)
        {
            ManipulatingObject.SendMessageUpwards("PerformManipulationStop");
        }
        IsManipulating = false;
        ManipulatingObject = null;
    }


    // Update is called once per frame
    void Update()
    {
        // Figure out which hologram is focused this frame.
        GameObject oldFocusObject = FocusedObject;

        // Do a raycast into the world based on the user&#39;s
        // head position and orientation.
        var headPosition = Camera.main.transform.position;
        var gazeDirection = Camera.main.transform.forward;

        RaycastHit hitInfo;
        if (Physics.Raycast(headPosition, gazeDirection, out hitInfo))
        {
            // If the raycast hit a hologram, use that as the focused object.
            FocusedObject = hitInfo.collider.gameObject;
        }
        else
        {
            // If the raycast did not hit a hologram, clear the focused object.
            FocusedObject = null;
        }

        // If the focused object changed this frame,
        // start detecting fresh gestures again.
        if (FocusedObject != oldFocusObject)
        {
            TapRecognizer.CancelGestures();
            TapRecognizer.StartCapturingGestures();
        }
    }
}