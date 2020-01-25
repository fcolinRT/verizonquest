using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IKManager : MonoBehaviour
{
    private Animator animator;

    public Transform lookAtTarget;
    public Transform rightHandTarget;
    public Transform leftHandTarget;
    public Transform rightFootTarget;
    public Transform leftFootTarget;

    void Start ()
    {
        animator = this.GetComponent<Animator>();
	}

    private void OnAnimatorIK(int layerIndex)
    {
        if (animator)
        {
            if (lookAtTarget)
            {
                animator.SetLookAtWeight(1);
                animator.SetLookAtPosition( lookAtTarget.position);
            }

            if (rightHandTarget)
            {
                animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
                animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
                animator.SetIKPosition(AvatarIKGoal.RightHand, rightHandTarget.position);
                animator.SetIKRotation(AvatarIKGoal.RightHand, rightHandTarget.rotation);
            }

            if (leftHandTarget)
            {
                animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
                animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
                animator.SetIKPosition(AvatarIKGoal.LeftHand, leftHandTarget.position);
                animator.SetIKRotation(AvatarIKGoal.LeftHand, leftHandTarget.rotation);
            }

            if (rightFootTarget)
            {
                animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, 1);
                animator.SetIKPosition(AvatarIKGoal.RightFoot, rightFootTarget.position);
            }

            if (leftFootTarget)
            {
                animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, 1);
                animator.SetIKPosition(AvatarIKGoal.LeftFoot, leftFootTarget.position);
            }
        }
    }
}
