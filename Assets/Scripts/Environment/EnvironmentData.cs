using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnvironmentData
{
    public List<string> pickedUpItems;     // Oyuncunun sahneden topladığı item'ların isim listesi
    public List<TreeData> treeData;        // Kesilmiş ağaçların konum ve rotasyon bilgileri
    public List<string> animals;           // Öldürülen ya da kaydedilen hayvanların adları
    public List<StorageData> storage;      // Sahnedeki sandıkların içeriği ve konumları

    public EnvironmentData(
        List<string> _pickedUpItems, 
        List<TreeData> _treeData, 
        List<string> _animals, 
        List<StorageData> _storage)
    {
        pickedUpItems = _pickedUpItems;
        treeData = _treeData;
        animals = _animals;
        storage = _storage;
    }
}

[System.Serializable]
public class TreeData
{
    public string name;           // Örneğin: "tree stump"
    public Vector3 position;      // Ağacın konumu
    public Vector3 rotation;      // Ağacın rotasyonu
}

[System.Serializable]
public class StorageData
{
    public List<StoredItemData> items = new List<StoredItemData>(); // Sandık içeriği
    public Vector3 position;     // Sandığın konumu
    public Vector3 rotation;     // Sandığın rotasyonu
}
