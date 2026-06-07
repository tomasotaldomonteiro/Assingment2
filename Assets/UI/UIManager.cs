using UnityEngine;
// using UnityEngine.UI; // no longer used because we only show text
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    
    [Header("Health UI")]
    public TextMeshProUGUI healthText; // Text to display current health number
    [Tooltip("Value to show on the UI before the player is found")]
    public float initialHealth = 100f;

    [Header("Death UI")]
    [Tooltip("Assign a UI panel (GameObject) to show when the player dies")]
    public GameObject deathScreen;
    
    private PlayerController playerController;
    
    void Awake()
    {
        // Singleton pattern
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // Ensure the UI shows a sensible initial health value immediately
        if (healthText != null)
        {
            healthText.text = $"Health: {initialHealth:F0}";
        }

        // If no deathScreen assigned in the Inspector, create a default one at runtime
        if (deathScreen == null)
        {
            CreateDefaultDeathScreen();
        }
    }
    
    void Start()
    {
        // Find the player (safe lookup)
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            playerController = playerObj.GetComponent<PlayerController>();
        }

        if (playerController == null)
        {
            Debug.LogWarning("PlayerController not found at UIManager.Start. Health text will update when player becomes available.");
        }

        // Initialize health display: show player's actual health if available, otherwise show the configured initial value
        if (playerController != null)
        {
            UpdateHealthDisplay();
        }
        else
        {
            if (healthText != null)
                healthText.text = $"Health: {initialHealth:F0}";
            else
                Debug.Log($"Initial player health: {initialHealth:F0}");
        }
    }

    void Update()
    {
        // If the playerController wasn't available at Start, try to find it each frame until found
        if (playerController == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                playerController = playerObj.GetComponent<PlayerController>();
                if (playerController != null)
                {
                    UpdateHealthDisplay();
                }
            }
        }

        // If death screen active and player presses Space, restart
        if (deathScreen != null && deathScreen.activeSelf)
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                RestartScene();
            }
        }
    }
    

    public void UpdateHealthDisplay()
    {
        if (playerController == null) return;

        float currentHealth = playerController.GetHealth();

        // Update health text to show only the number (rounded)
        if (healthText != null)
        {
            healthText.text = $"Health: {currentHealth:F0}";
        }
        else
        {
            Debug.Log($"Player health: {currentHealth:F0}");
        }
    }

    public void SetHealthNumber(float health)
    {
        if (healthText != null)
        {
            healthText.text = $"Health: {health:F0}";
        }
        else
        {
            Debug.Log($"Player health (forced): {health:F0}");
        }
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
        // Try to find existing Canvas
        Canvas canvas = FindObjectOfType<Canvas>();
        GameObject canvasGO;
        if (canvas != null)
        {
            canvasGO = canvas.gameObject;
        }
        else
        {
            // Create a new Canvas
            canvasGO = new GameObject("Canvas");
            canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGO.AddComponent<CanvasScaler>();
            canvasGO.AddComponent<GraphicRaycaster>();
        }

        // Create panel
        GameObject panel = new GameObject("DeathScreen");
        panel.transform.SetParent(canvasGO.transform, false);
        RectTransform rt = panel.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        Image img = panel.AddComponent<Image>();
        img.color = new Color(0f, 0f, 0f, 0.75f); // black with 75% opacity

        // Create centered TextMeshProUGUI
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

        // Ensure it's inactive by default
        panel.SetActive(false);

        deathScreen = panel;
    }
}

