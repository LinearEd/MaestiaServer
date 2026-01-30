# NPC Data Analysis

## Overview
- **Location**: `Unpacked_data\npc_data\`
- **File Count**: 5,526 files
- **Purpose**: NPC instance definitions and placement

## What NPC Data Contains

### NPC Instances
Each file likely defines:
- NPC unique ID
- Reference to model/appearance
- Position and zone placement
- Behavior script reference
- Interaction type (merchant, quest giver, enemy)
- Stats (HP, damage, defense)
- Loot tables
- Faction/alignment

### File Structure
Possible formats:
- XML/JSON text files
- Binary packed format
- Lua data tables
- Custom serialized format

## NPC Types

### Quest NPCs
- Quest givers
- Quest objectives
- Quest turn-ins

### Merchants
- Item vendors
- Equipment sellers
- Special traders

### Enemies/Monsters
- Common enemies
- Elite mobs
- Boss encounters
- Dungeon inhabitants

### Service NPCs
- Trainers
- Bank clerks
- Profession trainers
- Transport NPCs

## Relationship to Other Data
- **Scripts**: Reference NPC_data for behavior
- **CBD**: NPC templates/base stats
- **Zones**: Placement coordinates
- **Models**: Visual representation

## Analysis Priority
HIGH - NPC data essential for:
- World population
- Quest functionality
- Combat encounters
- Economy (merchants)

## Next Steps
1. Sample 10-20 NPC files to determine format
2. Document data structure
3. Create parser for batch extraction
4. Cross-reference with scripts and zones
5. Build NPC database/lookup table

## Status
⚠️ Not yet analyzed - sample examination needed
