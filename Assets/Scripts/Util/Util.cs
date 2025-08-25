using System;
using System.Collections.Generic;
using System.Linq;
using Random = System.Random;

namespace OnCloud7
{
    public static class Util
    {
        private static readonly Random _random = new Random(System.DateTime.Now.Millisecond);

        public static Random Random => _random;

        /*
        /// <summary>
        /// 변화 전, 또는 제거될 심볼의 인덱스 범위.
        /// Exhaustive search를 지원하기 위해 모든 심볼을 대상에 포함합니다.
        /// </summary>
        private static readonly Dictionary<int, List<int>> _beforeSymbolPronounToIndex = new Dictionary<int, List<int>>()
        {
            { 777, new List<int> { 0, 1, 2} },
            { 7770, Enumerable.Range(8, 82-8).ToList() },
            { 7777, Enumerable.Range(7, 82-7).ToList() },
            { 8007, new List<int> { 8, 9, 10, 11, 72, 20, 21, 22, 23, 24, 75 } },
            { 8017, new List<int> { 12, 13, 14, 15, 73, 20, 21, 22, 23, 24, 75 } },
            { 8027, new List<int> { 16, 17, 18, 19, 74, 20, 21, 22, 23, 24, 75 } },
            { 8067, new List<int> { 25, 26, 27, 28, 76, 29 } },
            { 8107, new List<int> { 30, 31, 32, 33, 77, 42, 43, 44, 45, 46, 80 } },
            { 8117, new List<int> { 34, 35, 36, 37, 78, 42, 43, 44, 45, 46, 80 } },
            { 8127, new List<int> { 38, 39, 40, 41, 79, 42, 43, 44, 45, 46, 80 } },
            { 8707, new List<int> { 8, 9, 10, 11, 30, 31, 32, 33, 72, 77, 20, 21, 22, 23, 24, 42, 43, 44, 45, 46, 75, 80 } },
            { 8717, new List<int> { 12, 13, 14, 15, 34, 35, 36, 37, 73, 78, 20, 21, 22, 23, 24, 42, 43, 44, 45, 46, 75, 80 } },
            { 8727, new List<int> { 16, 17, 18, 19, 38, 39, 40, 41, 74, 79, 20, 21, 22, 23, 24, 42, 43, 44, 45, 46, 75, 80 } },
            { 8767, new List<int> { 25, 26, 27, 28, 47, 48, 49, 50, 76, 81, 29, 51 } },
            { 9070, new List<int> { 12, 16, 20, 25, 52, 53, 10, 14, 18, 23, 28, 58, 59 } },
            { 9071, new List<int> { 8, 17, 21, 26, 54, 55, 10, 14, 18, 23, 28, 58, 59 } },
            { 9072, new List<int> { 9, 13, 22, 27, 56, 57, 10, 14, 18, 23, 28, 58, 59 } },
            { 9170, new List<int> { 34, 38, 42, 47, 62, 63, 32, 36, 40, 45, 50, 68, 69 } },
            { 9171, new List<int> { 30, 39, 43, 48, 64, 65, 32, 36, 40, 45, 50, 68, 69 } },
            { 9172, new List<int> { 31, 35, 44, 49, 66, 67, 32, 36, 40, 45, 50, 68, 69 } },
            { 9176, new List<int> { 33, 37, 41, 46, 70, 71, 51 } },
            { 9770, new List<int> { 12, 16, 20, 25, 34, 38, 42, 47, 52, 53, 62, 63, 10, 14, 18, 23, 28, 32, 36, 40, 45, 50, 58, 59, 68, 69 } },
            { 9771, new List<int> { 8, 17, 21, 26, 30, 39, 43, 48, 54, 55, 64, 65, 10, 14, 18, 23, 28, 32, 36, 40, 45, 50, 58, 59, 68, 69 } },
            { 9772, new List<int> { 9, 13, 22, 27, 31, 35, 44, 49, 56, 57, 66, 67, 10, 14, 18, 23, 28, 32, 36, 40, 45, 50, 58, 59, 68, 69 } },
            { 9776, new List<int> { 11, 15, 19, 24, 33, 37, 41, 46, 60, 61, 70, 71, 29, 51 } },
        };
        */

