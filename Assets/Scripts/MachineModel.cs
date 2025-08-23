using System.Collections.Generic;
using System.Linq;
using OnCloud7;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;


public class MachineModel
{
    private int _id;
    private List<SymbolModel> _symbolPool = new List<SymbolModel>();
    public int ID => _id;
    public List<SymbolModel> SymbolPool => _symbolPool;
    private List<SymbolModel> _result = new List<SymbolModel>();
    public List<SymbolModel> Result => _result;
    public SymbolView symbolPrefab;

    private SlotView _slotView;

    

    public void Initialize()
    {
        _symbolPool = new List<SymbolModel>();
        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                _symbolPool.Add(new SymbolModel(GameManager.Instance.SymbolTemplates[i]));
            }
        }
        _result = new List<SymbolModel>();
    }
    
    public void Roll()
    {
        _result.Clear();
        Random r = new Random(System.DateTime.Now.Millisecond);
        for (int i = 0; i < 9; i++)
        {
            int index = r.Next(_symbolPool.Count - 1);
            _result.Add(_symbolPool[index]);
            _symbolPool.RemoveAt(index);
        }
        GameManager.Instance.SymbolsRender(_result);
        CheckResult(_result);
        _symbolPool.AddRange(_result);
        _symbolPool.Sort();
    }

    public void CheckResult(List<SymbolModel> result)
    {
        int Atkgain = 0;
        int MPgain = 0;
        int Avoidgain = 0;
        //Check Bingos (3 rows, 3 columns, 2 diagonals)
        //Send gain values
    }




}
