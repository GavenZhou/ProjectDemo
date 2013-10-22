using UnityEngine;
using System.Collections.Generic;
using GameBaseData;

public class PlayerAnimationControl : MonoBehaviour {
	
	public enum PlayerAnimationState
	{
		Idel,
		Run,
		Walk,
		Attack1,
		Attack2,
		Attack3,
		Attack4,
		Jump,
		Rush,
		Catch,
		Die,
		Behit,
	}
	
	public PlayerAnimationState mAnimationState = PlayerAnimationState.Idel;
	
	bool isQueueAnimation = false;
	
	public bool attackCheckPoint = false;
	public byte attackCheckId = 0;
	public byte attackIdNow = 0;
	
	public bool isSkillPlaying = false;
/*	
	// wiil be true when animation finish
	public bool attack1Finish = false;
	public bool attack2Finish = false;
	public bool attack3Finish = false;
	public bool attack4Finish = false;
		
	
	// 1  means   animation stop normal
	// 2 	means   animation stop because the next attack start
	public byte attack1AnimationFinish = 0;   
	public byte attack2AnimationFinish = 0;
	public byte attack3AnimationFinish = 0;
	public byte attack4AnimationFinish = 0;
	 */
	
	// player clips
	AnimationState mRun;
	AnimationState mWalk;
	AnimationState mIdel;
	AnimationState mSkillIdel;
	AnimationState mAttack1;
	AnimationState mAttack2;
	AnimationState mAttack3;
	AnimationState mAttack4;
	AnimationState mSkill1;
	AnimationState mBeHit;
	AnimationState mDie;
	AnimationState mRush;
	AnimationState mCatch;
	AnimationState mJump;
	
	private PlayerMoveBase moveScript;
    private PlayerMainLogic main;
	
	// Use this for initialization
	void Start () {
		// initalize the animationstate
		InitalizeAnimationClips();
		
		//get the move script
		moveScript = (PlayerMoveBase)this.transform.parent.GetComponent<PlayerMoveBase>();
        main = transform.root.GetComponent<PlayerMainLogic>();
	}

	
	void InitalizeAnimationClips()
	{
		mRun = this.animation["1_011"];
		mRun.layer = 0;
		
		mWalk = this.animation["1_021"];
		mWalk.layer = 0;
		
		mIdel = this.animation["1_001"];
		mIdel.layer = 0;
		
	//	mRush = this.animation["1_001"];
	//	mRush.layer = 1;
		
	//	mCatch = this.animation["1_001"];
	//	mCatch.layer = 1;
		
		mJump = this.animation["1_031"];
		mJump.layer = 4;
		
		mAttack1 = this.animation["1_101"];
		mAttack1.layer = 3;
		
		mAttack2 = this.animation["1_102"];
		mAttack2.layer = 3;
		
		mAttack3 = this.animation["1_103"];
		mAttack3.layer = 3;
		
		mAttack4 = this.animation["1_104"];
		mAttack4.layer = 3;
		
//		mSkillIdel = this.animation["1_001"];
//		mSkillIdel.layer = 4;
		
		mSkill1 = this.animation["1_1101"];
		mSkill1.layer = 6;
		
		mBeHit = this.animation["1_501"];
		mBeHit.layer = 5;
		
//		mDie = this.animation["1_001"];
//		mDie.layer = 10;
	}
	
	
	
	// Update is called once per frame
	void Update () {
		
	
	}
	
	void UpdateAnimationState(string clipName)
	{
		switch(clipName)
		{
		case "1_011":
			mAnimationState = PlayerAnimationState.Run;
			break;
			
		case "1_021":
			mAnimationState = PlayerAnimationState.Walk;
			break;
			
		case "1_001":
			mAnimationState = PlayerAnimationState.Idel;
			break;
			
		case "1_031":
			mAnimationState = PlayerAnimationState.Jump;
			break;
			
		case "1_101":
			mAnimationState = PlayerAnimationState.Attack1;
			break;
			
		case "1_102":
			mAnimationState = PlayerAnimationState.Attack2;
			break;
			
		case "1_103":
			mAnimationState = PlayerAnimationState.Attack3;
			break;
			
		case "1_104":
			mAnimationState = PlayerAnimationState.Attack4;
			break;
			
		case "1_501":
			mAnimationState = PlayerAnimationState.Behit;
			break;
		}
		
	}
	