        private static readonly Dictionary<int, Func<SymbolModel, bool>> _beforeSymbolPronounToPredicate = new()
        {
            { 0, (symbol) => !symbol.IsImmutable && symbol.ID == 0 },
            { 1, (symbol) => !symbol.IsImmutable && symbol.ID == 1 },
            { 2, (symbol) => !symbol.IsImmutable && symbol.ID == 2 },
            { 7, (symbol) => !symbol.IsImmutable && symbol.ID == 7 },
            { 8, (symbol) => !symbol.IsImmutable && symbol.ID == 8 },
            { 9, (symbol) => !symbol.IsImmutable && symbol.ID == 9 },
            { 10, (symbol) => !symbol.IsImmutable && symbol.ID == 10 },
            { 777, (symbol) => !symbol.IsImmutable && symbol.ID is >= 0 and <= 2 },
            { 7770, (symbol) => !symbol.IsImmutable && symbol.ID >= 8 },
            { 7777, (symbol) => !symbol.IsImmutable && symbol.ID >= 7 },
            { 8007, (symbol) => !symbol.IsImmutable && symbol.ID is 8 or 10 && symbol.Arg0 == 0 && symbol.Arg1 is 0 or 777 },
            { 8017, (symbol) => !symbol.IsImmutable && symbol.ID is 8 or 10 && symbol.Arg0 == 0 && symbol.Arg1 is 1 or 777 },
            { 8027, (symbol) => !symbol.IsImmutable && symbol.ID is 8 or 10 && symbol.Arg0 == 0 && symbol.Arg1 is 2 or 777 },
            { 8067, (symbol) => !symbol.IsImmutable && symbol.ID is 8 or 10 && symbol.Arg0 == 0 && symbol.Arg1 is 7777 },
            { 8107, (symbol) => !symbol.IsImmutable && symbol.ID is 8 or 10 && symbol.Arg0 == 1 && symbol.Arg1 is 0 or 777 },
            { 8117, (symbol) => !symbol.IsImmutable && symbol.ID is 8 or 10 && symbol.Arg0 == 1 && symbol.Arg1 is 1 or 777 },
            { 8127, (symbol) => !symbol.IsImmutable && symbol.ID is 8 or 10 && symbol.Arg0 == 1 && symbol.Arg1 is 2 or 777 },
            { 8707, (symbol) => !symbol.IsImmutable && symbol.ID is 8 or 10 && symbol.Arg1 is 0 or 777 },
            { 8717, (symbol) => !symbol.IsImmutable && symbol.ID is 8 or 10 && symbol.Arg1 is 1 or 777 },
            { 8727, (symbol) => !symbol.IsImmutable && symbol.ID is 8 or 10 && symbol.Arg1 is 2 or 777 },
            { 8767, (symbol) => !symbol.IsImmutable && symbol.ID is 8 or 10 && symbol.Arg1 is 7777 },
            { 9070, (symbol) => !symbol.IsImmutable && symbol.Arg0 == 0 && (symbol.ID is 8 && symbol.Arg3 is 0 or 777 || symbol.ID is 9 && symbol.Arg1 is 0 or 777) },
            { 9071, (symbol) => !symbol.IsImmutable && symbol.Arg0 == 0 && (symbol.ID is 8 && symbol.Arg3 is 1 or 777 || symbol.ID is 9 && symbol.Arg1 is 1 or 777) },
            { 9072, (symbol) => !symbol.IsImmutable && symbol.Arg0 == 0 && (symbol.ID is 8 && symbol.Arg3 is 2 or 777 || symbol.ID is 9 && symbol.Arg1 is 2 or 777) },
            { 9170, (symbol) => !symbol.IsImmutable && symbol.Arg0 == 1 && (symbol.ID is 8 && symbol.Arg3 is 0 or 777 || symbol.ID is 9 && symbol.Arg1 is 0 or 777) },
            { 9171, (symbol) => !symbol.IsImmutable && symbol.Arg0 == 1 && (symbol.ID is 8 && symbol.Arg3 is 1 or 777 || symbol.ID is 9 && symbol.Arg1 is 1 or 777) },
            { 9172, (symbol) => !symbol.IsImmutable && symbol.Arg0 == 1 && (symbol.ID is 8 && symbol.Arg3 is 2 or 777 || symbol.ID is 9 && symbol.Arg1 is 2 or 777) },
            { 9176, (symbol) => !symbol.IsImmutable && symbol.Arg0 == 1 && (symbol.ID is 8 && symbol.Arg3 is 7777 || symbol.ID is 9 && symbol.Arg1 is 7777) },
            { 9770, (symbol) => !symbol.IsImmutable && (symbol.ID is 8 && symbol.Arg3 is 0 or 777 || symbol.ID is 9 && symbol.Arg1 is 0 or 777) },
            { 9771, (symbol) => !symbol.IsImmutable && (symbol.ID is 8 && symbol.Arg3 is 1 or 777 || symbol.ID is 9 && symbol.Arg1 is 1 or 777) },
            { 9772, (symbol) => !symbol.IsImmutable && (symbol.ID is 8 && symbol.Arg3 is 2 or 777 || symbol.ID is 9 && symbol.Arg1 is 2 or 777) },
            { 9776, (symbol) => !symbol.IsImmutable && (symbol.ID is 8 && symbol.Arg3 is 7777 || symbol.ID is 9 && symbol.Arg1 is 7777) },
        };

