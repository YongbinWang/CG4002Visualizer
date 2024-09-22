using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARTrackedImageManager))]
public class ImageTracker : MonoBehaviour
{
    [SerializeField]
    private GameObject placeablePrefab;
    private ARTrackedImageManager imageManager;
    private Dictionary<string, GameObject> spawnedPrefabs = new Dictionary<string, GameObject>();
    public Player opponent;
    public TMP_Text opponentStatus;

    private void Awake()
    {
        imageManager = FindObjectOfType<ARTrackedImageManager>();
        GameObject newPrefab = Instantiate(placeablePrefab, Vector3.zero, Quaternion.Euler(0f, 180f, 0f));
        newPrefab.name = placeablePrefab.name;
        spawnedPrefabs.Add(placeablePrefab.name, newPrefab);
       
    }
    private void OnEnable()
    {
        imageManager.trackedImagesChanged += ImageChanged;
    }

    private void OnDisable()
    {
        imageManager.trackedImagesChanged -= ImageChanged;
    }
    private void ImageChanged(ARTrackedImagesChangedEventArgs eventargs)
    {
        foreach(ARTrackedImage trackedImage in eventargs.added)
        {
            UpdateImage(trackedImage);
        }
        foreach (ARTrackedImage trackedImage in eventargs.updated)
        {
            UpdateImage(trackedImage);
        }
        foreach (ARTrackedImage trackedImage in eventargs.removed)
        {
            string name = trackedImage.referenceImage.name;
            spawnedPrefabs[name].SetActive(false);
            opponentStatus.text = trackedImage.trackingState.ToString();

        }
    }
    private void UpdateImage(ARTrackedImage trackedImage)
    {
        string name = trackedImage.referenceImage.name;
        Vector3 position = trackedImage.transform.position;
        if (trackedImage.trackingState == TrackingState.Tracking)
        {
            if (opponent.currentShieldHealth > 0)
            {
                spawnedPrefabs[name].SetActive(true);
                spawnedPrefabs[name].transform.position =position;
            }
            else
            {
                spawnedPrefabs[name].SetActive(false);
            }
            opponentStatus.color = Color.green;
        }
        else
        {
            if (spawnedPrefabs.ContainsKey(name))
            {
                spawnedPrefabs[name].SetActive(false);
            }
            opponentStatus.color = Color.red;
        }
    }
}

