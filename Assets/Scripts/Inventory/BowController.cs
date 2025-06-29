using System.Collections;
using UnityEngine;

// Bu script, oyuncunun yay ile ok atmasını kontrol eder
[RequireComponent(typeof(Animator))]
public class BowController : MonoBehaviour
{
    public Animator animator; // Yay animasyonlarını kontrol etmek için

    public GameObject arrowPrefab;         // Instantiate edilecek ok prefabı
    public Transform arrowSpawnPoint;      // Okun çıkacağı konum (yay ucu)
    public float drawTime = 1.0f;          // Yay çekme süresi (germe animasyonu sonrası ok fırlatılır)
    public AudioClip shootSound;           // Ok fırlatma sesi (ekstra efekt için)

    private bool isDrawing = false;        // Yay şu an çekiliyor mu kontrolü
    public GameObject AlertUI;             // "Ok yok" uyarısı gösterilecek UI objesi
    public float alertDuration = 2f;       // Uyarı kaç saniye görünecek

    void Start()
    {
        // Animator component'ini al
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        // Sol tıkla ok atma denemesi
        if (
            Input.GetMouseButtonDown(0) &&                              // Sol tık
            !InventorySystem.Instance.isOpen &&                         // Envanter açık değil
            !CraftingSystem.Instance.isOpen &&                          // Craft sistemi açık değil
            !ConstructionManager.Instance.inConstructionMode &&         // İnşaat modu aktif değil
            !SelectionManager.Instance.handIsVisible &&                 // Oyuncunun elinde bir şey yok
            !isDrawing                                                  // Yay şu an çekilmiyor
        )
        {
            StartCoroutine(DrawAndShoot());
        }
    }

    // Ok atma işlemini başlatan coroutine
    IEnumerator DrawAndShoot()
    {
        // Envanterde ok var mı kontrol et
        int arrowCount = InventorySystem.Instance.CheckItemAmount("Ok");

        if (arrowCount > 0)
        {
            isDrawing = true;

            // Germe sesi çal
            StartCoroutine(drawSoundDelay());

            // Animasyonu başlat
            animator.SetTrigger("Draw");

            // Germe süresi kadar bekle
            yield return new WaitForSeconds(drawTime);

            // Kamera nereye bakıyorsa oraya ray gönder
            Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
            Vector3 targetPoint;

            if (Physics.Raycast(ray, out RaycastHit hit, 100f))
            {
                targetPoint = hit.point; // Gerçek bir hedef varsa
            }
            else
            {
                targetPoint = ray.origin + ray.direction * 100f; // Boşluğa doğru 100 birim ileri
            }

            // Yay ucu hedefe doğru bakacak şekilde döndürülür
            arrowSpawnPoint.LookAt(targetPoint);

            // Oku oluştur
            Instantiate(arrowPrefab, arrowSpawnPoint.position, arrowSpawnPoint.rotation);

            // Envanterden 1 ok eksilt
            InventorySystem.Instance.RemoveOneItem("Ok");

            // Fırlatma sesi çal
            if (shootSound != null)
            {
                AudioSource.PlayClipAtPoint(shootSound, transform.position);
            }

            // Bırakma sesi ve animasyon
            StartCoroutine(realesedSoundDelay());
            animator.SetTrigger("Release");
        }
        else
        {
            // Envanterde ok yoksa uyarı ver
            Debug.Log("Ok yok!");
            DialogManagerGenel.Instance.ShowAlert("Okun kalmadı!");
        }

        isDrawing = false;
    }

    // UI üzerinden ok uyarısını göster (kullanılmıyor, eski sistem)
    void ShowAmmoAlert()
    {
        if (AlertUI == null) return;

        AlertUI.SetActive(true);

        StopAllCoroutines(); // Aynı anda iki uyarı olmaması için
        StartCoroutine(HideAmmoAlertAfterDelay(alertDuration));
    }

    // Uyarıyı belirli süre sonra gizle (kullanılmıyor, yedek sistem)
    IEnumerator HideAmmoAlertAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        AlertUI.SetActive(false);
    }

    // Germe sesi için küçük bir gecikme
    IEnumerator drawSoundDelay()
    {
        yield return new WaitForSeconds(0.02f);
        SoundManager.Instance.PlaySound(SoundManager.Instance.drawSound);
    }

    // Bırakma sesi için küçük bir gecikme
    IEnumerator realesedSoundDelay()
    {
        yield return new WaitForSeconds(0.002f);
        SoundManager.Instance.PlaySound(SoundManager.Instance.releasedSound);
    }
}
