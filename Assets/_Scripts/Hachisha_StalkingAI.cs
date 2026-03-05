using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class Hachisha_StalkingAI : MonoBehaviour
{
    public Transform[] targets; // Player + Friends
    public float stalkDistance = 15f;
    public float killDistance = 2f;
    public float teleportDistance = 50f;
    public float moveSpeed = 1.5f;
    public float pursuitSpeed = 4.0f;
    
    [Header("Visibility Settings")]
    public float crouchDetectionMultiplier = 0.3f;
    public float friendProximityPenalty = 15f; // 友人がこの距離内にいると隠密が無効化されやすい

    private NavMeshAgent agent;
    private Transform currentTarget;
    private MeshRenderer meshRenderer;
    private bool isPlayerInSafeZone = false;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        meshRenderer = GetComponent<MeshRenderer>();
        // ターゲットが未設定ならPlayerを探す
        if (targets == null || targets.Length == 0)
        {
            GameObject p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) targets = new Transform[] { p.transform };
        }
    }

    /// <summary>
    /// SafeZone からの入退場通知。SafeZone.cs から呼ばれる。
    /// true: プレイヤーが安全地帯に入った → 追跡停止・姿を消す
    /// false: 安全地帯を出た → 追跡再開
    /// </summary>
    public void SetSafeStatus(bool isSafe)
    {
        isPlayerInSafeZone = isSafe;
        if (agent != null) agent.isStopped = isSafe;
        // 安全地帯内では姿を消す（霧の中に溶け込む演出）
        if (meshRenderer != null) meshRenderer.enabled = !isSafe;
    }

    void Update()
    {
        // プレイヤーが安全地帯にいる間は一切の追跡をしない
        if (isPlayerInSafeZone) return;

        UpdateTarget();
        if (currentTarget == null) return;

        float distance = Vector3.Distance(transform.position, currentTarget.position);

        // 状態更新
        if (distance > teleportDistance)
        {
            TeleportNearTarget();
        }
        else if (IsTargetVisible(currentTarget))
        {
            PursueTarget(distance);
        }
        else
        {
            StalkTarget(distance);
        }

        // Kill Check
        if (distance < killDistance)
        {
            CatchTarget(currentTarget);
        }
    }

    void UpdateTarget()
    {
        float minScore = float.MaxValue;
        Transform bestTarget = null;

        foreach (Transform t in targets)
        {
            if (t == null) continue;
            
            float dist = Vector3.Distance(transform.position, t.position);
            float score = dist;

            // しゃがみ補正 (プレイヤーのみを想定)
            Hachisha_PlayerController pc = t.GetComponent<Hachisha_PlayerController>();
            if (pc != null && pc.isCrouching)
            {
                // 隠れているつもりでも、近くに友人がいたらバレる
                bool friendNear = false;
                foreach (Transform f in targets)
                {
                    if (f != t && Vector3.Distance(t.position, f.position) < friendProximityPenalty)
                    {
                        friendNear = true;
                        break;
                    }
                }
                
                if (!friendNear) score /= crouchDetectionMultiplier;
            }

            if (score < minScore)
            {
                minScore = score;
                bestTarget = t;
            }
        }
        currentTarget = bestTarget;
    }

    bool IsTargetVisible(Transform target)
    {
        float dist = Vector3.Distance(transform.position, target.position);
        
        // 懐中電灯チェック（プレイヤーの場合）
        float detectionRange = stalkDistance;
        Hachisha_PlayerController pc = target.GetComponent<Hachisha_PlayerController>();
        if (pc != null && pc.flashlight != null && pc.flashlight.enabled)
        {
            detectionRange *= 2.0f; // 懐中電灯をつけていると2倍の距離でバレる
        }

        if (dist > detectionRange) return false;

        // Raycast による視線（LOS）チェック
        Vector3 direction = (target.position + Vector3.up * 1.5f) - (transform.position + Vector3.up * 1.5f);
        RaycastHit hit;
        if (Physics.Raycast(transform.position + Vector3.up * 1.5f, direction, out hit, detectionRange))
        {
            if (hit.transform == target || hit.transform.root == target.root)
            {
                return true; // 遮蔽物なしで見えている
            }
        }

        return false;
    }

    void PursueTarget(float distance)
    {
        agent.isStopped = false;
        agent.speed = pursuitSpeed;
        agent.SetDestination(currentTarget.position);
        meshRenderer.enabled = true;
    }

    void StalkTarget(float distance)
    {
        // 距離を保ちつつ監視
        if (distance > stalkDistance + 5f)
        {
            agent.isStopped = false;
            agent.speed = moveSpeed;
            agent.SetDestination(currentTarget.position);
        }
        else
        {
            agent.isStopped = true;
            transform.LookAt(new Vector3(currentTarget.position.x, transform.position.y, currentTarget.position.z));
        }
        // 隠れている時は姿を消す演出もあり
    }

    void TeleportNearTarget()
    {
        Vector2 randomCircle = Random.insideUnitCircle.normalized * 30f;
        Vector3 newPos = currentTarget.position + new Vector3(randomCircle.x, 0, randomCircle.y);
        NavMeshHit hit;
        if (NavMesh.SamplePosition(newPos, out hit, 10f, NavMesh.AllAreas))
        {
            transform.position = hit.position;
        }
    }

    void CatchTarget(Transform target)
    {
        if (target.CompareTag("Player"))
        {
            Hachisha_GameManager.Instance.LoseGame("Captured by Hachishakusama.");
        }
        else
        {
            Debug.Log(target.name + " was taken away...");
            target.gameObject.SetActive(false); // 友人が連れ去られる
        }
    }
}
