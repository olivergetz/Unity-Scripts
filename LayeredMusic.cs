using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

/* Script: Layered Music
 * Author: Oliver Getz Rodahl
 * Date: 08/18/2018
 * 
 * Description: This script allows the user to fade in and out music tracks dynamically based on a set of conditions,
 * eg. amount of enemies on screen, gameplay intensity, or by pressing certain keys on the keyboard. It is meant as
 * an alternate solution for projects not big enough to need middleware such as FMOD or Wwise.
 * 
 * How to use:
 * 1. Create an empty game object.
 * 2. Attach this script to the game object.
 * 3. Create 3 empty game objects as children to the previous game object.
 * 4. The name of the 3 children must match the strings layerObj1-3 on lines 37-42 of this script.
 * 5. Add an Audio Source component to each of the children, add a .wav files to each, and set the mixer 
 *    output to your music AudioMixerGroup.
 * 6. Done!
 * 
 * To Do: Add option to move faders on AudioMixerGroups directly, rather than volume on Audio Source.
 */

public class LayeredMusic : MonoBehaviour {

    //Declarations
    private AudioSource MXL1;
    private AudioSource MXL2;
    private AudioSource MXL3;

    private float normalizedTime;
    [Range(0.1f, 10.0f)] //Min. range must be 0.1f or greater.
    [SerializeField]
    private float inDuration = 5.0f;
    [Range(0.1f, 10.0f)]
    [SerializeField]
    private float outDuration = 5.0f;

    [SerializeField]
    private string layerObj1 = "MX_Layer_1"; //Initialized here to allow renaming in editor.
    [SerializeField]
    private string layerObj2 = "MX_Layer_2";
    [SerializeField]
    private string layerObj3 = "MX_Layer_3";

    //Prevents a coroutine from being triggered multiple times before finishing.
    bool isRunning;

	// Use this for initialization
	void Start () {

        isRunning = false;
		
        /*  
         * Find Relevant Audio Sources on other empty game objects.
         * Return an error if there are no relevant audio sources.
         * Copy/Paste this format if you need more layers. 
         */
        if (GameObject.Find(layerObj1) != null){
            //Reference the gameobject.
            MXL1 = GameObject.Find(layerObj1).GetComponent<AudioSource>();
            //Set initial volume.
            MXL1.volume = 0.0f;
            //Loop the track.
            MXL1.loop = true;
            //Play the track. All the tracks have to be playing from scene start, or they won't sync properly.
            MXL1.Play();
        }
        else{
            Debug.LogError("Game Object named " + layerObj1 + " is either missing or " + 
                           "does not have an Audio Source component.");
            Application.Quit();
        }

        //Do the same for the 2nd layer.
        if (GameObject.Find(layerObj2) != null)
        {
            MXL2 = GameObject.Find(layerObj2).GetComponent<AudioSource>();
            MXL2.volume = 0.0f;
            MXL2.loop = true;
            MXL2.Play();
        }
        else
        {
            Debug.LogError("Game Object named " + layerObj2 + " is either missing or " +
                           "does not have an Audio Source component.");
            Application.Quit();
        }

        //Do the same for the 3rd layer.
        if (GameObject.Find(layerObj3) != null)
        {
            MXL3 = GameObject.Find(layerObj3).GetComponent<AudioSource>();
            MXL3.volume = 0.0f;
            MXL3.loop = true;
            MXL3.Play();
        }
        else
        {
            Debug.LogError("Game Object named " + layerObj3 + " is either missing or " +
                           "does not have an Audio Source component.");
            Application.Quit();
        }


	}
	
	// Update is called once per frame
	void Update () {

        //Conditions for when to fade in/fade out audio stems/layers.
        if (isRunning == false)
        {
            if (Input.GetKeyDown("1") && MXL1.volume == 0.0f) StartCoroutine(FadeIn(MXL1, 1.0f, inDuration));
            if (Input.GetKeyDown("1") && MXL1.volume != 0.0f) StartCoroutine(FadeOut(MXL1, outDuration));
            if (Input.GetKeyDown("2") && MXL2.volume == 0.0f) StartCoroutine(FadeIn(MXL2, 1.0f, inDuration));
            if (Input.GetKeyDown("2") && MXL2.volume != 0.0f) StartCoroutine(FadeOut(MXL2, outDuration));
            if (Input.GetKeyDown("3") && MXL3.volume == 0.0f) StartCoroutine(FadeIn(MXL3, 1.0f, inDuration));
            if (Input.GetKeyDown("3") && MXL3.volume != 0.0f) StartCoroutine(FadeOut(MXL3, outDuration));
        }
		
	}

    IEnumerator FadeIn(AudioSource source, float finish, float duration) {

        isRunning = true;

        for (float t = 0.0f; t < duration; t += Time.deltaTime) {
            normalizedTime = t / duration;
            source.volume = Mathf.Lerp(source.volume, finish, normalizedTime);
            yield return null;
        }

        isRunning = false;
        StopCoroutine("FadeIn");

    }

    IEnumerator FadeOut(AudioSource source, float duration) {
        
        isRunning = true;

        for (float t = 0.0f; t < duration; t += Time.deltaTime)
        {
            normalizedTime = t / duration;
            source.volume = Mathf.Lerp(source.volume, 0.0f, normalizedTime);
            yield return null;
        }

        /* 
         * For some reason, Lerp in coroutines sometimes don't reach their target values (eg. ends at 0.001023 or 0.9999).
         * We snap volume to 0 when fading out to make sure the volume is completely off after fading out because a value 
         * of 0.0f is required to trigger trigger another coroutine. We don't have to do this when fading in, as a 
         * difference of ~0.000001 is not audible.       
         */
        source.volume = 0.0f; 

        isRunning = false;
        StopCoroutine("FadeOut");

    }

}