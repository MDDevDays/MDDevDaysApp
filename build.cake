// Tools
#tool "nuget:?package=GitVersion.CommandLine"

// Addins
#addin "Cake.HockeyApp"
#addin "Cake.Plist"
#addin "Cake.Xamarin"
#addin "MagicChunks"

// Variables
var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

var buildDir = MakeAbsolute(Directory("./build"));
var secretsDir = MakeAbsolute(Directory("./secrets"));

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

        var major = Convert.ToInt32(versionParts[0]);
        var minor = Convert.ToInt32(versionParts[1]);
        var build = Convert.ToInt32(versionParts[2]);
        var versionCode = major * 1000000 +
                          minor *   10000 +
                          build;

        TransformConfig(@"./src/MDDevDaysApp/MDDevDaysApp.Droid/Properties/AndroidManifest.xml",
            new TransformationCollection {
                { "manifest/@android:versionCode", versionCode.ToString() }
        });
        Information($"Droid VersionCode set to {versionCode}");

        TransformConfig(@"./src/MDDevDaysApp/MDDevDaysApp.UWP/Package.appxmanifest",
            new TransformationCollection {
                { "Package/Identity/@Version", versionName }
        });

        Information($"UWP Version set to {versionName}");
    });

Task("Droid-Clean")
    .IsDependentOn("BuildVersion")
    .Does(() => {
        DotNetBuild(@"./src/MDDevDaysApp/MDDevDaysApp.Droid/MDDevDaysApp.Droid.csproj", configurator => {
            configurator.SetConfiguration(configuration)
                .SetVerbosity(Verbosity.Minimal)
                .WithTarget("Clean");
        });
    });

Task("Droid-CI-Package")
    .IsDependentOn("Droid-Clean")
    .Does(() => {
        AndroidPackage(@"./src/MDDevDaysApp/MDDevDaysApp.Droid/MDDevDaysApp.Droid.csproj", true, configurator => {
            configurator.SetConfiguration(configuration)
                .SetVerbosity(Verbosity.Minimal)
                .WithProperty("OutputPath", buildDir.Combine("Droid").Combine("bin").FullPath);
        });
    });

Task("Droid-Store-Package")
    .IsDependentOn("Droid-Clean")
    .Does(() => {
        var keystorePassword = EnvironmentVariable("KEYSTORE_PASSWORD");
        if (keystorePassword == null)
        {
            throw new Exception("You have to set the KEYSTORE_PASSWORD environment variable");
        }

        var keyAlias = EnvironmentVariable("KEY_ALIAS");
        if (keyAlias == null)
        {
            throw new Exception("You have to set the KEY_ALIAS environment variable");
        }

        var keyPassword = EnvironmentVariable("KEY_PASSWORD");
        if (keyPassword == null)
        {
            throw new Exception("You have to set the KEY_PASSWORD environment variable");
        }

        AndroidPackage(@"./src/MDDevDaysApp/MDDevDaysApp.Droid/MDDevDaysApp.Droid.csproj", true, configurator => {
            configurator.SetConfiguration(configuration)
                .SetVerbosity(Verbosity.Minimal)
                .WithProperty("AndroidKeyStore", "true")
                .WithProperty("AndroidSigningKeyStore", secretsDir.Combine("MDDevDays.keystore").FullPath)
                .WithProperty("AndroidSigningStorePass", keystorePassword)
                .WithProperty("AndroidSigningKeyAlias", EnvironmentVariable("KEY_ALIAS"))
                .WithProperty("AndroidSigningKeyPass", EnvironmentVariable("KEY_PASSWORD"))
                .WithProperty("OutputPath", buildDir.Combine("Droid").Combine("bin").FullPath);
        });
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

Task("UWP-Store-Package")
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
                .WithProperty("UapAppxPackageBuildMode", "StoreUpload")
                .WithProperty("AppxSymbolPackageEnabled", "true");
        });
    });

Task("UWP-UploadToHockeyApp")
    .IsDependentOn("UWP-Store-Package")
    .Does(() => {
        var appxBundle = GetFiles("./build/UWP/Package/*/*.appxbundle").First();
        var appxSym = GetFiles("./build/UWP/Package/*/*.appxsym").First();
        var versionName = XmlPeek(@"./src/MDDevDaysApp/MDDevDaysApp.UWP/Package.appxmanifest", 
                                   "Package:Package/Package:Identity/@Version",
                                   new XmlPeekSettings{
                                       Namespaces = new Dictionary<string, string>{{"Package", "http://schemas.microsoft.com/appx/manifest/foundation/windows10" }}
                                 });
        var versionParts = versionName.Split('.');

        UploadToHockeyApp(appxBundle, new HockeyAppUploadSettings {
            AppId = "7538aec68c2c478aaf6aba693a96ab68",
            Version = versionName,
            ShortVersion = $"{versionParts[0]}.{versionParts[1]}",
            Notes = "Uploaded via Continuous Integration",
            Notify = NotifyOption.AllTesters,
            Status = DownloadStatus.Allowed
        }, appxSym);
    });

// Run the target
RunTarget(target);