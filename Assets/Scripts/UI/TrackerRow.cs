using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

// Takip edilen görevler listesindeki satırı temsil eder.
// Bu script, görev ismi, açıklaması ve gereksinimlerini gösterir.
public class TrackerRow : MonoBehaviour
{
    // Görev adı
    public TextMeshProUGUI questName;

    // Görev açıklaması
    public TextMeshProUGUI description;

    // Görev için gerekli itemlar veya checkpoint'leri gösteren alan
    public TextMeshProUGUI requirements;
}
