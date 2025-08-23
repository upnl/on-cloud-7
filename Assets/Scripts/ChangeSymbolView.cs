using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OnCloud7
{

    public class ChangeSymbolView : SymbolView
    {
        [SerializeField] protected Image _beforeSymbol;
        [SerializeField] protected Image _afterSymbol;
        protected int _beforeSymbolID;
        protected int _afterSymbolID;
        [SerializeField] protected List<Sprite> _sprites;

        public override void Initialize(SymbolModel symbolModel)
        {
            Dictionary<int, int> _IDmap = new Dictionary<int, int>()
            {
                { 0, 0 }, { 1, 1 }, { 2, 2 },
                { 777, 3 }, { 7777, 4 }
            };
            _symbolModel = symbolModel;
            _beforeSymbolID = symbolModel.Arg1;
            _afterSymbolID = symbolModel.Arg3;
            Debug.Log(_beforeSymbolID + "," + _afterSymbolID);
            _beforeSymbol.sprite = _sprites[_IDmap[_beforeSymbolID]];
            _afterSymbol.sprite = _sprites[_IDmap[_afterSymbolID]];
        }

    }
}


