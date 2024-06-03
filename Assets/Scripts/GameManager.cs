using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private CharacterBehavior player, enemy;
    [SerializeField] private Button attack, fireball, superAttack;
    [SerializeField] private Text history;

    private bool playerWeakness = false;
    private bool enemyWeakness = false;

    private void Start()
    {
        // Inicializa os textos e os botoes
        UpdateText(player, playerWeakness); 
        UpdateText(enemy, enemyWeakness);
        history.text = "";
        attack.onClick.AddListener(PlayerAttack);
        fireball.onClick.AddListener(PlayerFireball);
        superAttack.onClick.AddListener(PlayerSuperAttack);
    }

    private void PlayerAttack()
    {
        // Comeca ataque e contra-ataque do inimigo
        player.Attack(enemy);
        UpdateText(enemy, enemyWeakness);

        StartCoroutine(EnemyAttack());
    }

    private void PlayerFireball()
    {
        // Comeca spell e contra-ataque do inimigo
        player.Fireball(enemy);
        UpdateText(enemy, enemyWeakness); 
        UpdateText(player, playerWeakness);

        StartCoroutine(EnemyAttack());
    }

    private void PlayerSuperAttack()
    {
        // Comeca super ataque e contra-ataque do inimigo
        player.SuperAttack(enemy);
        UpdateText(enemy, enemyWeakness);

        StartCoroutine(EnemyAttack());
    }

    private IEnumerator EnemyAttack()
    {
        // Depois de 1 segundo, ataca
        yield return new WaitForSeconds(1f);

        if (!enemy.IsHealthLow())
        {
            // Lanca bola de fogo se jogador tem fraqueza por fogo ou se inimigo decidiu usar magia
            if (playerWeakness || enemy.ShouldUseMagic()) { enemy.Fireball(player); }
            else { enemy.Attack(player); }
        }
        else { enemy.SuperAttack(player); }

        UpdateText (player, playerWeakness);
        UpdateText(enemy, enemyWeakness);

        StopCoroutine(EnemyAttack());
    }

    private void OnEnable() 
    { 
        CharacterBehavior.OnVunerableToFire += UpdateWeakness;
        CharacterBehavior.OnAttack += UpdateHistory;
    }

    private void OnDisable() 
    { 
        CharacterBehavior.OnVunerableToFire -= UpdateWeakness; 
        CharacterBehavior.OnAttack -= UpdateHistory;
    }

    private void UpdateWeakness(bool weakness, CharacterBehavior character)
    {
        // Atualiza a fraqueza do personagem
        if (character == player) { playerWeakness = weakness; }
        else if (character == enemy) {  enemyWeakness = weakness; }
    }

    private void UpdateText(CharacterBehavior character, bool showWeakness)     // CHECAR VIDA, TROCAR DE INIMIGO, VENCEDOR
    {
        // Pega texto e informacoes do personagem
        Text text = character.gameObject.GetComponent<Text>();
        SCharacter characterInfo = character.GetCharacterInfo();

        // Atualiza de acordo com a vulnerabilidade a fogo
        if (showWeakness)
        {
            text.text = $"<b>{characterInfo.Name}</b>\n" +
                        $"<size=18><color=orange>Is vulnerable to fire</color></size>\n" +
                        $"HP : {characterInfo.currentHealth}/{characterInfo.maxHealth}\t" +
                        $"MP : {characterInfo.currentMana}/{characterInfo.maxMana}";
        }
        else
        {
            text.text = $"<b>{characterInfo.Name}</b>\n" +
                        $"\n" +
                        $"HP : {characterInfo.currentHealth}/{characterInfo.maxHealth}\t" +
                        $"MP : {characterInfo.currentMana}/{characterInfo.maxMana}";
        }
    }

    private void UpdateHistory(CharacterBehavior character, string actionText)
    {
        // Atualiza o historico
        string characterName = character.GetCharacterInfo().Name;
        string color = character == player ? "cyan" : "red";
        history.text = $"<color={color}>{characterName}</color> " + actionText + "\n\n" + history.text;
    }
}
