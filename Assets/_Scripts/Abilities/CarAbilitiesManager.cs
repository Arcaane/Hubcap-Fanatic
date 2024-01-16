using System.Collections;
using UnityEngine;

public delegate void DefaultDelegate();
public delegate void TransformDelegate(Transform tr);
public delegate void ObjectDelegate(GameObject obj);
public delegate void CollisionDelegate(Collision col);


namespace Abilities
{
    public class CarAbilitiesManager : MonoBehaviour
    {
        public static CarAbilitiesManager instance;
        

        public CarController car;
        
        public ObjectDelegate OnEnemyCollision;
        public CollisionDelegate OnWallCollision;
        public DefaultDelegate OnStateEnter;
        public DefaultDelegate OnStateExit;
        public ObjectDelegate OnEnemyDamageTaken;
        public DefaultDelegate OnPlayerDamageTaken;
        public DefaultDelegate OnUpdate;
        
        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            car = CarController.instance;
        }

        //[Header("KIT")]
        [SerializeField] private Ability[] nitroAbilities;
        [SerializeField] private Ability[] driftAbilities;
        [SerializeField] public int damageOnCollisionWithEnemy;

        public void ActivateDriftAbilities()
        {
            foreach (var t in driftAbilities)
            {
                //if (!t.activable) return;
                t.StartAbility();
            }
        }
        public void DesactivateDriftAbilities()
        {
            foreach (var t in driftAbilities)
            {
                //if (!t.activable) return;
                t.StopAbility();
            }
        }
        public void ActivateNitroAbilities()
        {
            foreach (var t in nitroAbilities)
            {
                //if (!t.activable) return;
                t.StartAbility();
            }
        }
        public void DesactivateNitroAbilities()
        {
            foreach (var t in nitroAbilities)
            {
                t.StopAbility();
            }
        }

        public void Update()
        {
            OnUpdate.Invoke();
            
            foreach (var t in nitroAbilities)
            {
                t.UpdateAbility();
            }

            foreach (var t in driftAbilities)
            {
                t.UpdateAbility();
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.CompareTag("Enemy")) return;

            OnEnemyCollision.Invoke(other.gameObject);
            
            if (Vector3.Dot(other.transform.position - transform.position, transform.forward) > 0.75f) // EnemyCollision
            {
                other.GetComponent<IDamageable>()?.TakeDamage(damageOnCollisionWithEnemy);
                if (car.nitroMode)
                {
                    //OnCollideWithNitro.Invoke();
                }
            }
        }

        public LayerMask enemyLayerMask;

        #region Abilities

        #region Curse

        private void ApplyCurseExplodeOnDeath()
        {
            var cols = Physics.OverlapSphere(transform.position, 10f, enemyLayerMask);
            if (cols.Length < 1) return;
            for (var i = 0; i < cols.Length; i++)
            {
                PoliceCarBehavior police = cols[i].GetComponent<PoliceCarBehavior>();
                police.OnPoliceCarDie -= CurseExplodeOnDeath;
                police.OnPoliceCarDie += CurseExplodeOnDeath;
                Debug.Log(police.gameObject.name + " Add CurseExplodeOnDeath On Death Event");
            }
        }

        public GameObject P_TestExplo;

        private void CurseExplodeOnDeath(Transform tr)
        {
            var position = tr.position;
            Destroy(Instantiate(P_TestExplo, position, Quaternion.identity), 3.5f);
            var cols = Physics.OverlapSphere(position, 20, enemyLayerMask);
            if (cols.Length < 1) return;
            for (int i = 0; i < cols.Length; i++)
            {
                cols[i].GetComponent<IDamageable>()?.TakeDamage(50);
                Debug.Log($"{cols[i].name} took damage");
            }
        }

        #endregion

        #region Collision

        private void AddBoostIfCollideEnemyWithNitro()
        {
            Debug.Log("AddBoostIfCollideEnemyWithNitro");
            car.targetSpeed += 50f;
        }

        #endregion

        private void Explosion(Transform tr)
        {
            CurseExplodeOnDeath(tr);
        }

        #region Tick Damage

        private void ApplyTickDamageOnStrafe(IDamageable damageable)
        {
            TickDamageInIndefinitely(damageable, 1.5f);
        }

        private void TickDamageInIndefinitely(IDamageable damageable, float duration)
        {
            StartCoroutine(TickDamage(duration, damageable));
        }

        private void TickDamageForDuration(IDamageable damageable, float tickDuration, float curseDuration)
        {

        }

        private IEnumerator TickDamage(float tickDuration, IDamageable damageable)
        {
            yield return new WaitForSeconds(tickDuration);
            damageable?.TakeDamage(10);
            if (damageable.IsDamageable() == false) yield break;
            StartCoroutine(TickDamage(tickDuration, damageable));
        }

        #endregion

        #endregion

        private void OnGUI()
        {
            GUI.Label(new Rect(50, 550, 500, 150), "Target Speed: " + car.targetSpeed);
            GUI.Label(new Rect(50, 570, 500, 150), "Max Speed: " + car.maxSpeed);
            GUI.Label(new Rect(50, 590, 500, 150), "Current Speed: " + car.rb.velocity.magnitude);
        }
    }

}

public enum AbilitySocket
{
    AbilityNitro,
    AbilityDrift,
}
