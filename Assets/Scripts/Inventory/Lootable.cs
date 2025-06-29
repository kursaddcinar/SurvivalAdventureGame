using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Bu script, bir objenin yağmalanabilir (lootable) olmasını sağlar
public class Lootable : MonoBehaviour
{
    public List<LootPossibility> possibleLoot;  // Yağma ihtimali olan eşyaların listesi
    public List<LootRecieved> finalLoot;        // Gerçekleşmiş (rastgele seçilmiş) yağma listesi

    public bool wasLootCalculated;              // Yağma daha önce hesaplandı mı
}

[System.Serializable]
public class LootPossibility
{
    public GameObject item;     // Yağma ihtimali olan obje (örneğin: dal, taş)
    public int amountMin;       // Minimum miktar
    public int amountMax;       // Maksimum miktar
}

[System.Serializable]
public class LootRecieved
{
    public GameObject item;     // Yağmadan elde edilen obje
    public int amount;          // Bu objeden kaç tane alındı
}
