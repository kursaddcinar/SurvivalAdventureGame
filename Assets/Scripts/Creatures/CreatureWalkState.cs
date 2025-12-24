using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CreatureWalkState : StateMachineBehaviour
{
    float timer;                             // Yürüme süresi sayacı
    public float walkingTime = 10f;          // Toplam yürüme süresi
    Transform player;                        // Oyuncunun transform'u
    NavMeshAgent agent;                      // NavMesh ajanı (hareket için)
    public float detectionAreaRadius = 18f;  // Oyuncuyu algılama yarıçapı
    public float walkSpeed = 2f;             // Yürüme hızı
    List<Transform> waypointsList = new List<Transform>(); // Yürünecek noktalar

    // Duruma ilk girildiğinde çalışır (yürümeye başlanır)
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        agent = animator.GetComponent<NavMeshAgent>();

        agent.speed = walkSpeed;
        timer = 0;

        // NPC'nin waypoint kümesini al
        GameObject waypointsCluster = animator.GetComponent<NPCWaypoints>().npcWaypointsCluster;

        // Tüm waypoint noktalarını listeye ekle
        foreach (Transform t in waypointsCluster.transform)
        {
            waypointsList.Add(t);
        }

        // Rastgele bir hedef noktaya git
        Vector3 firstPosition = waypointsList[Random.Range(0, waypointsList.Count)].position;
        agent.SetDestination(firstPosition);
    }

    // Bu durumdayken her karede çalışır
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        // Eğer hedef noktaya ulaşıldıysa, başka bir rastgele noktaya git
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            agent.SetDestination(waypointsList[Random.Range(0, waypointsList.Count)].position);
        }

        // Belirli bir süre yürüdükten sonra idle'a geç
        timer += Time.deltaTime;
        if (timer > walkingTime)
        {
            animator.SetBool("isWalking", false);
        }

        // Oyuncuya yeterince yaklaşıldıysa chase durumuna geç
        float distanceFromPlayer = Vector3.Distance(player.position, animator.transform.position);
        if (distanceFromPlayer < detectionAreaRadius)
        {
            animator.SetBool("isChasing", true);
        }
    }

    // Bu durumdan çıkıldığında çalışır (hedef iptal edilir)
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        agent.SetDestination(agent.transform.position);
    }
}
