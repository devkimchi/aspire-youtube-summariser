# Set variables for GitHub Actions
Param(
    [string]
    [Parameter(Mandatory=$false)]
    $GitHubAlias = $null,

    [string]
    [Parameter(Mandatory=$false)]
    $GitHubRepository = "aspire-youtube-summariser",

    [switch]
    [Parameter(Mandatory=$false)]
    $Help
)

function Show-Usage {
    Write-Output "    This sets variables for GitHub Actions

    Usage: $(Split-Path $MyInvocation.ScriptName -Leaf) ``
            [-GitHubAlias      <GitHub alias>] ``
            [-GitHubRepository <GitHub repository name>] ``

            [-Help]

    Options:
        -GitHubAlias        GitHub username. eg) 'devkimchi' of https://github.com/devkimchi. Default is `$null.
        -GitHubRepository   GitHub repository name. Default is 'aspire-youtube-summariser'.

        -Help               Show this message.
"

    Exit 0
}

# Show usage
$needHelp = $Help -eq $true
if ($needHelp -eq $true) {
    Show-Usage
    Exit 0
}

if ($GitHubAlias -ne $null) {
    $vars = gh api repos/$GitHubAlias/$GitHubRepository/actions/variables | ConvertFrom-Json
    $env:AZURE_ENV_NAME = $($vars.variables | Where-Object { $_.name -eq "AZURE_ENV_NAME" }).value
}

gh variable set -f ./.azure/$env:AZURE_ENV_NAME/.env
