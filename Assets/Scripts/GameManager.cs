using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private enum GameState
    {
        MENU = 0,
        ROUND = 1,
        END = 2,
    }

    private GameState state = GameState.MENU;
    private int readyPlayers = 0;
    [SerializeField] private int maxScore = 5;
    [SerializeField] private List<PlayerKeyMapping> keyMapping = new List<PlayerKeyMapping>();
    [SerializeField] private List<ActionMapping> actionResponses = new List<ActionMapping>();

    private int gamePoints = 0;
    public int GamePoints { get { return gamePoints; } }
    [SerializeField] private Slider slider;

    private Dictionary<PlayerKeyMapping, ActionEnum> choices = new Dictionary<PlayerKeyMapping, ActionEnum>();

    private void Start()
    {
        slider.maxValue = maxScore + 1;
        slider.minValue = -(maxScore + 1);
        slider.value = 0;
    }

    public void Update()
    {
        switch (state)
        {
            case GameState.MENU:
                StartGame();
                break;
            case GameState.ROUND:
                GameRound();
                break;
            case GameState.END:
                GameEnd();
                break;
            default:
                break;
        }

    }

    private void StartGame()
    {
        foreach (PlayerKeyMapping map in keyMapping)
        {
            if (Input.GetKeyDown(map.enterKey))
            {
                map.ready = !map.ready;
            }
        }
        readyPlayers = keyMapping.Count(x => x.ready);
        if (readyPlayers >= keyMapping.Count)
        {
            state = GameState.ROUND;
            keyMapping.Where(x => x.ready).ToList().ForEach(x => x.ready = false);
            readyPlayers = 0;
        }
    }

    private void GameRound()
    {
        foreach (PlayerKeyMapping map in keyMapping)
        {
            if (map.ready)
            {
                continue;
            }
            foreach (KeyMapping key in map.attackCodes)
            {
                if (Input.GetKeyDown(key.keyCode))
                {
                    choices.Add(map, key.action);
                    map.ready = true;
                }
            }
        }
        readyPlayers = keyMapping.Count(x => x.ready);
        if (readyPlayers >= keyMapping.Count)
        {
            CalculateGameResult();
        }
    }

    private void CalculateGameResult()
    {
        List<KeyValuePair<PlayerKeyMapping, ActionEnum>> list = choices.ToList();
        for (int i = 0; i < list.Count; i++)
        {
            for (int j = i; j < list.Count; j++)
            {
                if (list[i].Key == list[j].Key)
                {
                    continue;
                }

                ActionMapping actionMapping = actionResponses[(int)list[i].Value];
                if (actionMapping.win.Contains(list[j].Value))
                {
                    gamePoints += i % 2 == 0 ? 1 : -1;
                }
                if (actionMapping.lose.Contains(list[j].Value))
                {
                    gamePoints += i % 2 == 0 ? -1 : 1;
                }
            }
        }
        slider.value = gamePoints;

        choices.Clear();
        readyPlayers = 0;
        keyMapping.Where(x => x.ready).ToList().ForEach(x => x.ready = false);

        if (gamePoints > maxScore || gamePoints < -maxScore)
        {
            state = GameState.END;
        }
    }

    private void GameEnd()
    {
        slider.value = 0;
        readyPlayers = 0;
        keyMapping.Where(x => x.ready).ToList().ForEach(x => x.ready = false);
        state = GameState.MENU;
    }
}
