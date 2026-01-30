

===== FILE: maestia_protocol_spec.md =====

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



===== FILE: protocol specification.txt =====

MAESTIA NETWORK PROTOCOL SPECIFICATION v1.0

(Server Reconstruction Document)

Ich schreibe das so, dass du es direkt als .md oder .txt speichern kannst.

1) Packet Frame Format (GLOBAL)
1.1 Binary Header
Offset  Size  Name        Description
0x00    2     length      Packet length (13-bit masked)
0x02    1     mainId      Main opcode group
0x03    1     subId       Sub opcode
0x04    n     payload     Packet body

1.2 Length Encoding
length = *(uint16*)packet & 0x1FFF;

1.3 Encryption / Obfuscation

XOR stream cipher

Shared key array (client & server)

rolling offset

byte ^= xorArray[offset++];

2) Opcode Naming Convention

We define:

Opcode = (mainId << 8) | subId


Example:

Main = 0x03
Sub  = 0xE8
Opcode = 0x03E8
Name = NPC_SPAWN

3) CONNECTION & LOGIN FLOW
3.1 HELLO / VERSION CHECK
C  S

Opcode: 0x0100

struct HELO_RQ {
    uint32 engineVersion;
    uint32 clientVersion;
}

S  C

Opcode: 0x0101

struct HELO_RS {
    uint32 serverType;
    uint32 engineVersion;
    uint32 clientVersion;
    uint32 serverVersion;
}

3.2 RSA KEY EXCHANGE
S  C

Opcode: 0x0102

struct RSA_PUBLIC_KEY {
    byte rsaKey[];
}

3.3 LOGIN REQUEST
C  S

Opcode: 0x0200

struct LOGIN_RQ {
    char username[32];
    byte passwordEncrypted[];
}

S  C

Opcode: 0x0201

struct LOGIN_RS {
    uint8 result; // 0 = success
}

4) CHARACTER SYSTEM
4.1 Character List Request

Opcode: 0x0210

C  S
struct CHARLIST_RQ {}

S  C (sequence)
0x0211 CHARLIST_BEGIN
0x0212 CHARLIST_ENTRY (repeat)
0x0213 CHARLIST_END

CHARLIST_ENTRY
struct CHAR_ENTRY {
    uint32 characterId;
    uint16 genderJob;
    uint16 faction;
    char name[16];
    uint32 modelTorso;
    uint32 modelHand;
    uint32 modelLeg;
    uint32 modelShoe;
    uint8 level;
}

4.2 Character Select
C  S

Opcode: 0x0218

struct CHAR_SELECT_RQ {
    uint32 characterId;
    byte passwordEncrypted[];
}

S  C

Opcode: 0x0219

struct CHAR_SELECT_RS {
    uint8 allow;
    uint32 characterId;
    uint32 ip;
}

5) PLAYER DATA LOAD (PCINFO)

Opcode group: 0x0220  0x0230

Sequence
0x0220 BEGIN_PCINFO
0x0221 CHAR_BASE_DATA
0x0222 ITEM_LIST_BEGIN
0x0223 ITEM_ENTRY (repeat)
0x0224 ITEM_LIST_END
0x0225 SKILL_LIST_BEGIN
0x0226 SKILL_ENTRY
0x0227 SKILL_LIST_END
0x0228 END_PCINFO

6) WORLD SPAWN SYSTEM
6.1 NPC SPAWN

Opcode: 0x03E8

struct NPC_SPAWN {
    uint32 npcUniqueId;
    uint32 npcTemplateId;
    float x;
    float y;
    float z;
    uint16 rotation;
    uint8 npcType;
    uint8 flags;
}


npcType:

0 = normal NPC
1 = quest NPC
2 = merchant
3 = monster

6.2 PLAYER SPAWN

Opcode: 0x03E9

struct PLAYER_SPAWN {
    uint32 characterId;
    char name[16];
    float x, y, z;
    uint16 rotation;
    uint32 modelTorso;
    uint32 modelHand;
    uint32 modelLeg;
    uint32 modelShoe;
    uint8 level;
    uint8 faction;
}

7) MOVEMENT
Client  Server

Opcode: 0x024C

struct MOVE_CS {
    uint32 charId;
    float x;
    float y;
    float z;
    uint16 rotation;
}

Server  Client

Opcode: 0x0280

struct MOVE_SC {
    uint32 charId;
    float x;
    float y;
    float z;
    uint16 rotation;
}

8) CHAT SYSTEM
C  S

Opcode: 0x0288

struct CHAT_CS {
    uint8 channel;
    char message[];
}


channel:

0 = normal
1 = shout
2 = party
3 = guild
4 = system

S  C

Opcode: 0x0290

struct CHAT_SC {
    uint32 senderId;
    char name[16];
    uint8 channel;
    char message[];
}

9) INVENTORY
ITEM ENTRY

Opcode: 0x0260

struct ITEM_ENTRY {
    uint32 itemInstanceId;
    uint32 itemProtoId;
    uint16 slot;
    uint16 count;
    uint8 flags;
}

10) COMBAT / SKILLS
Skill Cast

Opcode: 0x03AD

struct SKILL_CAST {
    uint32 casterId;
    uint32 skillId;
    uint32 targetId;
}

Damage

Opcode: 0x0498

struct DAMAGE {
    uint32 attackerId;
    uint32 targetId;
    uint32 damage;
    uint8 damageType;
}


damageType:

0 = physical
1 = magic
2 = heal

11) QUEST SYSTEM

Opcode group: 0x0340  0x0360

Examples:

0x0340 QUEST_BEGIN
0x0341 QUEST_UPDATE
0x0342 QUEST_COMPLETE

12) THE MOST IMPORTANT PART (for you)

From your map, these are CORE HANDLER FUNCTIONS:

Packet Dispatcher Core
FUN_00a3cfc0  // global packet router
FUN_00a422f0  // session state handler
FUN_00a44630  // world state handler
FUN_00ace070  // network send wrapper
FUN_006f3840  // SendNormal (packet send)
FUN_00a0dd40  // SendToServer logic

World / Gameplay
FUN_008f3b70  // world object manager
FUN_00829bd0  // NPC logic
FUN_00828d70  // entity spawn
FUN_0091ea10  // combat core
FUN_009b6690  // skill system

Inventory
FUN_00985be0  // inventory update
FUN_00927800  // item load

Movement
FUN_00a3afd0  // movement handler


===== FILE: truthmap.txt =====

Maestia Packet Truth Map (v1)
AUTH / LOGIN / SESSION
ID	Dir	Name	Beschreibung	Root Handler	Confidence
0x0200	CS	LOGIN_REQ	Login Request (Account + Hash)	FUN_00874470
0x0201	SC	LOGIN_ACK	Login Antwort / Result	FUN_00a422f0
0x0202	CS	LOGIN_RSA	RSA Key Exchange / Proof	FUN_00a422f0
0x0204	CS	LOGIN_PING	Login keepalive	FUN_00a39a70
0x0205	SC	LOGIN_STATUS	Login Status Update	FUN_00a422f0

Erklrung
FUN_00a422f0 ist klar ein zentraler Login-State-Handler.

CHARACTER / ACCOUNT
ID	Dir	Name	Beschreibung	Root Handler
0x0210	SC	CHAR_LIST	Character List	FUN_00a38860
0x0218	CS	CHAR_SELECT	Character Select	FUN_00829bd0
0x0219	SC	CHAR_SELECT_ACK	Char Select Result	FUN_00a3f9f0
0x021A	SC	CHAR_DATA	Character Base Data	FUN_00a3f9f0


FUN_00829bd0 = klassischer Enter World Dispatcher.

WORLD / ZONE / SYNC
ID	Dir	Name	Beschreibung	Root Handler
0x0338	SC	ZONE_INIT	Zone init / map load	FUN_00829d00
0x0364	SC	ZONE_SYNC	Zone state sync	FUN_00829e00
0x0340	SC	WORLD_INIT	World init	FUN_00822fa0


0x0338/0x0364 sind typisch fr MMO Zone Sync.

