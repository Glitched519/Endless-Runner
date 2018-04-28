using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CollisionManager))] 
public class ScoreKeeper : MonoBehaviour {
    double score;
    [SerializeField]
    int scoreRate = 250;
    [SerializeField]
    Text scoreText;

    class Leaderboard
    {
        float vPrev = 0;
        int currInitial = 0;
        float blinkTimer = 0;
        float blinkTime = 0.3f;
        char[] initialChars;
        int currInitialChar = 0;
        public const int numSlots = 8;
        public leaderboardEntry[] entries;

        string[] defaultLeaderboard = {
                                       "1:CAL:150000",
                                       "2:VAC:120000",
                                       "3:SUP:100000",
                                       "4:NUB:80000", 
                                       "5:LEL:70000",
                                       "6:BAD:50000", 
                                       "7:PLS:40000",
                                       "8:NOP:25000"
                                      };
        public Transform root;
        public LeaderboardUIEntry[] uiEntries;
        public Text uiContextMessage;
        string tempInitials = "";
        int place = -1;
        ulong score;

        public void close()
        {
            Save();
            root.gameObject.SetActive(false);
        }

        public void ENTERHIGHSCORE(int place, ulong score)
        {
            this.place = place;
            this.score = score;
            entries[place] = new leaderboardEntry("AAA", score);
        }
        public struct LeaderboardUIEntry
        {
            public Text[] initials;
            public Text score;
        };

        public void Update()
        {
            if(place < 0)
            {
                if (Input.anyKeyDown)
                {
                    GameManager.Instance.resetLevel();
                }
            }
            else
            {
                blinkTimer += Time.deltaTime;
                if(blinkTimer >= blinkTime)
                {
                    blinkTimer -= blinkTime;
                    uiEntries[place].initials[currInitial].enabled = !uiEntries[place].initials[currInitial].enabled;
                }
                float vNew = Input.GetAxisRaw("Vertical");
                float vDelta = vNew - vPrev;
                if (Mathf.Abs(vDelta) >= 0.3f && Mathf.Abs(vNew) >= 0.5f)
                {
                    int dur = Mathf.RoundToInt(vNew);
                    currInitialChar = Mathf.Abs((currInitialChar + dur) % initialChars.Length);
                    uiEntries[place].initials[currInitial].text = initialChars[currInitialChar].ToString();
                }
                else if (Input.anyKeyDown)
                {
                    tempInitials  += initialChars[currInitialChar].ToString(); 
                    if(++currInitial  > 2)
                    {
                        entries[place].initials = tempInitials;
                        currInitial = 0;
                        place = -1;
                        Save();
                        uiContextMessage.text = "Press any key to continue.";
                    }
                }
                vPrev = vNew;
            }
        }
        public void display()
        {
            root.gameObject.SetActive(true);
            for(int i = 0; i < numSlots; i++)
            {
                string initials = entries[i].initials;
                for(int j = 0; j < 3; j++)
                {
                    uiEntries[i].initials[j].text = initials[j].ToString();
                }
                uiEntries[i].score.text = entries[i].score.ToString();
            }
        }
        public void initializeUI(Transform canvas)
        {
            uiEntries = new LeaderboardUIEntry[numSlots];
            for (int i = 0; i < numSlots; i++)
            {
                uiEntries[i].initials = new Text[3];
            }
            root = canvas.Find("UI_Leaderboard");
            for (int i = 0; i < numSlots; i++)
            {
                Transform entryRoot = root.Find("entry_" + (i + 1));
                for (int j = 0; j < 3; j++)
                {
                    uiEntries[i].initials[j] = entryRoot.Find("Initial " + (j + 1)).GetComponent<Text>();
                }
                uiEntries[i].score = entryRoot.Find("Score").GetComponent<Text>();
                uiContextMessage = root.Find("Message").GetComponent<Text>();
            }
        }
        public Leaderboard()
        {
            initialChars = new char[26];
            int index = 0;
            for(char c = 'A'; c <= 'Z'; c++)
            {
                initialChars[index] = c;
                index++;
            }
            entries = new leaderboardEntry[numSlots];
            try
            {
                Load();
            }
            catch (FileNotFoundException e)
            {
                Debug.LogError("Leaderboard data missing.");
                Debug.LogException(e);
                Wipe();
            }
            catch (System.IndexOutOfRangeException e)
            {
                Debug.LogError("Leaderboard data corrupted");
                Debug.LogException(e);
                Wipe();
            }
            catch (System.FormatException e)
            {
                Debug.LogError("Format Invalid");
                Debug.LogException(e);
                Wipe();
            }

        }

