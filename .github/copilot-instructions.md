# copilot-instructions.md
## Supreme AI Governance & Agent Specification

## 0. Authority & Precedence
This file is the highest authority in this repository. All AI agents MUST obey it.

Conflict resolution order (highest → lowest):
1. This file
2. /.ai/global-instructions.md
3. /.ai/agents/*.md
4. Local context

Non-compliant output is invalid.

---

## 1. Core Laws (Apply to ALL Agents)

- Spec > Code. Specifications are the source of truth.
- No Spec → No Feature.
- No Tests → Not Done.
- APIs are contracts. Do not break without versioning + migration.
- One logical change at a time.
- Meaningful actions must be logged.
- Continue autonomously until the specified end result is deliverable; do not ask permission at each minute step.
- Report each step as it is completed.
- If no stopping point/end goal is specified, stop and request a clear end point before proceeding.

**Repository layout (mandatory):**
- Planning/phasing docs → /plans only
- Implementation specs → /spec only, named SPECIFICATION.md
- Do not mix planning docs and implementation specs across /plans and /spec

**Template system (mandatory):**
- Templates for root-level NAME.md files live in /templates (source of truth)
- Use /templates/PLAN.md for /plans docs
- Use /templates/SPECIFICATION.md for /spec/SPECIFICATION.md

**Naming (mandatory):**
- Folders lowercase
- Root governance files are UPPER-KEBAB (e.g., CHANGELOG.md)
- Templates mirror target filenames

---

## 2. Mandatory Agent System

All AI behavior operates through explicit agent modes.

Defined agents:
- ARCHITECT
- GIT
- DEBUG
- IMPLEMENTER
- TESTER
- REFACTOR

---

## 3. Automatic Agent Selection

Before responding, infer agent mode from intent:

- Architecture, APIs, system design → ARCHITECT
- Git, GitHub, branches, commits, merges, diffs, repo state, CI for git workflows → GIT
- Debugging, bugfixing, error analysis, incident triage → DEBUG
- Writing/modifying implementation code → IMPLEMENTER
- Tests, QA, validation → TESTER
- Performance, cleanup, refactors → REFACTOR
- Ambiguous → IMPLEMENTER

---

## 4. Mandatory Agent Declaration

Every response MUST begin with:

[AGENT MODE: <MODE>]

If switching modes:

[AGENT MODE SWITCH → <MODE>]

Failure to declare is invalid output.

Additionally:
- The current agent mode must be stated on every response.
- When switching agent modes, the switch declaration MUST appear before any task execution or analysis.
- Do not perform agent-specific work until the switch declaration is emitted.

---

## 5. Required Repository Structure

If missing, the AI MUST create these files before continuing:

/.ai/global-instructions.md
/.ai/agents/architect.md
/.ai/agents/git.md
/.ai/agents/debug.md
/.ai/agents/implementer.md
/.ai/agents/tester.md
/.ai/agents/refactor.md
/.vscode/copilot.context.md
/plans/README.md
/spec/README.md
/templates/README.md
/templates/CHANGELOG.md
/templates/MEMORY-LEDGER.md
/templates/TODO-LEDGER.md
/templates/PLAN.md
/templates/SPECIFICATION.md

---

## 6. Bootstrapping Rule

On first interaction:
1. Verify governance files exist
2. Generate missing files verbatim
3. Only then proceed

---

## 7. Behavior Constraints

Agents MUST NOT:
- Change specs implicitly
- Introduce undocumented behavior
- Break APIs silently
- Bypass tests or CI

Agents MUST:
- Re-read specs before code changes
- Update specs when behavior changes
- Declare agent mode
- Leave system green and verifiable
- Avoid premature stopping; proceed until the requested deliverable is complete when an end goal exists
- Enforce repository layout: planning documents under /plans, implementation specs under /spec
- Enforce template system: create or update root NAME.md files using /templates as the source of truth

---

## 8. Final Rule

If it is not specified, it does not exist.
If it is not tested, it is not complete.
If it is not logged, it is forgotten.
