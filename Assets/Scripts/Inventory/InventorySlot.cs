using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class InventorySlot : MonoBehaviour
{
    public TextMeshProUGUI amountTXT;   // Eşya adedini gösteren UI yazı nesnesi
    public InventoryItem itemInSlot;    // Bu slotta bulunan eşya

    private void Update()
    {
        // Slotta bir eşya var mı kontrol et
        InventoryItem item = CheckInventoryItem();

        // Eğer eşya varsa referansı güncelle
        if (item != null)
        {
            itemInSlot = item;
        }
        else
        {
            itemInSlot = null;
        }

        // Eşya varsa miktar göster, yoksa gizle
        if (itemInSlot != null)
        {
            amountTXT.gameObject.SetActive(true); // Miktar yazısı aktif hale getirilir
            amountTXT.text = $"{itemInSlot.amountInInventory}"; // Adet yazısı güncellenir
            amountTXT.transform.SetAsLastSibling(); // Yazı en üst katmana çekilir
        }
        else
        {
            amountTXT.gameObject.SetActive(false); // Eşya yoksa yazı gizlenir
        }
    }

    // Slot içindeki eşyayı kontrol eder (ilk InventoryItem bileşenini döndürür)
    public InventoryItem CheckInventoryItem()
    {
        foreach (Transform child in transform)
        {
            if (child.GetComponent<InventoryItem>())
            {
                return child.GetComponent<InventoryItem>();
            }
        }
        return null;
    }

    // Elle eşya kontrolü yapılmak istenirse bu fonksiyon kullanılabilir
    public void UpdateItemInSlot()
    {
        itemInSlot = CheckInventoryItem();
    }
}
