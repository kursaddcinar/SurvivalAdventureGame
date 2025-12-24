using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Oyuncunun etkileşime girebileceği nesneleri temsil eder (örn. yerdeki lootlar)
public class InteractableObject : MonoBehaviour
{
    public bool playerInRange;      // Oyuncu bu nesnenin yakınında mı?
    public string ItemName;         // Envantere eklenecek eşyanın adı (Resources klasörüne göre)

    // Bu nesnenin ismini döndürür (dış sistemler için erişim)
    public string GetItemName()
    {
        return ItemName;
    }

    void Update()
    {
        // Sol tıkla etkileşim + Oyuncu yakında + Nesne seçili
        if (Input.GetKeyDown(KeyCode.Mouse0) &&
            playerInRange &&
            SelectionManager.Instance.onTarget &&
            SelectionManager.Instance.selectedObject == gameObject)
        {
            // Envanterde en az 1 boş slot varsa
            if (InventorySystem.Instance.CheckSlotAvailable(1))
            {

                // Eşyayı envantere ekle
                InventorySystem.Instance.AddToInventory(ItemName, true);

                // Kaydedilen eşyalar listesine ekle (kayıt sistemi için)
                InventorySystem.Instance.itemsPickedUp.Add(gameObject.name);

                // Sahnedeki objeyi yok et
                Destroy(gameObject);
            }
            else
            {
                Debug.Log("inventory is full"); // Envanter dolu uyarısı
            }
        }
    }

    // Oyuncu menzile girince tetiklenir
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    // Oyuncu menzilden çıkınca tetiklenir
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
