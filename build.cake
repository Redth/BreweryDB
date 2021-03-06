#addin "Cake.FileHelpers"

var TARGET = Argument ("target", Argument ("t", "Default"));

var version = EnvironmentVariable ("APPVEYOR_BUILD_VERSION") ?? Argument("version", "0.0.9999");

Task ("Default").Does (() =>
{
	NuGetRestore ("./BreweryDB/BreweryDB.sln");

	DotNetBuild ("./BreweryDB/BreweryDB.sln", c => c.Configuration = "Release");
});

Task ("NuGetPack")
	.IsDependentOn ("Default")
	.Does (() =>
{
	NuGetPack ("./BreweryDB.nuspec", new NuGetPackSettings { 
		Version = version,
		Verbosity = NuGetVerbosity.Detailed,
		OutputDirectory = "./",
		BasePath = "./",
	});	
});

Task ("InjectKeys").Does (() =>
{
	// Get the API Key from the Environment variable
	var breweryDbApiKey = EnvironmentVariable ("BREWERY_DB_API_KEY") ?? "";

	// Replace the placeholder in our Keys.cs files
	ReplaceTextInFiles ("./**/Keys.cs", "{BREWERY_DB_API_KEY}", breweryDbApiKey);
});

RunTarget (TARGET);
