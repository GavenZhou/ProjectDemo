using UnityEngine;
using System.Collections;
using GameBaseData;

public class PlayerMainLogic : MonoBehaviour {
	
	
	public enum TouchState
	{
		None,
		AFingerOneTap,
		AFingerDoubleTap,
		AFingerHold,
		AFingerHoldStop,
		AFingerSlash,
		TwoFingersTap,
		TwoFingersDoubleTap,
		TwoFingersSlash,
	}
	
	public TouchState mTouchState = TouchState.None;
	
	private RaycastHit[] hit;
	
	private PlayerAnimationControl aniControlScript;
	
	private PlayerMoveBase moveBaseScript;
	
    private Player player;
	
	private Transform directionIcon;
	
    public GameObject[] attackSfx;
    
    public GameObject pickupSfx;
	
	public GameObject skill1Sfx;

	public Transform mTarget;
	
	private Transform specialAttackIcon;
	
	private float mAttackCdTime;
	
	public bool mContinueAttack;
	
	// Use this for initialization
	void Start () {
		aniControlScript = (PlayerAnimationControl)this.transform.FindChild("Male001").GetComponent<PlayerAnimationControl>();
		
		moveBaseScript = (PlayerMoveBase)this.GetComponent<PlayerMoveBase>();
		
		directionIcon = this.transform.FindChild("icon");
		specialAttackIcon = this.transform.FindChild("SpecialAttackIcon");
		
        player = gameObject.AddComponent<Player>();
        player.Init(1000);
        SceneMng.instance.AddMainPlayer(player);
	}
	
	

	
	// Update is called once per frame
	void Update () {

        if(mTarget == null 
			&& (moveBaseScript.curMovementState == PlayerMoveBase.PlayerMovementState.Run
			||moveBaseScript.curMovementState == PlayerMoveBase.PlayerMovementState.Trot
			||moveBaseScript.curMovementState == PlayerMoveBase.PlayerMovementState.Walk)) 
		{
			if(PlayerDataClass.isChangeToRun == true)
            	mTarget = player.GetNearestAttackTarget(PlayerDataClass.targetAttackPos-transform.position);// transform.forward);
			else 
				mTarget = player.GetNearestAttackTarget(transform.forward);
        }	
		
		if(aniControlScript.attackCheckPoint == true)
		{
			aniControlScript.attackCheckPoint = false;
			mTarget = player.GetNearestAttackTarget(transform.forward);
			if(mTarget != null)
			{
//				Debug.Log("there...........");
				if(PlayerDataClass.isChangeToRun == false)
				{
					TargetTheEnemy(mTarget.position);
					ContinueAttack();
				}
				return;
			}
			else
			{
				mTarget = null;
				PlayerDataClass.AttackStart = false;
			}
		}
		
		if(Time.time - mAttackCdTime > 2.0f)
			mContinueAttack = false;
		
		if(mTarget != null&& PlayerDataClass.AttackStart == false)
		{
			
			if(PlayerDataClass.isChangeToRun == false)
			{
//				Debug.Log("here...........");
				//turn to the target
				TargetTheEnemy(mTarget.position);
				PlayerDataClass.AttackStart = true;
				// attack!
				AttackAfterRun();
				return;
			}
		}
		
		CheckMoveState();
	}
	
