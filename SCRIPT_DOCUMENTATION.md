# KOMAL Project 2022-2023 - Script Documentation

## Overview
This Unity project appears to be a dice-based puzzle game called "KOMAL" where players control a die through various levels with different mechanics, charges, and switches. The game features multiple chapters with normal and bonus levels.

## Project Structure

### Core Game Management Scripts

#### `ManageGame.cs`
**Location:** `Assets/Scripts/GameManager/ManageGame.cs`

**Purpose:** Central game manager that handles level state, mechanics coordination, and game flow.

**Key Responsibilities:**
- Manages level data arrays (`levelData`, `floorData`, `mechanicData`)
- Tracks level progression and completion
- Coordinates all game mechanics (charges, switches, blocks)
- Handles tutorial panel display
- Manages frame rate controller settings
- Handles level completion and transitions

**Key Properties:**
- `width`, `length`: Level dimensions
- `levelID`, `chapterID`: Level identification
- `bonus`: Whether level is a bonus level
- `finishedLevels`: Static list of completed levels
- `furthestLevel`, `furthestChapter`: Player progression tracking

**Key Methods:**
- `CheckMechanics()`: Updates all mechanics when die moves
- `LevelComplete()`: Handles level completion sequence
- `NextLevel()`: Transitions to next level or menu

#### `GenerateLevel.cs`
**Location:** `Assets/Scripts/GameManager/GenerateLevel.cs`

**Purpose:** Procedurally generates level geometry and mechanics from image data.

**Key Responsibilities:**
- Reads level layout from texture images
- Instantiates floor tiles, walls, and mechanics
- Sets up color-coded mechanics based on pixel colors
- Configures mechanic properties and connections
- Manages prefab variants for different visual themes

**Key Features:**
- Uses color mapping to determine mechanic types
- Supports alternating floor tile patterns
- Handles pip filters, charge mechanics, and switch systems
- Sets up level boundaries and win conditions

#### `FrameRateController.cs`
**Location:** `Assets/Scripts/GameManager/FrameRateController.cs`

**Purpose:** Optimizes performance by managing frame rate based on game state.

**Key Features:**
- Switches between different frame rate targets
- `lockToFast`: Forces high frame rate for charge mechanics
- `SetFullFPSTime()`: Temporarily increases frame rate during user input

---

### Player Control Scripts

#### `DieController.cs`
**Location:** `Assets/Scripts/Player/DieController.cs`

**Purpose:** Controls the main player character (die) movement, rotation, and state.

**Key Responsibilities:**
- Handles die movement with rolling animation
- Manages die face orientation (`sides` dictionary)
- Processes player input for movement
- Manages charge application and removal
- Handles win condition checking
- Coordinates with camera for relative movement

**Key Properties:**
- `position`: Die position on grid
- `sides`: Dictionary mapping directions to pip values
- `chargeDirection`: Direction of applied charge
- `canControl`: Whether player can move die
- `isMoving`: Whether die is currently rolling

**Key Methods:**
- `MoveForward()`, `MoveBack()`, `MoveLeft()`, `MoveRight()`: Die movement functions
- `HandleMovement()`: Processes input and initiates movement
- `PowerUp()`: Applies charge to die face
- `PowerDown()`: Removes charge from die
- `WinCheck()`: Checks if die reached win position

#### `CameraScript.cs`
**Location:** `Assets/Scripts/Player/CameraScript.cs`

**Purpose:** Manages camera positioning and rotation around the game board.

**Key Features:**
- Multiple camera angles (isometric, overhead)
- Camera rotation around board center
- Smooth transitions between camera positions
- Coordinates with die controller for relative movement

#### `DieOverlayController.cs`
**Location:** `Assets/Scripts/UI/In-Game Menu/DieOverlayController.cs`

**Purpose:** Manages the overlay die that shows current pip values and charges.

**Key Features:**
- Displays die faces that aren't visible from current camera angle
- Updates to match main die rotation and charges
- Provides visual feedback for current die state

---

### Save System Scripts

#### `SaveDataManager.cs`
**Location:** `Assets/Scripts/SaveDataManager.cs`

**Purpose:** Handles persistent data storage for game progress and settings.

**Key Responsibilities:**
- Saves/loads level completion data using JSON
- Manages player preferences using PlayerPrefs
- Handles data reset functionality for development

**Saved Data:**
- **JSON File:** Level completion status
- **PlayerPrefs:** UI settings (transparency, scale, volume, FPS display, controls)

**Key Methods:**
- `readFile()`: Loads save data from JSON
- `writeFile()`: Saves current progress to JSON
- `setPlayerPrefs()`: Saves settings to PlayerPrefs
- `getPlayerPrefs()`: Loads settings from PlayerPrefs

#### `ActionRecorder.cs`
**Location:** `Assets/Scripts/ActionRecorder.cs`

**Purpose:** Implements undo functionality by recording game state changes.

**Key Features:**
- Maintains stack of game states
- Records die position, rotation, and mechanic states
- Enables undo of player moves
- Coordinates state restoration across all game systems

**Key Components:**
- `States` struct: Contains complete game state snapshot
- `stateStack`: Stack of previous game states
- `GetState()`: Captures current complete game state
- `Undo()`: Restores previous game state

---

### Mechanics System Scripts

#### `Mechanic.cs` (Abstract Base Class)
**Location:** `Assets/Scripts/Mechanics/Mechanic.cs`

**Purpose:** Abstract base class for all game mechanics.

**Key Features:**
- Provides common interface for all mechanics
- Handles pip filter checking
- Manages mechanic position and state
- Defines abstract methods all mechanics must implement

