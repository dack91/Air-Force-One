using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour {

    public static GameManager GM;
    public PlayerMovement Player;
    private int savesLeft = 3;
    public CanvasBehavior CV;
    public int maxLevels;
    private bool isGameOver;

    private void Awake()
    {
        if (GM == null)
        {
            GM = this;
        }
        else if (GM != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject); // Persist between levels
    }

    // Use this for initialization
    void Start () {
        Player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        Player.setSaves(savesLeft);
        CV = GameObject.FindGameObjectWithTag("UI").GetComponent<CanvasBehavior>();
        CV.setSaveText(savesLeft);
        isGameOver = false;
    }

    // Update is called once per frame
    void Update ()
    {
        if (!isGameOver)
        {
            // New Level Loaded
            if (Player == null)
            {
                Player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
                Player.setSaves(savesLeft);
            }
            if (CV == null)
            {
                CV = GameObject.FindGameObjectWithTag("UI").GetComponent<CanvasBehavior>();
                CV.setSaveText(savesLeft);
            }
        }
        else
        {
            GameObject[] stars = GameObject.FindGameObjectsWithTag("Stars");

            // Check length in case update runs before GameOver has loaded and array size is 0
            if (stars.Length > 0)
            {
                // Black out textures for unearned stars
                for (int i = stars.Length - 1; i >= savesLeft; i--)
                {
                    stars[i].GetComponent<Renderer>().material.color = (new Color(0, 0, 0));
                }
            }

            // Restart game
            if (Input.GetKeyDown(KeyCode.R)) {
                SceneManager.LoadScene("Level1");
                isGameOver = false;
                savesLeft = 3;
            }
        }
	}

    private void FixedUpdate()
    {
        if (!isGameOver)
        {
            // Get input for player movement
            float xMove = 0;
            if ((xMove = Input.GetAxis("Horizontal")) != 0)
            {
                Player.movePlayer(xMove);
            }
            if (Input.GetKeyDown(KeyCode.Space) && !Player.getIsJumping())
            {
                Player.jump();
            }

            // Help player death save text
            if (Player != null && CV != null &&
                Player.isAirWarning && 
                Player.getIsUncovered() &&
                Player.getSaves() > 0)
            {
                CV.showSaveHelpText();
            }
            else if (CV != null)
            {
                CV.hideSaveHelpText();
            }

            // Death Save
            if (Input.GetKeyDown(KeyCode.Tab) &&
                Player.isAirWarning &&
                Player.getIsUncovered() &&
                Player.getSaves() > 0 &&
                !Player.isSavedFromDeath())
            {
                Player.saveFromDeath();
                Player.useSave();
                CV.setSaveText(Player.getSaves());
            }
        }
    }

    public void setSaves(int i)
    {
        savesLeft = i;
        CV.setSaveText(savesLeft);
    }

    public void loadNextLevel()
    {

        // Find current level and increment for next level
        string nextLevel = SceneManager.GetActiveScene().name;
        char currLevel = nextLevel[5];
        int lev = (int)char.GetNumericValue(currLevel) + 1;

        // If player defeats last level, load game over scene
        if (lev > maxLevels) {
            nextLevel = "GameOver";
            isGameOver = true;

            // Destroy in game UI when moving to Game Over screen
            GameObject canv = GameObject.FindGameObjectWithTag("UI");
            if (canv != null)
            {
                Destroy(canv);
            }
        }
        else
        {
            // Build string for next level
            nextLevel = "Level" + lev.ToString();
        }

        SceneManager.LoadScene(nextLevel);
    }

}
