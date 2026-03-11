## PHASE 1 - DEFINITION

### 1. XY-Chain & Specificity (e.g. not "solve UI", but "user can't find item XY in UI":
- RotParams convert into one another
- User can convert from one RotParams to another
- -> User can convert from one RotParam+Vis+UI to another
- RotParams+Vis+UI Conversion is accesible from a cleanly documented location

### 2. Description
| **input**                                | **behaviour**                                            | **constraints** | **output**                       |
|------------------------------------------|----------------------------------------------------------|-----------------|----------------------------------|
| Click Dropdown -> other Parameterisation | convert RotParams, siwtch active Vis & UI(, bind UI/Vis) |                 | switched Parameterisation+Vis+UI |

### 3. Rice:
| Reach (#use-cases) | Impact (0-3) | Confidence | Est. Effort |
|--------------------|--------------|------------|-------------|
| 3                  | 3            | medium     | 1 day       |

Begin-Time: 2026-10-03, 17:06
Finish-Time: 

-> switch (use-case * impact): 
- \>=7: elegant solution

### 4. Kill Duck: 
am I creating this, only because it ... (strike-through wrong ones)
- ... is intellectually interesting?
- ~~... appears cool?~~  
- ~~... is fun to make?~~  
- ... helps an imaginary future? 

###  Workflow: : Summary : 
I need to be a bit careful, that I don't overdo the conversion system for some future parameterisations (that don't exist) or because some system seems innovative. 

# ________

## PHASE 2 - DESIGN

### Research: 
switch (complexity): 
 - **similar**: similarity-table 
 - **custom feature**: 
  - research <=min(0.5days, 3 answer) -> comparison table 
  - choose one and test <=(0.5day, acceptable test) -> result table

```mermaid
classDiagram
    class RotParams_Base{
        +CopyValues()
        +ResetToIdentity()
        +ToRotParams_XYZ()
        +ToSelfType(RotParams_Base otherParams)
        +From/ToUnityQuaternion()
        +RotateVector()
        
        +Concatenate()?
        +Concatenate_Implementation()?
    }

    RotParams_Base <|.. RotParams_XYZ
    class RotParams_XYZ{
        +ToRotParams_XYZ()
    }

    class RotUI_XYZ["RotUI_XYZ<br>(not a C# class <br> but a UI asset)"]
    
    class RotVis_GenericBase{
        +SetRotParamsByRef()
        +VisUpdate()
        +Get/SetRotParams
    }
    
    RotVis_GenericBase <|.. RotVis_TemplateBase
    class RotVis_TemplateBase{
        #rotParams
        +Get/SetRotParams
    }
    
    RotVis_TemplateBase <|.. RotVis_XYZ
    class RotVis_XYZ{
    }
    
    class RotCot_GenericBase{
        #rotObjPrefab
        +abstract Get/SetRotParams_Generic
        +abstract Get/SetRotVisGeneric
        +Get/SetRotObjCot
        
        +selfInitOnStart/OnAwake
        +abstract Initialize(Transform parent, VisualElement UIParent, RotatedObjectContainer)
        +OnPropertyChangedVisUpdate()
    }
    RotCot_GenericBase <|.. RotCot_TemplateBase
    
    class RotCot_TemplateBase{
        -rotVisPrefab
        -rotVisScript
        +RotParams - copies the ref, not the value
        +Get/SetRotParams_Generic
        +Get/SetRotVisGeneric
        +Initialize
        -SpawnVis/UI/RotObjCot
    }
    RotCot_TemplateBase o.. RotParams_Base
    RotCot_TemplateBase o.. RotVis_GenericBase
    
    RotCot_TemplateBase <|.. RotCot_XYZ : 1 foreach RotType
    class RotCot_XYZ{
        #VariableBindings
    }
    RotCot_XYZ o.. RotParams_XYZ
    RotCot_XYZ o.. RotVis_XYZ
    RotCot_XYZ o.. RotUI_XYZ
    RotCot_XYZ o..RotatedObjectContainer
    
    class RotatedObjectContainer{
        -appliedRotationObject
        -rotationObjectsParent
        -currentlyActiveRotationObject
        +ApplyRotation
        +Set/ResetAppliedRotation
        +Get/SetRotation
    }
    
    class ComboRotCot {
        -List: RotCot_XYZ
        +initializeOnStart/OnAwake
        -rotObjCotPrefab
        +rotatedObjectContainer
        +UI: UIAsset, panelSettingsAsset, uiComboContainerParentName, ...
        +Cam: cameraPrefab, cameraRotationPivot, visCamera
        +CoordinateGrid
        
    }
    ComboRotCot *-- RotCot_XYZ : 1 foreach RotCotType
    ComboRotCot o.. RotatedObjectContainer
```

