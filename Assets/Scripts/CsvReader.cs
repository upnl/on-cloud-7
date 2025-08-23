using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Text;
using UnityEngine;

namespace OnCloud7
{
    public static class CsvReader
    {
        private static readonly List<int> NullArg =  new() { -1 };
        
        /// <summary>
        /// SymbolTemplate.csv 파일 텍스트로부터 정보를 읽어와 SymbolTemplate 인스턴스 목록으로 변환합니다.
        /// </summary>
        /// <param name="csvTextAsset">CSV 파일의 전체 텍스트 내용</param>
        /// <param name="delimiter">CSV 열을 구분하는 글자</param>
        /// <returns></returns>
        public static List<SymbolTemplate> ToSymbolTemplates(this TextAsset csvTextAsset, char delimiter = '\t')
        {
            string csvText = csvTextAsset.text;
            bool isFirstLine = true;
            Dictionary<int, FieldInfo> fields = new Dictionary<int, FieldInfo>();
            Dictionary<int, Func<string, object>> typeConverter = new();
            List<SymbolTemplate> symbolTemplates = new List<SymbolTemplate>();

            // 서로 다른 운영체제의 줄바꿈 형식 고려
            csvText = csvText.Replace("\r\n", "\n").Replace("\r", "\n");

            foreach (string line in csvText.Split('\n'))
            {
                string l = line.Trim('\r'); // 줄바꿈에 있을 수 있는 '\r' 제거
                string[] tokens;
                if (isFirstLine)
                {
                    // 첫 번째 줄을 읽어 헤더로 사용합니다.
                    tokens = l.Split(delimiter);
                    for (int i = 0; i < tokens.Length; i++)
                    {
                        string token = tokens[i].Trim();
                        FieldInfo fieldInfo = typeof(SymbolTemplate).GetField(token,
                            BindingFlags.Instance | BindingFlags.Public);
                        if (fieldInfo != null)
                        {
                            fields.Add(i, fieldInfo);
                        }
                        
                        // 타입 변환기를 만듭니다.
                        switch (token)
                        {
                            case "ID":
                                typeConverter.Add(i, str =>
                                {
                                    if (string.IsNullOrEmpty(str)) return -1;
                                    return int.Parse(str);
                                });
                                break;
                            case "IsImmutable":
                                typeConverter.Add(i, str => bool.Parse(str));
                                break;
                            case "Name":
                            case "Description":
                                typeConverter.Add(i, str => str.Trim('"'));
                                break;
                            case "Type":
                                typeConverter.Add(i, str =>
                                {
                                    string[] keys = Enum.GetNames(typeof(SymbolTemplate.SymbolType));
                                    if (keys.Contains(str) && Enum.TryParse(str, out SymbolTemplate.SymbolType type))
                                    {
                                        return type;
                                    }
                                    else
                                    {
                                        return SymbolTemplate.SymbolType.Normal;
                                    }
                                });
                                break;
                            case "Arg0":
                            case "Arg1":
                            case "Arg2":
                            case "Arg3":
                                typeConverter.Add(i, str =>
                                {
                                    if (string.IsNullOrEmpty(str)) return NullArg;
                                    List<int> values = new List<int>();
                                    
                                    string[] intTokens = str.TrimStart('[').TrimEnd(']').Split(',');
                                    foreach (string intToken in intTokens)
                                    {
                                        values.Add(int.Parse(intToken));
                                    }
                                    return values;
                                });
                                break;
                            default:
                                Debug.LogError(ZString.Concat("CSV Error: 알 수 없는 ", i, "번째 타입 ", tokens[i]));
                                typeConverter.Add(i, str => str);
                                break;
                        }
                    }

                    isFirstLine = false;
                    continue;
                }

                // 두 번째 줄부터 진짜 데이터를 처리합니다.
                tokens = l.Split(delimiter);

                // 읽기가 끝났으면 종료합니다. (이게 없으면 마지막 줄을 읽을 때 오류 발생)
                if (tokens.Length == 0 || !int.TryParse(tokens[0], out _)) break;

                SymbolTemplate symbolTemplate = new SymbolTemplate();
                for (int i = 0; i < tokens.Length; i++)
                {
                    if (fields.Count <= i)
                    {
                        Debug.LogError(ZString.Concat("CSV Error: 헤더 길이를 초과하였습니다.\n", line));
                        continue; // Error
                    }

                    try
                    {
                        // Reflection을 사용해 템플릿 클래스의 해당 필드를 헤더에 있는 필드명으로부터 자동으로 찾아 여기에 값을 대입합니다.
                        fields[i].SetValue(symbolTemplate,
                            Convert.ChangeType(typeConverter[i](tokens[i].Trim()), fields[i].FieldType));
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                    }
                }

                symbolTemplates.Add(symbolTemplate);
            }

            List<SymbolTemplate> duplicatedTemplates = new List<SymbolTemplate>();
            foreach (var original in symbolTemplates)
            {
                for (int i0 = 0; i0 < original.Arg0.Count; i0++)
                {
                    for (int i1 = 0; i1 < original.Arg1.Count; i1++)
                    {
                        for (int i2 = 0; i2 < original.Arg2.Count; i2++)
                        {
                            for (int i3 = 0; i3 < original.Arg3.Count; i3++)
                            {
                                if (original.Type == SymbolTemplate.SymbolType.Change &&
                                    original.Arg1[i1] == original.Arg3[i3] &&
                                    original.Arg1[i1] < 100)
                                {
                                    // 같은 문양으로 바꾸는 효과는 의미 없음.
                                    // 단, 임의의 문양에서 임의의 문양으로는 바뀔 수 있음.
                                    continue;
                                }
                                var duplicated = new SymbolTemplate();
                                duplicated.CloneWithArgValues(original, original.Arg0[i0], original.Arg1[i1], original.Arg2[i2], original.Arg3[i3]);
                                duplicated.Initialize();
                                duplicatedTemplates.Add(duplicated);
                            }
                        }
                    }
                }
            }

            return duplicatedTemplates;
        }
        
