using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PlayerMovement : MonoBehaviour {
    private GameManager GM;

    private Rigidbody rb;           // Player physics forces
    public float force;             // horizontal movement
    public float jumpForce;         // jump
    public float airForce;          // air blasting
    private bool isJumping;         // disable infinite jumps
    private bool isUncovered;       // cover from air blasts
    private GameObject currCover;   // for levels with more than one cover object

    // Player boundaries in level
    public float minX;              // level start x position
    public float maxX;              // level end x position

    public Texture tex1;            // Regular player texture
    public Texture tex2;            // Saved immunity player texture
    public Texture tex3;            // Death player texture

    // Player sounds
    public AudioClip deathSound;
    public AudioClip savedSound;

    private int saves;              // number of player saves remaining
    private bool isDead;
    private bool isSaved;
    public bool isAirWarning;       // Active when warning plays just before air blast

    // Control coroutines to only run once
    private bool isAirCoroutineRunning;
    private bool isSavedCoroutineRunning;

    private GameObject light;       // warning light object

    // Use this for initialization
    void Start () {
        GM = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();

        rb = GetComponent<Rigidbody>();
        light = GameObject.FindGameObjectWithTag("WarningLight");
        isJumping = false;
        isUncovered = true;
        isDead = false;
        isSaved = false;
        isAirWarning = false;
        isAirCoroutineRunning = false;
        isSavedCoroutineRunning = false;
    }
	
	// Update is called once per frame
	void Update () {
        //Debug.Log("Cover x: " + other.gameObject.transform.position.x);
        //Debug.Log("Player x: " + transform.position.x);

        if (currCover != null)
        {
            if (Mathf.Abs(currCover.transform.position.x - transform.position.x) < 0.5f)
            {
                isUncovered = false;
            }
            else
            {
                isUncovered = true;
            }
        }

        if (isDead && ! isSaved)
        {
            // physically make player smaller to compensate for orthographic 
            // view as player moves away from camera
            transform.localScale = Vector3.Lerp(transform.localScale, 
                new Vector3(0.4f, 0.4f, 0.4f), 3f * Time.deltaTime);
            // apply red death texture
            GetComponent<Renderer>().material.SetTexture("_MainTex", tex3);    

        }
        else if (isSaved)
        {
            // change texture to yellow glow to indicate temp immunity
            GetComponent<Renderer>().material.SetTexture("_MainTex", tex2);
            StartCoroutine(playerIsSaved());
        }

        if (light.GetComponent<AirBlastBehavior>().getisBlastingAir())
        {
            StartCoroutine(playAirBlast());

            // continually check if player should die while air is blasting
            if (!isAirWarning)
            {
                if (isUncovered && !isSaved && !isDead)
                {
                    killPlayer();
                }
            }
        }

        //Debug.Log("PosX: " + cam.WorldToScreenPoint(transform.position));
    }
    // Physics Updates
    private void FixedUpdate()
    {
    }

    private void LateUpdate()
    {
        // move player back to playable area
        if (transform.position.x <= minX)
        {
            transform.position = new Vector3 (minX, transform.position.y, transform.position.z);
        }
        else if (transform.position.x >= maxX)
        {
            transform.position = new Vector3(maxX, transform.position.y, transform.position.z);
        }
    }

    public void movePlayer(float dir)
    {
        // float translateX = Input.GetAxis("Horizontal");
        //dir *= Time.deltaTime;
        //float moveX = dir * force;

        //// clamp player movement
        //if (transform.position.x + moveX >= minX && transform.position.x + moveX <= maxX)
        //{
        //    transform.Translate(moveX, 0, 0);
        //}

        ////transform.Translate(dir * force, 0, 0);

        //if (isGrounded)
        //{
            rb.velocity = new Vector3(force * dir, rb.velocity.y, 0);
        //}
    }
    public void jump()
    {
            rb.AddForce(0, 10 * jumpForce, 0);
            isJumping = true;
    }

    private IEnumerator playAirBlast()
    {
        // Play coroutine through once
        if (!isAirCoroutineRunning)
        {
            isAirCoroutineRunning = true;   // on first call mark coroutine has started

            isAirWarning = true;

            yield return new WaitForSeconds(1);

            isAirWarning = false;

            light.GetComponent<Renderer>().material.color = (new Color(1, 0, 0));   // Change to blasting red

            if (isUncovered && !isSaved)
            {
                killPlayer();
            }
            else
            {
                yield return new WaitForSeconds(2);

                light.GetComponent<AirBlastBehavior>().resetAirBlast();
                isDead = false;
                isSaved = false;
                isAirCoroutineRunning = false;
                GetComponent<Renderer>().material.SetTexture("_MainTex", tex1);
            }
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        // Disable jump while in air
        if (collision.gameObject.tag == "Ground")
        {
            isJumping = false;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Cover")
        {
            currCover = other.gameObject;
        }
        if (other.gameObject.tag == "CoverAesthetic")
        {
            other.gameObject.GetComponent<Renderer>().enabled = false;
        }
        else if (other.gameObject.tag == "SavePickup" && !other.gameObject.GetComponent<Pickup>().getHasBeenPickedUp())
        {   
            saves++;
            GM.setSaves(saves);
            other.gameObject.GetComponent<Pickup>().pickUpItem();
            StartCoroutine(pickupTriggered(other.gameObject));
        }
        else if (other.gameObject.tag == "Finish")
        {
            GM.setSaves(saves);
            GM.loadNextLevel();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "CoverAesthetic")
        {
            other.gameObject.GetComponent<Renderer>().enabled = true;
        }
    }
   
    private void killPlayer()
    {
        isDead = true;

        //Apply air force away from camera
        rb.constraints = RigidbodyConstraints.None;
        rb.AddForce(0, 0, 100 * airForce);

        GetComponent<AudioSource>().clip = deathSound;
        GetComponent<AudioSource>().Play();

        // Kill player and restart level
        StartCoroutine(playerDeadCoroutine());
    }
    // Death Process
    private IEnumerator playerDeadCoroutine()
    {
        yield return new WaitForSeconds(4);

        GetComponent<AudioSource>().Stop(); // stop death sfx

        // Reset player to level start
        transform.position = new Vector3(-7.001698f, -4.125368f, 0);
        transform.localScale = new Vector3(1f, 1f, 1f);
        rb.velocity = new Vector3(0f, 0f, 0f);
        GetComponent<Renderer>().material.SetTexture("_MainTex", tex1);

        // Reset air timer and player state
        light.GetComponent<Renderer>().material.color = (new Color(1, 1, 1));
        light.GetComponent<AirBlastBehavior>().resetAirBlast();
        isDead = false;
        isSaved = false;
        isAirCoroutineRunning = false;
    }

    private IEnumerator pickupTriggered(GameObject pickup)
    {
        pickup.GetComponent<Renderer>().enabled = false;    // disable while pickup sound plays, then destroy
        yield return new WaitForSeconds(1);
        Destroy(pickup);
    }

    private IEnumerator playerIsSaved()
    {
        if (!isSavedCoroutineRunning)
        {
            GetComponent<AudioSource>().clip = savedSound;
            GetComponent<AudioSource>().Play();
            isSavedCoroutineRunning = true;

            yield return new WaitForSeconds(1);

            isSavedCoroutineRunning = false;
            GetComponent<AudioSource>().Stop();
        }

    }

    // Getters & Setters for player cover and saves
    public void setIsUncovered(bool status)
    {
        isUncovered = status;
    }
    public bool getIsUncovered()
    {
        return isUncovered;
    }
    public void setSaves(int s)
    {
        saves = s;
    }
    public int getSaves()
    {
        return saves;
    }
    public void useSave()
    {
        saves--;
    }
    public void saveFromDeath()
    {
        isSaved = true;
    }
    public bool isSavedFromDeath()
    {
        return isSaved;
    }
    public bool getIsJumping()
    {
        return isJumping;
    }
}
