using System.Collections.Generic;
using OnCloud7;
using UnityEngine;

namespace OnCloud7
{
    public class SlotView : MonoBehaviour
    {
        private List<SymbolView> _symbols = new List<SymbolView>();
        private MachineModel _machineModel;
        public SymbolView symbolPrefab;
        public ChangeSymbolView changeSymbolPrefab;
        public AddSymbolView addSymbolPrefab;

        public void Initialize(MachineModel machineModel)
        {
            _machineModel = machineModel;
            for (int i = 0; i < 9; i++)
            {
                SymbolView sv = Instantiate(symbolPrefab, transform);
                sv.Initialize(machineModel.SymbolPool[i]);
                _symbols.Add(sv);
            }

        }

        public void SymbolsRender(List<SymbolModel> result)
        {
            for (int i = 0; i < _symbols.Count; i++)
            {
                Destroy(_symbols[i].gameObject);
            }
            _symbols.Clear();
            foreach (SymbolModel symbol in result)
            {
                if (symbol.Type == SymbolTemplate.SymbolType.Normal || symbol.Type == SymbolTemplate.SymbolType.Random || symbol.Type == SymbolTemplate.SymbolType.Rainbow)
                {
                    SymbolView sv = Instantiate(symbolPrefab, transform);
                    sv.Initialize(symbol);
                    _symbols.Add(sv);
                }
                else if (symbol.Type == SymbolTemplate.SymbolType.Change)
                {
                    ChangeSymbolView csv = Instantiate(changeSymbolPrefab, transform);
                    csv.Initialize(symbol);
                    _symbols.Add(csv);
                }
                else if (symbol.Type == SymbolTemplate.SymbolType.Add)
                {
                    AddSymbolView asv = Instantiate(addSymbolPrefab, transform);
                    asv.Initialize(symbol);
                    _symbols.Add(asv);
                }
            }
        }

    }
}