**Abstract Methods:**
- `CheckForActivation()`: Called when die moves
- `SetState()`: Used for state restoration during undo

#### Mechanic Implementations:

##### `ChargeController.cs`
**Purpose:** Manages charge-giving tiles that apply special powers to the die.

**States:**
- 0: Activated (charge applied to die)
- 1: Charge available on die
- 2: Not being used

##### `ToggleSwitchController.cs`
**Purpose:** Manages switches that toggle blocks on/off.

**Features:**
- Connected to X and O blocks
- Can be activated by stepping on them with correct pip count
- Changes state of all connected blocks

##### `SingleUseController.cs`
**Purpose:** Manages tiles that can only be stepped on once.

**States:**
- 0: Landed on
- 1: Primed (can be used)
- 20: Nothing (unused state)

##### `LegoSwitchController.cs`
**Purpose:** Manages colored switch system that controls gates.

**Features:**
- Multiple colors/types supported
- Controls corresponding colored gates
- Can require specific pip counts to activate

#### `PipFilterController.cs`
**Purpose:** Manages pip count requirements for mechanics activation.

**Features:**
- Checks if die's bottom face meets pip requirements
- Used by other mechanics to determine activation conditions

---

### UI Scripts

#### Main Menu Scripts

##### `MainMenuScript.cs`
**Location:** `Assets/Scripts/UI/Main Menu/MainMenuScript.cs`

**Purpose:** Controls main menu navigation and animations.

**Key Features:**
- Manages multiple menu screens (start, options, levels, tutorials, about)
- Handles smooth transitions between menus
- Manages tutorial panel pagination
- Controls credit sequence

##### `MainMenuLevelButtons.cs`
**Purpose:** Manages level selection buttons in main menu.

**Features:**
- Generates level selection interface
- Tracks level completion status
- Handles level loading

##### `ButtonScript.cs`
**Purpose:** Handles button interactions and animations in menus.

**Features:**
- Button press animations
- Sound effects integration
- Movement state management

#### In-Game UI Scripts

##### `LevelMenuScript.cs`
**Location:** `Assets/Scripts/UI/In-Game Menu/LevelMenuScript.cs`

**Purpose:** Manages in-game pause menu and level navigation.

##### `SliderOptions.cs`
**Purpose:** Manages slider-based options (transparency, scale).

**Features:**
- Real-time setting updates
- Integration with save system

##### `ButtonOptions.cs`
**Purpose:** Manages toggle button options (FPS display, control side).

#### Control UI Scripts

##### `DirectionalButtonController.cs`
**Location:** `Assets/Scripts/UI/Controls UI/DirectionalButtonController.cs`

**Purpose:** Manages on-screen directional controls and input handling.

**Features:**
- Touch/mouse input detection
- Keyboard input mapping
- Virtual button state management

##### `ToggleButtonUIController.cs`
**Purpose:** Manages toggle button UI elements.

---

### Audio Scripts

#### `AudioSourceManager.cs`
**Location:** `Assets/Scripts/Audio-Music/AudioSourceManager.cs`

**Purpose:** Centralized audio management system.

**Key Features:**
- Singleton pattern for global access
- Sound effect playback
- Volume control integration
- Audio pooling for performance

#### `AudioSliderController.cs`
**Purpose:** Manages audio volume sliders in settings.

**Features:**
- Separate music and SFX volume controls
- Real-time audio adjustment
- Integration with save system

---

### Scriptable Objects

#### `LevelSpecs.cs`
**Location:** `Assets/Scripts/Scriptable Objects/LevelSpecs.cs`

**Purpose:** Defines level-specific configuration data.

**Properties:**
- `camSizeIso`: Camera size for isometric view
- `camSizeOverhead`: Camera size for overhead view
- `levelID`: Level identifier string

#### `ColorPalette.cs`
**Purpose:** Defines color schemes for visual theming.

#### `TutorialPanel.cs`
**Purpose:** Defines tutorial panel content and associated level.

---

### Background Scripts

#### `ShaderManager.cs`
**Location:** `Assets/Scripts/Backgrounds/ShaderManager.cs`

**Purpose:** Manages background visual effects and shaders.

---

### Utility Scripts

#### `FlavorText.cs`
**Location:** `Assets/Scripts/UI/FlavorText.cs`

**Purpose:** Manages dynamic text display for game flavor/narrative.

---

## Game Flow

1. **Game Start:** Main menu loads with saved progress
2. **Level Selection:** Player chooses level, `GenerateLevel` creates the level
3. **Gameplay:** `DieController` handles movement, `ManageGame` coordinates mechanics
4. **State Management:** `ActionRecorder` tracks moves for undo functionality
5. **Level Completion:** `ManageGame` handles win sequence and progression
6. **Save System:** `SaveDataManager` persists progress and settings

## Key Design Patterns

- **Singleton Pattern:** Used for `ManageGame`, `DieController`, `AudioSourceManager`
- **Command Pattern:** Implied in `ActionRecorder` for undo functionality
- **State Pattern:** Used in mechanic controllers for different activation states
- **Observer Pattern:** Mechanics respond to die movement events
- **Factory Pattern:** `GenerateLevel` creates mechanics based on image data

## Dependencies

- **Unity Packages:** Universal Render Pipeline (URP), TextMeshPro
- **Third-party Assets:** LeanTween (animation), Graphy (performance monitoring)
- **Input Systems:** Both old and new Unity input systems supported

## Notes for Developers

- The game uses a grid-based coordinate system
- Color-coding in level images determines mechanic placement
- Frame rate optimization is crucial for smooth die rolling animations
- The undo system requires careful state management across all game systems
- Touch and keyboard controls are both supported with unified input handling
