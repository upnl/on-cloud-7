using System;
using System.Collections.Generic;
using Cysharp.Text;
using UnityEngine;


namespace OnCloud7
{
    [Serializable]
    public class SymbolTemplate : IDataTemplate
    {
        public enum SymbolType { Normal, Change, Add, Remove, Random, Rainbow, Pronoun }
        [ReadOnly] public int ID;
        [ReadOnly] public SymbolType Type;
        [ReadOnly] public bool IsImmutable;
        [ReadOnly] public List<int> Arg0;
        [ReadOnly] public List<int> Arg1;
        [ReadOnly] public List<int> Arg2;
        [ReadOnly] public List<int> Arg3;
        [ReadOnly] public string Name;
        [ReadOnly] public string Description;
        
        private int _argValue0,  _argValue1, _argValue2, _argValue3;
        
        public int ArgValue0 => _argValue0;
        public int ArgValue1 => _argValue1;
        public int ArgValue2 => _argValue2;
        public int ArgValue3 => _argValue3;

        public void Initialize()
        {
            Debug.Log(this.ToString());
        }

        public void CloneWithArgValues(SymbolTemplate original, int argValue0, int argValue1, int argValue2, int argValue3)
        {
            ID = original.ID;
            Type = original.Type;
            IsImmutable = original.IsImmutable;
            Name = original.Name;
            Description = original.Description;
            _argValue0 = argValue0;
            _argValue1 = argValue1;
            _argValue2 = argValue2;
            _argValue3 = argValue3;
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
                sb.Append("Type: ");
                sb.AppendLine(Type);
                sb.Append("IsImmutable: ");
                sb.AppendLine(IsImmutable);
                sb.Append("Description: ");
                sb.AppendLine(DescriptionWithSymbol.GetDescription(Description, ArgValue0, ArgValue1, ArgValue2, ArgValue3));

                return sb.ToString();
            }
        }
    }
}