using System;
using System.Collections.Generic;
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
        [ReadOnly] public int Level;
        [ReadOnly] public List<int> Arg0;
        [ReadOnly] public List<int> Arg1;
        [ReadOnly] public List<int> Arg2;
        [ReadOnly] public List<int> Arg3;
        [ReadOnly] public List<int> Arg4;
        [ReadOnly] public string Description;
        
        private int _argValue0,  _argValue1, _argValue2, _argValue3, _argValue4;
        
        public int ArgValue0 => _argValue0;
        public int ArgValue1 => _argValue1;
        public int ArgValue2 => _argValue2;
        public int ArgValue3 => _argValue3;
        public int ArgValue4 => _argValue4;
        
        public void Initialize()
        {
            Debug.Log(this.ToString());
        }

        public void CloneWithArgValues(RoundUpgradeTemplate original, int argValue0, int argValue1, int argValue2, int argValue3, int argValue4)
        {
            ID = original.ID;
            Type = original.Type;
            Name = original.Name;
            Level = original.Level;
            Description = original.Description;
            _argValue0 = argValue0;
            _argValue1 = argValue1;
            _argValue2 = argValue2;
            _argValue3 = argValue3;
            _argValue4 = argValue4;
        }

        public override string ToString()
        {
            using (Utf16ValueStringBuilder sb = ZString.CreateStringBuilder(false))
            {
                sb.Append("[RoundUpgradeTemplate] ");
                sb.Append("ID: ");
                sb.AppendLine(ID);
                sb.Append("Name: ");
                sb.AppendLine(Name);
                sb.Append("Type: ");
                sb.AppendLine(Type);
                sb.Append("Level: ");
                sb.AppendLine(Level);
                sb.Append("Description: ");
                sb.AppendLine(DescriptionWithSymbol.GetDescription(Description, ArgValue0, ArgValue1, ArgValue2, ArgValue3, ArgValue4));

                return sb.ToString();
            }
        }
    }
}