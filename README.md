# instabug-unity

An iOS & Android plugin for using [Instabug](http://instabug.com) with Unity.

### Setup
* Clone the [instabug-unity repository](https://github.com/LiveLike/instabug-unity) & add to your project.
* On iOS, the plugin includes an `OverrideAppDelegate.m` that overrides the Unity `AppDelegate` and initializes Instabug. If you use your own AppDelegate, you may need to merge the Instabug setup with your `AppDelegate`.

### Future Work
* Create the Android Plugin
* Expose functionality for accessing the Instabug API directly from Unity