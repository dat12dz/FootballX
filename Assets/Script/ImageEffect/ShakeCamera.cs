using UnityEngine;

public class CameraShake : MonoBehaviour
{
    // Shake parameters
    public float shakeDuration = 0.5f; // Duration of the shake
    public float shakeIntensity = 0.1f; // Intensity of the shake

    // Internal variables
    private Vector3 originalPosition;
    private float shakeTimer = 9999f;

    private void Start()
    {
        // Save the original position of the camera
        originalPosition = transform.localPosition;
    }
   [SerializeField] float ShakeSpeed = 0.2f;
    private void Update()
    {
        // Check if the boolean variable 'a' is true
        if (shake)
        {
            // Generate a random offset within the intensity for the shake
            Vector3 shakeOffset = Random.insideUnitSphere * shakeIntensity;

            // Apply the shake offset to the camera's position
            transform.localPosition = Vector3.Lerp(transform.localPosition, originalPosition + shakeOffset, Time.deltaTime * ShakeSpeed);


            // Decrease the shake timer
            shakeTimer -= Time.deltaTime;
        }
        else
        {
            // Reset the camera position when 'a' is false or shake timer is over
            transform.localPosition = originalPosition;
        }
    }

    // Boolean variable to trigger the camera shake
    public bool shake = false;

    // Call this method to manually trigger the camera shake
    public void ShakeCamera()
    {
        shakeTimer = shakeDuration;
    }
}
