# Paths
$packFolder = (Get-Item -Path "./" -Verbose).FullName
$slnPath = Join-Path $packFolder "../"
$srcPath = Join-Path $slnPath "src"

# List of projects
$projects = (
    "Kontecg",
    "Kontecg.AutoMapper",
    "Kontecg.BlobStoring",
    "Kontecg.BlobStoring.FileSystem",
    "Kontecg.Castle.Log4Net",
    "Kontecg.Castle.MsAdapter",
    "Kontecg.Castle.MsLogging",
    "Kontecg.Dapper",
    "Kontecg.EntityFrameworkCore",
	"Kontecg.EntityFrameworkCore.EFPlus",
	"Kontecg.FluentValidation",
    "Kontecg.HangFire",
    "Kontecg.MailKit",
    "Kontecg.MassTransit",
	"Kontecg.MemoryDb",
	"Kontecg.ML",
    "Kontecg.MongoDB",
    "Kontecg.Net",
    "Kontecg.Quartz",
    "Kontecg.Baseline",
    "Kontecg.Baseline.Ldap",
    "Kontecg.Baseline.EntityFrameworkCore",
    "Kontecg.RedisCache",
	"Kontecg.TestBase",
    "Kontecg.Workflows",
    "Kontecg.Workflows.EntityFrameworkCore"
)

# Rebuild solution
Set-Location $slnPath
& dotnet restore

# Copy all nuget packages to the pack folder
foreach($project in $projects) {

    $projectFolder = Join-Path $srcPath $project

    # Create nuget pack
    Set-Location $projectFolder
    Get-ChildItem (Join-Path $projectFolder "bin/Release") -ErrorAction SilentlyContinue | Remove-Item -Recurse
    & dotnet msbuild /p:Configuration=Release /p:RuntimeIdentifiers=win-x64
    & dotnet msbuild /p:Configuration=Release /p:RuntimeIdentifiers=win-x64 /t:pack /p:IncludeSymbols=true /p:SymbolPackageFormat=snupkg

    # Copy nuget package
    $projectPackPath = Join-Path $projectFolder ("/bin/Release/" + $project + ".*.nupkg")
    Move-Item $projectPackPath $packFolder

	# Copy symbol package
    $projectPackPath = Join-Path $projectFolder ("/bin/Release/" + $project + ".*.snupkg")
    Move-Item $projectPackPath $packFolder
}

# Go back to the pack folder
Set-Location $packFolder