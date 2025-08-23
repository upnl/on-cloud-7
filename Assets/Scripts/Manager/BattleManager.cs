using UnityEngine;

namespace OnCloud7
{
    public class BattleManager : MonoBehaviour
    {
        private bool _gameStarted = false;
        public bool GameStarted => _gameStarted;
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
        
        /*
         * BattleManager TODO:
         * 1. BattleManager.Initialize() 호출 -> Complete
         * 2. 깨달음 선택 후 LoadNextRound() 호출하며 다음 라운드 준비 (UI도 변경)
         * 3. 현재 남은 체력 등의 정보들을 UI에 표시
         * 4. Roll()의 결과로 나온 CheckResult()의 gains들을 BattleManager에 넘겨서 수치 변동에 반영할 수 있도록
         * 5. 라운드 끝날 때 UpgradeLevel()로 업그레이드 레벨 확인 후 RoundUpgradeTemplate에서 불러와서 증강체 3개 선택지 띄우기
         * 6. 적이 스킬을 돌아가면서 쓰도록 하기
         * 7. 적 특수 패턴 (예: 군대의 전역 등)
         */

        public void Initialize()
        {
            _gameStarted = true;
            _round = 0;
            _playerHealth = 100;
            GameManager.Instance.RoundUpgrade(0, 1);
            LoadNextRound();
        }

        public void LoadNextRound()
        {
            _enemyTemplate = GameManager.Instance.EnemyTemplates[_round];
            if (_round > 0)
            {
                GameManager.Instance.RoundUpgrade(_round, UpgradeLevel(_upgradePoint));
            }
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
