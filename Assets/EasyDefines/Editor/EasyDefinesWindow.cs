using UnityEngine;
using UnityEditor;

public class EasyDefinesWindow : EditorWindow
{
    private static bool m_autoRecompile = false;

	[MenuItem("Window/Easy Defines")]
	public static void ShowWindow()
	{
		EasyDefinesWindow definesWindow = EditorWindow.GetWindow<EasyDefinesWindow> ("Easy Defines");
	}

	void OnGUI()
	{
        bool changeDetected = false;
        bool recompileButtonPressed = false;
		// TODO : DRAW WINDOW

		// C# - smcs.rsp
		// C# Editor - gmcs.rsp
		// UnityScript = us.rsp

        // draw current list of entries with checkboxes to activate for a specific rsp
            // right click contextual menu to remove

        // textfield for adding new entries

        if ((changeDetected && m_autoRecompile) || recompileButtonPressed)
        {
            EasyDefineTools.ForceRecompile();
        }
	}
//“-define:UNITY_DEBUG”
// unsafe ?
}