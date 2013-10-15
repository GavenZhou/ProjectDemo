using UnityEngine;
using System.Collections;
using GameBaseData;

public class PlayerColliderBase : MonoBehaviour {
	
	
	public bool turnOnCatchRay;
	
	public Transform mCatchTarget;
	
	//catch ray target
	public GameObject catchArea;
	
	//attack ray target
	public GameObject[] attackArea;
	
	private RaycastHit[] attackHit;
	private RaycastHit catchHit;
	
	// Use this for initialization
	void Start () {
		mCatchTarget = null;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(turnOnCatchRay)
		{
			bool isCatchTarget = RayToCatchPoint();
		}
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
