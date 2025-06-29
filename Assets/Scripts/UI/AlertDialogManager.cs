// AlertDialogManager.cs
// Bu script, sahnedeki tek bir panel ile tüm onay kutularını (Evet/Hayır) gösterip yönetir.
// Singleton yapısı sayesinde tüm sahnelerde merkezi olarak kullanılabilir.

using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class AlertDialogManager : MonoBehaviour
{
    public static AlertDialogManager Instance { get; private set; }

    public GameObject panel;                   // Uyarı paneli (genel çerçeve)
    public TextMeshProUGUI messageText;        // Uyarı mesajının yazılacağı alan
    public Button confirmButton;               // "Evet"/"Onayla" butonu
    public Button cancelButton;                // "Hayır"/"İptal" butonu
    public Button okeyButton;                // "Tamam" butonu
    private Action onConfirm;                  // Evet'e basıldığında çalışacak olan fonksiyon

    void Awake()
    {
        // Singleton kur
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        panel.SetActive(false); // Başlangıçta panel gizli olsun
    }

    // Sadece bilgi gösterimi için tek butonlu uyarı paneli
    public void ShowInfo(string message)
    {
        panel.SetActive(true);
        messageText.text = message;
        onConfirm = null;

        confirmButton.onClick.RemoveAllListeners();
        cancelButton.onClick.RemoveAllListeners();

        // Sadece confirm (Tamam) butonunu göster
        confirmButton.gameObject.SetActive(false);
        cancelButton.gameObject.SetActive(false);

        okeyButton.gameObject.SetActive(true);

        okeyButton.onClick.AddListener(Close);
    }



    // Uyarı panelini göster ve onaylandığında çalışacak callback fonksiyonunu ayarla
    public void Show(string message, Action onConfirmCallback)
    {
        panel.SetActive(true);                 // Paneli aktif et
        messageText.text = message;           // Mesajı yaz
        onConfirm = onConfirmCallback;        // Callback'i sakla

        // Önceki tıklama fonksiyonlarını temizle
        confirmButton.onClick.RemoveAllListeners();
        cancelButton.onClick.RemoveAllListeners();

        
        confirmButton.gameObject.SetActive(true);
        cancelButton.gameObject.SetActive(true);
        okeyButton.gameObject.SetActive(false);

        // "Evet" butonuna basıldığında callback'i çağır, sonra paneli kapat
        confirmButton.onClick.AddListener(() =>
        {
            onConfirm?.Invoke();
            Close();
        });

        // "Hayır" butonuna basıldığında sadece paneli kapat
        cancelButton.onClick.AddListener(Close);
    }

    // Paneli kapat
    private void Close()
    {
        panel.SetActive(false);
    }
}
