using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Bu sınıf, sahne üzerinde görsel olarak NPC'nin etki alanlarını göstermek için kullanılır.
public class NPCGizmos : MonoBehaviour
{
    // Unity Editörü'nde sahne görünümünde çizim yapılmasını sağlar.
    //katsayı
    public int paremeter = 1;
    private void OnDrawGizmos()
    {
        // Saldırı mesafesi (kırmızı): Oyuncuya bu mesafede saldırmaya başlar.
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 7f*paremeter); // Attacking Distance

        // Takip başlatma mesafesi (mavi): Oyuncu bu mesafeye girerse takip etmeye başlar.
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, 20*paremeter); // Start chasing Distance

        // Takibi bırakma mesafesi (yeşil): Oyuncu bu mesafeyi aşarsa takip etmeyi bırakır.
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 23f*paremeter); // Stop chasing Distance
    }
}
