using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// Bu sınıf, UI'da bir eşya slotuna eşya bırakıldığında (OnDrop) ne olacağını kontrol eder
public class ItemSlot : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        Debug.Log("OnDrop");

        // Eğer bu slot boşsa (hiç ya da sadece yazı gibi 1 çocuk varsa)
        if (transform.childCount <= 1)
        {
            // Bırakma sesi çal
            SoundManager.Instance.PlaySound(SoundManager.Instance.dropItemSound);

            // Eşya bu slotun altına taşınır
            DragDrop.itemBeingDragged.transform.SetParent(transform);
            DragDrop.itemBeingDragged.transform.localPosition = new Vector2(0, 0);

            // Slot türüne göre quickslot bilgisi güncellenir ve envanter hesaplaması yapılır
            if (!transform.CompareTag("QuickSlot"))
            {
                DragDrop.itemBeingDragged.GetComponent<InventoryItem>().isInsideQuickSlot = false;
                InventorySystem.Instance.ReCalculateList();
            }

            if (transform.CompareTag("QuickSlot"))
            {
                DragDrop.itemBeingDragged.GetComponent<InventoryItem>().isInsideQuickSlot = true;
                InventorySystem.Instance.ReCalculateList();
            }
        }
        else // Slot zaten doluysa
        {
            InventoryItem draggedItem = DragDrop.itemBeingDragged.GetComponent<InventoryItem>();

            // Eğer bırakılan eşya ve mevcut eşya aynı türse ve sınır aşılmıyorsa birleştir
            if (draggedItem.thisName == GetStoredItem().thisName && IsLimitExceded(draggedItem) == false)
            {
                GetStoredItem().amountInInventory += draggedItem.amountInInventory;
                DestroyImmediate(DragDrop.itemBeingDragged); // Sürüklenen nesne sahneden silinir
            }
            else
            {
                // Eğer birleştirilemiyorsa, yine de bu slota yerleştirilir
                DragDrop.itemBeingDragged.transform.SetParent(transform);
            }
        }
    }

    // Slottaki mevcut eşyayı getir
    InventoryItem GetStoredItem()
    {
        return transform.GetChild(0).GetComponent<InventoryItem>();
    }

    // Bırakılacak eşya ile slottaki eşya birleştirildiğinde sınırı aşıp aşmayacağını kontrol eder
    bool IsLimitExceded(InventoryItem draggedItem)
    {
        if ((draggedItem.amountInInventory + GetStoredItem().amountInInventory) > InventorySystem.Instance.stackLimit)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}
