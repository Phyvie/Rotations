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

Simplified Class-Diagram: 
```mermaid
classDiagram
    direction LR
    class RotParams_Base{
        +abs Conversion-Functions
        +abs RotateVector()
    }

    RotParams_Base <|.. RotParams_XYZ
    class RotParams_XYZ{
        +parameters
    }

    class RotUI {
        not a C# class 
        but a UI asset 
        thus no abstraction
    }

    class RotVis_GenericBase{
        +abs Get/SetRotParams(ByRef)
        +abs VisUpdate()
    }

    RotVis_GenericBase <|.. RotVis_TemplateBase
    class RotVis_TemplateBase{
        #rotParams->Get/SetRotParams
    }

    RotVis_TemplateBase <|.. RotVis_XYZ
    class RotVis_XYZ{
        #VisObjects->VisUpdate
    }

    class RotCot_GenericBase{
        +oriented-Object - & prefab
        +UI - & asset
        +abs RotParams
        +abs RotVis

        +abs Initialize(Transform parent, VisualElement UIParent, OrientedObject)
    }
    RotCot_GenericBase <|.. RotCot_TemplateBase
    RotCot_GenericBase o..OrientedObject
    
    class RotCot_TemplateBase{
        RotParams - by ref
        rotVis - incl prefab/init
        UI - incl prefab/init
    }
    RotCot_TemplateBase o.. RotParams_Base
    RotCot_TemplateBase o.. RotVis_GenericBase

    RotCot_TemplateBase <|.. RotCot_XYZ : 1 foreach RotType
    class RotCot_XYZ{
        #VariableBindings
    }
    RotCot_XYZ o.. RotParams_XYZ
    RotCot_XYZ o.. RotVis_XYZ
    RotCot_XYZ o.. RotUI

    class OrientedObject{
        get/setAppliedRotation
        get/setRotation
        manage model visibility
    }

    class ComboRotCot {
        List: RotCot_XYZ
        initializeOnStart/OnAwake
        orientedObject
        UI
        Cam
        CoordinateGrid

    }
    ComboRotCot *-- RotCot_XYZ : 1 foreach RotCotType
    ComboRotCot o.. OrientedObject
```

Simplified Color Coded Class Diagram
```mermaid
flowchart TD
    subgraph Legend["Legend"]
        direction LR
        L1["Inheritance (is-a)"] --> L1_1["ClassA <|-- ClassB"]
        L2["Composition (has-a)"] --> L2_1["ClassA o-- ClassB"]
    end

    subgraph Rotation_Parameters["Rotation Parameters - RotParams"]
        RotParams_Base["RotParams_Base<br>«Abstract»<br>+ RotateVector()"]
        RotParams_XYZ["RotParams_XYZ"]
        
        RotParams_Base -.->|implements| RotParams_XYZ
    end

    subgraph Visual_Elements["RotVis"]
        RotVis_GenericBase["RotVis_GenericBase<br>«Abstract»<br>+ Get/SetRotParams()<br>+ VisUpdate()"]
        RotVis_TemplateBase["RotVis_TemplateBase<br># rotParams"]
        RotVis_XYZ["RotVis_XYZ<br># VisObjects"]
        
        RotVis_GenericBase -.->|implements| RotVis_TemplateBase
        RotVis_TemplateBase -.->|implements| RotVis_XYZ
    end
    RotUI["RotUI<br>«UI Asset»"]

    subgraph Controllers["Rotation Containers - RotCot"]
        RotCot_GenericBase["RotCot_GenericBase<br>«Abstract»<br>+ orientedObject<br>+ UI<br>+ RotParams<br>+ RotVis<br>+ Initialize()"]
        RotCot_TemplateBase["RotCot_TemplateBase<br>- RotParams<br>- rotVis<br>- UI"]
        RotCot_XYZ["RotCot_XYZ<br># VariableBindings"]
        
        RotCot_GenericBase -.->|implements| RotCot_TemplateBase
        RotCot_TemplateBase -.->|implements| RotCot_XYZ
    end

    OrientedObject["OrientedObject<br>+ get/setAppliedRotation()<br>+ get/setRotation()<br>+ manage model visibility"]
    ComboRotCot["ComboRotCot<br>+ List: RotCot_XYZ<br>+ initializeOnStart/OnAwake()<br>+ orientedObject<br>+ UI<br>+ Cam<br>+ CoordinateGrid"]

    %% Composition Relationships
    RotCot_TemplateBase o--o RotParams_Base
    RotCot_TemplateBase o--o RotVis_GenericBase
    
    RotCot_XYZ o--o RotParams_XYZ
    RotCot_XYZ o--o RotVis_XYZ
    RotCot_XYZ o--o RotUI
    
    RotCot_GenericBase o--o OrientedObject
    
    ComboRotCot o--o RotCot_XYZ
    ComboRotCot o--o OrientedObject

    %% Styling
    class RotParams_Base,RotVis_GenericBase,RotCot_GenericBase abstract
    classDef abstract fill:#070
    class RotVis_TemplateBase,RotCot_TemplateBase template
    classDef template fill:#660

    class RotParams_XYZ,RotVis_XYZ,RotCot_XYZ cls
    classDef cls fill:#750

    class RotUI,OrientedObject other
    classDef other fill:#505
    
    classDef core fill:#055
    class ComboRotCot core
```

### Full Class-Diagram see Miro: https://miro.com/app/board/uXjVIMvwpDc=/

### Happy-Path: 
- ~~**default** (<= 1day): flowchart & rubber-duck~~
- **complex** (week): separate into tasks

Scene Setup -> see Miro

Initialization -> see Miro

Switch Rotation Type -> see Miro

### Kill Duck
- am I using this solution, only because it ...
- ... is intellectually interesting?
- ... appears cool?
- ... is fun to make?
- ... helps an imaginary future?
-> any yes = kill

### Happy-Path Summary:
The Scene Hierarchy starts with only the ComboRotCot and empty objects with the RotCot_XYZ_Scripts. 
Each RotCot has its own initialisation, which checks the references and spawns unreferenced objects based on its prefabs. 
The ComboRotCot wraps the RotCot-initialisation: First it initialises the UIDocument, Camera and OrientedObject, then it passes these onto the RotCot-initialisations; Lastly it deactivates all RotCots - except the one set as start. 

Switching from one RotCot to another happens via the state-pattern. 



### Edge-Cases: 
- 5 min brainstorm (technical issues, user stupidity, internal curruption) into frequency-impact-time-list: 

| case                         | **frequency** | **impact** | solution-idea     | **solve-time** | solve? |
|------------------------------|---------------|------------|-------------------|----------------|--------|
| missing values (e.g. prefab) | 2             | 3          | null-ref check    | 1              | YES    |
| wrong values                 |               |            |                   |                |        |
|                              |               |            |                   |                |        |
| multiple active RotCots      | 1             | 3          | resetActiveButton | 2              | YES    |
| rotParams_value-problems     |               |            |                   |                |        |
|                              |               |            |                   |                |        |
| Conversion failed            | 2             | 2          | reset to identity | 1              | YES    |
|                              |               |            |                   |                |        |

### implement cases into Solution (from Happy-Path)

### Kill Duck: 
- implementable without further thinking?
- is it "boring"?
  - backwards-compatible?  
=> I need to ensure that during the remodelling all the steps are backward compatible. -> create an order in which I want to implement/refactor the functionality. 

### Solution Summary: 
!!!ZyKa MISSING ORDER OF IMPLEMENTATION/REFACTORING!!!

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