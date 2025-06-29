using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Her NPC'nin görev ilerlemesini temsil eden veri yapısı.
[System.Serializable]
public class NPCQuestProgressData
{
    public string npcName; // NPC'nin adı. Eşleşme için kullanılır.

    public int activeQuestIndex; // NPC'deki şu anki aktif görev indeksini belirtir.

    public List<QuestSaveData> quests = new List<QuestSaveData>(); // NPC'nin sahip olduğu görevlerin kaydedilmiş durumları.

    public bool npcDeactivated = false; // Tüm görevler bittiyse, bu NPC artık pasif olur.
}
