using UnityEngine;
using System.Collections;

public class ThirdPersonCamera : MonoBehaviour {

    public float distanceAway = 5f;
    public float distanceUp = 2.5f;
    public float smooth = 10f;
    public Transform follow;

    private Vector3 targetPosition;
    
	void Start() {
        follow = GameObject.FindGameObjectWithTag("Player").transform;
	}
	
	void Update() {
	
	}

    void OnDrawGizmos() {

    }

    void LateUpdate() {
        targetPosition = follow.position + Vector3.up * distanceUp - follow.forward * distanceAway;
        //Debug.DrawRay(follow.position, Vector3.up * distanceUp, Color.red);
        //Debug.DrawRay(follow.position, -1f * follow.forward * distanceAway, Color.blue);
        Debug.DrawLine(follow.position, targetPosition, Color.magenta);

        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smooth);

        transform.LookAt(follow);
    }
}
