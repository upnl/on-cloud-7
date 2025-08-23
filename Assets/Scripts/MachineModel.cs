using System.Collections.Generic;
using System.Linq;
using OnCloud7;
using UnityEditor;
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
            for (int j = 0; j < 6; j++)
            {
                _symbolPool.Add(new SymbolModel(GameManager.Instance.SymbolTemplates[i]));
            }

            for (int j = 0; j < 3; j++)
            {
                _symbolPool.Add(new SymbolModel(GameManager.Instance.SymbolTemplates[i + 3]));
            }
        }
        
        //temp change symbols
        for (int i = 0; i < 3; i++)
        {
            _symbolPool.Add(new SymbolModel(GameManager.Instance.SymbolTemplates[i + 10]));
        }
        
        //temp add symbols
        for (int i = 0; i < 3; i++)
        {
            _symbolPool.Add(new SymbolModel(GameManager.Instance.SymbolTemplates[58 + i]));
        }
        //temp remove symbols
        for (int i = 0; i < 3; i++)
        {
            _symbolPool.Add(new SymbolModel(GameManager.Instance.SymbolTemplates[78 + i]));
        }        
        _symbolPool.Add(new SymbolModel(GameManager.Instance.SymbolTemplates[6]));
        _result = new List<SymbolModel>();
        

        
    }

    public void ChangeSymbol(int beforeIndex, int afterIndex)
    {
        _symbolPool[beforeIndex] = new SymbolModel(GameManager.Instance.SymbolTemplates[afterIndex]);
    }

    public void AddSymbol(int addIndex)
    {
        _symbolPool.Add(new SymbolModel(GameManager.Instance.SymbolTemplates[addIndex]));
    }

    public void RemoveSymbol(int removeIndex)
    {
        _symbolPool.RemoveAt(removeIndex);
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
        SpecialEffects(_result);
        _symbolPool.AddRange(_result);
        _symbolPool.Sort();
        
    }

    public void CheckResult(List<SymbolModel> result)
    {
        Dictionary<int, int> gains = new Dictionary<int, int>();
        foreach (SymbolModel symbol in result)
        {
            if (!gains.ContainsKey(symbol.ID))
            {
                if (symbol.ID == 7)
                {
                    for (int i = 0; i < 3; i++)
                    {
                        if (gains.ContainsKey(i))
                        {
                            gains[i] += 1;
                        }
                        else
                        {
                            gains.Add(i, 1);
                        }
                    }

                    continue;
                }
                if (symbol.ID >= 3 && symbol.ID <= 5)
                {
                    if (!gains.ContainsKey(symbol.ID - 3))
                    {
                        gains.Add(symbol.ID - 3, 1);
                    }
                    else
                    {
                        gains[symbol.ID - 3] += 1;
                    }
                }
                else
                {
                    gains.Add(symbol.ID, 1);
                }
            }
            else
            {
                gains[symbol.ID]++;
            }
        }
        int[] Bingos = new int[] { 1, 1, 1 };
        int[][] BingoLines = new[]
        {
            new[] { 0, 1, 2 }, new[] { 3, 4, 5 }, new[] { 6, 7, 8 },
            new[] { 0, 3, 6 }, new[] { 1, 4, 7 }, new[] { 2, 5, 8 },
            new[] { 0, 4, 8 }, new[] { 2, 4, 6 },
            new[] { 0, 1, 3, 4 }, new[] { 1, 2, 4, 5 }, new[] { 3, 4, 6, 7 }, new[] { 4, 5, 7, 8 }
        };
        foreach (int[] line in BingoLines)
        {
            int CheckResult = CheckBingo(result, line);
            if (CheckResult == 7)
            {
                Bingos[0]++;
                Bingos[1]++;
                Bingos[2]++;
            }
            else if (CheckResult >= 0)
            {
                Bingos[CheckResult]++;
            }

        }

        for (int i = 0; i < 3; i++)
        {
            if (gains.ContainsKey(i))
            {
                gains[i] *= Bingos[i];
            }
            
        }
        Debug.Log(string.Join(",", gains));
        //Debug.Log(string.Join(",", Bingos));
        //Send gain values
    }
        
    

    private int CheckBingo(List<SymbolModel> result, int[] line)
    {
        int curSymbol = -1;
        foreach (int index in line)
        {
            if (curSymbol == -1)
            {
                curSymbol = result[index].ID;
                if (curSymbol == 7)
                {
                    curSymbol = 7; //Do Nothing
                }
                else if (curSymbol >= 6)
                {
                    return -1;
                }
                else if (curSymbol >= 3)
                {
                    curSymbol -= 3;
                }
            }
            
            else if (curSymbol != result[index].ID)
            {
                if (result[index].ID == 7)
                {
                    continue;
                }
                if (curSymbol == 7)
                {
                    if (result[index].ID >= 3 && result[index].ID <= 5)
                    {
                        curSymbol = result[index].ID - 3;
                        continue;
                    }
                    else if (result[index].ID <= 2)
                    {
                        curSymbol = result[index].ID;
                        continue;
                    }
                }
                else if (result[index].ID >= 3 && result[index].ID <= 5 && result[index].ID - 3 == curSymbol)
                {
                    continue;
                }
                return -1;
            }
        }
        return curSymbol;
    }

    private void SpecialEffects(List<SymbolModel> result)
    {
        foreach (SymbolModel symbol in result)
        {
            if (symbol.Type == SymbolTemplate.SymbolType.Change)
            {
                Debug.Log("Change");
                GameManager.Instance.ChangeRequest(symbol, symbol.Arg0);
            }
            else if (symbol.Type == SymbolTemplate.SymbolType.Add)
            {
                Debug.Log("Add");
                GameManager.Instance.AddRequest(symbol, symbol.Arg0);
            }
            else if (symbol.Type == SymbolTemplate.SymbolType.Remove)
            {
                Debug.Log("Remove");
                GameManager.Instance.RemoveRequest(symbol, symbol.Arg0);
            }
            else if (symbol.Type == SymbolTemplate.SymbolType.Random)
            {
                //pass
            }
        }
    }




}
