using UnityEngine;
using UnityEngine.UI;

public class MissScript : MonoBehaviour
{
    public GameObject imageToHide; // Assign this in the Inspector
    public float delay = 3f;  // Time to wait before hiding the image

    void Start()
    {
        imageToHide.SetActive(false);
    }
    public void hideMiss()
    {
        // Schedule the "HideImage" method to run after 'delay' seconds
        Invoke("HideImage", delay);
    }

    void HideImage()
    {
        // Hide the image by deactivating the GameObject or disabling the Image component
        imageToHide.SetActive(false);
        // Alternatively, disable just the Image component:
        // imageToHide.enabled = false;
    }

    public void ShowImage()
    {
        imageToHide.SetActive(true);
    }
}
