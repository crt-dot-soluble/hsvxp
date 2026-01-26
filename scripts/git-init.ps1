if (Test-Path ".git") {
  Write-Output "Git already initialized."
  exit 0
}

git init

Write-Output "Git initialized. Configure remote as required by policy."