	void PlayAnimation(AnimationState state, bool immedilate)
	{
		if(immedilate)
		{
//			Debug.Log(state.name);
			//this.animation.Stop();
			this.animation.Play(state.name);
			UpdateAnimationState(state.name);
			isQueueAnimation = false;
		}
		else
		{
			isQueueAnimation = true;
			this.animation.PlayQueued(state.name);
		}
	}
	
	// if immedilate == true,  play the animation right now,
	// else play it in queue
	void PlayAnimationCrossFade(AnimationState state, bool immedilate)
	{
		if(immedilate)
		{
//			Debug.Log(state.name);
			//this.animation.Stop();
			this.animation.CrossFade(state.name,0.2f);
		//	this.animation.Play(state.name);
			UpdateAnimationState(state.name);
			
			isQueueAnimation = false;
		}
		else
		{
			isQueueAnimation = true;
//			Debug.Log(state.name);
			this.animation.CrossFadeQueued(state.name);
		}
	}
	
	public void SetAttackAnimationTime(byte attackId, float time)
	{
		switch(attackId)
		{
		case 1:
			mAttack1.time = time;
			break;
		case 2:
			mAttack2.time = time;
			break;
		case 3:
			mAttack3.time = time;
			break;
		case 4:
			mAttack4.time = time;
			break;
		}
	}
	
	// only call by other script
	// get the cmd from "PlayerBaseControl", then play the animtion
	public void UpdateCmdFromControl(bool immedilate)
	{
//		Debug.Log("UpdateCmdFromControl ======   "+PlayerDataClass.playerAniCmdNext);
		switch(PlayerDataClass.playerAniCmdNext)
		{
		case PlayerDataClass.PlayerActionCommand.Player_Run:
			PlayAnimationCrossFade(mRun,immedilate);
			if(immedilate
				||moveScript.curMovementState == PlayerMoveBase.PlayerMovementState.Run)
				ChangeMovementStateByAnimation(PlayerMoveBase.PlayerMovementState.Run);
			PlayerDataClass.PlayerNextActionReset();
			break;		
		
		case PlayerDataClass.PlayerActionCommand.Player_Trot:
			PlayAnimationCrossFade(mRun,immedilate);
			if(immedilate)
				ChangeMovementStateByAnimation(PlayerMoveBase.PlayerMovementState.Trot);
			PlayerDataClass.PlayerNextActionReset();
			break;	
			
		case PlayerDataClass.PlayerActionCommand.Player_Attack1:
			if(immedilate)
				ChangeMovementStateByAnimation(PlayerMoveBase.PlayerMovementState.Attack1);
			PlayAnimationCrossFade(mAttack1,immedilate);
			PlayerDataClass.PlayerNextActionReset();
			break;
			
		case PlayerDataClass.PlayerActionCommand.Player_Attack2:
			if(immedilate)
				ChangeMovementStateByAnimation(PlayerMoveBase.PlayerMovementState.Attack2);
			PlayAnimationCrossFade(mAttack2,immedilate);
			PlayerDataClass.PlayerNextActionReset();
			break;
			
		case PlayerDataClass.PlayerActionCommand.Player_Attack3:
			
			if(immedilate)
				ChangeMovementStateByAnimation(PlayerMoveBase.PlayerMovementState.Attack3);
			PlayAnimationCrossFade(mAttack3,immedilate);
			PlayerDataClass.PlayerNextActionReset();
			break;
			
		case PlayerDataClass.PlayerActionCommand.Player_Attack4:
			if(immedilate)
				ChangeMovementStateByAnimation(PlayerMoveBase.PlayerMovementState.Attack4);
			PlayAnimationCrossFade(mAttack4,immedilate);
			PlayerDataClass.PlayerNextActionReset();
			break;

		case PlayerDataClass.PlayerActionCommand.Player_Skill1:
			if(immedilate)
				ChangeMovementStateByAnimation(PlayerMoveBase.PlayerMovementState.Skill1);
			PlayAnimation(mSkill1,immedilate);
			PlayerDataClass.PlayerNextActionReset();
			break;
			
		case PlayerDataClass.PlayerActionCommand.Player_BeHit:
			if(immedilate)
				ChangeMovementStateByAnimation(PlayerMoveBase.PlayerMovementState.BeHit);
			PlayAnimationCrossFade(mBeHit,immedilate);
			PlayerDataClass.PlayerNextActionReset();
			break;
			
		case PlayerDataClass.PlayerActionCommand.Player_Die:
			if(immedilate)
				ChangeMovementStateByAnimation(PlayerMoveBase.PlayerMovementState.Idel);
			PlayAnimationCrossFade(mDie,immedilate);
			PlayerDataClass.PlayerNextActionReset();
			break;
			
		case PlayerDataClass.PlayerActionCommand.Player_Idel:
			
			if(immedilate)
				ChangeMovementStateByAnimation(PlayerMoveBase.PlayerMovementState.Idel);
			PlayAnimationCrossFade(mIdel,immedilate);
			PlayerDataClass.PlayerNextActionReset();
			break;
			
		case PlayerDataClass.PlayerActionCommand.Plyaer_SkillIdel:
			
			if(immedilate)
				ChangeMovementStateByAnimation(PlayerMoveBase.PlayerMovementState.Idel);
			PlayAnimationCrossFade(mSkillIdel,immedilate);
			PlayerDataClass.PlayerNextActionReset();
			break;
			
		case PlayerDataClass.PlayerActionCommand.Player_Walk:
			
			if(immedilate)
				ChangeMovementStateByAnimation(PlayerMoveBase.PlayerMovementState.Walk);
			PlayAnimationCrossFade(mWalk,immedilate);
			PlayerDataClass.PlayerNextActionReset();
			break;		
		
		case PlayerDataClass.PlayerActionCommand.Player_Jump:
			
			if(immedilate)
				ChangeMovementStateByAnimation(PlayerMoveBase.PlayerMovementState.Jump);
			PlayAnimationCrossFade(mJump,immedilate);
			PlayerDataClass.PlayerNextActionReset();
			break;
		
		case PlayerDataClass.PlayerActionCommand.Player_Rush:
			
			if(immedilate)
				ChangeMovementStateByAnimation(PlayerMoveBase.PlayerMovementState.Rush);
			PlayAnimationCrossFade(mRush,immedilate);
			PlayerDataClass.PlayerNextActionReset();
			break;
		
		case PlayerDataClass.PlayerActionCommand.Player_Catch:
			
			if(immedilate)
				ChangeMovementStateByAnimation(PlayerMoveBase.PlayerMovementState.Catch);
			PlayAnimationCrossFade(mCatch,immedilate);
			PlayerDataClass.PlayerNextActionReset();
			break;
			
			
		default:
			break;
			
		}
	}
	
