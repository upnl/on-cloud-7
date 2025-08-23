using System.Collections.Generic;
using OnCloud7;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private TextAsset EnemySkillTemplateData;
    [SerializeField] private TextAsset EnemyTemplateData;
    [SerializeField] private TextAsset RewardTemplateData;
    [SerializeField] private TextAsset RoundUpgradeTemplateData;
    [SerializeField] private TextAsset SymbolTemplateData;

    private List<EnemySkillTemplate> _enemySkillTemplates;
    private List<EnemyTemplate> _enemyTemplates;
    private List<RewardTemplate> _rewardTemplates;
    private List<RoundUpgradeTemplate> _roundUpgradeTemplates;
    private List<SymbolTemplate> _symbolTemplates;
    
    void Awake()
    {
        _enemySkillTemplates = EnemySkillTemplateData.ToEnemySkillTemplates();
        _enemyTemplates = EnemyTemplateData.ToEnemyTemplates(_enemySkillTemplates);
        _rewardTemplates = RewardTemplateData.ToRewardTemplates();
        _roundUpgradeTemplates = RoundUpgradeTemplateData.ToRoundUpgradeTemplates();
        _symbolTemplates = SymbolTemplateData.ToSymbolTemplates();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
