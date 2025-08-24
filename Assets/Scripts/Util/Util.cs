using UnityEngine;
using Random = System.Random;

public static class Util
{
    private static readonly Random _random = new Random(System.DateTime.Now.Millisecond);
    
    public static Random Random => _random;
    
    public static int SymbolIDToIndex(int symbolID)
    {
        switch (symbolID)
        {
            case 8:
                // Change
                return Util.Random.Next(8, 52);
            case 9:
                // Add
                return Util.Random.Next(52, 72);
            case 10:
                // Remove
                return Util.Random.Next(72, 82);
            case 777:
            {
                return Util.Random.Next(3);
            }
            case 7777:
            {
                switch (Util.Random.Next(4))
                {
                    case 0:
                        // Rainbow 1개
                        return 7;
                    case 1:
                        // Change 40개
                        return Util.Random.Next(8, 52);
                    case 2:
                        // Add 20개
                        return Util.Random.Next(52, 72);
                    default:
                        // Remove 10개
                        return Util.Random.Next(72, 82);
                }
            }
            default:
                return symbolID;
        }
    }

}
