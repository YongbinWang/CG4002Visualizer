using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;  // For UI
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

[RequireComponent(typeof(ARTrackedImageManager))]
public class RainBombSpawner : MonoBehaviour
{
    [SerializeField] private GameObject objectToMovePrefab;  // The object prefab to spawn
    private ARTrackedImageManager imageManager;
    private GameObject spawnedObject;
    public GameObject hit;
    public GameObject miss;
    private bool shouldMove = false;
    private Vector3 targetPosition;
    public float moveSpeed = 15f;  // Speed at which the object moves
    private bool isImageTracked = false;
    public GameObject Cloud;
    public Player player;

    // Reference to your UI Button
    [SerializeField] private Button spawnButton;

    private void Awake()
    {
        imageManager = FindObjectOfType<ARTrackedImageManager>();
    }

    private void OnEnable()
    {
        imageManager.trackedImagesChanged += OnTrackedImagesChanged;

        // Attach the spawn action to the button's onClick event
        if (spawnButton != null)
        {
            spawnButton.onClick.AddListener(OnSpawnButtonClick);
        }
    }

    private void OnDisable()
    {
        imageManager.trackedImagesChanged -= OnTrackedImagesChanged;

        if (spawnButton != null)
        {
            spawnButton.onClick.RemoveListener(OnSpawnButtonClick);
        }
    }

    // Handle AR tracked image changes
    private void OnTrackedImagesChanged(ARTrackedImagesChangedEventArgs eventArgs)
    {
        foreach (ARTrackedImage trackedImage in eventArgs.added)
        {
            isImageTracked = true;
            UpdateTargetPosition(trackedImage);
        }

        foreach (ARTrackedImage trackedImage in eventArgs.updated)
        {
            UpdateTargetPosition(trackedImage);
        }

        foreach (ARTrackedImage trackedImage in eventArgs.removed)
        {
            isImageTracked = false;
        }
    }

    // Method called when the spawn button is clicked
    private void OnSpawnButtonClick()
    {
        if (player.currentBombs > 0)
        {
            Vector3 startPosition = Camera.main.transform.position + Camera.main.transform.up * 2f;
            spawnedObject = Instantiate(objectToMovePrefab, startPosition, Quaternion.identity);
            player.currentBombs--;
        }

        if (isImageTracked)  // Only spawn if an image is tracked
        {
            shouldMove = true;
        }
        else
        {
            Vector3 shootDirection = Camera.main.transform.forward;
            Rigidbody rb = spawnedObject.GetComponent<Rigidbody>();
            rb.AddForce(shootDirection * moveSpeed, ForceMode.Impulse);
            Destroy(spawnedObject, 3f);
            Instantiate(miss, Camera.main.transform.position + (Camera.main.transform.forward * 1f) - (Camera.main.transform.up * 0.1f), Quaternion.Euler(0f, 0f, 0f));
        }
    }

    // Update the target position based on the AR-tracked image
    private void UpdateTargetPosition(ARTrackedImage trackedImage)
    {
        targetPosition = trackedImage.transform.position;
        if (trackedImage.trackingState == TrackingState.Tracking)
        {
            isImageTracked = true;
        }
        else
        {
            isImageTracked = false;
        }
    }

    private void Update()
    {
        // If the object should move, move it towards the target position
        if (shouldMove && spawnedObject != null)
        {
            MoveObjectToTarget();
        }
    }

    // Move the object towards the AR-tracked image
    private void MoveObjectToTarget()
    {
        // Move the object towards the target position using Vector3.MoveTowards
        spawnedObject.transform.position = Vector3.MoveTowards(
            spawnedObject.transform.position,
            targetPosition,
            moveSpeed * Time.deltaTime);

        // Check for collision with the AR-tracked image
        if (Vector3.Distance(spawnedObject.transform.position, targetPosition) < 0.01f)
        {
            Destroy(spawnedObject);  // Destroy the object upon "collision"
            shouldMove = false;  // Stop moving
            Instantiate(Cloud, (targetPosition + Camera.main.transform.up * 1.5f), Quaternion.identity);
            Instantiate(hit, targetPosition, Quaternion.identity);
        }
    }
}
