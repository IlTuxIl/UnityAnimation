using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]

public class IKFoot : MonoBehaviour {
	Animator cController;
	public Vector3 leftPos;
	public Vector3 rightPos;

	public Quaternion leftNorm;
	public Quaternion RightNorm;

	public Vector3 offset = new Vector3(0.5f,0f,0f);
 	// Use this for initialization
	void Start () {
		cController = GetComponent<Animator> ();
	}

	void Update(){
		RaycastHit hitLeft;
		RaycastHit hitRight;

		if (Physics.Raycast (transform.position - offset, -Vector3.up, out hitLeft)) {
			leftPos = hitLeft.point;
			leftNorm = hitLeft.transform.rotation;
		}
		
		if (Physics.Raycast (transform.position + offset, -Vector3.up, out hitRight)) {
			rightPos = hitRight.point;
			RightNorm = hitRight.transform.rotation;
		}
	}

	// Update is called once per frame
	void OnAnimatorIK () {
		cController.SetIKPosition (AvatarIKGoal.LeftFoot, leftPos);
		cController.SetIKPosition (AvatarIKGoal.RightFoot, rightPos);

		cController.SetIKPositionWeight (AvatarIKGoal.LeftFoot, 1);
		cController.SetIKPositionWeight (AvatarIKGoal.RightFoot, 1);

		cController.SetIKRotation (AvatarIKGoal.LeftFoot, leftNorm);
		cController.SetIKRotation (AvatarIKGoal.RightFoot, RightNorm);

		cController.SetIKRotationWeight (AvatarIKGoal.LeftFoot, 1);
		cController.SetIKRotationWeight (AvatarIKGoal.RightFoot, 1);
	}
}
