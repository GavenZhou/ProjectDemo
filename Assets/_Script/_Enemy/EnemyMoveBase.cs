using UnityEngine;
using System.Collections;

public class EnemyMoveBase : MonoBehaviour {
	
	public enum EnemyMovementState
	{
		None,
		Idel,
		Run,
		RunOver,
		Walk,
		WalkOver,
		Attack,
		AttackOver,
		BeHit,
		BeHitOver,
		BeBlowUp,
		BeBlowUpOver,
		GetUp,
		GetUpOver,
		Dead,
	};
	
	struct EnemyMovementStruct
	{
		public float startSpeed;
		public float acceleration; // + -
		public float leftTime;  // -1 forever
	};
	
	EnemyMovementStruct enemy_Idel;
	EnemyMovementStruct enemy_Run;
	EnemyMovementStruct enemy_Walk;
	EnemyMovementStruct enemy_Attack;
	EnemyMovementStruct enemy_BeHit;
	EnemyMovementStruct enemy_BeBlowUp;
	EnemyMovementStruct enemy_GetUp;
	
	float ySpeed = 0;
	
	
	void InitalizeEnemyMovementStruct()
	{
		enemy_Idel.startSpeed = 0;
		enemy_Idel.acceleration = 0;
		enemy_Idel.leftTime = -1;
		
		enemy_Run.startSpeed = 2;
		enemy_Run.acceleration = -3;
		enemy_Run.leftTime = -1;
		
		enemy_Walk.startSpeed = 1;
		enemy_Walk.acceleration = 0;
		enemy_Walk.leftTime = -1;
		
		enemy_Attack.startSpeed = 0;
		enemy_Attack.acceleration = -90;
		enemy_Attack.leftTime =0.6f;
		
		enemy_BeHit.startSpeed = -3;
		enemy_BeHit.acceleration = 10;
		enemy_BeHit.leftTime = 0.5f;
		
		enemy_BeBlowUp.startSpeed = -2;
		enemy_BeBlowUp.acceleration = 10;
		enemy_BeBlowUp.leftTime = 0.7f;
		
		enemy_GetUp.startSpeed = 0;
		enemy_GetUp.acceleration = 0;
		enemy_GetUp.leftTime = 0.5f;
	}
	
	
	public EnemyMovementState curMovementState = EnemyMovementState.None;
	
	
	private GameObject player;
	
	private float mSpeed;
	private float mMoveTimeLeft = 0;
	private float mTime;
	
	// Use this for initialization
	void Start () {
		InitalizeEnemyMovementStruct();
		
		player = GameObject.FindGameObjectWithTag("player");
	
	}
	
	// Update is called once per frame
	void Update () {
		
		if(curMovementState == EnemyMovementState.Dead)
			return;
		
		if(Time.time - mTime < mMoveTimeLeft || mMoveTimeLeft < 0)
		{
			Move();
			if(curMovementState == EnemyMovementState.BeBlowUp)
				Y_Move();
		}
		else
		{
			MoveStop();
		}
		Vector3 pos = player.transform.position-transform.position;
		pos.y = 0;
		//todo
		
		
		if(curMovementState == EnemyMovementState.BeBlowUp
			||curMovementState == EnemyMovementState.GetUp
			||curMovementState == EnemyMovementState.GetUpOver
			||curMovementState == EnemyMovementState.BeBlowUpOver)
			return;
		
		if(Vector3.Distance(transform.position,player.transform.position) > 1)
		{	
			if(transform.position.y != 0)
			{
				Vector3 tempPos = transform.position;
				tempPos.y = 0;
				transform.position = tempPos;
			}
//			Debug.Log(transform.position.y + " = "+pos.y);
			this.transform.forward = Vector3.RotateTowards(transform.position,pos,Time.deltaTime*200,1000);
		}
		
	}
	
	
	void Move()
	{
		//todo
		if(mSpeed != 0)
			this.transform.Translate(0,0,Time.deltaTime*Random.Range(mSpeed-1,mSpeed+1));
	}
	
	
	void Y_Move()
	{
		ySpeed -= 1.0f;
		this.transform.Translate(0,ySpeed*Time.deltaTime,0);
	}
	
	void MoveStop()
	{
		switch(curMovementState)
		{
		case EnemyMovementState.Idel:
			break;
		case EnemyMovementState.BeHit:
			curMovementState = EnemyMovementState.BeHitOver;
			break;	
		case EnemyMovementState.BeBlowUp:
			curMovementState = EnemyMovementState.BeBlowUpOver;
			break;
		case EnemyMovementState.GetUp:
			curMovementState = EnemyMovementState.GetUpOver;
			break;
		case EnemyMovementState.Attack:
			curMovementState = EnemyMovementState.AttackOver;
			break;
		case EnemyMovementState.Run:
			curMovementState = EnemyMovementState.RunOver;
			break;
		case EnemyMovementState.Walk:
			curMovementState = EnemyMovementState.WalkOver;
			break;
		}
	}
	
	
	public void UpdateMovementState()
	{
		switch(curMovementState)
		{
		case EnemyMovementState.Attack:
			mSpeed = enemy_Attack.startSpeed;
			mMoveTimeLeft = enemy_Attack.leftTime;
			mTime = Time.time;
			break;
			
		case EnemyMovementState.Idel:
			mSpeed = enemy_Idel.startSpeed;
			mMoveTimeLeft = enemy_Idel.leftTime;
			mTime = Time.time;
			break;
			
		case EnemyMovementState.BeHit:
			mSpeed = enemy_BeHit.startSpeed;
			mMoveTimeLeft = enemy_BeHit.leftTime;
			mTime = Time.time;
			break;	
		
		case EnemyMovementState.BeBlowUp:
			mSpeed = enemy_BeBlowUp.startSpeed;
			mMoveTimeLeft = enemy_BeBlowUp.leftTime;
			ySpeed = 20;
			mTime = Time.time;
			break;	
		
		case EnemyMovementState.GetUp:
			mSpeed = enemy_GetUp.startSpeed;
			mMoveTimeLeft = enemy_GetUp.leftTime;
			mTime = Time.time;
			break;
			
		case EnemyMovementState.Run:
			mSpeed = enemy_Run.startSpeed;
			mMoveTimeLeft = enemy_Run.leftTime;
			mTime = Time.time;
			break;
			
		case EnemyMovementState.Walk:
			mSpeed = enemy_Walk.startSpeed;
			mMoveTimeLeft = enemy_Walk.leftTime;
			mTime = Time.time;
			break;
		}
	}
	
}
