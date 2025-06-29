using UnityEngine;

// Fırlatılan okların davranışlarını kontrol eden sınıf
public class Arrow : MonoBehaviour
{
    public float speed = 40f;     // Okun uçma hızı
    public float lifeTime = 5f;   // Okun sahnede kalacağı maksimum süre (saniye)
    public int damage = 80;       // Oku isabet eden hedefe verilecek hasar miktarı

    private Rigidbody rb;         // Okun fiziksel hareketlerini yöneten bileşen
    private bool hasHit = false;  // Okun daha önce bir hedefe çarpıp çarpmadığını takip eder

    void Start()
    {
        // Rigidbody bileşenini al ve oku ileri doğru hareket ettir
        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.forward * speed;

        // Belirtilen süre sonunda oku sahneden yok et
        Destroy(gameObject, lifeTime);
    }

    // Ok bir objeye çarptığında çalışır
    void OnCollisionEnter(Collision collision)
    {
        // Eğer ok zaten bir şeye çarptıysa tekrar işlem yapma
        if (hasHit) return;
        hasHit = true;

        // Okun fizik etkileşimini durdur (havada uçmaya devam etmesin)
        rb.isKinematic = true;

        // Oku çarptığı yüzeye sabitle
        transform.position = collision.contacts[0].point;
        transform.rotation = Quaternion.LookRotation(-collision.contacts[0].normal);

        // Eğer çarpılan obje bir hayvansa hasar ver
        Animal animal = collision.gameObject.GetComponent<Animal>();
        if (animal != null)
        {
            // Hasarı uygula
            animal.TakeDamage(damage);

            // Hayvanın fiziksel hareketini sıfırla (itilmeyi engelle)
            Rigidbody animalRb = collision.gameObject.GetComponent<Rigidbody>();
            if (animalRb != null)
            {
                animalRb.velocity = Vector3.zero;
                animalRb.angularVelocity = Vector3.zero;
                animalRb.isKinematic = true;
            }
        }
    }
}
