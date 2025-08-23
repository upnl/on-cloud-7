using System;
using System.Collections.Generic;
using Cysharp.Text;
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
        [ReadOnly] public int SkillID1;
        [ReadOnly] public int SkillID2;

        private List<EnemySkillTemplate> skills;
        
        public void Initialize()
        {
            Debug.Log(this.ToString());
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
            Debug.Log(this.ToString());
        }

        public override string ToString()
        {
            using (Utf16ValueStringBuilder sb = ZString.CreateStringBuilder(true))
            {
                sb.AppendLine("[EnemyTemplate] ");
                sb.Append("ID: ");
                sb.AppendLine(ID);
                sb.Append("Name: ");
                sb.AppendLine(Name);
                sb.Append("IsBoss: ");
                sb.AppendLine(IsBoss);
                sb.Append("Health: ");
                sb.AppendLine(Health);
                int skillIndex = 0;
                if (SkillID1 != 0 && skillIndex < skills.Count)
                {
                    sb.Append("Skill1: ");
                    sb.AppendLine(skills[skillIndex].Name);
                    skillIndex++;
                }
                if (SkillID2 != 0 && skillIndex < skills.Count)
                {
                    sb.Append("Skill2: ");
                    sb.AppendLine(skills[skillIndex].Name);
                    skillIndex++;
                }

                return sb.ToString();
            }
        }
    }
}
