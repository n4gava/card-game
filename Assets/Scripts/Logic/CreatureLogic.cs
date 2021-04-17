using System;
using System.Collections.Generic;

[System.Serializable]
public class CreatureLogic : ICharacter 
{
    private static Dictionary<int, CreatureLogic> CreaturesCreatedThisGame = new Dictionary<int, CreatureLogic>();

    /// <summary>
    /// Reset creatures logic for a new game
    /// </summary>
    public static void ResetCreaturesLogic()
    {
        CreaturesCreatedThisGame = new Dictionary<int, CreatureLogic>();
    }

    /// <summary>
    /// Find the <see cref="CreatureLogic"/> from its Id
    /// </summary>
    /// <param name="uniqueCreatureId">Creature unique Id</param>
    /// <returns><see cref="CreatureLogic"/>. Can be null if Id does not exist</returns>
    public static CreatureLogic FindCreatureLogicById(int uniqueCreatureId)
    {
        CreaturesCreatedThisGame.TryGetValue(uniqueCreatureId, out var creatureLogic);
        return creatureLogic;
    }

    private CreatureEffect _effect;
    private int _maxHealth;
    private int _attacksForOneTurn;


    public CreatureLogic(Player owner, CardAsset cardAsset)
    {
        ID = IDFactory.GetUniqueID();
        Owner = owner;
        CreaturesCreatedThisGame.Add(ID, this);

        LoadCreatureStatusFromCardAsset(owner, cardAsset);
    }

    /// <summary>
    /// Creature ID
    /// </summary>
    public int ID { get; private set; }

    /// <summary>
    /// Creature player owner
    /// </summary>
    public Player Owner { get; private set; }

    /// <summary>
    /// True when creature is frozen
    /// </summary>
    public bool Frozen { get; private set; }

    /// <summary>
    /// Creature health
    /// </summary>
    public int Health { get; private set; }

    /// <summary>
    /// Creature attack power
    /// </summary>
    public int AttackPower { get; private set; }

    /// <summary>
    /// Attacks left this turn
    /// </summary>
    public int AttacksLeftThisTurn { get; private set; } = 0;

    /// <summary>
    /// Indicates if creature can attack
    /// </summary>
    public bool CanAttack
    {
        get
        {
            bool ownersTurn = (TurnManager.Instance.WhoseTurn == Owner);
            return (ownersTurn && (AttacksLeftThisTurn > 0) && !Frozen);
        }
    }

    /// <summary>
    /// Indicates if creature has effects
    /// </summary>
    public bool HasEffect => _effect != null;

    /// <summary>
    /// Take damage
    /// </summary>
    /// <param name="damage">damage taken</param>
    public void TakeDamage(int damage)
    {
        Health -= damage;

        if (Health > _maxHealth)
            Health = _maxHealth;

        if (Health <= 0)
            Die();
    }

    /// <summary>
    /// Play the creature in game
    /// </summary>
    public void PlayInGame()
    {
        if (_effect != null)
            _effect.WhenACreatureIsPlayed();
    }

    /// <summary>
    /// When turn starts
    /// </summary>
    public void OnTurnStart()
    {
        AttacksLeftThisTurn = _attacksForOneTurn;
    }

    /// <summary>
    /// When creature dies
    /// </summary>
    public void Die()
    {   
        Owner.table.CreaturesOnTable.Remove(this);

        if (_effect != null)
        {
            _effect.WhenACreatureDies();
            _effect.UnRegisterEventEffect();
            _effect = null;
        }

        new CreatureDieCommand(ID, Owner).AddToQueue();
    }

    /// <summary>
    /// Attacks a character
    /// </summary>
    /// <param name="target">target</param>
    public void Attack(ICharacter target)
    {
        AttacksLeftThisTurn--;
        int targetHealthAfter = target.Health - AttackPower;
        int attackerHealthAfter = Health - target.AttackPower;

        new CreatureAttackCommand(target.ID, ID, target.AttackPower, AttackPower, attackerHealthAfter, targetHealthAfter).AddToQueue();

        target.TakeDamage(AttackPower);
        this.TakeDamage(target.AttackPower);
    }

    /// <summary>
    /// Attack creature from its id
    /// </summary>
    /// <param name="uniqueCreatureID">creature unique id</param>
    public void AttackCreatureWithID(int uniqueCreatureID)
    {
        CreatureLogic target = FindCreatureLogicById(uniqueCreatureID);
        Attack(target);
    }

    /// <summary>
    /// Load creature status from <see cref="CardAsset"/>
    /// </summary>
    /// <param name="owner">Creature Owner</param>
    /// <param name="cardAsset">Creature CardAsset</param>
    private void LoadCreatureStatusFromCardAsset(Player owner, CardAsset cardAsset)
    {
        _maxHealth = cardAsset.MaxHealth;
        _attacksForOneTurn = cardAsset.AttacksForOneTurn;
        Health = cardAsset.MaxHealth;
        AttackPower = cardAsset.Attack;

        if (cardAsset.Charge)
            AttacksLeftThisTurn = _attacksForOneTurn;

        LoadCreatureEffect(owner, cardAsset);
    }

    /// <summary>
    /// Load creature effect from <see cref="CardAsset"/>
    /// </summary>
    /// <param name="owner">Creature Owner</param>
    /// <param name="cardAsset">Creature CardAsset</param>
    private void LoadCreatureEffect(Player owner, CardAsset cardAsset)
    {
        if (!string.IsNullOrEmpty(cardAsset.CreatureScriptName))
        {
            _effect = Activator.CreateInstance(Type.GetType(cardAsset.CreatureScriptName), new object[] { owner, this, cardAsset.SpecialCreatureAmount }) as CreatureEffect;
            _effect.RegisterEventEffect();
        }
    }
}
