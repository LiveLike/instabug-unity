#if UNITY_IPHONE

using UnityEngine;
using System.Runtime.InteropServices;

public class IOSLogger {

    [DllImport("__Internal")]
    private static extern void logToiOS(string debugMessage);

    public static void Log(string logString, string stackTrace, LogType type) {
        if (Application.platform == RuntimePlatform.IPhonePlayer) {
            logToiOS(logString + "\n===============\n" + stackTrace);
        }
    }
}

#endif