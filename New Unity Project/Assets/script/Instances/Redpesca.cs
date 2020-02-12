using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Redpesca : MonoBehaviour
{

    private void Update()
    {
        gameObject.transform.Translate(0, 1 * Time.deltaTime, 0);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "inorganico")
        {
            Destroy(other.gameObject);
            Score.instanceScore.RestaBasura();
            Debug.Log("recogiste inorganico");
        }

        if (other.gameObject.tag == "pez")
        {
            Destroy(other.gameObject);
            Score.instanceScore.SumaFauna();
            Debug.Log("recogiste pez");
        }
    }
}
