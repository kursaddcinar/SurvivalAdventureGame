using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
    public static PlacementSystem Instance { get; set; }

    public GameObject placementHoldingSpot; // Yerleştirme sırasında objeyi geçici olarak tutacak nokta
    public GameObject enviromentPlaceables; // Yerleştirilen objelerin sahnede atanacağı ana obje (parent)

    public bool inPlacementMode; // Yerleştirme modu aktif mi
    [SerializeField] bool isValidPlacement; // Geçerli konuma mı yerleştiriliyor

    [SerializeField] GameObject itemToBePlaced; // Yerleştirilecek obje
    public GameObject inventoryItemToDestory;   // Envanterden silinecek obje referansı
    [SerializeField] GameObject placementModeUI; // Yerleştirme arayüzü

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

    public void ActivatePlacementMode(string itemToPlace)
    {
        // Prefab'ı sahneye instantiate et
        GameObject item = Instantiate(Resources.Load<GameObject>(itemToPlace));

        // (Clone) ifadesini kaldırmak için ismi güncelle
        item.name = itemToPlace;

        // Yerleştirme noktasına bağla (geçici parent)
        item.transform.SetParent(placementHoldingSpot.transform, false);

        // Referansı kaydet
        itemToBePlaced = item;

        // Yerleştirme modu aktif hale getir
        inPlacementMode = true;
    }

    private void Update()
    {
        // UI panelini yerleştirme moduna göre göster/gizle
        if (inPlacementMode)
        {
            placementModeUI.SetActive(true);
        }
        else
        {
            placementModeUI.SetActive(false);
        }

        // Obje sahnede varsa ve yerleştirme modundaysak konum kontrolü yap
        if (itemToBePlaced != null && inPlacementMode)
        {
            if (IsCheckValidPlacement())
            {
                isValidPlacement = true;
                itemToBePlaced.GetComponent<PlacebleItem>().SetValidColor(); // yeşil kenarlık
            }
            else
            {
                isValidPlacement = false;
                itemToBePlaced.GetComponent<PlacebleItem>().SetInvalidColor(); // kırmızı kenarlık
            }
        }

        // Sol tıklama ile yerleştir
        if (Input.GetMouseButtonDown(0) && inPlacementMode && isValidPlacement)
        {
            PlaceItemFreeStyle();
            DestroyItem(inventoryItemToDestory);
            Debug.Log("sol tık placement");
        }

        // X tuşuyla yerleştirmeyi iptal et
        if (Input.GetKeyDown(KeyCode.X))
        {
            inventoryItemToDestory.SetActive(true);
            inventoryItemToDestory = null;
            DestroyItem(itemToBePlaced);
            itemToBePlaced = null;
            inPlacementMode = false;
        }
    }

    private bool IsCheckValidPlacement()
    {
        if (itemToBePlaced != null)
        {
            return itemToBePlaced.GetComponent<PlacebleItem>().isValidToBeBuilt;
        }

        return false;
    }

    private void PlaceItemFreeStyle()
    {
        // Yerleştirilen objeyi sahnedeki çevre objelerinin altına ekle
        itemToBePlaced.transform.SetParent(enviromentPlaceables.transform, true);

        // Görsel efekti kaldır ve script'i devre dışı bırak
        itemToBePlaced.GetComponent<PlacebleItem>().SetDefaultColor();
        itemToBePlaced.GetComponent<PlacebleItem>().enabled = false;

        itemToBePlaced = null;

        StartCoroutine(Delay());
    }

    IEnumerator Delay()
    {
        // Yerleştirme modunu 1 saniye sonra kapat
        yield return new WaitForSeconds(1f);
        inPlacementMode = false;
    }

    private void DestroyItem(GameObject item)
    {
        // Obje yok edilir, envanter güncellenir
        DestroyImmediate(item);
        InventorySystem.Instance.ReCalculateList();
        CraftingSystem.Instance.RefreshNeedItems();
    }
}

/*
    private void DestroyItem(GameObject item)
    {
        InventoryItem invItem = item.GetComponent<InventoryItem>();
        if (invItem == null)
        {
            Debug.LogWarning("Item InventoryItem component içermiyor.");
            return;
        }

        Debug.Log(invItem.amountInInventory);
        invItem.amountInInventory--;
        Debug.Log(invItem.amountInInventory);
        
        if (invItem.amountInInventory <= 0)
        {
            item.SetActive(false);
            Destroy(item); // tamamen kaldır
        }

        InventorySystem.Instance.ReCalculateList();
        CraftingSystem.Instance.RefreshNeedItems();
    
    }
*/