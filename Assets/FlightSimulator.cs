using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlightSimulator : MonoBehaviour {

	public float flightSpeed = 30f;
	public float cameraSpray = 0.96f;

	[SerializeField] int stabilizingMargin = 45;
	[SerializeField] float zAxisStabilizingSpeed = 3f;
	[SerializeField] float xAxisStabilizingSpeed = 1f;

	private Rigidbody rb;
	// Use this for initialization
	void Start () {
		rb = GetComponent <Rigidbody> ();

	}
	
	// Update is called once per frame
	void FixedUpdate () {
//		Vector3 moveCamTo = transform.position - transform.forward * 10f + Vector3.up * 2f;
//
//		Camera.main.transform.position = Camera.main.transform.position * cameraSpray + moveCamTo * (1f - cameraSpray);
//
//		Camera.main.transform.LookAt (transform.position + transform.forward * 100f);

		//Debug.Log ("My angular velocity: " + (rb.angularVelocity * Mathf.Rad2Deg));

		transform.position += transform.forward * Time.deltaTime * flightSpeed;

		//rb.AddRelativeForce (Vector3.up * 10f);

		flightSpeed -= transform.forward.y * Time.deltaTime * 10f;

		//Flight controll:	
		transform.Rotate (Input.GetAxis ("Vertical") * 0.3f, 0, -Input.GetAxis ("Horizontal") * 0.8f); //TODO put dampners in variables

		float zSpinRate = (transform.localEulerAngles.z); 
		float xSpinRate = (transform.localEulerAngles.x);

		YrotateZstabilize (zSpinRate);
		XStabilize (xSpinRate);

		//Speed control:
		if (Input.GetAxis ("Propulsion") > 0) {
			flightSpeed += 0.1f;
		} else if (Input.GetAxis ("Propulsion") < 0) {
			flightSpeed -= 0.1f;
		}

		if (flightSpeed < 10f) {
			flightSpeed = 10f;
		} else if (flightSpeed > 60f) {
			flightSpeed = 60f;
		}
//		float terrainHeightWhereWeAre = Terrain.activeTerrain.SampleHeight (transform.position);
//		if (terrainHeightWhereWeAre > transform.position.y) {
//			transform.position = new Vector3 (transform.position.x, terrainHeightWhereWeAre, transform.position.z);
//		}
	}

	void YrotateZstabilize(float zSpinRate){
		if (zSpinRate < 90) {
			float horizonAngle = zSpinRate;
			transform.Rotate (Vector3.up * -horizonAngle * Time.deltaTime, Space.World);
			if (horizonAngle < stabilizingMargin && Input.GetAxis ("Horizontal") == 0) {
				float eveningAngle = Mathf.LerpAngle (zSpinRate, 0, zAxisStabilizingSpeed * Time.deltaTime);
				float xAngle = transform.localEulerAngles.x;
				float yAngle = transform.localEulerAngles.y;
				transform.localEulerAngles = new Vector3 (xAngle, yAngle, eveningAngle);
			}
		}
		if (zSpinRate > 270) {
			float horizonAngle = 360 - zSpinRate;
			transform.Rotate (Vector3.up * horizonAngle * Time.deltaTime, Space.World);
			if (horizonAngle < stabilizingMargin && Input.GetAxis ("Horizontal") == 0) {
				float eveningAngle = Mathf.LerpAngle (zSpinRate, 0, zAxisStabilizingSpeed * Time.deltaTime);
				float xAngle = transform.localEulerAngles.x;
				float yAngle = transform.localEulerAngles.y;
				transform.localEulerAngles = new Vector3 (xAngle, yAngle, eveningAngle);
			}
		}
		if (zSpinRate > 90 && zSpinRate < 180) {
			float horizonAngle = 180 - zSpinRate;
			transform.Rotate (Vector3.up * -horizonAngle * Time.deltaTime, Space.World);
			if (horizonAngle < stabilizingMargin && Input.GetAxis ("Horizontal") == 0) {
				float eveningAngle = Mathf.LerpAngle (zSpinRate, 180f, zAxisStabilizingSpeed * Time.deltaTime);
				float xAngle = transform.localEulerAngles.x;
				float yAngle = transform.localEulerAngles.y;
				transform.localEulerAngles = new Vector3 (xAngle, yAngle, eveningAngle);
			}
		}
		if (zSpinRate > 180 && zSpinRate < 270) {
			float horizonAngle = zSpinRate - 180;
			transform.Rotate (Vector3.up * horizonAngle * Time.deltaTime, Space.World);
			if (horizonAngle < stabilizingMargin && Input.GetAxis ("Horizontal") == 0) {
				float eveningAngle = Mathf.LerpAngle (zSpinRate, 180f, zAxisStabilizingSpeed * Time.deltaTime);
				float xAngle = transform.localEulerAngles.x;
				float yAngle = transform.localEulerAngles.y;
				transform.localEulerAngles = new Vector3 (xAngle, yAngle, eveningAngle);
			}
		}
	}

	void XStabilize(float xSpinRate){
		if (xSpinRate < 90) {
			float horizonAngle = xSpinRate;

			if (horizonAngle < stabilizingMargin && Input.GetAxis ("Vertical") == 0) {
				float eveningAngle = Mathf.LerpAngle (xSpinRate, 0, xAxisStabilizingSpeed * Time.deltaTime);
				float yAngle = transform.localEulerAngles.y;
				float zAngle = transform.localEulerAngles.z;
				transform.localEulerAngles = new Vector3 (eveningAngle, yAngle, zAngle);
			}
		}
		if (xSpinRate > 270) {
			float horizonAngle = 360 - xSpinRate;

			if (horizonAngle < stabilizingMargin && Input.GetAxis ("Vertical") == 0) {
				float eveningAngle = Mathf.LerpAngle (xSpinRate, 0, xAxisStabilizingSpeed * Time.deltaTime);
				float yAngle = transform.localEulerAngles.y;
				float zAngle = transform.localEulerAngles.z;
				transform.localEulerAngles = new Vector3 (eveningAngle, yAngle, zAngle);
			}
		}
		if (xSpinRate > 90 && xSpinRate < 180) {
			float horizonAngle = 180 - xSpinRate;

			if (horizonAngle < stabilizingMargin && Input.GetAxis ("Vertical") == 0) {
				float eveningAngle = Mathf.LerpAngle (xSpinRate, 180f, xAxisStabilizingSpeed * Time.deltaTime);
				float yAngle = transform.localEulerAngles.y;
				float zAngle = transform.localEulerAngles.z;
				transform.localEulerAngles = new Vector3 (eveningAngle, yAngle, zAngle);
			}
		}
		if (xSpinRate > 180 && xSpinRate < 270) {
			float horizonAngle = xSpinRate - 180;

			if (horizonAngle < stabilizingMargin && Input.GetAxis ("Vertical") == 0) {
				float eveningAngle = Mathf.LerpAngle (xSpinRate, 180f, xAxisStabilizingSpeed * Time.deltaTime);
				float yAngle = transform.localEulerAngles.y;
				float zAngle = transform.localEulerAngles.z;
				transform.localEulerAngles = new Vector3 (eveningAngle, yAngle, zAngle);
			}
		}
	}
}
