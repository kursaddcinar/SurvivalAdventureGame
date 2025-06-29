using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnvironmentManager : MonoBehaviour
{
    // Singleton instance
    public static EnvironmentManager Instance { get; set; }

    // Ortamdaki tüm eşya prefablarının/objelerinin tutulduğu ana obje
    public GameObject allItems;

    // Ağaçlar, hayvanlar ve yerleştirilebilir objeleri kapsayan referanslar
    public GameObject allTrees;
    public GameObject allAnimals;
    public GameObject placeables;

    private void Awake()
    {
        // Singleton kontrolü
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Zaten bir instance varsa bu nesne yok edilir
        }
        else
        {
            Instance = this; // Singleton olarak kendini atar
        }
    }
}
