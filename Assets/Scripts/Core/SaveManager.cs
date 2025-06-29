/// SaveManager.cs
// Bu sınıf, kullanıcıya ve slota göre oyunun verilerini kaydetmek, yüklemek,
// yeni oyun başlatmak ve sahne geçişleri arasında kayıt durumunu taşımak için kullanılır.

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SaveManager : MonoBehaviour
{
    // Singleton pattern
    public static SaveManager Instance { get; private set; }

    // Yeni oyun başlatma kontrolü için flag
    public bool isStartingNewGame = true;

    // Kullanıcı ve slot bilgileri
    public int currentUserId;
    public int currentSlotIndex;

    // Geçici olarak sahne geçişi sırasında tutulacak kayıt bilgileri
    public bool shouldLoadFromFile = false;
    public int pendingUserId;
    public int pendingSlotIndex;

    // Loading ekranı gösterilecekse
    public bool isLoading;
    public Canvas loadingScreen;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Aynı anda ikinci bir SaveManager varsa yok et
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Sahne değilse de silinmesin
    }

    // Kullanıcı ID'sini geçici olarak ayarla
    public void SetPendingUser(int id) => pendingUserId = id;

    // Geçici kullanıcı ID'sini döndür
    public int GetPendingUserId() => pendingUserId;

    // Geçici slot index'ini döndür
    public int GetPendingSlotIndex() => pendingSlotIndex;

    // Belirtilen kullanıcı ve slot için kayıt dosyası var mı?
    public bool HasSave(int userId, int slotIndex)
    {
        string path = GetSavePath(userId, slotIndex);
        return File.Exists(path);
    }

    // Kullanıcının herhangi bir slotunda kayıt var mı?
    public bool HasAnySave(int userId)
    {
        for (int i = 0; i < 5; i++)
        {
            string path = GetSavePath(userId, i);
            if (File.Exists(path))
                return true;
        }
        return false;
    }

    // Oyun verisini kaydeder (şu an sadece pozisyon)
    public void SaveGame(int userId, int slotIndex)
    {
        if (StorageManager.Instance != null && StorageManager.Instance.storageUIOpen)
        {
            var storage = StorageManager.Instance.selectedStorage;
            var storageUI = StorageManager.Instance.GetRelevantUI(storage);
            StorageManager.Instance.RecalculaTeStorage(storageUI); // 🔧 BURASI KRİTİK
        }

        GameObject player = GameObject.FindWithTag("activeConstructable");
        Vector3 playerPosition = player.transform.position;

        // PlayerState'ten sağlık verilerini al
        PlayerState ps = PlayerState.Instance;
        string[] inventoryItems = InventorySystem.Instance.itemList.ToArray();
        string[] quickSlotItems = GetQuickSlotContent();

        //Debug.Log("currentPoints: " + ps.currentPoints + ", GetPoints: " + ps.GetPoints());
        PlayerData playerData = new PlayerData(
            playerPosition,
            ps.currentHealth,
            ps.currentCalories,
            ps.currentHydrationPercent,
            ps.currentPoints,
            inventoryItems,
            quickSlotItems,
            ps.hasTriggeredTomurIntro,
            ps.gameIsCompleted);

        EnvironmentData envData = GetEnvironmentData();


        AllGameData allData = new AllGameData(playerData);
        allData.environmentData = envData; // 👈 Ortam verilerini kayda ekle

        allData.questProgressData = GetQuestProgressData();
        allData.currentNPCIndex = NPCManager.Instance.GetCurrentNPCIndex();

        string json = JsonUtility.ToJson(allData, true);
        File.WriteAllText(GetSavePath(userId, slotIndex), json);

        //Debug.Log($"Oyun kaydedildi: User {userId} - Slot {slotIndex}");
    }



    // Yeni oyun başlatma sırasında kullanıcı ve slot'u ayarla ve sahne geçişinden sonra kayıt için hazırla
    public void PrepareNewGame(int userId, int slotIndex)
    {
        Debug.Log("prepare");
        pendingUserId = userId;
        pendingSlotIndex = slotIndex;
        isStartingNewGame = true;
    }

    // Yeni oyun başlatılıyorsa (GameScene yüklendiğinde), pozisyonu ayarla ve ilk kaydı yap
    public void StartNewGame(int userId, int slotIndex)
    {
        SetPendingUser(userId);
        pendingSlotIndex = slotIndex;

        // DİKKAT: Bu fonksiyonda sahnede "Player" varsa kayıt yapılabilir
        SaveGame(userId, slotIndex);
    }

    // Yükleme öncesi kayıtlı kullanıcı ve slot'u hazırla
    public void TriggerLoadAfterScene(int userId, int slotIndex)
    {
        pendingUserId = userId;
        pendingSlotIndex = slotIndex;
    }

    // Sahne yüklendikten sonra kayıtlı pozisyonu uygula
    public void LoadGameDataAfterSceneLoad()
    {
        string path = GetSavePath(pendingUserId, pendingSlotIndex);
        if (!File.Exists(path))
        {
            Debug.LogWarning("Yüklenecek kayıt bulunamadı.");
            return;
        }

        string json = File.ReadAllText(path);
        AllGameData data = JsonUtility.FromJson<AllGameData>(json);

        // ENVANTERİ YÜKLE
        if (data.playerData.inventoryContent != null)
        {
            foreach (string item in data.playerData.inventoryContent)
            {
                InventorySystem.Instance.AddToInventory(item, true);
            }
        }
        else
        {
            Debug.LogWarning("Inventory content is null in save data.");
        }

        // QUICK SLOTS
        foreach (string item in data.playerData.quickSlotContent)
        {
            GameObject availableSlot = EquipSystem.Instance.FindNextEmptySlot();
            GameObject itemPrefab = Resources.Load<GameObject>(item);
            if (itemPrefab != null && availableSlot != null)
            {
                GameObject clone = GameObject.Instantiate(itemPrefab);
                clone.transform.SetParent(availableSlot.transform, false);
            }
        }

        GameObject player = GameObject.FindWithTag("activeConstructable");
        player.transform.position = data.playerData.GetPosition();

        // YÜKLENEN VERİLERİ OYUNCUYA UYGULA
        PlayerState.Instance.setHealth(data.playerData.health);
        PlayerState.Instance.setCalories(data.playerData.calories);
        PlayerState.Instance.setHydration(data.playerData.hydration);
        PlayerState.Instance.SetPoints(data.playerData.points);
        PlayerState.Instance.gameIsCompleted = data.playerData.gameIsCompleted;

        PlayerState.Instance.hasTriggeredTomurIntro = data.playerData.hasTriggeredTomurIntro;

        SetEnvironmentData(data.environmentData);

        NPCManager.Instance.SetCurrentNPCIndex(data.currentNPCIndex);

        ApplyQuestProgress(data.questProgressData);
        isStartingNewGame = false;
    }
    // Belirli kullanıcı ve slot için dosya yolunu döndür
    private string GetSavePath(int userId, int slotIndex)
    {
#if UNITY_EDITOR
        return Application.dataPath + $"/Resources/Saves/save_user_{userId}_slot_{slotIndex}.json";
#else
    return Application.persistentDataPath + $"/save_user_{userId}_slot_{slotIndex}.json";
#endif
    }

    private EnvironmentData GetEnvironmentData()
    {
        // Toplanan itemlar
        List<string> itemsPickedUp = InventorySystem.Instance.itemsPickedUp;

        // Ağaç verileri
        List<TreeData> treesToSave = new List<TreeData>();
        foreach (Transform tree in EnvironmentManager.Instance.allTrees.transform)
        {
            var td = new TreeData();
            td.position = tree.position;
            td.rotation = tree.rotation.eulerAngles;

            if (tree.CompareTag("Tree2"))
            {
                td.name = "Tree_Parent2"; // Prefab Resources içinde bu adla olmalı
            }
            else if (tree.CompareTag("Tree"))
            {
                td.name = "Tree_Parent";
            }
            else
            {
                td.name = "Stump";
            }

            treesToSave.Add(td);
        }

        // Hayvan verileri
        List<string> allAnimals = new List<string>();
        foreach (Transform animal in EnvironmentManager.Instance.allAnimals.transform)
        {
            if (animal.GetComponent<Animal>() != null)
            {
                allAnimals.Add(animal.name.Replace("(Clone)", ""));
            }
        }

        // Sandık verileri
        List<StorageData> allStorage = new List<StorageData>();
        foreach (Transform placeable in EnvironmentManager.Instance.placeables.transform)
        {
            StorageBox sb = placeable.GetComponent<StorageBox>();
            if (sb != null)
            {
                StorageData sd = new StorageData();
                sd.items = sb.items;
                sd.position = placeable.position;
                sd.rotation = placeable.rotation.eulerAngles;
                allStorage.Add(sd);
            }
        }

        return new EnvironmentData(itemsPickedUp, treesToSave, allAnimals, allStorage);
    }


    private string[] GetQuickSlotContent()
    {
        List<string> temp = new List<string>();
        foreach (GameObject slot in EquipSystem.Instance.quickSlotsList)
        {
            if (slot.transform.childCount != 0)
            {
                string name = slot.transform.GetChild(0).name;
                string cleanName = name.Replace("(Clone)", "");
                temp.Add(cleanName);
            }
        }
        return temp.ToArray();
    }


    private List<NPCQuestProgressData> GetQuestProgressData()
    {
        List<NPCQuestProgressData> dataList = new List<NPCQuestProgressData>();

        foreach (NPC npc in FindObjectsOfType<NPC>())
        {
            var npcData = new NPCQuestProgressData();
            npcData.npcName = npc.gameObject.name; // 👈 npcName yerine object name kullanıyoruz
            npcData.activeQuestIndex = npc.activeQuestIndex;
            npcData.npcDeactivated = !npc.gameObject.activeSelf;

            foreach (var quest in npc.quests) // 👈 assignedQuests değil, quests kullan
            {
                var qsd = new QuestSaveData();
                qsd.questName = quest.questName;
                qsd.accepted = quest.accepted;
                qsd.declined = quest.declined;
                qsd.initialDialogCompleted = quest.initialDialogCompleted;
                qsd.isCompleted = quest.isCompleted;
                npcData.quests.Add(qsd);
            }

            dataList.Add(npcData);
        }

        return dataList;
    }
    private void SetEnvironmentData(EnvironmentData data)
    {
        // Toplanan itemlar sahneden silinir
        foreach (Transform itemType in EnvironmentManager.Instance.allItems.transform)
        {
            foreach (Transform item in itemType)
            {
                if (data.pickedUpItems.Contains(item.name))
                {
                    Destroy(item.gameObject);
                }
            }
        }
        InventorySystem.Instance.itemsPickedUp = data.pickedUpItems;

        // Ağaçlar sıfırlanır (önce sahnedeki tüm ağaçlar silinir)
        foreach (Transform tree in EnvironmentManager.Instance.allTrees.transform)
        {
            Destroy(tree.gameObject);
        }

        // Kayıttan gelen ağaç prefab'ları instantiate edilip tekrar sahneye konur
        foreach (TreeData tree in data.treeData)
        {
            var treePrefab = Instantiate(Resources.Load<GameObject>(tree.name), tree.position, Quaternion.Euler(tree.rotation));
            treePrefab.transform.SetParent(EnvironmentManager.Instance.allTrees.transform);
        }

        // Hayvanlar: sadece kayıtta olanlar sahnede kalır, diğerleri silinir
        foreach (Transform animal in EnvironmentManager.Instance.allAnimals.transform)
        {
            if (animal.GetComponent<Animal>() != null)
            {
                string cleanName = animal.name.Replace("(Clone)", "");
                if (!data.animals.Contains(cleanName))
                {
                    Destroy(animal.gameObject);
                }
            }
        }

        // Mevcut tüm sandıklar sahneden temizlenir
        foreach (Transform s in EnvironmentManager.Instance.placeables.transform)
        {
            Destroy(s.gameObject);
        }

        // Kayıttan gelen her bir sandık sahneye instantiate edilir ve item'ları atanır
        foreach (StorageData storage in data.storage)
        {
            GameObject boxPrefab = Resources.Load<GameObject>("SandıkModel");
            if (boxPrefab != null)
            {
                GameObject b = Instantiate(boxPrefab, storage.position, Quaternion.Euler(storage.rotation));
                b.GetComponent<StorageBox>().items = storage.items;
                b.transform.SetParent(EnvironmentManager.Instance.placeables.transform);
            }
        }
    }


    private void ApplyQuestProgress(List<NPCQuestProgressData> savedData)
    {
        foreach (var npcData in savedData)
        {
            foreach (NPC npc in FindObjectsOfType<NPC>())
            {
                // NPC'yi object adıyla eşleştir
                if (npc.gameObject.name == npcData.npcName)
                {
                    // Aktif görev index'i ve NPC'nin aktiflik durumu yüklenir
                    npc.activeQuestIndex = npcData.activeQuestIndex;
                    npc.gameObject.SetActive(!npcData.npcDeactivated);

                    // NPC'ye ait görevlerin durumu tek tek aktarılır
                    for (int i = 0; i < npcData.quests.Count && i < npc.quests.Count; i++)
                    {
                        var savedQuest = npcData.quests[i];
                        var quest = npc.quests[i];

                        quest.accepted = savedQuest.accepted;
                        quest.declined = savedQuest.declined;
                        quest.initialDialogCompleted = savedQuest.initialDialogCompleted;
                        quest.isCompleted = savedQuest.isCompleted;
                    }
                }
            }
        }
    }

    public PlayerData PeekSaveData(int userId, int slotIndex)
    {
        string path = GetSavePath(userId, slotIndex);
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            AllGameData data = JsonUtility.FromJson<AllGameData>(json);
            return data.playerData;
        }

        return null;
    }
}
