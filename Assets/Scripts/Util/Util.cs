using UnityEngine;
using Random = System.Random;

public static class Util
{
    public static int SymbolIDToIndex(int symbolID)
    {
        Random r = new Random(System.DateTime.Now.Millisecond);
        switch (symbolID)
        {
            case 8:
                // Change
                return r.Next(8, 48);
            case 9:
                // Add
                return r.Next(48, 68);
            case 10:
                // Remove
                return r.Next(68, 78);
            case 777:
            {
                return r.Next(3);
            }
            case 7777:
            {
                switch (r.Next(4))
                {
                    case 0:
                        // Rainbow 1개
                        return 7;
                    case 1:
                        // Change 40개
                        return r.Next(8, 48);
                    case 2:
                        // Add 20개
                        return r.Next(48, 68);
                    default:
                        // Remove 10개
                        return r.Next(68, 78);
                }
            }
            default:
                return symbolID;
        }
    }

}
