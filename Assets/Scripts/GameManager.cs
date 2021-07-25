public class GameManager : Singleton<GameManager> {

    RoadManager rm;
    CollisionManager cm;
    ScoreKeeper sk;
    PlayerController pc;

	// Use this for initialization
	void Start () {
        rm = GetComponent<RoadManager>();
        cm = GetComponent<CollisionManager>();
        sk = GetComponent<ScoreKeeper>();
        pc = PlayerController.Instance;
        cm.onObstacleCollision += pauseGameplay;
    }

    public void resetLevel()
    {
        sk.closeLeaderboard();
        rm.reset();
        pc.Reset();
        cm.enabled = true;
    }

    public void pauseGameplay()
    {
        pc.gameObject.SetActive(false);
        rm.enabled = false;
        cm.enabled = false;
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
