using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ItemChanger.Components
{
    /// <summary>
    /// Component which tracks the per-frame acceleration of a RigidBody to determine whether it is falling.
    /// </summary>
    public class AccelerationMonitor : MonoBehaviour
    {
        Rigidbody2D rb;
        Vector2 v;
        Vector2 dv;


        public void Awake()
        {
            rb = GetComponent<Rigidbody2D>();
            if (rb == null) rb = gameObject.AddComponent<Rigidbody2D>();
            v = rb.velocity;
        }

        public void FixedUpdate()
        {
            dv = rb.velocity - v;
            v = rb.velocity;
        }

        public bool IsFalling()
        {
            // freefall is dv.y == -1.2f, approximately
            return dv.y < -0.6f;
        }
    }
}
