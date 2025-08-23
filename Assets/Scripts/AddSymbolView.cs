using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OnCloud7
{

    public class AddSymbolView : SymbolView
    {
        [SerializeField] protected Image _addSymbol;
        protected int _addSymbolID;
        protected int _addNum;
        [SerializeField] protected List<Sprite> _sprites;

        public override void Initialize(SymbolModel symbolModel)
        {
            Dictionary<int, int> _IDmap = new Dictionary<int, int>()
            {
                { 0, 0 }, { 1, 1 }, { 2, 2 },
                { 777, 3 }, { 7777, 4 }
            };
            _symbolModel = symbolModel;
            _addSymbolID = symbolModel.Arg1;
            _addSymbol.sprite = _sprites[_IDmap[_addSymbolID]];
        }

    }
}


