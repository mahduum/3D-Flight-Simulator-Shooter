using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FormationController : MonoBehaviour {

	// standard 'V' formation, sneak attack (keeping distance from the player and then sudden strike
	//TODO consider putting target here
	public GameObject enemyPrefab;
	public int squadronSize = 0;
	public Quaternion leaderRotation;

	float[] offsetsOnX;
	float[] offsetsOnZ;
	GameObject formationSlot;
	GameObject enemyLeader;
	GameObject enemyFighter;
	EnemyMovement enemyMovement;
	EnemyInfo enemyInfo;
	EnemyInfo[] enemyInfos;
	GameObject[] formationSlots;
	Vector3 offsetToLeader;
	Vector3[] offsetsToLeader;




	//spawn dispair number of enemies for the leader to be in the center, half to the left and half to the right, maybe put them in a list or dictionary

	int leaderIndex;

	void Awake() {
		enemyInfos = GetComponentsInChildren <EnemyInfo> ();
		//EstablishHierarchy (enemyInfos);
	}

	void Start () {

		enemyLeader = Instantiate (enemyPrefab, transform.position, Quaternion.identity, transform);
		enemyInfo = enemyLeader.GetComponent <EnemyInfo> ();
		enemyInfo.rank = EnemyInfo.Rank.Leader;
		enemyMovement = enemyLeader.GetComponent <EnemyMovement> ();
		enemyMovement.target = GameObject.FindGameObjectWithTag ("Player").transform;


		CreateOffsetsArrays ();

		CreateFormationSlots ();

		CreateSquadron ();

		//for each instatiated new enemy a place in slot is assigned
		//create the rest of a squadron, put them in a list, instantiate at the right transform, and set the rotation to the leader, than set the target, for example if(isAttackMode)

	}

	void LateUpdate () {
		foreach (GameObject formationSlot in formationSlots) {
			transform.rotation = Quaternion.identity;
		}
	}


	void CreateOffsetsArrays(){
		int slots = squadronSize;
		int incrementorX = 10;
		float functionOfX = -1f;
		float valueX = (float) incrementorX * slots / -2;
		offsetsOnX = new float[slots];
		offsetsOnZ = new float[slots];
		for (int i = 0; i < slots; i++) {
			if (valueX == 0) {//TODO in another version here the position is created as well, so the function does not skip over 0,0 position
				valueX += incrementorX;
			}
			offsetsOnX[i] = valueX;
			offsetsOnZ[i] = Mathf.Abs (valueX) * functionOfX;
			valueX += incrementorX;
		}
	}

	void CreateFormationSlots() {
		formationSlots = new GameObject[squadronSize];
		offsetsToLeader = new Vector3[squadronSize];//TODO consider eliminating, maybe array is unnecessary
		
		for (int i = 0; i < squadronSize; i++) {
			offsetToLeader = new Vector3 (offsetsOnX [i], 0, offsetsOnZ [i]);
			offsetsToLeader [i] = offsetToLeader;
			formationSlot = new GameObject ("formationSlot");
			formationSlot.transform.SetParent(enemyLeader.transform);
			formationSlots [i] = formationSlot;
			formationSlots [i].transform.position = enemyLeader.transform.position + offsetsToLeader[i];
		}
	}

	void CreateSquadron() {
		for (int i = 0; i < squadronSize; i++) {
			enemyFighter = Instantiate (enemyPrefab, formationSlots [i].transform.position, Quaternion.identity, transform);
			enemyFighter.GetComponent <EnemyMovement>().target = formationSlots[i].transform;
			Debug.Log ("Creating squadron...");
			enemyInfo = enemyFighter.GetComponent <EnemyInfo> ();
			if (i < squadronSize / 2) {
				enemyInfo.rank = EnemyInfo.Rank.LeftWing;
			} else {
				enemyInfo.rank = EnemyInfo.Rank.RightWing;
			}
		}
	}

	void OnEnemyLeaderDown () {
		//subscribe to the event and implement this function, that is establish new leader
	}

	public void OnEnemyDown(EnemyInfo enemy) {
		Debug.Log ("Enemy down: " + enemy.name);
	}
}
