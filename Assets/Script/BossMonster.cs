using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMonster : MonoBehaviour
{
    public GameObject Decal, arm;
    private GameObject obj1, target;
    private Vector3 dir;
    private Quaternion lookTarget;
    private Animator anim;

    private bool Moving = true, PlayerInvincible = false;
    private float Distance, timer = 0;

    private int HitCount = 0; 
    private int HP = 8;       
    private bool isDead = false; 


    void Start()
    {
        anim = GetComponent<Animator>();
        target = GameObject.FindWithTag("Player");
    }

    void Update()
    {
        obj1 = GameObject.Find("MagicDecal(Clone)");
        if (obj1 != null) Destroy(obj1);

        if (!isDead)
        {
            Movement();
            Attack();
        }
        if(anim.GetCurrentAnimatorStateInfo(0).IsName("Attack02")){
            if(anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 0.6f){
                arm.GetComponent<CapsuleCollider>().enabled = true;
            }else{
                arm.GetComponent<CapsuleCollider>().enabled = false;
            }
        }
        Invincible();
    }

    public void Movement()
    {
        Vector3 TTP = new Vector3(target.transform.position.x, 0, target.transform.position.z);
        if (Moving) {transform.position = Vector3.MoveTowards(gameObject.transform.position, TTP, Time.deltaTime * 3f);}

        dir = target.transform.position - transform.position;
        lookTarget = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Lerp(transform.rotation, lookTarget, 0.25f);
    }

    public void Attack()
    {
        Distance = Vector3.Distance(transform.position, target.transform.position);

        if (Distance < 5)
        {
            transform.rotation = Quaternion.Euler(-2, transform.eulerAngles.y, 0);
            anim.SetBool("Attack", true);
            anim.SetBool("Running", false);
            Moving = false;
        }
        else
        {
            anim.SetBool("Attack", false);
            anim.SetBool("Running", true);
            Moving = true;
        }
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        HitCount += damage;
        Debug.Log($"{gameObject.name}이(가) {damage} 데미지를 받았습니다! 현재 HP: {HP - HitCount}");

        if (HitCount < HP){
            anim.Play("GetHit");}
        else {Die();}
    }

    private void Die()
    {
        if (isDead) return;

        isDead = true;
        Moving = false;

        anim.Play("Die");

        // 게임 클리어 UI 호출
        PlayerUIManager playerUI = FindObjectOfType<PlayerUIManager>();
        if (playerUI != null)
        {
            playerUI.ShowClearUI();
        }

        Destroy(gameObject, 2);
    }

    private void Invincible(){  //플레이어 무적 - 중첩공격 방지
        if(PlayerInvincible){
            timer += Time.deltaTime;
            if(timer > 0.8f){
                timer = 0;
                PlayerInvincible = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) 
        {
            if(!PlayerInvincible){
                PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(20);
                    //Debug.Log("플레이어가 공격을 받았습니다!");
                }
                PlayerInvincible = true;
            }
        }
    }

    private void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.name == "Magic fire pro green(Clone)")
        {
            TakeDamage(1);
            print(HP);
            Destroy(col.gameObject);
        }
        
    }
}

