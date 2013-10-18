using UnityEngine;
using System.Collections;

public class PlayerMoveBase : MonoBehaviour {
	
	public enum PlayerMovementState
	{
		None,
		Idel,
		Run,
		RunOver,
		Trot,
		TrotOver,
		Walk,
		WalkOver,
		Attack1,
		Attack1Over,
		Attack2,
		Attack2Over, 
		Attack3,
		Attack3Over,
		Attack4,
		Attack4Over,
		BeHit,
		BeHitOver,
		Jump,
		JumpOver, 
		Rush,
		RushOver,
		Catch,
		CatchOver,
	}
	
	struct PlayerMovementStruct
	{
		public float startSpeed;
		public float acceleration; // + -
		public float leftTime;  // -1 forever
	}
	
	PlayerMovementStruct player_Idel;
	PlayerMovementStruct player_Run;
	PlayerMovementStruct player_Trot;
	PlayerMovementStruct player_Walk;
	PlayerMovementStruct player_Attack1;
	PlayerMovementStruct player_Attack2;
	PlayerMovementStruct player_Attack3;
	PlayerMovementStruct player_Attack4;
	PlayerMovementStruct player_BeHit;
	PlayerMovementStruct player_Jump;
	PlayerMovementStruct player_Rush;
	PlayerMovementStruct player_Catch;
	
	public PlayerMovementState curMovementState = PlayerMovementState.None;
	
	
	public float mSpeed = 0;
	public float mMoveTimeLeft = 0;
	public float mAcceleration = 0;
	
	private float mTime;
	
	public Vector3 targetPos;
	
	Vector3 lookDirection;
	
	void InitalizePlayerMovementStruct()
	{
		player_Idel.startSpeed = 0;
		player_Idel.acceleration = 0;
		player_Idel.leftTime = -1;
		
		player_Run.startSpeed = 8;
		player_Run.acceleration = -2;
		player_Run.leftTime = 2;
		
		player_Trot.startSpeed = 8;
		player_Trot.acceleration = -3;
		player_Trot.leftTime = 0.5f;
		
		player_Walk.startSpeed = 2.5f;
		player_Walk.acceleration = -1.0f;
		player_Walk.leftTime = 1.5f;
		
		player_Attack1.startSpeed = 10;
		player_Attack1.acceleration = -30;
		player_Attack1.leftTime =0.9f;
		
		player_Attack2.startSpeed = 10;
		player_Attack2.acceleration = -30; 
		player_Attack2.leftTime = 0.9f;
		
		player_Attack3.startSpeed = 15;
		player_Attack3.acceleration = -30;
		player_Attack3.leftTime = 0.9f;
		
		player_Attack4.startSpeed = 20;
		player_Attack4.acceleration = -50;
		player_Attack4.leftTime = 1.4f;
		
		player_BeHit.startSpeed = 0;
		player_BeHit.acceleration = 0;
		player_BeHit.leftTime = 0.5f;
		
		player_Jump.startSpeed = 20;
		player_Jump.acceleration = -40;
		player_Jump.leftTime =0.4f;
		
		player_Rush.startSpeed = 40;
		player_Rush.acceleration = -90;
		player_Rush.leftTime = 0.5f;
		
		player_Catch.startSpeed = 0;
		player_Catch.acceleration = 0;
		player_Catch.leftTime = 0.3f;
	}
	
	
	// Use this for initialization
	void Start () {
		InitalizePlayerMovementStruct();
		UpdateMovementState();
	}
	
