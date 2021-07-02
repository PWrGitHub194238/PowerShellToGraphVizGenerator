<#
    .SYNOPSIS
        Short description of Get-ExampleDependency2.
    .DESCRIPTION
        Long description.
    .PARAMETER DatabaseName
        Database ID.
    .EXAMPLE
        Import-Function -Module 'Module' -Name Get-ExampleDependency2
        [string]$item = Get-ExampleDependency2 `
            -DatabaseName 'master' `
            -Verbose
#>
function global:Get-ExampleDependency2
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