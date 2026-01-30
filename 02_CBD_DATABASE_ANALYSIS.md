# CBD Database Analysis

## File Information
- **Location**: `Unpacked_data\cbd\cbd.cbd`
- **Size**: 28,915,536 bytes (28.9 MB)
- **Type**: Binary database file

## What is CBD?
CBD appears to be Maestia's Central Binary Database - a packed format containing:
- Item definitions
- Skill/spell data
- Quest information
- NPC templates
- Game configuration
- Localization strings

## Structure (Hypothesis)
Based on file size and game structure:
- Header with table definitions
- Multiple data tables (items, skills, NPCs, etc.)
- Possibly uses offsets/indices for quick lookups
- Likely compressed or packed format

## Extraction Requirements
To fully extract CBD data, need to:
1. Identify file header format
2. Locate table boundaries
3. Determine record structures
4. Handle data types (strings, integers, floats)
5. Extract to readable format (JSON/CSV/SQL)

## Tools Available
- Python scripts in `decode_scripts/` may have CBD parsers
- Need to examine emulator code for CBD loading logic

## Priority
HIGH - This file likely contains all item/skill/quest definitions needed for server emulation

## Status
⚠️ Not yet parsed - needs custom parser development or existing tool identification
