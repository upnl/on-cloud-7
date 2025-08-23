using System;
using UnityEngine;

namespace OnCloud7
{
    [Serializable]
    public class EnemySkillTemplate : IDataTemplate
    {
        [ReadOnly] public int ID;
        [ReadOnly] public int Cooltime;
        [ReadOnly] public int MinDamage;
        [ReadOnly] public int MaxDamage;
        [ReadOnly] public string Name;
        [ReadOnly] public string Description;
        
        public void Initialize()
        {
            
        }
    }
}