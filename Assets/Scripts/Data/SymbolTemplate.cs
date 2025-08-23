using System;
using Cysharp.Text;
using UnityEngine;


namespace OnCloud7
{
    [Serializable]
    public class SymbolTemplate : IDataTemplate
    {
        [ReadOnly] public int ID;
        [ReadOnly] public bool IsNormal;
        [ReadOnly] public int Level;
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
                sb.Append("[SymbolTemplate] ");
                sb.Append("ID: ");
                sb.AppendLine(ID);
                sb.Append("Name: ");
                sb.AppendLine(Name);
                sb.Append("IsNormal");
                sb.AppendLine(IsNormal);
                sb.Append("Level: ");
                sb.AppendLine(Level);
                sb.Append("Description: ");
                sb.AppendLine(Description);

                return sb.ToString();
            }
        }
    }
}