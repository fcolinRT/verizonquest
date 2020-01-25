using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public Sprite spritePlay;
    public Sprite spritePause;
    public Sprite spriteFfwd;
    public Sprite spriteBcwd;

    public Image currentStateImage;

    private void Awake()
    {
        instance = this;
    }

    public void ChangeStatePlay()
    {
        currentStateImage.sprite = spritePlay;
    }

    public void ChangeStatePause()
    {
        currentStateImage.sprite = spritePause;
    }

    public void ChangeStateFfwd()
    {
        currentStateImage.sprite = spriteFfwd;
    }

    public void ChangeStateBcwd()
    {
        currentStateImage.sprite = spriteBcwd;
    }

    public Text timeData;
    public Text gpsData;
    public Text activeNodes;


}