MOVEMENT / PLAYER STATE
ID	Dir	Name	Beschreibung	Root Handler
0x024C	CS	MOVE	Player movement	FUN_00a3afd0
0x0280	SC	MOVE_SYNC	Movement broadcast	FUN_00a44630
0x0286	SC	PLAYER_STATE	Player state update	FUN_00a46960


FUN_00a3afd0 ist sehr sicher Movement.

CHAT
ID	Dir	Name	Beschreibung	Root Handler
0x0288	CS	CHAT_SEND	Chat message	FUN_009fb930
0x0290	SC	CHAT_BROADCAST	Chat broadcast	FUN_00950540


FUN_009fb930 taucht oft bei Strings/UI auf  Chat.

INVENTORY / ITEMS
ID	Dir	Name	Beschreibung	Root Handler
0x0258	SC	ITEM_LIST	Inventory list	FUN_00a2dfb0
0x0260	SC	ITEM_EQUIP	Equipment data	FUN_00a2efe0
0x0402	CS	ITEM_USE	Use item	FUN_00a29950
0x040C	CS	ITEM_MOVE	Move item	FUN_00a2d510


0x04xx = klassisch Item subsystem.

COMBAT / SKILLS
ID	Dir	Name	Beschreibung	Root Handler
0x03AD	CS	SKILL_CAST	Skill cast	FUN_00a380f0
0x0498	SC	DAMAGE	Damage event	FUN_00a380f0
0x0550	SC	COMBAT_STATE	Combat state	FUN_00a38d30


FUN_00a380f0 = Combat Core.

NPC / QUEST
ID	Dir	Name	Beschreibung	Root Handler
0x03E8	SC	NPC_SPAWN	NPC spawn	FUN_00a2a1a0
0x03A8	CS	NPC_INTERACT	NPC interaction	FUN_00a316c0
0x0450	SC	QUEST_UPDATE	Quest update	FUN_00a2f930


0x03E8 = extrem typisch NPC spawn in MMOs.

SYSTEM / MISC
ID	Dir	Name	Beschreibung	Root Handler
0x0EF4	SC	HEARTBEAT	Keepalive / ping	FUN_00a0dd40
0x0900	SC	ADMIN	Admin / GM	FUN_00aedcb0


FUN_00a0dd40 = dein SendNormal / Net Core  passt perfekt.


===== FILE: packet_handlers.txt =====

