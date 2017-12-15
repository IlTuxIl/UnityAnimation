using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;

[RequireComponent(typeof(Animator))]

public class IKFoot : MonoBehaviour {
	Animator cController;
	public Transform leftPos;
	public Transform rightPos;
	// Use this for initialization
	void Start () {
		cController = GetComponent<Animator> ();
	}
	
	// Update is called once per frame
	void OnAnimatorIK () {
		cController.SetIKPosition (AvatarIKGoal.LeftFoot, leftPos.position);
		cController.SetIKPosition (AvatarIKGoal.RightFoot, rightPos.position);

		cController.SetIKPositionWeight (AvatarIKGoal.LeftFoot, 1);
		cController.SetIKPositionWeight (AvatarIKGoal.RightFoot, 1);
	}
}
