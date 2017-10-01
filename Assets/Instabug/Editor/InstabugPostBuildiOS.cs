using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Instabug.Internal.Editor.Postbuild {

    public class InstabugPostBuildiOS {

        private const string InstabugPluginPath = "Plugins/iOS/Instabug";
        private const string InstabugFrameworkName = "Instabug.framework";
        private const string BuildPhaseName = "Embed Instabug Framework";

        private struct PermissionsData {
            public string key;
            public string value;
        }

        private static PermissionsData[] permissionsDatas = new PermissionsData[] {
            new PermissionsData() {
                key = "NSMicrophoneUsageDescription", 
                value = "App needs access to the microphone to be able to attach voice notes."
            },
            new PermissionsData() {
                key = "NSPhotoLibraryUsageDescription", 
                value = "App needs access to your photo library for you to be able to attach images."
            }
        };

        [PostProcessBuild(100)]
        public static void OnPostprocessBuild(BuildTarget buildTarget, string buildPath) {
            // BuiltTarget.iOS is not defined in Unity 4, so we just use strings here
            if (buildTarget.ToString() == "iOS" || buildTarget.ToString() == "iPhone") {
                PrepareProject(buildPath);
                PreparePlist(buildPath);
            }
        }

        private static void PrepareProject(string buildPath) {
            string projPath = PBXProject.GetPBXProjectPath(buildPath);

            AddEmbeddedBinaries(projPath);
            AddRunScriptBuildPhase(projPath);
        }

        private static void AddEmbeddedBinaries(string projPath) {
            PBXProject project = new PBXProject();
            project.ReadFromFile(projPath);

            string targetName = PBXProject.GetUnityTargetName();
            string targetGuid = project.TargetGuidByName(targetName);

            if (EmbeddedBinariesNotAdded(project)) {
                AddEmbeddedFrameworks(project, targetGuid);
                AddCodeSignOnCopy(project, BuildPhaseName);
                AddSearchPaths(project, targetGuid);

                File.WriteAllText(projPath, project.WriteToString());
            }
        }

        private static bool EmbeddedBinariesNotAdded(PBXProject project) {
            // TODO: Need a better way to do this
            string projectContents = project.WriteToString();
            return projectContents.Contains(BuildPhaseName);
        }

        private static void AddEmbeddedFrameworks(PBXProject project, string targetGuid) {
            string frameworkRelativePath = Path.Combine(InstabugPluginPath, InstabugFrameworkName);
            string frameworkAbsolutePath = Path.Combine(Application.dataPath, frameworkRelativePath);

            string frameworkGuid = project.AddFile(frameworkAbsolutePath, "Frameworks/" + InstabugFrameworkName, PBXSourceTree.Source);
            project.AddFileToBuild(targetGuid, frameworkGuid);

            string embedPhaseGuid = project.AddCopyFilesBuildPhase(targetGuid, BuildPhaseName, "", "10" /* Frameworks */);
            project.AddFileToBuildSection(targetGuid, embedPhaseGuid, frameworkGuid);

            // HACK: Use the correct path in the project. Not sure why it's using the absolute path.
            string projectContents = project.WriteToString();
            projectContents = projectContents.Replace(frameworkAbsolutePath, "Frameworks/" + frameworkRelativePath);
            project.ReadFromString(projectContents);
        }

        // TODO: This needs to be done via the xcode API once functionality is available
        private static void AddCodeSignOnCopy(PBXProject project, string phaseName) {

            string projectContents = project.WriteToString();
            string pattern = "(?<=" + phaseName + ")(?:.*)(\\/\\* Instabug\\.framework \\*\\/)(?=; };)";
            MatchEvaluator matchEvaluator = m => m.Value.Replace("/* " + InstabugFrameworkName + " */",
                "/* " + InstabugFrameworkName + " */; settings = {ATTRIBUTES = (CodeSignOnCopy, ); }");

            projectContents = Regex.Replace(projectContents, pattern, matchEvaluator);

            project.ReadFromString(projectContents);
        }

        private static void AddSearchPaths(PBXProject project, string targetGuid) {
            project.AddBuildProperty(targetGuid, "FRAMEWORK_SEARCH_PATHS", "$(SRCROOT)/Frameworks/" + InstabugPluginPath);
            project.AddBuildProperty(targetGuid, "LD_RUNPATH_SEARCH_PATHS", "$(inherited) @executable_path/Frameworks");
        }

        private static void AddRunScriptBuildPhase(string projPath) {
            string[] xcodeProjectLines = File.ReadAllLines(projPath);

            if (ScriptNotSetup(xcodeProjectLines)) {
                SetupShellScript(projPath, xcodeProjectLines);
            }
        }

        private static bool ScriptNotSetup(string[] xcodeProjectLines) {
            bool notSetup = true;

            foreach (string line in xcodeProjectLines) {
                if (line.Contains("Instabug.bundle/strip-frameworks.sh")) {
                    notSetup = false;
                    break;
                }
            }

            return notSetup;
        }

        private static void SetupShellScript(string projPath, string[] xcodeProjectLines) {
            
            string scriptUUID = System.Guid.NewGuid().ToString("N").Substring(0, 24).ToUpper();
            bool inBuildPhases = false;
            StringBuilder sb = new StringBuilder();

            bool hasScriptBuildPhase = false;
            foreach (string line in xcodeProjectLines) {
                if (line.Contains("/* Begin PBXShellScriptBuildPhase section */")) {
                    hasScriptBuildPhase = true;
                }
            }

            string shellScript = "bash \\\"${BUILT_PRODUCTS_DIR}/${FRAMEWORKS_FOLDER_PATH}/" +
                "Instabug.framework/Instabug.bundle/strip-frameworks.sh\\\"";

            string shellScriptLines = 
                "\t\t" + scriptUUID + " /* ShellScript */ = {\n" +
                "\t\t\tisa = PBXShellScriptBuildPhase;\n" +
                "\t\t\tbuildActionMask = 2147483647;\n" +
                "\t\t\tfiles = (\n" +
                "\t\t\t);\n" +
                "\t\t\tinputPaths = (\n" +
                "\t\t\t);\n" +
                "\t\t\toutputPaths = (\n" +
                "\t\t\t);\n" +
                "\t\t\trunOnlyForDeploymentPostprocessing = 1;\n" +
                "\t\t\tshellPath = \"/bin/sh -x\";\n" +
                "\t\t\tshellScript = \"" + shellScript + "\";\n" +
                "\t\t};\n";

            foreach (string line in xcodeProjectLines) {
                if (hasScriptBuildPhase && line.Contains("/* Begin PBXShellScriptBuildPhase section */")) {
                    sb.AppendLine(line);               
                    sb.Append(shellScriptLines);
                } else if (!hasScriptBuildPhase && line.Contains("/* End PBXResourcesBuildPhase section */")) {
                    sb.AppendLine(line);
                    sb.AppendLine("/* Begin PBXShellScriptBuildPhase section */");
                    sb.Append(shellScriptLines);
                    sb.AppendLine("/* End PBXShellScriptBuildPhase section */");
                } else if (line.Contains("buildPhases = (")) {
                    inBuildPhases = true;
                    sb.AppendLine(line);
                } else if (inBuildPhases && line.Contains(");")) {
                    inBuildPhases = false;
                    sb.AppendLine("\t\t\t\t" + scriptUUID + " /* ShellScript */,");
                    sb.AppendLine(line);
                } else {
                    sb.AppendLine(line);
                }
            }

            File.WriteAllText(projPath, sb.ToString());
        }

        private static void PreparePlist(string buildPath) {
            AddInstabugInfoToPlist(buildPath);
        }

        private static void AddInstabugInfoToPlist(string buildPath) {
            string plistPath = Path.Combine(buildPath, "Info.plist");

            PlistDocument plist = new PlistDocument();
            plist.ReadFromFile(plistPath);

            PlistElementDict rootDict = plist.root.AsDict();

            foreach (PermissionsData perms in permissionsDatas) {
                AddToPlist(rootDict, perms.key, perms.value);
            }

            plist.WriteToFile(plistPath);
        }

        private static void AddToPlist(PlistElementDict parent, string key, string value) {
            PlistElementString retrievedVal = (PlistElementString) parent[key];
            if (retrievedVal == null) {
                parent.SetString(key, value);
            }
        }
    }
}