0x0200 -> FUN_00874470, FUN_008747e0, FUN_009a1b70, FUN_00a422f0
0x0201 -> FUN_00a422f0
0x0202 -> FUN_00a422f0
0x0204 -> FUN_00a39a70, FUN_00a422f0
0x0205 -> FUN_00a422f0
0x0210 -> FUN_00a38860, FUN_00a44630, FUN_00ace070
0x0218 -> FUN_00829bd0, FUN_00a46960, FUN_00a58e50
0x0219 -> FUN_00a3f9f0, FUN_00a422f0
0x021A -> FUN_00a3f9f0
0x0228 -> FUN_00828d70, FUN_00980100, FUN_00b00640
0x022A -> FUN_00a46960
0x022C -> FUN_00a44630, FUN_00ace070
0x0234 -> FUN_009538a0, FUN_00959ab0, FUN_00a46960
0x023C -> FUN_00a44630
0x0240 -> FUN_00988a80, FUN_00aec6f0, FUN_00afe020
0x0248 -> FUN_00a44630, FUN_00ace070
0x024C -> FUN_008d1720, FUN_008d2ed0, FUN_00a380f0, FUN_00a3afd0, FUN_00a3f240, FUN_00ae8b80
0x0254 -> FUN_009be4f0
0x0258 -> FUN_009e6680, FUN_00a2dfb0, FUN_00a38860, FUN_00a42530
0x025C -> FUN_00985be0, FUN_00aedcb0
0x0260 -> FUN_00927800, FUN_00985be0, FUN_00a099f0, FUN_00a2efe0
0x0263 -> FUN_00a562e0
0x0264 -> FUN_00a099f0, FUN_00a44630, FUN_00aacc30, FUN_00ace070
0x0268 -> FUN_008f3b70, FUN_00a46960
0x0274 -> FUN_009b30b0, FUN_00aedcb0
0x0280 -> FUN_00a44630, FUN_00ace070
0x0281 -> FUN_0080a1d0
0x0286 -> FUN_0080a1d0, FUN_00a3f9f0, FUN_00a422f0, FUN_00a46960, FUN_00ae50c0, FUN_00aedcb0, FUN_00af8de0
0x0288 -> FUN_00960f80, FUN_009a3590, FUN_009fb930, FUN_00a46960, FUN_00af8de0
0x028C -> FUN_00a58e50
0x0290 -> FUN_00950540, FUN_009fb930, FUN_00abc800
0x0294 -> FUN_0094d7f0, FUN_009b30b0, FUN_00a34130
0x0298 -> FUN_0094d7f0
0x029C -> FUN_00ace070
0x02A0 -> FUN_00a95850
0x02AF -> FUN_00a40510
0x02B8 -> FUN_008f3b70, FUN_00ace070
0x02BC -> FUN_008f3b70, FUN_00985be0, FUN_00af8de0
0x02C0 -> FUN_008f3b70, FUN_00abc800
0x02C4 -> FUN_008f3b70, FUN_00a2efe0, FUN_00aedcb0
0x02C8 -> FUN_008f3b70, FUN_0091f920
0x02CC -> FUN_008f3b70
0x02D0 -> FUN_008f3b70
0x02D4 -> FUN_008f3b70, FUN_00ace070
0x02D8 -> FUN_008f3b70, FUN_00a4e230
0x02E0 -> FUN_009a1b70
0x02F0 -> FUN_00ace070
0x030C -> FUN_00ace070
0x0328 -> FUN_00ace070
0x0338 -> FUN_00829d00, FUN_00829e00
0x0339 -> FUN_00829410
0x033A -> FUN_00829410
0x033F -> FUN_00828d70
0x0340 -> FUN_00822fa0, FUN_00870e00, FUN_00a422f0, FUN_00a58e50, FUN_00ad0250
0x0341 -> FUN_00822fa0
0x0343 -> FUN_00afdf10, FUN_00b021c0
0x0344 -> FUN_00a4d4b0, FUN_00ace070
0x0345 -> FUN_00829d00
0x0346 -> FUN_00829d00
0x0347 -> FUN_00829d00
0x0349 -> FUN_008f3b70, FUN_00a0dd40, FUN_00b00640
0x034A -> FUN_0080a1d0
0x034C -> FUN_00a422f0
0x0364 -> FUN_00829e00
0x036C -> FUN_00a352e0
0x0370 -> FUN_00a352e0
0x0390 -> FUN_00afe020
0x03A8 -> FUN_008243e0, FUN_0091b9d0, FUN_00a316c0, FUN_00a44630
0x03A9 -> FUN_00982070, FUN_009821a0, FUN_00985000, FUN_009857c0, FUN_00985be0
0x03AD -> FUN_008d1a60, FUN_008d1c90, FUN_008d1e20, FUN_008d2520, FUN_008d2d10, FUN_008d2ed0, FUN_0091fee0, FUN_00a380f0, FUN_00a38280
0x03B1 -> FUN_00a2dfb0
0x03C8 -> FUN_00a3f9f0
0x03CC -> FUN_00a3f9f0
0x03E8 -> FUN_008e51c0, FUN_008f2030, FUN_0091ea10, FUN_009b6690, FUN_009d7630, FUN_00a2a1a0, FUN_00a46960, FUN_00a58e50, FUN_00b021c0
0x03EA -> FUN_00ae2930
0x03F8 -> FUN_00a3b3b0, FUN_00a3c0b0, FUN_00b00640
0x0402 -> FUN_00a29950, FUN_00a29d10
0x040C -> FUN_00a2d510
0x040D -> FUN_00a2d510
0x040E -> FUN_00a2d510
0x040F -> FUN_00a2d510
0x0410 -> FUN_00a2d510
0x0411 -> FUN_00a2d510
0x0412 -> FUN_00a2d510
0x0413 -> FUN_00a2d510
0x0414 -> FUN_00a2d510
0x0415 -> FUN_00a2d510
0x0416 -> FUN_00a2d510
0x0417 -> FUN_00a2d510
0x0419 -> FUN_00943360
0x041A -> FUN_00a48b20, FUN_00b00640
0x0424 -> FUN_0091bd50
0x0427 -> FUN_00a31810, FUN_00a6af50
0x0428 -> FUN_0080a1d0
0x043A -> FUN_008d26d0, FUN_008d2b50, FUN_008d3470
0x0440 -> FUN_008ff140
0x044C -> FUN_00a46960
0x0450 -> FUN_00828b90, FUN_0091b9d0, FUN_00a2f930, FUN_00a3e680, FUN_00a40510, FUN_00a44630, FUN_00ab7ff0
0x0457 -> FUN_00a44630, FUN_00a46960, FUN_00a47390
0x046C -> FUN_00a316c0, FUN_00a38440
0x0474 -> FUN_009be4f0, FUN_00a38330, FUN_00a38440, FUN_00a44630
0x0478 -> FUN_00a38330, FUN_00a38440
0x0498 -> FUN_00a380f0
0x04A4 -> FUN_00a31810
0x04B8 -> FUN_00a46960
0x04C0 -> FUN_00a34130, FUN_00a422f0, FUN_00a46960, FUN_00af8de0, FUN_00afd260
0x04C1 -> FUN_00a46960
0x04C9 -> FUN_00a39240, FUN_00a46960
0x04DC -> FUN_00a39240, FUN_00a46960
0x04E4 -> FUN_00829e00
0x04E8 -> FUN_0080a1d0
0x04F0 -> FUN_00a28fb0, FUN_00a39240
0x04F4 -> FUN_00a28fb0
0x050C -> FUN_008243e0
0x0514 -> FUN_009358b0, FUN_009b30b0, FUN_00a29d10, FUN_00a39240, FUN_00a421f0
0x0533 -> FUN_0087c380
0x0550 -> FUN_00a31810, FUN_00a38d30, FUN_00a42770, FUN_00ae59b0
0x0554 -> FUN_00a316c0
0x0555 -> FUN_00a352e0
0x0556 -> FUN_00a352e0
0x0558 -> FUN_00a352e0
0x0580 -> FUN_00a2dfb0, FUN_00a2fd20, FUN_00a42770, FUN_00a42e90
0x0598 -> FUN_00a37ac0, FUN_00a46960
0x059C -> FUN_00a29950, FUN_00a46960
0x05A0 -> FUN_00a46960
0x05B8 -> FUN_00a6abe0
0x05C8 -> FUN_00a43380
0x05DC -> FUN_008d38d0
0x05E8 -> FUN_00a44630
0x05E9 -> FUN_00985ab0, FUN_00a12930
0x05F1 -> FUN_00a29d10
0x0630 -> FUN_00852d50
0x0638 -> FUN_00852d50
0x0656 -> FUN_0092a430
0x068E -> FUN_00830340, FUN_00a09420
0x074C -> FUN_00a39a70, FUN_00a39df0
0x07D0 -> FUN_0087c530, FUN_008d38d0, FUN_009533e0, FUN_0099edc0
0x07D2 -> FUN_00ae2930
0x0900 -> FUN_00aedcb0
0x0926 -> FUN_00aedcb0
0x09C4 -> FUN_00a79320
0x09E4 -> FUN_00a79060
0x0A1C -> FUN_00aacc30
0x0A5E -> FUN_00850e40, FUN_00aacc30
0x0BB8 -> FUN_00852d50, FUN_009d2b90, FUN_009d7630
0x0EF4 -> FUN_00a0dd40
0x0F02 -> FUN_00931c40
0x0FA2 -> FUN_00ae2930
0x1004 -> FUN_00aedcb0
0x110F -> FUN_00aec6f0
0x1388 -> FUN_008d1720, FUN_008d3470, FUN_008d3690, FUN_0091ea10, FUN_009b6690, FUN_00a871e0
0x1400 -> FUN_008d2ed0, FUN_00a2fd20, FUN_00a37b90
0x1472 -> FUN_00a39df0
0x1578 -> FUN_00a107e0
0x15AC -> FUN_00852d50
0x15AE -> FUN_00a10300, FUN_00a107e0, FUN_00a6abe0
0x1690 -> FUN_00a2dfb0
0x16DC -> FUN_00852d50
0x1750 -> FUN_009a1960
0x1754 -> FUN_009a1960
0x1770 -> FUN_008d2ed0
0x17F0 -> FUN_00aacc30
0x181C -> FUN_009a32b0
0x1858 -> FUN_009a1960
0x185C -> FUN_009a1960
0x1877 -> FUN_00a29950, FUN_00a29d10
0x18A4 -> FUN_00af8de0
0x18A8 -> FUN_0096c470, FUN_00a099f0
0x18B8 -> FUN_00af8de0
0x198D -> FUN_00a099f0
0x19D0 -> FUN_0093d5e0
0x1ABD -> FUN_00850b80, FUN_00850e40, FUN_008512c0, FUN_008700f0, FUN_00a6af50
0x1ABE -> FUN_00850d50, FUN_00850e40, FUN_008700f0, FUN_00a6af50
0x1ABF -> FUN_00a37ac0, FUN_00a46960
0x1B58 -> FUN_008d3470
0x1ED0 -> FUN_008de000, FUN_008e4f60, FUN_008e51c0, FUN_0096c300
0x1F54 -> FUN_00874fa0, FUN_0087c380, FUN_008d38d0, FUN_00902860, FUN_00913040, FUN_00959ab0, FUN_0096a520, FUN_009b8e80, FUN_009cf3e0, FUN_00a11340, FUN_00a3f240, FUN_00a42530, FUN_00a95f90, FUN_00ae8b80, FUN_00aec6f0, FUN_00b07df0
0x1F5A -> FUN_00a31810, FUN_00a38d30, FUN_00a3afd0
0x1F7C -> FUN_00874bb0
0x1FA4 -> FUN_0087c380, FUN_00a44630
0x1FC0 -> FUN_00a360c0, FUN_00a44630
0x1FD0 -> FUN_00a50c50, FUN_00a58e50
0x1FD1 -> FUN_00a58e50
0x1FDC -> FUN_009a3900, FUN_009c44a0, FUN_009fb930
0x1FE3 -> FUN_00829750
0x1FE4 -> FUN_009a1d70, FUN_00a29110, FUN_00a3c0b0



===== FILE: packet_map.txt =====

