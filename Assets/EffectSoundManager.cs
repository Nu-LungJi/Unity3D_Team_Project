using UnityEngine;
using UnityEngine.InputSystem;

public class EffectSoundManager : MonoBehaviour
{
    [Header("Audio Clips")]
    [SerializeField] private AudioClip leftClickSound;  // 마우스 좌클릭 효과음
    [SerializeField] private AudioClip qSkillSound;     // Q 스킬 효과음
    [SerializeField] private AudioClip eSkillSound;     // E 스킬 효과음
    [SerializeField] private AudioClip rSkillSound;     // R 스킬 효과음
    [SerializeField] private AudioClip backgroundMusic1; // 초기 배경음악
    [SerializeField] private AudioClip bossMusic;        // 보스 배경음악
    [SerializeField] private AudioClip bossAppearSound;  // 보스 등장 효과음

    [Header("Audio Settings")]
    [SerializeField] private AudioSource audioSource;   // 효과음
    [SerializeField] private AudioSource bgmSource;     // 배경음악

    private PlayerInput playerInput;

    private InputAction leftClickAction;
    private InputAction qSkillAction;
    private InputAction eSkillAction;
    private InputAction rSkillAction;

    private void Awake()
    {
        // PlayerInput 설정
        playerInput = GetComponent<PlayerInput>();

        // AudioSource 설정
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
            audioSource.volume = 1.0f;
        }

        if (bgmSource == null)
        {
            bgmSource = gameObject.AddComponent<AudioSource>();
            bgmSource.playOnAwake = false;
            bgmSource.loop = true;
            bgmSource.volume = 0.5f;
        }

        // InputAction 설정
        if (playerInput != null)
        {
            leftClickAction = playerInput.actions["Shoot"];
            qSkillAction = playerInput.actions["SkillAttack1"];
            eSkillAction = playerInput.actions["SkillAttack2"];
            rSkillAction = playerInput.actions["SkillAttack3"];
        }
    }

    private void Start()
    {
        PlayBackgroundMusic();
    }

    private void OnEnable()
    {
        if (leftClickAction != null) leftClickAction.performed += _ => PlaySound(leftClickSound);
        if (qSkillAction != null) qSkillAction.performed += _ => PlaySound(qSkillSound);
        if (eSkillAction != null) eSkillAction.performed += _ => PlaySound(eSkillSound);
        if (rSkillAction != null) rSkillAction.performed += _ => PlaySound(rSkillSound);
    }

    private void OnDisable()
    {
        if (leftClickAction != null) leftClickAction.performed -= _ => PlaySound(leftClickSound);
        if (qSkillAction != null) qSkillAction.performed -= _ => PlaySound(qSkillSound);
        if (eSkillAction != null) eSkillAction.performed -= _ => PlaySound(eSkillSound);
        if (rSkillAction != null) rSkillAction.performed -= _ => PlaySound(rSkillSound);
    }

    // 일반 효과음 재생
    private void PlaySound(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    // 초기 배경음악 재생
    private void PlayBackgroundMusic()
    {
        if (backgroundMusic1 != null && bgmSource != null)
        {
            bgmSource.clip = backgroundMusic1;
            bgmSource.Play();
        }
    }

    // 보스 등장 시 배경음악 변경 및 효과음 재생
    public void PlayBossMusic()
    {
        if (bgmSource.isPlaying)
            bgmSource.Stop();

        if (bossMusic != null)
        {
            bgmSource.clip = bossMusic;
            bgmSource.Play();
        }


        if (bossAppearSound != null)
        {
            audioSource.PlayOneShot(bossAppearSound);
        }

    }
}
