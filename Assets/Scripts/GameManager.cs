using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // player variables
    public float money = 10;
    [HideInInspector] public int day = 0;



    // economic variables
    public int population = 10;
    private float _demand;
    private float _governmentEffect;
    private float _weatherEffect;
    private float _catastropheEffect;
    [Range(0, 1)] public float preferenceForPrimaryGood;

    // goods
    public GameObject banana;
    public int bananas = 0;
    public int bananaPrice = 1;
    public GameObject apple;
    public int apples = 0;
    public int applePrice = 1;

    // world
    public GameObject[] npcs;
    private int _npcCount;
    public Vector3 npcSpawnLocation;
    public float npcSpawnDelay = 2f;
    private float _npcSpawnTimer = 0;
    public string worldStateString = "Normal";
    private bool _isCatastrophe = false;


    // game state
    private bool _shouldSpawnNPCs = false;

    [SerializeField] private TextMeshProUGUI _moneyText;
    [SerializeField] private TextMeshProUGUI _dayText;
    [SerializeField] private TextMeshProUGUI _applesCount;
    [SerializeField] private TextMeshProUGUI _bananasCount;
    [SerializeField] private TextMeshProUGUI _worldStateText;
    [SerializeField] private Button _nextDayButton;
    [SerializeField] private Button _buyApplesButton;
    [SerializeField] private Button _buyBananasButton;


    private void Start(){
        EnableButtons();
    }
    private void Update(){
        if (_shouldSpawnNPCs){
            DisableButtons();
            if (_npcCount <= 0){
                _shouldSpawnNPCs = false;
                EnableButtons();
                SpoilFruit();
            }
            else if (_npcSpawnTimer <= 0){
                SpawnNPC();
                _npcSpawnTimer = npcSpawnDelay;
                _npcCount--;
            }
            else{
                _npcSpawnTimer -= Time.deltaTime;
            }
        }

        UpdateUI();

    }

    private void DisableButtons(){
        _nextDayButton.interactable = false;
        _buyApplesButton.interactable = false;
        _buyBananasButton.interactable = false;
    }
    private void EnableButtons(){
        _nextDayButton.interactable = true;
        _buyApplesButton.interactable = true;
        _buyBananasButton.interactable = true;
    }
    private void SpoilFruit(){
        apples = (int) (apples * 0.25f);
        bananas = (int) (bananas * 0.25f);
    }

    private void UpdateUI(){
        _moneyText.text = "Money: " + money;
        _dayText.text = "Day: " + day;
        _applesCount.text = "Apples: " + apples;
        _bananasCount.text = "Bananas: " + bananas;
        _worldStateText.text = worldStateString;
    }


    private void SpawnNPC(){
        int randomIndex = Random.Range(0, npcs.Length);
        GameObject npc = Instantiate(npcs[randomIndex], npcSpawnLocation, Quaternion.identity);
    }

    public void NextDay(){
        WorldEffects();
        day++;
        money++;
        // calculate _demand
        _demand = _demand * population;
        _npcCount = (int) _demand;
        Debug.Log("Demand: " + _demand);
        _shouldSpawnNPCs = true;
    }

    public void NPCBuyGood(string good){
        if (good == "banana"){
            if (bananas <= 0) return;
            bananas--;
            money += bananaPrice * 2;
        }
        else if (good == "apple"){
            if (apples <= 0) return;
            apples--;
            money += applePrice * 2;
        }
    }
    public void PlayerBuyGood(string good){
        if (money < 1) return;
        if (good == "banana"){
            bananas++;
            money -= bananaPrice;
        }
        else if (good == "apple"){
            apples++;
            money -= applePrice;
        }
    }

    public void WorldEffects(){
        _weatherEffect = CalculateWeatherEffect(day);
        _catastropheEffect = CalculateCatasropheEffect();
        _governmentEffect = CalculateGovernmentEffect();
        _demand = _governmentEffect * _weatherEffect * _catastropheEffect;

        worldStateString = "";
        if (_isCatastrophe) worldStateString += "A Catastrophe has occured! Demand is down! ";
        if (_weatherEffect >= 1) worldStateString += "The weather is good! Demand is up by " + (_weatherEffect - 1) * 100 + "%! ";
        if (_weatherEffect < 1) worldStateString += "The weather is bad! Demand is down by " + (1 - _weatherEffect) * 100 + "%! ";
        if (_governmentEffect >= 1) worldStateString += "The government is increasing money supply! Demand is up by " + (_governmentEffect - 1) * 100 + "%! ";
        if (_governmentEffect < 1) worldStateString += "The government is decreasing money supply! Demand is down by " + (1 - _governmentEffect) * 100 + "%! ";
        worldStateString += "Demand today was: " + (int) (_demand * population) + " goods! ";

    }
    private float CalculateWeatherEffect(int curDay){
        float tempValue = (Mathf.Sin(curDay/3f) + Mathf.Sin(Mathf.PI * curDay/4f)) / 2f + 1f;
        return tempValue;
    }

    private float CalculateCatasropheEffect(){
        if (Random.Range(0, 100) < 5){
            _isCatastrophe = true;
            return 0.33f;
        }
        else{
            _isCatastrophe = false;
            return 1;
        }
    }

    private float CalculateGovernmentEffect(){
        float tempValue = 0;
        if (_isCatastrophe) tempValue = Random.Range(1.15f, 2f);
        else tempValue = Random.Range(0.85f, 1.15f);
        return tempValue;
    }

}
