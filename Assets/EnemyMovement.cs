using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour {


	public Transform target;//TODO move it to Enemy info, it will be asigned there according to the status of the Enemy Ship, and all other targets
	//target will be referred to as enemyInfo.target
	public Vector3 offsetToTarget = Vector3.zero;

	[SerializeField] float speed = 10f;
	[SerializeField] float rotationalDamp = 0.3f;

	[SerializeField] float raycastPlacementOffset = 1.7f;
	[SerializeField] float detectionDistance = 10f;

	Vector3 rotationLast;
	Vector3 rotationDelta;
	Vector3 headingBefore;
    Vector3 newRotationalVector = Vector3.up;
    Vector3 currentRotationalVector = Vector3.up;
    Vector3 lastRotationalVector;

    bool isInLoop;

    float cruiseFlightAltitude;
	float currentTurningSpeed;
	float maxTurningSpeed = 1f;
	float turningSpeedScaler;//clamped betwee 0 and 1 regulates the aircraft's rotation along z axis
	float currentAngleZ = 0f;
	float lastAngleZ = 0f;
	float changeInAngleZ;
    float zRotationSpeed = 1.5f;
    float angleToTarget;

	Rigidbody rb;
	EnemyInfo enemyInfo;
	// Use this for initialization
	void Start() {
		rb = GetComponent <Rigidbody> ();
		enemyInfo = GetComponent <EnemyInfo> ();

        rotationLast = transform.rotation.eulerAngles;
		headingBefore = transform.forward;
	
        //rotationalVector = target.position - transform.position;
        //maxTurningSpeed = currentTurningSpeed;
        //Debug.Log("Starting speed: " + currentTurningSpeed);
    }
	// Update is called once per frame
	void FixedUpdate () {


        //Debug.DrawRay(transform.position, (target.position-transform.position), Color.blue);
        //TODO if isLeader then perform the following, otherwise follow the leader up to a certain distance before the player (set the attackEvent), when surpassed the player (dot product) then follow the leader again

        Move ();

        TurningSpeed ();

        NormalizeTurningSpeed ();

        SmoothHalfLoopOnTurn();

        //HeightControl();
        /*New method here, that calculates the distance from the ground and from the nearest obstacle, 
         * and if necessary from the target when in attack mode when it will take precedence from the nearest obstacle,
         * so it can crash against it; it will lock a ray on it if within a certain degree, the aiming ray is separate       
        */
		//Pathfinding ();//better up pathfinding according to flight amplitude
		//rotationDelta = transform.rotation.eulerAngles - rotationLast;
		//rotationLast = transform.rotation.eulerAngles;
	}

    private void LateUpdate()
    {
       
    }

    void HeightControl()
    {
        //TODO if statement (position.y < highest peak)
        Ray ray = new Ray(transform.position, Vector3.down);
        RaycastHit raycastHit;
        Debug.DrawRay(transform.position,Vector3.down * 100, Color.red);
        if (Physics.Raycast(ray, out raycastHit, cruiseFlightAltitude)){
            if (raycastHit.distance.Equals(10f))//make it minimum safety altitude variable
            {
                //pull up or turn, combine it with front ray
                //the resulting x rotation is passed in place of lookRotZFIxed.eulerAngles, the same with obstacle avoidance maybe
                //override x in quaternion look rotation, or maybe just deactivate 'target' for the moment???

                //will need to combine the front and downward ray in order to calculate x rotation

                //OBSTACLE AVOIDANCE:
                //TODO pull up until the front ray will indicate the safe distance from the nearest obstacle, slerp the angle between horizontal ray and transform.forward ray...
                //...or more simply until in danger slerp towards 0 x degree, and if still in danger slerp towards 90 degree until in danger
                //TODO make the turning left and right similar also by temporarily disabling the target: if (the distance is safe) {PointTowardsTarget} and decrease altitude only in proximity
                //among attack modes use 'reacquiringTarget' during which the enemy will have to raise the altitude.

                //PATHFINDING:
                //TODO while following player on low altitude if there is an obstacle then calculate the nearest obstacle free point in space as a temporary target, 
                //when it is reached and the player is now behind another obstacle to operation is repeated; if the distance between player and enemy is too great,
                //the 'closeFollow' mode is deactivated and 'reacquiringTarget' mode is activated
                //!!!maybe make the movable target that will move from and to the depending on the obstacles? it will have rays pointed to the enemy and the player
                //and use cross product vector ray to calculate safe position for itself from obstacles? when it can no longer maintain contact it will dissapear and activate different mode
            }
        }
    }


    void PointTowardsTarget(){
		Vector3 direction;
		if (enemyInfo.attackModeOn == false) {
			direction = target.position - transform.position + offsetToTarget;
		} else {
			direction = target.position - transform.position;
		}

		Quaternion lookRotZFixed = Quaternion.LookRotation (direction);
        //lookRotCorrected adds into calculation the rotation along Z axis that is independent and simulates aircraft inclination based on how fast is it turning
        Quaternion lookRotZCorrected = Quaternion.Euler(lookRotZFixed.eulerAngles.x, lookRotZFixed.eulerAngles.y, transform.localEulerAngles.z);

			
		transform.rotation = Quaternion.Slerp (transform.rotation, lookRotZCorrected, rotationalDamp * Time.deltaTime);
	
	}

	void Move(){
		
		transform.position += transform.forward * speed * Time.deltaTime;
	}

	void TurningSpeed(){
		
		Vector3 headingNow = transform.forward;

		float angleInRadians = Mathf.Deg2Rad * (Vector3.SignedAngle (headingNow, headingBefore, newRotationalVector));

		headingBefore = headingNow;

		currentTurningSpeed = angleInRadians / Time.fixedDeltaTime;

		//Debug.Log ("Current turning speed: " + currentTurningSpeed + ", Max turning speed: " + maxTurningSpeed);

	}


	void NormalizeTurningSpeed() {
		if (Mathf.Abs (currentTurningSpeed) > Mathf.Abs (maxTurningSpeed)) {
			maxTurningSpeed = currentTurningSpeed;
		}
		if (Mathf.Abs (maxTurningSpeed) > 1.5f) {//TODO eliminate later when the speed is controlled by AngleToTarget
			maxTurningSpeed = 1f;
		}

        turningSpeedScaler = Mathf.Abs (maxTurningSpeed);

		currentAngleZ = 90 * currentTurningSpeed/turningSpeedScaler;//TODO when the speen changes rapidly the angle is somehow reversed...

		//Debug.Log ("current angle z: " + currentAngleZ);
		changeInAngleZ = currentAngleZ - lastAngleZ;
			
		lastAngleZ = currentAngleZ;

	}



    void SmoothHalfLoopOnTurn()
    {
        var step = speed * 5f * Time.fixedDeltaTime;
        Vector3 direction = target.position - transform.position;
        //rotationalVector = Vector3.up;// (Vector3.up, new Vector3(-direction.x, 0, -direction.z), Time.deltaTime);
        //for the sharpness of turn (TODO try the same when it's bottom's up)
        angleToTarget = Vector3.Angle(transform.forward, target.forward);
        if (angleToTarget > 105)
        {
            zRotationSpeed = Mathf.Lerp(zRotationSpeed, 0.5f, Time.deltaTime);
        }
        else
        {
            zRotationSpeed = Mathf.Lerp(zRotationSpeed, 1.5f, Time.deltaTime);
        }

        if (currentRotationalVector != newRotationalVector)
        {
            currentRotationalVector = Vector3.Lerp(currentRotationalVector, newRotationalVector, 5 * Time.deltaTime);
            //currentAngleZ = -currentAngleZ;//This is for the is in loop condition, not as clean
        }
        if (transform.eulerAngles.x > 180)//half loop upwards
        {
            if (transform.eulerAngles.x <= 315 && transform.eulerAngles.x > 220 && isInLoop == false)//TODO base the value on the angle to target not the own angle
            {
                isInLoop = true;//TODO optionally when z != 0 decrease or increase the step.
                newRotationalVector = new Vector3(-direction.x, 0, -direction.z).normalized;//minus at x is optional but this way looks better

                Debug.Log("isInLoop true");
            }
            else if (transform.eulerAngles.x > 315 && isInLoop)// maybe try 315 and higher 305 to make round 45s TODO and if enemy is not in front?
            {
                newRotationalVector = Vector3.up;
                isInLoop = false;
                Debug.Log(isInLoop);
            }
        }
        else//half loop downwards TODO this has to be improved... including distance to target, to the ground ecc.
        {
            if (transform.eulerAngles.x > 45 && transform.eulerAngles.x < 90 && isInLoop == false)//TODO add maybe also a limit distance to target
            {
                isInLoop = true;
                //newRotationalVector = new Vector3(direction.x, 0, -direction.z);//too slow rotation, rotates on the side
                newRotationalVector = new Vector3(-direction.x, 0, -direction.z).normalized;
            }
            else if (transform.eulerAngles.x > 135 || transform.eulerAngles.x < 45)// maybe try 315 and higher 305 to make round 45s
            {
                newRotationalVector = Vector3.up;
                isInLoop = false;
            }
        }
       
        Quaternion lookRotation = Quaternion.LookRotation(direction, currentRotationalVector);//rotVec = lastVector towards new Vector

        float lookX = lookRotation.eulerAngles.x;
        float lookY = lookRotation.eulerAngles.y;
     
        //float lookZ = isInLoop ? lookRotation.eulerAngles.z : currentAngleZ;//Mathf.LerpAngle(lookRotation.eulerAngles.z, currentAngleZ, 100f * Time.deltaTime);//lookRotation.eulerAngles.z;//TODO (make a smoother transition) interchangable with transform.eulerAngles.z when not in a half loop
        float lookZ = currentRotationalVector != Vector3.up ? lookRotation.eulerAngles.z : currentAngleZ;

        //zRotationSpeed = 1.5f;


        Quaternion lookRotationCompounded = Quaternion.Euler(lookX, lookY, lookZ);//for external z axis input 
        Quaternion sphericalRotation = Quaternion.Slerp(transform.rotation, lookRotationCompounded, zRotationSpeed * Time.deltaTime);
        transform.rotation = sphericalRotation;//TODO MINIMIZE HOW SHARP IT CAN TURN!!!

        //transform.rotation = Quaternion.RotateTowards(transform.rotation, lookRotationCompounded, step); //TODO if I want to use it I have to set the rotational speed limit for rotation

        //lastRotationalVector = currentRotVector;
    }


    //--------here the proper script ends------------

    void Pathfinding(){
        RaycastHit hit;
        Vector3 raycastOffset = Vector3.zero;

		Vector3 left = transform.position - transform.right * raycastPlacementOffset; //transform.right means one unit
		Vector3 right = transform.position + transform.right * raycastPlacementOffset;
		Vector3 up = transform.position + transform.up * raycastPlacementOffset;
		Vector3 down = transform.position - transform.up * raycastPlacementOffset;

		Debug.DrawRay (left, transform.forward * detectionDistance, Color.red);
		Debug.DrawRay (right, transform.forward * detectionDistance, Color.red);
		Debug.DrawRay (up, transform.forward * detectionDistance, Color.red);
		Debug.DrawRay (down, transform.forward * detectionDistance, Color.red);
		//TODO below set hit.distance to initiate turning only if equal to a specified distance, but if following the player, the player takes precedence and so can make enemy hit the obstacle
		//TODO update raycast only if below certain aplitude, or if pointing player, or if inclined downwards
		if (Physics.Raycast (left, transform.forward, out hit, detectionDistance)) {
			raycastOffset += Vector3.right;
		} else if (Physics.Raycast (right, transform.forward, out hit, detectionDistance)) {
			raycastOffset -= Vector3.right;
		}

		if (Physics.Raycast (up, transform.forward, out hit, detectionDistance)) {
			raycastOffset -= Vector3.up;
		} else if (Physics.Raycast (down, transform.forward, out hit, detectionDistance)) {
			raycastOffset += Vector3.up;
		}

		if (raycastOffset != Vector3.zero) {
			transform.Rotate (raycastOffset * 100f * Time.deltaTime);
		} else {
			
			PointTowardsTarget ();
			
		}

		//Debug.Log (hit.collider.name);
	}
	float AngleToTargetY(){//TODO connect the circle distance so it better regulates turning speed
		Vector3 targetDir = target.position - transform.position;
		Vector3 forward = transform.forward;
		float turningAngleY = Vector3.SignedAngle (targetDir, forward, Vector3.up); // this is the angle between vectors that enemy ship needs to compensate to point towards player
		
		float distance = targetDir.magnitude;
		//turning distance calculated as the lenght of the opposite in a triangle:
		float distAsTriangleOpposite = 2 * distance * (Mathf.Sin ((turningAngleY/2) * Mathf.Deg2Rad)); 
		//turning distance calculated as a section of a circle, full circle is 6.28 radians * radius (here distance from target)
		float distAsCircleSection = (turningAngleY * Mathf.PI/180) * distance;
		
		float turningSpeedMultiplier = distAsCircleSection / distance;
	
		return turningAngleY;
	}

	float AngleToTargetX(){
		Vector3 targetDir = target.position - transform.position;
		Vector3 forward = transform.forward;
		float turningAngleX = Vector3.SignedAngle (targetDir, forward, Vector3.right); // this is the angle between vectors that enemy ship needs to compensate to point towards player

		float distance = targetDir.magnitude;
		//turning distance calculated as the lenght of the opposite in a triangle:
		float distAsTriangleOpposite = 2 * distance * (Mathf.Sin ((turningAngleX/2) * Mathf.Deg2Rad)); 
		//turning distance calculated as a section of a circle, full circle is 6.28 radians * radius (here distance from target)
		float distAsCircleSection = (turningAngleX * Mathf.PI/180) * distance;

		float turningSpeedMultiplier = distAsCircleSection / distance;

		return turningAngleX;
	}
}

