using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FMODFootsteps : MonoBehaviour
{
    //adding footsteps sounds through fmod below

    [FMODUnity.EventRef]
    public string fsevent;
    bool playerismoving;
    public float fsfrequency;

    void Update()
    {
        if (Input.GetAxis("Vertical") >= 0.01f || Input.GetAxis("Horizontal") >= 0.01f || Input.GetAxis("Vertical") <= -0.01f || Input.GetAxis("Horizontal") <= -0.01f)
        {
            //Debug.Log("player is moving");
            playerismoving = true;
        }

        else if (Input.GetAxis("Vertical") == 0 || Input.GetAxis("Horizontal") == 0)
        {
            //Debug.Log("player not moving");
            playerismoving = false;
        }
    }

    void CallFootsteps()
    {
        if (playerismoving == true)
        {
            //Debug.Log("sound should be playing");
            FMODUnity.RuntimeManager.PlayOneShot(fsevent);
        }
    }

    void Start()
    {
        InvokeRepeating("CallFootsteps", 0, fsfrequency);
        
    }

    void OnDisabled()
    {
        playerismoving = false;
    }
}