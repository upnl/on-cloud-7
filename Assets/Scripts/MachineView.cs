using System.Collections.Generic;
using UnityEngine;

namespace OnCloud7
{

    public class MachineView : MonoBehaviour
    {
        private List<SymbolView> _symbols = new List<SymbolView>();
        private MachineModel _machineModel;
        
        public SymbolView symbolPrefab;

        public void Initialize(MachineModel machineModel)
        {
            _machineModel = machineModel;
            for (int i = 0; i < machineModel.SymbolPool.Count; i++)
            {
                SymbolView sv = Instantiate(symbolPrefab, transform);
                sv.Initialize(machineModel.SymbolPool[i]);
                _symbols.Add(sv);
            }

        }

    }
}
