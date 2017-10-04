#if !UNITY_EDITOR

using UnityEngine;
using System;

namespace Instabug
{
    class Setup
    {
        const string BetaToken = "BETA_TOKEN";
        const string LiveToken = "LIVE_TOKEN";

#if UNITY_IOS || UNITY_IPHONE

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeInstabugiOS()
        {

        }

#elif UNITY_ANDROID

        // TODO: All of this needs to go in an aar.
        //       Need to figure out how to export an aar that can reference the instabug.aar
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void InitializeInstabugAndroid()
        {
            // Implementation follows Instabug documentation:
            //   https://docs.instabug.com/docs/android-integration
            // 
            // new Instabug.Builder(<application>, "<Token>").setInvocationEvent(<eventType>).build();
            // 

            using (AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) 
            {
                using (AndroidJavaObject activityObj = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity")) 
                {
                    using (AndroidJavaObject appObj = activityObj.Call<AndroidJavaObject>("getApplication")) 
                    {
                        using (AndroidJavaObject builderObj = new AndroidJavaObject("com/instabug/library/Instabug$Builder", appObj, BetaToken))
                        {
                            builderObj.Call<AndroidJavaObject>("build");
                        }
                    }
                }
            }
        }

        private static AndroidJavaObject GetApplicationObject() 
        {
            AndroidJavaObject applicationObject = null;

            using (AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) {
                AndroidJavaObject activityObj = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity");
                applicationObject = activityObj.Call<AndroidJavaObject>("getApplication");
            }

            return applicationObject;
        }
#endif
    }
}

#endif // UNITY_EDITOR