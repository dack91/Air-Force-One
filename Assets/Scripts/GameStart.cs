using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameStart : MonoBehaviour {

    public GameObject player;
    public GameObject light;
    public GameObject cover;

    private float airTimer;
    private float timer;
    private bool isAirCoroutineRunning;
    private bool isDead;

    public float force;
    public Texture tex1;
    public Texture tex2;

	// Use this for initialization
	void Start () {
        airTimer = 3f;
        timer = 0f;
        isAirCoroutineRunning = false;
        isDead = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene("Level1");
        }

        timer += Time.deltaTime;

        if (timer >= airTimer)
        {
           StartCoroutine(playBlast());
        }
        if (isDead)
        {
            //lerp size
            player.transform.localScale = Vector3.Lerp(player.transform.localScale, new Vector3(0.4f, 0.4f, 0.4f), 3f * Time.deltaTime);
            player.GetComponent<Renderer>().material.SetTexture("_MainTex", tex2);
        }


    }

    private void FixedUpdate()
    {
        player.GetComponent<Rigidbody>().AddForce(new Vector3(10, 0, 0));
        if (Mathf.Abs(cover.transform.position.x - player.transform.position.x) <= 0.9f)
        {
            cover.GetComponent<Renderer>().enabled = false;
        }
        else
        {
            cover.GetComponent<Renderer>().enabled = true;
        }
    }

    private IEnumerator playBlast()
    {
        if (!isAirCoroutineRunning)
        {
            isAirCoroutineRunning = true;   // on first call mark coroutine has started
            light.GetComponent<Renderer>().material.color = (new Color(1, 1, 0));   // Change to warning yellow
            GetComponents<AudioSource>()[0].Play();

            yield return new WaitForSeconds(1);

            isDead = true;

            light.GetComponent<Renderer>().material.color = (new Color(1, 0, 0));   // Change to blasting red
            
            //Apply air force away from camera
            player.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.None;
            player.GetComponent<Rigidbody>().AddForce(0, 0, 100 * 100);
            GetComponents<AudioSource>()[1].Play();

            yield return new WaitForSeconds(4);

            // Reset player to level start
            player.transform.position = new Vector3(-7.001698f, -0.98f, 0);
            player.transform.localScale = new Vector3(1f, 1f, 1f);
            player.GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 0f);
            player.GetComponent<Renderer>().material.SetTexture("_MainTex", tex1);
            GetComponents<AudioSource>()[1].Stop();
            GetComponents<AudioSource>()[0].Stop();
            light.GetComponent<Renderer>().material.color = (new Color(1, 1, 1));
            timer = 0f;
            isDead = false;
            isAirCoroutineRunning = false;        
        }
    }
}
