using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Oyuncunun sağlık durumunu gösteren sağlık barı UI bileşeni
public class HealthBar : MonoBehaviour
{
    private Slider slider; // Sağlık barı olarak kullanılan Slider UI bileşeni
    public Text healthCounter; // Sağlık değerini (örn: 80/100) metin olarak gösteren UI bileşeni

    public GameObject playerState; // Oyuncunun sağlık bilgilerini içeren nesne (PlayerState.cs)

    private float currentHealth, maxHealth; // Mevcut ve maksimum sağlık değerleri

    void Awake()
    {
        // Bu bileşenin üzerinde bulunan Slider komponentini al
        slider = GetComponent<Slider>();
    }

    void Update()
    {
        // Her frame'de oyuncunun sağlık bilgilerini güncelle
        currentHealth = playerState.GetComponent<PlayerState>().currentHealth;
        maxHealth = playerState.GetComponent<PlayerState>().maxHealth;

        // Slider'ın doluluk oranını güncelle (örn: 0.8)
        float fillValue = currentHealth / maxHealth;
        slider.value = fillValue;

        // Metin olarak da sayısal sağlık durumunu göster (örn: 80/100)
        healthCounter.text = currentHealth + "/" + maxHealth;
    }
}
