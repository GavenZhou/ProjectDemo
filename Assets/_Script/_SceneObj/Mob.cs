using UnityEngine;
using System.Collections.Generic;
using System.Collections;


public class Mob : Actor, ISpawn {

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
    protected void Awake() {

        enemyMainLogic = transform.GetComponent<EnemyMainLogic>() as EnemyMainLogic;
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    // ------------------------------------------------------------------ 
    // virtual 
    // ------------------------------------------------------------------

    public override void Init(int _id) {

        // 
        base.Init(_id); 
        
        type = SceneObjType.Mob;
        Hp = MaxHp = 100;
        IsDie = false;
        enemyMainLogic.Init();
        Name = type.ToString() + mobTemplate.ToString() + "_" + _id;
        gameObject.name = Name;

        InitHudText();
    }

    public virtual void OnSpawn() {
        Init(CombatUtility.GenNextMobID());
        SceneMng.instance.AddSceneObj(this);
    }

    public virtual void OnDespawn() {
        // 
        gameObject.SetActive(false);
        SceneMng.instance.RemoveSceneObj(this);
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

        StartCoroutine("Dead_Coroutine", 10.0f);
    }

    // ------------------------------------------------------------------ 
    // private
    // ------------------------------------------------------------------

    IEnumerator Dead_Coroutine(float _f) {

        yield return new WaitForSeconds(_f);

        float timer = 0.0f;
        float duration = 4.0f;
        Vector3 start = transform.position;
        Vector3 end = start + new Vector3( 0.0f, -2.0f, 0.0f );

        while ( timer <= duration ) {
            float ratio = timer / duration;
            Vector3 pos = Vector3.Lerp(start, end, ratio);
            transform.position = pos;

            //
            timer += Time.deltaTime;
            yield return 0;
        }
        Spawner.instance.DespawnMob(this);
    }

    void InitHudText() {
        hudMesh = gameObject.GetComponentInChildren<TextMesh>() as TextMesh;
        UpdateHudText();
    }

    void UpdateHudText() {
        if (hudMesh != null) {
            if (Hp > 0) {
                hudMesh.text = "<color=red>" + Name + "</color> <color=red>" + Hp + "/" + MaxHp + "</color>";
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
