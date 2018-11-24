﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class RangedEnemyBehavior : EnemyBehavior
{
    public float shootRange = 20;
    public float fleeingRange = 15;
    public float shootingSpeed = 0.1f;
    public Transform[] bulletSpawn;
    public GameObject bulletPrefab;

    private float lastShot = 0;

    
    protected override void ChasePlayer()
    {
        //State action
        target = FindClosestPlayer();
        moveTarget = target;

        enemyAction += MoveTowardTarget;
        if (IsPlayerWithinSight())
        {
            enemyMovement += ShootTarget;
        }

        //State Check
        if (FindPlayerVisible() && FindPlayerWithinRange(shootRange))
        {
            stateAction = AttackPlayer;
        }
        if (!FindPlayerVisible() || !FindPlayerWithinRange(detectionRange))
        {
            if (IsHidden())
            {
                stateAction = Patrol;
            }
        }
        else
        {
            timeHidden = 0;
        }
        if (IsLowOnHealth())
        {
            stateAction = PlaySafe;
        }
    }

    void AttackPlayer()
    {
        //State action
        target = FindClosestPlayer();
        moveTarget = target;

        enemyAction += RotateTowardTarget;
        enemyAction += ShootTarget;
        //State Check

        if (IsLowOnHealth())
        {
            stateAction = PlaySafe;
        }
        if (!IsPlayerWithinSight())
        {
            stateAction = ChasePlayer;
        }
    }

    void PlaySafe()
    {
        //State Action
        target = FindClosestPlayer();
        enemyAction = MoveBackward;
        if (IsPlayerWithinSight())
        {
            enemyAction += ShootTarget;
        }

        //State Check
        if (!IsLowOnHealth())
        {
            stateAction = Patrol;
        }
        if (FindPlayerVisible())
        {
            stateAction = RunAway;
        }
    }


    void RunAway()
    {
        if (true)
        {
            stateAction = Patrol;
        }
        if (true)
        {
            stateAction = PlaySafe;
        }
    }
    protected void ShootTarget(Vector3 target)
    {
        lastShot += Time.deltaTime;
        if (lastShot > shootingSpeed)
        {
            lastShot = 0;
            CmdFire();
        }
    }

    [Command]
    void CmdFire()
    {
        for (int i = 0; i < bulletSpawn.Length; i++)
        {
            lastShot = 0;
            var bullet = (GameObject)Instantiate(
            bulletPrefab,
            bulletSpawn[i].position,
            bulletSpawn[i].rotation);

            bullet.GetComponent<Rigidbody>().velocity = bullet.transform.forward * 30;

            NetworkServer.Spawn(bullet);

            Destroy(bullet, 5.0f);
        }
    }
}
