#!/usr/bin/env bash
#
# Creates the two GitHub rulesets that stop direct pushes:
#   1. "Protect main"             - main can only change through a reviewed pull request
#   2. "Protect learner branches" - learner/* can only change through a pull request
#
# Needs the GitHub CLI (https://cli.github.com) signed in as a repository admin:  gh auth login
# Run it ONCE, from inside the repository, after:
#   - main has been pushed and the CI workflow has finished at least once
#   - the learner branches have been created
#
set -euo pipefail

script_dir="$(cd "$(dirname "$0")" && pwd)"

echo "Creating ruleset: Protect main"
gh api --method POST "repos/{owner}/{repo}/rulesets" --input "$script_dir/github/ruleset-main.json" --jq '.name + " -> " + .enforcement'

echo "Creating ruleset: Protect learner branches"
gh api --method POST "repos/{owner}/{repo}/rulesets" --input "$script_dir/github/ruleset-learner-branches.json" --jq '.name + " -> " + .enforcement'

echo
echo "Done. Review them on GitHub under Settings > Rules > Rulesets."
