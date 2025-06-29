using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingSystem : MonoBehaviour
{
    public GameObject craftingScreenUI; // Ana üretim arayüzü
    public GameObject toolsScreenUI, survivalScreenUI, refineScreenUI, constructionScreenUI;

    public List<string> inventoryItemList = new List<string>(); // Oyuncunun sahip olduğu eşya listesi

    // Kategori butonları
    Button toolsBTN, survivalBTN, refineBTN, constructionBTN;

    // Üretim butonları
    Button craftAxeBTN, craftBowBTN, craftArrowBTN, craftPlankBTN, craftFoundationBTN, craftWallBTN, craftStorageBoxBTN;

    // Gerekli malzeme yazıları
    Text AxeReq1, AxeReq2,BowReq1, BowReq2,ArrowReq1, ArrowReq2, PlankReq1, FoundationReq1, WallReq1, StorageBoxReq1;

    public bool isOpen; // Arayüz açık mı

    // Blueprint tanımları
    // (ürün adı, üretilecek miktar, gerekli malzeme sayısı, malzeme1 adı, adedi, malzeme2 adı, adedi)
    private Blueprint AxeBLP = new Blueprint("Balta", 1, 2, "Tas", 3, "Dal", 3);
    private Blueprint BowBLP = new Blueprint("Yay", 1, 2, "Tel", 1, "Kereste", 1);
    private Blueprint ArrowBLP = new Blueprint("Ok", 1, 2, "Tas", 1, "Dal", 1);
    private Blueprint PlankBLP = new Blueprint("Kereste", 2, 1, "Kutuk", 1, "", 0);
    private Blueprint FoundationBLP = new Blueprint("Zemin", 1, 1, "Kereste", 4, "", 0);
    private Blueprint WallBLP = new Blueprint("Duvar", 1, 1, "Kereste", 2, "", 0);
    private Blueprint StorageBoxBLP = new Blueprint("Sandık", 1, 1, "Kereste", 2, "", 0);

    public static CraftingSystem Instance { get; set; }

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

        // Kategori butonlarına fonksiyon bağla
        toolsBTN = craftingScreenUI.transform.Find("ToolsButton").GetComponent<Button>();
        toolsBTN.onClick.AddListener(delegate { OpenToolsCategory(); });

        survivalBTN = craftingScreenUI.transform.Find("SurvivalButton").GetComponent<Button>();
        survivalBTN.onClick.AddListener(delegate { OpenSurvivalCategory(); });

        refineBTN = craftingScreenUI.transform.Find("RefineButton").GetComponent<Button>();
        refineBTN.onClick.AddListener(delegate { OpenRefineCategory(); });

        constructionBTN = craftingScreenUI.transform.Find("ConstructionButton").GetComponent<Button>();
        constructionBTN.onClick.AddListener(delegate { OpenConstructionCategory(); });

        // Balta üretimi UI bağlantıları
        AxeReq1 = toolsScreenUI.transform.Find("Balta").transform.Find("req1").GetComponent<Text>();
        AxeReq2 = toolsScreenUI.transform.Find("Balta").transform.Find("req2").GetComponent<Text>();
        craftAxeBTN = toolsScreenUI.transform.Find("Balta").transform.Find("Button").GetComponent<Button>();
        craftAxeBTN.onClick.AddListener(delegate { CraftAnyItem(AxeBLP); });
        
        // Yay üretimi UI bağlantıları
        BowReq1 = toolsScreenUI.transform.Find("Yay").transform.Find("req1").GetComponent<Text>();
        BowReq2 = toolsScreenUI.transform.Find("Yay").transform.Find("req2").GetComponent<Text>();
        craftBowBTN = toolsScreenUI.transform.Find("Yay").transform.Find("Button").GetComponent<Button>();
        craftBowBTN.onClick.AddListener(delegate { CraftAnyItem(BowBLP); });
        
        // Ok üretimi UI bağlantıları
        ArrowReq1 = toolsScreenUI.transform.Find("Ok").transform.Find("req1").GetComponent<Text>();
        ArrowReq2 = toolsScreenUI.transform.Find("Ok").transform.Find("req2").GetComponent<Text>();
        craftArrowBTN = toolsScreenUI.transform.Find("Ok").transform.Find("Button").GetComponent<Button>();
        craftArrowBTN.onClick.AddListener(delegate { CraftAnyItem(ArrowBLP); });

        // Kereste üretimi UI bağlantıları
        PlankReq1 = refineScreenUI.transform.Find("Kereste").transform.Find("req1").GetComponent<Text>();
        craftPlankBTN = refineScreenUI.transform.Find("Kereste").transform.Find("Button").GetComponent<Button>();
        craftPlankBTN.onClick.AddListener(delegate { CraftAnyItem(PlankBLP); });

        // Zemin üretimi UI bağlantıları
        FoundationReq1 = constructionScreenUI.transform.Find("Zemin").transform.Find("req1").GetComponent<Text>();
        craftFoundationBTN = constructionScreenUI.transform.Find("Zemin").transform.Find("Button").GetComponent<Button>();
        craftFoundationBTN.onClick.AddListener(delegate { CraftAnyItem(FoundationBLP); });

        // Duvar üretimi UI bağlantıları
        WallReq1 = constructionScreenUI.transform.Find("Duvar").transform.Find("req1").GetComponent<Text>();
        craftWallBTN = constructionScreenUI.transform.Find("Duvar").transform.Find("Button").GetComponent<Button>();
        craftWallBTN.onClick.AddListener(delegate { CraftAnyItem(WallBLP); });

        // Sandık üretimi UI bağlantıları
        StorageBoxReq1 = survivalScreenUI.transform.Find("Sandık").transform.Find("req1").GetComponent<Text>();
        craftStorageBoxBTN = survivalScreenUI.transform.Find("Sandık").transform.Find("Button").GetComponent<Button>();
        craftStorageBoxBTN.onClick.AddListener(delegate { CraftAnyItem(StorageBoxBLP); });
    }

    void OpenToolsCategory()
    {
        // Tüm ekranları kapat, sadece tools ekranını aç
        craftingScreenUI.SetActive(false);
        survivalScreenUI.SetActive(false);
        refineScreenUI.SetActive(false);
        constructionScreenUI.SetActive(false);
        ConstructionManager.Instance.inConstructionMode = false;

        toolsScreenUI.SetActive(true);
    }


        void OpenSurvivalCategory()
    {
        // Survival kategorisini aç, diğer tüm UI panellerini kapat
        craftingScreenUI.SetActive(false);
        toolsScreenUI.SetActive(false);
        refineScreenUI.SetActive(false);
        constructionScreenUI.SetActive(false);
        ConstructionManager.Instance.inConstructionMode = false;

        survivalScreenUI.SetActive(true);
    }

    void OpenRefineCategory()
    {
        // Refine (işleme) kategorisini aç
        craftingScreenUI.SetActive(false);
        toolsScreenUI.SetActive(false);
        survivalScreenUI.SetActive(false);
        constructionScreenUI.SetActive(false);
        ConstructionManager.Instance.inConstructionMode = false;

        refineScreenUI.SetActive(true);
    }

    void OpenConstructionCategory()
    {
        // Yapı üretim kategorisini aç
        craftingScreenUI.SetActive(false);
        toolsScreenUI.SetActive(false);
        survivalScreenUI.SetActive(false);
        refineScreenUI.SetActive(false);

        constructionScreenUI.SetActive(true);
        ConstructionManager.Instance.inConstructionMode = true;
    }

    void CraftAnyItem(Blueprint blueprintToCraft)
    {
        // Üretim sesi oynatılır
        SoundManager.Instance.PlaySound(SoundManager.Instance.craftingSound);

        // Ürün üretimi gecikmeli yapılır (ses ile senkron)
        StartCoroutine(craftedDelayForSound(blueprintToCraft));

        // Envanterden gerekli malzemeleri eksilt
        if (blueprintToCraft.numOfRequirements == 1)
        {
            InventorySystem.Instance.RemoveItem(blueprintToCraft.Req1, blueprintToCraft.Req1amount);
        }
        else if (blueprintToCraft.numOfRequirements == 2)
        {
            InventorySystem.Instance.RemoveItem(blueprintToCraft.Req1, blueprintToCraft.Req1amount);
            InventorySystem.Instance.RemoveItem(blueprintToCraft.Req2, blueprintToCraft.Req2amount);
        }

        // Gerekli hesaplamaları 1 saniye sonra yap
        StartCoroutine(calculate());
    }

    public IEnumerator calculate()
    {
        yield return new WaitForSeconds(1f);
        InventorySystem.Instance.ReCalculateList(); // Envanteri güncelle
        RefreshNeedItems(); // Gerekli malzeme listelerini yenile
    }

    IEnumerator craftedDelayForSound(Blueprint blueprintToCraft)
    {
        yield return new WaitForSeconds(1f);

        // Belirtilen sayıda üretilen eşyayı envantere ekle
        for (var i = 0; i < blueprintToCraft.numOfItemsToProduce; i++)
        {
            InventorySystem.Instance.AddToInventory(blueprintToCraft.itemName, true);
        }
    }

    void Update()
    {
        RefreshNeedItems(); // Sürekli ihtiyaç listesi güncellenir

        // C tuşuna basıldığında üretim arayüzünü aç/kapat
        if (Input.GetKeyDown(KeyCode.C) && !isOpen && ConstructionManager.Instance.inConstructionMode == false)
        {
            craftingScreenUI.SetActive(true);

            Cursor.lockState = CursorLockMode.None; // İmleç serbest
            Cursor.visible = true;

            SelectionManager.Instance.DisabledSelection();
            SelectionManager.Instance.GetComponent<SelectionManager>().enabled = false;

            isOpen = true;
            RefreshNeedItems();
        }
        else if (Input.GetKeyDown(KeyCode.C) && isOpen)
        {
            craftingScreenUI.SetActive(false);
            toolsScreenUI.SetActive(false);
            survivalScreenUI.SetActive(false);
            refineScreenUI.SetActive(false);
            constructionScreenUI.SetActive(false);
            ConstructionManager.Instance.inConstructionMode = false;

            if (InventorySystem.Instance.isOpen == false)
            {
                Cursor.lockState = CursorLockMode.Locked; // İmleç kilitli
                Cursor.visible = false;

                SelectionManager.Instance.DisabledSelection();
                SelectionManager.Instance.GetComponent<SelectionManager>().enabled = true;
                SelectionManager.Instance.centerDotImage.enabled = true;
            }

            isOpen = false;
        }
    }



    public void RefreshNeedItems()
    {
        // Envanterdeki eşya sayacı
        int stone_count = 0;
        int stick_count = 0;
        int string_count = 0;
        int log_count = 0;
        int plank_count = 0;

        // Envanter listesi alınır
        inventoryItemList = InventorySystem.Instance.itemList;

        // Envanterdeki her item için tür sayımı yapılır
        foreach (string itemName in inventoryItemList)
        {
            switch (itemName)
            {
                case "Tas":
                    stone_count += 1;
                    break;

                case "Tel":
                    string_count += 1;
                    break;

                case "Dal":
                    stick_count += 1;
                    break;

                case "Kutuk":
                    log_count += 1;
                    break;

                case "Kereste":
                    plank_count += 1;
                    break;
            }
        }

        // 🔧 BALTA
        AxeReq1.text = "3 Tas [" + stone_count + "]";
        AxeReq2.text = "3 Dal [" + stick_count + "]";

        if (stone_count >= 3 && stick_count >= 3 && InventorySystem.Instance.CheckSlotAvailable(1))
        {
            craftAxeBTN.gameObject.SetActive(true);
        }
        else
        {
            craftAxeBTN.gameObject.SetActive(false);
        }

        // 🔧 Yay
        BowReq1.text = "1 Tel [" + string_count + "]";
        BowReq2.text = "1 Kereste [" + plank_count + "]";

        if (string_count >= 1 && plank_count >= 1 && InventorySystem.Instance.CheckSlotAvailable(1))
        {
            craftBowBTN.gameObject.SetActive(true);
        }
        else
        {
            craftBowBTN.gameObject.SetActive(false);
        }

        // 🔧 Ok
        ArrowReq1.text = "1 Tas [" + stone_count + "]";
        ArrowReq2.text = "1 Dal [" + stick_count + "]";

        if (stone_count >= 1 && stick_count >= 1 && InventorySystem.Instance.CheckSlotAvailable(1))
        {
            craftArrowBTN.gameObject.SetActive(true);
        }
        else
        {
            craftArrowBTN.gameObject.SetActive(false);
        }

        // 🪵 KERESTE
        PlankReq1.text = "1 Kutuk [" + log_count + "]";

        if (log_count >= 1 && InventorySystem.Instance.CheckSlotAvailable(2))
        {
            craftPlankBTN.gameObject.SetActive(true);
        }
        else
        {
            craftPlankBTN.gameObject.SetActive(false);
        }

        // 🧱 ZEMİN
        FoundationReq1.text = "4 Kereste [" + plank_count + "]";

        if (plank_count >= 4 && InventorySystem.Instance.CheckSlotAvailable(1))
        {
            craftFoundationBTN.gameObject.SetActive(true);
        }
        else
        {
            craftFoundationBTN.gameObject.SetActive(false);
        }

        // 🧱 DUVAR
        WallReq1.text = "2 Kereste [" + plank_count + "]";

        if (plank_count >= 2 && InventorySystem.Instance.CheckSlotAvailable(1))
        {
            craftWallBTN.gameObject.SetActive(true);
        }
        else
        {
            craftWallBTN.gameObject.SetActive(false);
        }

        // 📦 SANDIK
        StorageBoxReq1.text = "2 Kereste [" + plank_count + "]";

        if (plank_count >= 2 && InventorySystem.Instance.CheckSlotAvailable(1))
        {
            craftStorageBoxBTN.gameObject.SetActive(true);
        }
        else
        {
            craftStorageBoxBTN.gameObject.SetActive(false);
        }
    }

}
