using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace OnCloud7
{
    public class BattleManager : MonoBehaviour
    {
        private bool _gameStarted = false;
        public bool GameStarted => _gameStarted;
        
        public enum BattleState { SelectMachine, RollAndAttack, SelectUpgrade }
        
        private BattleState _state;
        
        /// <summary>
        /// 몇 라운드인가? (1 ~ 7)
        /// </summary>
        private int _round;
        
        /// <summary>
        /// 플레이어 남은 체력 (라운드
        /// </summary>
        private int _playerHealth;

        /// <summary>
        /// 플레이어 명중률 (1f - 회피율)
        /// </summary>
        private float _hitRate;

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

        public async UniTask LoadNextRound()
        {
            _enemyTemplate = GameManager.Instance.EnemyTemplates[_round];
            if (_round > 0)
            {
                await GameManager.Instance.RoundUpgrade(_round, UpgradeLevel(_upgradePoint));
            }
            _round++;
            _enemyCurrentHealth = _enemyTemplate.Health;
            _upgradePoint = 0;
            _enemySkillIndex = 0;
        }

        public async UniTask ProcessRollResult(Dictionary<int, int> gains)
        {
            // 기본 문양만 여기서 처리합니다.
            // 특수 문양은 MachineModel.SpecialEffects() 참고.
            _hitRate = 1f;
            
            foreach (var (symbolID, power) in gains)
            {
                SymbolTemplate symbol = GameManager.Instance.SymbolTemplates[symbolID];
                switch (symbolID)
                {
                    case 0:
                        // 눈: 깨달음의 경지 상승
                        _upgradePoint += power;
                        break;
                    case 1:
                        // 클라우드: 회피
                        _hitRate *= (1f - power / 100f);
                        break;
                    case 2:
                        // 상승 기류: 공격
                        _enemyCurrentHealth -= power;
                        break;
                }
            }

            if (CheckRoundEnd()) return;
            
            // 적 공격 차례
            await ProcessEnemyAttack();

            if (CheckRoundEnd()) return;
            
            // 다시 Roll하러 이동
            GameManager.Instance.BackToChoice();
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

        private async UniTask ProcessEnemyAttack()
        {
            if (_enemyTemplate == null) return;
            if (_enemySkillIndex >= _enemyTemplate.SkillSequence.Count)
            {
                _enemySkillIndex = 0;
            }
            System.Random random = new System.Random();
            int damage = random.Next(_enemyTemplate.SkillSequence[_enemySkillIndex].MinDamage, _enemyTemplate.SkillSequence[_enemySkillIndex].MaxDamage + 1);

            // 공격 연출 재생하기
            await UniTask.Delay(TimeSpan.FromSeconds(1f));
            
            if (damage < 0)
            {
                // 음수 피해량은 적이 자기 자신에게 입히는 피해
                _enemyCurrentHealth -= damage;
            }
            else
            {
                _playerHealth -= damage;
            }
        }

        private async UniTask ProcessDeath()
        {
            // TODO
            Debug.Log("Death");
        }

        private bool CheckRoundEnd()
        {
            if (_enemyCurrentHealth <= 0)
            {
                // 적 처치
                LoadNextRound().Forget();
                return true;
            }
            else if (_playerHealth <= 0)
            {
                // 플레이어 사망
                ProcessDeath().Forget();
                return true;
            }

            return false;
        }
    }
}
