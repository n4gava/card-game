using UnityEngine;

public enum TargetingOptions
{
    NoTarget,
    AllCreatures, 
    EnemyCreatures,
    YourCreatures, 
    AllCharacters, 
    EnemyCharacters,
    YourCharacters
}

public class CardAsset : ScriptableObject 
{
    [Header("General info")]
    public CharacterAsset CharacterAsset; 
    [TextArea(2,3)]
    public string Description;  
	public Sprite CardImage;
    public int ManaCost;

    [Header("Creature Info")]
    public int MaxHealth;   
    public int Attack;
    public int AttacksForOneTurn = 1;
    public bool Charge;
    public string CreatureScriptName;
    public int SpecialCreatureAmount;

    [Header("SpellInfo")]
    public string SpellScriptName;
    public int SpecialSpellAmount;
    public TargetingOptions Targets;

    public bool IsCreatureCard => MaxHealth > 0;

    public bool IsSpellCard => !IsCreatureCard;

}