        /// <summary>
        /// 변화 후, 또는 생성될 심볼의 인덱스 범위.
        /// Type에 따라 한 번 더 묶여 있습니다. (확률 보정)
        /// 대명사 의도에 벗어나는 부작용을 방지하기 위해, 임의의 심볼을 생성하는 심볼들은 제외됩니다. (예: 임의의 기본 심볼로 변화시키는 심볼)
        /// </summary>
        private static readonly Dictionary<int, List<List<int>>> _afterSymbolPronounToIndex = new()
        {
            { 777, new List<List<int>> { new() { 0, 1, 2 } } },
            { 7770, new List<List<int>> { Enumerable.Range(8, 52 - 8).ToList(), Enumerable.Range(52, 72 - 52).ToList(), Enumerable.Range(72, 82 - 72).ToList() } },
            { 7777, new List<List<int>> { new() { 7 }, Enumerable.Range(8, 52 - 8).ToList(), Enumerable.Range(52, 72 - 52).ToList(), Enumerable.Range(72, 82 - 72).ToList() } },
            { 8007, new List<List<int>> { new() { 8, 9, 10, 11 }, new() { 72 } } },
            { 8017, new List<List<int>> { new() { 12, 13, 14, 15 }, new() { 73 } } },
            { 8027, new List<List<int>> { new() { 16, 17, 18, 19 }, new() { 74 } } },
            { 8067, new List<List<int>> { new() { 25, 26, 27, 28 }, new() { 76 } } },
            { 8107, new List<List<int>> { new() { 30, 31, 32, 33 }, new() { 77 } } },
            { 8117, new List<List<int>> { new() { 34, 35, 36, 37 }, new() { 78 } } },
            { 8127, new List<List<int>> { new() { 38, 39, 40, 41 }, new() { 79 } } },
            { 8707, new List<List<int>> { new() { 8, 9, 10, 11, 30, 31, 32, 33 }, new() { 72, 77 } } },
            { 8717, new List<List<int>> { new() { 12, 13, 14, 15, 34, 35, 36, 37 }, new() { 73, 78 } } },
            { 8727, new List<List<int>> { new() { 16, 17, 18, 19, 38, 39, 40, 41 }, new() { 74, 79 } } },
            { 8767, new List<List<int>> { new() { 25, 26, 27, 28, 47, 48, 49, 50 }, new() { 76, 81 } } },
            { 9070, new List<List<int>> { new() { 12, 16, 20, 25 }, new() { 52, 53 } } },
            { 9071, new List<List<int>> { new() { 8, 17, 21, 26 }, new() { 54, 55 } } },
            { 9072, new List<List<int>> { new() { 9, 13, 22, 27 }, new() { 56, 57 } } },
            { 9170, new List<List<int>> { new() { 34, 38, 42, 47 }, new() { 62, 63 } } },
            { 9171, new List<List<int>> { new() { 30, 39, 43, 48 }, new() { 64, 65 } } },
            { 9172, new List<List<int>> { new() { 31, 35, 44, 49 }, new() { 66, 67 } } },
            { 9176, new List<List<int>> { new() { 33, 37, 41, 46 }, new() { 70, 71 } } },
            { 9770, new List<List<int>> { new() { 12, 16, 20, 25, 34, 38, 42, 47 }, new() { 52, 53, 62, 63 } } },
            { 9771, new List<List<int>> { new() { 8, 17, 21, 26, 30, 39, 43, 48 }, new() { 54, 55, 64, 65 } } },
            { 9772, new List<List<int>> { new() { 9, 13, 22, 27, 31, 35, 44, 49 }, new() { 56, 57, 66, 67 } } },
            { 9776, new List<List<int>> { new() { 11, 15, 19, 24, 33, 37, 41, 46 }, new() { 60, 61, 70, 71 } } },
        };

        /// <summary>
        /// After에서 사용
        /// </summary>
        /// <param name="symbolID"></param>
        /// <returns></returns>
        public static int SymbolIDToIndex(int symbolID)
        {
            switch (symbolID)
            {
                case 0:
                case 1:
                case 2:
                case 7:
                    return symbolID;
                case 8:
                    // Change
                    return Util.Random.Next(8, 52);
                case 9:
                    // Add
                    return Util.Random.Next(52, 72);
                case 10:
                    // Remove
                    return Util.Random.Next(72, 82);
                /*
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
                */
                default:
                    if (_afterSymbolPronounToIndex.TryGetValue(symbolID, out var indexList))
                    {
                        List<int> l = indexList[Util.Random.Next(indexList.Count)];
                        return l[Util.Random.Next(l.Count)];
                    }

                    return symbolID;
            }
        }

        /// <summary>
        /// Before에 사용
        /// </summary>
        /// <returns>symbol이 symbolID가 지칭하는 심볼 중 하나이면 true</returns>
        public static bool SymbolPredicate(int symbolID, SymbolModel symbol)
        {
            if (_beforeSymbolPronounToPredicate.TryGetValue(symbolID, out var predicate))
            {
                return predicate(symbol);
            }

            return false;
        }

    }
}
