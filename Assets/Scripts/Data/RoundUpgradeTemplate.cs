using System;
using Cysharp.Text;
using UnityEngine;


namespace OnCloud7
{
    [Serializable]
    public class RoundUpgradeTemplate : IDataTemplate
    {
        public enum UpgradeType { Change, Add, Remove, Upgrade, SoloMachine, Cooldown }
        
        [ReadOnly] public int ID;
        [ReadOnly] public UpgradeType Type;
        [ReadOnly] public string Name;
        [ReadOnly] public string Level;
        [ReadOnly] public string Description;
        
        public void Initialize()
        {
            Debug.Log(this.ToString());
        }

        public override string ToString()
        {
            using (Utf16ValueStringBuilder sb = ZString.CreateStringBuilder(true))
            {
                sb.AppendLine("[RoundUpgradeTemplate] ");
                sb.Append("ID: ");
                sb.AppendLine(ID);
                sb.Append("Name: ");
                sb.AppendLine(Name);
                sb.Append("Type: ");
                sb.AppendLine(Type);
                sb.Append("Level: ");
                sb.AppendLine(Level);
                sb.Append("Description: ");
                sb.AppendLine(Description);

                return sb.ToString();
            }
        }
    }
}