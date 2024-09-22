using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ReloadScript : MonoBehaviour
{
    public float timerDuration = 3f;
    public Slider timerSlider;

    float timer;

    void Start()
    {
        timerSlider.value = 0;
    }

    // Call this function to start the reload timer
    public void StartReload()
    {
        StartCoroutine(ReloadTimer());
    }

    private IEnumerator ReloadTimer()
    {
        timer = 0;

        // Loop until the timer reaches the duration
        while (timer < timerDuration)
        {
            timer += Time.deltaTime;

            // Animate timer from 1 to 0
            float normalizedTime = Mathf.Clamp01(timer / timerDuration);

            // Update slider
            timerSlider.value = normalizedTime;

            yield return null; // Wait until the next frame
        }

        // Reset slider and timer
        timerSlider.value = 0;
        timer = 0;
    }
}
