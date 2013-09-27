using UnityEngine;
using System.Collections;
using GameBaseData;

public class PlayerAnimationControl : MonoBehaviour {
	
	// wiil be true when animation finish
	public bool attack1Finish = false;
	public bool attack2Finish = false;
	public bool attack3Finish = false;
	
	// 1  means   animation stop normal
	// 2 	means   animation stop because the next attack start
	public byte attack1AnimationFinish = 0;   
	public byte attack2AnimationFinish = 0;
	public byte attack3AnimationFinish = 0;
	
	// player clips
	AnimationState mRun;
	AnimationState mWalk;
	AnimationState mIdel;
	AnimationState mAttack1;
	AnimationState mAttack2;
	AnimationState mAttack3;
	AnimationState mBeHit;
	AnimationState mDie;
	AnimationState mSkill;
	AnimationState mRush;
	AnimationState mCatch;
	AnimationState mJump;
	
	private PlayerMoveBase moveScript;
	
	// Use this for initialization
	void Start () {
		// initalize the animationstate
		InitalizeAnimationClips();
		
		//get the move script
		moveScript = (PlayerMoveBase)this.transform.parent.GetComponent<PlayerMoveBase>();
	}

	
	void InitalizeAnimationClips()
	{
		mRun = this.animation["pao"];
		mRun.layer = 0;
		
		mWalk = this.animation["zou"];
		mWalk.layer = 0;
		
		mIdel = this.animation["gongjidaiji"];
		mIdel.layer = 0;
		
		mRush = this.animation["chongci"];
		mRush.layer = 1;
		
		mCatch = this.animation["zhuaju"];
		mCatch.layer = 1;
		
		mJump = this.animation["shanbi"];
		mJump.layer = 1;
		
		mAttack1 = this.animation["gongjiEdit"];
		mAttack1.layer = 3;
		
		mAttack2 = this.animation["gongji2Edit"];
		mAttack2.layer = 3;
		
		mAttack3 = this.animation["gongji3Edit"];
		mAttack3.layer = 3;
		
		mBeHit = this.animation["beiji"];
		mBeHit.layer = 4;
		
		mDie = this.animation["siwang"];
		mDie.layer = 10;
	}
	
	
	
	// Update is called once per frame
	void Update () {
		
	
	}
	
	// if immedilate == true,  play the animation right now,
	// else play it in queue
	void PlayAnimation(AnimationState state, bool immedilate)
	{
		if(immedilate)
		{
			this.animation.Stop();
			this.animation.CrossFade(state.name);
		}
		else
		{
			this.animation.CrossFadeQueued(state.name);
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
			PlayAnimation(mRun,immedilate);
			ChangeMovementStateByAnimation(PlayerMoveBase.PlayerMovementState.Run);
			PlayerDataClass.PlayerNextActionReset();
			break;	
			
		case PlayerDataClass.PlayerActionCommand.Player_Attack1:
			ChangeMovementStateByAnimation(PlayerMoveBase.PlayerMovementState.Attack1);
			PlayAnimation(mAttack1,immedilate);
			PlayerDataClass.PlayerNextActionReset();
			break;
			
		case PlayerDataClass.PlayerActionCommand.Player_Attack2:
			ChangeMovementStateByAnimation(PlayerMoveBase.PlayerMovementState.Attack2);
			PlayAnimation(mAttack2,immedilate);
			PlayerDataClass.PlayerNextActionReset();
			break;
			
		case PlayerDataClass.PlayerActionCommand.Player_Attack3:
			ChangeMovementStateByAnimation(PlayerMoveBase.PlayerMovementState.Attack3);
			PlayAnimation(mAttack3,immedilate);
			PlayerDataClass.PlayerNextActionReset();
			break;
			
		case PlayerDataClass.PlayerActionCommand.Player_BeHit:
			ChangeMovementStateByAnimation(PlayerMoveBase.PlayerMovementState.BeHit);
			PlayAnimation(mBeHit,immedilate);
			PlayerDataClass.PlayerNextActionReset();
			break;
			
		case PlayerDataClass.PlayerActionCommand.Player_Die:
			ChangeMovementStateByAnimation(PlayerMoveBase.PlayerMovementState.Idel);
			PlayAnimation(mDie,immedilate);
			PlayerDataClass.PlayerNextActionReset();
			break;
			
		case PlayerDataClass.PlayerActionCommand.Player_Idel:
			ChangeMovementStateByAnimation(PlayerMoveBase.PlayerMovementState.Idel);
			PlayAnimation(mIdel,immedilate);
			PlayerDataClass.PlayerNextActionReset();
			break;
			
		case PlayerDataClass.PlayerActionCommand.Player_Walk:
			ChangeMovementStateByAnimation(PlayerMoveBase.PlayerMovementState.Walk);
			PlayAnimation(mWalk,immedilate);
			PlayerDataClass.PlayerNextActionReset();
			break;		
		
		case PlayerDataClass.PlayerActionCommand.Player_Jump:
			ChangeMovementStateByAnimation(PlayerMoveBase.PlayerMovementState.Jump);
			PlayAnimation(mJump,immedilate);
			PlayerDataClass.PlayerNextActionReset();
			break;
		
		case PlayerDataClass.PlayerActionCommand.Player_Rush:
			ChangeMovementStateByAnimation(PlayerMoveBase.PlayerMovementState.Rush);
			PlayAnimation(mRush,immedilate);
			PlayerDataClass.PlayerNextActionReset();
			break;
		
		case PlayerDataClass.PlayerActionCommand.Player_Catch:
			ChangeMovementStateByAnimation(PlayerMoveBase.PlayerMovementState.Catch);
			PlayAnimation(mCatch,immedilate);
			PlayerDataClass.PlayerNextActionReset();
			break;
			
			
		default:
			break;
			
		}
	}
	
	// change the movementState by animation clip
	void ChangeMovementStateByAnimation(PlayerMoveBase.PlayerMovementState mState)
	{
//		Debug.Log("ChangeMovementStateByAnimation ========    "+mState);
		moveScript.SetMovementState(mState);
	}
	
		
	// AnimationEvent
	// this function will be called when attack part is finish
	// attackId  is  1 , 2,  3
	//todo
	void AttackFinish(int attackId)
	{
		switch(attackId)
		{
		case 1:
			attack1Finish = true;
			break;
			
		case 2:
			attack2Finish = true;
			break;
			
		case 3:
			attack3Finish = true;
			break;
		}
	}
	
	
	void AttackAnimationFinish(int attackId)
	{
		switch(attackId)
		{
		case 1:
			attack1AnimationFinish = 1;
			break;
			
		case 2:
			attack2AnimationFinish = 1;
			break;
			
		case 3:
			attack3AnimationFinish = 1;
			break;
		}
	}
	
	
	void StartToAttack(int attackId)
	{
		switch(attackId)
		{
		case 1:
			break;
			
		case 2:
			break;
			
		case 3:
			break;
		}
	}
	
}
