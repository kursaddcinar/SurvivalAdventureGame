using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CreatureAttackState : StateMachineBehaviour
{
    Transform player;              // Oyuncuya ait transform (activeConstructable tag'li)
    NavMeshAgent agent;            // Hareket için kullanılan NavMesh agent
    public float stopAttackingDistance = 7.1f; // Bu mesafeden fazla uzaklaşırsa saldırmayı bırakır
    public float attackRate = 1f;              // Saldırı sıklığı (saniyede 1 kez)
    private float attackTimer;                // Saldırı zamanlayıcısı
    public int damageToInflict = 1;           // Verilecek hasar

    // Duruma ilk girildiğinde (saldırı animasyonu başlarken) çalışır
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Oyuncuyu bul
        player = GameObject.FindGameObjectWithTag("activeConstructable").transform;
        // Agent bileşenini al
        agent = animator.GetComponent<NavMeshAgent>();
    }

    // Her frame'de çalışır (bu durumdayken)
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        LooakAtPlayer(); // Yaratık oyuncuya bakar

        // Saldırı zamanlayıcısı dolduysa saldırı yap
        if (attackTimer <= 0)
        {
            Attack(); // Hasar uygula
            attackTimer = 1f / attackRate; // Zamanlayıcıyı sıfırla
        }
        else
        {
            attackTimer -= Time.deltaTime; // Zamanlayıcıyı azalt
        }

        // Eğer oyuncu saldırı mesafesinin dışına çıkarsa saldırı durumu sonlanır
        float distanceFromPlayer = Vector3.Distance(player.position, animator.transform.position);
        if (distanceFromPlayer > stopAttackingDistance)
        {
            animator.SetBool("isAttacking", false);
        }
    }

    // Durumdan çıkıldığında (örneğin saldırıdan başka bir animasyona geçilince) çalışır
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Şu an özel bir çıkış davranışı tanımlanmamış
    }

    // Yaratık sürekli oyuncuya bakar
    public void LooakAtPlayer()
    {
        Vector3 direction = player.position - agent.transform.position;
        agent.transform.rotation = Quaternion.LookRotation(direction);

        var yRotation = agent.transform.eulerAngles.y;
        agent.transform.rotation = Quaternion.Euler(0, yRotation, 0); // Sadece Y ekseninde döndür
    }

    // Saldırı işlemi: ses çalınır ve oyuncuya hasar verilir
    private void Attack()
    {
        Animal animalComponent = agent.gameObject.GetComponent<Animal>();
        if (animalComponent != null)
        {
            animalComponent.PlayAttackSound();

            int damage = animalComponent.GetAttackDamage(); // hayvana özel hasar değeri al
            PlayerState.Instance.TakeDamage(damage); // oyuncuya uygula
        }
    }

}
