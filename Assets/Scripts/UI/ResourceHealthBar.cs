using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Bu sınıf, oyundaki belirli bir kaynak nesnesinin sağlık durumunu gösteren UI barını kontrol eder.
public class ResourceHealthBar : MonoBehaviour
{
    private Slider slider; // Slider bar komponenti (dolu-boş çubuğu)
    private float currentHealth, maxHealth;

    public GameObject globalState; // GlobalState objesine referans (kaynak sağlığı buradan alınır)

    private void Awake()
    {
        slider = GetComponent<Slider>(); // Bu GameObject üzerindeki Slider bileşeni alınır
    }

    private void Update()
    {
        // GlobalState içindeki kaynak sağlık değerleri alınır
        currentHealth = globalState.GetComponent<GlobalState>().resourceHealth;
        maxHealth = globalState.GetComponent<GlobalState>().resourceMaxHealth;

        // Barın doluluk oranı hesaplanarak Slider’a aktarılır
        float fillValue = currentHealth / maxHealth;
        slider.value = fillValue;
    }
}
