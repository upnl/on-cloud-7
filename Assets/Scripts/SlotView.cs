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
            foreach (SymbolModel symbol in result)
            {
                SymbolView sv = Instantiate(symbolPrefab, transform);
                sv.Initialize(symbol);
                _symbols.Add(sv);
            }
        }

    }
}
