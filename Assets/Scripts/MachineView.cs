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
        public AddSymbolView addSymbolPrefab;
        public RemoveSymbolView removeSymbolPrefab;
        [SerializeField] private Button _chooseButton;

        public void Initialize(MachineModel machineModel)
        {
            CleanPool();
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
                    ChangeSymbolView csv = Instantiate(changeSymbolPrefab, transform);
                    csv.Initialize(curSymbol);
                    _symbols.Add(csv);
                }
                else if (curSymbol.Type == SymbolTemplate.SymbolType.Add)
                {
                    AddSymbolView asv = Instantiate(addSymbolPrefab, transform);
                    asv.Initialize(curSymbol);
                    _symbols.Add(asv);
                    
                }
                else if (curSymbol.Type == SymbolTemplate.SymbolType.Remove)
                {
                    RemoveSymbolView rsv = Instantiate(removeSymbolPrefab, transform);
                    rsv.Initialize(curSymbol);
                    _symbols.Add(rsv);
                }
                

            }
        }

        public void StartSpin()
        {
            GameManager.Instance.StartSpin(_machineModel.ID);
        }

        public void CleanPool()
        {
            for (int i = 0; i < _symbols.Count; i++)
            {
                if (_symbols[i] != null)
                    Destroy(_symbols[i].gameObject);
            }
            _symbols.Clear();
        }


        
    }
}
