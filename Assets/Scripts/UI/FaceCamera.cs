using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Bu script, objenin her zaman kameraya dönük olmasını sağlar.
// Genellikle dünya üzerindeki UI yazıları için kullanılır (örneğin oyuncu ismi, uyarı metni).
public class FaceCamera : MonoBehaviour
{
    private Transform localTrans; // Bu objenin Transform referansı

    // Başlangıçta bu objenin transform bileşeni alınır
    void Start()
    {
        localTrans = GetComponent<Transform>();
    }

    // Her karede objeyi kameraya bakacak şekilde döndür
    void Update()
    {
        // Eğer sahnede ana kamera varsa işlem yap
        if (Camera.main)
        {
            // Kamera yönüne bakacak şekilde objeyi döndür
            // Bu ifade objeyi kameraya doğru döndürür ama tam olarak ters yöne değil, "kamera tarafından görülür" şekilde hizalar
            localTrans.LookAt(2 * localTrans.position - Camera.main.transform.position);
        }
    }
}
