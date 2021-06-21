using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using ItemChanger.Util;

namespace ItemChanger.Components
{
    public class DropItemOnDeath : MonoBehaviour
    {
        public GameObject item;
        public Container container;
        public FlingType flingType;
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
            flung = true;
            Rigidbody2D rb = item.GetComponent<Rigidbody2D>() ?? item.AddComponent<Rigidbody2D>();
            AccelerationMonitor am = item.GetComponent<AccelerationMonitor>() ?? item.AddComponent<AccelerationMonitor>();

            item.SetActive(true);
            rb.position = gameObject.transform.position;
            if (flingType != FlingType.Everywhere)
            {
                rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX;
                rb.velocity = new Vector2(0, -1f);
            }
            else
            {
                rb.constraints = RigidbodyConstraints2D.FreezeRotation;
                rb.velocity = new Vector2(UnityEngine.Random.Range(-5, 5), UnityEngine.Random.Range(10f, 20f));
            }
            am.StartCoroutine(DetectLanding(rb, am));
        }

        private IEnumerator DetectLanding(Rigidbody2D rb, AccelerationMonitor am)
        {
            yield return new WaitForSeconds(0.05f);
            while (am.IsFalling())
            {
                yield return null;
            }
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }
    }
}
