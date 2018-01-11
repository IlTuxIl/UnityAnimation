using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]

public class IdleToRun : MonoBehaviour {
    
    Animator myAnimator;
    CharacterController cController;
    void Start()
    {
        myAnimator = GetComponent<Animator>();
        cController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {

        float tmp = 2 - Input.GetAxis("Sprint");
        
        myAnimator.SetFloat("vSpeed", Input.GetAxis("Vertical") / tmp);
        myAnimator.SetFloat("hSpeed", Input.GetAxis("Horizontal") / tmp);
        //myAnimator.SetBool("Grounded", cController.isGrounded);

    }
}
