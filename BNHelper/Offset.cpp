#include "Global.h"
#include "Offset.h"


DWORD GameBase;
DWORD GameVersion;

//
DWORD SayInRoomEntry;
DWORD SayInRoomBack;
DWORD SayInRoomSkip;

DWORD SayInGameEntry;
DWORD SayInGameBack;
DWORD SayInGameSkip;

DWORD DisplayInRoomEntry;
DWORD DisplayInRoomBack;

DWORD GameStateChangeEntry;
DWORD GameStateChangeBack;

DWORD TextInRoomFunction;
DWORD TextInRoomOffset1;
DWORD TextInRoomOffset2;
DWORD TextInRoomOffset3;

DWORD TextInGameFunction;
DWORD TextInGameWriteData;

//
DWORD IsInGame1;
DWORD IsInGame2;

DWORD GetPlayerName1;
DWORD GetPlayerName2;

DWORD GetPlayerHandleFunction;

DWORD ClickEntry;
DWORD ClickBack;
DWORD IsUnitInvisibleEntry;
DWORD IsUnitInvisibleOffset;
DWORD GetPlayerColorFunction;

DWORD ClickHookOffset1;
DWORD ClickHookOffset2;
DWORD ClickHookOffset3;



void InitializeOffset()
{
	if (GameVersion == V124E)
	{
		//
		SayInRoomEntry = GameBase + V124E_SAY_IN_ROOM_ENTRY;
		SayInRoomBack = GameBase + V124E_SAY_IN_ROOM_BACK;
		SayInRoomSkip = GameBase + V124E_SAY_IN_ROOM_SKIP;

		SayInGameEntry = GameBase + V124E_SAY_IN_GAME_ENTRY;
		SayInGameBack = GameBase + V124E_SAY_IN_GAME_BACK;
		SayInGameSkip = GameBase + V124E_SAY_IN_GAME_SKIP;

		DisplayInRoomEntry = GameBase + V124E_DISPLAY_IN_ROOM_ENTRY;
		DisplayInRoomBack = GameBase + V124E_DISPLAY_IN_ROOM_BACK;

		GameStateChangeEntry = GameBase + V124E_GAME_STATE_CHANGE_ENTRY;
		GameStateChangeBack = GameBase + V124E_GAME_STATE_CHANGE_BACK;

		TextInRoomFunction = GameBase + V124E_TEXT_IN_ROOM_FUNCTION;
		TextInRoomOffset1 = V124E_TEXT_IN_ROOM_OFFSET_1;
		TextInRoomOffset2 = V124E_TEXT_IN_ROOM_OFFSET_2;
		TextInRoomOffset3 = V124E_TEXT_IN_ROOM_OFFSET_3;

		TextInGameFunction = GameBase + V124E_TEXT_IN_GAME_FUNCTION;
		TextInGameWriteData = GameBase + V124E_TEXT_IN_GAME_WRITE_DATA;

		//
		IsInGame1 = GameBase + V124E_IS_IN_GAME_1;
		IsInGame2 = GameBase + V124E_IS_IN_GAME_2;

		GetPlayerName1 = GameBase + V124E_GET_PLAYER_NAME_1;
		GetPlayerName2 = GameBase + V124E_GET_PLAYER_NAME_2;

		GetPlayerHandleFunction = GameBase + V124E_GET_PLAYER_HANDLE_FUNCTION;

		ClickEntry = GameBase + V124E_CLICK_ENTRY;
		ClickBack = GameBase + V124E_CLICK_BACK;
		IsUnitInvisibleEntry = GameBase + V124E_IS_UNIT_INVISIBLE_ENTRY;
		IsUnitInvisibleOffset = V124E_IS_UNIT_INVISIBLE_OFFSET;
		GetPlayerColorFunction = GameBase + V124E_GET_PLAYER_COLOR_FUNCTION;

		ClickHookOffset1 = GameBase + V124E_CLICK_HOOK_OFFSET_1;
		ClickHookOffset2 = GameBase + V124E_CLICK_HOOK_OFFSET_2;
		ClickHookOffset3 = V124E_CLICK_HOOK_OFFSET_3;
	}
	else if (GameVersion == V124B)
	{
				//
		SayInRoomEntry = GameBase + V124B_SAY_IN_ROOM_ENTRY;
		SayInRoomBack = GameBase + V124B_SAY_IN_ROOM_BACK;
		SayInRoomSkip = GameBase + V124B_SAY_IN_ROOM_SKIP;

		SayInGameEntry = GameBase + V124B_SAY_IN_GAME_ENTRY;
		SayInGameBack = GameBase + V124B_SAY_IN_GAME_BACK;
		SayInGameSkip = GameBase + V124B_SAY_IN_GAME_SKIP;

		DisplayInRoomEntry = GameBase + V124B_DISPLAY_IN_ROOM_ENTRY;
		DisplayInRoomBack = GameBase + V124B_DISPLAY_IN_ROOM_BACK;

		GameStateChangeEntry = GameBase + V124B_GAME_STATE_CHANGE_ENTRY;
		GameStateChangeBack = GameBase + V124B_GAME_STATE_CHANGE_BACK;

		TextInRoomFunction = GameBase + V124B_TEXT_IN_ROOM_FUNCTION;
		TextInRoomOffset1 = V124B_TEXT_IN_ROOM_OFFSET_1;
		TextInRoomOffset2 = V124B_TEXT_IN_ROOM_OFFSET_2;
		TextInRoomOffset3 = V124B_TEXT_IN_ROOM_OFFSET_3;

		TextInGameFunction = GameBase + V124B_TEXT_IN_GAME_FUNCTION;
		TextInGameWriteData = GameBase + V124B_TEXT_IN_GAME_WRITE_DATA;

		//
		IsInGame1 = GameBase + V124B_IS_IN_GAME_1;
		IsInGame2 = GameBase + V124B_IS_IN_GAME_2;

		GetPlayerName1 = GameBase + V124B_GET_PLAYER_NAME_1;
		GetPlayerName2 = GameBase + V124B_GET_PLAYER_NAME_2;

		GetPlayerHandleFunction = GameBase + V124B_GET_PLAYER_HANDLE_FUNCTION;

		ClickEntry = GameBase + V124B_CLICK_ENTRY;
		ClickBack = GameBase + V124B_CLICK_BACK;
		IsUnitInvisibleEntry = GameBase + V124B_IS_UNIT_INVISIBLE_ENTRY;
		IsUnitInvisibleOffset = V124B_IS_UNIT_INVISIBLE_OFFSET;
		GetPlayerColorFunction = GameBase + V124B_GET_PLAYER_COLOR_FUNCTION;

		ClickHookOffset1 = GameBase + V124B_CLICK_HOOK_OFFSET_1;
		ClickHookOffset2 = GameBase + V124B_CLICK_HOOK_OFFSET_2;
		ClickHookOffset3 = V124B_CLICK_HOOK_OFFSET_3;
	}
}