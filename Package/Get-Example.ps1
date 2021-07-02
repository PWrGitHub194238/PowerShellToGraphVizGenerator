Import-Function -Module 'Module' -Name Get-ExampleDependency1
Import-Function -Module 'Module' -Name Get-ExampleDependency2
<#
    .SYNOPSIS
        Short description of Get-Example.
    .DESCRIPTION
        Long description.
    .PARAMETER DatabaseName
        Database ID.
    .PARAMETER Language
        Language (short format ex. 'en', 'fr-FR').
    .PARAMETER SubitemsSorting
        Sorting
    .EXAMPLE
        Import-Function -Module 'Module' -Name Get-Example
        [string]$item = Get-Example `
            -DatabaseName 'master' `
            -Language 'en' `
			-Sorting 'Created `
            -Verbose
#>
function global:Get-Example
{
    [CmdletBinding(PositionalBinding = $false, DefaultParameterSetName = 'Template')]
    [OutputType([Sitecore.Data.Items.Item])]
    param (
		[Parameter(Mandatory = $true)]
        [ValidateNotNullOrEmpty()]
        [string]$DatabaseName
	,
        [Parameter(Mandatory = $true)]
        [ValidateNotNullOrEmpty()]
        [Sitecore.Globalization.Language]$Language
    ,
        [Parameter(Mandatory = $false)]
        [ValidateNotNullOrEmpty()]
		[ValidateSet('Created', 'Default', 'Display name', 'Logical', 'Reverse', 'Updated')]
        [string]$SubitemsSorting = 'Created'
    )
	
	[string]$result1 = Get-ExampleDependency1 `
		-DatabaseName $DatabaseName
		
	[string]$result2 = Get-ExampleDependency2 `
		-DatabaseName $DatabaseName
	
	return $DatabaseName + $result1 + $result2
}