using UnityEngine;
using System.Collections;

public class testAni : MonoBehaviour {
	
	int curAttackId = 0;
	
	bool isGoOnAttack = false;
	
	public int goOnNum;
	
	
	AnimationState mRun;
	AnimationState mAttack1;
	AnimationState mAttack2;
	AnimationState mAttack3;
	AnimationState mAttack4;
	
	
	AnimationState mAttack11;
	AnimationState mAttack22;
	AnimationState mAttack33;
	AnimationState mAttack44;
	
	// Use this for initialization
	void Start () {
		
		mRun = this.animation["1_011"];
		mRun.layer = 0;
		
		mAttack1 = this.animation["1_101"];
		mAttack1.layer = 3;
		
		mAttack2 = this.animation["1_102"];
		mAttack2.layer = 3;
		
		mAttack3 = this.animation["1_103"];
		mAttack3.layer = 3;
		
		mAttack4 = this.animation["1_104"];
		mAttack4.layer = 3;
		
	}
	
	
	void PlayAnimation(AnimationState state, bool immedilate, float time)
	{
		if(immedilate)
		{
			state.time = time;
			this.animation.CrossFade(state.name,0.2f);
		//	state.time = 0;
		}
		else
		{
			state.time = time;
			this.animation.CrossFadeQueued(state.name);
	//		state.time = 0;
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnGUI()
	{
		if(GUI.Button(new Rect(Screen.width*0.1f,Screen.height*0.3f,Screen.width*0.1f,Screen.width*.1f),"xxx"))
		{
		//	if(curAttackId == 0)
			{
				PlayAnimation(mAttack1,true,0);
				curAttackId = 1;
				return;
			}
			/*
			else if(curAttackId == 1)
			{
				PlayAnimation(mAttack2,true);
				curAttackId = 2;
			}			
			else if(curAttackId == 2)
			{
				PlayAnimation(mAttack3,true);
				curAttackId = 3;
			}		
			else if(curAttackId == 3)
			{
				PlayAnimation(mAttack4,true);
				curAttackId = 0;
			}
			*/
		}
	}
	
	void OnAnimationStart(int attackId)
	{	
		switch(attackId)
		{
		case 1:
			Debug.Log("OnAnimationStart == 1");
			break;
		case 2:
			Debug.Log("OnAnimationStart == 2");
			break;
		case 3:
			Debug.Log("OnAnimationStart == 3");
			break;
		case 4:
			Debug.Log("OnAnimationStart == 4");
			break;
		}
	}

	void OnAttackStart(int attackId)
    {
		switch(attackId)
		{
		case 1:
			Debug.Log("OnAttackStart == 1");
			break;
		case 2:
			Debug.Log("OnAttackStart == 2");
			break;
		case 3:
			Debug.Log("OnAttackStart == 3");
			break;
		case 4:
			Debug.Log("OnAttackStart == 4");
			break;
		}
    }

	void OnAttackFinish(int attackId)
    {
		switch(attackId)
		{
		case 1:
			PlayAnimation(mAttack2,true,0.2f);
			break;
		case 2:
			PlayAnimation(mAttack3,true,0.2f);
			break;
		case 3:
			PlayAnimation(mAttack4,true,0.2f);
			break;
		case 4:
			break;
		}
    }

    void OnAnimationFinish(int attackId)
    {
		switch(attackId)
		{
		case 1:
			Debug.Log("OnAnimationFinish == 1");
			break;
		case 2:
			Debug.Log("OnAnimationFinish == 2");
			break;
		case 3:
			Debug.Log("OnAnimationFinish == 3");
			break;
		case 4:
			Debug.Log("OnAnimationFinish == 4");
			break;
		}
    }
}
