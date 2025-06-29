using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Oyun içindeki ses efektlerini ve arka plan müziklerini yöneten sınıf.
public class SoundManager : MonoBehaviour
{
    // Singleton yapı: Oyunda sadece 1 tane SoundManager olması için
    public static SoundManager Instance { get; set; }

    // --- SES EFEKTLERİ --- //
    public AudioSource dropItemSound;      // Eşya yere bırakıldığında çalacak ses
    public AudioSource toolSwingSound;     // Oyuncu bir aracı salladığında çıkan ses
    public AudioSource craftingSound;      // Craft işlemi gerçekleştiğinde çıkan ses
    public AudioSource chopSound;          // Ağaç kesme sesi
    public AudioSource pickUpItemSound;    // Eşya toplama sesi
    public AudioSource grassWalkSound;     // Oyuncu çimen üzerinde yürürken çıkan ses

    // --- ARKA PLAN MÜZİĞİ --- //
    public AudioSource startingZoneBGMusic; // Başlangıç bölgesinde çalan müzik
    public AudioSource drawSound;          // yay germe sesi
    public AudioSource releasedSound;    // yay serbest bırakma sesi

    // Singleton başlatma ve kontrol
    public void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Daha önce başka bir örnek varsa bunu yok et
        }
        else
        {
            Instance = this;
        }
    }

    // Bir ses efektini çalmak için kullanılır
    public void PlaySound(AudioSource soundToPlay)
    {
        // Ses zaten çalmıyorsa başlat
        if (soundToPlay.isPlaying == false)
        {
            soundToPlay.Play();
        }
    }
}
