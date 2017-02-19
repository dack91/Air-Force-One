using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour {

    public int rotationSpeed;
    private bool hasBeenPickedUp;
        
	// Use this for initialization
	void Start () {
        hasBeenPickedUp = false;
	}
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(Vector3.up * (rotationSpeed * Time.deltaTime));		
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player" && !hasBeenPickedUp)
        {
            GetComponent<AudioSource>().Play();
        }
    }

    public bool getHasBeenPickedUp()
    {
        return hasBeenPickedUp;
    }
    public void pickUpItem()
    {
        hasBeenPickedUp = true; // Make sure item is only picked up once
    }
}
