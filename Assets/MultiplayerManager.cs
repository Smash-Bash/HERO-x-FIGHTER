using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MultiplayerManager : MonoBehaviour
{
    [Header("Match")]
    public int round = 0;
    public int rounds = 3;
    public int stocks = 3;
    public float timeScale = 1;
    public PlayerScript[] players;
    public PlayerScript[] allPlayers;
    public Color[] colors;
    public CameraScript cameraScript;
    public CutsceneSystem cutscenes;
    public Vector2 blastZone;
    public bool activeBlastZones = false;
    public gamemodeType gamemode;
    public Transform platformHUDDisplays;
    public GameObject platformHUDPrefab;
    public HeadsUpDisplay[] traditionalHUDDisplays;
    public GameObject blastEffectPrefab;
    public GameObject traditionalHUD;
    public GameObject platformHUD;
    public float universalTimeStop;
    public Animator announcements;
    public int alivePlayers;
    public bool endOfRound;
    public float endOfRoundTime = 0;
    public bool endOfMatch;
    public EndOfMatchScreen endOfMatchScreen;
    public Image roundDisplay;
    public Image announcementDisplay;
    public Sprite fight;
    public Sprite down;
    public Sprite game;
    public GameObject hitEffect;
    public Sprite[] roundNumbers;
    public Fighter defaultFighter;
    public FighterCatalogue fighterCatalogue;

    public enum gamemodeType
    {
        Traditional, Platform
    }

    // Credit: https://www.tutorialsteacher.com/linq/linq-sorting-operators-orderby-orderbydescending
    public void OrderLoadedByPlayerID()
    {
        IList<PlayerScript> playerList = players;

        var orderByResult = from p in playerList
                            orderby p.playerID
                            select p;

        var orderByAscendingResult = from p in playerList
                                     orderby p.playerID ascending
                                     select p;

        players = orderByAscendingResult.ToArray<PlayerScript>();
    }
    public void OrderAllByPlayerID()
    {
        IList<PlayerScript> playerList = allPlayers;

        var orderByResult = from p in playerList
                            orderby p.playerID
                            select p;

        var orderByAscendingResult = from p in playerList
                                     orderby p.playerID ascending
                                     select p;

        allPlayers = orderByAscendingResult.ToArray<PlayerScript>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (GameSystems.fighters == null || GameSystems.fighters.Count == 0)
        {
            if (false)
            {
                //GameSystems.fighters.Add(defaultFighter);
                //GameSystems.inputIDs.Add(0);
                //GameSystems.fighters.Add(defaultFighter);
                //GameSystems.inputIDs.Add(1);
            }
            else if (true)
            {
                GameSystems.fighters.Add(fighterCatalogue.fighters[1]);
                GameSystems.inputIDs.Add(-1);
                GameSystems.fighters.Add(fighterCatalogue.fighters[3]);
                GameSystems.inputIDs.Add(-1);
                GameSystems.fighters.Add(fighterCatalogue.fighters[2]);
                GameSystems.inputIDs.Add(-1);
                GameSystems.fighters.Add(fighterCatalogue.fighters[0]);
                GameSystems.inputIDs.Add(-1);
            }
            else if (false)
            {
                GameSystems.fighters.Add(fighterCatalogue.fighters[0]);
                GameSystems.inputIDs.Add(0);
                GameSystems.fighters.Add(fighterCatalogue.fighters[3]);
                GameSystems.inputIDs.Add(-1);
            }
        }
        List<PlayerScript> newPlayers = new List<PlayerScript>();
        if (GameSystems.fighters.Count > 0)
        {
            int index = 0;
            foreach (Fighter fighter in GameSystems.fighters)
            {
                Fighter newFighter;
                if (fighter is RandomCharacter)
                {
                    newFighter = GameObject.Instantiate(fighterCatalogue.fighters[Random.Range(0, fighterCatalogue.fighters.Length)].gameObject, Vector3.zero, transform.rotation).GetComponent<Fighter>();
                }
                else
                {
                    newFighter = GameObject.Instantiate(fighter.gameObject, Vector3.zero, transform.rotation).GetComponent<Fighter>();
                }
                newFighter.player = newFighter.GetComponent<PlayerScript>();
                if (GameSystems.inputIDs[index] == -1)
                {
                    newFighter.player.input = newFighter.gameObject.AddComponent<ComputerInput>();
                }
                else if (GameSystems.inputIDs[index] == 0)
                {
                    newFighter.player.input = newFighter.gameObject.AddComponent<KeyboardInput>();
                }
                else if (GameSystems.inputIDs[index] > 0)
                {
                    newFighter.player.input = newFighter.gameObject.AddComponent<GamepadInput>();
                    newFighter.player.input.GetComponent<GamepadInput>().gamepadID = GameSystems.inputIDs[index];
                }
                index++;
                newFighter.player.playerID = index;
            }
        }
        players = newPlayers.ToArray();
        allPlayers = newPlayers.ToArray();

        if (gamemode == gamemodeType.Platform)
        {
            rounds = 1;
        }

        NewRound();

        if (!announcements.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            universalTimeStop = 0.125f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        players = FindObjectsOfType<PlayerScript>(false);
        allPlayers = FindObjectsOfType<PlayerScript>(true);
        OrderLoadedByPlayerID();
        OrderAllByPlayerID();

        traditionalHUD.SetActive(gamemode == gamemodeType.Traditional && !endOfMatchScreen.isActiveAndEnabled);
        platformHUD.SetActive(gamemode == gamemodeType.Platform && !endOfMatchScreen.isActiveAndEnabled);

        if (!announcements.GetCurrentAnimatorStateInfo(0).IsName("Idle"))
        {
            universalTimeStop = 0.125f;
        }

        if (universalTimeStop != 0)
        {
            universalTimeStop = Mathf.MoveTowards(universalTimeStop, 0, Time.unscaledDeltaTime);
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = timeScale;
        }

        alivePlayers = 0;
        if (gamemode == gamemodeType.Platform)
        {
            int index = 0;
            foreach (PlayerScript player in players)
            {
                if (!player.unconscious && player.stocks > 0)
                {
                    alivePlayers++;
                }
                if (player.hud == null)
                {
                    player.hud = Instantiate(platformHUDPrefab, platformHUDDisplays).GetComponent<HeadsUpDisplay>();
                    player.hud.player = player;
                }
                else
                {
                    float scale = Mathf.Abs(1f - ((Mathf.Max(players.Length + (players.Length > 4 ? 1 : 0), 4f) - 4f) / 8));
                    //platformHUDDisplays.GetComponent<RectTransform>().anchoredPosition = new Vector3(0, 100 * scale, 0);
                    player.hud.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, 450 * scale);
                    player.hud.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 250 * scale);
                    player.hud.GetComponent<RectTransform>().localScale = new Vector3(scale, scale, scale);
                }
                index++;
            }
        }
        else if (gamemode == gamemodeType.Traditional)
        {
            int index = 0;
            foreach (PlayerScript player in players)
            {
                if (!player.unconscious && player.stocks > 0)
                {
                    alivePlayers++;
                }
                if (player.hud == null && index < traditionalHUDDisplays.Length)
                {
                    traditionalHUDDisplays[index].transform.parent.gameObject.SetActive(true);
                    player.hud = traditionalHUDDisplays[index];
                    player.hud.player = player;
                }
                index++;
            }
        }

        if (alivePlayers <= 1 && !endOfRound)
        {
            if (round < rounds)
            {
                EndRound();
            }
            else
            {
                EndMatch();
            }
        }

        if (endOfRound)
        {
            endOfRoundTime += Time.deltaTime;

            if (endOfRoundTime > 5f && !endOfMatch)
            {
                NewRound();
            }
        }
        else
        {
            endOfRoundTime = 0;
        }
        endOfMatchScreen.gameObject.SetActive(endOfMatch);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(Vector3.zero, new Vector3(blastZone.x, blastZone.y, 0));
        Gizmos.DrawWireCube(Vector3.Scale(Camera.main.transform.position, new Vector3(1, 1, 0)), new Vector3((Camera.main.orthographicSize * 2) * (Camera.main.aspect), Camera.main.orthographicSize * 2, 1));
    }

    public void NewRound()
    {
        round++;

        endOfRound = false;
        endOfRoundTime = 0;

        cameraScript.transform.position = cameraScript.spawnPoint;
        cameraScript.camera.orthographicSize = cameraScript.spawnSize;
        foreach (PlayerScript player in players)
        {
            player.gameObject.SetActive(false);
            if (gamemode == gamemodeType.Traditional)
            {
                player.stocks = 1;
            }
            else
            {
                player.stocks = stocks;
            }
            player.damage = 0;
            player.transform.position = player.spawnPoint;
            player.velocity = Vector2.zero;
            player.direction = player.spawnDirection;
            player.unconscious = false;
            player.launched = false;
            player.hitstop = 0;
            player.hitstun = 0;
            player.model.animator.SetBool("Launched", false);
            player.model.animator.SetBool("Hitstun", false);
            player.gameObject.SetActive(true);
            player.model.animator.Play("Grounded");
            player.model.animator.Play("Grounded");
        }

        announcementDisplay.sprite = fight;
        roundDisplay.sprite = roundNumbers[round];
        if (rounds > 1)
        {
            announcements.Play("Round");
        }
        else
        {
            announcements.Play("Fight");
        }
        Time.timeScale = 0;
    }

    public void EndRound()
    {
        endOfRound = true;
        announcementDisplay.sprite = down;
        announcements.Play("Game");
        endOfRoundTime = 0;
        Time.timeScale = 0;
    }

    public void EndMatch()
    {
        endOfRound = true;
        endOfMatch = true;
        announcementDisplay.sprite = game;
        announcements.Play("Game");
        endOfRoundTime = 0;
        Time.timeScale = 0;
    }
}
