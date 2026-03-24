## PHASE 1 - DEFINITION

### 1. XY-Chain & Specificity (e.g. not "solve UI", but "user can't find item XY in UI":
- clean shader-access-diagram
- shader-access-hierarchy overview as diagram

### 2. Description
| **input**       | **behaviour**                         | **constraints**            | **output**                                |
|-----------------|---------------------------------------|----------------------------|-------------------------------------------|
| Script, Shaders | change in Script propagates to shader | linearity, no backtracking | clear overview of shader access hierarchy |

### 3. Rice:
| Reach (#use-cases) | Impact (0-3) | Confidence | Est. Effort |
|--------------------|--------------|------------|-------------|
| 2                  | 1            | medium     | 2h          |
Begin-Time: 2026-03-24, 16:07
Finish-Time: 

-> switch (use-case * impact): 
- <=3: brute-force <= 1h or backlog
- ~~4-6: acceptable solution <= 1day or backlog~~
- ~~\>=7: elegant solution~~

### 4. Kill Duck: 
am I creating this, only because it ... (strike-through wrong ones)
- ... is intellectually interesting?
- ... appears cool?  
- ~~... is fun to make?~~  
- ... helps an imaginary future? 
-> any yes = backlog
=> I should actually keep this minimal; no rework, just make a quick overview diagram; 
- Additionally this shows less skill / consideration than the lockableVector-class

==> Aborted

###  Workflow: : Summary : 

# ________

## PHASE 2 - DESIGN

### Research: 
switch (complexity): 
 - **pre-built**: quick-check for reuse
 - **similar**: similarity-table 
 - **custom feature**: 
  - research <=min(0.5days, 3 answer) -> comparison table 
  - choose one and test <=(0.5day, acceptable test) -> result table
 - **custom system**:
   - research <=(2days, 3 answers) -> comparison table
   - choose 2 test each <=(1.5 days, acceptable test) -> result table

| solution | advantage | disadvantage |
|----------|-----------|--------------|
|          |           |              |
|          |           |              |


###  Workflow: is research done?
### Research Summary: 

### Happy-Path: 
- **simple** (<= 1hour): pseudo-code lines
- **default** (<= 1day): flowchart & rubber-duck
- **complex** (week): separate into tasks
- **refactor**: check current documentation, goto corresponding case

### Kill Duck
- am I using this solution, only because it ...
- ... is intellectually interesting?
- ... appears cool?
- ... is fun to make?
- ... helps an imaginary future?
-> any yes = kill

###  Workflow: confirm happy-path
### Happy-Path Summary:

### Edge-Cases: 
- 5 min brainstorm (technical issues, user stupidity, internal curruption) into frequency-impact-time-list: 

| case | **frequency** | **impact** | solution-idea | **solve-time** | solve? |
|------|---------------|------------|---------------|----------------|--------|
|      |               |            |               |                |        |

### implement cases into Solution (from Happy-Path)

### Kill Duck: 
- implementable without further thinking?
- is it "boring"?
  - common patterns?
  - no surprises?
  - obvious error handling?
  - backwards-compatible?

###  Workflow: confirm solution design
### Solution Summary: 

# ________

## PHASE 3 - IMPLEMENTATION

### Happy-Path: 
- implement feature-documentation
- implement solution
- implement happy-path test
- compare with design

###  workflow: test success? continue!

### Edge-Cases: 
- implement edge-case-documentation
- implement edge-case test
- implement solution
    - parameterize if necessary
    - extract if necessary
    - rename new variables/functions
    - no structural changes (= no abstraction, no extra classes)

###  workflow: tests succeed? continue!

# ________

## PHASE 4 - POSTMORTEM:

### compare: 

| planned | executed |
|---------|----------|
|         |          

work problems list: 
- meow

success list: 
- meow

| estimated time | actual time |
|----------------|-------------|
|                |             |

### recheck alternatives

# ________

## PHASE 5 - Feedback: 

Notes: 
- 