

using System;

namespace OnCloud7
{
    public class SymbolModel : IComparable<SymbolModel>
    {
        private int _id;
        public int ID => _id;
        private bool _isNormal;
        private int _level;
        public int Level => _level;
        private string _name;
        private string _description;

        public SymbolModel(int id, bool isNormal, int level, string name, string description)
        {
            _id = id;
            _isNormal = isNormal;
            _level = level;
            _name = name;
            _description = description;
        }

        public SymbolModel(SymbolTemplate symbolTemplate)
        {
            _id = symbolTemplate.ID;
            _isNormal = symbolTemplate.IsNormal;
            _level = symbolTemplate.Level;
            _name = symbolTemplate.Name;
            _description = symbolTemplate.Description;
        }


        public int CompareTo(SymbolModel other)
        {
            if (this.ID < other.ID)
            {
                return -1;
            }
            else if (this.ID > other.ID)
            {
                return 1;
            }
            else if (this.Level < other.Level)
            {
                return 1;
            }
            else if (this.Level > other.Level)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }
    }
}
