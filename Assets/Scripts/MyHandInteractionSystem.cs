using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyHandInteractionSystem : MonoBehaviour
{
    public GameObject avatar;
    public GameObject rIndex;

    public GameObject selector;
    public GameObject prefabSelect;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(rIndex == null)
        {
            rIndex = FindGameObjectInChildren( avatar,"CC_Base_R_Index3");
        }
        else
        {
            if (rIndex != null)
            {
                if (selector == null)
                {
                    selector = Instantiate(prefabSelect);
                }
                selector.transform.position = rIndex.transform.position;
            }
        }
        
    }
    public GameObject FindGameObjectInChildren(GameObject parent, string name)
    {
        // Check if the current GameObject has the desired name
        if (parent.name == name)
        {
            return parent;
        }

        // Iterate through all child objects recursively
        for (int i = 0; i < parent.transform.childCount; i++)
        {
            GameObject foundObject = FindGameObjectInChildren(parent.transform.GetChild(i).gameObject, name);
            if (foundObject != null)
            {
                return foundObject;
            }
        }

        // If the desired GameObject is not found in the children, return null
        return null;
    }
}