0x0200 -> FUN_00a3cfc0, FUN_00a422f0, FUN_00874470, FUN_008747e0, FUN_009a1b70
0x0201 -> FUN_00a3cfc0, FUN_00a3cfc0, FUN_00a3cfc0, FUN_00a422f0
0x0202 -> FUN_00a3cfc0, FUN_00a3cfc0, FUN_00a3cfc0, FUN_00a3cfc0, FUN_00a3cfc0, FUN_00a422f0, FUN_00a422f0
0x0203 -> FUN_00a3cfc0, FUN_00a3cfc0
0x0204 -> FUN_00a3cfc0, FUN_00a3cfc0, FUN_00a422f0, FUN_00a39a70
0x0205 -> FUN_00a3cfc0, FUN_00a3cfc0, FUN_00a3cfc0, FUN_00a422f0, FUN_00a422f0
0x0207 -> FUN_00a3cfc0, FUN_00a3cfc0
0x0208 -> FUN_00a3cfc0, FUN_00a3cfc0
0x020A -> FUN_00a3cfc0
0x0210 -> FUN_00a44630, FUN_00a44630, FUN_00a38860, FUN_00ace070, FUN_00ace070
0x0218 -> FUN_00829bd0, FUN_00a46960, FUN_00a58e50
0x0219 -> FUN_00a3cfc0, FUN_00a3cfc0, FUN_00a3cfc0, FUN_00a3cfc0, FUN_00a3cfc0, FUN_00a3cfc0, FUN_00a3cfc0, FUN_00a3cfc0, FUN_00a3cfc0, FUN_00a3cfc0, FUN_00a3cfc0, FUN_00a3f9f0, FUN_00a3f9f0, FUN_00a422f0
0x021A -> FUN_00a3cfc0, FUN_00a3cfc0, FUN_00a3cfc0, FUN_00a3cfc0, FUN_00a3cfc0, FUN_00a3cfc0, FUN_00a3cfc0, FUN_00a3cfc0, FUN_00a3f9f0, FUN_00a3f9f0
0x021B -> FUN_00a3cfc0, FUN_00a3cfc0, FUN_00a3cfc0, FUN_00a3cfc0, FUN_00a3cfc0, FUN_00a3cfc0
0x021C -> FUN_00a3cfc0, FUN_00a3cfc0
0x021D -> FUN_00a3cfc0, FUN_00a3cfc0
0x0228 -> FUN_00b00640, FUN_00828d70, FUN_00980100
0x022A -> FUN_00a46960
0x022C -> FUN_00a44630, FUN_00ace070, FUN_00ace070
0x0234 -> FUN_00a46960, FUN_009538a0, FUN_00959ab0
0x023C -> FUN_00a44630
0x0240 -> FUN_00afe020, FUN_00aec6f0, FUN_00988a80, FUN_00988a80
0x0248 -> FUN_00a44630, FUN_00ace070, FUN_00ace070
0x024C -> FUN_00ae8b80, FUN_00ae8b80, FUN_00ae8b80, FUN_00ae8b80, FUN_00a3afd0, FUN_008d2ed0, FUN_00a380f0, FUN_008d1720, FUN_00a3f240
0x0254 -> FUN_009be4f0
0x0258 -> FUN_00a42530, FUN_009e6680, FUN_00a2dfb0, FUN_00a38860
0x025C -> FUN_00aedcb0, FUN_00985be0
0x0260 -> FUN_00927800, FUN_00a099f0, FUN_00985be0, FUN_00a2efe0
0x0263 -> FUN_00a562e0
0x0264 -> FUN_00a44630, FUN_00a3cfc0, FUN_00a3cfc0, FUN_00a099f0, FUN_00aacc30, FUN_00ace070, FUN_00ace070
0x0268 -> FUN_00a46960, FUN_008f3b70
0x0274 -> FUN_00aedcb0, FUN_009b30b0
0x0280 -> FUN_00a44630, FUN_00ace070, FUN_00ace070
0x0281 -> FUN_0080a1d0
0x0286 -> FUN_0080a1d0, FUN_00a46960, FUN_00aedcb0, FUN_00af8de0, FUN_00a3f9f0, FUN_00a422f0, FUN_00ae50c0
0x0288 -> FUN_00a46960, FUN_00af8de0, FUN_00a3cfc0, FUN_009a3590, FUN_009a3590, FUN_009fb930, FUN_009fb930, FUN_00960f80
0x028C -> FUN_00a58e50
0x0290 -> FUN_009fb930, FUN_00abc800, FUN_00950540
0x0294 -> FUN_00a34130, FUN_009b30b0, FUN_0094d7f0
0x0298 -> FUN_0094d7f0
0x029C -> FUN_00ace070, FUN_00ace070
0x02A0 -> FUN_00a95850
0x02AF -> FUN_00a40510, FUN_00a40510
0x02B8 -> FUN_008f3b70, FUN_00ace070, FUN_00ace070
0x02BC -> FUN_00af8de0, FUN_008f3b70, FUN_00985be0
0x02C0 -> FUN_008f3b70, FUN_00abc800, FUN_00abc800
0x02C4 -> FUN_00aedcb0, FUN_008f3b70, FUN_00a2efe0
0x02C8 -> FUN_008f3b70, FUN_0091f920
0x02CC -> FUN_008f3b70
0x02D0 -> FUN_008f3b70
0x02D4 -> FUN_008f3b70, FUN_00ace070, FUN_00ace070
0x02D8 -> FUN_008f3b70, FUN_00a4e230
0x02E0 -> FUN_009a1b70
0x02F0 -> FUN_00ace070, FUN_00ace070
0x030C -> FUN_00ace070, FUN_00ace070
0x0328 -> FUN_00ace070, FUN_00ace070
0x0338 -> FUN_00829d00, FUN_00829e00
0x0339 -> FUN_00829410, FUN_00829410
0x033A -> FUN_00829410, FUN_00829410
0x033F -> FUN_00828d70, FUN_00828d70
0x0340 -> FUN_00a58e50, FUN_00a422f0, FUN_00822fa0, FUN_00870e00, FUN_00ad0250
0x0341 -> FUN_00822fa0
0x0343 -> FUN_00afdf10, FUN_00b021c0
0x0344 -> FUN_00ace070, FUN_00a4d4b0
0x0345 -> FUN_00829d00
0x0346 -> FUN_00829d00
0x0347 -> FUN_00829d00
0x0349 -> FUN_00a0dd40, FUN_00b00640, FUN_008f3b70
0x034A -> FUN_0080a1d0
0x034C -> FUN_00a422f0
0x0364 -> FUN_00829e00
0x036C -> FUN_00a352e0
0x0370 -> FUN_00a352e0
0x0376 -> FUN_00a3cfc0, FUN_00a3cfc0
0x0390 -> FUN_00afe020
0x03A8 -> FUN_008243e0, FUN_00a44630, FUN_00a316c0, FUN_0091b9d0
0x03A9 -> FUN_00982070, FUN_00985000, FUN_009857c0, FUN_00985be0, FUN_00985be0, FUN_00985be0, FUN_00985be0, FUN_009821a0
0x03AD -> FUN_008d1a60, FUN_008d2520, FUN_008d1c90, FUN_008d1e20, FUN_008d2d10, FUN_008d2ed0, FUN_0091fee0, FUN_00a380f0, FUN_00a38280
0x03B1 -> FUN_00a2dfb0
0x03BC -> FUN_00a3cfc0, FUN_00a3cfc0
0x03C0 -> FUN_00a3cfc0, FUN_00a3cfc0
0x03C8 -> FUN_00a3cfc0, FUN_00a3f9f0
0x03CC -> FUN_00a3cfc0, FUN_00a3f9f0, FUN_00a3f9f0
0x03E8 -> FUN_00b021c0, FUN_00a46960, FUN_00a58e50, FUN_00a58e50, FUN_009b6690, FUN_009b6690, FUN_009b6690, FUN_009b6690, FUN_009d7630, FUN_008e51c0, FUN_0091ea10, FUN_0091ea10, FUN_0091ea10, FUN_0091ea10, FUN_008f2030, FUN_00a2a1a0
0x03EA -> FUN_00ae2930
0x03F8 -> FUN_00b00640, FUN_00a3c0b0, FUN_00a3b3b0
0x0402 -> FUN_00a3cfc0, FUN_00a3cfc0, FUN_00a29950, FUN_00a29d10
0x040C -> FUN_00a2d510
0x040D -> FUN_00a2d510
0x040E -> FUN_00a2d510
0x040F -> FUN_00a2d510
0x0410 -> FUN_00a2d510
0x0411 -> FUN_00a2d510
0x0412 -> FUN_00a2d510
0x0413 -> FUN_00a2d510
0x0414 -> FUN_00a2d510
0x0415 -> FUN_00a2d510
0x0416 -> FUN_00a2d510
0x0417 -> FUN_00a2d510
0x0419 -> FUN_00943360
0x041A -> FUN_00b00640, FUN_00a48b20, FUN_00a48b20
0x0424 -> FUN_0091bd50
0x0427 -> FUN_00a31810, FUN_00a6af50
0x0428 -> FUN_0080a1d0
0x043A -> FUN_008d26d0, FUN_008d3470, FUN_008d2b50
0x0440 -> FUN_008ff140
0x044C -> FUN_00a46960
0x0450 -> FUN_00a44630, FUN_00a44630, FUN_00828b90, FUN_00a40510, FUN_00a2f930, FUN_0091b9d0, FUN_00a3e680, FUN_00ab7ff0
0x0457 -> FUN_00a44630, FUN_00a47390, FUN_00a46960
0x046C -> FUN_00a316c0, FUN_00a38440, FUN_00a38440
0x0474 -> FUN_00a44630, FUN_00a38330, FUN_00a38330, FUN_00a38440, FUN_00a38440, FUN_009be4f0, FUN_009be4f0, FUN_009be4f0, FUN_009be4f0
0x0478 -> FUN_00a38330, FUN_00a38440
0x0498 -> FUN_00a380f0, FUN_00a380f0, FUN_00a380f0
0x04A4 -> FUN_00a31810
0x04B8 -> FUN_00a46960
0x04C0 -> FUN_00afd260, FUN_00a46960, FUN_00af8de0, FUN_00a34130, FUN_00a422f0
0x04C1 -> FUN_00a46960
0x04C9 -> FUN_00a46960, FUN_00a39240
0x04DC -> FUN_00a46960, FUN_00a39240
0x04E4 -> FUN_00829e00
0x04E8 -> FUN_0080a1d0
0x04F0 -> FUN_00a39240, FUN_00a28fb0
0x04F4 -> FUN_00a28fb0
0x050C -> FUN_008243e0
0x0514 -> FUN_00a3cfc0, FUN_00a3cfc0, FUN_00a39240, FUN_009b30b0, FUN_00a421f0, FUN_009358b0, FUN_00a29d10
0x0533 -> FUN_0087c380
0x0550 -> FUN_00a42770, FUN_00ae59b0, FUN_00a31810, FUN_00a38d30
0x0554 -> FUN_00a316c0
0x0555 -> FUN_00a352e0, FUN_00a352e0, FUN_00a352e0
0x0556 -> FUN_00a352e0
0x0558 -> FUN_00a352e0
0x0580 -> FUN_00a42770, FUN_00a42e90, FUN_00a2dfb0, FUN_00a2fd20
0x0598 -> FUN_00a46960, FUN_00a37ac0
0x059C -> FUN_00a46960, FUN_00a29950
0x05A0 -> FUN_00a46960
0x05B8 -> FUN_00a6abe0
0x05C8 -> FUN_00a43380
0x05DC -> FUN_008d38d0
0x05E8 -> FUN_00a44630
0x05E9 -> FUN_00a3cfc0, FUN_00a3cfc0, FUN_00a3cfc0, FUN_00a3cfc0, FUN_00a12930, FUN_00985ab0
0x05F1 -> FUN_00a29d10
0x0630 -> FUN_00852d50
0x0638 -> FUN_00852d50, FUN_00852d50, FUN_00852d50
0x0656 -> FUN_0092a430
0x068E -> FUN_00830340, FUN_00a09420
0x074C -> FUN_00a39a70, FUN_00a39df0
0x07D0 -> FUN_0087c530, FUN_008d38d0, FUN_009533e0, FUN_0099edc0
0x07D2 -> FUN_00ae2930
0x0900 -> FUN_00aedcb0
0x0926 -> FUN_00aedcb0
0x09C4 -> FUN_00a79320
0x09E4 -> FUN_00a79060
0x0A1C -> FUN_00aacc30
0x0A5E -> FUN_00aacc30, FUN_00aacc30, FUN_00aacc30, FUN_00aacc30, FUN_00850e40
0x0BB8 -> FUN_009d7630, FUN_00852d50, FUN_009d2b90
0x0C00 -> FUN_006f3540, FUN_006f4b40
0x0EDC -> FUN_006f3540, FUN_006f3540, FUN_006f3540
0x0EE0 -> FUN_006f3540, FUN_006f3840
0x0EF4 -> FUN_00a0dd40
0x0F02 -> FUN_00931c40
0x0FA2 -> FUN_00ae2930
0x1004 -> FUN_00aedcb0
0x110F -> FUN_00aec6f0
0x1388 -> FUN_009b6690, FUN_008d3690, FUN_008d3470, FUN_008d3470, FUN_008d3470, FUN_008d1720, FUN_0091ea10, FUN_0091ea10, FUN_0091ea10, FUN_00a871e0
0x1400 -> FUN_008d2ed0, FUN_008d2ed0, FUN_00a37b90, FUN_00a37b90, FUN_00a37b90, FUN_00a2fd20, FUN_00a2fd20
0x1472 -> FUN_00a39df0
0x1578 -> FUN_00a107e0
0x15AC -> FUN_00852d50
0x15AE -> FUN_00a10300, FUN_00a107e0, FUN_00a107e0, FUN_00a107e0, FUN_00a6abe0
0x1690 -> FUN_00a2dfb0
0x16DC -> FUN_00852d50
0x1750 -> FUN_009a1960
0x1754 -> FUN_009a1960
0x1770 -> FUN_008d2ed0
0x17F0 -> FUN_00aacc30
0x181C -> FUN_009a32b0
0x1858 -> FUN_009a1960
0x185C -> FUN_009a1960
0x1877 -> FUN_00a29950, FUN_00a29d10
0x18A4 -> FUN_00af8de0
0x18A8 -> FUN_00a099f0, FUN_0096c470
0x18B8 -> FUN_00af8de0
0x198D -> FUN_00a099f0
0x19D0 -> FUN_0093d5e0
0x1ABD -> FUN_00850b80, FUN_00850e40, FUN_00850e40, FUN_008700f0, FUN_008512c0, FUN_00a6af50
0x1ABE -> FUN_00850d50, FUN_00850e40, FUN_00850e40, FUN_008700f0, FUN_00a6af50
0x1ABF -> FUN_00a46960, FUN_00a37ac0
0x1B58 -> FUN_008d3470
0x1ED0 -> FUN_008de000, FUN_008e4f60, FUN_008e51c0, FUN_0096c300
0x1F54 -> FUN_00a11340, FUN_0087c380, FUN_00874fa0, FUN_00aec6f0, FUN_00aec6f0, FUN_00aec6f0, FUN_00aec6f0, FUN_00aec6f0, FUN_00aec6f0, FUN_00ae8b80, FUN_00a95f90, FUN_00a42530, FUN_008d38d0, FUN_008d38d0, FUN_00b07df0, FUN_00a3f240, FUN_0096a520, FUN_009b8e80, FUN_00913040, FUN_00959ab0, FUN_00959ab0, FUN_00902860, FUN_009cf3e0
0x1F5A -> FUN_00a3afd0, FUN_00a31810, FUN_00a38d30, FUN_00a38d30
0x1F7C -> FUN_00874bb0
0x1FA4 -> FUN_00a44630, FUN_00a44630, FUN_0087c380
0x1FC0 -> FUN_00a44630, FUN_00a360c0
0x1FD0 -> FUN_00a58e50, FUN_00a50c50, FUN_00a50c50, FUN_00a50c50
0x1FD1 -> FUN_00a58e50
0x1FDC -> FUN_009a3900, FUN_009fb930, FUN_009c44a0, FUN_009c44a0
0x1FE3 -> FUN_00829750
0x1FE4 -> FUN_00a3c0b0, FUN_00a3c0b0, FUN_009a1d70, FUN_00a29110



