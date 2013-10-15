using UnityEngine;
using System.Collections;

namespace GameBaseData
{
	#region Player
	
	public class PlayerDataClass
	{  
		public static bool isSpecialAttack;
		
		// 20 monster at most
		//todo
		public static Transform[] AllMonsterInAttackArea = new Transform[20];
		
		public static void ClearMonsterArrayInAttackArea()
		{
			for(int i=0; i<AllMonsterInAttackArea.Length; i++)
			{
				AllMonsterInAttackArea[i] = null;
			}
		}
		
		public static bool AddMonsterToAttackAreaArray(Transform tranf)
		{
			for(int i=0; i<AllMonsterInAttackArea.Length; i++)
			{
				if(AllMonsterInAttackArea[i] == tranf)
				{
					Debug.Log(" Add  failed :   "+ tranf+ " is aready exist in array !!");
					return false;
				}
				if(AllMonsterInAttackArea[i] == null)
				{
					AllMonsterInAttackArea[i] = tranf;
					return true;
				}
			}
			//array full
			Debug.Log("AddMonsterToAttackAreaArray :  the array is full !!!  ");
			return false;
		}
		
		public static bool AttackStart = false;
		
		public enum PlayerActionCommand
		{
			Player_None,
			Player_Run,
			Player_Trot,
			Player_Walk,
			Player_Idel,
			Plyaer_SkillIdel,
			Player_Die,
			Player_BeHit,
			Player_Attack1,
			Player_Attack2,
			Player_Attack3,
			Player_Skill1,
			Player_Rush,
			Player_Jump,
			Player_Catch,
		};
		
		// the action next
		public static PlayerActionCommand playerAniCmdNext = PlayerActionCommand.Player_Idel;
		
		public static void PlayerNextActionSet(PlayerActionCommand cmd)
		{
			playerAniCmdNext = cmd;
		}
		
		public static void PlayerNextActionReset()
		{
			playerAniCmdNext = PlayerActionCommand.Player_None;
		}
		
		private static float mSpecialAttackTime=0;
		
		public static bool CheckSpecialAttack()
		{
			if(Time.time - mSpecialAttackTime > 3)
			{
				isSpecialAttack = true;
			}
			else
				isSpecialAttack = false;
			return isSpecialAttack;
		}
		
		public static void ResetSpecialAttackTime()
		{
			mSpecialAttackTime = Time.time;
		}
	}
	
	
	#endregion
	
	#region  Input	
	public class InputStateClass
	{
		//save the position for tap
		public static Vector3 touchPointPos;
		
		public static Vector3 oldSlashPos;
		
		//save the position for slash touch
		public static Vector3[] touchSlashPos = new Vector3[10];
		
		public static float DisPointToPoint = Screen.width/80;
		
		public static void ClearTouchSlashPosArray()
		{
			for(int i=0; i<touchSlashPos.Length; i++)
			{
				touchSlashPos[i] = Vector3.zero;
			}
		}
		
		public static bool AddPointToSlashPosArray(Vector3 pos)
		{
			for(int i=0; i<touchSlashPos.Length; i++)
			{
				if(touchSlashPos[i] != Vector3.zero)
				{
					touchSlashPos[i] = pos;
					return true;
				}
			}
			return false;
		}
		
			
		
	}
	
	#endregion
	
	#region Enemy
	/*
	public class EnemyDataClass
	{
		public enum EnemyState
		{
			Walk,
			Run,
			Attack,
			
			
			
			
		};
	}
	*/
	
	#endregion
}
