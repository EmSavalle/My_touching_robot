using UnityEngine;

public class VRSlider : MonoBehaviour
{
    // The sphere prefab to instantiate
    public GameObject spherePrefab;
    public GameObject Left;
    public GameObject Right;
    public GameObject sphere;
    // The maximum and minimum values for the slider
    public float percentage;
    public float percentToA;
    public float percentToB;
    // The position where the sphere will be instantiated
    public Transform sphereSpawnPoint;

    // The variable to hold the current value of the slider
    private int sliderValue = -1;

    // Update is called once per frame
    void Update()
    {
    }

    public void reset()
    {
        Destroy(sphere);
        sphere = null;
        percentage = -1;
    }
    void OnCollisionStay(Collision collision)
    {
        Vector3 point = collision.contacts[0].point;
        if (sphere == null)
        {
            sphere = Instantiate(spherePrefab, point, Quaternion.identity, gameObject.transform);
        }
        else
        {
            sphere.transform.position = point;
        }
        float distanceAC = Vector3.Distance(Left.transform.position, point);
        float distanceBC = Vector3.Distance(Right.transform.position, point);
        float totalDistance = distanceAC + distanceBC;

        percentToA = (distanceAC / totalDistance) * 100f;
        percentToB = (distanceBC / totalDistance) * 100f;
        percentage = percentToA;
        /*if (collision.gameObject.name.Contains("Index") || collision.gameObject.name.Contains("index"))
        {

            Vector3 point = collision.contacts[0].point;
            if (sphere == null)
            {
                sphere = Instantiate(spherePrefab, point, Quaternion.identity);
            }
            else
            {
                sphere.transform.position = point;
            }
            float distanceAC = Vector3.Distance(Left.transform.position, point);
            float distanceBC = Vector3.Distance(Right.transform.position, point);
            float totalDistance = distanceAC + distanceBC;

            percentToA = (distanceAC / totalDistance) * 100f;
            percentToB = (distanceBC / totalDistance) * 100f;
            percentage = percentToA;
        }
        else
        {
            Debug.Log("Wrong collider " + collision.gameObject.name);
        }*/
    }
    void OnCollisionEnter(Collision collision)
    {
        Vector3 point = collision.contacts[0].point;
        if (sphere == null)
        {
            sphere = Instantiate(spherePrefab, point, Quaternion.identity, gameObject.transform);
        }
        else
        {
            sphere.transform.position = point;
        }
        float distanceAC = Vector3.Distance(Left.transform.position, point);
        float distanceBC = Vector3.Distance(Right.transform.position, point);
        float totalDistance = distanceAC + distanceBC;

        percentToA = (distanceAC / totalDistance) * 100f;
        percentToB = (distanceBC / totalDistance) * 100f;
        percentage = percentToA;
        /*if(collision.gameObject.name.Contains("Index") || collision.gameObject.name.Contains("index"))
        {
            Vector3 point = collision.contacts[0].point;
            if (sphere == null)
            {
                sphere = Instantiate(spherePrefab, point, Quaternion.identity);
            }
            else
            {
                sphere.transform.position = point;
            }
            float distanceAC = Vector3.Distance(Left.transform.position, point);
            float distanceBC = Vector3.Distance(Right.transform.position, point);
            float totalDistance = distanceAC + distanceBC;

            percentToA = (distanceAC / totalDistance) * 100f;
            percentToB = (distanceBC / totalDistance) * 100f;
            percentage = percentToA;
        }
        else
        {
            Debug.Log("Wrong collider "+ collision.gameObject.name);
        }*/
    }
}
