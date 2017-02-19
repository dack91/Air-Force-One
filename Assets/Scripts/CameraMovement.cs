using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour {
    Transform player;
    public float xMargin = 1f;
    public float xSmooth = 8f;
    public float min = 0f;
    public float max = 17f;

    // Use this for initialization
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Allow limited player movement without matched camera movement
    bool checkMove()
    {
        return Mathf.Abs(transform.position.x - player.position.x) > xMargin;
    }

    void trackPlayer()
    {
        float targetX = transform.position.x;   // camera position

        // If player has moved outside dead zone, move camera
        if (checkMove())
        {
            targetX = Mathf.Lerp(transform.position.x, player.position.x, xSmooth * Time.deltaTime);
        }

        // Keep camera movement within bounds of level
        targetX = Mathf.Clamp(targetX, min, max);
        transform.position = new Vector3(targetX, transform.position.y, transform.position.z);
    }

    private void FixedUpdate()
    {
        trackPlayer();
    }

    // Update is called once per frame
    void Update () {
    }
}
