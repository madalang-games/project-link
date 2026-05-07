# shared — Cross-Cutting Definitions

## Nav
| path | role |
|------|------|
| `contracts/` | Shared C# DTO contracts (server + Unity) | → `contracts/AGENTS.md` |
| `datas/` | Game meta data / design data (source for gen-data) | → `datas/AGENTS.md` |
| `types/` | Shared enums and constants | → `types/AGENTS.md` |

## Rules
- This directory contains ONLY language-agnostic source definitions
- Generated output lives in `*/generated/` — never here
- `_` prefix files/dirs are skipped by gen tools (use for examples/drafts)
- When adding a new subdomain: create subdirectory + `AGENTS.md` + update Nav above