	// change the movementState by animation clip
	void ChangeMovementStateByAnimation(PlayerMoveBase.PlayerMovementState mState)
	{
		moveScript.SetMovementState(mState);
	}
		

	void OnAnimationStart(int attackId)
	{
	}

	void OnAttackStart(int attackId)
    {
        main.OnStartToAttack(attackId);
		switch(attackId)
		{
		case 1:
			break;
		case 2:
			break;
		case 3:
			break;
		case 4:
			break;
		}
    }

	void OnAttackFinish(int attackId)
    {
        main.OnAttackAnimationFinish(attackId);
	//	Debug.Log("OnAttackFinish == "+ attackId);
		if(attackId == 4)
		{
			attackCheckId = 0;
			return;
		}
		if(PlayerDataClass.targetAttackPos == Vector3.zero)
			attackCheckPoint = true;
		attackCheckId = (byte)attackId;
    }

    void OnAnimationFinish(int attackId)
    {
        main.OnAttackFinish(attackId);

//		Debug.Log("OnAnimationFinish == "+ attackId);
		switch(attackId)
		{
		case 1:
			break;
			
		case 2:
			break;
			
		case 3:
			break;
			
		case 4:
			break;
		}
    }
	
	void StartSkill(int skillId)
	{
		main.OnStartSkill(skillId);
	}
	
	void FinishSkill(int skillId)
	{
		isSkillPlaying = false;
	}
	
	void SkillShootPoint(int skillId)
	{
        // create the skill area here	
        main.OnSkillShootPoint(skillId);
	}
	
 	void ChangeAnimationState(int actionId)
	{
		switch(actionId)
		{
		case 11: //run  011 
//			Debug.Log(mAnimationState+"   "+isQueueAnimation);
			if(mAnimationState == PlayerAnimationState.Attack1
				||mAnimationState == PlayerAnimationState.Attack2
				||mAnimationState == PlayerAnimationState.Attack3
				||mAnimationState == PlayerAnimationState.Attack4)
			{
				if(isQueueAnimation)
				{
			//		Debug.Log("queue run");
					isQueueAnimation = false;
					mAnimationState = PlayerAnimationState.Run;
					ChangeMovementStateByAnimation(PlayerMoveBase.PlayerMovementState.Run);
				}
			}
			break;
		}
	}

	
}
