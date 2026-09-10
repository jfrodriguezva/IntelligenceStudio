# System context

MIS is the application boundary. Football providers supply observations; ML computes from controlled inputs; AI providers generate interpretations. None determines editorial truth or directly modifies canonical business data.

```mermaid
flowchart LR
    Analyst[Analyst and content creator] --> MIS[Madrid Intelligence Studio]
    Editors[Future editors and viewers] --> MIS
    Provider[API-Football initial provider] -->|Football observations| MIS
    MIS -->|Controlled data and model requests| ML[Private Python ML runtime]
    ML -->|Versioned numerical results| MIS
    MIS -->|Evidence and versioned prompts, later| AI[AI provider]
    AI -->|Interpretations, later| MIS
    MIS -->|Reviewed outlines and assets, later| Content[Podcast and content workflow]
```

The browser crosses an authenticated application boundary. API-Football and future AI providers are untrusted external inputs and availability dependencies. Python is a private compute boundary. PostgreSQL is the business system of record and is accessed through the .NET application. No browser holds provider secrets or directly calls Python.

v0.1 activates the analyst, application, and football-provider paths. Other paths show the target architecture without requiring empty modules or services now. External publishing is future work, requiring explicit user action and integration design.
