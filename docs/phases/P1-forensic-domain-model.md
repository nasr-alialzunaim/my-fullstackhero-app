# P1 — Forensic Domain Model and Module Boundaries

**Status:** In progress — design gate  
**Baseline:** P0 merged at `22178fe12d2084b9c8e1fd1cf897383bc67b3b0a`  
**Branch:** `feat/p1-forensic-domain-model`

## Purpose

P1 defines the forensic domain language, aggregate ownership, module boundaries, and cross-module
contracts before any forensic schema, API, user interface, import, or matching implementation begins.

This document opens the phase; it does not approve the candidate model below.

## Locked constraints inherited from P0

- One installation; the product is not SaaS and has no tenant lifecycle.
- Operates on an isolated local network and remains useful without internet access.
- No runtime dependency on cloud services or public CDNs.
- PostgreSQL and all required infrastructure are deployable with the local installation.
- Every evidentiary change must be attributable, auditable, and time ordered.
- Existing module boundaries remain enforced; a module may depend on another module only through contracts.
- `src/BuildingBlocks` remains protected unless a separately reviewed decision explicitly requires a change.

## P1 design questions

The following questions must be answered with the domain owner before implementation:

1. Which forensic discipline is in scope, and what vocabulary is legally authoritative?
2. What is the distinction among case, subject, specimen, evidence item, sample, analysis, result, and profile?
3. Which identifiers are external, which are generated locally, and which must never change?
4. What chain-of-custody events are mandatory, and can any event be corrected or only superseded?
5. Which workflow states require two-person review or approval?
6. What data may be amended, expunged, retained, exported, or disclosed?
7. Which matching engine or external file formats—if any—must be supported offline?
8. What permissions exist for intake, laboratory work, review, search, matching, and reporting?

## Candidate bounded contexts

These are hypotheses for review, not approved implementation modules.

| Candidate context | Owns | Does not own |
|---|---|---|
| Cases | Case identity, classification, status, participants, case-level lifecycle | Physical custody events or laboratory results |
| Evidence | Evidence/specimen identity, seals, locations, transfers, chain of custody | Analytical interpretation |
| Laboratory | Work items, examination workflow, instruments/method references, review states | Case authority or match decisions |
| Profiles | Validated forensic profiles/results and their version history | Candidate-search workflow |
| Matching | Search requests, candidates, review and disposition of potential matches | Source profile mutation |
| Reporting | Immutable report snapshots and disclosure packages | Authoring the source facts it renders |

Existing Identity, Auditing, Files, and Notifications capabilities are supporting infrastructure. P1 must
decide whether each retained capability remains adequate for a single-installation forensic system; it
must not silently move forensic ownership into those generic modules.

## Required design artifacts

- Ubiquitous-language glossary with Arabic and English terms.
- Context map showing upstream/downstream relationships.
- Aggregate list with invariants and transaction boundaries.
- State machines for evidence custody and laboratory review.
- Cross-module command/query/event contract catalogue.
- Permission matrix and separation-of-duties rules.
- Audit, retention, correction, and deletion policy.
- Data classification and offline import/export threat model.
- Architecture tests proposed for every accepted dependency rule.
- Thin vertical-slice plan for the first implementation increment.

## Work sequence

1. **P1.1 — Language:** confirm terminology, actors, identifiers, and legal invariants.
2. **P1.2 — Boundaries:** approve contexts, aggregate ownership, and forbidden dependencies.
3. **P1.3 — Contracts:** approve state transitions, permissions, and integration contracts.
4. **P1.4 — Proof:** add architecture tests and one minimal end-to-end domain slice.

## Exit gate

P1 may be marked complete only when:

- every design question above is resolved or explicitly deferred with an owner;
- candidate contexts are accepted, merged, renamed, or rejected;
- each aggregate has an owner, invariants, and a persistence boundary;
- custody and review state transitions are unambiguous and tested;
- no forensic module has an undeclared runtime dependency;
- backend tests and both frontend builds are green;
- offline runtime validation remains green;
- the phase PR records the exact validation run and commit.

## Implementation hold

Until P1.1 and P1.2 are approved, do not add forensic database migrations, endpoints, pages, matching
logic, or production data import. Documentation and test-only boundary scaffolding are permitted.
