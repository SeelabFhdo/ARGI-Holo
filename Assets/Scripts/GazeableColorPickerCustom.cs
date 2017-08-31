using HoloToolkit.Unity.InputModule;
using UnityEngine;
using UnityEngine.Events;

namespace Lighthouse
{
    public class GazeableColorPickerCustom : MonoBehaviour
    {
        public Renderer rendererComponent;

        [System.Serializable]
        public class PickedColorCallback : UnityEvent<Color> { }

        public PickedColorCallback OnPickedColor = new PickedColorCallback();

        void Update()
        {
         
        }

        void UpdatePickedColor(PickedColorCallback cb)
        {
            var headPosition = Camera.main.transform.position;
            var gazeDirection = Camera.main.transform.forward;

            RaycastHit hit;
            if (!Physics.Raycast(headPosition, gazeDirection, out hit))
            {
                return;
            }
            if (hit.transform.gameObject != rendererComponent.gameObject) return;

            Texture2D texture = rendererComponent.material.mainTexture as Texture2D;
            Vector2 pixelUV = hit.textureCoord;
            pixelUV.x *= texture.width;
            pixelUV.y *= texture.height;

            Color col = texture.GetPixel((int)pixelUV.x, (int)pixelUV.y);
            cb.Invoke(col);
            SendMessageUpwards("OnColorCommand", col);
        }

        public void SetColor(Color c)
        {
            OnPickedColor.Invoke(c);
        }

        public void OnSelect()
        {
            Debug.Log("Color selected");
            UpdatePickedColor(OnPickedColor);
        }
    }
}