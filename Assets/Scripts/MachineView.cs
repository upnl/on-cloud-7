using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace OnCloud7
{

    public class MachineView : MonoBehaviour
    {
        private List<SymbolView> _symbols = new List<SymbolView>();
        private MachineModel _machineModel;

        public SymbolView symbolPrefab;
        public ChangeSymbolView changeSymbolPrefab;
        [SerializeField] private Button _chooseButton;

        public void Initialize(MachineModel machineModel)
        {
            _machineModel = machineModel;
            for (int i = 0; i < machineModel.SymbolPool.Count; i++)
            {
                SymbolModel curSymbol = machineModel.SymbolPool[i];
                /*SymbolView sv = Instantiate(symbolPrefab, transform);
                sv.Initialize(machineModel.SymbolPool[i]);
                _symbols.Add(sv);*/
                if (curSymbol.Type == SymbolTemplate.SymbolType.Normal ||
                    curSymbol.Type == SymbolTemplate.SymbolType.Random ||
                    curSymbol.Type == SymbolTemplate.SymbolType.Rainbow)
                {
                    SymbolView sv = Instantiate(symbolPrefab, transform);
                    sv.Initialize(curSymbol);
                    _symbols.Add(sv);
                }
                else if (curSymbol.Type == SymbolTemplate.SymbolType.Change)
                {
                    ChangeSymbolView sv = Instantiate(changeSymbolPrefab, transform);
                    sv.Initialize(curSymbol);
                    _symbols.Add(sv);
                }

            }
        }

        public void StartSpin()
            {
                GameManager.Instance.StartSpin(_machineModel.ID);
            }


        
    }
}
