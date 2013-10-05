using UnityEngine;
using System.Collections.Generic;

public class Mob : Actor {

    ///////////////////////////////////////////////////////////////////////////////
    // variable
    ///////////////////////////////////////////////////////////////////////////////
    
    public int mobTemplate = 0;

    // ------------------------------------------------------------------ 
    // private
    // ------------------------------------------------------------------

    float attackRadius = 3.5f;
    float attackAngle = 120;

    // ------------------------------------------------------------------ 
    // reference
    // ------------------------------------------------------------------

	EnemyMainLogic enemyMainLogic;
    AudioSource audioSource;
    TextMesh hudMesh;

    // ------------------------------------------------------------------ 
    // static
    // ------------------------------------------------------------------

    static AudioClip audioClipInjury = Resources.Load("Audio/injury", typeof(AudioClip)) as AudioClip;


    ///////////////////////////////////////////////////////////////////////////////
    // function
    ///////////////////////////////////////////////////////////////////////////////

    // ------------------------------------------------------------------ 
    // mono
    // ------------------------------------------------------------------
    protected void Start() {

        this.Init(CombatUtility.GenNextMobID());
        SceneMng.instance.AddSceneObj(this);
    }

    // ------------------------------------------------------------------ 
    // virtual 
    // ------------------------------------------------------------------

    public override void Init(int _id) {

        // 
        base.Init(_id); 
        
        type = SceneObjType.Player;
        Hp = MaxHp = 100;

        enemyMainLogic = (EnemyMainLogic)this.transform.GetComponent<EnemyMainLogic>();
        audioSource = gameObject.AddComponent<AudioSource>();

        InitHudText();
    }

    public override void OnDespawn() {
        // 
        base.OnDespawn();
    }

    public override void Attack() {
        
        //
        base.Attack();

        Vector3 _pos = transform.position;
        Vector3 _dir = transform.forward;
        CombatUtility.CombatParam_AttackRange param = CombatUtility.GetConeParam(_pos, _dir, attackAngle * Mathf.Deg2Rad, attackRadius);
        List<Player> targets = CombatUtility.GetInteractiveObjects<Player>(SceneMng.instance, ref param);
        foreach (Player actor in targets) {
            if (!actor.IsDie) {
                actor.Hurt(this);
            }
        }
    }

    public override bool Hurt(SceneObj _object) {
        Hp -= 15;
        UpdateHudText();
        bool isdead = base.Hurt(_object);
		if(isdead)
			enemyMainLogic.mState = EnemyMainLogic.EnemyState.Die;
		else
			enemyMainLogic.ChangeAnimationByState(EnemyMainLogic.EnemyState.BeHit,true);

        audioSource.PlayOneShot(audioClipInjury);
		return isdead;
    }

    public override void Dead(SceneObj _object) {
        base.Dead(_object);
    }

    // ------------------------------------------------------------------ 
    // private
    // ------------------------------------------------------------------

    void InitHudText() {
        hudMesh = gameObject.GetComponentInChildren<TextMesh>() as TextMesh;
        UpdateHudText();
    }

    void UpdateHudText() {
        if (hudMesh != null) {
            if (Hp > 0) {
                hudMesh.text = "<color=red>Mob:" + id + "</color> <color=red>" + Hp + "/" + MaxHp + "</color>";
            }
            else {
                hudMesh.text = "";
            }
        }
    }

#if false
    void OnDrawGizmos() {

        Gizmos.color = Color.red;

        Vector3 _pos = transform.position;
        Vector3 _dir = transform.forward;

        Quaternion q1 = Quaternion.Euler(0, attackAngle / 2, 0);
        Vector3 _left = q1 * _dir;
        Gizmos.DrawLine(_pos, _pos + _left * attackRadius);

        Quaternion q2 = Quaternion.Euler(0, -attackAngle / 2, 0);
        Vector3 _right = q2 * _dir;
        Gizmos.DrawLine(_pos, _pos + _right * attackRadius);

        GizmosHelper.DrawConeArc(Quaternion.identity, _pos, _dir, attackRadius, attackAngle);

        CombatUtility.CombatParam_AttackRange param = CombatUtility.GetConeParam(_pos, _dir, attackAngle * Mathf.Deg2Rad, attackRadius);
        List<SceneObj> interatives = CombatUtility.GetInteractiveObjects<SceneObj>(SceneMng.instance, ref param);

        Gizmos.color = Color.green;
        foreach (SceneObj o in interatives) {
            Gizmos.DrawWireSphere(o.transform.position + Vector3.up * 2.2f, 0.25f);
        }
    }
#endif
}
