using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Oyuncunun kalori seviyesini ekranda gösteren arayüz bileşenidir
public class CaloriesBar : MonoBehaviour
{
    private Slider slider; // Dolu-boş bar kontrolü için referans
    public Text caloriesCounter; // Sayısal gösterim (örn: 350/500)

    public GameObject playerState; // Oyuncunun sağlık/veri durumlarını tutan nesneye referans

    private float currentCalories, maxCalories;

    void Awake()
    {
        slider = GetComponent<Slider>(); // Bu bileşene bağlı Slider nesnesi alınır
    }

    void Update()
    {
        // Oyuncunun güncel ve maksimum kalori bilgileri alınır
        currentCalories = playerState.GetComponent<PlayerState>().currentCalories;
        maxCalories = playerState.GetComponent<PlayerState>().maxCalories;

        // Slider barı doldurulacak oran hesaplanır (0-1 arası float)
        float fillValue = currentCalories / maxCalories;
        slider.value = fillValue;

        // Sayısal değer text’e yazılır
        caloriesCounter.text = currentCalories + "/" + maxCalories; // örnek: 320/500
    }
}
