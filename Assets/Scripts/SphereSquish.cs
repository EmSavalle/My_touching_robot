using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereSquish : MonoBehaviour
{
    public float squishAmount = 0.2f; // Amount to squish the sphere
    public float squishSpeed = 5f; // Speed of the squish effect
    public float minScaleFactor = 0.5f; // Minimum scale factor relative to original scale
    private float originSquishSpeed = 0;
    private Vector3 originalScale; // Original scale of the sphere
    private Vector3 squishedScale; // Target squished scale
    private Coroutine squishCoroutine; // Reference to the squish coroutine

    void Start()
    {
        // Store the original scale of the sphere
        originalScale = transform.localScale;
        // Calculate the squished scale based on the squish amount
        squishedScale = originalScale - Vector3.up * squishAmount;
        originSquishSpeed = squishSpeed;
    }

    void ApplySquish()
    {
        // Apply a smooth squish effect using a coroutine
        if (squishCoroutine != null)
        {
            StopCoroutine(squishCoroutine); // Stop previous coroutine if running
        }
        squishSpeed = originSquishSpeed;
        squishCoroutine = StartCoroutine(SquishSmooth(transform.localScale, squishedScale));
    }

    void ApplyUnsquish()
    {
        // Restore the original scale
        if (squishCoroutine != null)
        {
            StopCoroutine(squishCoroutine); // Stop previous coroutine if running
        }
        squishSpeed *= 2;
        squishCoroutine = StartCoroutine(SquishSmooth(transform.localScale, originalScale));
    }

    IEnumerator SquishSmooth(Vector3 startScale, Vector3 targetScale)
    {
        float elapsedTime = 0f;

        while (elapsedTime < squishSpeed)
        {
            // Interpolate between the current scale and the target scale
            transform.localScale = Vector3.Lerp(startScale, targetScale, elapsedTime / squishSpeed);

            // Clamp the scale to ensure it doesn't go below the minimum scale
            transform.localScale = Vector3.Max(transform.localScale, originalScale * minScaleFactor);

            // Update the elapsed time
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        // Ensure we end exactly at the target scale
        transform.localScale = targetScale;
    }

    void OnCollisionEnter(Collision collision)
    {
        // Apply the squish effect
        ApplySquish();
    }

    void OnCollisionExit(Collision collision)
    {

        // Restore the original shape when collision ends
        ApplyUnsquish();
    }
}
