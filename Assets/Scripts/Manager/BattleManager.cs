using UnityEngine;

namespace OnCloud7
{
    public class BattleManager : MonoBehaviour
    {
        /// <summary>
        /// 몇 라운드인가? (1 ~ 7)
        /// </summary>
        private int _round;
        
        /// <summary>
        /// 플레이어 남은 체력 (라운드
        /// </summary>
        private int _playerHealth;

        /// <summary>
        /// 플레이어 회피율 (최대 1f)
        /// </summary>
        private float _avoidability;

        /// <summary>
        /// 깨달음(증강체) 경지
        /// </summary>
        private int _upgradePoint;

        private EnemyTemplate _enemyTemplate;

        /// <summary>
        /// 적 남은 체력
        /// </summary>
        private int _enemyCurrentHealth;

        /// <summary>
        /// 적의 다음 스킬 순서
        /// </summary>
        private int _enemySkillIndex;

        public void Initialize()
        {
            _round = 0;
            _playerHealth = 100;
            LoadNextRound();
        }

        public void LoadNextRound()
        {
            _enemyTemplate = GameManager.Instance.EnemyTemplates[_round];
            _round++;
            _enemyCurrentHealth = _enemyTemplate.Health;
            _upgradePoint = 0;
            _enemySkillIndex = 0;
        }

        private int UpgradeLevel(int upgradePoint)
        {
            switch (upgradePoint)
            {
                case < 50:
                    return 0;
                case < 150:
                    return 1;
                case < 500:
                    return 2;
                default:
                    return 3;
            }
        }
    }
}
