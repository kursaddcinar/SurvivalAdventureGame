using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    // --- Eşya çöpe atılabilir mi --- //
    public bool isTrashable;

    // --- Eşya bilgisi UI öğeleri --- //
    private GameObject itemInfoUI;
    private Text itemInfoUI_itemName;
    private Text itemInfoUI_itemDescription;
    private Text itemInfoUI_itemFunctionality;

    public string thisName, thisDescription, thisFunctionality;

    // --- Tüketilebilirlik (yemek, içecek vb.) --- //
    private GameObject itemPendingConsumption;
    public bool isConsumable;
    public float healthEffect;
    public float caloriesEffect;
    public float hydrationEffect;

    // --- Donatılabilirlik (equip) --- //
    public bool isEquippable;
    private GameObject itemPendingEquipping;
    public bool isInsideQuickSlot;

    public bool isSelected;     // Hızlı slotta seçili mi
    public bool isUseable;      // Özel işlevli eşya mı (örn. harita, fener)

    public int amountInInventory = 1; // Bu eşya türünden kaç tane var

    private void Start()
    {
        itemInfoUI = InventorySystem.Instance.ItemInfoUi;

        itemInfoUI_itemName = itemInfoUI.transform.Find("itemName").GetComponent<Text>();
        itemInfoUI_itemDescription = itemInfoUI.transform.Find("itemDescription").GetComponent<Text>();
        itemInfoUI_itemFunctionality = itemInfoUI.transform.Find("itemFunctionality").GetComponent<Text>();
    }

    void Update()
    {
        // Seçili item'lar sürüklenemez
        GetComponent<DragDrop>().enabled = !isSelected;
    }

    // Fare bu item'in üzerine geldiğinde tetiklenir
    public void OnPointerEnter(PointerEventData eventData)
    {
        itemInfoUI.SetActive(true);
        itemInfoUI_itemName.text = thisName;
        itemInfoUI_itemDescription.text = thisDescription;
        itemInfoUI_itemFunctionality.text = thisFunctionality;
    }

    // Fare bu item'den ayrıldığında tetiklenir
    public void OnPointerExit(PointerEventData eventData)
    {
        itemInfoUI.SetActive(false);
    }

    // Fare item'e tıklandığında tetiklenir
    public void OnPointerDown(PointerEventData eventData)
    {
        // Sağ tıklama
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (isConsumable)
            {

                // Tüketim için işaretle
                //itemPendingConsumption = gameObject;

                // Etkilerini uygula
                consumingFunction(healthEffect, caloriesEffect, hydrationEffect);
            }

            // Eğer donatılabilirse ve zaten hızlı slottaysa eklenmesin
            if (isEquippable && !isInsideQuickSlot && !EquipSystem.Instance.CheckIfFull())
            {
                EquipSystem.Instance.AddToQuickSlots(gameObject);
                isInsideQuickSlot = true;
            }

            // Eğer özel kullanıma sahipse (tek seferlik)
            if (isUseable)
            {
                //gameObject.SetActive(false); // Sahneden kaldır
                UseItem(); // Özel işlev tetiklenir
            }
        }
    }

    // Fare düğmesi serbest bırakıldığında tetiklenir
    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            // Tüketim tamamlandıysa item silinir
            if (isConsumable && itemPendingConsumption == gameObject)
            {
                DestroyImmediate(gameObject);
                InventorySystem.Instance.ReCalculateList();
                CraftingSystem.Instance.RefreshNeedItems();
            }
        }
    }
    private void UseItem()
    {
        // Bilgi panelini kapat
        itemInfoUI.SetActive(false);

        // Envanter ekranlarını kapat
        InventorySystem.Instance.isOpen = false;
        InventorySystem.Instance.inventoryScreenUI.SetActive(false);

        CraftingSystem.Instance.isOpen = false;
        CraftingSystem.Instance.craftingScreenUI.SetActive(false);
        CraftingSystem.Instance.toolsScreenUI.SetActive(false);
        CraftingSystem.Instance.survivalScreenUI.SetActive(false);
        CraftingSystem.Instance.refineScreenUI.SetActive(false);
        CraftingSystem.Instance.constructionScreenUI.SetActive(false);

        // İmleç kilidini aç
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // Seçim sistemini yeniden aktif et
        SelectionManager.Instance.EnabledSelection();
        SelectionManager.Instance.enabled = true;

        // Eşyanın adına göre işlemler
        switch (gameObject.name)
        {
            case "Zemin(Clone)":
            case "Zemin":
                ConstructionManager.Instance.itemPendingToBeDestroyed = gameObject;
                ConstructionManager.Instance.ActivateConstructionPlacement("ZeminModel");
                break;

            case "Duvar(Clone)":
            case "Duvar":
                ConstructionManager.Instance.itemPendingToBeDestroyed = gameObject;
                ConstructionManager.Instance.ActivateConstructionPlacement("DuvarModel");
                break;

            case "Sandık(Clone)":
            case "Sandık":
                PlacementSystem.Instance.inventoryItemToDestory = gameObject;
                PlacementSystem.Instance.ActivatePlacementMode("SandıkModel");
                print("clicked on the storage box");
                break;

            default:
                // Bilinmeyen eşya, işlem yapılmaz
                break;
        }
    }
    private void consumingFunction(float healthEffect, float caloriesEffect, float hydrationEffect)
    {
        // Et ürünü tüketiliyorsa
        if (thisName.ToLower().Contains("et"))
        {
            itemInfoUI.SetActive(false);
            caloriesEffectCalculation(120); // Kalori hesapla
            amountInInventory--;

            if (amountInInventory <= 0)
            {
                gameObject.SetActive(false);
                Destroy(gameObject);
            }

            InventorySystem.Instance.ReCalculateList();
            CraftingSystem.Instance.RefreshNeedItems();
        }

        // Ekmek ürünü tüketiliyorsa
        else if (thisName.ToLower().Contains("ekmek"))
        {
            itemInfoUI.SetActive(false);
            caloriesEffectCalculation(350); // Kalori hesapla
            amountInInventory--;

            if (amountInInventory <= 0)
            {
                gameObject.SetActive(false);
                Destroy(gameObject);
            }

            InventorySystem.Instance.ReCalculateList();
            CraftingSystem.Instance.RefreshNeedItems();
        }

        // Su ürünü tüketiliyorsa
        else if (thisName.ToLower().Contains("su"))
        {
            itemInfoUI.SetActive(false);
            hydrationEffectCalculation(15); // Su etkisi
            amountInInventory--;

            if (amountInInventory <= 0)
            {
                gameObject.SetActive(false);
                Destroy(gameObject);
            }

            InventorySystem.Instance.ReCalculateList();
            CraftingSystem.Instance.RefreshNeedItems();
        }
    }
    private static void caloriesEffectCalculation(float caloriesEffect)
    {
        // Oyuncunun mevcut ve maksimum kalori değerlerini al
        float caloriesBeforeConsumption = PlayerState.Instance.currentCalories;
        float maxCalories = PlayerState.Instance.maxCalories;

        if (caloriesEffect != 0)
        {
            // Kalori etkisini uygula, maksimum değeri geçmeyecek şekilde
            if ((caloriesBeforeConsumption + caloriesEffect) > maxCalories)
            {
                PlayerState.Instance.setCalories(maxCalories);
            }
            else
            {
                PlayerState.Instance.setCalories(caloriesBeforeConsumption + caloriesEffect);
            }
        }
    }
    private static void hydrationEffectCalculation(float hydrationEffect)
    {
        // Oyuncunun mevcut ve maksimum su oranlarını al
        float hydrationBeforeConsumption = PlayerState.Instance.currentHydrationPercent;
        float maxHydration = PlayerState.Instance.maxHydrationPercent;

        if (hydrationEffect != 0)
        {
            // Su etkisini uygula, maksimum değeri geçmeyecek şekilde
            if ((hydrationBeforeConsumption + hydrationEffect) > maxHydration)
            {
                PlayerState.Instance.setHydration(maxHydration);
            }
            else
            {
                PlayerState.Instance.setHydration(hydrationBeforeConsumption + hydrationEffect);
            }
        }
    }
}