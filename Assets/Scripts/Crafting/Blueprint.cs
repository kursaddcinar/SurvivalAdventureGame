using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blueprint : MonoBehaviour
{
    public string itemName; // Üretilecek ürünün adı

    public string Req1;     // Gerekli 1. malzeme adı
    public string Req2;     // Gerekli 2. malzeme adı

    public int Req1amount;  // Gerekli 1. malzeme miktarı
    public int Req2amount;  // Gerekli 2. malzeme miktarı

    public int numOfRequirements;     // Gerekli toplam malzeme çeşidi sayısı (1 veya 2 olabilir)
    public int numOfItemsToProduce;   // Üretilecek ürün sayısı

    // Yapıcı (constructor) - üretim tarifinin verilerini ayarlar
    // Parametreler sırasıyla: ürün adı, üretilecek ürün sayısı, ihtiyaç duyulan malzeme türü sayısı,
    // 1. malzeme adı, 1. malzeme adedi, 2. malzeme adı, 2. malzeme adedi
    public Blueprint(string name, int producedItems, int reqNUM, string R1, int R1num, string R2, int R2num)
    {
        itemName = name;
        numOfRequirements = reqNUM;
        numOfItemsToProduce = producedItems;
        Req1 = R1;
        Req2 = R2;
        Req1amount = R1num;
        Req2amount = R2num;
    }
}
