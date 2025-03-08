using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundOnAnimationState : StateMachineBehaviour
{
    public AudioClip Sound;
    public bool ShouldLoop;
    private AudioSource Source;


    public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
    {
        Source = animator.GetComponent<AudioSource>();
        if (Source == null) { return; }

        Source.Stop();

        Source.loop = ShouldLoop;

        Source.clip = Sound;
        Source.Play();
    }

}
