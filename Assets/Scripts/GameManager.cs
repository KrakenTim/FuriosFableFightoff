using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEngine.Rendering.Universal;

public class GameManager : MonoBehaviour
{
    public GameObject GameTitle;
    public GameObject DependencyTriangle;
    public GameObject Controls_P1_Tutorial_Icons;
    public GameObject Controls_P2_Tutorial_Icons;
    public GameObject ProgressBar;
    public GameObject P1Start; 
    public GameObject P2Start;

    public GameObject Sword_beats_Axe;
    public GameObject Axe_beats_Shield;
    public GameObject Shield_beats_Sword;

    public GameObject Sword_P1;
    public GameObject Sword_P2;
    public GameObject Shield_P1;
    public GameObject Shield_P2;
    public GameObject Axe_P1;
    public GameObject Axe_P2;



    public GameObject Waiting_for_P2;
    public GameObject Waiting_for_P1;
    public GameObject Make_your_Choice;
    public GameObject Fight;
    public GameObject The_Dark_advances;
    public GameObject The_Light_advances;
    public GameObject The_Dark_Wins;
    public GameObject The_Light_Wins;

    public Light2D LightEffect;
    [SerializeField] private float LightChange; //einstellung wie sich das licht bei sieg niederlaghe ändert

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
    private int animationsCompleted = 0;


    [SerializeField] private int maxScore = 2;
    [SerializeField] private List<PlayerKeyMapping> keyMapping = new List<PlayerKeyMapping>();
    [SerializeField] private List<ActionMapping> actionResponses = new List<ActionMapping>();

    private int gamePoints = 0;
    [Header("UI")]
    [SerializeField] private Slider slider;
    [SerializeField] private Transform leftSide;
    [SerializeField] private Transform rightSide;
    [SerializeField] private PlayerModel playerPrefab1;
    [SerializeField] private PlayerModel playerPrefab2;

    private Dictionary<PlayerKeyMapping, ActionEnum> choices = new Dictionary<PlayerKeyMapping, ActionEnum>();

