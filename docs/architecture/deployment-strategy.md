# Deployment strategy

Azure is the intended production target from AGENTS.md; Phase Zero provisions nothing and commits no spending. Service availability, supported versions, regional choices, SKUs and cost estimates must be checked against official documentation when deployment work starts.

## Initial topology

Use Azure Container Apps for web and .NET API; keep API reachable only through the intended ingress/private application path. PostgreSQL is managed through Azure Database for PostgreSQL; assets use Azure Blob Storage when needed. Key Vault holds secrets; OpenTelemetry exports to the selected Application Insights/Azure monitoring integration. Python runs as a private container service when introduced. Redis is conditional on a measured need; choose the then-supported managed offering through a deployment ADR. No AKS/Kubernetes.

Infrastructure as Code belongs in `infrastructure/azure` when deployment begins. Select Bicep or Terraform with an ADR based on team operations; do not generate both. Use separate development/staging/production configuration and least-privilege identities. No shared development credentials in production.

## Release process

1. Pass required CI and build immutable images tagged with code revision/digest.
2. Provision/update reviewed IaC; verify private access, TLS, secret resolution and permissions.
3. Back up and run one explicit migration job using a separate privileged identity; application replicas never race to migrate at startup.
4. Deploy compatible API/web revisions, check readiness and authenticated smoke flows in staging, then promote the same images.
5. Monitor error rates, database health, job backlog and import freshness after promotion. Retain the previous compatible revision for application rollback.

Favor expand/contract schema changes. Rolling back an image cannot undo a destructive migration; restoration/recovery procedures must be tested before an irreversible schema change. Record migration compatibility windows. Use single API replica initially for predictable jobs; test persistent Quartz recovery. Enable clustering plus shared domain locking before scaling replicas. Avoid scale-to-zero while in-process scheduled jobs require timely execution unless an explicit external wake-up design exists.

## Data protection and operations

Set database backup retention, restore procedures, object versioning/lifecycle and secret rotation before production. Proposed initial planning targets are RPO ≤ 24 hours and RTO ≤ 4 hours, subject to owner acceptance and demonstrated restore timing; these are not achieved SLAs. Retain frozen model artifacts/evidence according to data rights and reproducibility needs. Prove a restore can recover database references and corresponding objects together.

First deployment gates: real identity provider and role mapping, provider rights/quota budget, exposure review, tested backup restore, observability/runbook, cost estimate and approved resource scope. See the [risk register](../product/risks-and-assumptions.md). Local v0.1 implementation can proceed before these hosting decisions.
