using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoundationCheck : MonoBehaviour
{
    public Checkpoint foundationCheck; // Bağlı görev verisi (Checkpoint üzerinden)

    private void Update()
    {
        CheckConstructionStatus(); // Her karede inşa durumu kontrol edilir
    }

    void CheckConstructionStatus()
    {
        // Yerleştirilmiş zemin (Foundation) sayısını bul
        int foundationCount = GameObject.FindGameObjectsWithTag("placedFoundation").Length;

        // Yerleştirilmiş duvar (Wall) sayısını bul
        int wallCount = GameObject.FindGameObjectsWithTag("placedWall").Length;

        // Eğer 1 adet zemin ve 4 adet duvar varsa görevi tamamla
        if (foundationCount == 1 && wallCount == 4)
        {
            foundationCheck.isCompleted = true;
            Debug.Log(foundationCheck.name + " görevi tamamlandi!");
        }
        else
        {
            foundationCheck.isCompleted = false;
        }
    }
}
