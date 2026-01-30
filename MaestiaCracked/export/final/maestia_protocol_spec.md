# MAESTIA PROTOCOL SPECIFICATION (RECONSTRUCTED)

## Packet Frame
Offset | Size | Name | Description
0x00 | 2 | length | Packet length masked with 0x1FFF
0x02 | 1 | mainId | Main opcode group
0x03 | 1 | subId | Sub opcode
0x04 | n | payload | Data

## XOR Encryption
Rolling XOR using static key array and offset.

---

# Core Packet Map (Truth Map)

## Connection / Handshake
0x0100 HELO_RQ - Client hello/version check
0x0101 HELO_RS - Server hello/version response
0x0102 RSA_PUBLIC_KEY - RSA key exchange

## Login
0x0200 LOGIN_RQ - Login request
0x0201 LOGIN_RS - Login response

## Character System
0x0210 CHARLIST_RQ - Request character list
0x0211 CHARLIST_BEGIN - Begin list
0x0212 CHARLIST_ENTRY - Character entry
0x0213 CHARLIST_END - End list
0x0218 CHAR_SELECT_RQ - Select character
0x0219 CHAR_SELECT_RS - Select response

## Player Info (PCINFO)
0x0220 PCINFO_BEGIN
0x0221 CHAR_BASE_DATA
0x0222 ITEM_LIST_BEGIN
0x0223 ITEM_ENTRY
0x0224 ITEM_LIST_END
0x0225 SKILL_LIST_BEGIN
0x0226 SKILL_ENTRY
0x0227 SKILL_LIST_END
0x0228 PCINFO_END

## World / Entities
0x03E8 NPC_SPAWN
0x03E9 PLAYER_SPAWN
0x03EA ENTITY_DESPAWN

## Movement
0x024C MOVE_CS - Client movement
0x0280 MOVE_SC - Server movement broadcast

## Chat
0x0288 CHAT_CS
0x0290 CHAT_SC

## Inventory
0x0260 ITEM_ENTRY_UPDATE

## Combat / Skills
0x03AD SKILL_CAST
0x0498 DAMAGE_EVENT

---

# Struct Examples

## NPC_SPAWN (0x03E8)
uint32 npcUniqueId
uint32 npcTemplateId
float x
float y
float z
uint16 rotation
uint8 npcType
uint8 flags

## PLAYER_SPAWN (0x03E9)
uint32 characterId
char name[16]
float x,y,z
uint16 rotation
uint32 modelTorso, modelHand, modelLeg, modelShoe
uint8 level
uint8 faction

## MOVE_CS (0x024C)
uint32 characterId
float x,y,z
uint16 rotation

## CHAT_CS (0x0288)
uint8 channel
char message[]

---

# Core Engine Functions (Ghidra Mapping)

FUN_006f3840 = SendNormal
FUN_00a0dd40 = SendToServer
FUN_00a3cfc0 = Global Packet Router
FUN_00a422f0 = Session State Handler
FUN_00a44630 = World State Handler
FUN_00ace070 = Network Send Wrapper
FUN_008f3b70 = World Object Manager
FUN_00829bd0 = NPC Logic Core
FUN_0091ea10 = Combat Core
FUN_009b6690 = Skill System Core

---

# Notes
This document is reconstructed from reverse engineering and packet mapping.
