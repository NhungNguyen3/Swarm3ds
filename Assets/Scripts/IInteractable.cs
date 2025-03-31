
public interface IInteractable
{
    void Pickup();
}

public interface IDamageable
{
    void Damage(float dmg);
}


public interface IHeathable
{
    void Regen(float hp);
}