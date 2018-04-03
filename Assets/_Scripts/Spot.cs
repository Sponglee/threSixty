using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spot : MonoBehaviour {

    //blocked for expanding
    private bool blocked;
    public bool Blocked    {get{return blocked;}set{blocked = value;}}


    // Use this for initialization
    void Start () {
        blocked = false;
	}
	
	// Update is called once per frame
	void Update () {
      
	}


    public void OnTriggerEnter2D(Collider2D other)
    {
       
        if (other.CompareTag("line") && gameObject.CompareTag("spot"))
        {
            
            GameManager.Instance.currentSpot = gameObject;
        }
        else if (other.CompareTag("line") && gameObject.CompareTag("spawn"))
        {
            GameManager.Instance.currentSpawn = gameObject;
        }
    }
}
