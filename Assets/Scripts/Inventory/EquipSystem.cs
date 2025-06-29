using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipSystem : MonoBehaviour
{
    public static EquipSystem Instance { get; set; }

    // -- UI -- //
    public GameObject quickSlotsPanel; // Hızlı erişim slotlarının yer aldığı panel
    public List<GameObject> quickSlotsList = new List<GameObject>(); // Slotlar listesi

    public GameObject numbersHolder; // Slot numaralarının UI holder objesi

    public int selectedNumber = -1;        // Seçili slot numarası (-1 = seçili değil)
    public GameObject selectedItem;        // Seçilen item referansı
    public GameObject toolHolder;          // Ekipmanın sahnede gösterileceği yer
    public GameObject selectedItemModel;   // Seçili itemin model örneği

    private void Awake()
    {
        // Singleton oluşturulur
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    private void Start()
    {
        PopulateSlotList(); // Slotları otomatik olarak doldur
    }

    void Update()
    {
        // Sayı tuşlarına basıldığında ilgili slot seçilir
        if (Input.GetKeyDown(KeyCode.Alpha1)) { selectQuickSlot(1); }
        else if (Input.GetKeyDown(KeyCode.Alpha2)) { selectQuickSlot(2); }
        else if (Input.GetKeyDown(KeyCode.Alpha3)) { selectQuickSlot(3); }
        else if (Input.GetKeyDown(KeyCode.Alpha4)) { selectQuickSlot(4); }
        else if (Input.GetKeyDown(KeyCode.Alpha5)) { selectQuickSlot(5); }
        else if (Input.GetKeyDown(KeyCode.Alpha6)) { selectQuickSlot(6); }
        else if (Input.GetKeyDown(KeyCode.Alpha7)) { selectQuickSlot(7); }
    }

    void selectQuickSlot(int number)
    {
        if (checkIfSlotFull(number))
        {
            if (selectedNumber != number)
            {
                selectedNumber = number;

                // Önceki eşya varsa seçimi kaldır
                if (selectedItem != null)
                {
                    selectedItem.GetComponent<InventoryItem>().isSelected = false;
                }

                // Yeni eşya seçilir ve seçili hale getirilir
                selectedItem = getSelectedItem(number);
                selectedItem.GetComponent<InventoryItem>().isSelected = true;

                SetEquippedModel(selectedItem); // Model sahneye yerleştirilir

                // UI'da tüm numaraları gri yap, seçileni beyaz
                foreach (Transform child in numbersHolder.transform)
                {
                    child.transform.Find("Text").GetComponent<Text>().color = Color.gray;
                }

                Text toBeChanged = numbersHolder.transform.Find("number" + number).Find("Text").GetComponent<Text>();
                toBeChanged.color = Color.white;
            }
            else // Aynı slota tekrar basıldıysa seçimi iptal et
            {
                selectedNumber = -1;

                if (selectedItem != null)
                {
                    selectedItem.GetComponent<InventoryItem>().isSelected = false;
                    selectedItem = null;
                }

                if (selectedItemModel != null)
                {
                    DestroyImmediate(selectedItemModel);
                    selectedItemModel = null;
                }

                // Tüm numaraları gri yap
                foreach (Transform child in numbersHolder.transform)
                {
                    child.transform.Find("Text").GetComponent<Text>().color = Color.gray;
                }
            }
        }
    }

    // Seçili silahın hasarını döndürür
    internal object GetWeaponDamage()
    {
        if (selectedItem != null)
        {
            return selectedItem.GetComponent<Weapon>().weaponDamage;
        }
        else
        {
            return 0;
        }
    }

    // Seçilen item bir silah mı kontrol eder
    internal bool IsHoldingWeapon()
    {
        return selectedItem != null && selectedItem.GetComponent<Weapon>() != null;
    }

    // Swing (vuruş) kilitli mi kontrol eder
    internal bool IsThereASwingLock()
    {
        if (selectedItemModel && selectedItemModel.GetComponent<EquipableItem>())
        {
            return selectedItemModel.GetComponent<EquipableItem>().swingWait;
        }
        else
        {
            return false;
        }
    }
    // Seçilen item'in modelini sahnede göster
    private void SetEquippedModel(GameObject selectedItem)
    {
        // Eski model varsa sahneden sil
        if (selectedItemModel != null)
        {
            DestroyImmediate(selectedItemModel.gameObject);
            selectedItemModel = null;
        }

        // Prefab ismini "(Clone)" ifadesinden temizle
        string selectedItemName = selectedItem.name.Replace("(Clone)", "");

        // İlgili "_Model" prefab'ını instantiate et
        selectedItemModel = Instantiate(
            Resources.Load<GameObject>(selectedItemName + "_Model"),
            new Vector3(1.08f, 0.18f, 1.95f),
            Quaternion.Euler(0, -12.5f, -20f)
        );

        // Arayüzdeki tool holder içine yerleştir
        selectedItemModel.transform.SetParent(toolHolder.transform, false);
    }

    // Slot numarasına göre o slottaki eşyayı getir
    GameObject getSelectedItem(int slotNumber)
    {
        return quickSlotsList[slotNumber - 1].transform.GetChild(0).gameObject;
    }

    // Slot dolu mu kontrol et
    bool checkIfSlotFull(int slotNumber)
    {
        return quickSlotsList[slotNumber - 1].transform.childCount > 0;
    }

    // UI üzerindeki tüm hızlı slotları listeye al
    private void PopulateSlotList()
    {
        foreach (Transform child in quickSlotsPanel.transform)
        {
            if (child.CompareTag("QuickSlot"))
            {
                quickSlotsList.Add(child.gameObject);
            }
        }
    }

    // Envanterdeki bir item'ı hızlı slota yerleştir
    public void AddToQuickSlots(GameObject itemToEquip)
    {
        GameObject availableSlot = FindNextEmptySlot();
        itemToEquip.transform.SetParent(availableSlot.transform, false);

        InventorySystem.Instance.ReCalculateList(); // Envanteri güncelle
    }

    // İlk boş quickslot'u bulur
    public GameObject FindNextEmptySlot()
    {
        foreach (GameObject slot in quickSlotsList)
        {
            if (slot.transform.childCount == 0)
            {
                return slot;
            }
        }
        return new GameObject(); // Boş slot bulunamazsa yeni boş obje döndürülür (önlem)
    }

    // Tüm slotlar dolu mu kontrol eder
    public bool CheckIfFull()
    {
        int counter = 0;

        foreach (GameObject slot in quickSlotsList)
        {
            if (slot.transform.childCount > 0)
            {
                counter += 1;
            }
        }

        return counter == 7;
    }
}
