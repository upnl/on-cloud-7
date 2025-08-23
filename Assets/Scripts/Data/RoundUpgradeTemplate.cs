using System;
using UnityEngine;


namespace OnCloud7
{
    [Serializable]
    public class RoundUpgradeTemplate : IDataTemplate
    {
        public enum UpgradeType { Change, Add, Remove, Upgrade, SoloMachine, Cooldown }
        
        [ReadOnly] public int ID;
        [ReadOnly] public string Type;
        [ReadOnly] public string Name;
        [ReadOnly] public string Level;
        [ReadOnly] public string Description;
        
        public void Initialize()
        {
            
        }
    }
}