using UnityEngine;
using UnityEngine.SceneManagement;

public class MySceneManager : MonoBehaviour
{
    static MySceneManager _instance;
    public static MySceneManager Instance { get { return _instance; } }
    
    string battleScene = "Battle Scene";
    string journeyScene = "Journey Scene";
    string blackSmithScene = "Blacksmith Scene";
    string tavernScene = "Inn Scene";
    string shopScene = "Market Scene";
    string shrineScene = "Shrine Scene";
    string mainMenuScene = "Main Menu";
    string rewardSceme = "Reward Scene";

    void Awake()
    {
        if (_instance != null && _instance != this)
            Destroy(this.gameObject);
        else
            _instance = this;

        DontDestroyOnLoad(this);
    }

    ProgressSceneLoader progressSceneLoader;
    void Start()
    {
        //progressSceneLoader = this.GetComponent<ProgressSceneLoader>();
        progressSceneLoader = FindObjectOfType<ProgressSceneLoader>();

        // Fixed
        Application.targetFrameRate = 60;
    }

    public void LoadGeneral(string scenename)
    {
        Debug.Log("sceneName to load: " + scenename);

        progressSceneLoader = FindObjectOfType<ProgressSceneLoader>();
        progressSceneLoader.LoadScene(scenename);
    }

    public void LoadMainMenu() => LoadGeneral(mainMenuScene);
    
    public void LoadBattleScene()
    {
        LoadGeneral(battleScene);
    }

    public void LoadJourneyScene()
    {
        LoadGeneral(journeyScene);
    }

    public void LoadBlacksmithScene()
    {
        LoadGeneral(blackSmithScene);
    }

    public void LoadTavernScene()
    {
        LoadGeneral(tavernScene);
    }

    public void LoadShopScene()
    {
        LoadGeneral(shopScene);
    }

    public void LoadShrineScene() => LoadGeneral(shrineScene);
    public void LoadRewardScene() => LoadGeneral(rewardSceme);

}
