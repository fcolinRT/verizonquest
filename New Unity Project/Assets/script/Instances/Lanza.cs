using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lanza : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "inorganico")
        {
            Destroy(other.gameObject);
            Score.instanceScore.SumaBasura();
        }

        if (other.gameObject.tag == "pez")
        {
            Destroy(other.gameObject);
            Score.instanceScore.RestaBasura();
        }
    }
}
