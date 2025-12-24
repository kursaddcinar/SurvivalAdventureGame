using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))] // Bu scriptin bağlı olduğu objede BoxCollider olmasını zorunlu kılar
public class ChoppableTree : MonoBehaviour
{
    public bool playerInRange;      // Oyuncu ağaca yakın mı
    public bool canBeChopped;       // Ağaç şu anda kesilebilir mi

    public float treeHealth;        // Ağacın mevcut canı
    public float treeMaxHealth;     // Ağacın maksimum canı

    public Animator animator;       // Ağaç için kullanılacak animasyon (örneğin sallanma)

    public float caloriesSpentChoppingWood = 20; // Bir vuruşta harcanacak kalori miktarı

    private void Start()
    {
        // Ağaç canı başta sabit bir değerle başlatılmış
        treeHealth = 10;
        treeMaxHealth = 10;

        // Animator bileşeni ağacın en üst parent'ından alınır
        animator = transform.parent.transform.parent.GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Oyuncu yaklaştığında işaretle
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Oyuncu uzaklaştığında işareti kaldır
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    public void GetHit()
    {
        // Ağaç sallanma animasyonu oynatır
        animator.SetTrigger("shake");

        // Can azaltılır ve oyuncunun kalorisi düşürülür
        treeHealth -= 1;
        PlayerState.Instance.currentCalories -= caloriesSpentChoppingWood;

        // Can 0 veya altına düşerse ağaç kesilmiş sayılır
        if (treeHealth <= 0)
        {
            treeIsDead();
        }
    }

    // Gerekiyorsa animasyon gibi işlemlerle bekleme süresi tanımlanabilir
    public IEnumerator hit()
    {
        yield return new WaitForSeconds(0.6f);
    }

    void treeIsDead()
    {
        Vector3 treePosition = transform.position;

        // Ağaç prefab'ı sahneden kaldırılır
        Destroy(transform.parent.transform.parent.gameObject);
        canBeChopped = false;

        // Seçim sistemi temizlenir
        SelectionManager.Instance.selectedTree = null;
        SelectionManager.Instance.chopHolder.gameObject.SetActive(false);

        // Kesilmiş ağaç prefab'ı sahneye eklenir
        GameObject brokenTree = Instantiate(
            Resources.Load<GameObject>("ChoppedTree"),
            new Vector3(treePosition.x, treePosition.y + 1.5f, treePosition.z + 0.5f),
            Quaternion.Euler(0, 0, 0)
        );

        // Yeni ağacın parent'ı sahnede uygun yapıya atanır
        brokenTree.transform.SetParent(transform.parent.transform.parent.parent);
    }

    private void Update()
    {
        // Eğer kesilebilir durumdaysa, global sağlık bilgileri güncellenir
        if (canBeChopped)
        {
            GlobalState.Instance.resourceHealth = treeHealth;
            GlobalState.Instance.resourceMaxHealth = treeMaxHealth;
        }
    }
}
