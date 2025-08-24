using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OnCloud7
{
    public class UpgradeView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private TextMeshProUGUI _descriptionText;
        [SerializeField] private Button _button;

        private RoundUpgradeTemplate _upgradeTemplate;
        private int _level;

        public void Initialize(RoundUpgradeTemplate template)
        {
            _upgradeTemplate = template;
            _nameText.SetText(_upgradeTemplate.Name);
            _descriptionText.SetText(DescriptionWithSymbol.GetDescription(_upgradeTemplate.Description,
                _upgradeTemplate.ArgValue0, _upgradeTemplate.ArgValue1, _upgradeTemplate.ArgValue2,
                _upgradeTemplate.ArgValue3, _upgradeTemplate.ArgValue4));
            GetComponent<Image>().color = GetLevelColor(_upgradeTemplate.Level);
            _button.onClick.RemoveAllListeners();
            _button.onClick.AddListener(_upgradeTemplate.InvokeUpgrade);
            _button.onClick.AddListener(() => GameManager.Instance.UpgradeCompleted = true);
        }

        private static Color GetLevelColor(int level)
        {
            switch (level)
            {
                case 0:
                    return new Color(0.98f, 0.98f, 0.98f);
                case 1:
                    return new Color(0.45f, 0.79f, 0.98f);
                case 2:
                    return new Color(0.89f, 0.48f, 0.98f);
                default:
                    return new Color(0.98f, 0.68f, 0.23f);
            }
        }
    }
}
