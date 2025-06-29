using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    public static PlayerState Instance { get; set; }

    // ----- Sağlık
    public float currentHealth;
    public float maxHealth;

    // ----- Kalori
    public float currentCalories;
    public float maxCalories;

    float distanceTravalled = 0;
    Vector3 lastPosition;

    public GameObject playerBody;

    // ----- Su (Hidrasyon)
    public float currentHydrationPercent;
    public float maxHydrationPercent;
    public bool isHydrationActive;

    // ----- Skor
    public int currentPoints;

    private float hydrationDamageTimer = 0f;

    //tömürün indeksi
    public int hasTriggeredTomurIntro = 0;
    public bool gameIsCompleted = false;
    private void Awake()
    {
        // Singleton kontrolü
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        // Eğer kayıt yüklenmemişse varsayılan değerleri ata
        if (currentHealth == 0) currentHealth = maxHealth;
        if (currentCalories == 0) currentCalories = maxCalories;
        if (currentHydrationPercent == 0) currentHydrationPercent = maxHydrationPercent;

        // Zamanla hidrasyon azalması
        StartCoroutine(decreaseHydration());

        //zamanla canın yükselmesi
        StartCoroutine(regenerateHealthOverTime()); 
    }

    IEnumerator decreaseHydration()
    {
        while (true)
        {
            currentHydrationPercent -= 1;
            yield return new WaitForSeconds(10);
        }
    }
    IEnumerator regenerateHealthOverTime()
    {
        while (true)
        {
            yield return new WaitForSeconds(20f); // Her 20 saniyede bir çalışır

            // Maksimum canı aşmaması için kontrol
            if (currentHealth < maxHealth)
            {
                currentHealth = Mathf.Min(currentHealth + 1, maxHealth);
            }
        }
    }


    void Update()
    {
        // Yürüme sonucu kalori tüketimi
        distanceTravalled += Vector3.Distance(playerBody.transform.position, lastPosition);
        lastPosition = playerBody.transform.position;

        if (distanceTravalled >= 6)
        {
            distanceTravalled = 0;
            currentCalories -= 1;
        }

        // Test amaçlı tuşla statüsünü düşür
        if (Input.GetKeyDown(KeyCode.N))
        {
            currentHealth -= 10;
            currentCalories -= 300;
            currentHydrationPercent -= 20;
        }

        // Ölüm kontrolü
        if (currentHealth <= 0)
        {
            DeathManager.Instance.ShowDeathPanel();
        }

        // Su bitince her 4 saniyede bir can kaybı
        if (currentHydrationPercent <= 0)
        {
            hydrationDamageTimer += Time.deltaTime;
            if (hydrationDamageTimer >= 4f)
            {
                setHealth(currentHealth - 1f);
                hydrationDamageTimer = 0f;
            }
        }
    }

    // Statü güncelleme fonksiyonları
    public void setHealth(float inputHealth) => currentHealth = inputHealth;
    public void setCalories(float inputCalories) => currentCalories = inputCalories;
    public void setHydration(float inputHydration) => currentHydrationPercent = inputHydration;

    public void SetPoints(int value)
    {
        currentPoints = value;
        PointManager.Instance.UpdatePointUI();
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log(currentHealth <= 0 ? "Player is dead" : "Player is hurt");
    }

    public void IncreaseCalories(int amount) => currentCalories -= amount;
    public void IncreaseHydration(int amount) => currentHydrationPercent -= amount;

    public int GetPoints() => currentPoints;
}
