using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;

public class ControllerManager : MonoBehaviour
{
    private EntityScript[] currentEntities;
    public bool pausedScene;

    public bool repeatScene = false;
    public PlayableDirector timelineDirctor;
    public Animator animatorCrash;
    
    public Image spriteController;
    public Image ffwdImage;
    public Image bcwdImage;
    public Sprite spritePause;
    public Sprite spritePlay;
    public Sprite spriteFfwd;
    public Sprite spriteBckd;

    private KeyCode[] keyCodes = {
         KeyCode.Alpha1,
         KeyCode.Alpha2,
         KeyCode.Alpha3,
         KeyCode.Alpha4,
         KeyCode.Alpha5,
         KeyCode.Alpha6,
         KeyCode.Alpha7,
         KeyCode.Alpha8,
         KeyCode.Alpha9,
     };

    void Update ()
    {
        if (repeatScene)
        {
            
            

            if (Input.GetKeyDown(KeyCode.JoystickButton0) ||
                Input.GetKeyDown(KeyCode.JoystickButton1) ||
                Input.GetKeyDown(KeyCode.JoystickButton2) ||
                Input.GetKeyDown(KeyCode.JoystickButton3) ||
                Input.GetKeyDown(KeyCode.Escape)) 
            {
                timelineDirctor.Play();
                animatorCrash.Play("Begin");
            }

                return;
        }

        switch (ObjectPositioner.instance.statePlayback)
        {
            case ObjectPositioner.EnumStatePlayback.realTime:
                if (pausedScene)
                {
                    if (Input.GetKeyDown(KeyCode.JoystickButton0) || Input.GetKeyDown(KeyCode.DownArrow))
                    {
                        ObjectPositioner.instance.statePlayback = ObjectPositioner.EnumStatePlayback.pause;
                        currentEntities = GameObject.FindObjectsOfType<EntityScript>();

                        spriteController.sprite = spritePlay;

                        ObjectPositioner.speed = 1;

                        for (int i = 0; i < currentEntities.Length; i++)
                            currentEntities[i].ChangeOnPause();

                        UIManager.instance.ChangeStatePause();
                    }
                }
                else
                {
                    if (Input.GetKeyDown(KeyCode.JoystickButton0) || Input.GetKeyDown(KeyCode.RightArrow))
                    {
                        /*
                        ObjectPositioner.instance.statePlayback = ObjectPositioner.EnumStatePlayback.ffwd;
                        currentEntities = GameObject.FindObjectsOfType<EntityScript>();

                        ffwdImage.sprite = spritePlay;

                        ObjectPositioner.speed = 2;

                        for (int i = 0; i < currentEntities.Length; i++)
                            currentEntities[i].ChangeOnForward();
                        */
                        ObjectPositioner.instance.statePlayback = ObjectPositioner.EnumStatePlayback.ffwd;
                        ObjectPositioner.instance.ChangeFastForward();

                        UIManager.instance.ChangeStateFfwd();
                    }

                    if (Input.GetKeyDown(KeyCode.JoystickButton1) || Input.GetKeyDown(KeyCode.LeftArrow))
                    {
                        /*
                        ObjectPositioner.instance.statePlayback = ObjectPositioner.EnumStatePlayback.bckwd;
                        currentEntities = GameObject.FindObjectsOfType<EntityScript>();

                        bcwdImage.sprite = spritePlay;

                        ObjectPositioner.speed = 2;

                        for (int i = 0; i < currentEntities.Length; i++)
                            currentEntities[i].ChangeOnBackward();
                        */
                        ObjectPositioner.instance.statePlayback = ObjectPositioner.EnumStatePlayback.bckwd;
                        ObjectPositioner.instance.ChangeFastBackward();

                        UIManager.instance.ChangeStateBcwd();
                    }
                }
               
                
            break;

            case ObjectPositioner.EnumStatePlayback.pause:
                if (Input.GetKeyDown(KeyCode.JoystickButton0) || Input.GetKeyDown(KeyCode.UpArrow))
                {
                    ObjectPositioner.instance.statePlayback = ObjectPositioner.EnumStatePlayback.realTime;
                    currentEntities = GameObject.FindObjectsOfType<EntityScript>();

                    spriteController.sprite = spritePause;

                    ObjectPositioner.speed = 1;

                    for (int i = 0; i < currentEntities.Length; i++)
                        currentEntities[i].ChangeOnResume();

                    UIManager.instance.ChangeStatePlay();
                }
                /*else if (Input.GetKeyDown(KeyCode.JoystickButton1) || Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    ObjectPositioner.instance.statePlayback = ObjectPositioner.EnumStatePlayback.bckwd;
                    currentEntities = GameObject.FindObjectsOfType<EntityScript>();
                    ObjectPositioner.speed = 2;
                    for (int i = 0; i < currentEntities.Length; i++)
                        currentEntities[i].ChangeOnBackward();
                }*/
            break;

            case ObjectPositioner.EnumStatePlayback.bckwd:
                if (Input.GetKeyDown(KeyCode.JoystickButton0) || Input.GetKeyDown(KeyCode.RightArrow))
                {
                    /*
                    ObjectPositioner.instance.statePlayback = ObjectPositioner.EnumStatePlayback.ffwd;
                    currentEntities = GameObject.FindObjectsOfType<EntityScript>();

                    ffwdImage.sprite = spritePlay;
                    bcwdImage.sprite = spriteBckd;

                    ObjectPositioner.speed = 2;

                    for (int i = 0; i < currentEntities.Length; i++)
                        currentEntities[i].ChangeOnForwardFromBack();
                    */
                    ObjectPositioner.instance.statePlayback = ObjectPositioner.EnumStatePlayback.ffwd;
                    ObjectPositioner.instance.ChangeFastForward();

                    UIManager.instance.ChangeStateFfwd();
                }

                if (Input.GetKeyDown(KeyCode.JoystickButton1) || Input.GetKeyDown(KeyCode.UpArrow))
                {
                    /*
                    ObjectPositioner.instance.statePlayback = ObjectPositioner.EnumStatePlayback.realTime;
                    currentEntities = GameObject.FindObjectsOfType<EntityScript>();

                    bcwdImage.sprite = spriteBckd;

                    ObjectPositioner.speed = 1;

                    for (int i = 0; i < currentEntities.Length; i++)
                        currentEntities[i].ChangeOnResumeFromBack();
                    */
                    ObjectPositioner.instance.statePlayback = ObjectPositioner.EnumStatePlayback.realTime;
                    ObjectPositioner.instance.ChangeRealtime();

                    UIManager.instance.ChangeStatePlay();
                }
            break;

            case ObjectPositioner.EnumStatePlayback.ffwd:
                if (Input.GetKeyDown(KeyCode.JoystickButton0) || Input.GetKeyDown(KeyCode.UpArrow))
                {
                    /*
                    ObjectPositioner.instance.statePlayback = ObjectPositioner.EnumStatePlayback.realTime;
                    currentEntities = GameObject.FindObjectsOfType<EntityScript>();

                    ffwdImage.sprite = spriteFfwd;

                    ObjectPositioner.speed = 1;

                    for (int i = 0; i < currentEntities.Length; i++)
                        currentEntities[i].ChangeOnResume();
                    */
                    ObjectPositioner.instance.statePlayback = ObjectPositioner.EnumStatePlayback.realTime;
                    ObjectPositioner.instance.ChangeRealtime();

                    UIManager.instance.ChangeStatePlay();
                }
                else if (Input.GetKeyDown(KeyCode.JoystickButton1) || Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    /*
                    ObjectPositioner.instance.statePlayback = ObjectPositioner.EnumStatePlayback.bckwd;
                    currentEntities = GameObject.FindObjectsOfType<EntityScript>();

                    ffwdImage.sprite = spriteFfwd;
                    bcwdImage.sprite = spritePlay;

                    ObjectPositioner.speed = 2;

                    for (int i = 0; i < currentEntities.Length; i++)
                        currentEntities[i].ChangeOnBackward();
                    */
                    ObjectPositioner.instance.statePlayback = ObjectPositioner.EnumStatePlayback.bckwd;
                    ObjectPositioner.instance.ChangeFastBackward();

                    UIManager.instance.ChangeStateBcwd();
                }
            break;

            default:
            break;
        }


        if (pausedScene)
        {
            if (Input.GetKeyDown(KeyCode.JoystickButton3) || Input.GetKeyDown(KeyCode.X))
            {
                ObjectPositioner.instance.lastIndexActivated++;
                if (ObjectPositioner.instance.lastIndexActivated > ObjectPositioner.instance.getStructureNode.vehicles.Count)
                {
                    ObjectPositioner.instance.activeNodesIndex.Clear();
                    ObjectPositioner.instance.activeNodesIndex.Add(0);
                    ObjectPositioner.instance.lastIndexActivated = 0;
                }
                else
                    ObjectPositioner.instance.activeNodesIndex.Add(ObjectPositioner.instance.lastIndexActivated);

                UIManager.instance.activeNodes.text = "Index active nodes: ";
                string nodesText = "";
                for (int k = 0; k < ObjectPositioner.instance.activeNodesIndex.Count; k++)
                {
                    nodesText = nodesText + ObjectPositioner.instance.activeNodesIndex[k].ToString() + ",";
                    
                }
                UIManager.instance.activeNodes.text = UIManager.instance.activeNodes.text + nodesText;

            }
        }

        /*
        int numberPressed = -1;
        for (int i = 0; i < keyCodes.Length; i++)
        {
            if (Input.GetKeyDown(keyCodes[i]))
            {
                numberPressed = i + 1;
            }
        }
        if (ObjectPositioner.instance.activeNodesIndex.Contains(numberPressed))
            ObjectPositioner.instance.activeNodesIndex.Remove(numberPressed);
        else
            ObjectPositioner.instance.activeNodesIndex.Add(numberPressed);*/
    }


}