        /// <summary>
        /// EnemySkillTemplate.csv 파일 텍스트로부터 정보를 읽어와 EnemySkillTemplate 인스턴스 목록으로 변환합니다.
        /// </summary>
        /// <param name="csvTextAsset">CSV 파일의 전체 텍스트 내용</param>
        /// <param name="delimiter">CSV 열을 구분하는 글자</param>
        /// <returns></returns>
        public static List<EnemySkillTemplate> ToEnemySkillTemplates(this TextAsset csvTextAsset, char delimiter = '\t')
        {
            string csvText = csvTextAsset.text;
            bool isFirstLine = true;
            Dictionary<int, FieldInfo> fields = new Dictionary<int, FieldInfo>();
            Dictionary<int, Func<string, object>> typeConverter = new();
            List<EnemySkillTemplate> enemySkillTemplates = new List<EnemySkillTemplate>();

            // 서로 다른 운영체제의 줄바꿈 형식 고려
            csvText = csvText.Replace("\r\n", "\n").Replace("\r", "\n");

            foreach (string line in csvText.Split('\n'))
            {
                string l = line.Trim('\r'); // 줄바꿈에 있을 수 있는 '\r' 제거
                string[] tokens;
                if (isFirstLine)
                {
                    // 첫 번째 줄을 읽어 헤더로 사용합니다.
                    tokens = l.Split(delimiter);
                    for (int i = 0; i < tokens.Length; i++)
                    {
                        string token = tokens[i].Trim();
                        FieldInfo fieldInfo = typeof(EnemySkillTemplate).GetField(token,
                            BindingFlags.Instance | BindingFlags.Public);
                        if (fieldInfo != null)
                        {
                            fields.Add(i, fieldInfo);
                        }
                        
                        // 타입 변환기를 만듭니다.
                        switch (token)
                        {
                            case "ID":
                            case "Cooltime":
                            case "MinDamage":
                            case "MaxDamage":
                                typeConverter.Add(i, str =>
                                {
                                    if (string.IsNullOrEmpty(str)) return -1;
                                    return int.Parse(str);
                                });
                                break;
                            case "Name":
                            case "Description":
                                typeConverter.Add(i, str => str.Trim('"'));
                                break;
                            default:
                                Debug.LogError(ZString.Concat("CSV Error: 알 수 없는 ", i, "번째 타입 ", tokens[i]));
                                typeConverter.Add(i, str => str);
                                break;
                        }
                    }

                    isFirstLine = false;
                    continue;
                }

                // 두 번째 줄부터 진짜 데이터를 처리합니다.
                tokens = l.Split(delimiter);

                // 읽기가 끝났으면 종료합니다. (이게 없으면 마지막 줄을 읽을 때 오류 발생)
                if (tokens.Length == 0 || !int.TryParse(tokens[0], out _)) break;

                EnemySkillTemplate enemySkillTemplate = new EnemySkillTemplate();
                for (int i = 0; i < tokens.Length; i++)
                {
                    if (fields.Count <= i)
                    {
                        Debug.LogError(ZString.Concat("CSV Error: 헤더 길이를 초과하였습니다.\n", line));
                        continue; // Error
                    }

                    try
                    {
                        // Reflection을 사용해 템플릿 클래스의 해당 필드를 헤더에 있는 필드명으로부터 자동으로 찾아 여기에 값을 대입합니다.
                        fields[i].SetValue(enemySkillTemplate,
                            Convert.ChangeType(typeConverter[i](tokens[i].Trim()), fields[i].FieldType));
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                    }
                }

                enemySkillTemplate.Initialize();
                enemySkillTemplates.Add(enemySkillTemplate);
            }

            return enemySkillTemplates;
        }
        
