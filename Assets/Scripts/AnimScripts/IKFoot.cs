using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]

public class IKFoot : MonoBehaviour {
	Animator cController;

    public float offsetY;

    float lFootWeight;
    float rFootWeight;

	Quaternion leftRot;
	Quaternion RightRot;

    Transform LeftFoot;
    Transform RightFoot;

    Vector3 targetLFoot;
    Vector3 targetRFoot;

    public LayerMask ignore;

 	// Use this for initialization
	void Start () {
		cController = GetComponent<Animator> ();
        
        LeftFoot = cController.GetBoneTransform(HumanBodyBones.LeftFoot);
        RightFoot = cController.GetBoneTransform(HumanBodyBones.RightFoot);

        leftRot = LeftFoot.rotation;
        RightRot = RightFoot.rotation;

	}

	void Update(){

      //  LeftFoot = cController.GetBoneTransform(HumanBodyBones.LeftFoot);
      //  RightFoot = cController.GetBoneTransform(HumanBodyBones.RightFoot);

        RaycastHit hitLeft;
		RaycastHit hitRight;

        Vector3 lpos = LeftFoot.TransformPoint(Vector3.zero);
        Vector3 rpos = RightFoot.TransformPoint(Vector3.zero);

        Debug.DrawRay(lpos, -Vector3.up, new Color(1, 0, 0));
        Debug.DrawRay(rpos, -Vector3.up, new Color(0, 0, 1));

        if (Physics.Raycast (lpos, Vector3.down, out hitLeft, 1, ignore)) {
            targetLFoot = hitLeft.point;
            leftRot = Quaternion.FromToRotation(transform.up, hitLeft.normal) * transform.rotation;
		}
		
		if (Physics.Raycast (rpos, Vector3.down, out hitRight, 1, ignore)) {
            targetRFoot = hitRight.point;
			RightRot = Quaternion.FromToRotation(transform.up, hitRight.normal) * transform.rotation;
        }
	}

	// Update is called once per frame
	void OnAnimatorIK () {

        lFootWeight = cController.GetFloat("LeftFootWeight");
        rFootWeight = cController.GetFloat("RightFootWeight");

        cController.SetIKPosition (AvatarIKGoal.LeftFoot, targetLFoot + new Vector3(0,offsetY, 0));
		cController.SetIKPosition (AvatarIKGoal.RightFoot, targetRFoot + new Vector3(0, offsetY, 0));

		cController.SetIKPositionWeight (AvatarIKGoal.LeftFoot, lFootWeight);
		cController.SetIKPositionWeight (AvatarIKGoal.RightFoot, rFootWeight);

		cController.SetIKRotation (AvatarIKGoal.LeftFoot, leftRot);
		cController.SetIKRotation (AvatarIKGoal.RightFoot, RightRot);

		cController.SetIKRotationWeight (AvatarIKGoal.LeftFoot, lFootWeight);
		cController.SetIKRotationWeight (AvatarIKGoal.RightFoot, rFootWeight);
	}
}
