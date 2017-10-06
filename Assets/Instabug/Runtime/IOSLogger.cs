#if (UNITY_IOS || UNITY_IPHONE) && !UNITY_EDITOR

using UnityEngine;
using System.Runtime.InteropServices;

public class IOSLogger {

    [DllImport("__Internal")]
    private static extern void logToiOS(string debugMessage);

    public static void Log(string logString, string stackTrace, LogType type) {
        logToiOS(logString + "\n===============\n" + stackTrace);
    }
}

#endif