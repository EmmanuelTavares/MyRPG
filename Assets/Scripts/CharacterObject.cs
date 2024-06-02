using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class CharacterObject : ScriptableObject
{
    public string displayName;
    public float maxHealth = 100f, maxMana = 100f, physicalMultiply = 1f, magicMultiply = 1f;
    public bool vulnerableToFire = true;

    [HideInInspector] public float baseDamage = 10f, spellCost = 10f;

    private void Awake() { spellCost *= physicalMultiply; } // Custo de feitico inversamente proporcional ao dano fisico base
}
