using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OnCloud7
{

    public class ChangeSymbolView : SymbolView
    {
        [SerializeField] private GameObject _beforeSymbol;
        [SerializeField] private GameObject _afterSymbol;
        public override void Initialize(SymbolModel symbolModel)
        {
            _symbolModel = symbolModel;
            _symbolIcon.sprite = _symbolSprites[symbolModel.ID];
        }

    }
}


