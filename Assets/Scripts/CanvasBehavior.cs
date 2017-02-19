using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasBehavior : MonoBehaviour {
    public static CanvasBehavior CV;
    public Text textSaves;
    public Text textSaveHelp;

    private void Awake()
    {
        if (CV == null)
        {
            CV = this;
        }
        else if (CV != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject); // Persist between levels
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    // Update number of saves remaining UI
    public void setSaveText(int savesLeft)
    {
        textSaves.text = "SAVES: " + savesLeft.ToString();
    }
    // Hide and show text to prompt player for saves
    public void hideSaveHelpText()
    {
        textSaveHelp.enabled = false;
    }
    public void showSaveHelpText()
    {
        textSaveHelp.enabled = true;
    }
}
