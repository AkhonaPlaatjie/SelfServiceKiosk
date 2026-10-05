<#
.SYNOPSIS
    Creates the two GitHub rulesets that stop direct pushes:
      1. "Protect main"             - main can only change through a reviewed pull request
      2. "Protect learner branches" - learner/* can only change through a pull request

.NOTES
    Needs the GitHub CLI (https://cli.github.com) signed in as a repository admin:  gh auth login
    Run it ONCE, from inside the repository, after:
      - main has been pushed and the CI workflow has finished at least once
      - the learner branches have been created
#>

$rulesets = @(
    @{ Name = 'Protect main';             File = Join-Path $PSScriptRoot 'github/ruleset-main.json' },
    @{ Name = 'Protect learner branches'; File = Join-Path $PSScriptRoot 'github/ruleset-learner-branches.json' }
)

foreach ($ruleset in $rulesets) {
    Write-Host "Creating ruleset: $($ruleset.Name)"
    gh api --method POST 'repos/{owner}/{repo}/rulesets' --input $ruleset.File --jq '.name + " -> " + .enforcement'
    if ($LASTEXITCODE -ne 0) {
        Write-Error "Could not create the ruleset '$($ruleset.Name)'."
        exit 1
    }
}

Write-Host ""
Write-Host "Done. Review them on GitHub under Settings > Rules > Rulesets."
