using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorMove : MonoBehaviour
{
    private Animator anim;

    private void Start()
    {
       anim = GetComponent<Animator>(); 
    }

    // Update is called once per frame
    private void OnAnimatorMove()
    {
        if (anim != null)
            return;
        transform.parent.position += anim.deltaPosition; 
    }
}
