using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OnCloud7
{

    public class SymbolView : MonoBehaviour
    {
        [SerializeField] private Image _symbolIcon;
        private SymbolModel _symbolModel;
        [SerializeField] private List<Sprite> _symbolSprites;

        public void Initialize(SymbolModel symbolModel)
        {
            _symbolModel = symbolModel;
            _symbolIcon.sprite = _symbolSprites[symbolModel.ID];
        }

    }
}


