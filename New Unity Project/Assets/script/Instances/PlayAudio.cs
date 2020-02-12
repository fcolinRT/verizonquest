using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAudio : MonoBehaviour
{
    public AudioSource fuente;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            fuente.Play();
        }
    }
}
