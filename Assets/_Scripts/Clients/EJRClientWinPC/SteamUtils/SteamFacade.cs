using System.Runtime.InteropServices;
using System;
using System.Collections.Generic;
using Steamworks;
using EJROrbEngine;

namespace ZZA {
public class SteamFacade {

        public static bool SteamIsInitialized = false;
        public static bool steamControllerDetected = false;


        public static bool SteamAPI_Init()
        {
            if (SteamManager.Initialized)
                SteamIsInitialized = true;
            else SteamIsInitialized = false;
            return SteamIsInitialized;
        }
        public static void SteamAPI_Shutdown()
        {

        }
        
   
    public static bool SteamAPI_RestartAppIfNecessary( uint unOwnAppID )
        {
            return false;
        }
//    [DllImportAttribute(DllName,EntryPoint = "SteamAPI_ISteamController_GetConnectedControllers", CallingConvention = CallingConvention.Cdecl)]
  //      public static extern int ISteamController_GetConnectedControllers(IntPtr instancePtr, [In, Out] ulong[] handlesOut);

    
    internal static void SteamAPI_RunCallbacks()
        {

        }

        public static void SteamAPI_ISteamController_RunFrame(IntPtr instancePtr)
        {

        }


	public static bool setAchievement(string achievementName)
	{
		if(EJRConsts.Instance.usingSteam())
		{
                //                IntPtr pSteamUserStats = SteamAPIInterop.SteamUserStats();          
                //                bool ret = SteamAPI_ISteamUserStats_SetAchievement(pSteamUserStats, achievementName);
                //          SteamAPI_ISteamUserStats_StoreStats(pSteamUserStats);
                bool ret = SteamUserStats.SetAchievement(achievementName);
                SteamUserStats.StoreStats();

                return ret;
		} else return false;


	}
    public static bool indicateAchievementProgress(string achievementName, uint nCurProgress, uint nMaxProgress)
    {
        if(EJRConsts.Instance.usingSteam())
        {
                //            IntPtr pSteamUserStats = SteamUserStats();
                //          bool ret = SteamAPI_ISteamUserStats_IndicateAchievementProgress(pSteamUserStats, achievementName, nCurProgress, nMaxProgress);
                //        SteamAPI_ISteamUserStats_StoreStats(pSteamUserStats);
                bool ret = SteamUserStats.IndicateAchievementProgress(achievementName, nCurProgress, nMaxProgress);
                SteamUserStats.StoreStats();
                return ret;
        } else return false;

    }


    public static void RunCallbacks()
    {
      //      EngineConsts.DebugLog("RunCallbacks");
/*         if(iSteamController == IntPtr.Zero) 
             initializeControllers();

         SteamAPI_ISteamController_RunFrame(iSteamController);
         if(iSteamController != IntPtr.Zero && !steamControllerDetected)
         {
             ISteamController_GetConnectedControllers(iSteamController, controllerHandle);
             foreach(ulong cH in controllerHandle)
                 if(cH != 0)  steamControllerDetected = true;
        }

        SteamAPI_RunCallbacks();*/
    }
        /*
    public static string getLang()
    {
        EngineConsts.DebugLog("getSteam Lang");
        IntPtr ptr = SteamAPIInterop.SteamUtils();
        string langCode = PtrToStringUTF8(NativeEntrypoints.SteamAPI_ISteamUtils_GetSteamUILanguage(ptr)); 
        if(langCode == "polish") langCode = "pl";
        if(langCode == "english") langCode = "en";
        return langCode;
    }*/
    
    //Taken from STEAMWORKS.NET
    // This continues to exist for both 'out string' and strings returned by Steamworks functions.
        public static string PtrToStringUTF8(IntPtr nativeUtf8) {
            if (nativeUtf8 == IntPtr.Zero) {
                return string.Empty;
            }

            int len = 0;

            while (Marshal.ReadByte(nativeUtf8, len) != 0) {
                ++len;
            }

            if (len == 0) {
                return string.Empty;
            }

            byte[] buffer = new byte[len];
            Marshal.Copy(nativeUtf8, buffer, 0, buffer.Length);
        //EngineConsts.DebugLog("Steam Lang: " + System.Text.Encoding.UTF8.GetString(buffer));
            return System.Text.Encoding.UTF8.GetString(buffer);
        } 