	void AttackAfterRun()
	{
		if(mContinueAttack)
		{
			switch(aniControlScript.attackIdNow)
			{
			case 1:
				mContinueAttack = false;
	//			Debug.Log("tingdun Player_Attack2");
				aniControlScript.attackIdNow = 2;
				aniControlScript.SetAttackAnimationTime(2,0);
				moveBaseScript.SetAttackMoveTime(2,0);
				mAttackCdTime = Time.time;
				ChangeAnimationByActionCmd(PlayerDataClass.PlayerActionCommand.Player_Attack2,true);
				break;
				
			case 2:
				mContinueAttack = false;
		//		Debug.Log("tingdun Player_Attack3");
				aniControlScript.attackIdNow = 3;
				aniControlScript.SetAttackAnimationTime(3,0);
				moveBaseScript.SetAttackMoveTime(3,0);
				mAttackCdTime = Time.time;
				ChangeAnimationByActionCmd(PlayerDataClass.PlayerActionCommand.Player_Attack3,true);
				break;
				
			case 3:
//				Debug.Log("tingdun Player_Attack4");
				mContinueAttack = false;
				aniControlScript.attackIdNow = 0;
				aniControlScript.SetAttackAnimationTime(4,0);
				moveBaseScript.SetAttackMoveTime(4,0);
				mAttackCdTime = Time.time;
				ChangeAnimationByActionCmd(PlayerDataClass.PlayerActionCommand.Player_Attack4,true);
				break;
			}
		}
		else
		{
			aniControlScript.attackIdNow = 1;
			mAttackCdTime = Time.time;
			ChangeAnimationByActionCmd(PlayerDataClass.PlayerActionCommand.Player_Attack1,true);
		}
	}
	
	
	void ContinueAttack()
	{
		switch(aniControlScript.attackCheckId)
		{
		case 1:
			mAttackCdTime = Time.time;
			aniControlScript.attackIdNow = 2;
//			Debug.Log("lianxu Player_Attack2");
			aniControlScript.SetAttackAnimationTime(2,0.2f);
			moveBaseScript.SetAttackMoveTime(2,0.2f);
			ChangeAnimationByActionCmd(PlayerDataClass.PlayerActionCommand.Player_Attack2,true);
			break;
			
		case 2:
	//		Debug.Log("lianxu Player_Attack3");
			aniControlScript.attackIdNow = 3;
			mAttackCdTime = Time.time;
			aniControlScript.SetAttackAnimationTime(3,0.2f);
			moveBaseScript.SetAttackMoveTime(3,0.2f);
			ChangeAnimationByActionCmd(PlayerDataClass.PlayerActionCommand.Player_Attack3,true);
			break;
			
		case 3:
//			Debug.Log("lianxu Player_Attack4");
			aniControlScript.attackIdNow = 0;
			mContinueAttack = false;
			mAttackCdTime = Time.time;
			aniControlScript.SetAttackAnimationTime(4,0.2f);
			moveBaseScript.SetAttackMoveTime(4,0.2f);
			ChangeAnimationByActionCmd(PlayerDataClass.PlayerActionCommand.Player_Attack4,true);
			break;
			
		case 4:
			break;
		}
	}
	
	
	void CheckMoveState()
	{
		if(moveBaseScript.curMovementState == PlayerMoveBase.PlayerMovementState.Run)
		{	
			if(PlayerDataClass.isSpecialAttack == false)
			{
				if(PlayerDataClass.CheckSpecialAttack())
					specialAttackIcon.renderer.enabled = true;
			}
		}
		else
		{
			specialAttackIcon.renderer.enabled = false;
			PlayerDataClass.ResetSpecialAttackTime();
		}
		
		
		bool immedilate = false;
		switch(moveBaseScript.curMovementState)
		{
		case PlayerMoveBase.PlayerMovementState.RunOver:
		case PlayerMoveBase.PlayerMovementState.TrotOver:
			ChangeAnimationByActionCmd(PlayerDataClass.PlayerActionCommand.Player_Walk,true);
			mTarget = null;
			break;
			
		case PlayerMoveBase.PlayerMovementState.WalkOver:
			ChangeAnimationByActionCmd(PlayerDataClass.PlayerActionCommand.Player_Idel,true);
			mTarget = null;
			break;
			
		case PlayerMoveBase.PlayerMovementState.BeHitOver:
			ChangeAnimationByActionCmd(PlayerDataClass.PlayerActionCommand.Player_Idel,true);
			mTarget = null;
			break;
			
		case PlayerMoveBase.PlayerMovementState.Attack1Over:
	//		Debug.Log("Player_Trot Attack1Over");
			mContinueAttack = true;
			PlayerDataClass.AttackStart = false;
			aniControlScript.SetAttackAnimationTime(2,0);
			if(PlayerDataClass.isChangeToRun == false)
				ChangeAnimationByActionCmd(PlayerDataClass.PlayerActionCommand.Player_Trot,true);
			mTarget = null;
			break;
			
		case PlayerMoveBase.PlayerMovementState.Attack2Over:
		//	Debug.Log("Player_Trot Attack2Over");
			mContinueAttack = true;
			PlayerDataClass.AttackStart = false;
			aniControlScript.SetAttackAnimationTime(3,0);
			if(PlayerDataClass.isChangeToRun == false)
				ChangeAnimationByActionCmd(PlayerDataClass.PlayerActionCommand.Player_Trot,true);
			mTarget = null;
			break;
			
		case PlayerMoveBase.PlayerMovementState.Attack3Over:
		//	Debug.Log("Player_Trot  Attack3Over");
			mContinueAttack = true;
			PlayerDataClass.AttackStart = false;
			aniControlScript.SetAttackAnimationTime(4,0);
			if(PlayerDataClass.isChangeToRun == false)
				ChangeAnimationByActionCmd(PlayerDataClass.PlayerActionCommand.Player_Trot,true);
			mTarget = null;
			break;		
		
		case PlayerMoveBase.PlayerMovementState.Attack4Over:
		//	Debug.Log("Player_Trot Attack4Over");
			PlayerDataClass.AttackStart = false;
			if(PlayerDataClass.isChangeToRun == false)
				ChangeAnimationByActionCmd(PlayerDataClass.PlayerActionCommand.Player_Trot,true);
			mTarget = null;
			break;	
			
		case PlayerMoveBase.PlayerMovementState.Skill1Over:
			PlayerDataClass.AttackStart = false;
			if(PlayerDataClass.isChangeToRun == false)
				ChangeAnimationByActionCmd(PlayerDataClass.PlayerActionCommand.Player_Trot,true);
			mTarget = null;
			break;
		
		case PlayerMoveBase.PlayerMovementState.JumpOver:
			ChangeAnimationByActionCmd(PlayerDataClass.PlayerActionCommand.Player_Trot,true);
			mTarget = null;
			break;
			
		case PlayerMoveBase.PlayerMovementState.RushOver:
			ChangeAnimationByActionCmd(PlayerDataClass.PlayerActionCommand.Player_Catch,true);
			mTarget = null;
			break;
			
		case PlayerMoveBase.PlayerMovementState.CatchOver:
	//		ChangeAnimationByActionCmd(PlayerDataClass.PlayerActionCommand.Player_Idel,true);
	//		colliderBaseScript.mTarget = null;
			break;
			
		}
	}
	
