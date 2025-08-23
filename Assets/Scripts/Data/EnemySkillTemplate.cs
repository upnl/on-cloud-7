using System;
using Cysharp.Text;
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
            Debug.Log(this.ToString());
        }

        public override string ToString()
        {
            using (Utf16ValueStringBuilder sb = ZString.CreateStringBuilder(true))
            {
                sb.Append("[EnemySkillTemplate] ");
                sb.Append("ID: ");
                sb.AppendLine(ID);
                sb.Append("Name: ");
                sb.AppendLine(Name);
                sb.Append("Cooltime: ");
                sb.AppendLine(Cooltime);
                sb.Append("Damage: [");
                sb.Append(MinDamage);
                sb.Append(", ");
                sb.Append(MaxDamage);
                sb.AppendLine("]");
                sb.Append("Description: ");
                sb.AppendLine(Description);

                return sb.ToString();
            }
        }
    }
}