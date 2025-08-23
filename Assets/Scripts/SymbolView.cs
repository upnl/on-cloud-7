using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OnCloud7
{

    public class SymbolView : MonoBehaviour
    {
        [SerializeField] protected Image _symbolIcon;
        protected SymbolModel _symbolModel;
        [SerializeField] protected List<Sprite> _symbolSprites;

        public virtual void Initialize(SymbolModel symbolModel)
        {
            _symbolModel = symbolModel;
            _symbolIcon.sprite = _symbolSprites[symbolModel.ID];
        }

    }
}


