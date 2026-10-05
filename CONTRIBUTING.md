# Contributing

How to work in this repository as a learner. Facilitators setting up the repository should read [docs/GITHUB_SETUP.md](docs/GITHUB_SETUP.md).

## How the branches fit together

```
main                                   The starter. Protected. Facilitators only.
 │
 ├── learner/<your-name>               Your project. Protected: changes arrive by pull request.
 │     ├── feature/<your-name>/fr-01-registration      one branch per feature
 │     └── feature/<your-name>/fr-06-top-up
 │
 └── learner/<someone-else>            Not yours. Leave it alone.
```

- You never push to `main`. GitHub will reject it.
- You never push directly to `learner/<your-name>` either. You build each feature on its own branch and merge it in through a pull request. That gives you the "pull requests per feature" history the brief asks for.
- Learner branches are never merged into `main`. This is an individual project, so each learner branch is a separate, complete solution.

## One-time setup

```
git clone <repository-url>
cd <repository-folder>
git switch learner/<your-name>
```

Then follow "Getting started" in the [README](README.md).

## For every feature

**1. Start from your up-to-date learner branch**

```
git switch learner/<your-name>
git pull
git switch -c feature/<your-name>/fr-01-registration
```

Name the branch `feature/<your-name>/<requirement-id>-<short-description>`. Do not start a branch name with `learner/`; that prefix is protected.

**2. Commit small, and say what changed**

```
git add .
git commit -m "feat: add registration endpoint (FR-01)"
```

| Prefix | Use it for |
| --- | --- |
| `feat:` | A new feature |
| `fix:` | A bug fix |
| `test:` | Adding or changing tests |
| `refactor:` | Restructuring without changing behaviour |
| `docs:` | Documentation only |
| `chore:` | Build, tooling, packages |

**3. Check it before you push**

```
dotnet build
dotnet test
```

**4. Push and open a pull request**

```
git push -u origin feature/<your-name>/fr-01-registration
```

On GitHub, open a pull request and set:

- **base:** `learner/<your-name>` — GitHub defaults to `main`, so change it
- **compare:** your feature branch

Fill in the template. The **build-and-test** check must be green before the pull request can be merged.

**5. Merge and tidy up**

Once the check passes (and your mentor has approved, if they ask to review), merge the pull request on GitHub, then:

```
git switch learner/<your-name>
git pull
git branch -d feature/<your-name>/fr-01-registration
```

## Picking up changes to the starter

When a facilitator announces an update to `main`:

```
git switch learner/<your-name>
git pull
git switch -c chore/<your-name>/sync-main
git fetch origin
git merge origin/main
```

Resolve any conflicts, run `dotnet build` and `dotnet test`, push, and open a pull request into `learner/<your-name>` as usual.

## Before you ask for a review

- [ ] The pull request targets your own learner branch
- [ ] It builds, and every test passes
- [ ] No secrets are committed: connection strings, JWT keys, passwords
- [ ] No real personal information in code, seed data, tests or screenshots
- [ ] Controllers contain no business logic and return DTOs
- [ ] New business rules have unit tests
- [ ] You can explain every line
