using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Text;
using UnityEngine;

public static class CsvReader
{
    /// <summary>
    /// CSV 파일 텍스트로부터 대사를 읽어와 대사 목록으로 변환합니다.
    /// </summary>
    /// <param name="csvText">대사가 들어있는 CSV 파일의 전체 텍스트 내용</param>
    /// <param name="delimiter">CSV 열을 구분하는 글자</param>
    /// <returns></returns>
    public static List<DialogEntity> ToDialogs(this string csvText, char delimiter = '\t')
    {
        bool isFirstLine = true;
        bool isSecondLine = false;
        Dictionary<int, FieldInfo> fields = new Dictionary<int, FieldInfo>();
        Dictionary<int, Func<string, object>> typeConverter = new();
        List<DialogEntity> dialogEntities = new List<DialogEntity>();
        
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
                    FieldInfo fieldInfo = typeof(DialogEntity).GetField(tokens[i].Trim(), BindingFlags.Instance | BindingFlags.Public);
                    if (fieldInfo != null)
                    {
                        fields.Add(i, fieldInfo);
                    }
                }
                isFirstLine = false;
                isSecondLine = true;
                continue;
            }

            if (isSecondLine)
            {
                // 두 번째 줄을 읽어 타입 변환기를 설정합니다.
                tokens = l.Split(delimiter);
                for (int i = 0; i < tokens.Length; i++)
                {
                    switch (tokens[i].Trim())
                    {
                        case "int":
                            typeConverter.Add(i, str =>
                            {
                                if (string.IsNullOrEmpty(str)) return -1;
                                return int.Parse(str);
                            });
                            break;
                        case "float":
                            typeConverter.Add(i, str => float.Parse(str));
                            break;
                        case "bool":
                            typeConverter.Add(i, str => bool.Parse(str));
                            break;
                        case "string":
                            typeConverter.Add(i, str => str);
                            break;
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
                                            Debug.LogWarning(ZString.Concat("CSV Warning: Invalid color hex format (", colorHex, ")"));
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
                        default:
                            Debug.LogError(ZString.Concat("CSV Error: 알 수 없는 ", i, "번째 타입 ", tokens[i]));
                            typeConverter.Add(i, str => str);
                            break;
                    }
                }
                isSecondLine = false;
                continue;
            }
            
            // 세 번째 줄부터 진짜 데이터를 처리합니다.
            tokens = l.Split(delimiter);
            
            // 읽기가 끝났으면 종료합니다. (이게 없으면 마지막 줄을 읽을 때 오류 발생)
            if (tokens.Length == 0 || !int.TryParse(tokens[0], out _)) break;
            
            //DialogEntity dialogEntity = ScriptableObject.CreateInstance<DialogEntity>();
            DialogEntity dialogEntity = new DialogEntity();
            for (int i = 0; i < tokens.Length; i++)
            {
                if (fields.Count <= i)
                {
                    Debug.LogError(ZString.Concat("CSV Error: 헤더 길이를 초과하였습니다.\n", line));
                    continue; // Error
                }

                try
                {
                    // Reflection을 사용해 DialogEntity의 해당 필드를 헤더에 있는 필드명으로부터 자동으로 찾아 여기에 값을 대입합니다.
                    fields[i].SetValue(dialogEntity, Convert.ChangeType(typeConverter[i](tokens[i].Trim()), fields[i].FieldType));
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                }
            }
            dialogEntity.Initialize();
            dialogEntities.Add(dialogEntity);
        }
        return dialogEntities;
    }
}