        /// <summary>
        /// EnemyTemplate.csv 파일 텍스트로부터 정보를 읽어와 EnemyTemplate 인스턴스 목록으로 변환합니다.
        /// </summary>
        /// <param name="csvTextAsset">CSV 파일</param>
        /// <param name="delimiter">CSV 열을 구분하는 글자</param>
        /// <returns></returns>
        public static List<EnemyTemplate> ToEnemyTemplates(this TextAsset csvTextAsset, List<EnemySkillTemplate> skillTemplates, char delimiter = '\t')
        {
            string csvText = csvTextAsset.text;
            bool isFirstLine = true;
            Dictionary<int, FieldInfo> fields = new Dictionary<int, FieldInfo>();
            Dictionary<int, Func<string, object>> typeConverter = new();
            List<EnemyTemplate> enemyTemplates = new List<EnemyTemplate>();

            // 서로 다른 운영체제의 줄바꿈 형식 고려
            csvText = csvText.Replace("\r\n", "\n").Replace("\r", "\n");

            foreach (string line in csvText.Split('\n'))
            {
                string l = line.Trim('\r'); // 줄바꿈에 있을 수 있는 '\r' 제거
                string[] tokens;
                if (isFirstLine)
                {
                    // 첫 번째 줄을 읽어 헤더로 사용합니다.
                    tokens = l.Split(delimiter);
                    for (int i = 0; i < tokens.Length; i++)
                    {
                        string token = tokens[i].Trim();
                        FieldInfo fieldInfo = typeof(EnemyTemplate).GetField(token,
                            BindingFlags.Instance | BindingFlags.Public);
                        if (fieldInfo != null)
                        {
                            fields.Add(i, fieldInfo);
                        }
                        
                        // 타입 변환기를 만듭니다.
                        switch (token)
                        {
                            case "ID":
                            case "Health":
                                typeConverter.Add(i, str =>
                                {
                                    if (string.IsNullOrEmpty(str)) return -1;
                                    return int.Parse(str);
                                });
                                break;
                            /*
                            case "float":
                                typeConverter.Add(i, str => float.Parse(str));
                                break;
                                */
                            case "IsBoss":
                                typeConverter.Add(i, str => bool.Parse(str));
                                break;
                            case "Name":
                            case "Description":
                                typeConverter.Add(i, str => str.Trim('"'));
                                break;
                            case "Skills":
                                typeConverter.Add(i, str =>
                                {
                                    if (string.IsNullOrEmpty(str)) return NullArg;
                                    List<int> values = new List<int>();
                                    
                                    string[] intTokens = str.TrimStart('[').TrimEnd(']').Split(',');
                                    foreach (string intToken in intTokens)
                                    {
                                        values.Add(int.Parse(intToken));
                                    }
                                    return values;
                                });
                                break;
                            default:
                                Debug.LogError(ZString.Concat("CSV Error: 알 수 없는 ", i, "번째 타입 ", tokens[i]));
                                typeConverter.Add(i, str => str);
                                break;
                        }
                    }

                    isFirstLine = false;
                    continue;
                }

                // 두 번째 줄부터 진짜 데이터를 처리합니다.
                tokens = l.Split(delimiter);

                // 읽기가 끝났으면 종료합니다. (이게 없으면 마지막 줄을 읽을 때 오류 발생)
                if (tokens.Length == 0 || !int.TryParse(tokens[0], out _)) break;

                EnemyTemplate enemyTemplate = new EnemyTemplate();
                for (int i = 0; i < tokens.Length; i++)
                {
                    if (fields.Count <= i)
                    {
                        Debug.LogError(ZString.Concat("CSV Error: 헤더 길이를 초과하였습니다.\n", line));
                        continue; // Error
                    }

                    try
                    {
                        // Reflection을 사용해 템플릿 클래스의 해당 필드를 헤더에 있는 필드명으로부터 자동으로 찾아 여기에 값을 대입합니다.
                        fields[i].SetValue(enemyTemplate,
                            Convert.ChangeType(typeConverter[i](tokens[i].Trim()), fields[i].FieldType));
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                    }
                }

                enemyTemplate.Initialize(skillTemplates);
                enemyTemplates.Add(enemyTemplate);
            }

            return enemyTemplates;
        }
        
