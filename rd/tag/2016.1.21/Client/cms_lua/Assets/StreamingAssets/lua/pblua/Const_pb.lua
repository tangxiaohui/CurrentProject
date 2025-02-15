-- Generated By protoc-gen-lua Do not Edit
local protobuf = require "protobuf"
module('Const_pb')


local CHATTYPE = protobuf.EnumDescriptor();
local CHATTYPE_CHAT_WORLD_ENUM = protobuf.EnumValueDescriptor();
local CHATTYPE_CHAT_BROADCAST_ENUM = protobuf.EnumValueDescriptor();
local CHATTYPE_WORLD_BROADCAST_ENUM = protobuf.EnumValueDescriptor();
local KICKREASON = protobuf.EnumDescriptor();
local KICKREASON_DUPLICATE_LOGIN_ENUM = protobuf.EnumValueDescriptor();
local KICKREASON_SERVER_SHUTDOWN_ENUM = protobuf.EnumValueDescriptor();
local KICKREASON_LOGIN_FORBIDEN_ENUM = protobuf.EnumValueDescriptor();
local KICKREASON_KICKED_OUT_ENUM = protobuf.EnumValueDescriptor();
local KICKREASON_SERVER_RESTART_ENUM = protobuf.EnumValueDescriptor();
local REWARDREASON = protobuf.EnumDescriptor();
local REWARDREASON_SYS_PRESENT_ENUM = protobuf.EnumValueDescriptor();
local REWARDREASON_NPC_ENUM = protobuf.EnumValueDescriptor();
local REWARDREASON_QUEST_ENUM = protobuf.EnumValueDescriptor();
local REWARDREASON_ACITVITY_ENUM = protobuf.EnumValueDescriptor();

CHATTYPE_CHAT_WORLD_ENUM.name = "CHAT_WORLD"
CHATTYPE_CHAT_WORLD_ENUM.index = 0
CHATTYPE_CHAT_WORLD_ENUM.number = 1
CHATTYPE_CHAT_BROADCAST_ENUM.name = "CHAT_BROADCAST"
CHATTYPE_CHAT_BROADCAST_ENUM.index = 1
CHATTYPE_CHAT_BROADCAST_ENUM.number = 2
CHATTYPE_WORLD_BROADCAST_ENUM.name = "WORLD_BROADCAST"
CHATTYPE_WORLD_BROADCAST_ENUM.index = 2
CHATTYPE_WORLD_BROADCAST_ENUM.number = 3
CHATTYPE.name = "chatType"
CHATTYPE.full_name = ".chatType"
CHATTYPE.values = {CHATTYPE_CHAT_WORLD_ENUM,CHATTYPE_CHAT_BROADCAST_ENUM,CHATTYPE_WORLD_BROADCAST_ENUM}
KICKREASON_DUPLICATE_LOGIN_ENUM.name = "DUPLICATE_LOGIN"
KICKREASON_DUPLICATE_LOGIN_ENUM.index = 0
KICKREASON_DUPLICATE_LOGIN_ENUM.number = 1
KICKREASON_SERVER_SHUTDOWN_ENUM.name = "SERVER_SHUTDOWN"
KICKREASON_SERVER_SHUTDOWN_ENUM.index = 1
KICKREASON_SERVER_SHUTDOWN_ENUM.number = 2
KICKREASON_LOGIN_FORBIDEN_ENUM.name = "LOGIN_FORBIDEN"
KICKREASON_LOGIN_FORBIDEN_ENUM.index = 2
KICKREASON_LOGIN_FORBIDEN_ENUM.number = 3
KICKREASON_KICKED_OUT_ENUM.name = "KICKED_OUT"
KICKREASON_KICKED_OUT_ENUM.index = 3
KICKREASON_KICKED_OUT_ENUM.number = 4
KICKREASON_SERVER_RESTART_ENUM.name = "SERVER_RESTART"
KICKREASON_SERVER_RESTART_ENUM.index = 4
KICKREASON_SERVER_RESTART_ENUM.number = 5
KICKREASON.name = "kickReason"
KICKREASON.full_name = ".kickReason"
KICKREASON.values = {KICKREASON_DUPLICATE_LOGIN_ENUM,KICKREASON_SERVER_SHUTDOWN_ENUM,KICKREASON_LOGIN_FORBIDEN_ENUM,KICKREASON_KICKED_OUT_ENUM,KICKREASON_SERVER_RESTART_ENUM}
REWARDREASON_SYS_PRESENT_ENUM.name = "SYS_PRESENT"
REWARDREASON_SYS_PRESENT_ENUM.index = 0
REWARDREASON_SYS_PRESENT_ENUM.number = 1
REWARDREASON_NPC_ENUM.name = "NPC"
REWARDREASON_NPC_ENUM.index = 1
REWARDREASON_NPC_ENUM.number = 2
REWARDREASON_QUEST_ENUM.name = "QUEST"
REWARDREASON_QUEST_ENUM.index = 2
REWARDREASON_QUEST_ENUM.number = 3
REWARDREASON_ACITVITY_ENUM.name = "ACITVITY"
REWARDREASON_ACITVITY_ENUM.index = 3
REWARDREASON_ACITVITY_ENUM.number = 4
REWARDREASON.name = "RewardReason"
REWARDREASON.full_name = ".RewardReason"
REWARDREASON.values = {REWARDREASON_SYS_PRESENT_ENUM,REWARDREASON_NPC_ENUM,REWARDREASON_QUEST_ENUM,REWARDREASON_ACITVITY_ENUM}

ACITVITY = 4
CHAT_BROADCAST = 2
CHAT_WORLD = 1
DUPLICATE_LOGIN = 1
KICKED_OUT = 4
LOGIN_FORBIDEN = 3
NPC = 2
QUEST = 3
SERVER_RESTART = 5
SERVER_SHUTDOWN = 2
SYS_PRESENT = 1
WORLD_BROADCAST = 3

