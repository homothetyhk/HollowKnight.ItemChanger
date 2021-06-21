using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ItemChanger.Components
{
    [Obsolete("Bad code")]
    public class DropIntoPlace : MonoBehaviour
    {
        Rigidbody2D rb;
        bool unpaused;

        public void Awake()
        {
            rb = gameObject.GetComponent<Rigidbody2D>() ?? gameObject.AddComponent<Rigidbody2D>();
            rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
            StartCoroutine(Pause());
        }

        public void Update()
        {
            if (unpaused)
            {
                float mag = rb.velocity.magnitude;
                if (mag < 0.05)
                {
                    rb.constraints = RigidbodyConstraints2D.FreezeAll;
                    this.enabled = false;
                }
            }
        }

        private IEnumerator Pause()
        {
            yield return new WaitForSeconds(0.05f);
            unpaused = true;
        }

    }
}
