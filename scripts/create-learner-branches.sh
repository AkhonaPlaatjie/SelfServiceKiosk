#!/usr/bin/env bash
#
# Creates one branch per learner on GitHub, each starting from the tip of main.
# Safe to run again: branches that already exist are skipped.
#
# Usage (from anywhere inside the repository, after main has been pushed):
#   ./scripts/create-learner-branches.sh                  # uses scripts/learners.txt
#   ./scripts/create-learner-branches.sh my-list.txt      # a different list
#   ./scripts/create-learner-branches.sh my-list.txt develop   # a different base branch
#
set -euo pipefail

script_dir="$(cd "$(dirname "$0")" && pwd)"
list_file="${1:-$script_dir/learners.txt}"
base_branch="${2:-main}"

if [ ! -f "$list_file" ]; then
  echo "Learner list not found: $list_file" >&2
  exit 1
fi

echo "Fetching origin/$base_branch ..."
git fetch --quiet origin "$base_branch"

created=0
skipped=0

while IFS= read -r line || [ -n "$line" ]; do
  # Drop comments, Windows line endings and surrounding spaces.
  name="$(printf '%s' "$line" | tr -d '\r' | sed -e 's/#.*$//' -e 's/^[[:space:]]*//' -e 's/[[:space:]]*$//')"
  [ -z "$name" ] && continue

  # "Thabo Mokoena" -> "thabo-mokoena"
  slug="$(printf '%s' "$name" | tr '[:upper:]' '[:lower:]' | sed -e 's/[^a-z0-9][^a-z0-9]*/-/g' -e 's/^-//' -e 's/-$//')"
  [ -z "$slug" ] && continue
  branch="learner/$slug"

  if [ -n "$(git ls-remote --heads origin "$branch")" ]; then
    echo "skipped  $branch (already exists)"
    skipped=$((skipped + 1))
    continue
  fi

  # Creates the branch on GitHub directly from origin/<base>; nothing is checked out locally.
  git push --quiet origin "refs/remotes/origin/$base_branch:refs/heads/$branch"
  echo "created  $branch"
  created=$((created + 1))
done < "$list_file"

echo
echo "Done: $created created, $skipped skipped."
