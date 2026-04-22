## PHASE 1 - DEFINITION

### 1. XY-Chain & Specificity (e.g. not "solve UI", but "user can't find item XY in UI":
- Find Usability Issues and fix them if necessary

### 2. Description
| **input**                          | **behaviour** | **constraints** | **output**          |
|------------------------------------|---------------|-----------------|---------------------|
| me testing (maybe test-framework?) |               |                 | fixed bugs / issues |

### 3. Rice:
| Reach (#use-cases) | Impact (0-3) | Confidence | Est. Effort |
|--------------------|--------------|------------|-------------|
| ?                  | ?            | medium     | 3h          |
Begin-Time: 2026-13-03, 15:30
Finish-Time: 

### 4. Kill Duck: 
am I creating this, only because it ... (strike-through wrong ones)
- ~~... is intellectually interesting?~~
- ~~... appears cool?~~  
- ~~... is fun to make?~~  
- ~~... helps an imaginary future?~~ 
-> any yes = backlog

###  Workflow: : Summary : 
I need to test to ensure the main functionality works & when bugs exist they don't brake everything. 100% test coverage is not required, I don't need to fix every detail; just the ones which actually brake stuff. 

# ________

## PHASE 2 - DESIGN

| issue                                    | **frequency** | **impact** | solution-idea     | **solve-time** | solve?           |
|------------------------------------------|---------------|------------|-------------------|----------------|------------------|
| AngleTypeSelectors have no default-value | 3             | 2          | set default value | ?              | CHECK complexity |
| AxisAngle-RotationAxis is not normalized | 3             | 3          |                   |                |                  |
|                                          |               |            |                   |                |                  |

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