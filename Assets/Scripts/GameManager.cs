using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private CharacterBehavior player, enemy;    
    [SerializeField] private Button attack, fireball, superAttack;
    [SerializeField] private Text history, end;
    [SerializeField] private GameObject hud;
    [SerializeField] private List<CharacterObject> characters;

    private bool playerHasFireWeakness, enemyHasFireWeakness, isBattleEnd;

    private void Start()
    {
        // Inicializa os textos e os botoes
        UpdateCharacterObject(player); UpdateCharacterObject(enemy);
        UpdateText(player, playerHasFireWeakness); UpdateText(enemy, enemyHasFireWeakness);
        history.text = ""; end.text = "";
        attack.onClick.AddListener(PlayerAttack);
        fireball.onClick.AddListener(PlayerFireball);
        superAttack.onClick.AddListener(PlayerSuperAttack);
    }

    private void UpdateCharacterObject(CharacterBehavior character)
    {
        // Atualiza personagem se a lista nao tiver vazia (fim de jogo)
        if (characters == null || characters.Count == 0)
        {
            isBattleEnd = true;
            end.text = character.GetCharacterInfo().Name + " lost!"; 
            hud.SetActive(false);
        }
        else
        {
            Shuffle(characters);
            character.SetCharacterObject(characters[0]);
            characters.RemoveAt(0);
        }
    }

    private void Shuffle<T>(List<T> list)   // Algoritmo Fisher-Yates
    {
        System.Random randObject = new System.Random();
        int listCount = list.Count;

        while (listCount > 1)
        {
            listCount--;
            int rand = randObject.Next(listCount + 1);
            
            T value = list[rand];
            list[rand] = list[listCount];
            list[listCount] = value;
        }
    }

    private void PlayerAttack()
    {
        // Comeca ataque e contra-ataque do inimigo
        player.Attack(enemy);
        UpdateText(enemy, enemyHasFireWeakness);

        StartCoroutine(EnemyAttack());
    }

    private void PlayerFireball()
    {
        // Comeca spell e contra-ataque do inimigo
        player.Fireball(enemy);
        UpdateText(enemy, enemyHasFireWeakness); 
        UpdateText(player, playerHasFireWeakness);

        StartCoroutine(EnemyAttack());
    }

    private void PlayerSuperAttack()
    {
        // Comeca super ataque e contra-ataque do inimigo
        player.SuperAttack(enemy, enemyHasFireWeakness);
        UpdateText(enemy, enemyHasFireWeakness);

        StartCoroutine(EnemyAttack());
    }

    private IEnumerator EnemyAttack()
    {
        // Depois de 1 segundo, ataca
        yield return new WaitForSeconds(1f);

        if (!isBattleEnd) // Evita inimigo atacar quando jogo acabar
        {
            if (!enemy.IsHealthLow())
            {
                // Lanca bola de fogo se jogador tem fraqueza por fogo ou se inimigo decidiu usar magia
                if (playerHasFireWeakness || enemy.ShouldUseMagic()) { enemy.Fireball(player); }
                else { enemy.Attack(player); }
            }
            else { enemy.SuperAttack(player, playerHasFireWeakness); }

            UpdateText(player, playerHasFireWeakness);
            UpdateText(enemy, enemyHasFireWeakness);
        }

        StopCoroutine(EnemyAttack());
    }

    private void UpdateText(CharacterBehavior character, bool showWeakness)
    {
        // Pega texto e informacoes do personagem
        Text text = character.gameObject.GetComponent<Text>();
        SCharacter characterInfo = character.GetCharacterInfo();

        // Atualiza de acordo com a vulnerabilidade a fogo
        if (showWeakness)
        {
            text.text = $"<b>{characterInfo.Name}</b>\n" +
                        $"<size=18><color=orange>Is vulnerable to fire</color></size>\n" +
                        $"HP : {characterInfo.currentHealth:0}/{characterInfo.maxHealth:0}\t" +
                        $"MP : {characterInfo.currentMana:0}/{characterInfo.maxMana:0}";
        }
        else
        {
            text.text = $"<b>{characterInfo.Name}</b>\n" +
                        $"\n" +
                        $"HP : {characterInfo.currentHealth:0}/{characterInfo.maxHealth:0}\t" +
                        $"MP : {characterInfo.currentMana:0}/{characterInfo.maxMana:0}";
        }
    }

    private void OnEnable()
    { 
        CharacterBehavior.OnVunerableToFire += UpdateWeakness;
        CharacterBehavior.OnAttack += UpdateHistory;
        CharacterBehavior.OnDeath += UpdateCharacterObject;
    }

    private void OnDisable()
    { 
        CharacterBehavior.OnVunerableToFire -= UpdateWeakness; 
        CharacterBehavior.OnAttack -= UpdateHistory;
        CharacterBehavior.OnDeath -= UpdateCharacterObject;
    }

    private void UpdateWeakness(bool weakness, CharacterBehavior character)
    {
        // Atualiza a fraqueza do personagem
        if (character == player) { playerHasFireWeakness = weakness; }
        else if (character == enemy) { enemyHasFireWeakness = weakness; }
    }

    private void UpdateHistory(CharacterBehavior character, string actionText)
    {
        // Atualiza o historico
        string characterName = character.GetCharacterInfo().Name;
        string color = character == player ? "cyan" : "red";
        history.text = $"<color={color}>{characterName}</color> " + actionText + "\n\n" + history.text;
    }
}
