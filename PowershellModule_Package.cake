#tool nuget:?package=NUnit.ConsoleRunner&version=3.7.0
#tool nuget:?package=Cake.CoreCLR&version=0.21.1
#addin "Cake.Powershell"

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var ConfigFile = Argument("ConfigFile","");
var BinaryVersion = Argument("BinaryVersion","");

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

// Define directories.
//The compiled module will be sent to modulePublishDir
var modulePublishDir = MakeAbsolute(Directory("./Octoposh/Publish/")).FullPath;

var ManifestPath = Directory(modulePublishDir) + Directory("Twinfield.Octoposh.psd1");

var ModuleNuspecPath = Directory(modulePublishDir) + File("Twinfield.Octoposh.nuspec");
var artifactsPath = MakeAbsolute(Directory("./Artifacts/")).FullPath;

var pathsToClean = new string[]{artifactsPath};

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")    
    .Does(() =>
{
    //Add the path to clean to pathstoClean
    foreach(var dir in pathsToClean){
      CleanDirectory(dir);  
    }
});


Task("Update-Module-Manifest")
	.IsDependentOn("Clean")
    .Description("Updates the module mainfest")   
    .Does(() =>
{    
    StartPowershellFile("Scripts/UpdateModuleManifest.ps1", new PowershellSettings()
        .SetFormatOutput()
        .SetLogOutput()
        .WithArguments(args=>
        {
            args.Append("Version",BinaryVersion);
            args.Append("ManifestPath",ManifestPath);            
        }));    
});


Task("Create-Package")
    .IsDependentOn("Update-Module-Manifest")
    .Description("Create PS module nuget package")   
    .Does(() =>
{
    var nuGetPackSettings   = new NuGetPackSettings {
		Version = BinaryVersion,
		OutputDirectory = artifactsPath
	};
	NuGetPack(ModuleNuspecPath, nuGetPackSettings);
       
});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Create-Package"); 

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);