# Contributing to ResultPattern

Thank you for your interest in contributing! This repository demonstrates a simple and expandable `Result` pattern implementation.

## Getting Started
- Ensure you have .NET 10 SDK installed.
- Use Visual Studio 2026 or VS Code with C# extensions.
- Restore dependencies with `dotnet restore`.

## Development Workflow
1. Fork the repository and create a feature branch from `main`.
2. Implement changes with tests. Aim for meaningful unit test coverage.
3. Run `dotnet build` and `dotnet test` locally.
4. Ensure code style is enforced by `.editorconfig`.
5. Commit with clear messages. Use conventional prefixes where useful (feat, fix, docs, test, refactor).
6. Open a Pull Request to `main` and link related issues.

## Coding Standards
- Follow the rules in `.editorconfig` (naming, formatting, analyzers).
- Prefer small, focused changes.
- Public APIs should be documented with XML comments.
- Keep the `Result` type minimal and extensible.

## Testing
- Use MSTest (`Microsoft.VisualStudio.TestTools.UnitTesting`).
- Place tests in the `ResultPattern.Test` project.
- Name tests clearly, asserting both behavior and serialization.

## Branching and Releases
- Default branch: `main`.
- Use feature branches: `feature/<short-name>`.
- Bugfix branches: `fix/<short-name>`.
- Releases are tagged with semantic versioning: `vX.Y.Z`.

## Pull Requests
- Include description of changes and rationale.
- Checklist:
  - [ ] Builds cleanly
  - [ ] Tests pass
  - [ ] Style checks pass
  - [ ] No API breaking changes without justification

## Continuous Integration
- Recommended: GitHub Actions running `dotnet build` and `dotnet test` on `push` and `pull_request`.

## License
- MIT. By contributing, you agree your contributions will be licensed under the MIT License.