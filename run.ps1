[string]$repoDir = "C:\Users\strza\source"
[string]$projDir = "C:\Users\strza\source\PowerShellToGraphVizGenerator"
[string]$package = "C:\Users\strza\source\PowerShellToGraphVizGenerator\PowerShell\PowerShellMigrationSitecorePackage\Content Package-1"
[string]$outDir = "$projDir\Published"
[string]$dot = "C:\Program Files (x86)\Graphviz2.38\bin\dot.exe"

Remove-Item $outDir -Recurse -Force > $null
New-Item -ItemType Directory -Path $outDir > $null

dotnet clean > $null
dotnet restore > $null
dotnet build > $null
dotnet publish $projDir\PowerShellToGraphViz.Generator\PowerShellToGraphViz.Generator.csproj `
    --configuration Release `
    --output $outDir `
    --verbosity quiet > $null

. $outDir\PowerShellToGraphViz.Generator.exe "$($package)\Script Library\TS_Migration\Functions"
. $dot -Tsvg "$outDir\SitecorePowerShellExtensionGraph.dot" -o "$repoDir\SitecorePowerShellExtensionGraph.svg"