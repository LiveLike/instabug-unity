#if (UNITY_IOS || UNITY_IPHONE) && !UNITY_EDITOR

using UnityEngine;
using System;
using System.Runtime.InteropServices;

namespace Instabug
{
    class InstabugInitiOS
    {
        const string BetaToken = "<BETA_TOKEN>";
        const string LiveToken = "<LIVE_TOKEN>";

        [DllImport ("__Internal")]
        private static extern void initInstabug(string BetaToken, string LiveToken);

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            initInstabug(BetaToken, LiveToken);

            // TODO: Remove the event once app closes
            Application.logMessageReceived += IOSLogger.Log;
        }
    }
}

#endif