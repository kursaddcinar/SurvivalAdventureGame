using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageManager : MonoBehaviour
{
    public static StorageManager Instance { get; set; }

    [SerializeField] GameObject storageBoxSmallUI;      // Küçük sandığın UI paneli
    //[SerializeField] StorageBox selectedStorage;        // Şu anda etkileşimde olunan sandık
    public bool storageUIOpen;                          // UI açık mı kontrolü
    public StorageBox selectedStorage;


    private void Awake()
    {
        // Singleton yapısı
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    // Sandık arayüzünü aç
    public void OpenBox(StorageBox storage)
    {
        SetSelectedStorage(storage);

        PopulateStorage(GetRelevantUI(selectedStorage)); // UI'yi doldur

        GetRelevantUI(selectedStorage).SetActive(true);  // UI panelini aç
        storageUIOpen = true;

        Cursor.lockState = CursorLockMode.None;          // İmleç görünür ve serbest olur
        Cursor.visible = true;

        // Seçim sistemi devre dışı bırakılır
        SelectionManager.Instance.DisabledSelection();
        SelectionManager.Instance.GetComponent<SelectionManager>().enabled = false;
    }

    // Sandık içeriğini UI slotlarına yerleştir
    private void PopulateStorage(GameObject storageUI)
    {
        List<GameObject> uiSlots = new List<GameObject>();

        foreach (StoredItemData data in selectedStorage.items)
        {
            foreach (GameObject slot in uiSlots)
            {
                if (slot.transform.childCount < 1)
                {
                    var itemToAdd = Instantiate(
                        Resources.Load<GameObject>(data.itemName),
                        slot.transform.position,
                        slot.transform.rotation
                    );

                    itemToAdd.name = data.itemName;
                    itemToAdd.transform.SetParent(slot.transform);
                    itemToAdd.transform.localPosition = Vector3.zero;

                    InventoryItem inv = itemToAdd.GetComponent<InventoryItem>();
                    if (inv != null)
                    {
                        inv.amountInInventory = data.amount;
                    }
                    break;
                }
            }
        }

    }

    // Sandığı kapat
    public void CloseBox()
    {
        RecalculaTeStorage(GetRelevantUI(selectedStorage)); // Yeni veriler alınır

        GetRelevantUI(selectedStorage).SetActive(false); // UI gizlenir
        storageUIOpen = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Seçim sistemi yeniden aktif edilir
        SelectionManager.Instance.EnabledSelection();
        SelectionManager.Instance.GetComponent<SelectionManager>().enabled = true;
    }

    // Sandık içeriğini yeniden hesapla ve string listesine kaydet
    public void RecalculaTeStorage(GameObject storageUI)
    {
        List<GameObject> uiSlots = new List<GameObject>();

        selectedStorage.items.Clear();

        foreach (GameObject slot in uiSlots)
        {
            if (slot.transform.childCount > 0)
            {
                var item = slot.transform.GetChild(0).GetComponent<InventoryItem>();
                if (item != null)
                {
                    StoredItemData entry = new StoredItemData();
                    entry.itemName = item.thisName;
                    entry.amount = item.amountInInventory;

                    selectedStorage.items.Add(entry);
                }
            }
        }

    }

    // Seçilen sandık referansını güncelle
    public void SetSelectedStorage(StorageBox storage)
    {
        selectedStorage = storage;
    }

    // İleride farklı sandık tipleri için UI döndürme işlemleri yapılabilir
    public GameObject GetRelevantUI(StorageBox storage)
    {
        return storageBoxSmallUI;
    }
}
