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
                            case "Level":
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
                            case "IsNormal":
                                typeConverter.Add(i, str => bool.Parse(str));
                                break;
                            case "Name":
                            case "Description":
                                typeConverter.Add(i, str => str.Trim('"'));
                                break;
                            /*
                            case "List_Color":
                                typeConverter.Add(i, str =>
                                {
                                    string[] colorHexes = str.TrimStart('[').TrimEnd(']').Split(' ');
                                    List<Color> colors = new List<Color>();
                                    foreach (string colorHex in colorHexes)
                                    {
                                        string hex = colorHex;

                                        // #을 색상 코드 맨 앞에 붙여도 되고 안 붙여도 됩니다.
                                        if (colorHex.StartsWith("#")) hex = colorHex[1..];

                                        switch (hex.Length)
                                        {
                                            case 6:
                                                colors.Add(new Color(
                                                    Convert.ToInt32(hex[0..2], 16) / 255f,
                                                    Convert.ToInt32(hex[2..4], 16) / 255f,
                                                    Convert.ToInt32(hex[4..6], 16) / 255f)
                                                );
                                                break;
                                            case 8:
                                                colors.Add(new Color(
                                                    Convert.ToInt32(hex[0..2], 16) / 255f,
                                                    Convert.ToInt32(hex[2..4], 16) / 255f,
                                                    Convert.ToInt32(hex[4..6], 16) / 255f,
                                                    Convert.ToInt32(hex[6..8], 16) / 255f
                                                ));
                                                break;
                                            default:
                                                Debug.LogWarning(ZString.Concat(
                                                    "CSV Warning: Invalid color hex format (", colorHex, ")"));
                                                colors.Add(Color.white);
                                                break;
                                        }
                                    }

                                    return colors;
                                });
                                break;
                            case "Enum_Emotion":
                                typeConverter.Add(i, str =>
                                {
                                    string[] keys = Enum.GetNames(typeof(Character.Emotion));
                                    if (keys.Contains(str) && Enum.TryParse(str, out Character.Emotion emotion))
                                    {
                                        return emotion;
                                    }
                                    else
                                    {
                                        return Character.Emotion.Default;
                                    }
                                });
                                break;
                                */
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

                symbolTemplate.Initialize();
                symbolTemplates.Add(symbolTemplate);
            }

            return symbolTemplates;
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
                            case "SkillID1":
                            case "SkillID2":
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
        /// RewardTemplate.csv 파일 텍스트로부터 정보를 읽어와 RewardTemplate 인스턴스 목록으로 변환합니다.
        /// </summary>
        /// <param name="csvTextAsset">CSV 파일의 전체 텍스트 내용</param>
        /// <param name="delimiter">CSV 열을 구분하는 글자</param>
        /// <returns></returns>
        public static List<RewardTemplate> ToRewardTemplates(this TextAsset csvTextAsset, char delimiter = '\t')
        {
            string csvText = csvTextAsset.text;
            bool isFirstLine = true;
            Dictionary<int, FieldInfo> fields = new Dictionary<int, FieldInfo>();
            Dictionary<int, Func<string, object>> typeConverter = new();
            List<RewardTemplate> rewardTemplates = new List<RewardTemplate>();

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
                        FieldInfo fieldInfo = typeof(RewardTemplate).GetField(token,
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
                            case "Name":
                            case "Description":
                                typeConverter.Add(i, str => str.Trim('"'));
                                break;
                            case "Type":
                                typeConverter.Add(i, str =>
                                {
                                    string[] keys = Enum.GetNames(typeof(RewardTemplate.RewardType));
                                    if (keys.Contains(str) && Enum.TryParse(str, out RewardTemplate.RewardType type))
                                    {
                                        return type;
                                    }
                                    else
                                    {
                                        return RewardTemplate.RewardType.Change;
                                    }
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

                RewardTemplate rewardTemplate = new RewardTemplate();
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
                        fields[i].SetValue(rewardTemplate,
                            Convert.ChangeType(typeConverter[i](tokens[i].Trim()), fields[i].FieldType));
                    }
                    catch (Exception e)
                    {
                        Debug.LogError(e);
                    }
                }

                rewardTemplate.Initialize();
                rewardTemplates.Add(rewardTemplate);
            }

            return rewardTemplates;
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

                roundUpgradeTemplate.Initialize();
                roundUpgradeTemplates.Add(roundUpgradeTemplate);
            }

            return roundUpgradeTemplates;
        }
    }
}
