using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoBehaviour
{
    // Singleton yapısı: Tüm sahnede yalnızca bir NPCManager olması sağlanır
    public static NPCManager Instance { get; private set; }

    // Editörden atanacak tüm NPC'lerin listesi
    public List<NPC> npcList;

    // Aktif olan NPC'nin listede kaçıncı sırada olduğunu tutar
    private int currentNPCIndex = 0;

    public GameObject girisNPC;

    private void Awake()
    {
        // Singleton kontrolü: Zaten bir örneği varsa bu nesneyi yok et
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    private void Start()
    {
        // Oyunun başında yalnızca aktif olan NPC'yi etkinleştir
        ActivateCurrentNPCOnly();
        if (PlayerState.Instance.hasTriggeredTomurIntro == 0)
            girisNPC.SetActive(true);
    }

    // Sadece listedeki aktif NPC'yi görünür yapar, diğerlerini pasif eder
    private void ActivateCurrentNPCOnly()
    {
        for (int i = 0; i < npcList.Count; i++)
        {
            npcList[i].gameObject.SetActive(i == currentNPCIndex);
        }
    }

    // Görev ilerleyişine göre hangi NPC'nin aktif olacağını belirler
    public void CheckQuestProgressAndAdvance()
    {
        while (currentNPCIndex < npcList.Count)
        {
            NPC currentNPC = npcList[currentNPCIndex];

            // Eğer aktif görev tamamlanmadıysa veya henüz alınmadıysa o NPC aktif kalır
            if (currentNPC.activeQuestIndex < currentNPC.quests.Count &&
                !currentNPC.quests[currentNPC.activeQuestIndex].isCompleted)
            {
                ActivateCurrentNPCOnly();
                return;
            }

            // Eğer görev tamamlandı ama NPC'de başka görev varsa, sıradaki görevi aynı NPC verecek
            if (currentNPC.activeQuestIndex + 1 < currentNPC.quests.Count)
            {
                ActivateCurrentNPCOnly();
                return;
            }

            // Bu NPC'deki tüm görevler bitti → pasif yap ve sıradaki NPC'ye geç
            currentNPC.gameObject.SetActive(false);
            currentNPCIndex++;
        }

        // Eğer hâlâ NPC kaldıysa onu aktif yap
        if (currentNPCIndex < npcList.Count)
        {
            npcList[currentNPCIndex].gameObject.SetActive(true);
        }
        else
        {
            Debug.Log("Tüm NPC görev zinciri tamamlandı.");
        }
    }

    // Mevcut aktif NPC index’ini döner
    public int GetCurrentNPCIndex()
    {
        return currentNPCIndex;
    }

    // NPC index’ini dışarıdan ayarlamak için kullanılır ve o NPC'yi aktif eder
    public void SetCurrentNPCIndex(int index)
    {
        currentNPCIndex = Mathf.Clamp(index, 0, npcList.Count - 1);
        ActivateCurrentNPCOnly();
    }
}
