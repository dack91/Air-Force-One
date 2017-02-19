using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirBlastBehavior : MonoBehaviour {

    public AudioSource alarmSound;
    public AudioSource windSound;

    // Time range between air blasts
    public float maxTime = 5;
    public float minTime = 2;

    // Timer
    private float airTimer;
    private float currTime;

    private bool isBlastingAir;

    // Use this for initialization
    void Start () {
        alarmSound = GetComponent<AudioSource>();
        currTime = 0;
        setAirTimer();
        isBlastingAir = false;
	}
	
	// Update is called once per frame
	void Update () {
        // Increment timer if not already blasting
        if (!isBlastingAir)
        {
            currTime += Time.deltaTime;

            // If timer goes off, start air blast, notify player of blast through isBlastingAir
            if (currTime >= airTimer)
            {
                alarmSound.Play();
                windSound.Play();
                GetComponent<Renderer>().material.color = (new Color(1, 1, 0)); // Change to warning yellow
                isBlastingAir = true;
            }
        }    
	}

    // Reset timer in range
    void setAirTimer()
    {
        airTimer = Random.Range(minTime, maxTime);
    }
    // Get if air is blasting
    public bool getisBlastingAir()
    {
        return isBlastingAir;
    }
    // Reset timer after air blast
    public void resetAirBlast()
    {
        isBlastingAir = false;
        currTime = 0;
        setAirTimer();
        GetComponent<Renderer>().material.color = (new Color(1, 1, 1)); // Change to neutral white
        alarmSound.Stop();
        windSound.Stop();
    }
}
