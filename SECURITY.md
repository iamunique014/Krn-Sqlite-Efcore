# Security policy

## Supported versions

Security fixes are applied to the `main` branch. Deployments should use the latest commit that has passed the required CI and security checks.

## Reporting a vulnerability

Please do not open a public issue for a suspected vulnerability. Report it privately through GitHub's **Report a vulnerability** feature on this repository. Include the affected component, reproduction steps, impact, and any suggested mitigation.

We will acknowledge reports as soon as practical, investigate them privately, and coordinate disclosure after a fix or mitigation is available.

## Automated checks

Pull requests and pushes to `main` run build and test gates, compiler/analyzer checks, NuGet vulnerability checks, CodeQL analysis, dependency review, and secret scanning. Dependabot opens update pull requests for NuGet and GitHub Actions dependencies.
