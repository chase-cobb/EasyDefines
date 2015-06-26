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
    
    public static void ForceRecompile()
    {
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
        
        m_defines.Clear();
        foreach (string define in allDefines)
        {
            EasyDefine easyDefine = GetEasyDefineFromDefineText(define);

            m_defines.Add(easyDefine);
        }

        return m_defines;
    }

    public static void SyncEasyDefinesToSyncFile()
    {
        string filePath = Application.dataPath + SYNC_FILE_LOCATION;
        if (!File.Exists(filePath))
        {
            FileStream fs = File.Create(filePath);
            fs.Close();
        }

        List<string> allDefines = new List<string>();
        foreach (EasyDefine easyDefine in m_defines)
        {
            allDefines.Add(GetDefineTextFromEasyDefine(easyDefine));
        }

        File.WriteAllLines(filePath, allDefines.ToArray());
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

    private static string GetDefineTextFromEasyDefine(EasyDefine easyDefine)
    {
        return string.Format(FULL_DEFINE_TEXT,
                             "D",
                             easyDefine.m_defineName,
                             easyDefine.m_csActive ? "1" : "0",
                             easyDefine.m_editorActive ? "1" : "0",
                             easyDefine.m_usActive ? "1" : "0");
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