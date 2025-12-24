using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CreatureChaseState : StateMachineBehaviour
{
    Transform player;             // Oyuncu (Player tag'li nesne)
    NavMeshAgent agent;           // NavMesh üzerinden hareket sağlayan bileşen
    public float chaseSpeed = 6f; // Kovalama hızı
    float timer;
    public float stopChasingDistance = 21f;  // Bu mesafeden uzaklaşırsa kovalamayı bırakır
    public float attackingDistance = 2.5f;   // Bu mesafeye girerse saldırıya geçer

    // Bu duruma ilk girildiğinde (animasyon başlarken) çalışır
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Oyuncu ve ajan referansları alınır
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = animator.GetComponent<NavMeshAgent>();

        agent.speed = chaseSpeed; // Ajanın hızı kovalamaya göre ayarlanır

        // Eğer ajan NavMesh dışında bir konumdaysa, en yakın NavMesh konumuna taşınır
        NavMeshHit hit;
        if (NavMesh.SamplePosition(agent.transform.position, out hit, 10.0f, NavMesh.AllAreas))
        {
            agent.Warp(hit.position); // Ajan güvenli NavMesh alanına yerleştirilir
        }
        else
        {
            Debug.LogError("Hata: Yakındaki NavMesh bulunamadı! NavMesh Bake işlemini kontrol edin.");
        }
    }

    // Bu durum aktifken her karede çalışır
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Hedef olarak oyuncu belirlenir
        agent.SetDestination(player.position);
        animator.transform.LookAt(player); // Yaratık oyuncuya döner

        float distanceFromPlayer = Vector3.Distance(player.position, animator.transform.position);

        // Eğer oyuncu çok uzaksa, kovalamayı bırak
        if (distanceFromPlayer > stopChasingDistance)
        {
            animator.SetBool("isChasing", false);
        }

        // Eğer oyuncuya yeterince yaklaştıysa, saldırıya geç
        if (distanceFromPlayer < attackingDistance)
        {
            animator.SetBool("isAttacking", true);
        }
    }

    // Bu durumdan çıkıldığında çalışır
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Ajanın hareketi durdurulur (geçici olarak pozisyonuna hedef atanır)
        agent.SetDestination(agent.transform.position);
    }
}
