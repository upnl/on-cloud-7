using System.Collections.Generic;
using UnityEngine;

namespace OnCloud7
{

    public class MachineView : MonoBehaviour
    {
        private List<SymbolModel> _symbols = new List<SymbolModel>();
        private MachineModel _machineModel;

        public void Initialize(MachineModel machineModel)
        {
            _machineModel = machineModel;
        }

    }
}
