using System;  
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Oyuncuya bir uyarı (Alert) veya onay kutusu göstermek için kullanılan UI yöneticisi
public class AlertDialogManager2 : MonoBehaviour
{
    public GameObject dialogBox;                    // Diyalog kutusunun tamamını temsil eden GameObject
    public TextMeshProUGUI messageText;             // Diyalog kutusunda gösterilecek mesaj metni
    public Button okButton;                         // 'Tamam' (OK) butonu
    public Button cancelButton;                     // 'İptal' (Cancel) butonu

    private System.Action<bool> responceCallback;   // Butona basıldığında dışarıya geri bildirim gönderecek callback fonksiyonu

    private void Start()
    {
        dialogBox.SetActive(false);                 // Başlangıçta diyalog kutusu kapalıdır
        okButton.onClick.AddListener(() => HandleResponse(true));      // OK butonuna basılırsa true döner
        cancelButton.onClick.AddListener(() => HandleResponse(false)); // Cancel butonuna basılırsa false döner
    }

    // Diyalog kutusunu ekranda gösterir
    public void ShowDialog(string message, System.Action<bool> callback)
    {
        responceCallback = callback;                // Geri çağırma fonksiyonu atanır
        messageText.text = message;                 // Mesaj kutusuna yazı yerleştirilir
        dialogBox.SetActive(true);                  // UI açılır
    }

    // Oyuncunun verdiği cevaba göre geri çağırma fonksiyonu tetiklenir
    private void HandleResponse(bool responce)
    {
        dialogBox.SetActive(false);                 // Diyalog kutusu kapatılır
        responceCallback?.Invoke(responce);         // Cevap varsa geri bildirim olarak gönderilir (null kontrolü)
    }
}
