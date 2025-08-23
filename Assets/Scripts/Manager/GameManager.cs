using System.Collections.Generic;
using OnCloud7;
using UnityEngine;
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
    private int curMachineIndex;

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
        _machineLaunch.SetActive(true);
        _slotView.Initialize(_machines[curMachineIndex]);
        Spin();
    }

    public void Spin()
    {
        _machines[curMachineIndex].Roll();
    }

    public void SymbolsRender(List<SymbolModel> result)
    {
        _slotView.SymbolsRender(result);
    }

    public void BackToChoice()
    {
        _machineChoice.SetActive(true);
        _machineLaunch.SetActive(false);
        for (int i = 0; i < _machines.Count; i++)
        {
            _machineViews[i].Initialize(_machines[i]);
        }

    }

    public void ChangeRequest(SymbolModel ChangeSymbol, int machineID)
    {
        List<SymbolModel> symbolPool = _machines[machineID].SymbolPool;
        int beforeSymbolID = ChangeSymbol.Arg1;
        int afterSymbolID = Util.SymbolID(ChangeSymbol.Arg3);
        if (beforeSymbolID >= 0 && beforeSymbolID <= 2)
        {
            for (int i = 0; i < symbolPool.Count; i++) 
            { 
                if (symbolPool[i].ID == beforeSymbolID) 
                {
                    _machines[machineID].ChangeSymbol(beforeSymbolID, afterSymbolID);
                }
            }
        }
        else if (beforeSymbolID == 777)
        {
            List<int> changeIndexPool = new List<int>();
            foreach (SymbolModel cand in symbolPool)
            {
                if (cand.ID >= 0 && cand.ID <= 2)
                {
                    changeIndexPool.Add(cand.ID);
                }
            }

            if (changeIndexPool.Count > 0)
            {
                Random r = new Random(System.DateTime.Now.Millisecond);
                int changeIndex = r.Next(changeIndexPool.Count);
                _machines[machineID].ChangeSymbol(changeIndex, afterSymbolID);
            }

        }
        else if (beforeSymbolID == 7777)
        {
            List<int> changeIndexPool = new List<int>(); 
            foreach (SymbolModel cand in symbolPool)
            {
                if (cand.ID >= 6)
                {
                    changeIndexPool.Add(cand.ID);
                }
            }

            if (changeIndexPool.Count > 0)
            {
                Random r = new Random(System.DateTime.Now.Millisecond);
                int changeIndex = r.Next(changeIndexPool.Count);
                _machines[machineID].ChangeSymbol(changeIndex, afterSymbolID);
            }
        }
    }


    public void AddRequest(SymbolModel AddSymbol, int machineID)
    {
        _machines[machineID].AddSymbol(addIndex);
    }

    public void RemoveRequest(SymbolModel RemoveSymbol, int machineID)
    {
        _machines[machineID].RemoveSymbol(removeIndex);
    }
}
