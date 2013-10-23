using UnityEngine;
using System.Collections;

public class EnemyAnimationControl : MonoBehaviour {
	
	
	// player clips
	AnimationState mRun;
	AnimationState mWalk;
	AnimationState mIdel;
	AnimationState mAttack;
	AnimationState mBeHit;
	AnimationState mBlowUp;
	AnimationState mGetUp;
	AnimationState mDie;
	AnimationState mSkill;
	
	
	EnemyMainLogic mainLogicScript;
	
	EnemyMoveBase moveScript;
	
	
	void InitalizeAnimationClips()
	{   
		mRun = this.animation["011"];
		mRun.layer = 0;
		
		mWalk = this.animation["021"];
		mWalk.layer = 0;
		
		mIdel = this.animation["001"];
		mIdel.layer = 0;
		
		mAttack = this.animation["2101"];
		mAttack.layer = 1;
		
		mBeHit = this.animation["501"];
		mBeHit.layer = 2;
		
		mBlowUp = this.animation["502"];
		mBlowUp.layer = 2;
		
		mGetUp = this.animation["503"];
		mGetUp.layer = 2;
		
		mDie = this.animation["504"];
		mDie.layer = 3;
	}
	
	
	// Use this for initialization
	void Awake () {
		
		mainLogicScript = (EnemyMainLogic)this.transform.parent.GetComponent<EnemyMainLogic>();		
		moveScript = (EnemyMoveBase)this.transform.parent.GetComponent<EnemyMoveBase>();		
		InitalizeAnimationClips();
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
			return;
		}
		else
		{
			this.animation.CrossFadeQueued(state.name);
			return;
		}
	}
	
	
	void ChangeMoveState(EnemyMoveBase.EnemyMovementState state)
	{
		moveScript.curMovementState = state;
		moveScript.UpdateMovementState();
	}
	
	public void UpdateEnemyStateForAnimation(bool immedilate)
	{
		switch(mainLogicScript.mState)
		{
		case EnemyMainLogic.EnemyState.Attack:
			PlayAnimation(mAttack,immedilate);
			ChangeMoveState(EnemyMoveBase.EnemyMovementState.Attack);
			break;

		case EnemyMainLogic.EnemyState.BeHit:
			PlayAnimation(mBeHit,immedilate);
			ChangeMoveState(EnemyMoveBase.EnemyMovementState.BeHit);
			break;	
		
		case EnemyMainLogic.EnemyState.BeBlowUp:
			PlayAnimation(mBlowUp,immedilate);
			ChangeMoveState(EnemyMoveBase.EnemyMovementState.BeBlowUp);
			break;
			
		case EnemyMainLogic.EnemyState.GetUp:
			PlayAnimation(mGetUp,immedilate);
			ChangeMoveState(EnemyMoveBase.EnemyMovementState.GetUp);
			break;
			
		case EnemyMainLogic.EnemyState.Die:
			PlayAnimation(mDie,immedilate);
			ChangeMoveState(EnemyMoveBase.EnemyMovementState.Dead);
			break;
			
		case EnemyMainLogic.EnemyState.Hover:
			PlayAnimation(mIdel,immedilate);
			ChangeMoveState(EnemyMoveBase.EnemyMovementState.Idel);
			break;
			
		case EnemyMainLogic.EnemyState.Idel:
			PlayAnimation(mIdel,immedilate);
			ChangeMoveState(EnemyMoveBase.EnemyMovementState.Idel);
			break;
			
		case EnemyMainLogic.EnemyState.Patrol:
			PlayAnimation(mWalk,immedilate);
			ChangeMoveState(EnemyMoveBase.EnemyMovementState.Walk);
			break;
			
		case EnemyMainLogic.EnemyState.Run:
			PlayAnimation(mWalk,immedilate);
			ChangeMoveState(EnemyMoveBase.EnemyMovementState.Run);
			break;	
		}
	}
}
