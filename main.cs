using HarmonyLib;
using Il2CppRUMBLE.Players;
using MelonLoader;
using RumbleModdingAPI.RMAPI;
using UnityEngine;
using UIFramework;
using System.IO;

namespace RecursiveRockCam;

public static class BuildInfo
{
    public const string ModName = "RecursiveRockCam";
    public const string ModVersion = "1.2.0";
    public const string Description = "Makes the Rock Cam screen visible in Rock Cam";
    public const string Author = "Kalamart";
    public const string Company = "";
}
public partial class MainClass : MelonMod
{
    internal const string USER_DATA = "UserData/RecursiveRockCam/";
    internal const string CONFIG_FILE = "config.cfg";

    public static bool screenVisible = true;
    public static GameObject screen = null;

    internal static MelonPreferences_Category GeneralCategory;
    internal static MelonPreferences_Entry<bool> ScreenVisible;

    /**
    * <summary>
    * Log to console.
    * </summary>
    */
    private static void Log(string msg)
    {
        MelonLogger.Msg(msg);
    }
    /**
    * <summary>
    * Log to console but in yellow.
    * </summary>
    */
    private static void LogWarn(string msg)
    {
        MelonLogger.Warning(msg);
    }
    /**
    * <summary>
    * Log to console but in red.
    * </summary>
    */
    private static void LogError(string msg)
    {
        MelonLogger.Error(msg);
    }

    /**
     * <summary>
     * Initializes the MelonPreferences entries and creates the config file if it doesn't exist.
     * </summary>
     */
    private void PrefInit()
    {
        if (!Directory.Exists(USER_DATA))
            Directory.CreateDirectory(USER_DATA);

        GeneralCategory = MelonPreferences.CreateCategory("RecursiveRockCam", "General Settings");
        GeneralCategory.SetFilePath(Path.Combine(USER_DATA, CONFIG_FILE));

        ScreenVisible = GeneralCategory.CreateEntry("Screen visible", true, "Make Rock Cam screen visible in Rock Cam recordings.");
    }

    /**
     * <summary>
     * Called when the user saves a configuration in ModUI
     * </summary>
     */
    private void OnUISaved()
    {
        updateRockCamScreen();
    }

    /**
     * <summary>
     * Called when the mod is initialized, before any scene is loaded.
     * Initializes the MelonPreferences and registers the onsave event handler.
     * </summary>
     */
    public override void OnInitializeMelon()
    {
        PrefInit();
        UI.Register((MelonBase)this, GeneralCategory).OnModSaved += OnUISaved;
    }

    /**
    * <summary>
    * Called when the scene has finished loading.
    * </summary>
    */
    public override void OnSceneWasLoaded(int buildIndex, string sceneName)
    {
        if (sceneName != "Loader")
        {
            screen = null;
            updateRockCamScreen();
        }
    }

    /**
    * <summary>
    * Changes the layer of the Rock Cam screen
    * </summary>
    */
    private static void updateRockCamScreen()
    {
        try
        {
            screenVisible = ScreenVisible.Value;
            PlayerController playerController = Calls.Players.GetLocalPlayerController();
            if (playerController is null)
            {
                return;
            }
            if (screen is null)
            {
                screen = playerController.gameObject.transform.GetChild(7).GetChild(0).GetChild(2).GetChild(0).GetChild(3).GetChild(0).gameObject;
            }
            if (screenVisible)
            {
                screen.layer = LayerMask.NameToLayer("UI");
            }
            else
            {
                screen.layer = LayerMask.NameToLayer("RecordingCamera");
            }
        }
        catch (System.Exception e){}
    }

    /**
    * <summary>
    * Harmony patch that is called when the local player is initialized
    * </summary>
    */
    [HarmonyPatch(typeof(PlayerController), "Initialize", new System.Type[] { typeof(Player) })]
    public static class playerspawn
    {
        private static void Postfix(ref PlayerController __instance, ref Player player)
        {
            if (Calls.Players.GetLocalPlayer() == player)
            {
                updateRockCamScreen();
            }
        }
    }
}
