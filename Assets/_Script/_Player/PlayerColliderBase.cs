using UnityEngine;
using System.Collections;
using GameBaseData;

public class PlayerColliderBase : MonoBehaviour {
	
	
	public bool turnOnCatchRay;
	
	public Transform mTarget;
	
	public Transform mCatchTarget;
	
	//catch ray target
	public GameObject catchArea;
	
	//attack ray target
	public GameObject[] attackArea;
	
	private RaycastHit[] attackHit;
	private RaycastHit catchHit;
	
	// Use this for initialization
	void Start () {
		mTarget = null;
		mCatchTarget = null;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(mTarget == null)
		{
			// empty the monster array before search monster start
			//PlayerDataClass.ClearMonsterArrayInAttackArea();
			bool isFindTarget = RayToAttackArea();
		}
		
		if(turnOnCatchRay)
		{
			bool isCatchTarget = RayToCatchPoint();
		}
	}
	
	
	public bool RayToAttackArea()
	{
		for(int i=0; i<attackArea.Length; i++)
		{
			Vector3 targetPos = attackArea[i].transform.position;
			Vector3 startPos = transform.position;
			startPos.y = targetPos.y;
			attackHit = Physics.RaycastAll(startPos,targetPos-startPos, Vector3.Distance(targetPos,startPos));
			Debug.DrawLine(startPos,targetPos,Color.red);
			foreach(RaycastHit hit1 in attackHit)
			{
				if(hit1.transform != null)
				{
					if(hit1.transform.tag == "monster" || hit1.transform.tag == "boss")
					{
//						Debug.Log(hit1.transform.name);
						mTarget = hit1.transform;
						return true;
						// Select monster from the array, need AI system
						// todo
						// PlayerDataClass.AddMonsterToAttackAreaArray(hit1.transform);
					}
				}
			}
		}
		mTarget = null;
		return false;
	}
	
	
	public bool RayToCatchPoint()
	{
		Vector3 targetPos = catchArea.transform.position;
		Vector3 startPos = transform.position;
		startPos.y = targetPos.y;
		
		Debug.DrawLine(startPos,targetPos,Color.black);
		
		if(Physics.Raycast(startPos,targetPos-startPos,out catchHit,Vector3.Distance(startPos,targetPos)))
		{
//			Debug.Log(catchHit.transform.name);
			
			if(catchHit.transform.tag == "enemy")
			{
				mCatchTarget = catchHit.transform;
				return true;
			}
		}
		mCatchTarget = null;
		return false;
	}
	
	
	
	
}
