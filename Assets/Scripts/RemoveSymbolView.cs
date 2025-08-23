using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OnCloud7
{

    public class RemoveSymbolView : SymbolView
    {
        [SerializeField] protected Image _removeSymbol;
        protected int _removeSymbolID;
        protected int _removeNum;
        [SerializeField] protected List<Sprite> _sprites;

        public override void Initialize(SymbolModel symbolModel)
        {
            Dictionary<int, int> _IDmap = new Dictionary<int, int>()
            {
                { 0, 0 }, { 1, 1 }, { 2, 2 },
                { 777, 3 }, { 7777, 4 }
            };
            _symbolModel = symbolModel;
            _removeSymbolID = symbolModel.Arg1;
            _removeSymbol.sprite = _sprites[_IDmap[_removeSymbolID]];
        }

    }
}


