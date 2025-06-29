using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Oyuncunun hareket ve bakış kontrolünü yöneten sınıf
public class MovementManager : MonoBehaviour
{
    // Singleton Instance
    public static MovementManager Instance { get; private set; } 

    // Oyuncunun hareket edip edemeyeceğini belirler
    public bool canMove = true;

    // Oyuncunun fare ile etrafa bakıp bakamayacağını belirler
    public bool canLookAround = true;

    private void Awake()
    {
        // Singleton kurulumu
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;
    }

    // Hareketi aktif/pasif hale getirir
    public void EnableMovement(bool trigger)
    {
        canMove = trigger;
    }

    // Bakış kontrolünü aktif/pasif hale getirir
    public void EnableLook(bool trigger)
    {
        canLookAround = trigger;
    }
}
