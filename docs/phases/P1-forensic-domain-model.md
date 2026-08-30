# P1 — Canonical DNA Domain Model and Module Boundaries

**Status:** In progress — P1.3 contracts and P1.4 architecture proof
**Baseline:** P0 merged at `22178fe12d2084b9c8e1fd1cf897383bc67b3b0a`  
**Branch:** `feat/p1-forensic-domain-model`  
**Planning source:** the previously agreed GENis-system design conversation

## Purpose

P1 defines the canonical forensic-DNA model and its modular-monolith boundaries before database
migrations, endpoints, user interfaces, scientific processing, matching, or production imports begin.

## Decisions restored from the agreed plan

### Runtime and deployment

- The system runs offline inside a private local network.
- It is one installation, not SaaS and not multi-tenant.
- FullStackHero remains the application foundation.
- The architecture is a modular monolith.
- PostgreSQL is the system of record.
- Files/object storage are local to the installation (local MinIO/files).
- Scientific engines are isolated behind adapters.
- Runtime operation must not require the public internet.

### Explicit exclusions

Do not introduce any of the following unless a later, separately approved requirement proves it
necessary:

- `TenantId` or a multitenancy domain;
- internet or public-cloud runtime dependencies;
- microservices;
- Kafka;
- Kubernetes;
- Elasticsearch;
- MongoDB;
- a Redis cluster.

### Canonical DNA model spine

The agreed canonical model follows this order:

```text
Case
  -> EvidenceItem
    -> BiologicalSample
      -> GeneticProfile
        -> Loci / Alleles / Peaks
```

This establishes the domain spine only. Cardinalities, identity rules, lifecycle transitions, correction
rules, and persistence ownership must be made explicit in P1 before implementation.

### Agreed module boundaries

The following module names and separation were agreed previously and replace the generic candidate
contexts in the initial P1 draft:

1. `Cases`
2. `Evidence`
3. `Samples`
4. `Genetics/Profiles`
5. `STR Kits`
6. `Frequency Tables`
7. `Matching`
8. `Interpretation`
9. `Kinship`
10. `MPI/MissingPersons`
11. `DVI`
12. `ScientificAnalysis/AnalysisRun`
13. `Reporting`
14. `Audit`
15. `Identity/Admin`

Names containing a slash record a conceptual boundary from the earlier plan; P1 must choose the final
.NET project name without collapsing distinct domain ownership accidentally.

## P1 work products

### P1.1 — Canonical language and identities

- Arabic/English ubiquitous-language glossary.
- Stable identifier policy for every canonical object.
- Relationship and cardinality model for the canonical spine.
- Versioning and correction rules for scientific and evidentiary records.
- Definitions that distinguish evidence items, biological samples, analyses, results, and profiles.

### P1.2 — Ownership and dependency map

For every agreed module, record:

- the aggregates and value objects it owns;
- its transaction boundary;
- data that other modules may reference but never mutate;
- allowed contract-only dependencies;
- forbidden runtime-project dependencies;
- commands, queries, and integration events exposed to other modules.

### P1.3 — Forensic invariants and workflows

- Evidence and sample chain-of-custody state machines.
- Analysis-run lifecycle and reproducibility rules.
- Profile validation, review, approval, and versioning.
- Match search, candidate review, and disposition.
- Interpretation and kinship calculation provenance.
- Missing-person and DVI workflow separation.
- Report snapshot, disclosure, correction, and supersession rules.
- Audit attribution and separation-of-duties permissions.

### P1.4 — Architecture proof

- Architecture tests for every approved dependency rule.
- Module/contract scaffolding only after P1.1 and P1.2 are approved.
- One minimal vertical slice proving the accepted model without implementing the full scientific scope.
- Complete backend, frontend, migration/seed, health, and offline-runtime validation.

## Details not recoverable verbatim

The prior conversation confirms the phase, constraints, module list, and canonical model spine above.
The available history does not expose the exact previously discussed:

- aggregate ownership table and cardinalities;
- dependency arrows and integration-event catalogue;
- lifecycle/status enumerations;
- permission matrix;
- retention, deletion, and legal-correction policy;
- supported import formats or scientific/matching-engine selection;
- names and acceptance criteria of phases after P1.

These details must not be guessed. They remain P1 design decisions until recovered from the earlier
conversation or confirmed by the domain owner.

## Exit gate

P1 may be marked complete only when:

- the canonical glossary and model are approved;
- every agreed module has explicit ownership and dependency rules;
- custody, analysis, review, matching, and reporting transitions are unambiguous;
- forensic history is append-only or uses an explicitly approved supersession model;
- no forensic module has an undeclared runtime dependency;
- scientific engines remain replaceable behind local adapters;
- backend tests and both frontend builds are green;
- migration/seed, health, login, and offline runtime validation remain green;
- the phase PR records the exact validated commit and workflow run.

## Implementation hold

P1.1 and P1.2 are approved for limited architecture scaffolding. Do not add forensic migrations,
endpoints, pages, scientific algorithms, matching logic, or production-data import until their P1.3
contracts and invariants are approved.
