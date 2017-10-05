using UnityEngine;
using System.Runtime.InteropServices;

public class IOSLogger {

#if UNITY_IPHONE
    [DllImport("__Internal")]
    private static extern void logToiOS(string debugMessage);
#endif

    public static void Log(string logString, string stackTrace, LogType type) {
#if UNITY_IPHONE
        if (Application.platform == RuntimePlatform.IPhonePlayer) {
            logToiOS(logString + "\n===============\n" + stackTrace);
        }
#endif
    }
}