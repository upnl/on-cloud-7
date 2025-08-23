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
        [ReadOnly] public List<int> Skills;
        [ReadOnly] public string Description;

        private List<EnemySkillTemplate> skillSequence;
        
        public void Initialize()
        {
            throw new NotImplementedException();
        }

        public void Initialize(List<EnemySkillTemplate> skillTemplates)
        {
            skillSequence = new List<EnemySkillTemplate>();
            foreach (var skillID in Skills)
            {
                if (skillID >= 0 && skillID < skillTemplates.Count)
                {
                    skillSequence.Add(skillTemplates[skillID]);
                }
            }

            Debug.Log(this.ToString());
        }

        public override string ToString()
        {
            string str;
            using (Utf16ValueStringBuilder sb = ZString.CreateStringBuilder(true))
            {
                sb.Append("[EnemyTemplate] ");
                sb.Append("ID: ");
                sb.AppendLine(ID);
                sb.Append("Name: ");
                sb.AppendLine(Name);
                sb.Append("IsBoss: ");
                sb.AppendLine(IsBoss);
                sb.Append("Health: ");
                sb.AppendLine(Health);
                sb.Append("Skills: [");
                foreach (var skill in skillSequence)
                {
                    sb.Append(skill.Name);
                    sb.Append(", ");
                }

                str = sb.ToString();
            }

            return ZString.Concat(str.Trim().TrimEnd(','), "]\n");
        }
    }
}
