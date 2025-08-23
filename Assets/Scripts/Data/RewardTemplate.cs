using System;
using UnityEngine;

namespace OnCloud7
{
    [Serializable]
    public class RewardTemplate : IDataTemplate
    {
        public enum RewardType { Enchant, Special, Change, Add, StaticRemove, RandomRemove }
        
        [ReadOnly] public int ID;
        [ReadOnly] public RewardType Type;
        [ReadOnly] public string Name;
        [ReadOnly] public string Description;
        
        public void Initialize()
        {
            
        }
    }
}