	public void ChangeAnimationByActionCmd(PlayerDataClass.PlayerActionCommand cmd, bool immedilate)
	{
		PlayerDataClass.PlayerNextActionSet(cmd);
		aniControlScript.UpdateCmdFromControl(immedilate);
	}
	
	// only call by other script
	public void GetInputState()
	{
		Vector3 targetPos ;
		switch(mTouchState)
		{
		case TouchState.AFingerOneTap:
			mTarget = null;
			PlayerDataClass.AttackStart = false;
			mTouchState = TouchState.None;
			
			targetPos = RayColliderByTapPos(InputStateClass.touchPointPos);
			
			if(targetPos != Vector3.zero)
			{
				// turn to the direction first
			//	SetPlayerToRun(targetPos-this.transform.position);
				
				if(directionIcon.renderer.enabled == true)
				{
					directionIcon.renderer.enabled = false;
					return;
				}
				
				if(aniControlScript.isSkillPlaying)
				{
					return;
				}
				
				// check the attack area
                mTarget = player.GetNearestAttackTarget(transform.forward);
				
				// if there is no enemy in the area, then player start to run
				if(moveBaseScript.curMovementState != PlayerMoveBase.PlayerMovementState.Jump
					&& moveBaseScript.curMovementState != PlayerMoveBase.PlayerMovementState.Attack1
					&& moveBaseScript.curMovementState != PlayerMoveBase.PlayerMovementState.Attack2
					&& moveBaseScript.curMovementState != PlayerMoveBase.PlayerMovementState.Attack3
					&& moveBaseScript.curMovementState != PlayerMoveBase.PlayerMovementState.Attack4)
				{
					if(mTarget == null)
					{
						PlayerDataClass.isChangeToRun = false;
						PlayerDataClass.targetAttackPos = Vector3.zero;;
						float dis = Vector3.Distance(targetPos,transform.position);
						if(dis < 1)
						{				
							ChangeAnimationByActionCmd(PlayerDataClass.PlayerActionCommand.Player_Idel,true);
						}
						else
						{
							moveBaseScript.targetPos = targetPos;
							if(moveBaseScript.curMovementState == PlayerMoveBase.PlayerMovementState.Run)
							{
								ChangeAnimationByActionCmd(PlayerDataClass.PlayerActionCommand.Player_Run,false);
								
							}
							else
								ChangeAnimationByActionCmd(PlayerDataClass.PlayerActionCommand.Player_Run,true);
						}
					}
					else
					{
						AttackAfterRun();
					}
				}
				else
				{
					aniControlScript.attackCheckPoint = false;
					mTarget = null;
					PlayerDataClass.targetAttackPos = targetPos;
					moveBaseScript.targetPos = targetPos;
				//	Debug.Log("xxxxxxx");
					PlayerDataClass.isChangeToRun = true;
					ChangeAnimationByActionCmd(PlayerDataClass.PlayerActionCommand.Player_Run,false);
				}
			}
			break;
			
		case TouchState.AFingerDoubleTap:
			mTouchState = TouchState.None;
			if(moveBaseScript.curMovementState != PlayerMoveBase.PlayerMovementState.Jump)
			{
				targetPos = RayColliderByTapPos(InputStateClass.touchPointPos);
				SetPlayerToRun(targetPos-this.transform.position);	
				if(aniControlScript.isSkillPlaying == false)
				{
					ChangeAnimationByActionCmd(PlayerDataClass.PlayerActionCommand.Player_Jump,true);
				}				
			}
			break;
			
		case TouchState.AFingerHold:
			if(moveBaseScript.curMovementState == PlayerMoveBase.PlayerMovementState.Attack3)
				ChangeAnimationByActionCmd(PlayerDataClass.PlayerActionCommand.Player_Attack3,false);
			else
				ChangeAnimationByActionCmd(PlayerDataClass.PlayerActionCommand.Player_Attack3,true);
				
        //    mTouchState = TouchState.None;
			break;	
		
		case TouchState.AFingerHoldStop:
			Debug.Log("Trot here");
			if(PlayerDataClass.isChangeToRun == false)
				ChangeAnimationByActionCmd(PlayerDataClass.PlayerActionCommand.Player_Trot,true);
            mTouchState = TouchState.None;
			break;
			
		case TouchState.AFingerSlash:
//            colliderBaseScript.turnOnCatchRay = true;
//            colliderBaseScript.mCatchTarget = null;
            mTouchState = TouchState.None;
            //targetPos = RayColliderByTapPos(InputStateClass.oldSlashPos);
            //SetPlayerToRun(targetPos-this.transform.position);
            //if(aniControlScript.isSkillPlaying == false)
            //{
            //    ChangeAnimationByActionCmd(PlayerDataClass.PlayerActionCommand.Player_Rush,true);
            //}			
			break;
			
		case TouchState.TwoFingersDoubleTap:
			mTouchState = TouchState.None;
			break;
			
		case TouchState.TwoFingersSlash:
			mTouchState = TouchState.None;
			break;
			
		case TouchState.TwoFingersTap:
			mTouchState = TouchState.None;
			break;
			
		default:
			break;
		}
	}
	
