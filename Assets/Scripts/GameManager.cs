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
        ANIMATION = 2,
        END = 3,
    }

    private GameState state = GameState.MENU;
    private int readyPlayers = 0;
    private const int PLAYERMODULO = 2;


    [SerializeField] private int maxScore = 5;
    [SerializeField] private List<PlayerKeyMapping> keyMapping = new List<PlayerKeyMapping>();
    [SerializeField] private List<ActionMapping> actionResponses = new List<ActionMapping>();

    private int gamePoints = 0;
    public int GamePoints { get { return gamePoints; } }
    [Header("UI")]
    [SerializeField] private Slider slider;
    [SerializeField] private Transform leftSide;
    [SerializeField] private Transform rightSide;
    [SerializeField] private PlayerModel playerPrefab1;
    [SerializeField] private PlayerModel playerPrefab2;

    private Dictionary<PlayerKeyMapping, ActionEnum> choices = new Dictionary<PlayerKeyMapping, ActionEnum>();

    private void Start()
    {
        slider.maxValue = maxScore + 1;
        slider.minValue = -(maxScore + 1);
        slider.value = 0;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
#if UNITY_EDITOR
            Debug.Break();
#else
            Application.Quit();
#endif
        }
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
        List<PlayerKeyMapping> list = keyMapping.ToList();
        for (int i = 0; i < list.Count; i++)
        {
            if (Input.GetKeyDown(list[i].enterKey))
            {
                list[i].ready = !list[i].ready;
                list[i].playerModel = UpdatePlayerModel(list[i].playerModel, i % PLAYERMODULO);
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
                    map.playerModel.UpdateReady(map.ready);
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
        List<PlayerKeyMapping> attackers = new List<PlayerKeyMapping>();
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
                    gamePoints += i % PLAYERMODULO == 0 ? 1 : -1;
                    attackers.Add(list[i].Key);
                }
                if (actionMapping.lose.Contains(list[j].Value))
                {
                    gamePoints += i % PLAYERMODULO == 0 ? -1 : 1;
                    attackers.Add(list[j].Key);
                }
            }
        }

        state = GameState.ANIMATION;
        foreach (PlayerKeyMapping player in attackers)
        {
            StartCoroutine(PlayAnimation(player));
        }

        choices.Clear();
        readyPlayers = 0;
        keyMapping.Where(x => x.ready).ToList().ForEach(x =>
        {
            x.ready = false;
            x.playerModel.UpdateReady(x.ready);
        });

        if (gamePoints > maxScore || gamePoints < -maxScore)
        {
            state = GameState.END;
        }
        else
        {
            state = GameState.ROUND;
        }
    }

    private IEnumerator PlayAnimation(PlayerKeyMapping player)
    {     
        yield return new WaitForSeconds(player.playerModel.animationTimeUntilAttack);

        slider.value = gamePoints;

        yield return new WaitForSeconds(player.playerModel.animationTimeReturn);
    }

    private void GameEnd()
    {
        slider.value = 0;
        readyPlayers = 0;
        keyMapping.Where(x => x.ready).ToList().ForEach(x => x.ready = false);
        state = GameState.MENU;

        UpdateTransformSides(leftSide);
        UpdateTransformSides(rightSide);
    }

    private void UpdateTransformSides(Transform side)
    {
        for (int i = 0; i < side.childCount; i++)
        {
            UpdatePlayerModel(side.GetChild(i).GetComponent<PlayerModel>());
        }
    }

    private PlayerModel UpdatePlayerModel(PlayerModel child = null, int modulo = 0)
    {
        if (child != null)
        {
            Destroy(child.gameObject);
            return null;
        }
        else
        {
            Transform parent = modulo == 0 ? leftSide : rightSide;
            PlayerModel playerModel = modulo == 0 ? playerPrefab1 : playerPrefab2;
            return Instantiate(playerModel, parent);
        }
    }
}
