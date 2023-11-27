# Run post-provisioning tasks
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
    Write-Output "    This runs the post-provisioning tasks

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

$rg = "rg-$env:AZURE_ENV_NAME"

# Provision AOAI instances
Write-Output "Provisioning Azure OpenAI instance ..."

$provisioned = az deployment group create -n aoai -g $rg `
    --template-file ./biceps/openAI.bicep `
    --parameters environmentName=$env:AZURE_ENV_NAME | ConvertFrom-Json

Write-Output "... Provisioned"

$openAI = $provisioned.properties.outputs.aoaiInstance.value | `
    Select-Object -Property @{ Name = "Endpoint"; Expression = "endpoint" }, @{ Name = "ApiKey"; Expression = "apiKey" }, @{ Name = "DeploymentId"; Expression = "deploymentName" }

# Get resource token
Write-Output "Updating Azure Storage ..."

$token = az deployment group create -n resourceToken -g $rg `
    --template-file ./biceps/resourceGroup.bicep | ConvertFrom-Json

$token = $token.properties.outputs.resourceToken.value

# Get connection string
$connectionString = az storage account show-connection-string -g $rg -n storage$token --query "connectionString" -o tsv

# Add tables to table storage
$queue = az storage queue create -n summaries --connection-string $connectionString
$table = az storage table create -n videos --connection-string $connectionString

Write-Output "... Updated"

# Update appsettings.Development.json
Write-Output "Updating appsettings.Development.json ..."

Copy-Item -Path ./AspireYouTubeSummariser.AppHost/appsettings.Development.sample.json `
          -Destination ./AspireYouTubeSummariser.AppHost/appsettings.Development.json -Force

$appsettings = Get-Content -Path ./AspireYouTubeSummariser.AppHost/appsettings.Development.json | ConvertFrom-Json
$appsettings.ConnectionStrings.queue = $connectionString
$appsettings.ConnectionStrings.table = $connectionString
$appsettings.OpenAI = $openAI
$appsettings | ConvertTo-Json -Depth 100 | Out-File -Path ./AspireYouTubeSummariser.AppHost/appsettings.Development.json -Force

Write-Output "... Updated"
