using UnityEngine;
using Random = System.Random;

public static class Util
{
    static int SymbolID(int symbolID)
    {
        if (symbolID == 777)
        {
            Random r = new Random(System.DateTime.Now.Millisecond);
            return r.Next(3);
        }
        else if (symbolID == 7777)
        {
            Random r = new Random(System.DateTime.Now.Millisecond);
            return r.Next(6, 68);
        }
        return symbolID;
    }

}
