# Decode Scripts and Tools Analysis

## Overview
- **Location**: `decode_scripts\`
- **Language**: Python
- **Purpose**: Extract and parse Maestia game data

## Available Tools

### MRF Archive Extractor
- Unpacks .mrf archive files
- Used to extract client data files
- Handles compression/encryption

### CBD Parser
- Parses cbd.cbd database file
- Extracts item/skill/quest definitions
- Outputs to readable format

### File Structure Analyzers
- Examines binary file formats
- Identifies patterns and structures
- Assists in reverse engineering

## Tool Status
⚠️ Need to examine actual scripts to determine:
- Which tools are functional
- What data they can extract
- How to use them effectively
- What output formats they produce

## Priority
HIGH - These tools are key to:
- Extracting CBD database contents
- Parsing zone/NPC files
- Understanding file formats
- Automating data extraction

## Next Steps
1. List all Python scripts in decode_scripts/
2. Examine each script's functionality
3. Test tools on sample data
4. Document usage instructions
5. Identify missing tools needed

## Integration
These tools feed into:
- Emulator data loading
- Database population
- Documentation generation
- Content analysis

## Status
⚠️ Requires detailed examination - script inventory needed
