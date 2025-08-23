using System.Collections.Generic;
using System.Linq;
using Cysharp.Text;

namespace OnCloud7
{
    public static class DescriptionWithSymbol
    {
        private static readonly Dictionary<int, string> symbolIDToName;

        static DescriptionWithSymbol()
        {
            symbolIDToName = new Dictionary<int, string>();
            symbolIDToName.Add(-1, string.Empty);
            symbolIDToName.Add(0, "눈");
            symbolIDToName.Add(1, "클라우드");
            symbolIDToName.Add(2, "상승 기류");
            symbolIDToName.Add(777, "임의의 기본");
            symbolIDToName.Add(7777, "임의의 특수");
        }
        
        public static string GetDescription(string description, int arg0 = -1, int arg1 = -1, int arg2 = -1, int arg3 = -1, int arg4 = -1)
        {
            return description
                .Replace("{0}", arg0.ToString())
                .Replace("{1}", arg1.ToString())
                .Replace("{2}", arg2.ToString())
                .Replace("{3}", arg3.ToString())
                .Replace("{4}", arg4.ToString())
                .Replace("{0s}", SymbolIDToName(arg0))
                .Replace("{1s}", SymbolIDToName(arg1))
                .Replace("{2s}", SymbolIDToName(arg2))
                .Replace("{3s}", SymbolIDToName(arg3))
                .Replace("{4s}", SymbolIDToName(arg4));
        }

        private static string SymbolIDToName(int symbolID)
        {
            if (!GameManager.Instance || GameManager.Instance.SymbolTemplates == null)
            {
                return symbolIDToName.GetValueOrDefault(symbolID, ZString.Concat(symbolID, "?"));
            }

            foreach (var symbol in GameManager.Instance.SymbolTemplates)
            {
                if (symbol.ID == symbolID)
                {
                    return symbol.Name;
                }
            }
            
            return symbolIDToName.GetValueOrDefault(symbolID, ZString.Concat(symbolID, "?"));
        }
    }
}
