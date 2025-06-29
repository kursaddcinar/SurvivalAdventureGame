using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.PlayerLoop;

public class Animal : MonoBehaviour
{
    public string animalName; // Hayvanın adı
    public bool playerInRange; // Oyuncu yakınında mı

    [SerializeField] float currentHealth; // Mevcut can
    [SerializeField] float maxHealth;     // Maksimum can

    [Header("Sounds")]
    [SerializeField] AudioSource soundChannel; // Ses çıkışı
    [SerializeField] AudioClip animalHitAndScream; // Yaralanma sesi
    [SerializeField] AudioClip animalHitAndDie;    // Ölüm sesi
    [SerializeField] AudioClip animalAttack;       // Saldırı sesi

    private Animator animator;
    public bool isDead;

    [SerializeField] ParticleSystem bloodSplashParticles; // Kan sıçraması efekti
    public GameObject bloodPuddle; // Kan birikintisi objesi

    public Slider healthBarSlider; // Can çubuğu

    enum AnimalType
    {
        Rabbit,
        Bear,
        Wolf,
        Puma,
        Leopar,
        Elephant
    }

    [SerializeField] AnimalType thisAnimalType; // Bu hayvanın türü
    [SerializeField] private int attackDamage = 1; // Hayvanın vereceği hasar

    private void Start()
    {
        currentHealth = maxHealth;
        animator = GetComponent<Animator>();
        UpdateHealthBar();

        switch (thisAnimalType)
        {
           
            case AnimalType.Wolf:
                attackDamage = 5;
                break; 
            case AnimalType.Bear:
                attackDamage = 7;
                break;
            default:
                attackDamage = 18;
                break;
        }
    
    }

    private void Update()
    {
        // Can çubuğunu sürekli güncelle
        healthBarSlider.value = currentHealth / maxHealth;
    }

    private void UpdateHealthBar()
    {
        if (healthBarSlider != null)
        {
            healthBarSlider.value = currentHealth / maxHealth;
        }
    }

    public void TakeDamage(int damage)
    {
        UpdateHealthBar();

        if (!isDead)
        {
            currentHealth -= damage;
            bloodSplashParticles.Play();

            if (currentHealth <= 0 && !isDead)
            {
                PlayDyingSound();
                isDead = true;

                animator.SetTrigger("DIE");

                // Ölünce puan ver
                PointManager pointManager = FindObjectOfType<PointManager>();
                if (pointManager != null)
                {
                    int pointValue = -1;
                    if (thisAnimalType == AnimalType.Rabbit) pointValue = 1;
                    else if (thisAnimalType == AnimalType.Bear) pointValue = 4;
                    else if (thisAnimalType == AnimalType.Wolf) pointValue = 3;
                    else if (thisAnimalType == AnimalType.Elephant) pointValue = 9;
                    else pointValue = 1;

                    pointManager.AddPoints(pointValue);
                }

                StartCoroutine(PuddleDelay());
                Die(); // Ölme işlemini başlat
            }
            else
            {
                PlayHitSound();
                animator.SetTrigger("HURT");
            }
        }
    }

    IEnumerator PuddleDelay()
    {
        yield return new WaitForSeconds(1f);
        bloodPuddle.SetActive(true); // Kan birikintisi bir süre sonra açılır
    }

    private void PlayDyingSound()
    {
        soundChannel.PlayOneShot(animalHitAndDie);
    }

    private void PlayHitSound()
    {
        soundChannel.PlayOneShot(animalHitAndScream);
    }

    public void PlayAttackSound()
    {
        soundChannel.PlayOneShot(animalAttack);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("activeConstructable"))
        {
            playerInRange = true;
            healthBarSlider.gameObject.SetActive(true); // Oyuncu yaklaşınca can çubuğu görünür
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("activeConstructable"))
        {
            playerInRange = false;
            healthBarSlider.gameObject.SetActive(false); // Oyuncu uzaklaşınca çubuk gizlenir
        }
    }

    public void Die()
    {

        // Hareketi durdur
        AI_Movement ai = GetComponent<AI_Movement>();
        if (ai != null) ai.enabled = false;

        // Collider kapatılır (etkileşim kapanır)
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        // Can çubuğu kapanır
        if (healthBarSlider != null) healthBarSlider.gameObject.SetActive(false);
    }


    public int GetAttackDamage()
    {
        return attackDamage;
    }

}
