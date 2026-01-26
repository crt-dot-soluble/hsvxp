$policyPath = Join-Path $PSScriptRoot "..\governance.config.json"
if (-not (Test-Path $policyPath)) { throw "Missing governance.config.json" }

$required = @(
  "version", "policyGeneratedBy", "bootstrap", "versionControl", "testing",
  "documentation", "language", "autonomy", "phases", "ciCdEnforced", "remoteRequired"
)

$policy = Get-Content $policyPath -Raw | ConvertFrom-Json
$missing = @()
foreach ($key in $required) {
  if (-not ($policy.PSObject.Properties.Name -contains $key)) {
    $missing += $key
  }
}

if ($missing.Count -gt 0) {
  throw "Missing policy keys: $($missing -join ', ')"
}

Write-Output "Policy validated"
