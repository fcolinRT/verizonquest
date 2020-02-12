using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    public static Score instanceScore;

    public int Puntos_por_inorganicos;
    public int Puntos_por_Peces;
    public int contador;

    public Text textoUI;

    private void Awake()
    {
        instanceScore = this;
    }

    private void Start()
    {
        textoUI.text = "score:  0" ;
    }

    public void SumaBasura()
    {
        contador += Puntos_por_inorganicos;
        textoUI.text = "score:  " + contador;
        Debug.Log("score:  " + contador);
    }

    public void SumaFauna()
    {
        contador += Puntos_por_Peces;
        textoUI.text = "score:  " + contador;
        Debug.Log("score:  " + contador);
    }

    public void RestaFauna()
    {
        contador -= Puntos_por_Peces;
        textoUI.text = "score:  " + contador;
        Debug.Log("score:  " + contador);
    }
    public void RestaBasura()
    {
        contador -= Puntos_por_inorganicos;
        textoUI.text = "score:  " + contador;
        Debug.Log("score:  " + contador);
    }
}