	Vector3 RayColliderByTapPos(Vector3 pos)
	{
		// NGUI area
	//	if(pos.x > Screen.width *0.8f)
	//		return Vector3.zero;
		
		Ray ray1 = Camera.mainCamera.ScreenPointToRay(pos);
		
		hit = Physics.RaycastAll(ray1,100,1<<1);
		foreach(RaycastHit hit1 in hit)
		{
			if(hit1.transform.tag == "ground")
				return hit1.point;
		}
		return Vector3.zero;
	}
	
	
	void SetPlayerToRun(Vector3 pos)
	{
		pos.y = transform.position.y;
		
		Vector3 dir = Vector3.Cross(transform.forward,pos.normalized);
		float dot = Vector3.Dot(transform.forward,pos.normalized);
		float angle = (float)Mathf.Acos(dot)*Mathf.Rad2Deg;
//		Debug.Log("pos="+pos+"  dir="+dir+"  dot="+dot+"  angle"+angle);
		
		if(float.IsNaN(angle))
		{
			angle = 0;
		}
		if(dir.y  < 0)
			angle = -1*angle;
		
		transform.RotateAround(transform.position,Vector3.up,angle);
	}
	
	void NGUI_Skill1()
	{			
		Time.timeScale = 0.2f;
		ChangeAnimationByActionCmd(PlayerDataClass.PlayerActionCommand.Player_Skill1,true);
		directionIcon.renderer.enabled = true;
		aniControlScript.isSkillPlaying = true;
        skill1Sfx.GetComponent<particleControl>().turnOn = true;
	}
	
	
	
	
	public void OnStartSkill(int skillId)
	{
		directionIcon.renderer.enabled = false;
	}

	public void OnSkillShootPoint(int skillId)
	{
		Time.timeScale = 1;
        player.PlaySkill(skillId);
	}

	void TargetTheEnemy(Vector3 enemy)
	{
		enemy.y = transform.position.y;
		transform.LookAt(enemy);
	}

    public void OnStartToAttack(int attackId)
    {
        attackSfx[attackId - 1].GetComponent<particleControl>().turnOn = true;
        player.Attack();
        OrbitCameraCtrl.instance.ShakeCamera(0);
    }

    public void OnAttackFinish(int attackId)
    {
    }

    public void OnAttackAnimationFinish(int attackId)
    {

    }

    // 物品被拾取，播放拾取特效
    public void OnPickup(SceneObj _object)
    {
        pickupSfx.GetComponent<particleControl>().turnOn = true;
    }
	
	


}
