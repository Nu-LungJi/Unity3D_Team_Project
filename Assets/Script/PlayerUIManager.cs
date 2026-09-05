using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class PlayerUIManager : MonoBehaviour
{
    [Header("Skill Cooldown UI")]
    public Image qSkillImage;
    public Image eSkillImage;
    public Image rSkillImage;

    [Header("Skill Cooldowns")]
    public float qSkillCooldown = 8f;
    public float eSkillCooldown = 5f;
    public float rSkillCooldown = 6f;

    [Header("Health UI")]
    public Image healthBar;
    public int maxHealth = 100;
    private int currentHealth;

    [Header("Point System")]
    public Image pointBar;
    public int maxPoints = 300;
    private int currentPoints = 0;

    [Header("Boss Settings")]
    public GameObject bossPrefab;
    public GameObject SpawnPoint;

    [Header("Game Over UI")]
    public GameObject gameOverPanel;
    public Button restartButton;
    public Button quitButton;

    [Header("Game Clear UI")]
    public GameObject clearPanel; // 클리어 UI 패널
    public Button clearRestartButton;
    public Button clearQuitButton;

    private bool isBossSpawned = false;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHealthUI();
        UpdatePointUI();

        if (gameOverPanel != null) gameOverPanel.SetActive(false);
        if (clearPanel != null) clearPanel.SetActive(false);

        restartButton.onClick.AddListener(RestartGame);
        quitButton.onClick.AddListener(QuitGame);

        if (clearRestartButton != null)
            clearRestartButton.onClick.AddListener(RestartGame);
        if (clearQuitButton != null)
            clearQuitButton.onClick.AddListener(QuitGame);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            StartCoroutine(StartCooldown(qSkillImage, qSkillCooldown));

        if (Input.GetKeyDown(KeyCode.E))
            StartCoroutine(StartCooldown(eSkillImage, eSkillCooldown));

        if (Input.GetKeyDown(KeyCode.R))
            StartCoroutine(StartCooldown(rSkillImage, rSkillCooldown));
    }

    private IEnumerator StartCooldown(Image skillImage, float cooldownTime)
    {
        float elapsed = 0f;
        skillImage.fillAmount = 1f;

        while (elapsed < cooldownTime)
        {
            elapsed += Time.deltaTime;
            skillImage.fillAmount = 1f - (elapsed / cooldownTime);
            yield return null;
        }
        skillImage.fillAmount = 0f;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0) currentHealth = 0;

        UpdateHealthUI();

        if (currentHealth == 0)
            PlayerDie();
    }

    private void UpdateHealthUI()
    {
        healthBar.fillAmount = (float)currentHealth / maxHealth;
    }

    public void AddPoints(int points)
    {
        currentPoints += points;
        if (currentPoints > maxPoints) currentPoints = maxPoints;

        UpdatePointUI();

        if (currentPoints >= maxPoints && !isBossSpawned)
            SpawnBoss();
    }

    private void UpdatePointUI()
    {
        pointBar.fillAmount = (float)currentPoints / maxPoints;
    }

    private void SpawnBoss()
    {
        isBossSpawned = true;

        if (bossPrefab != null)
        {
            Instantiate(bossPrefab, SpawnPoint.transform.position, Quaternion.identity);
            Debug.Log("보스 등장");

            FindObjectOfType<EffectSoundManager>()?.PlayBossMusic();
        }
    }

    private void PlayerDie()
    {
        Debug.Log("플레이어가 사망했습니다!");

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
            Time.timeScale = 0f;
        }
    }

    public void ShowClearUI()
    {
        Debug.Log("게임 클리어!");
        if (clearPanel != null)
        {
            clearPanel.SetActive(true);
            Time.timeScale = 0f; // 게임 멈춤
        }
    }

    private void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void QuitGame()
    {
        Debug.Log("게임 종료!");
        Application.Quit();
    }
}