        public static void OpenWWW()
        {
    //      IntPtr pSteamFriends = SteamAPIInterop.SteamFriends();
      //  NativeEntrypoints.SteamAPI_ISteamFriends_ActivateGameOverlayToWebPage(pSteamFriends, "http://store.steampowered.com/app/461850");

        }
        /*
        private static IntPtr iSteamController = IntPtr.Zero;
        private static ulong[] controllerHandle;
        private static Dictionary<string, ulong> actionHandles;
        private static Dictionary<string, ulong> actionSetHandles;

        public static string GetControllerAction()
        {
            if(!SteamIsInitialized) return "";
          //  MainConsts.DebugLog("steam init ok " + iSteamController.ToString());
            if (iSteamController == IntPtr.Zero )
                initializeControllers();
         //   MainConsts.DebugLog("GetControllerAction");
            if(iSteamController == IntPtr.Zero || !steamControllerDetected)   return "";
            foreach(ulong cH in controllerHandle)
            {
                if(cH != 0)
                {            
                //    MainConsts.DebugLog("GetControllerAction ch " + cH.ToString());
                 //   if(NativeEntrypoints.SteamAPI_ISteamController_GetDigitalActionData(iSteamController, cH, actionHandles["Action_Undo"]).bState)   return "Action_Undo";

                    ControllerAnalogActionData_t actionMoveData = NativeEntrypoints.SteamAPI_ISteamController_GetAnalogActionData(iSteamController, cH, actionHandles["Action_Move"]);
                  //  if( actionMoveData.y < 0  && actionMoveData.x < 0) return "GameMoveLeftDown";
                    //if( actionMoveData.y > 0  && actionMoveData.x > 0) return "GameMoveRightUp";
                    //if(actionMoveData.y > 0  && actionMoveData.x < 0) return "GameMoveLeftUp";
                    //if( actionMoveData.y < 0  && actionMoveData.x > 0) return "GameMoveRightDown";
                    if( actionMoveData.y < 0) return "GameMoveDown";
                    if( actionMoveData.y > 0) return "GameMoveUp";
                    if( actionMoveData.x < 0) return "GameMoveLeft";
                    if (actionMoveData.x > 0) return "GameMoveRight";
                        
                    if(NativeEntrypoints.SteamAPI_ISteamController_GetDigitalActionData(iSteamController, cH, actionHandles["Action_Left"]).bState)   return "Action_Left";
                    if(NativeEntrypoints.SteamAPI_ISteamController_GetDigitalActionData(iSteamController, cH, actionHandles["Action_Right"]).bState)   return "Action_Right";
                    if(NativeEntrypoints.SteamAPI_ISteamController_GetDigitalActionData(iSteamController, cH, actionHandles["Menu_Select"]).bState)   return "Menu_Select";
                    if(NativeEntrypoints.SteamAPI_ISteamController_GetDigitalActionData(iSteamController, cH, actionHandles["Menu_Start"]).bState)   return "Menu_Start";

                    if(NativeEntrypoints.SteamAPI_ISteamController_GetDigitalActionData(iSteamController, cH, actionHandles["Quicksave"]).bState)   return "Quicksave";
                    if(NativeEntrypoints.SteamAPI_ISteamController_GetDigitalActionData(iSteamController, cH, actionHandles["Map"]).bState)   return "Map";
                    if(NativeEntrypoints.SteamAPI_ISteamController_GetDigitalActionData(iSteamController, cH, actionHandles["Character"]).bState)   return "Character";
                    if(NativeEntrypoints.SteamAPI_ISteamController_GetDigitalActionData(iSteamController, cH, actionHandles["Quest"]).bState)   return "Quest";
                    if(NativeEntrypoints.SteamAPI_ISteamController_GetDigitalActionData(iSteamController, cH, actionHandles["Water"]).bState)   return "Water";
                    if(NativeEntrypoints.SteamAPI_ISteamController_GetDigitalActionData(iSteamController, cH, actionHandles["Battery"]).bState)   return "Battery";
                    if(NativeEntrypoints.SteamAPI_ISteamController_GetDigitalActionData(iSteamController, cH, actionHandles["Antirad"]).bState)   return "Antirad";
                    if(NativeEntrypoints.SteamAPI_ISteamController_GetDigitalActionData(iSteamController, cH, actionHandles["PrevGun"]).bState)   return "PrevGun";
                    if(NativeEntrypoints.SteamAPI_ISteamController_GetDigitalActionData(iSteamController, cH, actionHandles["NextGun"]).bState)   return "NextGun";

                }
            }
            return "";
        }
        public static bool getSteamControllerAnalogMouse(out float joyX, out float joyY)
        {
            joyX = 0;
            joyY = 0;
            if(!SteamIsInitialized) return false;
            if(iSteamController == IntPtr.Zero || !steamControllerDetected)   return false;
            foreach(ulong cH in controllerHandle)
            {
                if(cH != 0)
                {
                     ControllerAnalogActionData_t actionMoveData = NativeEntrypoints.SteamAPI_ISteamController_GetAnalogActionData(iSteamController, cH, actionHandles["Action_Move"]);
                    joyX = actionMoveData.x;
                    joyY = actionMoveData.y;
                    return true;
                }
            }
            return false;
        }
        public static void initializeControllers()
        {
            EngineConsts.DebugLog("Initializing Steam Controller");
            if(iSteamController == IntPtr.Zero)
                iSteamController = SteamAPIInterop.SteamController();
            EngineConsts.DebugLog("iSteamController = " + iSteamController.ToString());
            if (iSteamController != IntPtr.Zero)
            {
                NativeEntrypoints.SteamAPI_ISteamController_Init(iSteamController);
                controllerHandle = new ulong[16];
                ISteamController_GetConnectedControllers(iSteamController, controllerHandle);
                foreach (ulong cH in controllerHandle)
                    if (cH != 0)
                        steamControllerDetected = true;
                initActionHandles();       
            }
        }
        private static void initActionHandles()
        {
            actionHandles = new Dictionary<string, ulong>();
            actionHandles.Add("Action_Move", NativeEntrypoints.SteamAPI_ISteamController_GetAnalogActionHandle(iSteamController, "Move"));    
            actionHandles.Add("Action_Left", NativeEntrypoints.SteamAPI_ISteamController_GetDigitalActionHandle(iSteamController, "Action_Left")); 
            actionHandles.Add("Action_Right", NativeEntrypoints.SteamAPI_ISteamController_GetDigitalActionHandle(iSteamController, "Action_Right")); 
            actionHandles.Add("Menu_Start", NativeEntrypoints.SteamAPI_ISteamController_GetDigitalActionHandle(iSteamController, "Menu_Start")); 
            actionHandles.Add("Menu_Select", NativeEntrypoints.SteamAPI_ISteamController_GetDigitalActionHandle(iSteamController, "Menu_Select")); 
            actionHandles.Add("Quicksave", NativeEntrypoints.SteamAPI_ISteamController_GetDigitalActionHandle(iSteamController, "Quicksave")); 
            actionHandles.Add("Map", NativeEntrypoints.SteamAPI_ISteamController_GetDigitalActionHandle(iSteamController, "Map")); 
            actionHandles.Add("Character", NativeEntrypoints.SteamAPI_ISteamController_GetDigitalActionHandle(iSteamController, "Character")); 
            actionHandles.Add("Quest", NativeEntrypoints.SteamAPI_ISteamController_GetDigitalActionHandle(iSteamController, "Quest")); 
            actionHandles.Add("Water", NativeEntrypoints.SteamAPI_ISteamController_GetDigitalActionHandle(iSteamController, "Water")); 
            actionHandles.Add("Battery", NativeEntrypoints.SteamAPI_ISteamController_GetDigitalActionHandle(iSteamController, "Battery")); 
            actionHandles.Add("Antirad", NativeEntrypoints.SteamAPI_ISteamController_GetDigitalActionHandle(iSteamController, "Antirad")); 
            actionHandles.Add("PrevGun", NativeEntrypoints.SteamAPI_ISteamController_GetDigitalActionHandle(iSteamController, "PrevGun")); 
            actionHandles.Add("NextGun", NativeEntrypoints.SteamAPI_ISteamController_GetDigitalActionHandle(iSteamController, "NextGun")); 

            actionSetHandles = new Dictionary<string, ulong>();
            actionSetHandles.Add("Controls", NativeEntrypoints.SteamAPI_ISteamController_GetActionSetHandle(iSteamController, "Controls")); 
            foreach(ulong cH in controllerHandle)
                if(cH != 0)  steamControllerDetected = true;
            EngineConsts.DebugLog("steamControllerDetected: " + steamControllerDetected);
        }

        private static string lastSet = "";
        public static void activateActionSet(string actionSet)
        {
            if(SteamIsInitialized && iSteamController != IntPtr.Zero && steamControllerDetected && lastSet != actionSet && actionSetHandles.ContainsKey(actionSet))
            {
                foreach(ulong cH in controllerHandle)
                    if(cH != 0)  {
                        NativeEntrypoints.SteamAPI_ISteamController_ActivateActionSet(iSteamController, cH, actionSetHandles[actionSet]);
                        lastSet = actionSet;
                    }
            }
        }
        public static string ControllerEnumToString(EControllerActionOrigin econtroller)
        {
            switch(econtroller) 
            {
                case EControllerActionOrigin.k_EControllerActionOrigin_None: return "";
                case EControllerActionOrigin.k_EControllerActionOrigin_A: return "button_a";
                case EControllerActionOrigin.k_EControllerActionOrigin_B: return "button_b";
                case EControllerActionOrigin.k_EControllerActionOrigin_X: return "button_x";
                case EControllerActionOrigin.k_EControllerActionOrigin_Y: return "button_y";
                case EControllerActionOrigin.k_EControllerActionOrigin_LeftBumper: return "shoulder_l";
                case EControllerActionOrigin.k_EControllerActionOrigin_RightBumper: return "shoulder_r";
                case EControllerActionOrigin.k_EControllerActionOrigin_LeftGrip: return "grip_l";
                case EControllerActionOrigin.k_EControllerActionOrigin_RightGrip: return "grip_r";
                case EControllerActionOrigin.k_EControllerActionOrigin_Start: return "button_start";
                case EControllerActionOrigin.k_EControllerActionOrigin_Back: return "button_select";
                case EControllerActionOrigin.k_EControllerActionOrigin_LeftPad_Touch: return "pad_l_touch";
                case EControllerActionOrigin.k_EControllerActionOrigin_LeftPad_Swipe: return "pad_l_swipe";
                case EControllerActionOrigin.k_EControllerActionOrigin_LeftPad_Click: return "pad_l_click";
                case EControllerActionOrigin.k_EControllerActionOrigin_LeftPad_DPadNorth: return "pad_l_dpad_n";
                case EControllerActionOrigin.k_EControllerActionOrigin_LeftPad_DPadSouth: return "pad_l_dpad_s";
                case EControllerActionOrigin.k_EControllerActionOrigin_LeftPad_DPadWest: return "pad_l_dpad_w";
                case EControllerActionOrigin.k_EControllerActionOrigin_LeftPad_DPadEast: return "pad_l_dpad_e";
                case EControllerActionOrigin.k_EControllerActionOrigin_RightPad_Touch: return "pad_r_touch";
                case EControllerActionOrigin.k_EControllerActionOrigin_RightPad_Swipe: return "pad_r_swipe";
                case EControllerActionOrigin.k_EControllerActionOrigin_RightPad_Click: return "pad_r_click";
                case EControllerActionOrigin.k_EControllerActionOrigin_RightPad_DPadNorth: return "pad_r_dpad_n";
                case EControllerActionOrigin.k_EControllerActionOrigin_RightPad_DPadSouth: return "pad_r_dpad_s";
                case EControllerActionOrigin.k_EControllerActionOrigin_RightPad_DPadWest: return "pad_r_dpad_w";
                case EControllerActionOrigin.k_EControllerActionOrigin_RightPad_DPadEast: return "pad_r_dpad_e";
                case EControllerActionOrigin.k_EControllerActionOrigin_LeftTrigger_Pull: return "trigger_l_pull";
                case EControllerActionOrigin.k_EControllerActionOrigin_LeftTrigger_Click: return "trigger_l_click";
                case EControllerActionOrigin.k_EControllerActionOrigin_RightTrigger_Pull: return "trigger_r_pull";
                case EControllerActionOrigin.k_EControllerActionOrigin_RightTrigger_Click: return "trigger_r_click";
                case EControllerActionOrigin.k_EControllerActionOrigin_LeftStick_Move: return "stick_move";
                case EControllerActionOrigin.k_EControllerActionOrigin_LeftStick_Click: return "stick_click";
                case EControllerActionOrigin.k_EControllerActionOrigin_LeftStick_DPadNorth: return "stick_dpad_n";
                case EControllerActionOrigin.k_EControllerActionOrigin_LeftStick_DPadSouth: return "stick_dpad_s";
                case EControllerActionOrigin.k_EControllerActionOrigin_LeftStick_DPadWest: return "stick_dpad_w";
                case EControllerActionOrigin.k_EControllerActionOrigin_LeftStick_DPadEast: return "stick_dpad_e";
                case EControllerActionOrigin.k_EControllerActionOrigin_Gyro_Move: return "gyro";
                case EControllerActionOrigin.k_EControllerActionOrigin_Gyro_Pitch: return "gyro";
                case EControllerActionOrigin.k_EControllerActionOrigin_Gyro_Yaw: return "gyro";
                case EControllerActionOrigin.k_EControllerActionOrigin_Gyro_Roll: return "gyro";
                case EControllerActionOrigin.k_EControllerActionOrigin_Count: return "";
            };
            return "";
        }

        //currently not used
        public static string getRawKeyByAction(string action)
        {
            if(!steamControllerDetected)
            {
//                if(action == "exitGame") return "button_select";
  
            } else {
                uint outOrigin = 0;
                foreach(ulong cH in controllerHandle)
                    if(cH != 0)  
                    {
  //                      if(action == "exitGame")  NativeEntrypoints.SteamAPI_ISteamController_GetDigitalActionOrigins(iSteamController, cH, actionSetHandles["InGameControls"], actionHandles["exitGame"], ref outOrigin);

//                        if(outOrigin != 0) return ControllerEnumToString((EControllerActionOrigin)outOrigin);
                    }
            }
            return "";
        } 

        public static string getSteamKeyForAction(string action)
        {
            return "";
        } 
        */

}
}