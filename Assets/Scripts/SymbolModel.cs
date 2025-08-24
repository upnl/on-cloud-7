

using System;

namespace OnCloud7
{
    public class SymbolModel : IComparable<SymbolModel>
    {
        private int _id;
        private SymbolTemplate.SymbolType _type;
        private bool _isImmutable;
        private int _arg0, _arg1, _arg2, _arg3;
        private string _name;
        private string _description;
        
        public int ID => _id;
        public SymbolTemplate.SymbolType Type => _type;
        public int Arg0 => _arg0;
        public int Arg1 => _arg1;
        public int Arg2 => _arg2;
        public int Arg3 => _arg3;
        public bool IsImmutable => _isImmutable;
        

        public SymbolModel(int id, SymbolTemplate.SymbolType type, bool isImmutable, int arg0, int arg1, int arg2, int arg3, string name, string description)
        {
            _id = id;
            _type = type;
            _isImmutable = isImmutable;
            _arg0 = arg0;
            _arg1 = arg1;
            _arg2 = arg2;
            _arg3 = arg3;
            _name = name;
            _description = description;
        }

        public SymbolModel(SymbolTemplate symbolTemplate)
        {
            _id = symbolTemplate.ID;
            _type = symbolTemplate.Type;
            _isImmutable = symbolTemplate.IsImmutable;
            _arg0 = symbolTemplate.ArgValue0;
            _arg1 = symbolTemplate.ArgValue1;
            _arg2 = symbolTemplate.ArgValue2;
            _arg3 = symbolTemplate.ArgValue3;
            _name = symbolTemplate.Name;
            _description = symbolTemplate.Description;
        }


        public int CompareTo(SymbolModel other)
        {
            int a = this.ID;
            int b = other.ID;
            if (a is >= 3 and < 6) a -= 3;
            if (b is >= 3 and < 6) b -= 3;
            if (a < b)
            {
                return -1;
            }
            else if (a > b)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }
    }
}
