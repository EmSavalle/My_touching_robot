using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    public Material red, orange,green;

    public bool selected;
    public float selectionTime;
    public float lastTouch;
    public bool touching;
    public int value;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (touching)
        {
            touched();
        }
    }
    public void reset()
    {
        touching = false;
        selected = false;
        unselected();
        untouched();
    }
    public void touched()
    {
        if (!touching)
        {
            touching = true;
            lastTouch = Time.time;
            if (!selected)
            {
                selecting();
            }
        }
        else if (Time.time > lastTouch + selectionTime)
        {
            setSelected();
        }
        
    }
    public void selecting()
    {
        gameObject.GetComponent<MeshRenderer>().material = orange;
    }
    public void setSelected()
    {
        selected = true;
        gameObject.GetComponent<MeshRenderer>().material = green;

    }
    public void unselected()
    {
        selected = false;
        gameObject.GetComponent<MeshRenderer>().material = red;
    }
    public void untouched()
    {
        touching = false;
        if(!selected)
        {
            gameObject.GetComponent<MeshRenderer>().material = red;
        }
        
    }
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Contact" || "CC_Base_R_Index3" == other.gameObject.name)
        {
            Debug.Log("Entering" + other.gameObject.name);
            touched();
        }
    }


    // Called when another collider exits the cube's collider
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Contact" || "CC_Base_R_Index3" == other.gameObject.name)
        {
            Debug.Log("Leaving" + other.gameObject.name);
            untouched();
        }
    }
}

