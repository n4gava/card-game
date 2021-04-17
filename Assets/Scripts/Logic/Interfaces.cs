public interface ICharacter : IIdentifiable
{	
    int Health { get; }

    int AttackPower { get; }

    void Die();

    void TakeDamage(int damage);
}

public interface IIdentifiable
{
    int ID { get; }
}
