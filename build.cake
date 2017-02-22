// Tools
#tool "nuget:?package=GitVersion.CommandLine"

// Addins
#addin "Cake.Plist"
#addin "MagicChunks"

// Variables
var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

var buildDir = MakeAbsolute(Directory("./build"));

// Tasks
Task("Default")
    .Does(() => {
    });

// Manually bump version numbers with the help of GitVersion
Task("Version")
    .Does(() => {
        // AssemblyInfo
        var version = GitVersion(new GitVersionSettings { UpdateAssemblyInfo = true });
        Information($"AssemblyInfo Version set to {version.MajorMinorPatch}");

        var versionName = $"{version.Major}.{version.Minor}";

        // Android
        var versionCode = version.Major * 1000000 +
                          version.Minor *   10000;
        TransformConfig(@"./src/MDDevDaysApp/MDDevDaysApp.Droid/Properties/AndroidManifest.xml",
            new TransformationCollection {
                { "manifest/@android:versionName", versionName },
                { "manifest/@android:versionCode", versionCode.ToString() }
        });
        Information($"Android VersionName set to {versionName}");
        Information($"Android VersionCode set to {versionCode}");

        // iOS
        dynamic data = DeserializePlist("./src/MDDevDaysApp/MDDevDaysApp.iOS/Info.plist");
        data["CFBundleShortVersionString"] = versionName;
        data["CFBundleVersion"] = "0";
        SerializePlist("./src/MDDevDaysApp/MDDevDaysApp.iOS/Info.plist", data);

        Information($"iOS Version set to {versionName}");
        Information($"iOS Build set to 0");

        // UWP
        TransformConfig(@"./src/MDDevDaysApp/MDDevDaysApp.UWP/Package.appxmanifest",
            new TransformationCollection {
                { "Package/Identity/@Version", versionName + ".0.0" }
        });
        Information($"UWP Version set to {versionName}.0.0");
    });

Task("Clean")
    .Does(() => {
        CleanDirectory(buildDir);
    });

Task("Restore-NuGet-Packages")
    .IsDependentOn("Clean")
    .Does(() => {
        NuGetRestore("./src/MDDevDaysApp.sln");
    });

Task("BuildVersion")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() => {
        var currentVersionName = XmlPeek(@"./src/MDDevDaysApp/MDDevDaysApp.UWP/Package.appxmanifest", 
                                   "Package:Package/Package:Identity/@Version",
                                    new XmlPeekSettings{
                                        Namespaces = new Dictionary<string, string>{{"Package", "http://schemas.microsoft.com/appx/manifest/foundation/windows10" }}
                                });
        var versionParts = currentVersionName.Split('.');
        versionParts[2] = GitVersion().CommitsSinceVersionSource;
        var versionName = String.Join(".", versionParts);

        TransformConfig(@"./src/MDDevDaysApp/MDDevDaysApp.UWP/Package.appxmanifest",
            new TransformationCollection {
                { "Package/Identity/@Version", versionName }
        });

        Information($"UWP Version set to {versionName}");
    });

Task("UWP-CI-Package")
    .IsDependentOn("BuildVersion")
    .Does(() => {
        DotNetBuild("./src/MDDevDaysApp/MDDevDaysApp.UWP/MDDevDaysApp.UWP.csproj", configurator => {
            configurator.SetConfiguration(configuration)
                .SetVerbosity(Verbosity.Minimal)
                .WithTarget("Rebuild")
                .WithProperty("OutputPath", buildDir.Combine("UWP").Combine("bin").FullPath)
                .WithProperty("AppxPackageDir", buildDir.Combine("UWP").Combine("Package").FullPath)
                .WithProperty("AppxBundle", "Always")
                .WithProperty("AppxBundlePlatforms", "x86|x64|ARM")
                .WithProperty("BuildAppxUploadPackageForUap", "true")
                .WithProperty("UapAppxPackageBuildMode", "CI")
                .WithProperty("AppxSymbolPackageEnabled", "true");
        });

    });

// Run the target
RunTarget(target);
