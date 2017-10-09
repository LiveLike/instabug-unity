# instabug-unity

An iOS & Android plugin for using [Instabug](http://instabug.com) with Unity.

### Setup
* Clone the [instabug-unity repository](https://github.com/LiveLike/instabug-unity) & add to your project.
* The plugin Unity's `RuntimeInitializeOnLoadMethod` to initialize the plugin. So there shouldn't be anything else you need to do.

### Known Issues & Caveats
* The iOS changes require overriding Unity's prefix.pch file with our own prefix.pch. This is to ensure that iOS NSLogs are included in all Instabug iOS log requests. See [Instabug iOS Logging Documentation](https://docs.instabug.com/docs/ios-logging) for more info.
* On Android, screencapture & video capture is broken. It's been tough to fix because we're directly using the resources in `instabug.aar` rather than creating our own. So we have no control over what those resources do. Worst case, we might need to only use the `instabug.jar` and re-create the UI separately in another aar.
* On Android, we're using Instabug v4.0.3 because the structure of the jar after that version doesn't work in Unity. (Results in `ClassNotfoundexception`).

### Future Work
* Make Android plugin purely native instead of using JNI.
* Expose functionality for accessing the Instabug API directly from Unity
