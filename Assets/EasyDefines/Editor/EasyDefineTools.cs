//Copyright 2015 Chase Cobb
//
//Licensed under the Apache License, Version 2.0 (the "License");
//you may not use this file except in compliance with the License.
//You may obtain a copy of the License at
//		
//		http://www.apache.org/licenses/LICENSE-2.0
//		
//Unless required by applicable law or agreed to in writing, software
//distributed under the License is distributed on an "AS IS" BASIS,
//WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//See the License for the specific language governing permissions and
//limitations under the License.
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections.Generic;

/// <summary>
/// Tools for basic Easy Define functionality.
/// </summary>
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
    
	/// <summary>
	/// Forces a recompile of scripts.
	/// </summary>
    public static void ForceRecompile()
	{
		AssetDatabase.StartAssetEditing();
		string[] allAssetPaths = AssetDatabase.GetAllAssetPaths();
		foreach (string assetPath in allAssetPaths)
		{
			MonoScript script = AssetDatabase.LoadAssetAtPath(assetPath, typeof(MonoScript)) as MonoScript;
			if (script != null)
			{
				AssetDatabase.ImportAsset(assetPath);
			}
		}
		AssetDatabase.StopAssetEditing();
    }

	/// <summary>
	/// Gets all defines.
	/// </summary>
	/// <returns>The all defines.</returns>
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

	/// <summary>
	/// Syncs the easy defines to sync file.
	/// </summary>
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

	/// <summary>
	/// Syncs the defines to the appropriate RSP files.
	/// </summary>
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

        WriteActiveDefineListToRSP(Application.dataPath + "/smcs.rsp", cSharpDefines);
        WriteActiveDefineListToRSP(Application.dataPath + "/gmcs.rsp", editorDefines);
        WriteActiveDefineListToRSP(Application.dataPath + "/us.rsp", unityScriptDefines);
    }

	/// <summary>
	/// Writes the active define list to RSP files.
	/// </summary>
	/// <param name="rspPath">Rsp path.</param>
	/// <param name="activeDefines">Active defines.</param>
    private static void WriteActiveDefineListToRSP(string rspPath, string[] activeDefines)
    {
        if(!File.Exists(rspPath))
        {
            FileStream fs = File.Create(rspPath);
            fs.Close();
        }

        char[] delims = {':'};

        
        List<string> newDefines = new List<string>();
        
        foreach(string define in activeDefines)
        {
            newDefines.Add("-define:" + define);
        }
        
        File.WriteAllLines(rspPath, newDefines.ToArray());
    }

	/// <summary>
	/// Gets the define text from easy define.
	/// </summary>
	/// <returns>The define text from easy define.</returns>
	/// <param name="easyDefine">Easy define.</param>
    private static string GetDefineTextFromEasyDefine(EasyDefine easyDefine)
    {
        return string.Format(FULL_DEFINE_TEXT,
                             "D",
                             easyDefine.m_defineName,
                             easyDefine.m_csActive ? "1" : "0",
                             easyDefine.m_editorActive ? "1" : "0",
                             easyDefine.m_usActive ? "1" : "0");
    }

	/// <summary>
	/// Gets the easy define from define text.
	/// </summary>
	/// <returns>The easy define from define text.</returns>
	/// <param name="define">Define.</param>
    private static EasyDefine GetEasyDefineFromDefineText(string define)
    {
		// TODO : chase : is this string a valid define?

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