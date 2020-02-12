using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SynchronizerData;

public class BeatBehavior : MonoBehaviour
{
    private BeatObserver observador;

    private void Start()
    {
        observador = GetComponent<BeatObserver>();
    }

    private void Update()
    {
        if ((observador.beatMask & BeatType.UpBeat) == BeatType.UpBeat)
        {
            Debug.Log("Beat");
        }
    }

}
