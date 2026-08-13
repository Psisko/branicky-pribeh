using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GOAnimation : MonoBehaviour
{
    public Animator animator;

    // This function will be called by the Animation Event, when it needs to display the GO sign
    public void StartAnimation()
    {
        animator.SetTrigger("start");
    }
}
