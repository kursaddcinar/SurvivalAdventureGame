using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Oyunun genel kaynak sağlık durumlarını tutmak için kullanılan singleton sınıf.
// Örneğin, oyuncunun odun kesme işlemi sırasında ağacın kalan canını göstermek gibi amaçlar için kullanılabilir.
public class GlobalState : MonoBehaviour
{
    // Singleton Instance tanımı
    public static GlobalState Instance { get; set; }

    // Kaynağın (örneğin ağaç) mevcut canı
    public float resourceHealth;

    // Kaynağın maksimum can değeri
    public float resourceMaxHealth;

    // Singleton yapı kurulumu
    public void Awake()
    {
        // Eğer başka bir örnek varsa bu nesneyi yok et
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
}