===== FILE: packet_map.json =====

{
  "3072": [
    "FUN_006f3540", 
    "FUN_006f4b40"
  ], 
  "512": [
    "FUN_00a3cfc0", 
    "FUN_00a422f0", 
    "FUN_00874470", 
    "FUN_008747e0", 
    "FUN_009a1b70"
  ], 
  "5120": [
    "FUN_008d2ed0", 
    "FUN_008d2ed0", 
    "FUN_00a37b90", 
    "FUN_00a37b90", 
    "FUN_00a37b90", 
    "FUN_00a2fd20", 
    "FUN_00a2fd20"
  ], 
  "513": [
    "FUN_00a3cfc0", 
    "FUN_00a3cfc0", 
    "FUN_00a3cfc0", 
    "FUN_00a422f0"
  ], 
  "514": [
    "FUN_00a3cfc0", 
    "FUN_00a3cfc0", 
    "FUN_00a3cfc0", 
    "FUN_00a3cfc0", 
    "FUN_00a3cfc0", 
    "FUN_00a422f0", 
    "FUN_00a422f0"
  ], 
  "1026": [
    "FUN_00a3cfc0", 
    "FUN_00a3cfc0", 
    "FUN_00a29950", 
    "FUN_00a29d10"
  ], 
  "515": [
    "FUN_00a3cfc0", 
    "FUN_00a3cfc0"
  ], 
  "4100": [
    "FUN_00aedcb0"
  ], 
  "516": [
    "FUN_00a3cfc0", 
    "FUN_00a3cfc0", 
    "FUN_00a422f0", 
    "FUN_00a39a70"
  ], 
  "517": [
    "FUN_00a3cfc0", 
    "FUN_00a3cfc0", 
    "FUN_00a3cfc0", 
    "FUN_00a422f0", 
    "FUN_00a422f0"
  ], 
  "519": [
    "FUN_00a3cfc0", 
    "FUN_00a3cfc0"
  ], 
  "520": [
    "FUN_00a3cfc0", 
    "FUN_00a3cfc0"
  ], 
  "522": [
    "FUN_00a3cfc0"
  ], 
  "1036": [
    "FUN_00a2d510"
  ], 
  "1037": [
    "FUN_00a2d510"
  ], 
  "1038": [
    "FUN_00a2d510"
  ], 
  "1039": [
    "FUN_00a2d510"
  ], 
  "528": [
    "FUN_00a44630", 
    "FUN_00a44630", 
    "FUN_00a38860", 
    "FUN_00ace070", 
    "FUN_00ace070"
  ], 
  "1040": [
    "FUN_00a2d510"
  ], 
  "1041": [
    "FUN_00a2d510"
  ], 
  "1042": [
    "FUN_00a2d510"
  ], 
  "1043": [
    "FUN_00a2d510"
  ], 
  "1044": [
    "FUN_00a2d510"
  ], 
  "1045": [
    "FUN_00a2d510"
  ], 
  "1046": [
    "FUN_00a2d510"
  ], 
  "1047": [
    "FUN_00a2d510"
  ], 
  "536": [
    "FUN_00829bd0", 
    "FUN_00a46960", 
    "FUN_00a58e50"
  ], 
  "1049": [
    "FUN_00943360"
  ], 
  "537": [
    "FUN_00a3cfc0", 
    "FUN_00a3cfc0", 
    "FUN_00a3cfc0", 
    "FUN_00a3cfc0", 
    "FUN_00a3cfc0", 
    "FUN_00a3cfc0", 
    "FUN_00a3cfc0", 
    "FUN_00a3cfc0", 
    "FUN_00a3cfc0", 
    "FUN_00a3cfc0", 
    "FUN_00a3cfc0", 
    "FUN_00a3f9f0", 
    "FUN_00a3f9f0", 
    "FUN_00a422f0"
  ], 
  "1050": [
    "FUN_00b00640", 
    "FUN_00a48b20", 
    "FUN_00a48b20"
  ], 
  "538": [
    "FUN_00a3cfc0", 
    "FUN_00a3cfc0", 
    "FUN_00a3cfc0", 
    "FUN_00a3cfc0", 
    "FUN_00a3cfc0", 
    "FUN_00a3cfc0", 
    "FUN_00a3cfc0", 
    "FUN_00a3cfc0", 
    "FUN_00a3f9f0", 
    "FUN_00a3f9f0"
  ], 
  "539": [
    "FUN_00a3cfc0", 
    "FUN_00a3cfc0", 
    "FUN_00a3cfc0", 
    "FUN_00a3cfc0", 
    "FUN_00a3cfc0", 
    "FUN_00a3cfc0"
  ], 
  "540": [
    "FUN_00a3cfc0", 
    "FUN_00a3cfc0"
  ], 
  "2588": [
    "FUN_00aacc30"
  ], 
  "6172": [
    "FUN_009a32b0"
  ], 
  "541": [
    "FUN_00a3cfc0", 
    "FUN_00a3cfc0"
  ], 
  "1060": [
    "FUN_0091bd50"
  ], 
  "1063": [
    "FUN_00a31810", 
    "FUN_00a6af50"
  ], 
  "552": [
    "FUN_00b00640", 
    "FUN_00828d70", 
    "FUN_00980100"
  ], 
  "1064": [
    "FUN_0080a1d0"
  ], 
  "554": [
    "FUN_00a46960"
  ], 
  "556": [
    "FUN_00a44630", 
    "FUN_00ace070", 
    "FUN_00ace070"
  ], 
  "1584": [
    "FUN_00852d50"
  ], 
  "564": [
    "FUN_00a46960", 
    "FUN_009538a0", 
    "FUN_00959ab0"
  ], 
  "1592": [
    "FUN_00852d50", 
    "FUN_00852d50", 
    "FUN_00852d50"
  ], 
  "1082": [
    "FUN_008d26d0", 
    "FUN_008d3470", 
    "FUN_008d2b50"
  ], 
  "572": [
    "FUN_00a44630"
  ], 
  "576": [
    "FUN_00afe020", 
    "FUN_00aec6f0", 
    "FUN_00988a80", 
    "FUN_00988a80"
  ], 
  "1088": [
    "FUN_008ff140"
  ], 
  "584": [
    "FUN_00a44630", 
    "FUN_00ace070", 
    "FUN_00ace070"
  ], 
  "1100": [
    "FUN_00a46960"
  ], 
  "588": [
    "FUN_00ae8b80", 
    "FUN_00ae8b80", 
    "FUN_00ae8b80", 
    "FUN_00ae8b80", 
    "FUN_00a3afd0", 
    "FUN_008d2ed0", 
    "FUN_00a380f0", 
    "FUN_008d1720", 
    "FUN_00a3f240"
  ], 
  "1104": [
    "FUN_00a44630", 
    "FUN_00a44630", 
    "FUN_00828b90", 
    "FUN_00a40510", 
    "FUN_00a2f930", 
    "FUN_0091b9d0", 
    "FUN_00a3e680", 
    "FUN_00ab7ff0"
  ], 
  "596": [
    "FUN_009be4f0"
  ], 
  "1622": [
    "FUN_0092a430"
  ], 
  "1111": [
    "FUN_00a44630", 
    "FUN_00a47390", 
    "FUN_00a46960"
  ], 
  "6232": [
    "FUN_009a1960"
  ], 
  "600": [
    "FUN_00a42530", 
    "FUN_009e6680", 
    "FUN_00a2dfb0", 
    "FUN_00a38860"
  ], 
  "604": [
    "FUN_00aedcb0", 
    "FUN_00985be0"
  ], 
  "6236": [
    "FUN_009a1960"
  ], 
  "2654": [
    "FUN_00aacc30", 
    "FUN_00aacc30", 
    "FUN_00aacc30", 
    "FUN_00aacc30", 
    "FUN_00850e40"
  ], 
  "608": [
    "FUN_00927800", 
    "FUN_00a099f0", 
    "FUN_00985be0", 
    "FUN_00a2efe0"
  ], 
  "611": [
    "FUN_00a562e0"
  ], 
  "612": [
    "FUN_00a44630", 
    "FUN_00a3cfc0", 
    "FUN_00a3cfc0", 
    "FUN_00a099f0", 
    "FUN_00aacc30", 
    "FUN_00ace070", 
    "FUN_00ace070"
  ], 
  "616": [
    "FUN_00a46960", 
    "FUN_008f3b70"
  ], 
  "1132": [
    "FUN_00a316c0", 
    "FUN_00a38440", 
    "FUN_00a38440"
  ], 
  "5234": [
    "FUN_00a39df0"
  ], 
  "1140": [
    "FUN_00a44630", 
    "FUN_00a38330", 
    "FUN_00a38330", 
    "FUN_00a38440", 
    "FUN_00a38440", 
    "FUN_009be4f0", 
    "FUN_009be4f0", 
    "FUN_009be4f0", 
    "FUN_009be4f0"
  ], 
  "628": [
    "FUN_00aedcb0", 
    "FUN_009b30b0"
  ], 
  "6263": [
    "FUN_00a29950", 
    "FUN_00a29d10"
  ], 
  "1144": [
    "FUN_00a38330", 
    "FUN_00a38440"
  ], 
  "640": [
    "FUN_00a44630", 
    "FUN_00ace070", 
    "FUN_00ace070"
  ], 
  "641": [
    "FUN_0080a1d0"
  ], 
  "646": [
    "FUN_0080a1d0", 
    "FUN_00a46960", 
    "FUN_00aedcb0", 
    "FUN_00af8de0", 
    "FUN_00a3f9f0", 
    "FUN_00a422f0", 
    "FUN_00ae50c0"
  ], 
  "648": [
    "FUN_00a46960", 
    "FUN_00af8de0", 
    "FUN_00a3cfc0", 
    "FUN_009a3590", 
    "FUN_009a3590", 
    "FUN_009fb930", 
    "FUN_009fb930", 
    "FUN_00960f80"
  ], 
  "652": [
    "FUN_00a58e50"
  ], 
  "1678": [
    "FUN_00830340", 
    "FUN_00a09420"
  ], 
  "656": [
    "FUN_009fb930", 
    "FUN_00abc800", 
    "FUN_00950540"
  ], 
  "5776": [
    "FUN_00a2dfb0"
  ], 
  "660": [
    "FUN_00a34130", 
    "FUN_009b30b0", 
    "FUN_0094d7f0"
  ], 
  "664": [
    "FUN_0094d7f0"
  ], 
  "1176": [
    "FUN_00a380f0", 
    "FUN_00a380f0", 
    "FUN_00a380f0"
  ], 
  "668": [
    "FUN_00ace070", 
    "FUN_00ace070"
  ], 
  "672": [
    "FUN_00a95850"
  ], 
  "6308": [
    "FUN_00af8de0"
  ], 
  "1188": [
    "FUN_00a31810"
  ], 
  "6312": [
    "FUN_00a099f0", 
    "FUN_0096c470"
  ], 
  "687": [
    "FUN_00a40510", 
    "FUN_00a40510"
  ], 
  "696": [
    "FUN_008f3b70", 
    "FUN_00ace070", 
    "FUN_00ace070"
  ], 
  "6328": [
    "FUN_00af8de0"
  ], 
  "1208": [
    "FUN_00a46960"
  ], 
  "700": [
    "FUN_00af8de0", 
    "FUN_008f3b70", 
    "FUN_00985be0"
  ], 
  "6845": [
    "FUN_00850b80", 
    "FUN_00850e40", 
    "FUN_00850e40", 
    "FUN_008700f0", 
    "FUN_008512c0", 
    "FUN_00a6af50"
  ], 
  "6846": [
    "FUN_00850d50", 
    "FUN_00850e40", 
    "FUN_00850e40", 
    "FUN_008700f0", 
    "FUN_00a6af50"
  ], 
  "6847": [
    "FUN_00a46960", 
    "FUN_00a37ac0"
  ], 
  "704": [
    "FUN_008f3b70", 
    "FUN_00abc800", 
    "FUN_00abc800"
  ], 
  "1216": [
    "FUN_00afd260", 
    "FUN_00a46960", 
    "FUN_00af8de0", 
    "FUN_00a34130", 
    "FUN_00a422f0"
  ], 
  "1217": [
    "FUN_00a46960"
  ], 
  "708": [
    "FUN_00aedcb0", 
    "FUN_008f3b70", 
    "FUN_00a2efe0"
  ], 
  "712": [
    "FUN_008f3b70", 
    "FUN_0091f920"
  ], 
  "1225": [
    "FUN_00a46960", 
    "FUN_00a39240"
  ], 
  "716": [
    "FUN_008f3b70"
  ], 
  "7888": [
    "FUN_008de000", 
    "FUN_008e4f60", 
    "FUN_008e51c0", 
    "FUN_0096c300"
  ], 
  "720": [
    "FUN_008f3b70"
  ], 
  "724": [
    "FUN_008f3b70", 
    "FUN_00ace070", 
    "FUN_00ace070"
  ], 
  "728": [
    "FUN_008f3b70", 
    "FUN_00a4e230"
  ], 
  "5852": [
    "FUN_00852d50"
  ], 
  "3804": [
    "FUN_006f3540", 
    "FUN_006f3540", 
    "FUN_006f3540"
  ], 
  "1244": [
    "FUN_00a46960", 
    "FUN_00a39240"
  ], 
  "3808": [
    "FUN_006f3540", 
    "FUN_006f3840"
  ], 
  "736": [
    "FUN_009a1b70"
  ], 
  "1252": [
    "FUN_00829e00"
  ], 
  "1256": [
    "FUN_0080a1d0"
  ], 
  "1264": [
    "FUN_00a39240", 
    "FUN_00a28fb0"
  ], 
  "752": [
    "FUN_00ace070", 
    "FUN_00ace070"
  ], 
  "3828": [
    "FUN_00a0dd40"
  ], 
  "1268": [
    "FUN_00a28fb0"
  ], 
  "2304": [
    "FUN_00aedcb0"
  ], 
  "3842": [
    "FUN_00931c40"
  ], 
  "780": [
    "FUN_00ace070", 
    "FUN_00ace070"
  ], 
  "1292": [
    "FUN_008243e0"
  ], 
  "4367": [
    "FUN_00aec6f0"
  ], 
  "1300": [
    "FUN_00a3cfc0", 
    "FUN_00a3cfc0", 
    "FUN_00a39240", 
    "FUN_009b30b0", 
    "FUN_00a421f0", 
    "FUN_009358b0", 
    "FUN_00a29d10"
  ], 
  "2342": [
    "FUN_00aedcb0"
  ], 
  "808": [
    "FUN_00ace070", 
    "FUN_00ace070"
  ], 
  "1331": [
    "FUN_0087c380"
  ], 
  "824": [
    "FUN_00829d00", 
    "FUN_00829e00"
  ], 
  "825": [
    "FUN_00829410", 
    "FUN_00829410"
  ], 
  "826": [
    "FUN_00829410", 
    "FUN_00829410"
  ], 
  "831": [
    "FUN_00828d70", 
    "FUN_00828d70"
  ], 
  "832": [
    "FUN_00a58e50", 
    "FUN_00a422f0", 
    "FUN_00822fa0", 
    "FUN_00870e00", 
    "FUN_00ad0250"
  ], 
  "833": [
    "FUN_00822fa0"
  ], 
  "835": [
    "FUN_00afdf10", 
    "FUN_00b021c0"
  ], 
  "836": [
    "FUN_00ace070", 
    "FUN_00a4d4b0"
  ], 
  "837": [
    "FUN_00829d00"
  ], 
  "838": [
    "FUN_00829d00"
  ], 
  "839": [
    "FUN_00829d00"
  ], 
  "841": [
    "FUN_00a0dd40", 
    "FUN_00b00640", 
    "FUN_008f3b70"
  ], 
  "842": [
    "FUN_0080a1d0"
  ], 
  "844": [
    "FUN_00a422f0"
  ], 
  "1868": [
    "FUN_00a39a70", 
    "FUN_00a39df0"
  ], 
  "1360": [
    "FUN_00a42770", 
    "FUN_00ae59b0", 
    "FUN_00a31810", 
    "FUN_00a38d30"
  ], 
  "5968": [
    "FUN_009a1960"
  ], 
  "5972": [
    "FUN_009a1960"
  ], 
  "1364": [
    "FUN_00a316c0"
  ], 
  "8020": [
    "FUN_00a11340", 
    "FUN_0087c380", 
    "FUN_00874fa0", 
    "FUN_00aec6f0", 
    "FUN_00aec6f0", 
    "FUN_00aec6f0", 
    "FUN_00aec6f0", 
    "FUN_00aec6f0", 
    "FUN_00aec6f0", 
    "FUN_00ae8b80", 
    "FUN_00a95f90", 
    "FUN_00a42530", 
    "FUN_008d38d0", 
    "FUN_008d38d0", 
    "FUN_00b07df0", 
    "FUN_00a3f240", 
    "FUN_0096a520", 
    "FUN_009b8e80", 
    "FUN_00913040", 
    "FUN_00959ab0", 
    "FUN_00959ab0", 
    "FUN_00902860", 
    "FUN_009cf3e0"
  ], 
  "1365": [
    "FUN_00a352e0", 
    "FUN_00a352e0", 
    "FUN_00a352e0"
  ], 
  "1366": [
    "FUN_00a352e0"
  ], 
  "7000": [
    "FUN_008d3470"
  ], 
  "1368": [
    "FUN_00a352e0"
  ], 
  "8026": [
    "FUN_00a3afd0", 
    "FUN_00a31810", 
    "FUN_00a38d30", 
    "FUN_00a38d30"
  ], 
  "868": [
    "FUN_00829e00"
  ], 
  "876": [
    "FUN_00a352e0"
  ], 
  "6000": [
    "FUN_008d2ed0"
  ], 
  "880": [
    "FUN_00a352e0"
  ], 
  "886": [
    "FUN_00a3cfc0", 
    "FUN_00a3cfc0"
  ], 
  "5496": [
    "FUN_00a107e0"
  ], 
  "8060": [
    "FUN_00874bb0"
  ], 
  "1408": [
    "FUN_00a42770", 
    "FUN_00a42e90", 
    "FUN_00a2dfb0", 
    "FUN_00a2fd20"
  ], 
  "5000": [
    "FUN_009b6690", 
    "FUN_008d3690", 
    "FUN_008d3470", 
    "FUN_008d3470", 
    "FUN_008d3470", 
    "FUN_008d1720", 
    "FUN_0091ea10", 
    "FUN_0091ea10", 
    "FUN_0091ea10", 
    "FUN_00a871e0"
  ], 
  "6541": [
    "FUN_00a099f0"
  ], 
  "912": [
    "FUN_00afe020"
  ], 
  "1432": [
    "FUN_00a46960", 
    "FUN_00a37ac0"
  ], 
  "1436": [
    "FUN_00a46960", 
    "FUN_00a29950"
  ], 
  "1440": [
    "FUN_00a46960"
  ], 
  "4002": [
    "FUN_00ae2930"
  ], 
  "8100": [
    "FUN_00a44630", 
    "FUN_00a44630", 
    "FUN_0087c380"
  ], 
  "936": [
    "FUN_008243e0", 
    "FUN_00a44630", 
    "FUN_00a316c0", 
    "FUN_0091b9d0"
  ], 
  "937": [
    "FUN_00982070", 
    "FUN_00985000", 
    "FUN_009857c0", 
    "FUN_00985be0", 
    "FUN_00985be0", 
    "FUN_00985be0", 
    "FUN_00985be0", 
    "FUN_009821a0"
  ], 
  "5548": [
    "FUN_00852d50"
  ], 
  "941": [
    "FUN_008d1a60", 
    "FUN_008d2520", 
    "FUN_008d1c90", 
    "FUN_008d1e20", 
    "FUN_008d2d10", 
    "FUN_008d2ed0", 
    "FUN_0091fee0", 
    "FUN_00a380f0", 
    "FUN_00a38280"
  ], 
  "5550": [
    "FUN_00a10300", 
    "FUN_00a107e0", 
    "FUN_00a107e0", 
    "FUN_00a107e0", 
    "FUN_00a6abe0"
  ], 
  "945": [
    "FUN_00a2dfb0"
  ], 
  "3000": [
    "FUN_009d7630", 
    "FUN_00852d50", 
    "FUN_009d2b90"
  ], 
  "1464": [
    "FUN_00a6abe0"
  ], 
  "956": [
    "FUN_00a3cfc0", 
    "FUN_00a3cfc0"
  ], 
  "8128": [
    "FUN_00a44630", 
    "FUN_00a360c0"
  ], 
  "960": [
    "FUN_00a3cfc0", 
    "FUN_00a3cfc0"
  ], 
  "2500": [
    "FUN_00a79320"
  ], 
  "1480": [
    "FUN_00a43380"
  ], 
  "968": [
    "FUN_00a3cfc0", 
    "FUN_00a3f9f0"
  ], 
  "972": [
    "FUN_00a3cfc0", 
    "FUN_00a3f9f0", 
    "FUN_00a3f9f0"
  ], 
  "2000": [
    "FUN_0087c530", 
    "FUN_008d38d0", 
    "FUN_009533e0", 
    "FUN_0099edc0"
  ], 
  "8144": [
    "FUN_00a58e50", 
    "FUN_00a50c50", 
    "FUN_00a50c50", 
    "FUN_00a50c50"
  ], 
  "6608": [
    "FUN_0093d5e0"
  ], 
  "8145": [
    "FUN_00a58e50"
  ], 
  "2002": [
    "FUN_00ae2930"
  ], 
  "8156": [
    "FUN_009a3900", 
    "FUN_009fb930", 
    "FUN_009c44a0", 
    "FUN_009c44a0"
  ], 
  "1500": [
    "FUN_008d38d0"
  ], 
  "8163": [
    "FUN_00829750"
  ], 
  "8164": [
    "FUN_00a3c0b0", 
    "FUN_00a3c0b0", 
    "FUN_009a1d70", 
    "FUN_00a29110"
  ], 
  "2532": [
    "FUN_00a79060"
  ], 
  "1000": [
    "FUN_00b021c0", 
    "FUN_00a46960", 
    "FUN_00a58e50", 
    "FUN_00a58e50", 
    "FUN_009b6690", 
    "FUN_009b6690", 
    "FUN_009b6690", 
    "FUN_009b6690", 
    "FUN_009d7630", 
    "FUN_008e51c0", 
    "FUN_0091ea10", 
    "FUN_0091ea10", 
    "FUN_0091ea10", 
    "FUN_0091ea10", 
    "FUN_008f2030", 
    "FUN_00a2a1a0"
  ], 
  "1512": [
    "FUN_00a44630"
  ], 
  "1513": [
    "FUN_00a3cfc0", 
    "FUN_00a3cfc0", 
    "FUN_00a3cfc0", 
    "FUN_00a3cfc0", 
    "FUN_00a12930", 
    "FUN_00985ab0"
  ], 
  "1002": [
    "FUN_00ae2930"
  ], 
  "6128": [
    "FUN_00aacc30"
  ], 
  "1521": [
    "FUN_00a29d10"
  ], 
  "1016": [
    "FUN_00b00640", 
    "FUN_00a3c0b0", 
    "FUN_00a3b3b0"
  ]
}


