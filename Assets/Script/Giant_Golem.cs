using System.Collections;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

public class Giant_Golem : MonoBehaviour
{
    public GameObject Decal, arm;
    private GameObject obj1, target;
    private Vector3 dir;
    private Quaternion lookTarget;
    private Animator anim;
    private CapsuleCollider collider;

    private bool Moving = true, PlayerInvincible = false;
    private float Distance, timer = 0;

    private int HitCount = 0; 
    private int HP = 0;       
    private bool isDead = false; 

    void Start()
    {
        anim = GetComponent<Animator>();
        target = GameObject.FindWithTag("Player");
        collider = GetComponent<CapsuleCollider>();


        if (gameObject.name.Contains("Golem_Normal")) { HP = 3; }
        if (gameObject.name.Contains("Golem_Blue")) { HP = 5; }
        if (gameObject.name.Contains("Golem_Red")) { HP = 7; }
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
            if(anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.2f){
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
        if (Moving)
        {
            transform.position = Vector3.MoveTowards(gameObject.transform.position, TTP, Time.deltaTime);
        }

        dir = target.transform.position - transform.position;
        dir.y = 0f;

        if (dir != Vector3.zero)
        {
            Quaternion lookTarget = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Lerp(transform.rotation, lookTarget, 0.25f);
        }
    }

    public void Attack()
    {
        Distance = Vector3.Distance(transform.position, target.transform.position);

        if (Distance < 2)
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

        if (HitCount < HP)
        {
            anim.Play("Hit_Small");
        }
        else
        {
            Die();
        }
    }

    private void Die()
    {
        

        if (isDead) return;

        isDead = true;
        Moving = false;

        anim.SetBool("Alive", false);

        PlayerUIManager playerUI = FindObjectOfType<PlayerUIManager>();
        if (playerUI != null)
        {
            if (gameObject.name.Contains("Red")) playerUI.AddPoints(50);        // Red 골렘: 50점
            else if (gameObject.name.Contains("Blue")) playerUI.AddPoints(30);  // Blue 골렘: 30점
            else if (gameObject.name.Contains("Normal")) playerUI.AddPoints(15);// Normal 골렘: 15점
        }

        
        Destroy(gameObject, 4); 
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
                    playerHealth.TakeDamage(25);
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
            Destroy(col.gameObject);
        }
        
    }
}

