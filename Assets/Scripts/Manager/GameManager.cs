using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using OnCloud7;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    [SerializeField]
    private BattleManager _battleManager;

    public BattleManager BattleManager => _battleManager;
    
    [SerializeField] private TextAsset EnemySkillTemplateData;
    [SerializeField] private TextAsset EnemyTemplateData;
    [SerializeField] private TextAsset RoundUpgradeTemplateData;
    [SerializeField] private TextAsset SymbolTemplateData;

    private List<EnemySkillTemplate> _enemySkillTemplates;
    private List<EnemyTemplate> _enemyTemplates;
    private List<RoundUpgradeTemplate> _roundUpgradeTemplates;
    private List<SymbolTemplate> _symbolTemplates;
    
    public List<SymbolTemplate> SymbolTemplates => _symbolTemplates;
    public List<EnemyTemplate> EnemyTemplates => _enemyTemplates;
    public List<EnemySkillTemplate> EnemySkillTemplates => _enemySkillTemplates;
    public List<RoundUpgradeTemplate> RoundUpgradeTemplates => _roundUpgradeTemplates;

    private List<MachineModel> _machines = new List<MachineModel>();
    [SerializeField]
    private List<MachineView> _machineViews;
    
    [SerializeField]
    private SlotView _slotView;

    [SerializeField] private GameObject _machineChoice;
    [SerializeField] private GameObject _machineLaunch;
    [SerializeField] private GameObject _gameOver;
    private int curMachineIndex;

    [SerializeField] private Button _backButton;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }

        Instance = this;

        _symbolTemplates = SymbolTemplateData.ToSymbolTemplates();
        _enemySkillTemplates = EnemySkillTemplateData.ToEnemySkillTemplates();
        _enemyTemplates = EnemyTemplateData.ToEnemyTemplates(_enemySkillTemplates);
        _roundUpgradeTemplates = RoundUpgradeTemplateData.ToRoundUpgradeTemplates();

        if (!_battleManager.GameStarted)
        {
            _battleManager.Initialize();
        }

        _machines.Clear();
        for (int i = 0; i < 2; i++)
        {
            MachineModel machine = new MachineModel();
            machine.Initialize();
            _machines.Add(machine);
            _machineViews[i].Initialize(machine);
        }
        
    }

    public void StartSpin(int curMachineIndex)
    {
        for (int i = 0; i < _machines.Count; i++)
        {
            _machineViews[i].CleanPool();
        }
        this.curMachineIndex = curMachineIndex;
        _machineChoice.SetActive(false);
        SetBackButtonInteractable(false);
        _machineLaunch.SetActive(true);
        _slotView.Initialize(_machines[curMachineIndex]);
        Spin();
    }

    public void Spin()
    {
        _machines[curMachineIndex].Roll();
    }

    public void RenderSymbols(List<SymbolModel> result)
    {
        _slotView.SymbolsRender(result);
    }

    public void SetBackButtonInteractable(bool interactable)
    {
        _backButton.interactable = interactable;
    }

    public void BackToChoice()
    {
        _machineChoice.SetActive(true);
        _machineLaunch.SetActive(false);
        SetBackButtonInteractable(false);
        for (int i = 0; i < _machines.Count; i++)
        {
            _machineViews[i].Initialize(_machines[i]);
        }

    }

    public void GameOver()
    {
        _machineChoice.SetActive(false);
        _machineLaunch.SetActive(false);
        _gameOver.SetActive(true);
        
    }

    public void ChangeRequest(SymbolModel targetSymbol, int machineID)
    {
        List<SymbolModel> symbolPool = _machines[machineID].SymbolPool;
        int beforeSymbolID = targetSymbol.Arg1;
        int afterSymbolID = Util.SymbolID(targetSymbol.Arg3);
        if (beforeSymbolID >= 0 && beforeSymbolID <= 2)
        {
            for (int i = 0; i < symbolPool.Count; i++) 
            { 
                if (symbolPool[i].ID == beforeSymbolID) 
                {
                    _machines[machineID].ChangeSymbol(beforeSymbolID, afterSymbolID);
                    break;
                }
            }
        }
        else if (beforeSymbolID == 777)
        {
            List<int> changeIndexPool = new List<int>();
            for (int i = 0; i < symbolPool.Count; i++)
            {
                if (symbolPool[i].ID >= 0 && symbolPool[i].ID <= 2)
                {
                    changeIndexPool.Add(i);
                }
            }

            if (changeIndexPool.Count > 0)
            {
                Random r = new Random(System.DateTime.Now.Millisecond);
                int changeIndex = changeIndexPool[r.Next(changeIndexPool.Count)];
                _machines[machineID].ChangeSymbol(changeIndex, afterSymbolID);
            }

        }
        else if (beforeSymbolID == 7777)
        {
            List<int> changeIndexPool = new List<int>(); 
            for (int i = 0; i < symbolPool.Count; i++)
            {
                if (symbolPool[i].ID >= 7)
                {
                    changeIndexPool.Add(i);
                }
            }

            if (changeIndexPool.Count > 0)
            {
                Random r = new Random(System.DateTime.Now.Millisecond);
                int changeIndex = changeIndexPool[r.Next(changeIndexPool.Count)];
                _machines[machineID].ChangeSymbol(changeIndex, afterSymbolID);
            }
        }
    }


    public void AddRequest(SymbolModel AddSymbol, int machineID)
    {
        if (_machines[machineID].SymbolPool.Count < 48)
        {
            _machines[machineID].AddSymbol(Util.SymbolID(AddSymbol.Arg1));
        }
        
    }

    public void RemoveRequest(SymbolModel RemoveSymbol, int machineID)
    {
        List<SymbolModel> symbolPool = _machines[machineID].SymbolPool;
        int removeTargetID = RemoveSymbol.Arg1;
        if (removeTargetID >= 0 && removeTargetID <= 2)
        {
            for (int i = 0; i < symbolPool.Count; i++)
            {
                if (symbolPool[i].ID == removeTargetID)
                {
                    _machines[machineID].RemoveSymbol(i);
                    break;
                }
            }
        }
        else if (removeTargetID == 777)
        {
            List<int> removeIndexPool = new List<int>();
            for (int i = 0; i < symbolPool.Count; i++)
            {
                if (symbolPool[i].ID >= 0 & symbolPool[i].ID <= 2)
                {
                    removeIndexPool.Add(i);
                }
            }

            if (removeIndexPool.Count > 0)
            {
                Random r = new Random(System.DateTime.Now.Millisecond);
                int removeIndex = removeIndexPool[r.Next(removeIndexPool.Count)];
                _machines[machineID].RemoveSymbol(removeIndex);
            }
        }
        else if (removeTargetID == 7777)
        {
            List<int> removeIndexPool = new List<int>();
            for (int i = 0; i < symbolPool.Count; i++)
            {
                if (symbolPool[i].ID >= 7)
                {
                    removeIndexPool.Add(i);
                }
            }

            if (removeIndexPool.Count > 0)
            {
                Random r = new Random(System.DateTime.Now.Millisecond);
                int removeIndex = removeIndexPool[r.Next(removeIndexPool.Count)];
                _machines[machineID].RemoveSymbol(removeIndex);
            }
        }
    }

    public async UniTask RoundUpgrade(int round, int upgradeGrade)
    {
        //pass
    }
}
