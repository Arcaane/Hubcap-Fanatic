public class RushAbility : Ability
{
    public override void StartAbility()
    {
        base.StartAbility();
        CarAbilitiesManager.Instance.damageOnCollisionWithEnemy += 100;
    }
    
    public override void StopAbility()
    {
        base.StopAbility();
        CarAbilitiesManager.Instance.damageOnCollisionWithEnemy -= 100;
    }
}
