﻿using UnityEngine;
using System.Collections;

public class CharacterControllerLogic : MonoBehaviour {
    public float directionDampTime = 0.25f;

    private Animator anim;
    private ThirdPersonCamera gamecam;

    private float speed = 0.0f;
    private float directionSpeed = 3f;
    private float direction = 0.0f;
    private float horizontal = 0.0f;
    private float vertical = 0.0f;

    private float rotationDegreePerSecond = 120f;

    private AnimatorStateInfo stateInfo;
    private AnimatorTransitionInfo transInfo;

    private int hashLocomotionID = 0;

	void Start () {
        anim = GetComponent<Animator>();
        if (anim.layerCount > 1) {
            anim.SetLayerWeight(1, 1);
        }

        gamecam = Camera.main.GetComponent<ThirdPersonCamera>();

        hashLocomotionID = Animator.StringToHash("Base Layer.Locomotion");
	}
	
	void Update () {
        stateInfo = anim.GetCurrentAnimatorStateInfo(0);

        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        //speed = new Vector2(horizontal, vertical).sqrMagnitude;

        StickToWorldspace(transform, gamecam.transform, ref direction, ref speed);
   
        anim.SetFloat("Speed", speed);
        anim.SetFloat("Direction", horizontal, directionDampTime, Time.deltaTime);
    }

    void FixedUpdate() {
        if (IsInLocomotion() /*&& ((direction >= 0 && horizontal >= 0) || (direction < 0 && horizontal < 0))*/) {
            Vector3 rotationAmount = Vector3.Lerp(Vector3.zero, new Vector3(0f, rotationDegreePerSecond * (horizontal < 0f ? -1f : 1f), 0f), Mathf.Abs(horizontal));
            Quaternion deltaRotation = Quaternion.Euler(rotationAmount * Time.deltaTime);
            transform.rotation = (transform.rotation * deltaRotation);
        }
    }

    void StickToWorldspace(Transform root, Transform cam, ref float direction, ref float speed) {
        Vector3 rootDirection = root.forward;
        Vector3 stickDirection = new Vector3(horizontal, 0, vertical);

        speed = stickDirection.sqrMagnitude;

        Vector3 cameraDirection = cam.forward;
        cameraDirection.y = 0f;
        Quaternion referentialShift = Quaternion.FromToRotation(Vector3.forward, cameraDirection);

        Vector3 moveDirection = referentialShift * stickDirection;
        Vector3 axisSign = Vector3.Cross(moveDirection, rootDirection);

        //Debug.DrawRay(new Vector3(root.position.x, root.position.y + 2f, root.position.z), moveDirection, Color.green);
        Debug.DrawRay(new Vector3(root.position.x, root.position.y + 2f, root.position.z), axisSign, Color.red);
        //Debug.DrawRay(new Vector3(root.position.x, root.position.y + 2f, root.position.z), rootDirection, Color.magenta);
        //Debug.DrawRay(new Vector3(root.position.x, root.position.y + 2f, root.position.z), stickDirection, Color.blue);

        float angleRootToMove = Vector3.Angle(rootDirection, moveDirection) * (axisSign.y >= 0 ? -1f:1f);
        angleRootToMove /= 180f;

        direction = angleRootToMove * directionSpeed;
    }

    public bool IsInLocomotion() {
        return stateInfo.nameHash == hashLocomotionID;
    }
}
