using HarmonyLib;
using Il2CppRUMBLE.Players;
using MelonLoader;
using RumbleModdingAPI;
using UnityEngine;
using RumbleModUI;

namespace RecursiveRockCam;

public static class BuildInfo
{
    public const string ModName = "RecursiveRockCam";
    public const string ModVersion = "1.0.1";
    public const string Description = "Makes the Rock Cam screen visible in Rock Cam";
    public const string Author = "Kalamart";
    public const string Company = "";
}
public partial class MainClass : MelonMod
{
    public static Mod Mod = new Mod();
    public static bool screenVisible = true;
    public static GameObject screen = null;

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
     * Specify the different options that will be used in the ModUI settings
     * </summary>
     */
    private void InitModUI()
    {
        UI.instance.UI_Initialized += OnUIInit;
        SetUIOptions();
    }

    /**
     * <summary>
     * Specify the different options that will be used in the ModUI settings
     * </summary>
     */
    private void SetUIOptions()
    {
        Mod.ModName = BuildInfo.ModName;
        Mod.ModVersion = BuildInfo.ModVersion;

        Mod.SetFolder(BuildInfo.ModName);
        Mod.AddToList("Screen visible", true, 0, "Make Rock Cam screen visible in Rock Cam recordings.", new Tags { });
        Mod.GetFromFile();
    }

    /**
     * <summary>
     * Called when the actual ModUI window is initialized
     * </summary>
     */
    private void OnUIInit()
    {
        Mod.ModSaved += OnUISaved;
        UI.instance.AddMod(Mod);
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
    * Called when the scene has finished loading.
    * </summary>
    */
    public override void OnSceneWasLoaded(int buildIndex, string sceneName)
    {
        if (sceneName == "Loader")
        {
            InitModUI();
            return;
        }
        screen = null;
        updateRockCamScreen();
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
            screenVisible = (bool)Mod.Settings[0].SavedValue;
            PlayerController playerController = Calls.Players.GetPlayerController();
            if (playerController is null)
            {
                return;
            }
            if (screen is null)
            {
                screen = playerController.gameObject.transform.GetChild(10).GetChild(2).GetChild(2).GetChild(0).GetChild(3).GetChild(0).gameObject;
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
