using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Oyuncunun belirli bir köye ulaşıp ulaşmadığını kontrol eden sınıf.
// Bu, bir görevin checkpoint'ini tamamlamak için kullanılır.
public class Village : MonoBehaviour
{
    // Bu script'e atanmış Checkpoint ScriptableObject.
    // Oyuncu bu köye ulaştığında tamamlanmış sayılır.
    public Checkpoint reachVillage_Alex;

    // Oyuncu bu nesneyle çarpıştığında çalışır.
    private void OnTriggerEnter(Collider other)
    {
        // Çarpan nesne oyuncuyu temsil ediyorsa
        if(other.CompareTag("activeConstructable"))
        {
            // İlgili checkpoint tamamlandı olarak işaretlenir.
            reachVillage_Alex.isCompleted = true;
        }
    }
}
