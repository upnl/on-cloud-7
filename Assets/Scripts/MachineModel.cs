using System.Collections.Generic;

public class MachineModel
{
    private int _id;
    private List<SymbolModel> _symbolPool = new List<SymbolModel>();
    public int ID => _id;
    public List<SymbolModel> SymbolPool => _symbolPool;

    public void Initialize()
    {
        //Init
    }
    
    public void Roll()
    {
        //Roll machine
    }

    public void CheckResult()
    {
        //Check Result
        
    }
    
}