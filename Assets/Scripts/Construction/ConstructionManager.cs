using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConstructionManager : MonoBehaviour
{
    public static ConstructionManager Instance { get; set; }
    [SerializeField] GameObject placementModeUI;
    public GameObject itemToBeConstructed; // İnşa edilecek nesne
    public bool inConstructionMode = false; // İnşa modu aktif mi
    public GameObject constructionHoldingSpot; // Geçici tutucu nesne (yerleştirme öncesi)

    public bool isValidPlacement; // Yerleştirme geçerli mi

    public bool selectingAGhost; // Ghost (önizleme) nesnesi seçiliyor mu
    public GameObject selectedGhost; // Seçilen ghost nesnesi

    // Ghost nesneleri için kullanılan materyaller
    public Material ghostSelectedMat;
    public Material ghostSemiTransparentMat; // test amaçlı
    public Material ghostFullTransparentMat;

    // Tüm aktif ghost nesneleri referans olarak tutar
    public List<GameObject> allGhostsInExistence = new List<GameObject>();

    public GameObject itemPendingToBeDestroyed; // Yok edilmesi beklenen nesne
    public GameObject ConstructionUI;
    public GameObject InventoryScreenUI;

    public GameObject player;

    public QuestBuildChecker buildQuestChecker; // Görev kontrolü için referans

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

    public void ActivateConstructionPlacement(string itemToConstruct)
    {
        // İlgili prefab'ı oluştur
        GameObject item = Instantiate(Resources.Load<GameObject>(itemToConstruct));

        // İsimden "(Clone)" ifadesini kaldır
        item.name = itemToConstruct;

        item.transform.SetParent(constructionHoldingSpot.transform, false);
        itemToBeConstructed = item;
        itemToBeConstructed.gameObject.tag = "activeConstructable";

        // Ray işlemi için collider devre dışı bırakılır
        itemToBeConstructed.GetComponent<Constructable>().solidCollider.enabled = false;

        // İnşa modu aktif hale getirilir
        inConstructionMode = true;
    }

    private void GetAllGhosts(GameObject itemToBeConstructed)
    {
        List<GameObject> ghostlist = itemToBeConstructed.gameObject.GetComponent<Constructable>().ghostList;

        foreach (GameObject ghost in ghostlist)
        {
            allGhostsInExistence.Add(ghost);
        }
    }

    private void PerformGhostDeletionScan()
    {
        // Aynı konumda birden fazla ghost varsa işaretle
        foreach (GameObject ghost in allGhostsInExistence)
        {
            if (ghost != null && ghost.GetComponent<GhostItem>().hasSamePosition == false)
            {
                foreach (GameObject ghostX in allGhostsInExistence)
                {
                    if (ghost.gameObject != ghostX.gameObject)
                    {
                        if (XPositionToAccurateFloat(ghost) == XPositionToAccurateFloat(ghostX) &&
                            ZPositionToAccurateFloat(ghost) == ZPositionToAccurateFloat(ghostX))
                        {
                            if (ghost != null && ghostX != null)
                            {
                                ghostX.GetComponent<GhostItem>().hasSamePosition = true;
                                break;
                            }
                        }
                    }
                }
            }
        }

        // İşaretlenen ghost'ları yok et
        foreach (GameObject ghost in allGhostsInExistence)
        {
            if (ghost != null && ghost.GetComponent<GhostItem>().hasSamePosition)
            {
                DestroyImmediate(ghost);
            }
        }
    }

    private float XPositionToAccurateFloat(GameObject ghost)
    {
        // Pozisyonu 2 ondalık basamağa yuvarla
        if (ghost != null)
        {
            Vector3 targetPosition = ghost.transform.position;
            float xFloat = Mathf.Round(targetPosition.x * 100f) / 100f;
            return xFloat;
        }
        return 0;
    }

    private float ZPositionToAccurateFloat(GameObject ghost)
    {
        // Pozisyonu 2 ondalık basamağa yuvarla
        if (ghost != null)
        {
            Vector3 targetPosition = ghost.transform.position;
            float zFloat = Mathf.Round(targetPosition.z * 100f) / 100f;
            return zFloat;
        }
        return 0;
    }

    private void Update()
    {
        if (inConstructionMode)
        {
            placementModeUI.SetActive(true);
        }
        else
        {
            placementModeUI.SetActive(false);
        }

        if (itemToBeConstructed != null)
        {
            if (itemToBeConstructed.name == "ZeminModel")
            {
                if (CheckValidConstructionPosition())
                {
                    isValidPlacement = true;
                    itemToBeConstructed.GetComponent<Constructable>().SetValidColor();
                }
                else
                {
                    isValidPlacement = false;
                    itemToBeConstructed.GetComponent<Constructable>().SetInvalidColor();
                }
            }

            // Fare konumuna ray gönder
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                var selectionTransform = hit.transform;

                // Ghost'a tıklanırsa seçim başlatılır
                if (selectionTransform.gameObject.CompareTag("ghost") && itemToBeConstructed.name == "ZeminModel")
                {
                    itemToBeConstructed.SetActive(false);
                    selectingAGhost = true;
                    selectedGhost = selectionTransform.gameObject;
                }
                else if (selectionTransform.gameObject.CompareTag("wallGhost") && itemToBeConstructed.name == "DuvarModel")
                {
                    itemToBeConstructed.SetActive(false);
                    selectingAGhost = true;
                    selectedGhost = selectionTransform.gameObject;
                }
                else
                {
                    itemToBeConstructed.SetActive(true);
                    selectedGhost = null;
                    selectingAGhost = false;
                }
            }
        }

        // Sol tıklama: yerleştirme
        if (Input.GetMouseButtonDown(0))
        {
            if (isValidPlacement && !selectingAGhost && itemToBeConstructed.name == "ZeminModel")
            {
                if (InventoryScreenUI.activeSelf)
                {
                    InventoryScreenUI.SetActive(false);
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }

                PlaceItemFreeStyle();
                DestroyItem(itemPendingToBeDestroyed);  // eski yapı
                InventoryScreenUI.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

                isValidPlacement = false;
                selectingAGhost = false;
            }

            if (selectingAGhost)
            {
                PlaceItemInGhostPosition(selectedGhost);
                DestroyItem(itemPendingToBeDestroyed);


                isValidPlacement = false;
                selectingAGhost = false;
            }
        }

        // Sağ tıklama: işlemi iptal et
        if (Input.GetKeyDown(KeyCode.X))
        {
            //itemPendingToBeDestroyed.SetActive(true);
            itemPendingToBeDestroyed = null;
            //DestroyItem(itemToBeConstructed);
            itemToBeConstructed = null;
            inConstructionMode = false;
            selectedGhost = null;

            InventoryScreenUI.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }


    }

    private void PlaceItemInGhostPosition(GameObject copyOfGhost)
    {
        Vector3 ghostPosition = copyOfGhost.transform.position;
        Quaternion ghostRotation = copyOfGhost.transform.rotation;

        selectedGhost.SetActive(false);

        itemToBeConstructed.SetActive(true);
        itemToBeConstructed.transform.SetParent(transform.parent.transform.parent, true);

        var randomOffset = UnityEngine.Random.Range(0.01f, 0.03f);
        itemToBeConstructed.transform.position = new Vector3(ghostPosition.x, ghostPosition.y, ghostPosition.z + randomOffset);
        itemToBeConstructed.transform.rotation = ghostRotation;

        itemToBeConstructed.GetComponent<Constructable>().solidCollider.enabled = true;
        itemToBeConstructed.GetComponent<Constructable>().SetDefaultColor();

        if (itemToBeConstructed.name == "ZeminModel")
        {
            itemToBeConstructed.GetComponent<Constructable>().ExtractGhostMembers();
            itemToBeConstructed.tag = "placedFoundation";
            GetAllGhosts(itemToBeConstructed);
            PerformGhostDeletionScan();
        }
        else
        {
            itemToBeConstructed.tag = "placedWall";
            DestroyItem(selectedGhost);
        }

        itemToBeConstructed = null;
        inConstructionMode = false;

        InventoryScreenUI.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // Görev kontrolü
        if (buildQuestChecker != null)
            buildQuestChecker.CheckIfHutBuilt();
    }
    /*
        void DestroyItem(GameObject item)
        {
            DestroyImmediate(item);
            InventorySystem.Instance.ReCalculateList();
            CraftingSystem.Instance.RefreshNeedItems();
        }
    */

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
            Debug.Log("if");
            item.SetActive(false);
            DestroyImmediate(item); // tamamen kaldır
        }

        InventorySystem.Instance.ReCalculateList();
        CraftingSystem.Instance.RefreshNeedItems();

    }*/
    private void DestroyItem(GameObject item)
    {
        InventoryItem invItem = item.GetComponent<InventoryItem>();
        if (invItem == null)
        {
            Debug.LogWarning("Item InventoryItem component içermiyor.");
            return;
        }

        invItem.amountInInventory--;

        if (invItem.amountInInventory <= 0)
        {
            item.SetActive(false); // UI’dan kaldır
            Destroy(item);         // sonraki frame’de temizle
        }
        else
        {
            // 🔄 slotu sadece güncelle
            InventorySystem.Instance.ReCalculateList(); // varsa UI güncelleyici fonksiyonu
        }

        InventorySystem.Instance.ReCalculateList();
        CraftingSystem.Instance.RefreshNeedItems();
    }


    private void PlaceItemFreeStyle()
    {
        itemToBeConstructed.transform.SetParent(transform.parent.transform.parent, true);
        itemToBeConstructed.GetComponent<Constructable>().ExtractGhostMembers();
        itemToBeConstructed.GetComponent<Constructable>().SetDefaultColor();
        itemToBeConstructed.tag = "placedFoundation";
        itemToBeConstructed.GetComponent<Constructable>().enabled = false;
        itemToBeConstructed.GetComponent<Constructable>().solidCollider.enabled = true;

        GetAllGhosts(itemToBeConstructed);
        PerformGhostDeletionScan();

        itemToBeConstructed = null;
        inConstructionMode = false;

        if (buildQuestChecker != null)
            buildQuestChecker.CheckIfHutBuilt();
    }

    private bool CheckValidConstructionPosition()
    {
        if (itemToBeConstructed != null)
        {
            return itemToBeConstructed.GetComponent<Constructable>().isValidToBeBuilt;
        }

        return false;
    }
}
