using System;
using System.Collections.Generic;
using UnityEngine;


namespace OnCloud7
{
    [Serializable]
    public class EnemyTemplate : IDataTemplate
    {
        [ReadOnly] public int ID;
        [ReadOnly] public string Name;
        [ReadOnly] public bool IsBoss;
        [ReadOnly] public int Health;
        [ReadOnly] public int Attack;
        [ReadOnly] public int SkillID1;
        [ReadOnly] public int SkillID2;

        private List<EnemySkillTemplate> skills;
        
        public void Initialize()
        {
            
        }

        public void Initialize(List<EnemySkillTemplate> skillTemplates)
        {
            skills = new List<EnemySkillTemplate>();
            foreach (var skillTemplate in skillTemplates)
            {
                if (skillTemplate.ID == SkillID1 && SkillID1 != 0)
                {
                    skills.Add(skillTemplate);
                }

                if (skillTemplate.ID == SkillID2 && SkillID2 != 0)
                {
                    skills.Add(skillTemplate);
                }
            }
        }
    }
}
