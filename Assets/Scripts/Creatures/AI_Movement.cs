using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_Movement : MonoBehaviour
{
    Animator animator; // Canlının animasyonlarını kontrol eden bileşen

    public float moveSpeed = 0.2f; // Yürüme hızı

    Vector3 stopPosition; // Durduğu konumu hatırlamak için

    float walkTime; // Yürüme süresi
    public float walkCounter; // Yürümeye kalan zaman
    float waitTime; // Bekleme süresi
    public float waitCounter; // Beklemeye kalan zaman

    int WalkDirection; // 0: ileri, 1: sağ, 2: sol, 3: geri

    public bool isWalking; // Şu anda yürüyor mu

    void Start()
    {
        animator = GetComponent<Animator>();

        // Tüm prefabların aynı anda yürümesini ve durmasını önlemek için zamanlar rastgele atanır
        walkTime = Random.Range(3, 6);
        waitTime = Random.Range(5, 7);

        waitCounter = waitTime;
        walkCounter = walkTime;

        ChooseDirection(); // Başlangıçta bir yön seçilir
    }

    void Update()
    {
        if (isWalking)
        {
            animator.SetBool("isRunning", true); // Yürüme animasyonu başlatılır

            walkCounter -= Time.deltaTime; // Yürüme süresi azalır

            // Yöne göre rotasyon verilip ileri doğru hareket ettirilir
            switch (WalkDirection)
            {
                case 0:
                    transform.localRotation = Quaternion.Euler(0f, 0f, 0f);
                    transform.position += transform.forward * moveSpeed * Time.deltaTime;
                    break;
                case 1:
                    transform.localRotation = Quaternion.Euler(0f, 90, 0f);
                    transform.position += transform.forward * moveSpeed * Time.deltaTime;
                    break;
                case 2:
                    transform.localRotation = Quaternion.Euler(0f, -90, 0f);
                    transform.position += transform.forward * moveSpeed * Time.deltaTime;
                    break;
                case 3:
                    transform.localRotation = Quaternion.Euler(0f, 180, 0f);
                    transform.position += transform.forward * moveSpeed * Time.deltaTime;
                    break;
            }

            // Yürüme süresi bittiğinde durma işlemleri yapılır
            if (walkCounter <= 0)
            {
                stopPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
                isWalking = false;

                transform.position = stopPosition; // Hareket durdurulur
                animator.SetBool("isRunning", false); // Animasyon durdurulur

                waitCounter = waitTime; // Bekleme süresi başlatılır
            }
        }
        else
        {
            waitCounter -= Time.deltaTime; // Bekleme süresi azalır

            // Bekleme süresi bittiğinde yeni bir yön seçilir
            if (waitCounter <= 0)
            {
                ChooseDirection();
            }
        }
    }

    public void ChooseDirection()
    {
        // Rastgele bir yön seçilir (0-3)
        WalkDirection = Random.Range(0, 4);

        isWalking = true;
        walkCounter = walkTime;
    }
}
