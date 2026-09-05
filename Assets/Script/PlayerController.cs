using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(CharacterController), typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    [Header("Player Settings")]
    [SerializeField] private float playerSpeed = 2.0f;
    [SerializeField] private float jumpHeight = 1.0f;
    [SerializeField] private float gravityValue = -9.81f;
    [SerializeField] private float rotationSpeed = 5f;

    [Header("Bullet Settings")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform barrelTransform;
    [SerializeField] private Transform bulletParent;
    [SerializeField] private float bulletHitMissDistance = 25f;

    [Header("Animation Settings")]
    [SerializeField] private float animationSmoothTime = 0.1f;
    [SerializeField] private float animationPlayTransition = 0.15f;

    [Header("Skill Settings")]
    [SerializeField] private float skillAttackRadius1 = 5f; 
    [SerializeField] private float skillAttackRange2 = 8f;  
    [SerializeField] private int skillAttackDamage1 = 10;   
    [SerializeField] private int skillAttackDamage2 = 5;    
    [SerializeField] private float buffDuration = 5f;       
    [SerializeField] private float speedMultiplier = 1.5f;

    [Header("Skill Effects")]
    [SerializeField] private GameObject qSkillEffectPrefab; 
    [SerializeField] private GameObject eSkillEffectPrefab; 
    [SerializeField] private GameObject rSkillEffectPrefab; 
    [SerializeField] private Transform skillEffectParent;   

    [SerializeField] private LayerMask enemyLayer;

    private CharacterController controller;
    private PlayerInput playerInput;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    private Transform cameraTransform;

    private InputAction moveAction, jumpAction, shootAction, skillAttack1Action, skillAttack2Action, skillAttack3Action;
    private Animator animator;
    private bool isBuffActive = false;

    private int jumpAnimation, basicAttackAnimation;
    private int skillAttack1Animation, skillAttack2Animation, skillBuffAnimation;

    Vector2 currentAnimationBlendVector, animationVelocity;

    private bool Q_Skill = true, E_Skill = true, R_Skill = true;
    private float Qtimer, Etimer = 5, Rtimer = 6;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        cameraTransform = Camera.main.transform;

        moveAction = playerInput.actions["Move"];
        jumpAction = playerInput.actions["Jump"];
        shootAction = playerInput.actions["Shoot"];
        skillAttack1Action = playerInput.actions["SkillAttack1"];
        skillAttack2Action = playerInput.actions["SkillAttack2"];
        skillAttack3Action = playerInput.actions["SkillAttack3"];

        animator = GetComponent<Animator>();
        jumpAnimation = Animator.StringToHash("Jump");
        basicAttackAnimation = Animator.StringToHash("BasicAttack");
        skillAttack1Animation = Animator.StringToHash("SkillAttack1");
        skillAttack2Animation = Animator.StringToHash("SkillAttack2");
        skillBuffAnimation = Animator.StringToHash("SkillBuff");
    }

    private void OnEnable()
    {
        shootAction.performed += _ => ShootGun();
        skillAttack1Action.performed += _ => PerformSkillAttack1();
        skillAttack2Action.performed += _ => PerformSkillAttack2();
        skillAttack3Action.performed += _ => ActivateBuffSkill();
    }

    private void Update()
    {
        groundedPlayer = controller.isGrounded;
        if (groundedPlayer && playerVelocity.y < 0)
            playerVelocity.y = 0f;

        Vector2 input = moveAction.ReadValue<Vector2>();
        currentAnimationBlendVector = Vector2.SmoothDamp(currentAnimationBlendVector, input, ref animationVelocity, animationSmoothTime);
        Vector3 move = new Vector3(currentAnimationBlendVector.x, 0, currentAnimationBlendVector.y);
        move = move.x * cameraTransform.right.normalized + move.z * cameraTransform.forward.normalized;
        move.y = 0f;

        controller.Move(move * Time.deltaTime * playerSpeed);

        animator.SetFloat("MoveX", currentAnimationBlendVector.x);
        animator.SetFloat("MoveZ", currentAnimationBlendVector.y);

        if (jumpAction.triggered && groundedPlayer)
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
            animator.CrossFade(jumpAnimation, animationPlayTransition);
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        Quaternion targetRotation = Quaternion.Euler(0, cameraTransform.eulerAngles.y, 0);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        Skill_CoolTime();

        groundedPlayer = controller.isGrounded;

        if (groundedPlayer && playerVelocity.y < 0)
        {
            playerVelocity.y = -2.0f;
        }
    }

    // Q 스킬
    private void PerformSkillAttack1()
    {
        
        if(Q_Skill){
            animator.CrossFade(skillAttack1Animation, animationPlayTransition);
            ShowSkillEffect(qSkillEffectPrefab, transform.position, skillAttackRadius1);

            Collider[] hitEnemies = Physics.OverlapSphere(transform.position, skillAttackRadius1, enemyLayer);
            foreach (Collider enemy in hitEnemies)
            {
                if (enemy.GetComponent<Giant_Golem>())
                {
                    enemy.GetComponent<Giant_Golem>().TakeDamage(skillAttackDamage1);
                    Debug.Log($"{enemy.name}에게 {skillAttackDamage1} 스킬 데미지!");
                }
                if (enemy.GetComponent<BossMonster>())
                {
                    enemy.GetComponent<BossMonster>().TakeDamage(skillAttackDamage1);
                    Debug.Log($"{enemy.name}에게 {skillAttackDamage1} 스킬 데미지!");
                }
            }

            Q_Skill = false;
            Qtimer = 8;
        }
        
    }

    // E 스킬
    private void PerformSkillAttack2()
    {
        if(E_Skill){
            animator.CrossFade(skillAttack2Animation, animationPlayTransition);
            Vector3 boxCenter = transform.position + transform.forward * (skillAttackRange2 / 2);
            ShowSkillEffect(eSkillEffectPrefab, boxCenter, skillAttackRange2);

            Vector3 boxHalfExtents = new Vector3(3f, 2f, skillAttackRange2 / 2);
            Collider[] hitEnemies = Physics.OverlapBox(boxCenter, boxHalfExtents, transform.rotation, enemyLayer);
            foreach (Collider enemy in hitEnemies)
            {
                if (enemy.GetComponent<Giant_Golem>())
                {
                    enemy.GetComponent<Giant_Golem>().TakeDamage(skillAttackDamage2);
                    Debug.Log($"{enemy.name}에게 {skillAttackDamage2} 스킬 데미지!");
                }
                if (enemy.GetComponent<BossMonster>())
                {
                    enemy.GetComponent<BossMonster>().TakeDamage(skillAttackDamage2);
                    Debug.Log($"{enemy.name}에게 {skillAttackDamage2} 스킬 데미지!");
                }
            }

            E_Skill = false;
            Etimer = 5;
        }
    }

    // R 스킬 (버프)
    private void ActivateBuffSkill()
    {
        if (!isBuffActive)
            StartCoroutine(BuffCoroutine());
    }

    private IEnumerator BuffCoroutine()
    {
        if(R_Skill){
            isBuffActive = true;
            playerSpeed *= speedMultiplier;
            ShowSkillEffect(rSkillEffectPrefab, transform.position, 3f);

            Debug.Log("이동 속도 상승!");
            yield return new WaitForSeconds(buffDuration);

            playerSpeed /= speedMultiplier;
            isBuffActive = false;
            Debug.Log("버프 종료!");

            R_Skill = false;
            Rtimer = 6;
        }
    }

    private void ShowSkillEffect(GameObject effectPrefab, Vector3 position, float scale)
    {
        if (effectPrefab != null)
        {
            GameObject effect = Instantiate(effectPrefab, position, Quaternion.identity, skillEffectParent);
            effect.transform.localScale = Vector3.one * scale;
            Destroy(effect, 2.0f); 
        }
    }

    private void ShootGun()
    {
        RaycastHit hit;
        GameObject bullet = Instantiate(bulletPrefab, barrelTransform.position, Quaternion.identity, bulletParent);
        BulletController bulletController = bullet.GetComponent<BulletController>();

        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, Mathf.Infinity))
            bulletController.target = hit.point;
        else
            bulletController.target = cameraTransform.position + cameraTransform.forward * bulletHitMissDistance;

        animator.CrossFade(basicAttackAnimation, animationPlayTransition);
    }

    private void Skill_CoolTime(){
        if(!Q_Skill){
            Qtimer -= Time.deltaTime;
            if(Qtimer <= 0){Q_Skill = true;}
        }
        if(!E_Skill){
            Etimer -= Time.deltaTime;
            if(Etimer <= 0){E_Skill = true;}
        }if(!R_Skill){
            Rtimer -= Time.deltaTime;
            if(Rtimer <= 0){R_Skill = true;}
        }
    }
}
