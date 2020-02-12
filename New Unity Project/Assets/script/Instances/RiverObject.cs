using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RiverObject : MonoBehaviour
{
    #region PROPETIES
    [Header("Movement properties")]
    public float speed = 1;

    [Header("Sounds")]
    public AudioClip correctAudioClip;
    public AudioClip wrongAudioClip;
    private AudioSource audioSource;

    [Header("Effects")]
    public PoolManager particleManager;
    #endregion

    #region PUBLIC_METHODS
    public virtual void ProcessCollect(int indexObject)
    {
        GameObject newParticle = particleManager.AskForDisableObject();
        newParticle.SetActive(true);
        newParticle.transform.position = this.transform.position;
        this.gameObject.SetActive(false);
    }
    #endregion

    #region UNITY_METHODS
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("DeadZone"))
        {
            this.gameObject.SetActive(false);
        }
    }
    #endregion
}
