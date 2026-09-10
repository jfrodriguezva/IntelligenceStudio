# Security model

## Trust boundaries

The browser is untrusted. ASP.NET Core enforces identity, role and resource authorization; Next.js forwarding must not trust identity headers supplied by browsers. PostgreSQL, private Python endpoints and object storage are inaccessible directly to the public browser. Football and AI provider payloads are untrusted data even when authenticated.

## Roles

| Action | Admin | Analyst | Editor | Viewer |
|---|---|---|---|---|
| Read permitted fixtures/analysis | Yes | Yes | Yes | Yes |
| Trigger imports / inspect operational errors | Yes | No | No | No |
| Request predictions / record analysis and notes, later | Yes | Yes | No | No |
| Create/edit permitted content drafts, later | Yes | Yes | Yes | No |
| Manage provider configuration and access | Yes | No | No | No |

For v0.1 all authenticated users can read the single workspace; no multi-tenant claim. Later note/content mutations require explicit ownership/workspace policy, not role alone. Published/external output requires the product's human editorial action. No automatic bet placement.

## Authentication approach

Use same-origin browser access with server-issued secure HTTP-only cookies for shared hosting. ASP.NET Core validates sessions and authorization. Use Secure and an appropriate SameSite policy, bounded session lifetime, logout invalidation, and CSRF token validation for unsafe cookie-authenticated requests; SameSite alone is not the entire defense. Do not put provider keys or session credentials in localStorage or client bundles. Identity provider selection is a pre-hosting ADR; do not build a custom password system by default.

Local implementation may use an explicit development identity only when environment is Development and listening is restricted to loopback. It must fail closed on shared/non-development deployments and cannot be enabled by a browser-controlled header. Tests must cover direct endpoint access, missing/incorrect role, CSRF, and disabled development bypass. This permits local work without silently creating anonymous administrative endpoints.

## Data and integration controls

Validate request length/ranges, DTO schemas, provider payloads and numeric values. Parameterize SQL, encode output, and avoid arbitrary file execution. Limit uploaded file size/type and inspect content when uploads arrive; never serve untrusted active content from a privileged origin. Tactical state is data, not executable code. Restrict server fetch destinations to configured provider/storage endpoints to prevent SSRF.

Use environment secrets locally and managed secret storage/identity in production. Application database permissions differ from migration permissions. Deny mutation of immutable historical records to the normal runtime role when those tables are implemented. Separate artifact permissions by service; Python cannot read/write football business tables.

AI execution receives only necessary evidence, treats external text/notes as untrusted context and cannot execute instructions found in that evidence. AI output remains interpretation with source references and human review. Version prompts; retain sensitive text only under a documented policy.

Apply security headers including a tested Content Security Policy, TLS in hosting, narrowly scoped CORS if cross-origin access is ever required, authentication/expensive-operation rate limits and server-side authorization. Do not proxy arbitrary provider URLs. Keep dependencies patched through automated checks, with actual versions verified at implementation time.

## Audit and retention

Audit configuration/access changes, sync requests, model registration and editorial state transitions with actor/time/target/outcome, excluding secrets. Set raw payload, note/content and artifact retention based on provider rights and user needs. Minimize personal data; do not promise permanent retention of licensed source bodies without reviewing rights. Preserve legal normalized evidence and metadata needed for declared reproducibility. Test database and object restore before production.
