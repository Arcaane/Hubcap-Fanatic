using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace VFXNameSpace
{
    public class FluidPuddle : MonoBehaviour
    {
        [SerializeField] private float yPos,sizeMin,sizeMax;
        [SerializeField] private ParticleSystem particleSystem;
        [SerializeField] private ParticleSystem[] subParticleSystem;
        public List<ParticleCollisionEvent> collisionEvents;
        public ParticleSystem.Particle[] particles;
        public int particleNb = 0;

        void Start()
        {
            particleSystem = GetComponent<ParticleSystem>();
            collisionEvents = new List<ParticleCollisionEvent>();
            particles = new ParticleSystem.Particle[0];
        }
        
        private void OnParticleCollision(GameObject other)
        {
            int numCollisionEvents = particleSystem.GetCollisionEvents(other, collisionEvents);
            
            int i = 0;

            while (i < numCollisionEvents)
            {
                Vector3 pos = collisionEvents[i].intersection;
                subParticleSystem[particleNb].transform.position = new Vector3(pos.x, yPos,pos.z);
                float size = Random.Range(sizeMin, sizeMax);
                subParticleSystem[particleNb].transform.localScale = new Vector3(size, 1,size);
                subParticleSystem[particleNb].gameObject.SetActive(true);
                particleNb++;
                i++;
            }
        }
    }   
}