        /// <summary>
        /// RoundUpgradeTemplate.csv 파일 텍스트로부터 정보를 읽어와 RoundUpgradeTemplate 인스턴스 목록으로 변환합니다.
        /// </summary>
        /// <param name="csvTextAsset">CSV 파일의 전체 텍스트 내용</param>
        /// <param name="delimiter">CSV 열을 구분하는 글자</param>
        /// <returns></returns>
        public static List<RoundUpgradeTemplate> ToRoundUpgradeTemplates(this TextAsset csvTextAsset, char delimiter = '\t')
        {
            string csvText = csvTextAsset.text;
            bool isFirstLine = true;
            Dictionary<int, FieldInfo> fields = new Dictionary<int, FieldInfo>();
            Dictionary<int, Func<string, object>> typeConverter = new();
            List<RoundUpgradeTemplate> roundUpgradeTemplates = new List<RoundUpgradeTemplate>();

            // 서로 다른 운영체제의 줄바꿈 형식 고려
            csvText = csvText.Replace("\r\n", "\n").Replace("\r", "\n");

            foreach (string line in csvText.Split('\n'))
            {
                string l = line.Trim('\r'); // 줄바꿈에 있을 수 있는 '\r' 제거
                string[] tokens;
                if (isFirstLine)
                {
                    // 첫 번째 줄을 읽어 헤더로 사용합니다.
                    tokens = l.Split(delimiter);
                    for (int i = 0; i < tokens.Length; i++)
                    {
                        string token = tokens[i].Trim();
                        FieldInfo fieldInfo = typeof(RoundUpgradeTemplate).GetField(token,
                            BindingFlags.Instance | BindingFlags.Public);
                        if (fieldInfo != null)
                        {
                            fields.Add(i, fieldInfo);
                        }
                        
                        // 타입 변환기를 만듭니다.
                        switch (token)
                        {
                            case "ID":
                            case "Level":
                                typeConverter.Add(i, str =>
                                {
                                    if (string.IsNullOrEmpty(str)) return -1;
                                    return int.Parse(str);
                                });
                                break;
                            case "Name":
                            case "Description":
                                typeConverter.Add(i, str => str.Trim('"'));
                                break;
                            case "Type":
                                typeConverter.Add(i, str =>
                                {
                                    string[] keys = Enum.GetNames(typeof(RoundUpgradeTemplate.UpgradeType));
                                    if (keys.Contains(str) && Enum.TryParse(str, out RoundUpgradeTemplate.UpgradeType type))
                                    {
                                        return type;
                                    }
                                    else
                                    {
                                        return RoundUpgradeTemplate.UpgradeType.Change;
                                    }
                                });
                                break;
                            case "Arg0":
                            case "Arg1":
                            case "Arg2":
                            case "Arg3":
                            case "Arg4":
                                typeConverter.Add(i, str =>
                                {
                                    if (string.IsNullOrEmpty(str)) return NullArg;
                                    List<int> values = new List<int>();
                                    
                                    string[] intTokens = str.TrimStart('[').TrimEnd(']').Split(',');
                                    foreach (string intToken in intTokens)
                                    {
                                        values.Add(int.Parse(intToken));
                                    }
                                    return values;
                                });
                                break;
                            default:
                                Debug.LogError(ZString.Concat("CSV Error: 알 수 없는 ", i, "번째 타입 ", tokens[i]));
                                typeConverter.Add(i, str => str);
                                break;
                        }
                    }

                    isFirstLine = false;
                    continue;
                }

                // 두 번째 줄부터 진짜 데이터를 처리합니다.
                tokens = l.Split(delimiter);

                // 읽기가 끝났으면 종료합니다. (이게 없으면 마지막 줄을 읽을 때 오류 발생)
                if (tokens.Length == 0 || !int.TryParse(tokens[0], out _)) break;

                RoundUpgradeTemplate roundUpgradeTemplate = new RoundUpgradeTemplate();
                for (int i = 0; i < tokens.Length; i++)
                {
                    if (fields.Count <= i)
                    {
                        Debug.LogError(ZString.Concat("CSV Error: 헤더 길이를 초과하였습니다.\n", line));
                        continue; // Error
                    }

                    try
                    {
                        // Reflection을 사용해 템플릿 클래스의 해당 필드를 헤더에 있는 필드명으로부터 자동으로 찾아 여기에 값을 대입합니다.
                        fields[i].SetValue(roundUpgradeTemplate,
                            Convert.ChangeType(typeConverter[i](tokens[i].Trim()), fields[i].FieldType));
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                    }
                }

                roundUpgradeTemplates.Add(roundUpgradeTemplate);
            }
            
            List<RoundUpgradeTemplate> duplicatedTemplates = new List<RoundUpgradeTemplate>();
            foreach (var original in roundUpgradeTemplates)
            {
                for (int i0 = 0; i0 < original.Arg0.Count; i0++)
                {
                    for (int i1 = 0; i1 < original.Arg1.Count; i1++)
                    {
                        for (int i2 = 0; i2 < original.Arg2.Count; i2++)
                        {
                            for (int i3 = 0; i3 < original.Arg3.Count; i3++)
                            {
                                if (original.Type == RoundUpgradeTemplate.UpgradeType.Change &&
                                    original.Arg1[i1] == original.Arg3[i3] &&
                                    original.Arg1[i1] < 100)
                                {
                                    // 같은 문양으로 바꾸는 효과는 의미 없음.
                                    // 단, 임의의 문양에서 임의의 문양으로는 바뀔 수 있음.
                                    continue;
                                }

                                for (int i4 = 0; i4 < original.Arg4.Count; i4++)
                                {
                                    var duplicated = new RoundUpgradeTemplate();
                                    duplicated.CloneWithArgValues(original, original.Arg0[i0], original.Arg1[i1], original.Arg2[i2], original.Arg3[i3], original.Arg4[i4]);
                                    duplicated.Initialize();
                                    duplicatedTemplates.Add(duplicated);
                                }
                            }
                        }
                    }
                }
            }

            return duplicatedTemplates;
        }
    }
}
