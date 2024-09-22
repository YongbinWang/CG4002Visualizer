using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleParticleSystem : MonoBehaviour
{
    // Reference to the Particle System component
    private ParticleSystem particleSystem;

    // To keep track of whether the system is active or not
    private bool isPlaying = false;

    void Start()
    {
        // Get the ParticleSystem component attached to the same GameObject
        particleSystem = GetComponent<ParticleSystem>();

        // Check if a Particle System is attached
        if (particleSystem == null)
        {
            Debug.LogError("No Particle System found on this GameObject.");
        }
    }

    void Update()
    {
        // Example: Toggle particle system with the space bar
        if (Input.GetKeyDown(KeyCode.Space))
        {
            ToggleParticles();
        }
    }

    // Method to toggle the particle system
    public void ToggleParticles()
    {
        if (particleSystem != null)
        {
            if (isPlaying)
            {
                particleSystem.Stop();
            }
            else
            {
                particleSystem.Play();
            }

            // Update the state
            isPlaying = !isPlaying;
        }
    }
}
