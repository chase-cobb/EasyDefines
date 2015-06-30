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
using System.Collections.Generic;

/// <summary>
/// Custom window for working with Easy Defines.
/// </summary>
public class EasyDefinesWindow : EditorWindow
{
    private static List<EasyDefine> m_defines;
 
    private string currentDefineString = string.Empty;

	[MenuItem("Window/#Easy Defines")]
	/// <summary>
	/// Shows the Easy Define window.
	/// </summary>
	public static void ShowWindow()
	{
		EasyDefinesWindow definesWindow = EditorWindow.GetWindow<EasyDefinesWindow> ("#Easy Defines");

		m_defines = EasyDefineTools.GetAllDefines();
	}

	/// <summary>
	/// Raises the GU event.
	/// </summary>
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
                            GUI.color = easyDefine.m_editorActive ? Color.green : Color.gray;
                            if(GUILayout.Button("Editor"))
                            {
                                easyDefine.m_editorActive = !easyDefine.m_editorActive;
                                changeDetected = true;
                            }
                            GUI.color = easyDefine.m_csActive ? Color.green : Color.gray;
                            if(GUILayout.Button("C#"))
                            {
                                easyDefine.m_csActive = !easyDefine.m_csActive;
								changeDetected = true;
                            }
                            GUI.color = easyDefine.m_usActive ? Color.green : Color.gray;
                            if(GUILayout.Button("JS"))
                            {
                                easyDefine.m_usActive = !easyDefine.m_usActive;
								changeDetected = true;
                            }
                            
                            GUI.color = Color.red;
                            if(GUILayout.Button("Delete"))
                            {
                                removeAtIndex = i;
								changeDetected = true;
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
}