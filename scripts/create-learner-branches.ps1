<#
.SYNOPSIS
    Creates one branch per learner on GitHub, each starting from the tip of main.
    Safe to run again: branches that already exist are skipped.

.EXAMPLE
    .\scripts\create-learner-branches.ps1
    .\scripts\create-learner-branches.ps1 -ListPath .\my-list.txt
    .\scripts\create-learner-branches.ps1 -BaseBranch develop
#>
param(
    [string]$ListPath = (Join-Path $PSScriptRoot 'learners.txt'),
    [string]$BaseBranch = 'main'
)

if (-not (Test-Path $ListPath)) {
    Write-Error "Learner list not found: $ListPath"
    exit 1
}

Write-Host "Fetching origin/$BaseBranch ..."
git fetch --quiet origin $BaseBranch
if ($LASTEXITCODE -ne 0) {
    Write-Error "Could not fetch origin/$BaseBranch. Has main been pushed yet?"
    exit 1
}

$created = 0
$skipped = 0

foreach ($line in Get-Content $ListPath) {
    # Drop comments and surrounding spaces.
    $name = ($line -replace '#.*$', '').Trim()
    if (-not $name) { continue }

    # "Thabo Mokoena" -> "thabo-mokoena"
    $slug = ($name.ToLowerInvariant() -replace '[^a-z0-9]+', '-').Trim('-')
    if (-not $slug) { continue }
    $branch = "learner/$slug"

    $existing = git ls-remote --heads origin $branch
    if ($existing) {
        Write-Host "skipped  $branch (already exists)"
        $skipped++
        continue
    }

    # Creates the branch on GitHub directly from origin/<base>; nothing is checked out locally.
    git push --quiet origin "refs/remotes/origin/${BaseBranch}:refs/heads/$branch"
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Could not create $branch."
        exit 1
    }

    Write-Host "created  $branch"
    $created++
}

Write-Host ""
Write-Host "Done: $created created, $skipped skipped."
