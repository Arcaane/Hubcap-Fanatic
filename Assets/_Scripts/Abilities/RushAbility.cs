using Abilities;

public class RushAbility : Ability
{
    public override void StartAbility()
    {
        base.StartAbility();
        CarAbilitiesManager.instance.damageOnCollisionWithEnemy += 100;
    }
    
    public override void StopAbility()
    {
        base.StopAbility();
        CarAbilitiesManager.instance.damageOnCollisionWithEnemy -= 100;
    }
}
