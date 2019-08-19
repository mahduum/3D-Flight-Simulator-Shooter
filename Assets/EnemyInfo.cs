using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInfo : MonoBehaviour {

	[SerializeField] int health = 100;

	public enum Rank { 
		Leader, 
		LeftWing,
		RightWing,
		Destroyed
	}

	public Rank rank;

	public bool attackModeOn = false;

	FormationController formationController;

	//public Vector3 targetOffset;

	//public delegate void EnemyEliminationHandler (EnemyInfo enemy);
	//EnemyEliminationHandler enemyDownHandler;
	System.Action<EnemyInfo> enemyDownHandler;

	void Start() {

		formationController = GameObject.FindObjectOfType<FormationController> ();
		//enemyDownHandler = enemyFormation.OnEnemyDown;
		enemyDownHandler = formationController.OnEnemyDown;//replaces standard delegate




		//if (rank == Rank.Leader) {
			
			Debug.Log ("I am the leader");

			
		//}
//		if (rank != Rank.Leader) {
//			//add an offset to the target or make them imitate leader
//		}
	}

	void Update(){
		if (health == 0 && rank != Rank.Destroyed) {
			enemyDownHandler (this);
			rank = Rank.Destroyed;
		}

//		if (rank == Rank.Leader) {
//			formationController.leaderRotation = transform.rotation;
//		}
//
//		if (rank != Rank.Leader) {
//			transform.rotation = formationController.leaderRotation;
//		}

		//TODO target the slot position and when its in place target the player with offset?
	}



	//subscribe to the event when the leader is destroyed, the method that creates a new array of transforms around newly appointed leader
	//TODO start with the above from formation controller

}
