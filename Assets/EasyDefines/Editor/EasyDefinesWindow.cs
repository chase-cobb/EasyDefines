using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class EasyDefinesWindow : EditorWindow
{
    private static bool m_autoRecompile = false;
    private static List<EasyDefine> m_defines;
 
    private string currentDefineString = string.Empty;

	[MenuItem("Window/Easy Defines")]
	public static void ShowWindow()
	{
		EasyDefinesWindow definesWindow = EditorWindow.GetWindow<EasyDefinesWindow> ("Easy Defines");

        m_defines = EasyDefineTools.GetAllDefines();
	}

	void OnGUI()
	{
        bool changeDetected = false;
        bool recompileButtonPressed = false;
        Color defaultGUIColor = GUI.color;

        GUILayout.BeginVertical();
        {
            // draw current list of entries with checkboxes to activate for a specific rsp
            // right click contextual menu to remove
            if(m_defines != null)
            {
                GUILayout.BeginVertical();
                {
                    for(int i = 0; i < m_defines.Count; ++i)
                    {
                        GUILayout.BeginHorizontal("Box");
                        {
                            EasyDefine easyDefine = m_defines[i];
                            GUILayout.Label(easyDefine.m_defineName, GUILayout.Width(150));
                            GUI.color = easyDefine.m_csActive ? Color.green : Color.red;
                            if(GUILayout.Button("CS"))
                            {
                                easyDefine.m_csActive = !easyDefine.m_csActive;
                            }
                            GUI.color = easyDefine.m_editorActive ? Color.green : Color.red;
                            if(GUILayout.Button("ED"))
                            {
                                easyDefine.m_editorActive = !easyDefine.m_editorActive;
                            }
                            GUI.color = easyDefine.m_usActive ? Color.green : Color.red;
                            if(GUILayout.Button("US"))
                            {
                                easyDefine.m_usActive = !easyDefine.m_usActive;
                            }
                            GUI.color = defaultGUIColor;
                            m_defines[i] = easyDefine;
                        }
                        GUILayout.EndHorizontal();
                    }
                }
                GUILayout.EndVertical();
            }

            // textfield for adding new entries
            GUILayout.BeginHorizontal();
            {
                currentDefineString = GUILayout.TextField(currentDefineString);
                if (GUILayout.Button("Add"))
                {
                    EasyDefineTools.AddDefine(currentDefineString);
                    currentDefineString = string.Empty;
                    recompileButtonPressed = true;// HACK
                }
            }
            GUILayout.EndHorizontal();
        }
        GUILayout.EndVertical();

        if ((changeDetected && m_autoRecompile) || recompileButtonPressed)
        {
            EasyDefineTools.SyncDefinesToRSP();
            EasyDefineTools.ForceRecompile();
        }
	}
// unsafe ?
}