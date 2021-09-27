using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ItemChanger.Components
{
    public class DropIntoPlace : MonoBehaviour
    {
        Rigidbody2D rb;
        public event Action OnLand;

        public void Awake()
        {
            rb = gameObject.GetComponent<Rigidbody2D>();
            if (rb == null) rb = gameObject.AddComponent<Rigidbody2D>();
        }

        public void OnEnable()
        {
            rb.constraints = RigidbodyConstraints2D.FreezeRotation | RigidbodyConstraints2D.FreezePositionX;
            AccelerationMonitor am = gameObject.GetComponent<AccelerationMonitor>();
            if (am == null) am = gameObject.AddComponent<AccelerationMonitor>();
            StartCoroutine(DetectLanding(rb, am));
        }

        private IEnumerator DetectLanding(Rigidbody2D rb, AccelerationMonitor am)
        {
            yield return new WaitForSeconds(0.05f); // free fall
            while (am.IsFalling())
            {
                yield return null;
            }
            OnLand?.Invoke();
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }
    }
}
