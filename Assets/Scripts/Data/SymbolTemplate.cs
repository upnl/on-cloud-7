using System;
using Cysharp.Text;
using UnityEngine;


namespace OnCloud7
{
    [Serializable]
    public class SymbolTemplate : IDataTemplate
    {
        public enum SymbolType { Normal, Change, Add, Remove, Random, Pronoun }
        [ReadOnly] public int ID;
        [ReadOnly] public SymbolType Type;
        [ReadOnly] public bool IsImmutable;
        [ReadOnly] public int Arg0;
        [ReadOnly] public int Arg1;
        [ReadOnly] public int Arg2;
        [ReadOnly] public int Arg3;
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
                sb.Append("Type: ");
                sb.AppendLine(Type);
                sb.Append("IsImmutable: ");
                sb.AppendLine(IsImmutable);
                sb.Append("Description: ");
                sb.AppendLine(Description
                    .Replace("{0}", Arg0.ToString())
                    .Replace("{1}", Arg1.ToString())
                    .Replace("{2}", Arg2.ToString())
                    .Replace("{3}", Arg3.ToString()));

                return sb.ToString();
            }
        }
    }
}