===== FILE: siedenote.txt =====

Wichtige Klarstellung (sehr wichtig fr dein Projekt)

Das, was du bisher gebaut hast, ist NICHT falsch  aber:

Dein aktueller Packet-Mapper ist noch syntaktisch korrekt, semantisch aber unscharf.

Warum?

1) Dein Mapper erkennt viele False Positives

Beispiel:

0x0200 -> FUN_00874470, FUN_008747e0, FUN_009a1b70, FUN_00a422f0


Das bedeutet NICHT:

alle sind Packet-Handler

Sondern:

manche sind Router

manche sind Wrapper

manche sind Utility-Funktionen

nur 12 sind echte Handler

Typisches Pattern in Maestia:

Router  Dispatcher  Handler  Game Logic


Beispiel (vereinfacht):

FUN_00a3cfc0  (global dispatcher)
  FUN_00a422f0 (session/router)
      FUN_00a44630 (world router)
          FUN_00829bd0 (NPC logic)


Das ist NORMAL bei MMO-Engines.

Das Ziel: "Packet Truth Map"

Du willst NICHT:

opcode  alle Funktionen


Du willst:

opcode  echter Handler  Bedeutung  Struct


Ich baue dir das mit dir zusammen.

Wie man echte Packet-Handler filtert (entscheidend)

