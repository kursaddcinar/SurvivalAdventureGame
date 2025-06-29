using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Oyuncunun su seviyesini (hidrasyonunu) UI üzerinden takip etmek için kullanılan sınıf
public class HydrationBar : MonoBehaviour
{
    private Slider slider; // Unity UI'daki slider bileşeni (dolu/boş çubuk)
    public Text hydrationCounter; // Yüzdelik sayıyı gösteren metin

    public GameObject playerState; // PlayerState bileşenini tutan nesne

    private float currentHydration, maxHydration;

    void Awake()
    {
        // Slider bileşenini al
        slider = GetComponent<Slider>();
    }

    void Update()
    {
        // Oyuncunun mevcut ve maksimum hidrasyon verilerini al
        currentHydration = playerState.GetComponent<PlayerState>().currentHydrationPercent;
        maxHydration = playerState.GetComponent<PlayerState>().maxHydrationPercent;

        // Slider değerini ayarla (0.0f - 1.0f arası)
        float fillValue = currentHydration / maxHydration;
        slider.value = fillValue;

        // Yüzdelik olarak metni güncelle
        hydrationCounter.text = currentHydration + "%";
    }
}
