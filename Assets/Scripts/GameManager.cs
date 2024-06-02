using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private CharacterBehavior player, enemy;
    [SerializeField] private Button attack, fireball, superAttack;

    private void Start()
    {
        // Inicializa os textos e os botoes
        UpdateText(player); UpdateText(enemy);
        attack.onClick.AddListener(StartAttack);
        fireball.onClick.AddListener(StartFireball);
        superAttack.onClick.AddListener(StartSuperAttack);
    }

    private void StartAttack()
    {
        player.Attack(enemy);
        UpdateText(enemy);
    }

    private void StartFireball()
    {
        player.Fireball(enemy);
        UpdateText(enemy); UpdateText(player);
    }

    private void StartSuperAttack()
    {
        player.SuperAttack(enemy);
        UpdateText(enemy);
    }

    private void UpdateText(CharacterBehavior character)
    {
        // Pega texto e informacoes do personagem
        Text text = character.gameObject.GetComponent<Text>();
        SCharacter characterInfo = character.GetCharacterInfo();


        // Atualiza de acordo com a vulnerabilidade a fogo
        if (!characterInfo.vulnerableToFire)
        {
            text.text = $"<b>{characterInfo.Name}</b>\n" +
                        $"\n" +
                        $"HP : {characterInfo.currentHealth}/{characterInfo.maxHealth}\t" +
                        $"MP : {characterInfo.currentMana}/{characterInfo.maxMana}";
        }
        else
        {
            text.text = $"<b>{characterInfo.Name}</b>\n" +
                        $"<size=18><color=orange>Is vulnerable to fire</color></size>\n" +
                        $"HP : {characterInfo.currentHealth}/{characterInfo.maxHealth}\t" +
                        $"MP : {characterInfo.currentMana}/{characterInfo.maxMana}";
        }
    }
}
