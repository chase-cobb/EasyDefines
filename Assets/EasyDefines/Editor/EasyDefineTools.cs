using UnityEngine;
using System;
using System.IO;
using System.Collections.Generic;

public class EasyDefineTools
{
    public static readonly string SYNC_FILE_LOCATION = "/EasyDefines/EasyDefines.txt";
    public static readonly string INACTIVE_DEFINE_PREFIX = ":0:0:0";

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
            File.Create(filePath);
        }

        string[] allLines = File.ReadAllLines();

        // does this define already exist?
        foreach(string line in allLines)
        {
            if(line.Contains(define))
            {
                Debug.Log("This define already exists");
                return;
            }
        }

        File.AppendAllText(filePath, define + INACTIVE_DEFINE_PREFIX + Environment.NewLine);
    }

    public static void RemoveDefine(string define)
    {
        string filePath = Application.dataPath + SYNC_FILE_LOCATION;
        if (!File.Exists(filePath))
        {
            File.Create(filePath);
        } 
        else
        {
            List<string> allLines = new List<string>(File.ReadAllLines());
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
    }

    public static void ForceRecompile()
    {
    }

    public static void ClearDefines()
    {
    }

    private static void SyncDefinesToRSP()
    {
        string filePath = Application.dataPath + SYNC_FILE_LOCATION;
        if (!File.Exists(filePath))
        {
            File.Create(filePath);
        }


        
        // TODO : parse sync file and add active defines to respective rsp files

    }

    private static void SyncDefinesFromRSP()
    {
    }
}