    private void Start()
    {
        /*StartMenu_Canvas.active = true;
        Game_Canvas.active = false;
        Game.active = false;*/
        slider.maxValue = maxScore + 1;
        slider.minValue = -(maxScore + 1);
        slider.value = 0;

        DependencyTriangle.SetActive(false);
        Controls_P1_Tutorial_Icons.SetActive(false);
        Controls_P2_Tutorial_Icons.SetActive(false);


        Sword_P1.SetActive(false);
        Sword_P2.SetActive(false);
        Shield_P1.SetActive(false);
        Shield_P2.SetActive(false);
        Axe_P1.SetActive(false);
        Axe_P2.SetActive(false);
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
#if UNITY_EDITOR
            EditorApplication.ExitPlaymode();
#else
 //           if(GameIsRunning == true)
 //           {
                Application.Quit();
 //           }
 //           else
 //           {
 //               StartMenu_Canvas.active = true;
 //               Game_Canvas.active = false;
 //               Game.active = false;
 //               GameIsRunning = false;
 //           }
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
                list[i].side = i % PLAYERMODULO;
                list[i].playerModel = UpdatePlayerModel(list[i].playerModel, list[i].side);
            }
        }
        readyPlayers = keyMapping.Count(x => x.ready);
        if (readyPlayers >= keyMapping.Count)
        {
            state = GameState.ROUND;
            keyMapping.Where(x => x.ready).ToList().ForEach(x => x.ready = false);
            readyPlayers = 0;

            P1Start.SetActive(false);
            P2Start.SetActive(false);
            GameTitle.SetActive(false);
            ProgressBar.SetActive(true);
            DependencyTriangle.SetActive(true);
            Controls_P1_Tutorial_Icons.SetActive(true);
            Controls_P2_Tutorial_Icons.SetActive(true);
            The_Light_Wins.SetActive(false);
            The_Dark_Wins.SetActive(false);
        }
    }

    private void GameRound()
    {
        Sword_beats_Axe.SetActive(false);
        Axe_beats_Shield.SetActive(false);
        Shield_beats_Sword.SetActive(false);

        Sword_P1.SetActive(false);
        Sword_P2.SetActive(false);
        Shield_P1.SetActive(false);
        Shield_P2.SetActive(false);
        Axe_P1.SetActive(false);
        Axe_P2.SetActive(false);

        foreach (PlayerKeyMapping map in keyMapping)
        {
            Fight.SetActive(true);
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
            Fight.SetActive(false);
            CalculateGameResult();
        }
    }

    private void CalculateGameResult()
    {
        
        DependencyTriangle.SetActive(false);
        Controls_P1_Tutorial_Icons.SetActive(false);
        Controls_P2_Tutorial_Icons.SetActive(false);

        List<KeyValuePair<PlayerKeyMapping, ActionEnum>> list = choices.ToList();
        Dictionary<PlayerKeyMapping, int> attackers =  new Dictionary<PlayerKeyMapping, int>();
        for (int i = 0; i < list.Count; i++)
        {
            for (int j = i; j < list.Count; j++)
            {
                if (list[i].Key == list[j].Key)
                {
                    //Sword_beats_Axe.SetActive(false);
                    //Axe_beats_Shield.SetActive(false);
                    //Shield_beats_Sword.SetActive(false);
                    continue;
                }

                ActionMapping actionMapping = actionResponses[(int)list[i].Value];
                if (actionMapping.win.Contains(list[j].Value))
                {
                    int points = list[i].Key.side == 0 ? 1 : -1;

                    if(attackers.ContainsKey(list[i].Key))
                    {
                        attackers[list[i].Key] += points;

                    }
                    else
                    {
                        attackers.Add(list[i].Key, points);
                    }
                }
                if (actionMapping.lose.Contains(list[j].Value))
                {
                    int points = list[j].Key.side == 0 ? 1 : -1;
                    if (attackers.ContainsKey(list[j].Key))
                    {
                        attackers[list[j].Key] += points;
                    }
                    else
                    {
                        attackers.Add(list[j].Key, points);
                    }
                }
                if ((list[i].Value == ActionEnum.AXT) && (list[j].Value == ActionEnum.SHIELD)) 
                {
                    Sword_beats_Axe.SetActive(false);
                    Axe_beats_Shield.SetActive(true);
                    Shield_beats_Sword.SetActive(false);
                    Axe_P2.SetActive(true);
                    Shield_P1.SetActive(true);
                }
                else if ((list[j].Value == ActionEnum.AXT) && (list[i].Value == ActionEnum.SHIELD))
                {
                    Sword_beats_Axe.SetActive(false);
                    Axe_beats_Shield.SetActive(true);
                    Shield_beats_Sword.SetActive(false);
                    Axe_P1.SetActive(true);
                    Shield_P2.SetActive(true);
                }
                else if ((list[i].Value == ActionEnum.SWORD) && (list[j].Value == ActionEnum.SHIELD) )
                {
                    Sword_beats_Axe.SetActive(false);
                    Axe_beats_Shield.SetActive(false);
                    Shield_beats_Sword.SetActive(true);
                    Sword_P2.SetActive(true);
                    Shield_P1.SetActive(true);
                }
                else if ((list[j].Value == ActionEnum.SWORD) && (list[i].Value == ActionEnum.SHIELD))
                {
                    Sword_beats_Axe.SetActive(false);
                    Axe_beats_Shield.SetActive(false);
                    Shield_beats_Sword.SetActive(true);
                    Sword_P1.SetActive(true);
                    Shield_P2.SetActive(true);
                }
                else if ((list[i].Value == ActionEnum.AXT) && (list[j].Value == ActionEnum.SWORD))
                {
                    Sword_beats_Axe.SetActive(true);
                    Axe_beats_Shield.SetActive(false);
                    Shield_beats_Sword.SetActive(false);
                    Axe_P2.SetActive(true);
                    Sword_P1.SetActive(true);
                }
                else if ( (list[j].Value == ActionEnum.AXT) && (list[i].Value == ActionEnum.SWORD))
                {
                    Sword_beats_Axe.SetActive(true);
                    Axe_beats_Shield.SetActive(false);
                    Shield_beats_Sword.SetActive(false);
                    Axe_P1.SetActive(true);
                    Sword_P2.SetActive(true);
                }

            }
        }

        DependencyTriangle.SetActive(true);
        Controls_P1_Tutorial_Icons.SetActive(true);
        Controls_P2_Tutorial_Icons.SetActive(true);

        state = GameState.ANIMATION;
        animationsCompleted = attackers.Count;
        foreach (KeyValuePair<PlayerKeyMapping, int> player in attackers)
        {
            player.Key.playerModel.StartAnimation();
            StartCoroutine(PlayAnimation(player));
        }

        StartCoroutine(CompleteRound());
    }

    private IEnumerator CompleteRound()
    {
        yield return new WaitUntil(() => animationsCompleted <= 0);
        choices.Clear();
        readyPlayers = 0;
        keyMapping.Where(x => x.ready).ToList().ForEach(x =>
        {
            x.ready = false;
            x.playerModel.UpdateReady(x.ready);
        });

        if (gamePoints > maxScore || gamePoints < -maxScore)
        {
            if(gamePoints >= maxScore)
            {
                The_Light_Wins.SetActive(true);
            }
            else if (gamePoints <= -maxScore)
            {
                The_Dark_Wins.SetActive(true);
            }
            state = GameState.END;
        }
        else
        {
            state = GameState.ROUND;
        }
        

    }

    private IEnumerator PlayAnimation(KeyValuePair<PlayerKeyMapping, int> player)
    {     
        yield return new WaitForSeconds(player.Key.playerModel.animationTimeUntilAttack);

        gamePoints += player.Value;
        slider.value = gamePoints;

        yield return new WaitForSeconds(player.Key.playerModel.animationTimeReturn);
        animationsCompleted--;
    }

    private void GameEnd()
    {
        slider.value = 0;
        readyPlayers = 0;
        keyMapping.Where(x => x.ready).ToList().ForEach(x => x.ready = false);
        state = GameState.MENU;

        UpdateTransformSides(leftSide);
        UpdateTransformSides(rightSide);

        GameTitle.SetActive(true);
        ProgressBar.SetActive(false);
        DependencyTriangle.SetActive(false);
        Controls_P1_Tutorial_Icons.SetActive(false);
        Controls_P2_Tutorial_Icons.SetActive(false);
        Sword_beats_Axe.SetActive(false);
        Axe_beats_Shield.SetActive(false);
        Shield_beats_Sword.SetActive(false);
        gamePoints = 0;


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
