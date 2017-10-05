# instabug-unity

An iOS & Android plugin for using [Instabug](http://instabug.com) with Unity.

### Setup
* Clone the [instabug-unity repository](https://github.com/LiveLike/instabug-unity) & add to your project.
* On iOS, the plugin includes an `OverrideAppDelegate.m` that overrides the Unity `AppDelegate` and initializes Instabug. If you use your own AppDelegate, you may need to merge the Instabug setup with your `AppDelegate`.

### Known Issues & Caveats
* The iOS changes require overriding Unity's prefix.pch file with our own prefix.pch. This is to ensure that Unity logs are included in all Instabug iOS log requests.
* On Android, screencapture & video capture is broken. It's been tough to fix because we're directly using the resources in `instabug.aar` rather than creating our own. So we have no control over what those resources do. Worst case, we might need to only use the `instabug.jar` and re-create the UI separately in another aar.

### Future Work
* Make Android plugin purely native instead of using JNI.
* Expose functionality for accessing the Instabug API directly from Unity