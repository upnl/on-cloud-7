using System.Collections.Generic;
using TMPro;
using System;
using Cysharp.Text;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

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

        public int PlayerHealth
        {
            get => _playerHealth;
            set
            {
                _playerHealth = value;
                _playerHPText.SetTextFormat("HP: {0} / 100", _playerHealth);
            }
        }

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

        public int EnemyCurrentHealth
        {
            get => _enemyCurrentHealth;
            set
            {
                _enemyCurrentHealth = value;
                _enemyHPText.SetTextFormat("HP: {0} / {1}", _enemyCurrentHealth, _enemyTemplate?.Health);
            }
        }

        /// <summary>
        /// 적의 다음 스킬 순서
        /// </summary>
        private int _enemySkillIndex;
        
        /*
         * BattleManager TODO:
         * 1. BattleManager.Initialize() 호출 -> Complete
         * 2. 깨달음 선택 후 LoadNextRound() 호출하며 다음 라운드 준비 (UI도 변경)
         * 3. 현재 남은 체력 등의 정보들을 UI에 표시
         * 4. Roll()의 결과로 나온 CheckResult()의 gains들을 BattleManager에 넘겨서 수치 변동에 반영할 수 있도록 -> Complete
         * 5. 라운드 끝날 때 UpgradeLevel()로 업그레이드 레벨 확인 후 RoundUpgradeTemplate에서 불러와서 증강체 3개 선택지 띄우기
         * 6. 적이 스킬을 돌아가면서 쓰도록 하기 -> Complete
         * 7. 적 특수 패턴 (예: 군대의 전역 등) -> Complete
         */

        [SerializeField] private TextMeshProUGUI _playerHPText;
        [SerializeField] private TextMeshProUGUI _enemyHPText;
        [SerializeField] private TextMeshProUGUI _enemyNameText;
        [SerializeField] private TextMeshProUGUI _enemyDescriptionText;
        [SerializeField] private TextMeshProUGUI _statusText;
        private int _playerAttack;
        private int _enemyAttack;

        public void Initialize()
        {
            _gameStarted = true;
            _round = 0;
            PlayerHealth = 100;
            LoadNextRound().Forget();
        }

        public async UniTask LoadNextRound()
        {
            _enemyTemplate = GameManager.Instance.EnemyTemplates[_round];
            if (_round > 0)
            {
                await GameManager.Instance.RoundUpgrade(_round, UpgradeLevel(_upgradePoint));
            }
            else
            {
                await GameManager.Instance.RoundUpgrade(0, 1);
            }
            _round++;
            EnemyCurrentHealth = _enemyTemplate.Health;
            _enemyDescriptionText.SetText(_enemyTemplate.Description);
            _enemyNameText.SetTextFormat("{0}. {1}", _round, _enemyTemplate.Name);
            _upgradePoint = 0;
            _enemySkillIndex = 0;
            _statusText.SetText("전투를 시작합니다!");
            GameManager.Instance.BackToChoice();
        }

        public async UniTask ProcessRollResult(Dictionary<int, int> gains)
        {
            // 기본 문양만 여기서 처리합니다.
            // 특수 문양은 MachineModel.SpecialEffects() 참고.
            _hitRate = 1f;

            int upgradeGain = 0;
            int damage = 0;
            foreach (var (symbolID, power) in gains)
            {
                SymbolTemplate symbol = GameManager.Instance.SymbolTemplates[symbolID];
                switch (symbolID)
                {
                    case 0:
                        // 눈: 깨달음의 경지 상승
                        _upgradePoint += power;
                        upgradeGain = power;
                        break;
                    case 1:
                        // 클라우드: 회피
                        _hitRate = Mathf.Pow(0.9f, power);
                        break;
                    case 2:
                        // 상승 기류: 공격
                        EnemyCurrentHealth -= power;
                        damage = power;
                        break;
                }
            }
            _statusText.SetTextFormat("회피율: {0}%\n공격: {1}\n깨달음: +{2}", (1f - _hitRate) * 100f, damage, upgradeGain);

            await UniTask.Delay(TimeSpan.FromSeconds(1.5f));

            if (CheckRoundEnd()) return;
            
            // 적 공격 차례
            await ProcessEnemyAttack(_hitRate);
            
            await UniTask.Delay(TimeSpan.FromSeconds(1f));

            if (CheckRoundEnd()) return;
            
            // 다시 Roll하러 이동
            //GameManager.Instance.BackToChoice();
            GameManager.Instance.SetBackButtonInteractable(true);
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
/*
        public void StateUpdate()
        {
            _playerHPText.text = _playerHealth.ToString();
            _enemyHPText.text = _enemyCurrentHealth.ToString();
        }

        public void PlayerAction(Dictionary<int, int> gains)
        {
            for (int i = 0; i < 3; i++)
            {
                if (gains.ContainsKey(i))
                {
                    if (i == 0)
                    {
                        _upgradePoint += gains[i];
                        Debug.Log(_upgradePoint);
                    }
                    else if (i == 1)
                    {
                        for (int j = 0; j < gains[i]; j++)
                        {
                            if (_avoidability > 0)
                            {
                                _avoidability *= 0.9f;
                                
                            }
                            else
                            {
                                _avoidability = 0.9f * (1 - _avoidability);
                            }

                            Debug.Log(_avoidability);
                        }
                    }
                    else if (i == 2)
                    {
                        _playerAttack = gains[i];
                        Debug.Log(_playerAttack);
                        
                    }
                }
            }


        }
        */
        private async UniTask ProcessEnemyAttack(float hitRate)
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
                EnemyCurrentHealth -= damage;
                _statusText.SetTextFormat("적이 {0}의 피해를 스스로 입었다!", damage);
            }
            else if (random.NextDouble() <= hitRate)
            {
                PlayerHealth -= damage;
                Debug.Log(ZString.Format("아야! {0}의 피해를 입었다!", damage));
                _statusText.SetTextFormat("아야! {0}의 피해를 입었다!", damage);
            }
            else
            {
                Debug.Log(ZString.Format("{0}%의 확률로 피했다!", (1f - hitRate) * 100f));
                _statusText.SetTextFormat("{0}%의 확률로 피했다!", (1f - hitRate) * 100f);
            }
        }

        private async UniTask ProcessDeath()
        {
            // TODO
            Debug.Log("Death");
            _statusText.SetText("으으윽... 내가 쓰러지다니.\n(게임을 껐다 켜시기 바랍니다.)");
        }

        private bool CheckRoundEnd()
        {
            if (EnemyCurrentHealth <= 0)
            {
                // 적 처치
                LoadNextRound().Forget();
                return true;
            }
            else if (PlayerHealth <= 0)
            {
                // 플레이어 사망
                ProcessDeath().Forget();
                return true;
            }

            return false;
        }
    }
}
