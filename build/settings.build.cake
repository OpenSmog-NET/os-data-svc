/**
 * File: settings.build.cake
 * Desc: General CAKE build system settings
 * Author: mmisztal1980
 */

/**
 * Tools
 */

// todo: Reenable this once vso-cake #30 is released
// #tool "nuget:?package=xunit.runner.console"

/**
 * Directories
 */

const string ArtifactsDir = "artifacts";
const string TestDir = "test";

/**
 * Targets
 */

const string Clean = "Clean";
const string Restore = "Restore";
const string Build = "Build";
const string Pack = "Pack";
const string Publish = "Publish";
const string UnitTests = "UnitTests";
const string IntegrationTests = "IntegrationTests";
const string ComponentTests = "ComponentTests";
const string FunctionalTests = "FunctionalTests";
const string UITests = "UITests";

/**
 * Commandline Arguments
 */
var @target          = Argument<string>("target", Build);            // CAKE Target
var @configuration   = Argument<string>("configuration", "Release"); // Build Configuration [Debug|Release]
var @platform        = Argument<string>("platform", "Any Cpu");      // Build Platform [x86|x64|Any Cpu]
var @branch          = Argument<string>("branch", null);             // The GIT branch name
var @buildNumber     = Argument<string>("buildNumber", null);        // The CI build number

/**
 * Auxiliaries
 */

var getTestProjects = new Func<string, IEnumerable<string>>((testType) => GetFiles($"./test/**/*.{testType}.csproj").Select(x => x.FullPath));
var getProjectsDirs = new Func<IEnumerable<string>, IEnumerable<string>>((paths) => paths.Select(x => $"./src/{x}"));

var getProjectName = new Func<string, string>(project => project.Split(new [] { "/" }, StringSplitOptions.RemoveEmptyEntries)
    .Last()
    .Replace(".csproj", string.Empty));

var canEmitArtifacts = new Func<string, bool>((branchName) => branchName != null && (branchName.Equals("dev") || branchName.Equals("master")));

var unitTests           = getTestProjects(UnitTests);
var integrationTests    = getTestProjects(IntegrationTests);
var componentTests      = getTestProjects(ComponentTests);
var functionalTests     = getTestProjects(FunctionalTests);
var uiTests             = getTestProjects(UITests);

var forEachPath = new Action<IEnumerable<string>, Func<string, string>, Action<string>>((files, map, action) => {
    foreach(var file in files) {
        if (map == null) action(file); else action(map(file));
    }
});

var publishFiles = new Action<string, Dictionary<string, string[]>>((app, includeFiles) => {
    if(includeFiles.ContainsKey(app)) {
        Information($"\nPublishing additional files for {app}:");

        foreach(var file in includeFiles[app]) {
            var from = $"./src/{app}/bin/{@configuration}/netcoreapp2.0/{file}";
            var to = $"{ArtifactsDir}/apps/{app}";

            Information($"\t{file} -> {to}");
            CopyFileToDirectory(from, to);
        }
    }
});

#load settings.netcore.cake
#load settings.webjobs.cake