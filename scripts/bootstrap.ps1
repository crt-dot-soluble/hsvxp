param(
  [Parameter(Mandatory=$false)][string]$Mode = "defaults",
  [Parameter(Mandatory=$false)][string]$VersionControl = "git-remote-ci",
  [Parameter(Mandatory=$false)][string]$Testing = "full",
  [Parameter(Mandatory=$false)][string]$Documentation = "inline",
  [Parameter(Mandatory=$false)][string]$Language = "unspecified",
  [Parameter(Mandatory=$false)][string]$Frameworks = "",
  [Parameter(Mandatory=$false)][string]$Autonomy = "feature"
)

$allowedVersionControl = @("git-local", "git-remote", "git-remote-ci")
$allowedTesting = @("full", "baseline")
$allowedDocumentation = @("inline", "comments-only", "generate")
$allowedAutonomy = @("feature", "milestone", "fully-autonomous")

if ($Mode -ne "customize") {
  $VersionControl = "git-remote-ci"
  $Testing = "full"
  $Documentation = "inline"
  $Language = "unspecified"
  $Frameworks = ""
  $Autonomy = "feature"
}

if ($Frameworks -eq "None") { $Frameworks = "" }

$readmePath = Join-Path $PSScriptRoot "..\README.md"
if (Test-Path $readmePath) {
  $readmeContent = Get-Content $readmePath -Raw
  if ($readmeContent -match "AI AGENTS MUST IGNORE THIS FILE") {
    Remove-Item $readmePath -Force
    Write-Output "Removed default README.md"
  }
}

if (-not ($allowedVersionControl -contains $VersionControl)) { throw "Invalid VersionControl" }
if (-not ($allowedTesting -contains $Testing)) { throw "Invalid Testing" }
if (-not ($allowedDocumentation -contains $Documentation)) { throw "Invalid Documentation" }
if (-not ($allowedAutonomy -contains $Autonomy)) { throw "Invalid Autonomy" }

$frameworkList = @()
if ($Frameworks -ne "") {
  $frameworkList = $Frameworks.Split(",") | ForEach-Object { $_.Trim() } | Where-Object { $_ -ne "" }
}

$policy = [ordered]@{
  version = "1.0.0"
  policyGeneratedBy = "bootstrap"
  bootstrap = [ordered]@{
    mode = $Mode
    skipped = ($Mode -ne "customize")
    timestamp = (Get-Date -Format "yyyy-MM-dd")
  }
  versionControl = $VersionControl
  testing = $Testing
  documentation = $Documentation
  language = [ordered]@{
    primary = $Language
    frameworks = $frameworkList
  }
  autonomy = $Autonomy
  phases = [ordered]@{
    required = $true
    list = @(0,1,2,3,4,5)
  }
  ciCdEnforced = $true
  remoteRequired = ($VersionControl -ne "git-local")
}

$policyPath = Join-Path $PSScriptRoot "..\governance.config.json"
$policy | ConvertTo-Json -Depth 6 | Out-File -FilePath $policyPath -Encoding UTF8
Write-Output "Wrote governance policy to $policyPath"
