//#if !UNITY_EDITOR

using UnityEngine;
using System;

namespace Instabug
{
    class InstabugInit
    {
#if UNITY_IOS || UNITY_IPHONE

        const string BetaToken = "<BETA_TOKEN>";
        const string LiveToken = "<LIVE_TOKEN>";

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeInstabugiOS()
        {
            // TODO: Remove the event once app closes
            Application.logMessageReceived += IOSLogger.Log;
        }

#elif UNITY_ANDROID

        const string BetaToken = "<BETA_TOKEN>";
        const string LiveToken = "<LIVE_TOKEN>";

        // TODO: All of this needs to go in an aar.
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeInstabugAndroid()
        {
            // Implementation follows Instabug documentation:
            //   https://docs.instabug.com/docs/android-integration
            // 
            // new Instabug.Builder(<application>, "<Token>").setInvocationEvent(<eventType>).build();
            // Instabug.setShakingThreshold(<threshold>);
            //

            using (AndroidJavaObject appObj = GetApplicationObject()) 
            {
                using (AndroidJavaClass instabugClass = new AndroidJavaClass("com/instabug/library/Instabug")) 
                {
                    AndroidJavaObject builderObj = new AndroidJavaObject("com/instabug/library/Instabug$Builder", appObj, BetaToken);

                    AndroidJavaClass eventEnum = new AndroidJavaClass("com/instabug/library/invocation/InstabugInvocationEvent");
                    AndroidJavaObject eventValue = eventEnum.GetStatic<AndroidJavaObject>("SHAKE");

                    builderObj = builderObj.Call<AndroidJavaObject>("setInvocationEvent", eventValue);
                    builderObj = builderObj.Call<AndroidJavaObject>("setShakingThreshold", 350);
                    builderObj.Call<AndroidJavaObject>("build");
                }
            }
        }

        private static AndroidJavaObject GetApplicationObject() 
        {
            AndroidJavaObject applicationObject = null;

            using (AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                AndroidJavaObject activityObj = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");
                applicationObject = activityObj.Call<AndroidJavaObject>("getApplication");
            }

            return applicationObject;
        }
#endif
    }
}

//#endif // UNITY_EDITOR