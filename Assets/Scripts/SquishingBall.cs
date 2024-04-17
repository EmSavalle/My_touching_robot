using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquishingBall : MonoBehaviour
{
    public float maxSquishAmount = 0.5f; // Maximum amount of squish when fully penetrated
    public float penetrationThreshold = 0.5f; // How deep the penetration needs to be to reach max squish
    public float squishSpeed = 1.0f; // Speed of squishing effect

    private Mesh originalMesh;
    private Vector3[] originalVertices;
    private MeshFilter meshFilter;
    private bool isDeforming = false;
    private float currentSquishAmount = 0.0f;
    private Collision collision;

    void Start()
    {
        // Get the original mesh and vertices
        meshFilter = GetComponent<MeshFilter>();
        originalMesh = meshFilter.mesh;
        originalVertices = originalMesh.vertices;
    }

    void OnCollisionEnter(Collision collision)
    {
        // Check if the collision is with the hand or another object
        // For simplicity, let's assume it deforms upon any collision
        isDeforming = true;
        this.collision = collision;
    }

    void OnCollisionExit(Collision collision)
    {
        // Reset deformation when the collision ends
        isDeforming = false;
        this.collision = null;
        ResetMesh();
    }

    void Update()
    {
        if (isDeforming)
        {
            DeformMesh();
        }
    }
    /*
    void badDeformMesh()
    {
        // Calculate penetration depth based on collision contact points
        float maxDepth = 0.0f;
        foreach (ContactPoint contact in GetComponent<Collider>().attachedRigidbody.GetContacts())
        {
            if (contact.otherCollider == GetComponent<Collider>())
            {
                float depth = contact.separation;
                if (depth > maxDepth)
                {
                    maxDepth = depth;
                }
            }
        }

        // Calculate squish amount based on penetration depth
        float targetSquishAmount = Mathf.Clamp(maxDepth * squishSpeed, 0.0f, maxSquishAmount);
        currentSquishAmount = Mathf.Lerp(currentSquishAmount, targetSquishAmount, Time.deltaTime);

        // Create a copy of the original vertices
        Vector3[] deformedVertices = originalVertices.Clone() as Vector3[];

        // Apply squishing deformation to vertices
        for (int i = 0; i < deformedVertices.Length; i++)
        {
            // Squish the sphere along the y-axis
            float squishScale = Mathf.Clamp(1.0f - currentSquishAmount, 0.1f, 1.0f); // Ensure squish is between 0.1 and 1.0
            deformedVertices[i].y *= squishScale;
        }

        // Update the mesh with the deformed vertices
        Mesh deformedMesh = new Mesh();
        deformedMesh.vertices = deformedVertices;
        deformedMesh.triangles = originalMesh.triangles;
        deformedMesh.RecalculateNormals();

        // Assign the deformed mesh back to the mesh filter
        meshFilter.mesh = deformedMesh;
    }

    */
    void DeformMesh()
    {
        Debug.Log("Deforming");
        // Calculate penetration depth based on collision contacts
        float maxDepth = 0.0f;
        foreach (ContactPoint contact in collision.contacts)
        {
            float depth = Vector3.Dot(contact.normal, (transform.position - contact.point));
            if (depth > maxDepth)
            {
                maxDepth = depth;
            }
        }

        // Calculate squish amount based on penetration depth
        float targetSquishAmount = Mathf.Clamp(maxDepth * squishSpeed, 0.0f, maxSquishAmount);
        currentSquishAmount = Mathf.Lerp(currentSquishAmount, targetSquishAmount, Time.deltaTime);

        // Create a copy of the original vertices
        Vector3[] deformedVertices = originalVertices.Clone() as Vector3[];

        // Apply squishing deformation to vertices
        for (int i = 0; i < deformedVertices.Length; i++)
        {
            // Squish the sphere along the y-axis
            float squishScale = Mathf.Clamp(1.0f - currentSquishAmount, 0.1f, 1.0f); // Ensure squish is between 0.1 and 1.0
            deformedVertices[i].y *= squishScale;
        }

        // Update the mesh with the deformed vertices
        Mesh deformedMesh = new Mesh();
        deformedMesh.vertices = deformedVertices;
        deformedMesh.triangles = originalMesh.triangles;
        deformedMesh.RecalculateNormals();

        // Assign the deformed mesh back to the mesh filter
        meshFilter.mesh = deformedMesh;
    }
    /*void SimpleDeformMesh()
    {
        // Create a copy of the original vertices
        Vector3[] deformedVertices = originalVertices.Clone() as Vector3[];

        // Apply squishing deformation to vertices
        for (int i = 0; i < deformedVertices.Length; i++)
        {
            // Squish the sphere along the y-axis
            float squishScale = Mathf.Clamp(1.0f - squishAmount, 0.1f, 1.0f); // Ensure squish is between 0.1 and 1.0
            deformedVertices[i].y *= squishScale;
        }

        // Update the mesh with the deformed vertices
        Mesh deformedMesh = new Mesh();
        deformedMesh.vertices = deformedVertices;
        deformedMesh.triangles = originalMesh.triangles;
        deformedMesh.RecalculateNormals();

        // Assign the deformed mesh back to the mesh filter
        meshFilter.mesh = deformedMesh;
    }
    */
    void ResetMesh()
    {
        // Reset mesh to its original vertices
        meshFilter.mesh.vertices = originalVertices;
        meshFilter.mesh.RecalculateNormals();
    }
}
