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
		if (m_defines == null)
		{
			m_defines = EasyDefineTools.GetAllDefines();
		}

        bool changeDetected = false;
        Color defaultGUIColor = GUI.color;

        GUILayout.BeginVertical();
        {
            // draw current list of entries with checkboxes to activate for a specific rsp
            // right click contextual menu to remove
            if(m_defines != null)
            {
                GUILayout.BeginVertical();
                {
                    int removeAtIndex = -1;
                    for(int i = 0; i < m_defines.Count; ++i)
                    {
                        GUILayout.BeginHorizontal("Box");
                        {
                            EasyDefine easyDefine = m_defines[i];
                            GUILayout.Label(easyDefine.m_defineName, GUILayout.Width(150));
                            GUI.color = easyDefine.m_csActive ? Color.green : Color.gray;
                            if(GUILayout.Button("CS"))
                            {
                                easyDefine.m_csActive = !easyDefine.m_csActive;
								changeDetected = true;// HACK
                            }
                            GUI.color = easyDefine.m_editorActive ? Color.green : Color.gray;
                            if(GUILayout.Button("ED"))
                            {
                                easyDefine.m_editorActive = !easyDefine.m_editorActive;
								changeDetected = true;// HACK
                            }
                            GUI.color = easyDefine.m_usActive ? Color.green : Color.gray;
                            if(GUILayout.Button("US"))
                            {
                                easyDefine.m_usActive = !easyDefine.m_usActive;
								changeDetected = true;// HACK
                            }
                            
                            GUI.color = Color.red;
                            if(GUILayout.Button("Delete"))
                            {
                                removeAtIndex = i;
								changeDetected = true;// HACK
                            }
                            GUI.color = defaultGUIColor;

                            m_defines[i] = easyDefine;
                        }
                        GUILayout.EndHorizontal();
                    }

                    if(removeAtIndex != -1)
                    {
                        m_defines.RemoveAt(removeAtIndex);

						changeDetected = true;
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
					if(currentDefineString != string.Empty)
					{
	                    EasyDefine easyDefine;
	                    easyDefine.m_defineName = currentDefineString;
	                    easyDefine.m_csActive = false;
	                    easyDefine.m_editorActive = false;
	                    easyDefine.m_usActive = false;
	                    m_defines.Add(easyDefine);
	                    currentDefineString = string.Empty;
						changeDetected = true;// HACK
					}
                }
            }
            GUILayout.EndHorizontal();
		}
		
		if (changeDetected)
		{
			EasyDefineTools.SyncEasyDefinesToSyncFile();
			EasyDefineTools.SyncDefinesToRSP();
		}

		if (GUILayout.Button ("Apply Changes"))
		{
			EasyDefineTools.ForceRecompile();
		}
        GUILayout.EndVertical();
	}
// unsafe ?
}