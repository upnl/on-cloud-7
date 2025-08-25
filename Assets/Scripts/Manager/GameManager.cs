using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using OnCloud7;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
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
    [SerializeField] private GameObject _upgradePanel;
    [SerializeField] private GameObject _gameOver;
    [SerializeField] private List<UpgradeView> _upgradeViews;
    [SerializeField] private GameObject _showMachinePanel;
    [SerializeField] private Button _showMachineButton;
    [SerializeField] private List<MachineView> _showMachineViews;
    
    private int curMachineIndex;

    [SerializeField] private Button _backButton;
    [SerializeField] private TextMeshProUGUI _backButtonText;
    
    public TextMeshProUGUI BackButtonText => _backButtonText;
    
    public bool UpgradeCompleted { private get; set; }

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
            machine.Initialize(i);
            _machines.Add(machine);
            _machineViews[i].Initialize(machine);
            _showMachineViews[i].Initialize(machine);
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
        _upgradePanel.SetActive(false);
        _gameOver.SetActive(false);
        _showMachineButton.gameObject.SetActive(true);
        _showMachinePanel.SetActive(false);
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
        SetBackButtonInteractable(false);
        if (BattleManager.EnemyCurrentHealth <= 0)
        {
            BattleManager.LoadNextRound().Forget();
        }
        else if (BattleManager.PlayerHealth <= 0)
        {
            GameOver();
        }
        else
        {
            _machineChoice.SetActive(true);
            _machineLaunch.SetActive(false);
            _upgradePanel.SetActive(false);
            _gameOver.SetActive(false);
            _showMachineButton.gameObject.SetActive(false);
            _showMachinePanel.SetActive(false);
            for (int i = 0; i < _machines.Count; i++)
            {
                _machines[i].SymbolPool.Sort();
                _machineViews[i].Initialize(_machines[i]);
            }

            BattleManager.SetNextEnemyAttackText();
        }
    }

    public void ShowMachinePanel()
    {
        for (int i = 0; i < _machines.Count; i++)
        {
            _showMachineViews[i].CleanPool();
            _showMachineViews[i].Initialize(_machines[i]);
        }

        _showMachinePanel.SetActive(true);
    }

    public void CloseMachinePanel()
    {
        _showMachinePanel.SetActive(false);
    }

    public void GameOver()
    {
        _machineChoice.SetActive(false);
        _machineLaunch.SetActive(false);
        _upgradePanel.SetActive(false);
        _gameOver.SetActive(true);
        _showMachinePanel.SetActive(false);
        _showMachineButton.gameObject.SetActive(true);
    }

    public void BackToTitleScene()
    {
        SceneManager.LoadScene(0);
    }

    public void ChangeRequest(SymbolModel targetSymbol, int machineID)
    {
        ChangeRequest(targetSymbol.Arg1, targetSymbol.Arg2, targetSymbol.Arg3, machineID);
    }

    public void ChangeRequest(int targetSymbolID, int repeat, int afterSymbolID, int machineID)
    {
        List<SymbolModel> symbolPool = _machines[machineID].SymbolPool;
        int afterSymbolIndex;
        List<int> changeIndexPool = new List<int>();
        Random r = Util.Random;
        for (int rep = 0; rep < repeat; rep++)
        {
            afterSymbolIndex = Util.SymbolIDToIndex(afterSymbolID);
            
            changeIndexPool.Clear();
            for (int i = 0; i < symbolPool.Count; i++)
            {
                if (Util.SymbolPredicate(targetSymbolID, symbolPool[i]))
                {
                    changeIndexPool.Add(i);
                }
            }

            if (changeIndexPool.Count > 0)
            {
                int changeIndex = changeIndexPool[r.Next(changeIndexPool.Count)];
                _machines[machineID].ChangeSymbol(changeIndex, afterSymbolIndex);
            }
            /*
            switch (targetSymbolID)
            {
                case >= 0 and <= 2:
                case 7:
                {
                    for (int i = 0; i < symbolPool.Count; i++)
                    {
                        if (symbolPool[i].ID == targetSymbolID && !symbolPool[i].IsImmutable)
                        {
                            _machines[machineID].ChangeSymbol(targetSymbolID, afterSymbolIndex);
                            break;
                        }
                    }

                    break;
                }
                case 8:
                case 9:
                case 10:
                    changeIndexPool.Clear();
                    for (int i = 0; i < symbolPool.Count; i++)
                    {
                        if (symbolPool[i].ID == targetSymbolID && !symbolPool[i].IsImmutable)
                        {
                            changeIndexPool.Add(i);
                        }
                    }

                    if (changeIndexPool.Count > 0)
                    {
                        int changeIndex = changeIndexPool[r.Next(changeIndexPool.Count)];
                        _machines[machineID].ChangeSymbol(changeIndex, afterSymbolIndex);
                    }
                    break;
                case 777:
                {
                    changeIndexPool.Clear();
                    for (int i = 0; i < symbolPool.Count; i++)
                    {
                        if (symbolPool[i].ID >= 0 && symbolPool[i].ID <= 2 && !symbolPool[i].IsImmutable)
                        {
                            changeIndexPool.Add(i);
                        }
                    }

                    if (changeIndexPool.Count > 0)
                    {
                        int changeIndex = changeIndexPool[r.Next(changeIndexPool.Count)];
                        _machines[machineID].ChangeSymbol(changeIndex, afterSymbolIndex);
                    }

                    break;
                }
                case 7777:
                {
                    changeIndexPool.Clear();
                    for (int i = 0; i < symbolPool.Count; i++)
                    {
                        if (symbolPool[i].ID >= 7 && !symbolPool[i].IsImmutable)
                        {
                            changeIndexPool.Add(i);
                        }
                    }

                    if (changeIndexPool.Count > 0)
                    {
                        int changeIndex = changeIndexPool[r.Next(changeIndexPool.Count)];
                        _machines[machineID].ChangeSymbol(changeIndex, afterSymbolIndex);
                    }

                    break;
                }
            }
            */
        }
    }


    public void AddRequest(SymbolModel addSymbol, int machineID)
    {
        AddRequest(addSymbol.Arg1, addSymbol.Arg2, machineID);
    }
    
    public void AddRequest(int symbolID, int repeat, int machineID)
    {
        if (symbolID < 0) return;
        for (int i = 0; i < repeat; i++)
        {
            if (_machines[machineID].SymbolPool.Count < 81)
            {
                _machines[machineID].AddSymbol(Util.SymbolIDToIndex(symbolID));
            }
        }
    }

    public void RemoveRequest(SymbolModel removeSymbol, int machineID)
    {
        RemoveRequest(removeSymbol.Arg1, removeSymbol.Arg2, machineID);
    }

    public void RemoveRequest(int targetSymbol1ID, int repeat1, int machineID)
    {
        RemoveRequest(targetSymbol1ID, repeat1, -1, -1, machineID);
    }

    public void RemoveRequest(int targetSymbol1ID, int repeat1, int targetSymbol2ID, int repeat2, int machineID)
    {
        List<SymbolModel> symbolPool = _machines[machineID].SymbolPool;
        List<int> removeIndexPool = new List<int>();
        Random r = Util.Random;
        for (int rep = 0; rep < repeat1; rep++)
        {
            Remove(targetSymbol1ID);
        }

        for (int rep2 = 0; rep2 < repeat2; rep2++)
        {
            Remove(targetSymbol2ID);
        }

        void Remove(int targetSymbolID)
        {
            removeIndexPool.Clear();
            for (int i = 0; i < symbolPool.Count; i++)
            {
                if (Util.SymbolPredicate(targetSymbolID, symbolPool[i]))
                {
                    removeIndexPool.Add(i);
                }
            }

            if (removeIndexPool.Count > 0)
            {
                int removeIndex = removeIndexPool[r.Next(removeIndexPool.Count)];
                _machines[machineID].RemoveSymbol(removeIndex);
            }
            /*
            switch (targetSymbolID)
            {
                case >= 0 and <= 2:
                case 7:
                {
                    for (int i = 0; i < symbolPool.Count; i++)
                    {
                        if (symbolPool[i].ID == targetSymbolID && !symbolPool[i].IsImmutable)
                        {
                            _machines[machineID].RemoveSymbol(i);
                            break;
                        }
                    }

                    break;
                }
                case 8:
                case 9:
                case 10:
                {
                    removeIndexPool.Clear();
                    for (int i = 0; i < symbolPool.Count; i++)
                    {
                        if (symbolPool[i].ID == targetSymbolID && !symbolPool[i].IsImmutable)
                        {
                            removeIndexPool.Add(i);
                        }
                    }

                    if (removeIndexPool.Count > 0)
                    {
                        int removeIndex = removeIndexPool[r.Next(removeIndexPool.Count)];
                        _machines[machineID].RemoveSymbol(removeIndex);
                    }

                    break;
                }
                case 777:
                {
                    removeIndexPool.Clear();
                    for (int i = 0; i < symbolPool.Count; i++)
                    {
                        if (symbolPool[i].ID >= 0 & symbolPool[i].ID <= 2 && !symbolPool[i].IsImmutable)
                        {
                            removeIndexPool.Add(i);
                        }
                    }

                    if (removeIndexPool.Count > 0)
                    {
                        int removeIndex = removeIndexPool[r.Next(removeIndexPool.Count)];
                        _machines[machineID].RemoveSymbol(removeIndex);
                    }

                    break;
                }
                case 7777:
                {
                    removeIndexPool.Clear();
                    for (int i = 0; i < symbolPool.Count; i++)
                    {
                        if (symbolPool[i].ID >= 7 && !symbolPool[i].IsImmutable)
                        {
                            removeIndexPool.Add(i);
                        }
                    }

                    if (removeIndexPool.Count > 0)
                    {
                        int removeIndex = removeIndexPool[r.Next(removeIndexPool.Count)];
                        _machines[machineID].RemoveSymbol(removeIndex);
                    }

                    break;
                }
            }
            */
        }
    }

    public void ResetRequest(int machineID)
    {
        _machines[machineID].Initialize(machineID);
        _machineViews[machineID].Initialize(_machines[machineID]);
    }

    public async UniTask RoundUpgrade(List<int> upgradeGrade)
    {
        _machineChoice.SetActive(false);
        _machineLaunch.SetActive(false);
        _upgradePanel.SetActive(true);
        _gameOver.SetActive(false);
        _showMachineButton.gameObject.SetActive(true);
        _showMachinePanel.SetActive(false);
        UpgradeCompleted = false;
        Dictionary<int, Dictionary<string, List<RoundUpgradeTemplate>>> candidates = new();
        Dictionary<int, List<string>> nameCandidates = new();

        foreach (var template in RoundUpgradeTemplates)
        {
            candidates.TryAdd(template.Level, new Dictionary<string, List<RoundUpgradeTemplate>>());
            candidates[template.Level].TryAdd(template.Name, new List<RoundUpgradeTemplate>());
            candidates[template.Level][template.Name].Add(template);
            nameCandidates.TryAdd(template.Level, new List<string>());
            if (!nameCandidates[template.Level].Contains(template.Name))
                nameCandidates[template.Level].Add(template.Name);
        }

        for (int i = 0; i < _upgradeViews.Count; i++)
        {
            int selectedNameIndex = Util.Random.Next(nameCandidates.Count);
            var list = candidates[upgradeGrade[i]][nameCandidates[upgradeGrade[i]][selectedNameIndex]];
            _upgradeViews[i].Initialize(list[Util.Random.Next(list.Count)]);
            nameCandidates[upgradeGrade[i]].RemoveAt(selectedNameIndex);
        }

        await UniTask.WaitUntil(this, (t) => t.UpgradeCompleted);
    }
}
