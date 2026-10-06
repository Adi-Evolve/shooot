using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int health = 100;
    public AudioClip hitSound;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Damage"))
        {
            DecreaseHealth(10);
        }
    }

    private void DecreaseHealth(int deacreaseAmount)
    {
        health -= deacreaseAmount;
        PlayerLook.Instance.AddShake(0.2f, 0.25f);
        UIManager.Instance.InvokeHitUI();
        AudioManager.Instance.PlaySFX(hitSound);
        UIManager.Instance.SetHealthValue(health);
        if (health <= 0)
        {
            Die();
        }
    }
    private void Die()
    {
        Time.timeScale = 0f;
        UIManager.Instance.DeathUI();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
