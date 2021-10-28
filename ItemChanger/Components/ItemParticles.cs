using System;
using System.Collections.Generic;
using System.Linq;
using ItemChanger.Internal;
using ItemChanger.Util;
using UnityEngine;

namespace ItemChanger.Components
{
    public class ItemParticles : MonoBehaviour
    {
        public IEnumerable<AbstractItem> items;
        GameObject soulParticles;
        ParticleSystem ps;
        public Vector3 offset;

        public void Awake()
        {
            if (ObjectCache.SoulTotemPreloader.PreloadLevel == PreloadLevel.None)
            {
                Destroy(this);
                return;
            }

            soulParticles = ObjectCache.SoulParticles;
            soulParticles.transform.SetParent(transform);
            ps = soulParticles.GetComponent<ParticleSystem>();
        }

        public void Start()
        {
            Vector3 pos = transform.position;
            pos.z -= 3f;
            pos += offset;
            soulParticles.transform.position = pos;
            soulParticles.SetActive(true);
            StartEmission();
        }

        public void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.CompareTag("Player"))
            {
                StopEmission();
            }
        }

        public void OnTriggerExit2D(Collider2D collider)
        {
            if (collider.CompareTag("Player"))
            {
                StartEmission();
            }
        }

        public void SetEmission(bool enable)
        {
            ParticleSystem.EmissionModule emission = ps.emission;
            emission.enabled = enable;
        }

        public void StartEmission()
        {
            if (items == null || items.All(item => item.IsObtained()))
            {
                SetEmission(false);
                return;
            }

            if (items.All(item => item.WasEverObtained()))
            {
                SetColor(ShinyUtility.WasEverObtainedColor);
            }

            SetEmission(true);
        }

        public void StopEmission() => SetEmission(false);

        public void SetColor(Color color)
        {
            // I'm not sure which of these actually sets the color
            ParticleSystem.ColorOverLifetimeModule col = ps.colorOverLifetime;
            col.color = color;
            ParticleSystem.MainModule main = ps.main;
            main.startColor = color;
        }
    }
}
