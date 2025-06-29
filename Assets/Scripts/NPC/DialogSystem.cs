using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Diyalog sistemini yöneten sınıf.
public class DialogSystem : MonoBehaviour
{
    // Bu sınıfın bir örneğine global olarak erişilmesini sağlayan Singleton yapısı.
    public static DialogSystem Instance { get; set; }

    // UI'de gösterilecek diyalog metni (TextMesh Pro kullanılıyor).
    public TextMeshProUGUI dialogText;

    // Oyuncuya sunulan ilk seçenek butonu.
    public Button option1BTN;

    // Oyuncuya sunulan ikinci seçenek butonu.
    public Button option2BTN;

    // Diyalog kutusunu içeren Canvas referansı.
    public Canvas dialogUI;

    // Diyalog UI ekranının açık mı kapalı mı olduğunu tutar.
    public bool dialogUIActive;

    private void Awake()
    {
        // Singleton kontrolü: Daha önce bir örnek atanmışsa bu nesneyi yok et.
        if((Instance != null && Instance != this))
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    // Diyalog arayüzünü aktif eder ve fare kontrolünü açar.
    public void OpenDialogUI()
    {
        dialogUI.gameObject.SetActive(true); // UI'yi görünür yap.
        dialogUIActive = true;

        // Fareyi görünür yap ve serbest bırak (oyuncunun UI ile etkileşim kurabilmesi için).
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Diyalog arayüzünü kapatır ve fareyi tekrar oyun moduna çeker.
    public void CloseDialogUI()
    {
        dialogUI.gameObject.SetActive(false); // UI'yi gizle.
        dialogUIActive = false;

        // Fareyi kilitle ve görünmez yap (FPS oyun kontrolüne dönüş için).
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