Initialization-Sequence: 
```mermaid
sequenceDiagram
    participant UnityInstance
    participant ComboRotCot
    participant RotatedObjectContainer
    participant ListRotCot["List#60;RotCot_XYZ#62;"]
    participant RotVis_XYZ
    participant RotUI_XYZ
    participant RotParams_XYZ
    
    UnityInstance ->> ComboRotCot: Awake
    ComboRotCot ->> ComboRotCot: SelfInitialize (if onAwake)
    UnityInstance ->> ComboRotCot: Start
    ComboRotCot ->> ComboRotCot: SelfInitialize (if onStart)
    
    ComboRotCot ->> ComboRotCot: InitUI
    ComboRotCot ->> ComboRotCot: FindOrCreateUIDocument 
    ComboRotCot ->> ComboRotCot: ComboContainerRoot into UIDocument 
    ComboRotCot ->> ComboRotCot: FindAndCacheUIMenuLine (&dataSource = this)
    ComboRotCot ->> ComboRotCot: UIRotParamsSlot
    
    %% do I need the UIRotParamsSlot here or should I simply add the UI and then de(activate) it, so that I don't have to continuously rebind?
    
    ComboRotCot ->> ComboRotCot: InitCam
    ComboRotCot ->> ComboRotCot: FindOrCreateCamPrefab
    ComboRotCot ->> ComboRotCot: SetCameraPivot
    
    ComboRotCot ->> ComboRotCot: InitRotObjCot
    ComboRotCot ->> ComboRotCot: FindOrCreateRotObjCotPrefab
    
    ComboRotCot ->> +ComboRotCot: InitializeRotCots
    ComboRotCot ->> RotCot_GenericBase: foreach in rotCotsList: Initialize(this.transform, uiRotParamsSlot, rotatedObjectContainer)
    
    RotCot_GenericBase ->> +RotCot_GenericBase: SpawnVis
    RotCot_GenericBase ->> RotCot_GenericBase: Destroy Old Visualisation Game Object
    %% TodoZyKa RotParams_Conversion: Do I really need to destroy the old visualisation
    RotCot_GenericBase ->> RotCot_GenericBase: Spawn New Visualisation from Prefab
    RotCot_GenericBase ->> RotCot_GenericBase: Get & Store the Visualisation script of the newly spawned prefab
    
    RotCot_GenericBase ->> RotVis_XYZ: SetRotParamsByRef
    RotVis_XYZ ->> RotVis_XYZ: unsubscribe rotParams.PropertyChanged
    RotVis_XYZ ->> RotVis_XYZ: set rotParams to new value
    RotVis_XYZ ->> RotVis_XYZ: subscribe to new rotParams.PropertyChanged
    RotVis_XYZ ->> RotVis_XYZ: VisUpdate
    RotVis_XYZ ->> RotCot_GenericBase: _
    RotCot_GenericBase ->> -RotCot_GenericBase:_
    
    RotCot_GenericBase ->> RotCot_GenericBase: SpawnRotObjCot (Spawn new oriented Object or replace reference)
    
    RotCot_GenericBase ->> +RotCot_GenericBase : SpawnUI
    RotCot_GenericBase ->> RotCot_GenericBase: Remove Old UI if it exists
    RotCot_GenericBase ->> RotCot_GenericBase: Spawn new UI and clone it into rotUIAsset
    RotCot_GenericBase ->> -RotCot_GenericBase: set dataSource of rotUIRoot to rotParams
    
    RotCot_GenericBase ->> ComboRotCot: finished InitializeRotCots
    ComboRotCot ->> -ComboRotCot: ActivateRotCot
```

Simplified Class-Diagram: 
```mermaid
classDiagram
    class RotParams_Base{
        +CopyValues()
        +ResetToIdentity()
        +ToRotParams_XYZ()
        +ToSelfType(RotParams_Base otherParams)
        +From/ToUnityQuaternion()
        +RotateVector()

        +Concatenate()?
        +Concatenate_Implementation()?
    }

    RotParams_Base <|.. RotParams_XYZ
    class RotParams_XYZ{
        +ToRotParams_XYZ()
    }

    class RotUI_XYZ["RotUI_XYZ<br>(not a C# class <br> but a UI asset)"]

    class RotVis_GenericBase{
        +SetRotParamsByRef()
        +VisUpdate()
        +Get/SetRotParams
    }

    RotVis_GenericBase <|.. RotVis_TemplateBase
    class RotVis_TemplateBase{
        #rotParams
        +Get/SetRotParams
    }

    RotVis_TemplateBase <|.. RotVis_XYZ
    class RotVis_XYZ{
    }

    class RotCot_GenericBase{
        #rotObjPrefab
        +abstract Get/SetRotParams_Generic
        +abstract Get/SetRotVisGeneric
        +Get/SetRotObjCot

        +selfInitOnStart/OnAwake
        +abstract Initialize(Transform parent, VisualElement UIParent, RotatedObjectContainer)
        +OnPropertyChangedVisUpdate()
    }
    RotCot_GenericBase <|.. RotCot_TemplateBase

    class RotCot_TemplateBase{
        -rotVisPrefab
        -rotVisScript
        +RotParams - copies the ref, not the value
        +Get/SetRotParams_Generic
        +Get/SetRotVisGeneric
        +Initialize
        -SpawnVis/UI/RotObjCot
    }
    RotCot_TemplateBase o.. RotParams_Base
    RotCot_TemplateBase o.. RotVis_GenericBase

    RotCot_TemplateBase <|.. RotCot_XYZ : 1 foreach RotType
    class RotCot_XYZ{
        #VariableBindings
    }
    RotCot_XYZ o.. RotParams_XYZ
    RotCot_XYZ o.. RotVis_XYZ
    RotCot_XYZ o.. RotUI_XYZ
    RotCot_XYZ o..RotatedObjectContainer

    class RotatedObjectContainer{
        -appliedRotationObject
        -rotationObjectsParent
        -currentlyActiveRotationObject
        +ApplyRotation
        +Set/ResetAppliedRotation
        +Get/SetRotation
    }

    class ComboRotCot {
        -List: RotCot_XYZ
        +initializeOnStart/OnAwake
        -rotObjCotPrefab
        +rotatedObjectContainer
        +UI: UIAsset, panelSettingsAsset, uiComboContainerParentName, ...
        +Cam: cameraPrefab, cameraRotationPivot, visCamera
        +CoordinateGrid

    }
    ComboRotCot *-- RotCot_XYZ : 1 foreach RotCotType
    ComboRotCot o.. RotatedObjectContainer
```

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