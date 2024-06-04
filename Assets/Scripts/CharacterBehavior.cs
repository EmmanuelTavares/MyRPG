using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterBehavior : MonoBehaviour
{
    static public Action<bool, CharacterBehavior> OnVunerableToFire;
    static public Action<CharacterBehavior, string> OnAttack;
    static public Action<CharacterBehavior> OnDeath;

    private CharacterObject characterObject;
    private float currentHealth, currentMana;
    private int attackCount;
    private bool countAttacks;

    public void SetCharacterObject(CharacterObject character)
    {
        // Defini personagen e reseta as variaveis
        characterObject = character;
        currentHealth = characterObject.maxHealth;
        currentMana = characterObject.maxMana;
        attackCount = 3;
        countAttacks = false;
        OnVunerableToFire?.Invoke(false, this);
    }

    public bool IsHealthLow()
    {
        // Retorna resultado da condicao de vida
        return currentHealth / characterObject.maxHealth < .4f;
    }

    public bool ShouldUseMagic()
    {
        // Retorna 70% do dano mais forte
        float rand = UnityEngine.Random.Range(0f, 1f);
        if (characterObject.magicMultiply > characterObject.physicalMultiply) { return rand < .7f ? true : false; } // 70% de chance usar magia (se mais forte)
        else { return rand < .7f ? false : true; }   // 70% de chance de nao usar magia (mais fraco)
    }

    private float GetPowerfulDamage()
    {
        // Retorna valor do dano mais auto
        return characterObject.magicMultiply > characterObject.physicalMultiply ? characterObject.magicMultiply : characterObject.physicalMultiply;
    }

    public bool HasMana()
    {
        // Retorna resultado da condicao de mana
        return currentMana >= characterObject.spellCost;
    }

    public bool CanSuperAttack()
    {
        // Pode realizar super ataque se tiver no terceiro turno (depois da primeira) e com pouca vida
        return attackCount == 3 && IsHealthLow();
    }

    private void TakeDamage(float damage, bool fireDamage)
    {
        // 25% mais de dano caso seja dano de fogo e o atacado seja vuneravel a fogo
        if (fireDamage && characterObject.vulnerableToFire) 
        { 
            currentHealth -= damage * 1.25f;
            OnVunerableToFire?.Invoke(true, this);  // Chama acao e retorna a si proprio e que tem fraqueza           
        }
        else { currentHealth -= damage; }

        // Checa morte do jogador
        if (currentHealth <= 0f)
        {
            currentHealth = 0f;
            OnDeath?.Invoke(this);
        }
    }

    public void Attack(CharacterBehavior other)
    {
        // Ataque fisico
        float damage = characterObject.baseDamage * characterObject.physicalMultiply;
        other.TakeDamage(damage, false);
        OnAttack?.Invoke(this, "attacked!");
        currentMana += 10f;

        if (countAttacks) { attackCount++; }
    }

    public void Fireball(CharacterBehavior other)
    {
        // Ataque magico se tiver mana
        if (HasMana()) 
        {
            float damage = characterObject.baseDamage * characterObject.magicMultiply;
            other.TakeDamage(damage, true);
            OnAttack?.Invoke(this, "spell a fireball!");
            currentMana -= characterObject.spellCost;

            if (countAttacks) { attackCount++; }
        }
        else { Attack(other); }
    }

    public void SuperAttack(CharacterBehavior other, bool otherHasFireWeakness)
    {
        // Super ataque no 
        if (CanSuperAttack()) 
        {
            float damage = characterObject.baseDamage * GetPowerfulDamage();
            other.TakeDamage(damage * 2f, false);
            OnAttack?.Invoke(this, "made a super attack!");
            countAttacks = true;
            attackCount = 0;
        }
        else // Aplica o ataque mais danoso
        {
            if (otherHasFireWeakness || ShouldUseMagic()) { Fireball(other); }
            else { Attack(other); }
        }        
    }

    public SCharacter GetCharacterInfo()
    {
        // Retorna informacoes do personagen
        return new SCharacter(characterObject.displayName, characterObject.maxHealth, characterObject.maxMana, currentHealth, currentMana, characterObject.vulnerableToFire);
    }
}

public struct SCharacter
{
    public string Name;
    public float maxHealth, maxMana, currentHealth, currentMana;
    public bool vulnerableToFire;

    public SCharacter(string name, float maxHealth, float maxMana, float currentHealth, float currentMana, bool vulnerableToFire)
    {
        Name = name;
        this.maxHealth = maxHealth;
        this.maxMana = maxMana;
        this.currentHealth = currentHealth;
        this.currentMana = currentMana;
        this.vulnerableToFire = vulnerableToFire;
    }
}
