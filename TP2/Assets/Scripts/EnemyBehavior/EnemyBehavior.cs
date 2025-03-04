﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Networking;

public abstract class EnemyBehavior : NetworkBehaviour
{
    protected delegate void StateAction();
    protected StateAction stateAction;

    protected delegate void EnemyAction(Vector3 position);
    protected EnemyAction enemyAction;

    protected delegate void EnemyMovement(Vector3 position);
    protected EnemyMovement enemyMovement;

    public float speed = 5;
    public float rotationSpeed = 5f;
    public float detectionRange = 20;
    public float aggroTime = 2f;
    public float lowHealthThreshold = 30f;

    protected float timeSpotted = 0;
    protected float timeHidden = 0;
    protected Health health;
    protected EnemyPathfinder pathfinder;
    protected PatrolPath patrolPath;
    protected GameObject[] players;
    protected List<GameObject> playersVisible = new List<GameObject>();
    protected Vector3 target;
    protected Vector3 moveTarget;
    protected EnemiesSituation enemiesSituation;

    // Use this for initialization
    protected void Start()
    {
        enemiesSituation = GameObject.FindGameObjectWithTag("EnemyHandler").GetComponent<EnemiesSituation>();
        health = GetComponent<Health>();
        stateAction = Patrol;

        patrolPath = GetComponent<PatrolPath>();
        pathfinder = GetComponent<EnemyPathfinder>();
    }

    // Update is called once per frame
    void Update()
    {
        players = GameObject.FindGameObjectsWithTag("Player");
        stateAction();
        if (enemyAction != null)
        {
            enemyAction(target);
        }
        enemyAction = null;
        if (enemyMovement != null)
        {
            enemyMovement(moveTarget);
        }
        enemyMovement = null;
    }

    protected abstract void ChasePlayer();

    protected void Patrol()
    {
        moveTarget = patrolPath.Patrol();
        enemyMovement += MoveTowardTarget;
        if (FindPlayerVisible() || AnyPlayerWithinRange(detectionRange))
        {
            if (IsFound())
            {
                patrolPath.StopPatrol();
                enemiesSituation.IncreaseAlertedEnemies();
                stateAction = ChasePlayer;
            }
        }
        else
        {
            timeSpotted = 0;
        }
    }

    protected bool AnyPlayerWithinRange(float range)
    {
        int playerInRange = players.Where(p => (p.transform.position - transform.position).magnitude <= range).ToArray().Length;
        return playerInRange>0;
    }

    protected Vector3 FindClosestPlayer()
    {
        GameObject min = players[0];
        float minDistance = (min.transform.position - transform.position).magnitude;
        foreach (GameObject go in players)
        {
            float distance = (go.transform.position - transform.position).magnitude;
            if (distance < minDistance)
            {
                minDistance = distance;
                min = go;
            }
        }
        return min.transform.position;
    }

    protected Vector3 FindClosestPlayer(out GameObject player)
    {
        GameObject min = players[0];
        float minDistance = (min.transform.position - transform.position).magnitude;
        foreach (GameObject go in players)
        {
            float distance = (go.transform.position - transform.position).magnitude;
            if (distance < minDistance)
            {
                minDistance = distance;
                min = go;
            }
        }
        player = min;
        return min.transform.position;
    }

    protected bool IsHidden()
    {
        bool hidden = false;
        timeHidden += Time.deltaTime;
        if(timeHidden > aggroTime)
        {
            hidden = true;
            timeHidden = 0;
        }
        return hidden;
    }

    protected bool IsFound()
    {
        bool found = false;
        timeSpotted += Time.deltaTime;
        if (timeSpotted > aggroTime)
        {
            found = true;
            timeHidden = 0;
        }
        return found;
    }

    protected Vector3 FindMovementTarget(Vector3 t)
    {
        /*List<Vector3> path = pathfinder.DijkstraFindPath(transform.position, t);
        if (path.Count > 0)
        {
            return path[0];
        }
        else
        {
            return t;
        }*/
        return t;
    }

    protected bool IsPlayerWithinSight()
    {
        RaycastHit hit;
        Physics.Raycast(transform.position, transform.forward, out hit);
        return hit.collider.tag == "Player";
    }

    protected bool FindPlayerVisible()
    {
        playersVisible.Clear();

        RaycastHit hit;
        foreach(GameObject player in players)
        {
            Vector3 direction = (player.transform.position - transform.position).normalized;
            if (Physics.Raycast(transform.position, direction, out hit))
            {
                if (hit.collider.tag == "Player")
                {
                    playersVisible.Add(player);
                }
            }
        }
        if (playersVisible.Count > 0)
        {
            timeSpotted += aggroTime;
        }
        return playersVisible.Count > 0;
    }

    protected bool IsLowOnHealth()
    {
        return health.currentHealth <= lowHealthThreshold;
    }

    protected void MoveTowardTarget(Vector3 target)
    {
        Vector3 moveTarget = FindMovementTarget(target);
        transform.rotation = Quaternion.Slerp(transform.rotation,
               Quaternion.LookRotation(moveTarget - transform.position), rotationSpeed * Time.deltaTime);
        transform.position += transform.forward * speed * Time.deltaTime;
    }

    protected void MoveBackward(Vector3 target)
    {
        Vector3 moveTarget = FindMovementTarget(target);
        transform.rotation = Quaternion.Slerp(transform.rotation,
               Quaternion.LookRotation(moveTarget - transform.position), rotationSpeed * Time.deltaTime);
        transform.position -= transform.forward * speed/2 * Time.deltaTime;
    }

    protected void RotateTowardTarget(Vector3 target)
    {
        Vector3 moveTarget = FindMovementTarget(target);
        transform.rotation = Quaternion.Slerp(transform.rotation,
               Quaternion.LookRotation(moveTarget - transform.position), rotationSpeed * Time.deltaTime);
    }
}
