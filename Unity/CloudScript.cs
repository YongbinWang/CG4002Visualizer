using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.ARFoundation;

public class CloudScript : MonoBehaviour
{
    private ARTrackedImageManager imageManager;
    private TMP_Text opponentStatus;
    private Renderer cloudRenderer;

    // Start is called before the first frame update

    private void Awake()
    {
        imageManager = FindObjectOfType<ARTrackedImageManager>();
    }
    void Start()
    {
        
        gameObject.AddComponent<ARAnchor>();
        cloudRenderer = gameObject.GetComponent<Renderer>();
    }

    private void Update()
    {
        CheckOpponent();
    }

    private void CheckOpponent()
    {
        Bounds cloudBounds = cloudRenderer.bounds;
        cloudBounds.Expand(new Vector3(0, 3.0f, 0)); // Expands the Y-axis by 1 meter (0.5m in both directions)
        foreach (var trackedImage in imageManager.trackables)
        {
            if (trackedImage.referenceImage.name == "Shield" && cloudBounds.Contains(trackedImage.transform.position))
            {
                GameObject textObject = GameObject.Find("OpponentStatus");
                opponentStatus = textObject.GetComponent<TextMeshProUGUI>(); // or UI.Text, depending on your setup
                opponentStatus.text = "Cloud detected!";
            }
            else
            {
                opponentStatus.text = "opponent";
            }
        }
    }
}