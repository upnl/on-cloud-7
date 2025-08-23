using System.Collections.Generic;
using OnCloud7;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    
    [SerializeField] private TextAsset EnemySkillTemplateData;
    [SerializeField] private TextAsset EnemyTemplateData;
    [SerializeField] private TextAsset RoundUpgradeTemplateData;
    [SerializeField] private TextAsset SymbolTemplateData;

    private List<EnemySkillTemplate> _enemySkillTemplates;
    private List<EnemyTemplate> _enemyTemplates;
    private List<RoundUpgradeTemplate> _roundUpgradeTemplates;
    private List<SymbolTemplate> _symbolTemplates;
    
    public List<SymbolTemplate> SymbolTemplates => _symbolTemplates;

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
}
