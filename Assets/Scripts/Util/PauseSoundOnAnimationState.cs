using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseSoundOnAnimationState : StateMachineBehaviour
{

    private AudioSource Source;


    public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        Source = animator.GetComponent<AudioSource>();
        if (Source == null) { return; }

        Source.Stop();

    }
}
