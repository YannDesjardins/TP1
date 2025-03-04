﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviorPicker : MonoBehaviour {

    public GameObject bulletPrefab;
    public AudioClip bulletSound;

	// Use this for initialization
	void Start () {
        PickBehavior();
	}

    private void PickBehavior()
    {
        if (StaticGameStats.Difficulty == 1)
        {
            BadRangedEnemyBehavior behavior = gameObject.AddComponent<BadRangedEnemyBehavior>();
            behavior.bulletPrefab = this.bulletPrefab;
            gameObject.GetComponent<Health>().healthRegen = 5;
            behavior.shootRange = 10f;
            behavior.shootRange = 15f;
            behavior.rotationSpeed = 10f;
            behavior.bulletSound = bulletSound;
        }
        if (StaticGameStats.Difficulty == 2)
        {
            RangedEnemyBehavior behavior = gameObject.AddComponent<AdvancedRangedEnemyBehavior>();
            gameObject.GetComponent<Health>().healthRegen = 5;
            behavior.bulletPrefab = this.bulletPrefab;
            behavior.shootRange = 15f;
            behavior.rotationSpeed = 10f;
            behavior.bulletSound = bulletSound;
        }
    }
}
