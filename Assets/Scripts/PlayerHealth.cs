using UnityEngine;
using UnityEngine.UI; // Slider için gerekli

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;

    public Slider healthSlider; // Buraya Unity'deki Slider'ı sürükleyeceksin

    void Start()
    {
        currentHealth = maxHealth;
        
        // Slider ayarlarını yapalım
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log($"Canın azaldı! Kalan: {currentHealth}");

        // Slider'ı güncelle
        if (healthSlider != null)
        {
            healthSlider.value = currentHealth;
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Oyuncu Öldü!");
        // Buraya ölüm animasyonu veya oyun bitiş ekranı kodları gelecek
        // gameObject.SetActive(false); 
    }
}