using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;
    public GameObject hitUI;
    public GameObject deathUI;
    public TextMeshProUGUI ammoText;
    public Image healthBar;
    public Gradient healthGradient;

    private void Awake()
    {
        Time.timeScale = 1f;
        Instance = this;
    }

    public void InvokeHitUI()
    {
        Instantiate(hitUI, transform);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void DeathUI()
    {
        deathUI.SetActive(true);
    }

    public void SetHealthValue(int health)
    {
        float floatHealth = (float)health /100;
        healthBar.color =healthGradient.Evaluate(floatHealth);
        healthBar.fillAmount = floatHealth;
    }
}
