# Issue and change log — INSS Finance Module

A running record of everything delivered to INSS: problems reported and resolved, and features
built on request. For a problem — what was seen, what actually caused it, how it was fixed. For
a feature — what was asked for and what was built. Both carry the same closing line: what to do
differently when this code is rebuilt, so the same problem is not reintroduced and the same
feature is not rediscovered from scratch.

**Add an entry as soon as a fix is finished**, not in batches.

**How to add one**

- Next free ID (`INSS-0NN`); IDs are identifiers, not a sequence — place the row in the index in
  date order but never renumber an existing entry, because other documents refer to them.
- Written in **English**, so it can be shared with the client without rewriting.
- Four things, in this order: what was **seen** (in the words the problem was reported in), what
  actually **caused** it, what the **fix** was, and — the reason this log exists — **what to do
  differently when rebuilding** so it is not reintroduced.
- Tag it with one or more of the categories below. Where a report has two causes, list both: the
  lesson is usually in the second one.
- Screenshots go in `issue_log_images/`, named with the issue ID. Never store real citizen data
  — crop it, or reproduce the screen with test data first.
- Update the counts in the three tables above and the total in **Summary**. They are the part
  of this document people read first, and a stale count discredits the rest of it.

---

## Fix or feature

Every entry is one of two types, marked in the index:

| Type | Count | Meaning |
|---|---:|---|
| **FIX** | 27 | Something was wrong and was corrected. Carries a tag and an origin, below. |
| **FEATURE** | 9 | Something new was built, or an existing behaviour deliberately changed on request. No origin — nothing was broken. |
| **BACKLOG** | 17 | Identified during other work and too involved to deliver in the same pass. Listed in *Backlog* below, to be scheduled. |
| **Total** | **53** | |

Features are recorded here for the same reason as fixes: the decisions behind them are the part
that is expensive to recover. A rebuild can read the code, but not the reason a rule was written
the way it was, or which option the client rejected.

Backlog items are recorded for a harder reason: they are invisible. A defect gets reported again;
something identified but not yet built is only remembered by the person who found it. Each one
below was analysed at the time it surfaced and judged too involved to fold into the change that
revealed it — that analysis is worth as much as the work itself, and it is the first thing lost
when people change.

---

## How issues are classified

Knowing the *kind* of cause matters more than the individual bug: it tells you where to look
first next time, and who has to act.

| Tag | Count | Meaning | Who fixes it |
|---|---:|---|---|
| **CODE** | 21 | Defect in the application source | Development |
| **DATA** | 2 | The code is right; the stored data is wrong or missing | Script or data entry |
| **CONFIG** | 2 | Per-task or per-environment configuration, which does **not** travel with a code deployment | Applied per environment |
| **INFRA** | 2 | Server, container, OS or reverse proxy | Client IT |
| **LIBRARY** | 2 | A third-party library's default behaviour, or a platform dependency it needs | Development or IT |
| **DEPLOY** | 2 | The code is right but what is running is not: missing rebuild, stale asset, wrong script version | Deployment process |
| **NOT-A-BUG** | 1 | Reported as a defect, turned out to be correct behaviour misread | Explanation only |

*Totals exceed 27 because an issue can carry more than one tag.*

A single report often has more than one tag. Where that happens both are listed, because the
lesson usually lives in the second one.

### Where it came from

The tag above says *what* is broken. It does not say *why the problem existed in the first
place*, which is what decides whether it can be prevented. Every entry therefore also carries an
**Origin**:

| Origin | Count | Meaning | What prevents it |
|---|---:|---|---|
| **LEGACY** | 11 | Already present in the inherited system; it never worked correctly | Only found by use or by review — assume nothing is safe because it is old |
| **GAP** | 5 | Never built. A rule or a step that has no implementation at all | Check the requirement against the code, not the code against itself |
| **SIDE-EFFECT** | 3 | A change made for one purpose broke something unrelated | Ask who else consumes what you changed: shared table, shared endpoint, shared lookup |
| **OURS** | 3 | Introduced by our own work — a design flaw, or a regression | Fix the cause in one shared place, not in the screen that reported it |
| **ENV** | 3 | The environment differs or changed — OS, proxy, cache, a restored database | Compare environments before reading code |
| **MISREAD** | 1 | Nothing was broken | Reproduce with a correct query before accepting the diagnosis in the report |
| **DEFAULT** | 1 | A third-party default nobody overrode | Read the defaults of anything whose output the user sees |
| **Total** | **26** | | |

---

## Summary

**53 entries, 2026-07-12 to 2026-08-23** — 27 fixes, 9 features and 17 items in the backlog. The counts per tag and per
origin are in the two tables above; update them when adding an entry.

**LEGACY and GAP together are 16 of the 27 fixes.** More than half of everything reported had
**never worked**, rather than having recently broken. Only 3 were introduced by our own work.

Two consequences worth acting on:

- **Review an area before extending it.** Most of these surfaced because someone finally used a
  path that had been wrong since before this engagement. Building on top of such a path without
  reading it first is how a GAP becomes an OURS.
- **The client's environments are a distinct source of failure.** ENV, INFRA and DEPLOY together
  are 7 tags across 5 issues, none of which could be reproduced on the development machine. When
  a report only happens on Staging or Production, compare the environments before reading code.

This is also why every entry ends with a *When rebuilding* line: the value of the log is not the
list of fixes, it is the list of things not to do again.

---

## Index