	// Update is called once per frame
	void Update () {
		
		if(Time.time - mTime < mMoveTimeLeft)
		{
			Move();
		}
		else
		{
			MoveStop();
		}
		
		if(curMovementState == PlayerMovementState.Run)
		{
			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDirection.normalized), Time.deltaTime * 10);
		}
	}
	
	
	void Move()
	{
		this.transform.Translate(0,0,Time.deltaTime*mSpeed);
		
		if(mSpeed > 0)
		{
			mSpeed += mAcceleration*Time.deltaTime;
			if(mSpeed <= 0)
			{
				mSpeed = 0;
			}
		}
		else if(mSpeed < 0)
		{
			mSpeed += mAcceleration*Time.deltaTime;
			if(mSpeed >= 0)
			{
				mSpeed = 0;
			}
		}
	}
	
	public void UpdateMovementState()
	{
//		Debug.Log("UpdateMovementState     "+curMovementState);
		
		switch(curMovementState)
		{
		case PlayerMovementState.Idel:
			mSpeed = player_Idel.startSpeed;
			mAcceleration = player_Idel.acceleration;
			mMoveTimeLeft = player_Idel.leftTime;
			mTime = Time.time;
			break;
			
		case PlayerMovementState.Run:
			lookDirection = targetPos - transform.position;
			lookDirection.y = 0;
			mSpeed = player_Run.startSpeed;
			mAcceleration = player_Run.acceleration;
			mMoveTimeLeft = player_Run.leftTime;
			mTime = Time.time;
			break;	
		
		case PlayerMovementState.Trot:
			mSpeed = player_Trot.startSpeed;
			mAcceleration = player_Trot.acceleration;
			mMoveTimeLeft = player_Trot.leftTime;
			mTime = Time.time;
			break;
			
		case PlayerMovementState.Walk:
			mSpeed = player_Walk.startSpeed;
			mAcceleration = player_Walk.acceleration;
			mMoveTimeLeft = player_Walk.leftTime;
			mTime = Time.time;
			break;
			
		case PlayerMovementState.Attack1:
			mSpeed = player_Attack1.startSpeed;
			mAcceleration = player_Attack1.acceleration;
			mMoveTimeLeft = player_Attack1.leftTime;
			mTime = Time.time;
			break;
			
		case PlayerMovementState.Attack2:
			mSpeed = player_Attack2.startSpeed;
			mAcceleration = player_Attack2.acceleration;
			mMoveTimeLeft = player_Attack2.leftTime;
			mTime = Time.time;
			break;
			
		case PlayerMovementState.Attack3:
			mSpeed = player_Attack3.startSpeed;
			mAcceleration = player_Attack3.acceleration;
			mMoveTimeLeft = player_Attack3.leftTime;
			mTime = Time.time;
			break;	
		
		case PlayerMovementState.Attack4:
			mSpeed = player_Attack4.startSpeed;
			mAcceleration = player_Attack4.acceleration;
			mMoveTimeLeft = player_Attack4.leftTime;
			mTime = Time.time;
			break;
			
		case PlayerMovementState.BeHit:
			mSpeed = player_BeHit.startSpeed;
			mAcceleration = player_BeHit.acceleration;
			mMoveTimeLeft = player_BeHit.leftTime;
			mTime = Time.time;
			break;	
		
		case PlayerMovementState.Jump:
			mSpeed = player_Jump.startSpeed;
			mAcceleration = player_Jump.acceleration;
			mMoveTimeLeft = player_Jump.leftTime;
			mTime = Time.time;
			break;
		
		case PlayerMovementState.Rush:
			mSpeed = player_Rush.startSpeed;
			mAcceleration = player_Rush.acceleration;
			mMoveTimeLeft = player_Rush.leftTime;
			mTime = Time.time;
			break;	
		
		case PlayerMovementState.Catch:
			mSpeed = player_Catch.startSpeed;
			mAcceleration = player_Catch.acceleration;
			mMoveTimeLeft = player_Catch.leftTime;
			mTime = Time.time;
			break;
			
		default:
			break;
		}
	}
	
	
	void MoveStop()
	{
		switch(curMovementState)
		{
		case PlayerMovementState.Idel:
			break;
			
		case PlayerMovementState.Run:
			curMovementState = PlayerMovementState.RunOver;
			break;	
		
		case PlayerMovementState.Trot:
			curMovementState = PlayerMovementState.TrotOver;
			break;
			
		case PlayerMovementState.Walk:
			curMovementState = PlayerMovementState.WalkOver;
			break;
			
		case PlayerMovementState.Attack1:
			curMovementState = PlayerMovementState.Attack1Over;
			break;
			
		case PlayerMovementState.Attack2:
			curMovementState = PlayerMovementState.Attack2Over;
			break;
			
		case PlayerMovementState.Attack3:
			curMovementState = PlayerMovementState.Attack3Over;
			break;	
		
		case PlayerMovementState.Attack4:
			curMovementState = PlayerMovementState.Attack4Over;
			break;
			
		case PlayerMovementState.BeHit:
			curMovementState = PlayerMovementState.BeHitOver;
			break;
			
		case PlayerMovementState.Jump:
			curMovementState = PlayerMovementState.JumpOver;
			break;
		
		case PlayerMovementState.Rush:
			curMovementState = PlayerMovementState.RushOver;
			break;	
		
		case PlayerMovementState.Catch:
			curMovementState = PlayerMovementState.CatchOver;
			break;
			
		default:
			break;
		}
	}
	
	
	public void SetMovementState(PlayerMovementState mState)
	{
		curMovementState = mState;
		UpdateMovementState();
	}
}
