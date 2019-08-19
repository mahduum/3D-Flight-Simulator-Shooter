using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlightController : MonoBehaviour {

	public float angleInDegrees;
	private Quaternion facing;
	private Vector3 initialDirection = new Vector3(0,0,1);
	public float speed = 10;
	// Use this for initialization
	void Start () {
		facing = transform.rotation;
	}
	
	// Update is called once per frame
	void Update () {

		Vector3 directionRay = new Vector3 (Mathf.Sin (angleInDegrees * Mathf.PI/180), 0, Mathf.Cos (angleInDegrees * Mathf.PI/180));
		Debug.DrawRay (transform.position, directionRay * 3, Color.red);
	
		Vector3 input = new Vector3 (Input.GetAxisRaw ("Horizontal"), Input.GetAxisRaw ("Vertical"), Input.GetAxisRaw ("Propulsion"));
		Vector3 currentDirection = input.normalized;
		print (currentDirection);
		Debug.DrawRay (transform.position, initialDirection * 10, Color.black);
		Debug.DrawRay (transform.position, currentDirection * 10, Color.red);

		Vector3 velocity = currentDirection * speed;
		Vector3 moveAmount = velocity * Time.deltaTime;

		float inputAngle = Mathf.Atan2 (currentDirection.x, currentDirection.y) * 180 / Mathf.PI; //or Rad2Deg;

		transform.position += moveAmount;

		print (inputAngle);

	

		//var rotation = Quaternion.FromToRotation (initialDirection, currentDirection);
	
		//transform.localRotation = rotation;

		//transform.eulerAngles = Vector3.up * inputAngle;



	}

	void LateUpdate(){

	}
}
