// Tools
#tool "nuget:?package=GitVersion.CommandLine"

// Addins
#addin "Cake.Plist"
#addin "Cake.Xamarin"
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
        TransformConfig(@"./src/MDDevDaysApp.Droid/Properties/AndroidManifest.xml",
            new TransformationCollection {
                { "manifest/@android:versionName", versionName },
                { "manifest/@android:versionCode", versionCode.ToString() }
        });
        Information($"Android VersionName set to {versionName}");
        Information($"Android VersionCode set to {versionCode}");

        // iOS
        dynamic data = DeserializePlist("./src/MDDevDaysApp.iOS/Info.plist");
        data["CFBundleShortVersionString"] = versionName;
        data["CFBundleVersion"] = "0";
        SerializePlist("./src/MDDevDaysApp.iOS/Info.plist", data);

        Information($"iOS Version set to {versionName}");
        Information($"iOS Build set to 0");

        // UWP
        TransformConfig(@"./src/MDDevDaysApp.UWP/Package.appxmanifest",
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
        NuGetRestore("./MDDevDaysApp.sln");
    });

Task("BuildVersion")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() => {
        var currentVersionName = XmlPeek(@"./src/MDDevDaysApp.UWP/Package.appxmanifest", 
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

        // Droid
        TransformConfig(@"./src/MDDevDaysApp.Droid/Properties/AndroidManifest.xml",
            new TransformationCollection {
                { "manifest/@android:versionCode", versionCode.ToString() }
        });
        Information($"Droid VersionCode set to {versionCode}");

        // iOS
        var plist = File("./src/MDDevDaysApp.iOS/Info.plist");
        dynamic data = DeserializePlist(plist);
        data["CFBundleVersion"] = versionParts[2];
        SerializePlist(plist, data);

        Information($"iOS Build Version set to {versionParts[2]}");
        
        // UWP
        TransformConfig(@"./src/MDDevDaysApp.UWP/Package.appxmanifest",
            new TransformationCollection {
                { "Package/Identity/@Version", versionName }
        });

        Information($"UWP Version set to {versionName}");
    });

Task("Droid-Clean")
    .IsDependentOn("BuildVersion")
    .Does(() => {
        DotNetBuild(@"./src/MDDevDaysApp.Droid/MDDevDaysApp.Droid.csproj", configurator => {
            configurator.SetConfiguration(configuration)
                .SetVerbosity(Verbosity.Minimal)
                .WithTarget("Clean");
        });
    });

Task("Droid-CI-Package")
    .IsDependentOn("Droid-Clean")
    .Does(() => {
        AndroidPackage(@"./src/MDDevDaysApp.Droid/MDDevDaysApp.Droid.csproj", true, configurator => {
            configurator.SetConfiguration(configuration)
                .SetVerbosity(Verbosity.Minimal)
                .WithProperty("OutputPath", buildDir.Combine("Droid").Combine("bin").FullPath);
        });
    });

Task("Droid-Store-Package")
    .IsDependentOn("Droid-Clean")
    .Does(() => {
        var keystore = EnvironmentVariable("KEYSTORE");
        if (keystore == null)
        {
            throw new Exception("You have to set the KEYSTORE environment variable");
        }

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

        AndroidPackage(@"./src/MDDevDaysApp.Droid/MDDevDaysApp.Droid.csproj", true, configurator => {
            configurator.SetConfiguration(configuration)
                .SetVerbosity(Verbosity.Minimal)
                .WithProperty("AndroidKeyStore", "true")
                .WithProperty("AndroidSigningKeyStore", keystore)
                .WithProperty("AndroidSigningStorePass", keystorePassword)
                .WithProperty("AndroidSigningKeyAlias", keyAlias)
                .WithProperty("AndroidSigningKeyPass", keyPassword)
                .WithProperty("OutputPath", buildDir.Combine("Droid").Combine("bin").FullPath);
        });
    });

Task("UWP-CI-Package")
    .IsDependentOn("BuildVersion")
    .Does(() => {
        DotNetBuild("./src/MDDevDaysApp.UWP/MDDevDaysApp.UWP.csproj", configurator => {
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
        DotNetBuild("./src/MDDevDaysApp.UWP/MDDevDaysApp.UWP.csproj", configurator => {
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

// Run the target
RunTarget(target);
