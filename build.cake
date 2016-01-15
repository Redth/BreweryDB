#addin "Cake.FileHelpers"

var TARGET = Argument ("target", Argument ("t", "Default"));

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
		Verbosity = NuGetVerbosity.Detailed,
		OutputDirectory = "./nupkg/",
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
