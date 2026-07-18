using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [Header("Health UI")]
    [SerializeField] private TextMeshProUGUI healthText;
    [Tooltip("Value to show on the UI before the player is found")]
    [SerializeField] private float initialHealth = 100f;

    [Header("Death UI")]
    [Tooltip("Assign a UI panel (GameObject) to show when the player dies")]
    [SerializeField] private GameObject deathScreen;

    private PlayerController playerController;

    private void Awake()
    {
        if (healthText != null)
        {
            healthText.text = $"Health: {initialHealth:F0}";
        }

        if (deathScreen == null)
        {
            CreateDefaultDeathScreen();
        }
    }

    private void Start()
    {
        BindPlayer();
    }

    private void OnDisable()
    {
        UnbindPlayer();
    }

    private void Update()
    {
        if (deathScreen != null && deathScreen.activeSelf && Input.GetKeyDown(KeyCode.Space))
        {
            RestartScene();
        }
    }

    private void BindPlayer()
    {
        UnbindPlayer();

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj == null)
        {
            Debug.LogWarning("PlayerController not found at UIManager.Start.");
            return;
        }

        if (!playerObj.TryGetComponent(out PlayerController controller))
        {
            Debug.LogWarning("Player object is missing PlayerController.");
            return;
        }

        playerController = controller;
        playerController.HealthChanged += HandleHealthChanged;
        playerController.Died += HandlePlayerDied;

        HandleHealthChanged(playerController.GetHealth(), playerController.GetMaxHealth());
    }

    private void UnbindPlayer()
    {
        if (playerController == null)
        {
            return;
        }

        playerController.HealthChanged -= HandleHealthChanged;
        playerController.Died -= HandlePlayerDied;
        playerController = null;
    }

    private void HandleHealthChanged(float currentHealth, float maxHealth)
    {
        if (healthText != null)
        {
            healthText.text = $"Health: {currentHealth:F0}";
            return;
        }

        Debug.Log($"Player health: {currentHealth:F0}/{maxHealth:F0}");
    }

    public void UpdateHealthDisplay()
    {
        if (playerController == null) return;

        HandleHealthChanged(playerController.GetHealth(), playerController.GetMaxHealth());
    }

    private void HandlePlayerDied()
    {
        ShowDeathScreen();
    }

    public void ShowDeathScreen()
    {
        if (deathScreen != null)
        {
            deathScreen.SetActive(true);
        }

        // Pause the game
        Time.timeScale = 0f;
    }

 
    public void HideDeathScreen()
    {
        if (deathScreen != null)
        {
            deathScreen.SetActive(false);
        }

        Time.timeScale = 1f;
    }

    public void RestartScene()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void CreateDefaultDeathScreen()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        GameObject canvasGO;
        if (canvas != null)
        {
            canvasGO = canvas.gameObject;
        }
        else
        {
            canvasGO = new GameObject("Canvas");
            canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();
        }

        GameObject panel = new GameObject("DeathScreen");
        panel.transform.SetParent(canvasGO.transform, false);
        RectTransform rt = panel.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        Image img = panel.AddComponent<Image>();
        img.color = new Color(0f, 0f, 0f, 0.75f);

        GameObject txtGO = new GameObject("DeathText");
        txtGO.transform.SetParent(panel.transform, false);
        RectTransform txtRt = txtGO.AddComponent<RectTransform>();
        txtRt.anchorMin = new Vector2(0.1f, 0.4f);
        txtRt.anchorMax = new Vector2(0.9f, 0.6f);
        txtRt.offsetMin = Vector2.zero;
        txtRt.offsetMax = Vector2.zero;

        TextMeshProUGUI tmp = txtGO.AddComponent<TextMeshProUGUI>();
        tmp.text = "Press SPACE to restart";
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontSize = 36;
        tmp.color = Color.white;

        panel.SetActive(false);

        deathScreen = panel;
    }
}
