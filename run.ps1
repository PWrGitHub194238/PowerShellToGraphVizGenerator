[string]$repoDir = (Get-Item -Path ".\..").FullName
[string]$projDir = [IO.Path]::Combine($repoDir, 
    "VisualStudio\PowerShellToGraphVizGenerator")
[string]$sitecorePckageDir = [IO.Path]::Combine($repoDir, 
    "Sitecore\PowerShellMigrationSitecorePackage\Content Package-1")
[string]$projOutDir = [IO.Path]::Combine($projDir, "Published")
[string]$graphOutDir = [IO.Path]::Combine($repoDir, "Wiki")
[string]$graphOutName = "SitecorePowerShellExtensionGraph"

[string]$dot = "C:\Program Files (x86)\Graphviz2.38\bin\dot.exe"

Remove-Item $projOutDir -Recurse -Force > $null
New-Item -ItemType Directory -Path $projOutDir > $null

dotnet clean $projDir
dotnet restore $projDir
dotnet build $projDir
dotnet publish $projDir\PowerShellToGraphViz.Generator\PowerShellToGraphViz.Generator.csproj `
    --configuration Release `
    --output $projOutDir

dotnet $projOutDir\PowerShellToGraphViz.Generator.dll "$sitecorePckageDir"
. $dot -Tsvg "$projOutDir\SitecorePowerShellExtensionGraph.dot" -o "$graphOutDir\$graphOutName.svg"