        void Wipe()
        {
            File.WriteAllLines(Application.dataPath + "/_Data/Leaderboard_Data.sav", defaultLeaderboard);
            try
            {
                Load();
            }
            catch (System.Exception e)
            {
                Debug.LogError("Something is seriously wrong");
                Debug.LogException(e);
            }
        }

        public void Save()
        {
            string[] data = new string[entries.Length];
            for (int i = 0; i < entries.Length; i++)
            {
                data[i] = "" + (i + 1) + ':' + entries[i].initials + ':' + entries[i].score;
            }
            
            File.WriteAllLines(Application.dataPath + "/StreamingAssets/Leaderboard_Data.sav", data);
        }


        public void Load()
        {
            TextAsset text = Resources.Load<TextAsset>("Saves/Leaderboard_Data");
            Debug.Log(text);
            string[] loadedEntries = File.ReadAllLines(Application.dataPath + "/StreamingAssets/Leaderboard_Data.sav");
            for (int i = 0; i < loadedEntries.Length; i++)
            {
                string[] loadedItemProperties = loadedEntries[i].Split(':');
                int place = int.Parse(loadedItemProperties[0]);
                string initials = loadedItemProperties[1];
                ulong score = ulong.Parse(loadedItemProperties[2]);
                entries[place - 1] = new leaderboardEntry(initials, score);

            }

        }
        
        public struct leaderboardEntry
        {
            public string initials;
            public ulong score;

            public leaderboardEntry(string initials, ulong score)
            {
                this.initials = initials;
                this.score = score;
            }
        }
    }

    public void closeLeaderboard()
    {
        leaderboard.close();
        scoreText.enabled = true;
        score = 0.0;
    }
    Leaderboard leaderboard;
	// Use this for initialization
	void Start () {
        score = 0;
        Debug.Log(GetComponent<CollisionManager>());
        GetComponent<CollisionManager>().onObstacleCollision += displayLeaderboard;
        leaderboard = new Leaderboard();
        InitializeUI();
	}

    void InitializeUI()
    {
        Transform canvas = GameObject.Find("LeaderboardCanvas").transform;
        leaderboard.initializeUI(canvas);
        scoreText = canvas.Find("UiScore").GetComponent<Text>();
    }

    private void displayLeaderboard()
    {
        if(score > leaderboard.entries[Leaderboard.numSlots-1].score)
        {
            leaderboard.uiContextMessage.text = "HIGHSCORE! Enter your initials:";
            int curr = Leaderboard.numSlots-1;
            while(--curr >= 0){
                if(score >= leaderboard.entries[curr].score)
                {
                    break;
                }
            }
            ++curr;
            for (int i = Leaderboard.numSlots - 1; i > curr; i--)
            {
                leaderboard.entries[i] = leaderboard.entries[i - 1];
            }
            leaderboard.ENTERHIGHSCORE(curr, (ulong)score); 
        }
        else
        {
            leaderboard.uiContextMessage.text = "You took an L, press any key to retry.";
        }

        scoreText.enabled = false;
        leaderboard.display();
    }
	
	// Update is called once per frame
	void Update () {
        if (leaderboard.root.gameObject.activeInHierarchy)
        {
            leaderboard.Update();
        }
        else
        {
            score += Time.deltaTime * scoreRate;
            scoreText.text = "Score: " + (ulong)score;
        }
	}
}
