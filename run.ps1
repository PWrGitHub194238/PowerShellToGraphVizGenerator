[string]$projDir = (Get-Item -Path ".").FullName
[string]$powerShellDir = [IO.Path]::Combine($projDir, "Package")
[string]$projOutDir = [IO.Path]::Combine($projDir, "Published")
[string]$graphOutName = "output"

[string]$dot = "C:\Program Files\Graphviz\bin\dot.exe"

Remove-Item $projOutDir -Recurse -Force > $null
New-Item -ItemType Directory -Path $projOutDir > $null

dotnet clean $projDir
dotnet restore $projDir
dotnet build $projDir
dotnet publish $projDir\PowerShellToGraphViz.Generator\PowerShellToGraphViz.Generator.csproj `
    --configuration Release `
    --output $projOutDir

dotnet $projOutDir\PowerShellToGraphViz.Generator.dll "$projDir\$graphOutName.dot" "$powerShellDir"
. $dot -Tsvg "$projDir\$graphOutName.dot" -o "$projDir\$graphOutName.svg"