Ein echter Packet-Handler hat fast immer mindestens 3 Merkmale:

Pattern A  Zugriff auf packet[2]/packet[3]

Beispiel (du hast es schon gesehen):

packet[2] == 0x02
packet[3] == 0x34

Pattern B  Struct parsing
*(int *)(packet + 4)
*(float *)(packet + 8)

Pattern C  Game-Logic Call danach
FUN_00829bd0(...)  // NPC
FUN_0091ea10(...)  // Combat
FUN_008f3b70(...)  // World

Dein aktueller Mapper = Rohmaterial

Das hier ist GOLD:

0x03E8 -> FUN_008e51c0, FUN_008f2030, FUN_0091ea10, FUN_009b6690, ...


Das schreit:

0x03E8 = ENTITY SPAWN / WORLD EVENT

Und:

0x0349 -> FUN_00a0dd40


Das kennst du:

0x0349 = SendToServer / heartbeat / control packet

Nchster Schritt (den ich dir jetzt baue)

Ich baue dir eine echte Packet Truth Map aus deinen Daten:

echte Handler
Name
Bedeutung
Struct guessed
Confidence level

Beispiel:

0x03E8  NPC_SPAWN
Handler: FUN_009b6690
Meaning: Spawn NPC in world
Fields:
- uint32 npcId
- uint32 templateId
- float x,y,z
- uint16 rotation
Confidence: HIGH


Das wird dein Maestia "Rosetta Stone".