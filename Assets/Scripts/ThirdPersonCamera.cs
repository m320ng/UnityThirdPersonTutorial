using UnityEngine;
using System.Collections;

public class ThirdPersonCamera : MonoBehaviour {

    public float distanceAway = 5f;
    public float distanceUp = 2.5f;
    public float smooth = 10f;
    public Transform followXForm;
	public Vector3 offset = new Vector3(0f, 1.5f, 0f);

	private Vector3 lookDir;
    private Vector3 targetPosition;
    
	void Start() {
        followXForm = GameObject.FindGameObjectWithTag("Player").transform;
	}
	
	void Update() {
	
	}

    void OnDrawGizmos() {

    }

    void LateUpdate() {
		Vector3 characterOffset = followXForm.position + offset;

		lookDir = characterOffset - transform.position;
		lookDir.y = 0;
		lookDir.Normalize();
		Debug.DrawRay(transform.position, lookDir, Color.green);

        targetPosition = followXForm.position + Vector3.up * distanceUp - followXForm.forward * distanceAway;
        //Debug.DrawRay(follow.position, Vector3.up * distanceUp, Color.red);
        //Debug.DrawRay(follow.position, -1f * follow.forward * distanceAway, Color.blue);
        Debug.DrawLine(followXForm.position, targetPosition, Color.magenta);

        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smooth);

        transform.LookAt(followXForm);
    }
}
