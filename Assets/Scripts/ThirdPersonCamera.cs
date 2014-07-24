using UnityEngine;
using System.Collections;

struct CameraPosition
{
    public Transform XForm { get; set; }
    public Vector3 Position { get; set; }
    public void Init(string camName, Vector3 pos, Transform transform, Transform parent) {
        Position = pos;
        XForm = transform;
        XForm.name = camName;
        XForm.parent = parent;
        XForm.localPosition = Vector3.zero;
        XForm.localPosition = Position;
    }
}

[RequireComponent(typeof(BarsEffect))]
public class ThirdPersonCamera : MonoBehaviour {
    public const float TARGETING_THRESHOLD = 0.01f;

    public float distanceAway = 5f;
    public float distanceUp = 2.5f;
    public float smooth = 10f;
    public Transform followXForm;
    public float widescreen = 0.2f;
    public float targetingTime = 0.5f;

    public float camSmoothDampTime = 0.1f;

    public float firstPersonThreshold = 0.5f;

    public enum CamState
    {
        Behind,
        FirstPerson,
        Target,
        Free
    }

	private Vector3 lookDir;
    private Vector3 targetPosition;
    private BarsEffect barsEffect;
    private CamState camState = CamState.Behind;

    private Vector3 velocityCamSmooth = Vector3.zero;

    private CameraPosition firstPersonPos;
    private float xAxisRot = 0.0f;
    private float lookWeight;

    private CharacterControllerLogic follow;
    
	void Start() {
        follow = GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterControllerLogic>();
        followXForm = GameObject.FindGameObjectWithTag("Player").transform;
        lookDir = followXForm.forward;

        barsEffect = GetComponent<BarsEffect>();
        if (barsEffect == null) {
            Debug.LogError("Attach a widescreen BarsEffect script to the camera", this);
        }

        firstPersonPos = new CameraPosition();
        firstPersonPos.Init(
            "",
            new Vector3(0.0f, 1.6f, 0.2f),
            new GameObject().transform,
            followXForm
            );
	}
	
	void Update() {
	
	}

    void OnDrawGizmos() {

    }

    void LateUpdate() {
        float leftX = Input.GetAxis("Horizontal");
        float leftY = Input.GetAxis("Vertical");
        float rightX = Input.GetAxis("RightStickX");
        float rightY = Input.GetAxis("RightStickY");

        Vector3 characterOffset = followXForm.position + new Vector3(0f, distanceUp, 0f);

        if (Input.GetAxis("Target") > TARGETING_THRESHOLD) {
            barsEffect.coverage = Mathf.SmoothStep(barsEffect.coverage, widescreen, targetingTime);
            camState = CamState.Target;
        } else {
            barsEffect.coverage = Mathf.SmoothStep(barsEffect.coverage, 0, targetingTime);

            if (rightY > firstPersonThreshold && !follow.IsInLocomotion()) {
                xAxisRot = 0f;
                lookWeight = 0f;
                camState = CamState.FirstPerson;
            }

            if ((camState == CamState.FirstPerson && Input.GetButton("ExitFPV")) ||
                (camState == CamState.Target && (Input.GetAxis("Target") <= TARGETING_THRESHOLD))) {
                camState = CamState.Behind;
            }
        }

        switch (camState) {
            case CamState.Behind:
		        lookDir = characterOffset - transform.position;
		        lookDir.y = 0;
		        lookDir.Normalize();
		        Debug.DrawRay(transform.position, lookDir, Color.green);

                //targetPosition = followXForm.position + Vector3.up * distanceUp - followXForm.forward * distanceAway;
                //targetPosition = characterOffset + followXForm.up * distanceUp - lookDir * distanceAway;

                //Debug.DrawRay(follow.position, Vector3.up * distanceUp, Color.red);
                //Debug.DrawRay(follow.position, -1f * follow.forward * distanceAway, Color.blue);
                //Debug.DrawLine(followXForm.position, targetPosition, Color.magenta);

                targetPosition = characterOffset + followXForm.up * distanceUp - lookDir * distanceAway;
                break;

            case CamState.Target:
                lookDir = followXForm.forward;

                targetPosition = characterOffset + followXForm.up * distanceUp - lookDir * distanceAway;
                break;

            case CamState.FirstPerson:
                lookDir = characterOffset + lookDir * distanceAway;

                targetPosition = characterOffset;
                characterOffset = lookDir;
                break;
        }

        compensateForWalls(characterOffset, ref targetPosition);

        //transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smooth);
        smoothPosition(transform.position, targetPosition);

        transform.LookAt(characterOffset);
    }

    private void smoothPosition(Vector3 fromPos, Vector3 toPos) {
        transform.position = Vector3.SmoothDamp(fromPos, toPos, ref velocityCamSmooth, camSmoothDampTime);
    }

    private void compensateForWalls(Vector3 fromObject, ref Vector3 toTarget) {
        Debug.DrawLine(fromObject, toTarget, Color.cyan);
        RaycastHit wallHit = new RaycastHit();
        if (Physics.Linecast(fromObject, toTarget, out wallHit)) {
            Debug.DrawLine(wallHit.point, Vector3.left, Color.red);
            toTarget = new Vector3(wallHit.point.x, toTarget.y, wallHit.point.z);
        }
    }
}
