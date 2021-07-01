using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemChanger.Util;

namespace ItemChanger.Components
{
    public class SpawnOnDeath : MonoBehaviour
    {
        public GameObject item;
        HealthManager hm;
        bool flung;

        public void Awake()
        {
            hm = GetComponent<HealthManager>();
            if (hm == null) throw new MissingComponentException($"DropItemOnDeath on object {gameObject.name} in {GameManager.instance.sceneName} could not find HealthManager.");

            hm.OnDeath += Fling;
        }

        public void OnDisable()
        {
            if (hm.isDead && !flung) Fling();
        }

        private void Fling()
        {
            if (flung) return;
            flung = true;
            item.SetActive(true);
            Rigidbody2D rb = item.GetComponent<Rigidbody2D>() ?? item.AddComponent<Rigidbody2D>();
            rb.position = gameObject.transform.position;
            item.transform.position = gameObject.transform.position;
            rb.velocity = Vector2.zero;
        }
    }
}
