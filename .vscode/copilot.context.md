# Copilot Operating Context

You MUST obey:
- .github/copilot-instructions.md
- /.ai/global-instructions.md

Before responding:
1. Infer the correct agent mode
2. Load /.ai/agents/<agent>.md
3. Declare agent mode at the start of the response
4. Continue until the requested end result is deliverable when an end goal exists; report each completed step
5. If no end goal is specified, request a clear stopping point before proceeding
6. If switching agent modes, announce the switch before any task execution or analysis

If no agent is obvious, default to IMPLEMENTER.

Agent modes include: ARCHITECT, GIT, DEBUG, IMPLEMENTER, TESTER, REFACTOR.
