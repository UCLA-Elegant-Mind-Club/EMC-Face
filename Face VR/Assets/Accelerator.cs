using UnityEditor;

public class Accelerator : Editor
{
    private static bool assembliesLocked;
    private static int sceneLockIndex;

    [InitializeOnLoadMethod]
    public static void initialize()
    {
        if (!SessionState.GetBool("FirstInitComplete", false))
        {
            EditorApplication.UnlockReloadAssemblies();
            SessionState.SetBool("assembliesLocked", false);
            EditorSettings.enterPlayModeOptionsEnabled = true;
            EditorSettings.enterPlayModeOptions = EnterPlayModeOptions.None;
            SessionState.SetBool("FirstInitComplete", true);
        }
        assembliesLocked = SessionState.GetBool("assembliesLocked", false);
        sceneLockIndex = (int)EditorSettings.enterPlayModeOptions;
    }

    #region Disable Auto Script Assembly Reloading
    [MenuItem("Accelerator/Disable Auto Script Assembly Reloading &R", false, priority = 1)]
    private static void disableAssemblyReload()
    {
        assembliesLocked = !SessionState.GetBool("assembliesLocked", false);
        SessionState.SetBool("assembliesLocked", assembliesLocked);
        if (assembliesLocked)
            EditorApplication.LockReloadAssemblies();
        else
            EditorApplication.UnlockReloadAssemblies();
        AssetDatabase.Refresh();
    }

    [MenuItem("Accelerator/Disable Auto Script Assembly Reloading &R", true)]
    private static bool disableAssemblyReloadValidate()
    {
        Menu.SetChecked("Accelerator/Disable Auto Script Assembly Reloading &R", assembliesLocked);
        return true;
    }
    #endregion

    #region Toggle Domain Reloading on Play
    [MenuItem("Accelerator/Disable Domain Reloading on Play", priority = 101)]
    private static void toggleDomainReload()
    {
        sceneLockIndex = (int)EditorSettings.enterPlayModeOptions;
        sceneLockIndex += sceneLockIndex % 2 == 1 ? -1 : 1;
        EditorSettings.enterPlayModeOptions = (EnterPlayModeOptions)sceneLockIndex;
    }

    [MenuItem("Accelerator/Disable Domain Reloading on Play", true)]
    private static bool toggleDomainReloadValidate()
    {
        Menu.SetChecked("Accelerator/Disable Domain Reloading on Play", sceneLockIndex % 2 == 1);
        return true;
    }
    #endregion

    #region Toggle Script Reloading on Play
    [MenuItem("Accelerator/Disable Script Reloading on Play", priority = 102)]
    private static void toggleScriptReload()
    {
        sceneLockIndex = (int)EditorSettings.enterPlayModeOptions;
        sceneLockIndex += sceneLockIndex >= 2 ? -2 : 2;
        EditorSettings.enterPlayModeOptions = (EnterPlayModeOptions)sceneLockIndex;
    }

    [MenuItem("Accelerator/Disable Script Reloading on Play", true)]
    private static bool toggleScriptReloadValidate()
    {
        Menu.SetChecked("Accelerator/Disable Script Reloading on Play", sceneLockIndex >= 2);
        return true;
    }
    #endregion
}