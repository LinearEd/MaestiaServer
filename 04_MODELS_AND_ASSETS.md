# 3D Models and Assets Analysis

## Overview
- **Location**: `Unpacked_data\model\`
- **File Count**: 16,064 files
- **File Types**: Unknown format (needs analysis)

## Content Types

### Character Models
Based on file naming patterns (if available):
- Player character models (multiple races/genders)
- NPC models
- Monster models
- Boss models

### Equipment Models
- Weapons
- Armor sets
- Accessories
- Visual effects

### Environment Assets
- Buildings and structures
- Terrain elements
- Props and decorations
- Vegetation

### Special Effects
- Skill visual effects
- Environmental effects
- UI elements
- Particles

## Technical Details

### File Format
⚠️ **Unknown** - Requires format analysis:
- Possible formats: Custom binary, DirectX mesh, proprietary
- May include: Vertex data, textures, animations, materials
- Check decode_scripts for model extractors

### Texture Handling
- Textures may be embedded or separate files
- Possible formats: DDS, TGA, PNG, custom
- May use texture atlases

### Animation Data
- Character animations likely separate or embedded
- Skeletal animation system probable
- Motion data for attacks, emotes, walking

## Extraction Priority
MEDIUM-HIGH - Visual assets needed for:
- Client modification
- Asset documentation
- Private server customization
- Understanding visual design

## Next Steps
1. Identify file format (hex analysis)
2. Find extraction tools in decode_scripts
3. Export sample models for verification
4. Document format specification

## Status
⚠️ Format unknown - requires technical analysis
