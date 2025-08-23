

using System;

namespace OnCloud7
{
    public class SymbolModel : IComparable<SymbolModel>
    {
        private int _id;
        public int ID => _id;
        private SymbolTemplate.SymbolType _type;
        private int _level;
        public int Level => _level;
        private string _name;
        private string _description;

        public SymbolModel(int id, SymbolTemplate.SymbolType type, int level, string name, string description)
        {
            _id = id;
            _type = type;
            _level = level;
            _name = name;
            _description = description;
        }

        public SymbolModel(SymbolTemplate symbolTemplate)
        {
            _id = symbolTemplate.ID;
            _type = symbolTemplate.Type;
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
