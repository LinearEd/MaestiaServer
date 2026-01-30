# Maestia Game Data - Complete Overview

## Summary
Maestia is an MMORPG with extensive game data extracted from client and server files. The game uses custom binary formats (.mrf, .cbd) and Lua scripting.

## Data Structure

### 1. Unpacked_data (28,137 files)
Main game assets extracted from client packages:
- **cbd/** - 1 large database file (28.9 MB) - likely central game database
- **event/** - 15 event definition files
- **model/** - 16,064 3D model/texture files (character models, items, environment)
- **npc_data/** - 5,526 NPC definition files
- **script/** - 6,070 Lua script files (game logic, quests, behaviors)
- **zone/** - 462 zone/map definition files

### 2. Maestia-client
Client executable and assets

### 3. Maestone-Emulator_original
Server emulator source code (C++/Lua hybrid)
- Character management
- Combat system
- Database integration
- Network protocols

### 4. decode_scripts
Custom Python scripts for unpacking/analyzing game data:
- MRF archive extractor
- CBD database parser
- File structure analyzers

## Key Findings

### Game Architecture
- **Client-Server**: Traditional MMORPG architecture
- **Scripting**: Heavy use of Lua for game logic
- **Database**: CBD format for structured data, likely binary serialized
- **Models**: Custom 3D format (needs further analysis)
- **Zones**: 462 playable areas/maps

### Content Scale
- 16,064 models (characters, items, environment assets)
- 5,526 NPCs 
- 6,070 scripts (quests, AI, events)
- 462 zones/maps
- 15 major events

## Next Steps for Analysis
1. Parse CBD database to extract item/skill/quest data
2. Analyze model format for asset extraction
3. Document NPC and script structures
4. Map zone files to understand world layout
5. Review emulator code for game mechanics
