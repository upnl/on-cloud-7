using System;
using Cysharp.Text;
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
            Debug.Log(this.ToString());
        }

        public override string ToString()
        {
            using (Utf16ValueStringBuilder sb = ZString.CreateStringBuilder(true))
            {
                sb.AppendLine("[RewardTemplate] ");
                sb.Append("ID: ");
                sb.AppendLine(ID);
                sb.Append("Name: ");
                sb.AppendLine(Name);
                sb.Append("Type: ");
                sb.AppendLine(Type);
                sb.Append("Description: ");
                sb.AppendLine(Description);

                return sb.ToString();
            }
        }
    }
}
