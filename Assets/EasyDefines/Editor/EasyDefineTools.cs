using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;

public class EasyDefineTools
{
    public static readonly string SYNC_FILE_LOCATION = "/EasyDefines/EasyDefines.txt";
    public static readonly string INACTIVE_DEFINE_PREFIX = ":0:0:0";
    public static readonly string FULL_DEFINE_TEXT = "{0}:{1}:{2}:{3}:{4}";

    private static List<EasyDefine> m_defines = new List<EasyDefine>();

    [Flags]
    public enum DefineType
    {
        C_SHARP = 1,
        C_SHARP_EDITOR = 2,
        UNITYSCRIPT = 4
    }

    public static void AddDefine(string define)
    {
        string filePath = Application.dataPath + SYNC_FILE_LOCATION;
        if (!File.Exists(filePath))
        {
            FileStream fs = File.Create(filePath);
            fs.Close();
        }

        string[] allLines = File.ReadAllLines(filePath);

        // TODO : MAKE SURE THE DEFINE HAS ONLY VALID CHARACTERS!!

        // does this define already exist?
        foreach(string line in allLines)
        {
            if(line.Contains(define))
            {
                Debug.Log("This define already exists");
                return;
            }
        }

        string newDefine = "D:" + define + INACTIVE_DEFINE_PREFIX + Environment.NewLine;
        m_defines.Add(GetEasyDefineFromDefineText(newDefine));
        File.AppendAllText(filePath, newDefine);
    }

    public static void RemoveDefine(string define)
    {
        string filePath = Application.dataPath + SYNC_FILE_LOCATION;
        if (!File.Exists(filePath))
        {
            FileStream fs = File.Create(filePath);
            fs.Close();
        } 
        else
        {
            List<string> allLines = new List<string>(File.ReadAllLines(filePath));
            string lineContainingDefine = string.Empty;

            foreach(string line in allLines)
            {
                if(line.Contains(define))
                {
                    lineContainingDefine = line;
                }
            }

            if(lineContainingDefine != string.Empty)
            {
                // TODO : remove from defines list
                allLines.Remove(lineContainingDefine);
                File.WriteAllLines(filePath, allLines.ToArray());
            }
            else
            {
                Debug.Log("Can't remove define, not found");
            }
        }
    }

    public static void SetDefineActive(string define,bool isActive, DefineType defineType)
    {
        string filePath = Application.dataPath + SYNC_FILE_LOCATION;
        if (!File.Exists(filePath))
        {
            FileStream fs = File.Create(filePath);
            fs.Close();
        } 
        else
        {
            List<string> allLines = new List<string>(File.ReadAllLines(filePath));
            string lineContainingDefine = string.Empty;
            char[] delims = {':'};
            
            int itemIndex = allLines.FindIndex(x => x.Split(delims)[1] == define);

            if(itemIndex != -1)
            {
                allLines[itemIndex] = string.Format(FULL_DEFINE_TEXT,
                                                    "D",
                                                    define,
                                                    ((defineType & DefineType.C_SHARP) != 0) ? 1 : 0,
                                                    ((defineType & DefineType.C_SHARP_EDITOR) != 0) ? 1 : 0,
                                                    ((defineType & DefineType.UNITYSCRIPT) != 0) ? 1 : 0);

                File.WriteAllLines(filePath, allLines.ToArray());
            }
        }
    }

    public static List<EasyDefine> GetAllDefines()
    {
        string filePath = Application.dataPath + SYNC_FILE_LOCATION;
        if (!File.Exists(filePath))
        {
            FileStream fs = File.Create(filePath);
            fs.Close();
        }

        string[] allDefines = File.ReadAllLines(filePath);

        foreach (string define in allDefines)
        {
            EasyDefine easyDefine = GetEasyDefineFromDefineText(define);

            m_defines.Add(easyDefine);
        }

        return m_defines;
    }

    public static void ForceRecompile()
    {
    }

    public static void ClearDefines()
    {
    }

    public static void SyncDefinesToRSP()
    {
        string filePath = Application.dataPath + SYNC_FILE_LOCATION;
        if (!File.Exists(filePath))
        {
            FileStream fs = File.Create(filePath);
            fs.Close();
        }
        
        char[] delims = {':'};
        List<string> allLines = new List<string>(File.ReadAllLines(filePath));
        string[] cSharpDefines = allLines.FindAll(x => x.Split(delims) [2] == "1").ToArray();
        string[] editorDefines = allLines.FindAll(x => x.Split(delims) [3] == "1").ToArray();
        string[] unityScriptDefines = allLines.FindAll(x => x.Split(delims) [4] == "1").ToArray();

        if (cSharpDefines != null && cSharpDefines.Length > 0)
        {
            WriteActiveDefineListToRSP(Application.dataPath + "/smcs.rsp", cSharpDefines);
        }
        if (editorDefines != null && editorDefines.Length > 0)
        {
            WriteActiveDefineListToRSP(Application.dataPath + "/gmcs.rsp", editorDefines);
        }
        if (unityScriptDefines != null && unityScriptDefines.Length > 0)
        {
            WriteActiveDefineListToRSP(Application.dataPath + "/us.rsp", unityScriptDefines);
        }
    }

    private static void SyncDefinesFromRSP()
    {
    }

    private static void WriteActiveDefineListToRSP(string rspPath, string[] activeDefines)
    {
        if(!File.Exists(rspPath))
        {
            FileStream fs = File.Create(rspPath);
            fs.Close();
        }

        char[] delims = {':'};

        for (int i = 0; i < activeDefines.Length; ++i)
        {
            activeDefines[i] = activeDefines[i].Split(delims)[1];
        }
        
        List<string> newDefines = new List<string>();
        
        foreach(string define in activeDefines)
        {
            newDefines.Add("-define:" + define);
        }
        
        File.WriteAllLines(rspPath, newDefines.ToArray());
    }

    private static EasyDefine GetEasyDefineFromDefineText(string define)
    {
        char[] delims = {':'};
        string[] defineInfo = define.Split(delims);
        
        EasyDefine easyDefine;
        easyDefine.m_defineName = defineInfo[1];
        easyDefine.m_csActive = defineInfo[2] == "1";
        easyDefine.m_editorActive = defineInfo[3] == "1";
        easyDefine.m_usActive = defineInfo[4] == "1";

        return easyDefine;
    }
}