| ID | Date | Type / Tags | One line |
|---|---|---|---|
| [INSS-001](#inss-001) | 2026-07-12 | CODE | New user accounts could not log in |
| [INSS-002](#inss-002) | 2026-07-12 | CODE | "Open Declaration" hung forever for internal officers |
| [INSS-003](#inss-003) | 2026-07-12 | CODE | Account codes repeated across the whole chart of accounts |
| [INSS-004](#inss-004) | 2026-07-12 | CODE | Approved budget lines disappeared after approval |
| [INSS-005](#inss-005) | 2026-07-13 | CODE | Parent reference on Cabimento/Compromisso looked wrong |
| [INSS-006](#inss-006) | 2026-07-13 | CODE | Opening balance screen showed code "0" on every row |
| [INSS-007](#inss-007) | 2026-07-15 | CODE | Reconciliation dropdown offered two meaningless options |
| [INSS-024](#inss-024) | 2026-07-17 | CODE | The "AD" tab of the Despesas report also listed committed expenses |
| [INSS-025](#inss-025) | 2026-07-17 | CODE / CONFIG | The Obrigação tab reported a payment status, not a real Obrigação |
| [INSS-026](#inss-026) | 2026-07-17 | DATA | The Budget Execution report showed nothing under Despesa |
| [INSS-008](#inss-008) | 2026-07-31 | CODE | "Cabimentar" button stayed visible with nothing left to commit |
| [INSS-009](#inss-009) | 2026-07-31 | CODE | A $0-budget line showed a large available balance |
| [INSS-010](#inss-010) | 2026-08-01 | CODE | Over-budget expenses could be registered, only blocked much later |
| [INSS-011](#inss-011) | 2026-08-01 | CODE | A task could advance leaving expenses unauthorised forever |
| [INSS-012](#inss-012) | 2026-08-12 | CODE | A screen froze permanently on "Please Wait" |
| [INSS-013](#inss-013) | 2026-08-14 | CODE | Second expense on the same budget line could not be authorised |
| [INSS-014](#inss-014) | 2026-08-17 | CODE | A dropdown was blank and could not be opened |
| [INSS-015](#inss-015) | 2026-08-17 | CONFIG | A deactivated option kept appearing until the next day |
| [INSS-016](#inss-016) | 2026-08-18 | DATA / DEPLOY | "Invalid object name" on the OSS Global report |
| [INSS-017](#inss-017) | 2026-08-18 | NOT-A-BUG / CODE | Budget balance appeared to ignore the activity |
| [INSS-018](#inss-018) | 2026-08-21 | DEPLOY | 18 icons vanished from the whole application |
| [INSS-019](#inss-019) | 2026-08-21 | INFRA | Attaching a document failed with "Something went wrong" |
| [INSS-020](#inss-020) | 2026-08-22 | LIBRARY | The payment total was printed on every page |
| [INSS-021](#inss-021) | 2026-08-22 | CODE | Recipient name and NISS were blank on the payment order |
| [INSS-022](#inss-022) | 2026-08-22 | LIBRARY / INFRA | Every Excel export failed on the Linux servers |
| [INSS-023](#inss-023) | 2026-08-22 | CODE | "Something went wrong" said nothing at all |
| [INSS-036](#inss-036) | 2026-08-23 | CODE | Company accounts had no name on Users Access Control |
| [INSS-027](#inss-027) | 2026-07-17 | FEATURE | Payment order grouped by bank, month and year |
| [INSS-028](#inss-028) | 2026-07-13 | FEATURE | Reconciliation: view the uploaded proof, filter by date |
| [INSS-029](#inss-029) | 2026-08-01 | FEATURE | Remaining balance shown on the expenditure screens |
| [INSS-030](#inss-030) | 2026-07/08 | FEATURE | Classificação Económica report and crosswalk |
| [INSS-031](#inss-031) | 2026-08-11 | FEATURE | Budget adjustment (top-up and transfer) |
| [INSS-032](#inss-032) | 2026-08-17 | FEATURE | Expense uniqueness on five keys, confirmation not block |
| [INSS-033](#inss-033) | 2026-08-20 | FEATURE | Document attachments: file name, picker, duplicate warning |
| [INSS-034](#inss-034) | 2026-08-22 | FEATURE | Warning when the account number disagrees with the IBAN |
| [INSS-035](#inss-035) | 2026-08-22 | FEATURE | Name and account number left-aligned on the payment order |

---

## INSS-001
**New user accounts could not log in** · 2026-07-12 · `CODE`
**Origin:** `GAP` — a new administration screen was built without satisfying a gate the legacy login still enforces.

**Seen:** an account created through the new user administration screen was rejected at login.

**Cause:** the legacy login path refuses any account with no legacy `Perfil` attached, and the
new screen never attached one.

**Fix:** on creation, the account is linked to a placeholder `Perfil` that grants no legacy
capability — enough to satisfy the old gate without giving any real permission. The legacy
login code was deliberately left untouched.

**When rebuilding:** if two permission systems coexist, every account-creation path must
satisfy *both*, even when one of them is considered obsolete.

---

## INSS-002
**"Open Declaration" hung forever for internal officers** · 2026-07-12 · `CODE`
**Origin:** `SIDE-EFFECT` — an API contract was changed for the external portal; the internal application was never updated to match.

**Seen:** searching a company by NISS from the internal screen, then opening its declaration,
spun forever.

**Cause:** an earlier change made the backend require an RSA-encrypted entity id, but the
encryption was only added to the external portal. The internal application still sent a plain
numeric id, and the decoder threw immediately.

**Fix:** ported the encoder into the internal application and applied it to the ten services
that needed it — each one checked against the actual backend method rather than copied blindly,
which caught two endpoints that still expect the plain id and would have broken.

**When rebuilding:** a change to a shared API contract has to be applied to **every** client of
that API. Two front-ends against one backend is exactly where this gets missed.

---

## INSS-003
**Account codes repeated across the whole chart of accounts** · 2026-07-12 · `CODE`
**Origin:** `LEGACY` — the original screens misread how the account tree stores codes, and every later screen copied that reading.

**Seen:** many different accounts displayed the same code, e.g. several rows all showing "1".

**Cause:** `Codigoconta` and `Agrupamentoconfig` store only the **local segment** for one level
of the tree, not the full account number. The screens displayed that raw segment.

**Fix:** the screens now walk the parent chain and concatenate. Codigoconta uses no separator
(`1211`), Agrupamentoconfig uses dots (`401.01.01`) — the two tables genuinely follow different
conventions. The raw local code is kept as a tooltip, since that is what is actually edited.

**When rebuilding:** never display these two tables' `codigo` column directly. Any new screen
over them must build the full path.

---

## INSS-004
**Approved budget lines disappeared after approval** · 2026-07-12, again 2026-07-14 · `CODE`
**Origin:** `OURS` — our own design reused a helper that creates data from a path that only reads.

**Seen:** after a budget batch was submitted, reviewed and approved, the screen came back empty
and the approved lines could not be found.

**Cause:** the *read* path called the same helper as the write paths, and that helper creates a
draft batch when none is open. Once the only batch reached APPROVED, every page load silently
created a new empty draft and displayed it, hiding the real data behind a growing pile of them.

**Fix:** split the two concerns — a pure read that returns the newest batch regardless of state,
and an explicit "start a new adjustment round" button for creating one. The stray empty drafts
were deactivated. The same defect existed on the Suplementar screen and was fixed there too.

**When rebuilding:** a GET must never create data. If a helper both reads and creates, do not
reuse it from a read path.

---

## INSS-005
**Parent reference on Cabimento/Compromisso looked wrong** · 2026-07-13 · `CODE`
**Origin:** `LEGACY` — the display convention was set before anyone noticed the number restarts every month.

**Seen:** the AD number shown on the Cabimento screen did not seem to match the AD.

**Cause:** the links were correct. `Numero` restarts every month, so it is only unique together
with the month. Each screen showed its own record as `Numero/Mes` but its parent as the bare
number — so two different parents looked identical.

**Fix:** parent references now carry the month as well.

**When rebuilding:** if an identifier is scoped to a period, never display it without that period.

---

## INSS-006
**Opening balance screen showed code "0" on every row** · 2026-07-13 · `CODE`
**Origin:** `OURS` — we fixed INSS-003 inside one component instead of a shared helper, so the next screen repeated it.

**Seen:** the whole Codigo column read "0".

**Cause:** two defects at once — the screen used the raw local code (see INSS-003), and it sorted
that code as text. Exactly twenty active rows have local code "0" and the page size is also
twenty, so page one happened to be nothing but "0" rows.

**Fix:** the screen builds the full code and sorts on it.

**When rebuilding:** this screen was written *after* INSS-003 was fixed and still repeated it,
because the earlier fix lived in the tree-building code and this screen is a flat list. A fix
applied in one component is not a fix of the underlying problem.

---

## INSS-007
**Reconciliation dropdown offered two meaningless options** · 2026-07-15 · `CODE`
**Origin:** `SIDE-EFFECT` — another feature added rows to a shared lookup bucket that this screen reads wholesale.

**Seen:** "Transactions to be Reconciled" listed Receita, Despesa, **Actividade, Funcional**.

**Cause:** the query read a whole shared lookup bucket and excluded only the two entries known to
be wrong. Two unrelated entries added later for another feature leaked through. Worse, selecting
one of them silently behaved exactly like "Despesa".

**Fix:** the query now allows exactly the two values the screen means, instead of blocking the
ones it does not.

**When rebuilding:** shared lookup buckets grow over time. Always allow-list; a block-list is
correct only until someone else adds a row.

---

## INSS-008
**"Cabimentar" button stayed visible with nothing left to commit** · 2026-07-31 · `CODE`
**Origin:** `LEGACY` — an imperative flag written when the screen had fewer states than it has now.

**Seen:** after every expense was authorised and committed, the button remained.

**Cause:** the controlling flag was only ever set to `true`, inside nested conditions. Once no
rows remained in the states that recompute it, it kept its last value forever.

**Fix:** replaced the flag with a getter derived from state that is recomputed on every load.

**When rebuilding:** if a value can be derived from existing state, derive it. Manually toggled
booleans that are only updated on some code paths are a recurring source of "stuck button"
reports in this codebase.

---

## INSS-009
**A $0-budget line showed a large available balance** · 2026-07-31 · `CODE`
**Origin:** `LEGACY` — a display object reused across selections, with only some fields reset.

**Seen:** an expense against a budget line with no funding was authorised with no warning; the
available balance shown was the figure belonging to a completely different expense.

**Cause:** the balance rows are one shared array mutated in place on each selection. When the
newly selected line had no budget, only two of the four fields were reset — the other two kept
the previous expense's values and fed the check.

**Fix:** reset all four fields on that path.

**When rebuilding:** never reuse and mutate a shared display object across selections. Rebuild
it, or reset every field, not the ones that happened to be noticed.

---

## INSS-010
**Over-budget expenses could be registered, only blocked much later** · 2026-08-01 · `CODE`
**Origin:** `GAP` — the rule was implemented at approval and simply never written for the entry step.

**Seen:** an expense far above the available budget was accepted at registration by one officer,
and only rejected later, at authorisation, by a different officer.

**Cause:** registration performed no budget validation at all; the balance was only consulted at
the authorisation step.

**Fix:** the same balance check now runs at registration, with two distinct messages — one for a
budget line with no allocation at all, one for an amount exceeding what is available, showing
both figures.

**When rebuilding:** validate at the point of entry, not only at the point of approval. A rule
enforced two steps later costs a full round trip through the workflow.

---

## INSS-011
**A task could advance leaving expenses unauthorised forever** · 2026-08-01 · `CODE`
**Origin:** `GAP` — the completeness check was written before the expense component existed; the source carries a TODO admitting it.

**Seen:** the workflow moved to the next step while expenses were still merely registered.

**Cause:** the completeness check before advancing covered text, classification and documents —
there was a comment in the source admitting the expense component was never added.

**Fix:** advancing is blocked while any expense in the process is still in the registered state.

**When rebuilding:** when a workflow engine validates "is this step complete", the list of
components it checks must be complete. A new component type without its own check is a silent gap.

---

## INSS-012
**A screen froze permanently on "Please Wait"** · 2026-08-12 · `CODE`
**Origin:** `LEGACY` — an assumption about the server's error shape, repeated by copy-paste across the codebase.

**Seen:** the budget rectification screen locked behind the loading overlay; only a page reload
recovered it.

**Cause:** the error handler assumed the server's error list was always an array and called
`.map()` on it. When the response had a different shape, the handler itself threw — so the code
that hides the spinner never ran.

**Fix:** the guard now checks the value really is an array. Applied to the thirteen occurrences
in the two files involved.

**When rebuilding:** an error handler must not be able to fail. It is the last line of defence,
and when it throws the user is left with no way forward. This pattern exists elsewhere in the
codebase and was deliberately not swept — if the same symptom appears on another screen, look
for it there first.

---

## INSS-013
**Second expense on the same budget line could not be authorised** · 2026-08-14 · `CODE`
**Origin:** `GAP` — cost centre was made part of allocation but never carried into consumption.

**Seen:** the first expense on a budget line authorised normally; a second one, for a different
cost centre, was refused, and a negative amount was displayed.

**Cause:** two problems. The balance was computed on four keys and **cost centre was not one of
them**, so every cost centre shared one pool even though each has its own allocation. And when
the balance went negative the Authorize button opened a dialog that explained nothing and then
did nothing at all — it never called the save.

**Fix:** cost centre added to the key, making it five. The hard block on duplicates was replaced
by a confirmation dialog listing the expenses already in progress, leaving the decision to the
user; only the real available balance now prevents overspending.

**When rebuilding:** the key used to *spend* a budget must be exactly the key used to *allocate*
it. If allocation is per cost centre, consumption must be too.

---

## INSS-014
**A dropdown was blank and could not be opened** · 2026-08-17 · `CODE`
**Origin:** `LEGACY` — a guard written for lists that were always longer than one.

**Seen:** the Institution field showed nothing and would not open.

**Cause:** the template only rendered the options when there was more than one — so with exactly
one option, none was rendered.

**Fix:** render from one option upwards, and select it automatically when it is the only one.

**When rebuilding:** the same `> 1` pattern exists on four other screens. It is harmless only
while those lists happen to have several entries.

---

## INSS-015
**A deactivated option kept appearing until the next day** · 2026-08-17 · `CONFIG`
**Origin:** `LEGACY` — a deliberate cache with a daily expiry, invisible to anyone changing the underlying data.

**Seen:** an option was deactivated in the database but stayed in the dropdown.

**Cause:** that lookup is served from a cache that expires at midnight. Several other master-data
endpoints share it.

**Fix:** none needed — the backend has to be restarted for the change to be visible immediately.

**When rebuilding:** cached master data needs either a short expiry or an explicit invalidation
when the underlying row changes. "It will be right tomorrow" is not obvious to anyone using it.

---

## INSS-016
**"Invalid object name" on the OSS Global report** · 2026-08-18 · `DATA` `DEPLOY`
**Origin:** `ENV` — the client restored the database to an earlier state, and the repair was run from an outdated copy of the script.

**Seen:** the report failed on a table that had existed for weeks.

**Cause:** the database had been restored to an earlier state, which dropped it. When the script
was re-run, it was an **older copy from a chat attachment** rather than the corrected version in
the repository: the inserts failed, and the script printed a hardcoded success line immediately
afterwards, so the run looked fine.

**Fix:** the script now verifies what it actually created and prints PRESENT/MISSING per item. A
single health-check script was added that reports and repairs every schema item the current code
expects, so a restore no longer surfaces as one broken screen per week.

**When rebuilding:** a script must verify its own result and never print success unconditionally.
Deliver scripts through the repository, and check which version was actually run.

---

## INSS-017
**Budget balance appeared to ignore the activity** · 2026-08-18 · `NOT-A-BUG` + `CODE`
**Origin:** `MISREAD` — an investigation query without ordering or joins made two different activities look like one.

**Seen:** two budget rows of $72,000 and $48,000 on the same account line; registering $72,000
was refused, saying only $48,000 was available.

**Verdict:** correct behaviour. The two rows belong to different activities; the form had the
activity whose budget is exactly $48,000 selected. The filter does use activity — the query used
to investigate simply had no ordering and no join to show which was which.

**Fixed anyway:** the messages now name the activity and cost centre the balance was computed
for. The old text showed only an amount, which reads as if the whole account line were nearly
empty — that is what made the behaviour look wrong.

**Also found while checking:** the consumption side did not filter by budget period while the
allocation side did, so a future year would have subtracted an earlier year's spending. Invisible
in 2026, wrong from 2027.

**When rebuilding:** when refusing an action on a computed figure, state which dimensions the
figure was computed for. A bare number invites the user to conclude the system is broken.

---

## INSS-018
**18 icons vanished from the whole application** · 2026-08-21 · `DEPLOY`
**Origin:** `OURS` — we replaced a hosted asset with a local copy and shipped an outdated build of it.

**Seen:** buttons appeared blank across many screens, including the access-control toggle.

**Cause:** the icon font was self-hosted to remove a dependency on an external CDN, but the file
shipped was an outdated build missing those glyphs. This font renders a missing icon as empty
space — no fallback text, no console error, nothing to search for.

**Fix:** shipped the current font file.

**When rebuilding:** when replacing a hosted asset with a local copy, verify the copy covers
everything actually used. Failures that produce *nothing* are far more expensive to find than
failures that produce an error.

---

## INSS-019
**Attaching a document failed with "Something went wrong"** · 2026-08-21 · `INFRA`
**Origin:** `ENV` — a default in the reverse proxy that only exists in front of Staging and Production.

**Seen:** on Staging, attaching a PDF failed. Even a 1 MB file failed. It worked on Dev.

**Cause:** the reverse proxy in front of the API was running with nginx's default upload limit of
1 MB. Files are sent base64-encoded, which makes the request roughly 1.33× the file size, so
anything above about 750 KB was rejected before it ever reached the application.

**Fix:** the client raised `client_max_body_size` on the API server block.

**When rebuilding:** Dev has no proxy in front of it, so no proxy-caused failure can ever be
reproduced there. When a problem appears only on Staging or Production, check the proxy before
reading application code.

---

## INSS-020
**The payment total was printed on every page** · 2026-08-22 · `LIBRARY`
**Origin:** `DEFAULT` — the PDF library repeats footers unless told not to, and none of the three copies said otherwise.

**Seen:** "Total Pagamentu" appeared at the foot of every page of the payment order, each time
showing the full total for that bank — as if each page had closed its own accounts.

**Cause:** the PDF table component repeats the footer on every page unless told otherwise, and
none of the three places that generate this document said otherwise.

**Fix:** the footer is printed once, at the end of each bank's list. Each bank keeps its own
total, because each is a separate transfer.

**When rebuilding:** check a library's defaults for anything that is visible to the user. Also:
this document is generated by **three near-identical copies** of the same code — any change to
it has to be applied to all three.

---

## INSS-021
**Recipient name and NISS were blank on the payment order** · 2026-08-22 · `CODE`
**Origin:** `LEGACY` — inherited code that discarded fields in the name of normalising the record.

**Seen:** the payment order printed empty NISS and name columns while account number and IBAN
were correct. The staff insisted they had entered the data — and they had.

**Cause:** when saving a payment, the backend built the recipient object in full and then, if the
recipient was a worker or an employer, **threw that object away** and rebuilt it with only the
foreign key. Name, NISS, TIN and address were discarded before reaching the database. For a
recipient that already existed, the same path continued into an update that copied the empty
values over the correct ones — destroying good data.

Why it looked inconsistent: the lookup checks the recipient table first and only falls back to
the worker registry when nothing is found. That fall-back is what sets the foreign key, so the
destructive branch ran exactly for people who had never been paid before. Anyone already on file
was unaffected, which is why some payment lists showed names and others did not.

**Fix:** the descriptive fields are kept; only the rule that the two foreign keys are mutually
exclusive was preserved. The defect existed in **two identical copies** of the same method and
both were corrected. A repair script refills the names already lost, from the registries that
still hold them.

**Order matters:** deploy the code first, then run the repair. In the other order the next
payment entry blanks everything again.

**When rebuilding:** never discard fields the client just supplied in order to "normalise" a
record. And an update that copies nulls over existing values will eventually erase real data —
decide explicitly whether a null means "clear this" or "leave it alone".

---

## INSS-022
**Every Excel export failed on the Linux servers** · 2026-08-22 · `LIBRARY` `INFRA` `CODE`
**Origin:** `ENV` — a component Windows ships with the operating system and Linux does not.

**Seen:** the Excel buttons returned an error. The same buttons worked on Dev.

**Cause:** the spreadsheet library depends on a graphics component that Windows provides as part
of the operating system and Linux does not. The container image did not have it installed, so the
first call into it failed.

**Fix:** two parts, and both are wanted.

- *Code.* The graphics component was only ever needed to size the columns to their content. The
  export component now attempts that sizing and, if the component is unavailable, produces the file
  without it and records a warning, instead of failing the request. The spreadsheet is correct
  either way; only the column widths differ. One change, in the shared export component, covers all
  fourteen exports.
- *Server.* The library is installed in the backend image, so the columns are sized as intended.
  Dev needs nothing — it runs on Windows.

Verified end to end on a Linux container without the library: the file downloads, and the log
records the warning where it previously recorded the failure.

**When rebuilding:** Dev is Windows, Staging and Production are Linux containers. Anything
touching graphics, fonts, file paths or case-sensitive filenames can work on Dev and fail in
production. Note also that this affected **every** Excel export in the application, not only the
screen that was reported, because they share one export component. The wider lesson is the code
one: a cosmetic step must not be able to fail the request that contains it. Column widths are not
worth an error dialog.

---

## INSS-023
**"Something went wrong" said nothing at all** · 2026-08-22 · `CODE`
**Origin:** `LEGACY` — the front-end was written to show friendly text and dropped the server's message to do it.

**Seen:** the generic error message appeared for two entirely different problems in two days
(INSS-019 and INSS-022). Both took browser developer tools or server logs to identify.

**Cause:** the server was already returning a real message; the front-end read only the error
code and discarded the text.

**Fix:** the error dialog now shows the HTTP status, the endpoint and the server's own message
underneath, in small grey type. Captured once in the HTTP layer, so it covers every screen. It
only appears when the code is generic — messages that already read well are unchanged.

**When rebuilding:** a user-facing error must carry enough for a support engineer to act on a
screenshot alone. Discarding the server's message to show a friendlier one costs hours later.

---

## INSS-024
**The "AD" tab of the Despesas report also listed committed expenses** · 2026-07-17 · `CODE`
**Origin:** `LEGACY` — a filter written against two states when the tab means one.

**Seen:** the five tabs of the Despesas report (AD / Cabimentada / Compromisso / Obrigação /
Executada) did not agree with what the client expected to find in each.

**Cause:** the AD tab filtered on **two** states instead of one, so it showed authorised and
committed expenses mixed together. Calling the API directly for all five tabs showed the other
four were behaving correctly — where the client saw an empty tab, it was because no expense
happened to be sitting in that state at that moment, not a defect.

**Fix:** the AD tab filters on the authorised state only. A dead branch left commented out in
the "Executada" path was also removed after confirming it changed no behaviour.

**When rebuilding:** an empty screen is not evidence of a defect. Query the API directly per
state before assuming one, otherwise real defects and empty data look identical.

---

## INSS-025
**The Obrigação tab reported a payment status, not a real Obrigação** · 2026-07-17 · `CODE` `CONFIG`
**Origin:** `GAP` — a legally required step had no place in the system, so a report improvised one from payment status.

**Seen:** the Obrigação tab did not reflect the client's legal process.

**Cause:** the legal process has five steps — AD, Cabimento, Compromisso, **Obrigação**,
Pagamento — where Obrigação is the confirmation that the goods or services were actually
delivered. The system had no such step: the report used "payment order issued" as a stand-in.
The task that resembled it ran **before** Compromisso existed, so it could not confirm delivery
against a commitment.

**Fix:** two parts. Configuration — the task was moved to run after Compromisso, and two
existing components were attached to it: a mandatory invoice attachment and a mandatory
confirmation-of-receipt text. Code — the report now checks those two real things instead of the
payment-status stand-in. Verified end to end by filling the invoice and text on a real expense
and watching it appear in the tab.

**When rebuilding:** if a legally required step has no place to record it, a report cannot
reconstruct it from something else. Model the step. Note also that the ordering of tasks is
**configuration** — it does not travel with a code deployment and must be applied per environment.

---

## INSS-026
**The Budget Execution report showed nothing under Despesa** · 2026-07-17 · `DATA`
**Origin:** `SIDE-EFFECT` — a column was added and became a filter, while the rows that predated it were never backfilled.

**Seen:** the report returned no execution data for 2025.

**Cause:** the report filters by Institution, and every approved 2025 budget line had no
Institution set. The field was added after that data already existed and the existing rows were
never backfilled. Only a single 2026 test row had one — and that row returned data correctly.

**Fix:** backfilled the Institution on all 135 affected rows. The report went from zero rows to
56 rows of real execution data.

**When rebuilding:** when a new column becomes a filter, existing rows must be backfilled in the
same change. A filter on a column that is null everywhere silently returns nothing, which reads
as a broken report.

---

## INSS-027
**Payment order grouped by bank, month and year** · 2026-07-17 · `FEATURE`

**Asked for:** the payment order was printed one payment at a time; the client needed one list
per bank per month, which is what a bank transfer actually needs.

**Built:** the document is grouped by bank + month/year, one page per group, with sequence
number, NISS, name, account, IBAN and amount, plus the two signature blocks. Both the screen that
issues payments and the screen that reviews history produce the same layout.

**When rebuilding:** this document is generated in **three** places (issue, review, and the
reconciliation screen's document icon). They are near-identical copies — every later change to it
had to be applied three times. Build it once.

---

## INSS-028
**Reconciliation: view the uploaded proof, and filter by date** · 2026-07-13 · `FEATURE`

**Asked for:** approve a payment guide against the real bank statement, see the evidence the
company uploaded, and cope with a screen that will accumulate volume over the years.

**Built:** a two-panel picker matching pending guides against unreconciled bank lines by amount, an
inline overlay showing the company's declared details with a PDF preview, and a date range on both
panels defaulting to a rolling three months.

**When rebuilding:** "paid" must mean *money confirmed received*, not *document looks acceptable*.
The screen this replaced approved on a self-reported amount with no bank check at all.

---

## INSS-029
**Remaining balance shown on the expenditure screens** · 2026-08-01 · `FEATURE`

**Asked for:** officers could not see how much budget was left before committing to an amount.

**Built:** remaining-balance columns on the expenditure screens, and the balance rules enforced at
entry rather than only at approval (see INSS-010).

**When rebuilding:** show the constraint at the moment of the decision. A rule the user cannot see
until someone else rejects their work is experienced as an obstruction.

---

## INSS-030
**Classificação Económica report and crosswalk** · 2026-07 to 2026-08 · `FEATURE`

**Asked for:** the OSS Global report, which the client confirmed as their first priority.

**Built:** the crosswalk between the account tree and the economic classification catalogue, plus
the report on both sides of the application.

**When rebuilding:** the catalogue already existed in the database but was seeded and unused, and a
database restore later dropped the crosswalk table (INSS-016). Anything that lives only in a
migration is one restore away from disappearing.

---

## INSS-031
**Budget adjustment (top-up and transfer)** · 2026-08-11 · `FEATURE`

**Asked for:** a way to move or add budget after execution had already started, which the existing
rectification flow cannot do.

**Built:** an adjustment feature that tops up or transfers between budget lines, tested end to end.

**When rebuilding:** rectification replaces a whole year and is only valid before execution. Once
money has been committed, the only safe operation is an adjustment that leaves history intact.

---

## INSS-032
**Expense uniqueness on five keys, with a confirmation instead of a block** · 2026-08-17 · `FEATURE`

**Asked for:** after INSS-013, the client decided what "the same expense" means: Institution +
Cost Centre + Activity + Functional + budget line. Department deliberately excluded.

**Built:** the five-key rule across allocation and consumption. When all five match, the system no
longer blocks — it lists the expenses already in progress and lets the user decide. Only the real
available balance prevents overspending.

**When rebuilding:** this was the client's explicit choice among three options — warn rather than
block. They also reframed FRSS as a *Regime* (a cost centre), not an institution; INSS is the only
institution. That reframing is not visible anywhere in the code.

---

## INSS-033
**Document attachments: real file name, working picker, duplicate warning** · 2026-08-20 · `FEATURE`

**Asked for:** the attachment list showed only the document *type*, so two different files of the
same type were indistinguishable; and the file field could not be opened at all.

**Built:** the file name is now stored and listed, with a sequence column; the file picker was
replaced with a native one that opens; attaching a second document of a type already present warns
first; and files attached during the session stay listed on screen, because the success message
disappears after a few seconds and the field is cleared.

**When rebuilding:** the file name was already being sent by the browser and thrown away by the
backend — the same shape of defect as INSS-021. Check what the client is already sending before
adding a field.

---

## INSS-034
**Warning when the account number disagrees with the IBAN** · 2026-08-22 · `FEATURE`

**Asked for:** after INSS-021 was fixed, the payment order showed account numbers that were in fact
the employee's NISS. A wrong account number sends the money to the wrong person, and the payment
order cannot detect that on its own.

**Built:** a national IBAN contains the account number (TL38 + 3 bank digits + 14 account digits +
2 check digits), so the two fields can be checked against each other at entry. When they disagree,
the screen shows both numbers and asks whether to continue.

It **warns rather than blocks**, deliberately: a false block would stop a legitimate payroll
payment, and the check does not apply to foreign IBANs or to formats not yet seen. Skipped
entirely for the Excel import path, which has no single account field.

**When rebuilding:** where one field can be derived from another, verify them against each other at
the point of entry. This class of error is invisible in every downstream document.

---

## INSS-035
**Name and account number left-aligned on the payment order** · 2026-08-22 · `FEATURE`

**Asked for:** on the payment order PDF, the employee name and the bank account number were
centred in their columns. Names vary in length and accounts vary in digit count, so every row
started at a different horizontal position and the columns could not be scanned down.

**Built:** both columns are now left-aligned in the body of the table. The column headers stay
centred, matching every other header; the amount column keeps its right alignment. Applied to all
three places that produce this document — issuing the payment order, listing executed payments and
the reconciliation screen — which share the design but not the code.

**When rebuilding:** identifiers and names are read by scanning a column downwards, so they belong
left-aligned; amounts belong right-aligned so the decimal points line up. Centring is for headers.
Note also that this one document is generated from three separate copies of the same code — a
change to its appearance has to be made three times, and is easy to make in only one.

---

---

## INSS-036
**Company accounts had no name on Users Access Control** · 2026-08-23 · `CODE`
**Origin:** `LEGACY` — the screen was written for one kind of account and the other kind was never considered.

**Seen:** on *Users Access Control*, the **Username** column was empty on every row. Reported as
missing data, alongside the *Actions* buttons not appearing.

**Cause:** a login belongs to one of two different kinds of subject. Staff logins point at a
worker record (`trabalhador_fk`); the company logins used by the Contributions module point at an
employer record (`utilizador_entidade_fk`). The query read the worker name and nothing else, so a
company account could never show a name — not for want of data. The company names were in the
database the whole time.

The blanks looked total on the client's environment only because the accounts they were looking at
happened to be company ones.

**Fix:** the name falls back to the employer's name when there is no worker. The search box
matches the employer name too — without that, the screen would show a name that the search could
not find, which reads as a second defect. The `admin` account stays blank: it belongs to neither
subject, and there is nothing to fall back to.

**Not part of this:** the missing *Actions* buttons reported in the same message were not a defect.
Those are gated on UPDATE rights for this screen, and the profile in use did not have them. Note
for support: rights are read from the token issued at login, so granting them requires the user to
log out and back in before the buttons appear.

**When rebuilding:** where a foreign key is one of several that answer the same question — *who is
this record about?* — reading only one of them fails silently for the rest. It shows as absent
data, so it gets reported as a data-entry problem and looked for in the wrong place. Whenever a
displayed field is derived from an optional relationship, ask what the other kinds of row show.

---

## Backlog

Identified while working on something else, analysed, and set aside to be implemented as its own
piece of work rather than squeezed into the change that revealed it. Each row states what it
would take and what it costs to wait, so it can be scheduled on evidence instead of memory.

| # | What | Why it needs its own pass | What it costs to wait |
|---|---|---|---|
| D-01 | **Carry the closing balance forward into the next year's opening balances** — a button that computes the prior year's real closing balance, pre-fills the table and lets the user adjust before confirming | Designed in detail; needs the prior year's full transaction history to compute against, so it is a piece of work in its own right. Year 1 has nothing to carry from, so manual entry was enough to start | Every subsequent year is entered by hand, against a figure the system already holds. The real ledger has a literal "SALDO TRANSITADO ANO ANTERIOR" line, so the concept is expected |
| D-02 | **Two-step accrual booking** — book receivable/revenue when the payment guide is issued, and bank/receivable when it is collected | Changes when entries are written, not just what they contain — it touches the guide, the collection and the reports together | The books do not show money owed but not yet received, which is what an accrual system is for |
| D-03 | **~97 duplicate account codes** across the chart of accounts, from two parallel imports | The ~9 codes needed for the payment work were corrected. The rest cannot be touched safely without cross-referencing every table that uses them first | Any new screen over the account tree can pick the wrong duplicate. One of the two batches is corrupted at every level |
| D-04 | **119 inactive account rows with no audit trail** — leftover seed data | Made visible with a status badge, which solved the reported symptom; deciding what to do with each row needs the client | They are still there, and still meaningless to whoever reads the tree |
| D-05 | **The error-handler pattern that can throw** (INSS-012) was fixed in 2 files, not swept | Scoped to the two files involved to keep that change reviewable; the sweep is a separate pass with its own testing | The same frozen-screen symptom can still appear anywhere else the pattern exists |
| D-06 | **Dropdowns that render no options when the list has exactly one** (INSS-014) — the same guard exists on 4 other screens | Outside the screen being fixed; each needs its own check | Each of them breaks the day its list drops to a single entry |
| D-07 | **Account-code search matches only the code, not the description** — typing "Água" returns nothing | Needs the search to index the description as well as the code | Users must know the numeric code to find anything |
| D-08 | **An expense authorised against an unfunded budget line becomes permanently stuck** — it cannot be committed, and the delete action only exists while it is still registered | Needs a decision from INSS on what the recovery path should be | The only way out is a database intervention, which the client cannot do themselves |
| D-09 | **No bulk entry for Cabimento or Compromisso** — only the payment stage accepts an Excel import | Not requested so far | Every line is entered one at a time, for processes that routinely have dozens |
| D-10 | **No single audit-log viewer** — there are three separate mechanisms (a log file, per-record columns, and text history) | Not requested so far as one feature | "Who changed this, and when" cannot be answered from the application |
| D-11 | **Upload limits disagree across the stack** — the screen accepts 50 MB, the application server stops at 30 MB | Surfaced while fixing INSS-019, where the proxy limit was the binding one; aligning the three limits needs a decision on the real maximum | A file between the two limits is accepted by the screen and rejected by the server |
| D-12 | **Creating a user does not create login credentials** | The screen predates this engagement and the legacy login path it has to satisfy | New accounts need a second, manual step that is not visible anywhere |
| D-13 | **Commitment numbering gap** found during the January import review | Flagged as blocking; superseded at the time by the report INSS confirmed as first priority | Numbering is relied on by the ledger |
| D-14 | **"Who approved this" is not displayed** anywhere, though it is stored | Reported alongside D-13 and carried with it | An approval chain that cannot be read is hard to audit |
| D-15 | **Status shown as a raw letter** ("R", "A") in the in-progress expenses dialog | Needs the status codes mapped to readable labels wherever they are shown | The dialog asks the user to make a decision using a code only developers read |
| D-17 | **The next payment-order number is derived from a single character** — `GetOrcamentoAprovadoDespesaByIdTarefaActivo` reads the sequence as the *first character* of the previous payment number, and takes "previous" as the last row of an unordered query | Correcting it changes how payment numbers are generated, and needs a decision on what to do with any process that has already passed its ninth payment order | From the tenth payment order in a process onward, the number generated repeats one already in use, silently — `10/08146/2026` is read as `1`, so the next is offered as `2`. Separately, a payment number that does not start with a digit makes the whole expenditure screen fail to load |
| D-16 | **Economic-classification crosswalk mapped at root level only** — the remaining ~150 leaf nodes were verified correct at root level but not mapped to an exact sub-level target | The report aggregates at root level today; the mapping is only needed once it breaks down further | Needed before the report can break down below root level |

### A. New capability — to propose and schedule with INSS

These add something the system does not do today, and each answers a need already expressed.

| # | Proposal |
|---|---|
| D-01 | Carry the closing balance forward into the next year's opening balances |
| D-02 | Two-step accrual booking — record the obligation when the guide is issued, not only when it is collected |
| D-09 | Bulk entry for Cabimento and Compromisso, as the payment stage already has |
| D-10 | A single audit-log viewer answering "who changed this, and when" |
| D-12 | Create the login credentials together with the user account |
| D-13 | Commitment numbering |
| D-14 | Show who approved each step |
| D-16 | Break the economic-classification report down below root level |

### B. Further improvements suggested

Refinements to what already exists — data quality, robustness and day-to-day usability. Smaller
than section A, and each one removes a specific irritation or risk that is present today.

| # | Suggestion | What it gives |
|---|---|---|
| D-03 | Resolve the duplicate account codes across the chart of accounts | Removes the risk of a screen picking the wrong one of two identical codes |
| D-04 | Decide what to do with the inactive rows left over from earlier imports | A chart of accounts where every row means something |
| D-05 | Apply the error-handling fix from INSS-012 across the remaining screens | Prevents the same frozen-screen symptom appearing elsewhere |
| D-06 | Apply the single-option dropdown fix from INSS-014 across the remaining screens | Prevents a field going blank the day its list drops to one entry |
| D-07 | Let the account search match the description, not only the numeric code | Users can find an account by name instead of memorising codes |
| D-08 | Give an expense authorised against an unfunded line a way out | Removes a dead end that currently needs database access to escape |
| D-11 | Align the upload limits across the screen, the proxy and the application server | One predictable maximum instead of three that disagree |
| D-15 | Show statuses as readable labels wherever the raw code still appears | Users decide on words rather than on single letters |
| D-17 | Derive the next payment-order number from the whole number rather than its first character, and from the highest number in use rather than an arbitrary row | Numbering stays correct past the ninth payment order in a process, and an unexpected number no longer blanks the expenditure screen |

**When rebuilding:** D-03 to D-07 share one shape — a correction applied where the problem was
reported rather than everywhere it exists. If the rebuild inherits any of this code, these are the
places to start, because each one is already known and located.

**Keep adding to this list.** Anything raised that turns out to be needed belongs here the moment
it is identified, with the same two columns: what it would take, and what it costs to wait. That
is what makes it possible to come back with a considered proposal instead of a recollection.

## Patterns worth carrying into any rebuild

Six of the issues above are the same mistakes in different places:

1. **Duplicated code drifts.** The payment order exists in three copies (INSS-020); the recipient
   mapping in two (INSS-021). Every fix had to be applied to all of them, and finding the copies
   was most of the work.
2. **A fix in one component is not a fix of the problem.** INSS-006 repeated INSS-003 because the
   original fix lived in one screen rather than in a shared helper.
3. **Manually maintained flags go stale** (INSS-008). Derive instead.
4. **Block-lists rot** (INSS-007). Allow-list what is meant.
5. **Dev cannot reproduce environment problems** (INSS-019, INSS-022). Different OS, no proxy.
   Before concluding "it must be their data", ask whether the code path touches the platform.
6. **Silent failures cost the most.** Blank icons (INSS-018), a script printing success after
   failing (INSS-016), a dialog that explains nothing and does nothing (INSS-013), an error
   handler that throws (INSS-012). Every one of them was cheap to fix and expensive to find.
