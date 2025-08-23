using System;
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
            
        }
    }
}