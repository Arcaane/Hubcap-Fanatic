using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ManagerNameSpace;
using UnityEngine;
using Random = UnityEngine.Random;

namespace EnemyNamespace
{
    public class EnemyFoddler : Enemy, IDamageable
    {
        private Collider myCol;
        //Variables
        [Space] [Header("Foddler Section")] 
        [SerializeField] private Animator animator;
        [SerializeField] private SkinnedMeshRenderer meshRenderer;
        [SerializeField] private Rigidbody[] ragdollHandler;
        [SerializeField] private List<Collider> ragdollColliders;
        [SerializeField] public FoddlerState State = FoddlerState.Spawning;
        [SerializeField] private float deadTimer = 0;
        [SerializeField] private LayerMask playerLayer;
        public float timer;
        public bool isAttacking = false;
        [SerializeField] private Transform puddleSocket;
        
        void Start()
        {
            State = FoddlerState.Spawning;
            Spawn();
        }
        
        void Update()
        {
            ExecuteState();
        }

        // ReSharper disable Unity.PerformanceAnalysis
        private void ExecuteState() // Update 
        {
            switch (State)
            {
                case FoddlerState.Spawning: Spawn(); break;
                case FoddlerState.FollowPlayer: FollowPlayer(); break;
                case FoddlerState.AttackPlayer: AttackPlayer(); break;
                case FoddlerState.Dead: DeadState(); break;
                default: throw new ArgumentOutOfRangeException();
            }

            timer += Time.deltaTime;
        }
        
        private void FollowPlayer()
        {
            float velocity = agent.velocity.magnitude/agent.speed;
            animator.SetFloat("Speed", velocity);
            aimingTimer += Time.deltaTime;
            if (aimingTimer > updatePath)
            {
                var destination = new Vector3(playerPos.position.x, 0.25f, playerPos.position.z);
                agent.SetDestination(destination);
                aimingTimer = 0;
            }

            if (Vector3.Distance(transform.position, playerPos.position) < 3f)
                SwitchState(FoddlerState.AttackPlayer);
        }
        
        private void AttackPlayer()
        {
            // Attack en direction du joueur au moment ou il trigger l'attaque
            // Si voiture présente après l'anim dans un radius -> Apply dégats
            // Switch state

            var tempTimer = 2f;
            if (timer > 1.75f)
            {
                if (timer > tempTimer)
                {
                    if (Physics.OverlapSphereNonAlloc(transform.position, 2.5f, new Collider[1], playerLayer) > 0)
                    {
                        CarHealthManager.instance.TakeDamage(1);
                    }
                    SwitchState(FoddlerState.FollowPlayer);
                }
                
                if (isAttacking) return;
                int randAttack = Random.Range(0, 3);
                LaunchAttackAnim(randAttack);
                
                // switch (randAttack)
                // {
                //     case 0: tempTimer = (float)(658 * 2) / 1000; break;
                //     case 1: tempTimer = (float)(1158 * 2) / 1000; break;
                //     case 2: tempTimer = (float) 566 * 2 / 1000; break;
                // }
                
            }
        }

        private async void LaunchAttackAnim(int i)
        {
            isAttacking = true;
            animator.SetBool("isAttack", isAttacking);
            animator.SetFloat("RandomAttack", i);
            await Task.Yield();
            int animTime = 0;
            switch (i)
            {
                case 0: animTime = 658; break;
                case 1: animTime = 1158; break;
                case 2: animTime = 566; break;
            }
            await Task.Delay(animTime * 2);
            animator.SetBool("isAttack", false);
        }

        private void DeadState()
        {
            return;
            // timer & au bout de 10 secondes dépop
            // Lerp du mat opacity 255 -> 0
            
            deadTimer += Time.deltaTime;
            Color col = new Color(255, 0, 0, Mathf.Lerp(255, 0, 10 - deadTimer));
            meshRenderer.material.color = col;
            if (deadTimer > 10)
            {
                Pooler.instance.DestroyInstance(Key.OBJ_Foddler, transform);
            }
        }

        private void SwitchState(FoddlerState nextState)
        {
            switch (nextState) // 
            {
                case FoddlerState.Spawning: break;
                case FoddlerState.FollowPlayer: ToFollow(); break;
                case FoddlerState.AttackPlayer: ToAttack(); break;
                case FoddlerState.Dead: break;
                default: throw new ArgumentOutOfRangeException(nameof(nextState), nextState, null);
            }

            State = nextState;
        }

        private void ToAttack()
        {
            agent.SetDestination(transform.position);
        }

        private void ToFollow()
        {
            timer = 0f;
            isAttacking = false;
            animator.SetBool("isAttack", false);
        }

        protected override void Spawn()
        {
            base.Spawn();
            //ragdollHandler = GetComponentsInChildren<Rigidbody>();
            
            // foreach (var t in ragdollHandler)
            // {
            //     if (t.GetComponent<CapsuleCollider>() != null)
            //     {
            //         CapsuleCollider col = t.GetComponent<CapsuleCollider>();
            //         if (col != null && col.isTrigger == false) col.enabled = false;
            //         ragdollColliders.Add(col);
            //     }
            //     
            //     t.isKinematic = true;
            // }
            SwitchState(FoddlerState.FollowPlayer);
        }
        
        private void EnableRagdoll()
        {
            foreach (var r in ragdollHandler)
            {
                r.isKinematic = false;
            }

            foreach (var r in ragdollColliders)
            {
                r.enabled = true;
            }

            isDead = true;
            agent.isStopped = true;
        }

        public void TakeDamage(int damages)
        {
            if (!IsDamageable()) return;
            EnemyTakeDamage(damages);
        }

        public bool IsDamageable() => gameObject.activeSelf;

        //private void Kill() => OnDie();

        protected override void OnDie()
        {
            base.OnDie();
            SwitchState(FoddlerState.Dead);
            Pooler.instance.DestroyInstance(Key.OBJ_Foddler, transform);
            //Pooler.instance.SpawnTemporaryInstance(Key.FX_Puddle, puddleSocket.position, puddleSocket.rotation, 15f);
        }
        
#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            //if (!Application.isPlaying) return;
          
            //Gizmos.color = Color.blue;
            //Gizmos.DrawWireSphere(transform.position + transform.forward * 1.5f + transform.up, 1.5f);
            
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, 2.5f);
            
            if (agent.destination != null)
            {
                Gizmos.color = Color.white;
                {
                    // Draw lines joining each path corner
                    Vector3[] pathCorners = agent.path.corners;
                
                    for (int i = 0; i < pathCorners.Length - 1; i++)
                    {
                        Gizmos.DrawLine(pathCorners[i], pathCorners[i + 1]);
                    }
                }
            }
        }
#endif
    }
}

public enum FoddlerState
{
    Spawning,
    FollowPlayer,
    AttackPlayer,
    Dead
}

public static class ExtensionMethods {
    public static float Remap (this float value, float from1, float to1, float from2, float to2) {
        return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
    }
}