using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StoredItemData
{
    public string itemName;
    public int amount;
}


public class StorageBox : MonoBehaviour
{
    public bool playerInRange; // Oyuncu kutuya yakın mı

    public List<StoredItemData> items = new List<StoredItemData>();  // Kutunun içinde bulunan item adları

    public enum BoxType
    {
        smallBox,  // Küçük sandık
        BigBox     // Büyük sandık
    }

    public BoxType thisBoxType; // Bu kutunun tipi

    private void Update()
    {
        // Oyuncuyla sandık arasındaki mesafeyi ölç
        float distance = Vector3.Distance(PlayerState.Instance.playerBody.transform.position, transform.position);

        // Eğer oyuncu 10 birimden yakınsa erişime açık olur
        if (distance < 10f)
        {
            playerInRange = true;
        }
        else
        {
            playerInRange = false;
        }
    }
}
