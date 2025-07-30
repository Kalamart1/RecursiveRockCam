using HarmonyLib;
using Il2CppRUMBLE.Players;
using MelonLoader;
using RumbleModdingAPI;
using UnityEngine;

namespace RecursiveRockCam;

public static class BuildInfo
{
    public const string ModName = "RecursiveRockCam";
    public const string ModVersion = "1.0.0";
    public const string Description = "Makes the Rock Cam screen visible in Rock Cam";
    public const string Author = "Kalamart";
    public const string Company = "";
}
public partial class MainClass : MelonMod
{
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
    * Called when the scene has finished loading.
    * </summary>
    */
    public override void OnSceneWasLoaded(int buildIndex, string sceneName)
    {
        editRockCamScreen();
    }

    private static void editRockCamScreen()
    {
        try
        {
            PlayerController playerController = Calls.Players.GetPlayerController();
            if (playerController is null)
            {
                return;
            }
            GameObject screen = playerController.gameObject.transform.GetChild(10).GetChild(2).GetChild(2).GetChild(0).GetChild(3).GetChild(0).gameObject;
            screen.layer = LayerMask.NameToLayer("UI");
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
                editRockCamScreen();
            }
        }
    }
}
