using System.Collections.Generic;
using UnityEngine;

namespace WelwiseEmotionsModule.Runtime.Client.Scripts.Animations
{
    public class ParticleEventController : MonoBehaviour
    {
        public GameObject[] ParticleObjects { get; private set; } = new GameObject[] {};
        
        private readonly List<ParticleSystem[]> _particleSystemsList = new List<ParticleSystem[]>();
        
        public void UpdateParticleObjects(GameObject[] particleObjects)
        {
            if (particleObjects == null || particleObjects.Length == 0)
            {
                Debug.LogWarning("ParticleEventController: no particleObjects assigned!");
                return;
            }

            StopAllParticles();
            
            ParticleObjects = particleObjects;

            _particleSystemsList.Clear();
            // Initialize: deactivate all and cache their ParticleSystems
            foreach (var obj in particleObjects)
            {
                if (obj == null) continue;

                obj.SetActive(false);
                var systems = obj.GetComponentsInChildren<ParticleSystem>();
                _particleSystemsList.Add(systems);
            }
        }

        public void PlayAllParticles()
        {
            for (int i = 0; i < ParticleObjects.Length; i++)
            {
                var obj = ParticleObjects[i];
                if (obj == null) continue;

                obj.SetActive(true);
                foreach (var ps in _particleSystemsList[i])
                    ps.Play();
            }
        }
        
        public void StopAllParticles()
        {
            for (int i = 0; i < ParticleObjects.Length; i++)
            {
                var obj = ParticleObjects[i];
                if (obj == null) continue;

                foreach (var ps in _particleSystemsList[i])
                    ps.Stop();
                
                obj.SetActive(false);
            }
        }
    }
}