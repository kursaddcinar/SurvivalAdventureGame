using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ScriptableObject olarak tanımlanmış bir Checkpoint sınıfı.
// Bu sınıf, görevlerde oyuncunun ulaşması veya tamamlaması gereken belirli kontrol noktalarını temsil eder.
[CreateAssetMenu(fileName = "Checkpoint", menuName = "ScriptableObjects/Checkpoint", order = 1)]
public class Checkpoint : ScriptableObject
{
    // Kontrol noktasının adı
    public string checkpointName;

    // Bu kontrol noktasının tamamlanıp tamamlanmadığını belirten bayrak
    public bool isCompleted = false;
}
