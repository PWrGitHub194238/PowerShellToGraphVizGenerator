<#
    .SYNOPSIS
        Short description of Get-ExampleDependency1.
    .DESCRIPTION
        Long description.
    .PARAMETER DatabaseName
        Database ID.
    .EXAMPLE
        Import-Function -Module 'Module' -Name Get-ExampleDependency1
        [string]$item = Get-ExampleDependency1 `
            -DatabaseName 'master' `
            -Verbose
#>
function global:Get-ExampleDependency1
{
    [CmdletBinding(PositionalBinding = $false, DefaultParameterSetName = 'Template')]
    [OutputType([Sitecore.Data.Items.Item])]
    param (
		[Parameter(Mandatory = $true)]
        [ValidateNotNullOrEmpty()]
        [string]$DatabaseName
    )
	
	return $DatabaseName
}