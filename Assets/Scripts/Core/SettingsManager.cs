using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using static SaveManager;

public class SettingsManager : MonoBehaviour
{
    // Singleton erişimi için static örnek
    public static SettingsManager Instance { get; set; }

    // Geri butonu referansı
    public Button backBTN;

    // Ses ayarları için sliderlar ve değerleri gösteren UI nesneleri
    public Slider masterSlider;
    public GameObject masterValue;

    public Slider musicSlider;
    public GameObject musicValue;

    public Slider effectsSlider;
    public GameObject effectsValue;
/*
    private void Start()
    {
        backBTN.onClick.AddListener(()=>
        {
            SaveManager.Instance.SaveVolumeSettings(masterSlider.value, musicSlider.value, effectsSlider.value);
        });

        StartCoroutine(LoadAndApplySettings());
    }

    private IEnumerator LoadAndApplySettings()
    {
        LoadAndSetVolume();

        // Load  Graphics Settings
        //Load Keybindings

        yield return new WaitForSeconds(0.1f);
    }
    
    private void LoadAndSetVolume()
    {
        VolumeSettings volumeSettings = SaveManager.Instance.LoadVolumeSettings();

        masterSlider.value = volumeSettings.master;
        musicSlider.value = volumeSettings.music;
        effectsSlider.value = volumeSettings.effects;
        
    }
    */
     private void Awake()
    {
        // Singleton yapı kurulumu
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Başka bir örnek varsa bunu yok et
        }
        else
        {
            Instance = this;
        }
    }

    private void Update()
    {
        // Her frame'de slider değerlerini UI'ya yazdırır
        masterValue.GetComponent<TextMeshProUGUI>().text = "" + (masterSlider.value) + "";
        musicValue.GetComponent<TextMeshProUGUI>().text = "" + (musicSlider.value) + "";
        effectsValue.GetComponent<TextMeshProUGUI>().text = "" + (effectsSlider.value) + "";
    }
}