using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySystem : MonoBehaviour
{
    public static InventorySystem Instance { get; set; } // Singleton erişim

    public GameObject inventoryScreenUI; // Envanter ekranı (UI)

    public List<InventorySlot> slotList = new List<InventorySlot>(); // Tüm slotların listesi
    public List<string> itemList = new List<string>(); // Envanterdeki eşya isimleri

    private GameObject itemToAdd;
    private InventorySlot whatSlotToEquip;

    public bool isOpen; // Envanter UI açık mı

    // Pickup (eşya alma) bildirimi UI
    public GameObject pickupAlert;
    public Text pickupName;
    public Image pickupImage;
    public GameObject ItemInfoUi;

    public List<string> itemsPickedUp; // Oyuncunun topladığı eşya isimleri listesi

    public int stackLimit = 3; // Aynı türden maksimum eşya adedi (stack)

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

    void Start()
    {
        isOpen = false;

        PopulateSlotList();  // UI'daki slotları listeye al
        ReCalculateList();   // Slotlardaki item'lara göre item listesi oluştur

        // ItemInfoUi.SetActive(true); // Gerekirse aktif edilebilir
        Cursor.visible = true;
    }

    private void PopulateSlotList()
    {
        // Envanter ekranındaki "Slot" tag'ine sahip nesneleri listeye ekle
        foreach (Transform child in inventoryScreenUI.transform)
        {
            if (child.CompareTag("Slot"))
            {
                InventorySlot slot = child.GetComponent<InventorySlot>();
                slotList.Add(slot);
            }
        }
    }

    void Update()
    {
        // I tuşuna basıldığında envanter aç/kapat işlemleri
        if (Input.GetKeyDown(KeyCode.I) && !isOpen && ConstructionManager.Instance.inConstructionMode == false)
        {
            inventoryScreenUI.SetActive(true);

            // Yeni menü sistemine geçildiği için sıralama ayarı yorum satırına alınmış
            // inventoryScreenUI.GetComponentInParent<Canvas>().sortingOrder = MenuManager.Instance.SetAsFront();

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            SelectionManager.Instance.DisabledSelection();
            SelectionManager.Instance.GetComponent<SelectionManager>().enabled = false;

            isOpen = true;
            ReCalculateList(); // Güncel eşyalarla listeyi yenile
        }
        else if (Input.GetKeyDown(KeyCode.I) && isOpen)
        {
            inventoryScreenUI.SetActive(false);

            if (CraftingSystem.Instance.isOpen == false)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;

                SelectionManager.Instance.DisabledSelection();
                SelectionManager.Instance.GetComponent<SelectionManager>().enabled = true;
            }

            isOpen = false;
            SelectionManager.Instance.centerDotImage.enabled = true;
        }
    }
    // Envantere yeni bir eşya ekler
    public void AddToInventory(string itemName, bool shouldStack)
    {
        // Eğer eşya zaten varsa ve yığın yapılacaksa o slota ekle
        InventorySlot stack = CheckIfStackExists(itemName);

        if (stack != null && shouldStack)
        {
            stack.itemInSlot.amountInInventory += 1;
            stack.UpdateItemInSlot();
        }
        else
        {
            GameObject itemPrefab = Resources.Load<GameObject>(itemName);
            if (itemPrefab == null)
            {
                Debug.LogError($"[AddToInventory] Prefab bulunamadı: {itemName}");
                return;
            }

            whatSlotToEquip = FindNextEmptySlot();

            itemToAdd = Instantiate(itemPrefab, whatSlotToEquip.transform.position, whatSlotToEquip.transform.rotation);
            itemToAdd.transform.SetParent(whatSlotToEquip.transform);

            whatSlotToEquip.itemInSlot = itemToAdd.GetComponent<InventoryItem>();

            itemList.Add(itemName); // Genel eşya listesine ekle
        }

        // Eğer şu an oyun yüklenmiyorsa (kayıttan), pickup efekti göster
        if (!SaveManager.Instance.isLoading)
        {
            SoundManager.Instance.PlaySound(SoundManager.Instance.pickUpItemSound);

            if (itemToAdd != null)
            {
                var image = itemToAdd.GetComponent<Image>();
                if (image != null)
                    TriggerPickupPop(itemName, image.sprite);
                else
                    Debug.LogWarning($"[AddToInventory] '{itemName}' prefabında Image bileşeni yok.");
            }
            else
            {
                Debug.LogWarning($"[AddToInventory] itemToAdd null: {itemName}");
            }
        }

        ReCalculateList();                          // Envanter verilerini güncelle
        CraftingSystem.Instance.RefreshNeedItems(); // Craft sistemi için güncelle
        QuestManager.Instance.RefreshTrackerList(); // Görev takip listesini güncelle
    }

    // Eşyanın yığın olarak eklenebileceği uygun bir slot var mı kontrol eder
    private InventorySlot CheckIfStackExists(string itemName)
    {
        foreach (InventorySlot inventorySlot in slotList)
        {
            inventorySlot.UpdateItemInSlot();

            if (inventorySlot != null && inventorySlot.itemInSlot != null)
            {
                if (inventorySlot.itemInSlot.thisName == itemName &&
                    inventorySlot.itemInSlot.amountInInventory < stackLimit)
                {
                    return inventorySlot;
                }
            }
        }
        return null;
    }

    // Eşya toplandığında üstte görünen uyarıyı tetikler
    void TriggerPickupPop(string itemName, Sprite itemSprite)
    {
        pickupAlert.SetActive(true);
        pickupName.text = itemName;
        pickupImage.sprite = itemSprite;

        StartCoroutine(HidePickupPopAfterDelay(3f));
    }

    // Pickup bildirimi belirli bir süre sonra kapanır
    IEnumerator HidePickupPopAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        pickupAlert.SetActive(false);
    }

    // İlk boş slotu bulur
    private InventorySlot FindNextEmptySlot()
    {
        foreach (InventorySlot slot in slotList)
        {
            // Bazı slotlarda metin objesi olabileceği için 1 veya daha az çocuk aranır
            if (slot.transform.childCount <= 1)
            {
                return slot;
            }
        }

        return new InventorySlot(); // Boş slot bulunamazsa yeni bir slot döner (uyarı amaçlı)
    }

    // Belirli sayıda boş slot var mı kontrol eder
    public bool CheckSlotAvailable(int emptyNeeded)
    {
        int emptySlot = 0;

        foreach (InventorySlot slot in slotList)
        {
            if (slot.transform.childCount <= 1)
            {
                emptySlot += 1;
            }
        }

        return emptySlot >= emptyNeeded;
    }
    // Envanterden istenilen miktarda eşya siler
    public void RemoveItem(string itemName, int amountToRemove)
    {
        int remainingAmountToRemove = amountToRemove;

        while (remainingAmountToRemove > 0)
        {
            int previousRemainingAmount = remainingAmountToRemove;

            foreach (InventorySlot slot in slotList)
            {
                if (slot.itemInSlot != null && slot.itemInSlot.thisName == itemName)
                {
                    if (slot.itemInSlot.amountInInventory > 0)
                    {
                        // 1 adet sil
                        slot.itemInSlot.amountInInventory -= 1;
                        remainingAmountToRemove -= 1;

                        // Eğer envanterdeki eşya tükendiyse sahneden sil
                        if (slot.itemInSlot.amountInInventory <= 0)
                        {
                            Destroy(slot.itemInSlot.gameObject);
                            slot.itemInSlot = null;
                        }

                        break; // Bir eşya bulunduğunda döngüden çık
                    }
                }
            }

            // Eğer döngüde hiç eşya bulunmadıysa dur
            if (previousRemainingAmount == remainingAmountToRemove)
            {
                Debug.Log("item not found or insufficient quantity in inventory");
                break;
            }

            // Güncellemeleri yap
            ReCalculateList();
            CraftingSystem.Instance.RefreshNeedItems();
            QuestManager.Instance.RefreshTrackerList();
        }
    }

    // itemList listesini slotlardan güncelleyerek yeniden oluşturur
    public void ReCalculateList()
    {
        itemList.Clear();

        foreach (InventorySlot inventorySlot in slotList)
        {
            if (inventorySlot.GetComponent<InventorySlot>())
            {
                InventoryItem item = inventorySlot.itemInSlot;
                if (item != null)
                {
                    if (item.amountInInventory > 0)
                    {
                        for (int i = 0; i < item.amountInInventory; i++)
                        {
                            itemList.Add(item.thisName); // item ismi kadar listeye eklenir
                        }
                    }
                }
            }
        }
    }

    // Belirli bir eşya türünden envanterde kaç tane olduğunu döndürür
    public int CheckItemAmount(string name)
    {
        int itemCounter = 0;

        foreach (string item in itemList)
        {
            if (item == name)
            {
                itemCounter += 1;
            }
        }

        return itemCounter;
    }

    // Belirli bir eşya türünden sadece 1 adet siler
    public void RemoveOneItem(string itemName)
    {
        foreach (InventorySlot slot in slotList)
        {
            if (slot.itemInSlot != null && slot.itemInSlot.thisName == itemName)
            {
                slot.itemInSlot.amountInInventory -= 1;

                if (slot.itemInSlot.amountInInventory <= 0)
                {
                    Destroy(slot.itemInSlot.gameObject);
                    slot.itemInSlot = null;
                }

                ReCalculateList();
                CraftingSystem.Instance.RefreshNeedItems();
                QuestManager.Instance.RefreshTrackerList();
                break;
            }
        }
    }
}
