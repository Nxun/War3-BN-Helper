#include "Global.h"
#include "Offset.h"

extern DWORD GameBase;

extern DWORD IsInGame1;
extern DWORD IsInGame2;

extern DWORD GetPlayerName1;
extern DWORD GetPlayerName2;

extern DWORD GetPlayerHandleFunction;

extern DWORD ClickEntry;
extern DWORD ClickBack;
extern DWORD IsUnitInvisibleEntry;
extern DWORD IsUnitInvisibleOffset;
extern DWORD GetPlayerColorFunction;

extern DWORD ClickHookOffset1;
extern DWORD ClickHookOffset2;
extern DWORD ClickHookOffset3;

char* name;
DWORD playerId;
DWORD playerHandle;
DWORD playerColor;
DWORD unit;
DWORD address;
DWORD result;

CRITICAL_SECTION ClickSection;


char* GetPlayerName(UINT hPlayer)
{
	char* retaddr = "Unkown Player";

	__asm
	{
		mov ecx, hPlayer;
		//mov eax, GameBase;
		//lea eax, dword ptr ds:[GetPlayerName1];
		//call eax;
		call GetPlayerName1;
		test eax, eax;
		jz NOPLAYER
			;
		push 1;
		mov ecx, eax;
		//mov eax, GameBase;
		//lea eax, dword ptr ds:[GetPlayerName2];
		//call eax;
		call GetPlayerName2;
NOPLAYER:
		mov retaddr, eax;
	}

	return retaddr;
}

UINT GetPlayerHandle(int indexPlayer)
{
	UINT hPlayer;

	if (indexPlayer < 0)
		indexPlayer = 0;

	if (indexPlayer > 15)
		indexPlayer = 15;

	__asm
	{
		pushad;
		push indexPlayer;
		//mov eax, GameBase;
		//lea eax, dword ptr cs:[GetPlayerHandleFunction]
		//call eax;
		call GetPlayerHandleFunction;
		mov hPlayer,eax;
		pop eax;
		popad;
	}
	return hPlayer;
}

void __declspec(naked) GetPlayerColor()
{
	__asm
	{
		PUSHAD;
		mov ecx, playerId;
		call GetPlayerColorFunction;
		mov eax, dword ptr ds:[eax];
		mov playerColor, eax;
		POPAD;
		RETN;
	}
}

bool IsInGame()
{
	return ((*(DWORD*)IsInGame1) == 4 && (*(DWORD*)IsInGame2) == 4);
}

void PrintAndSend()
{
	CStringW s1, s2, text;
	s1 = UTF8ToUTF16(GetPlayerName(playerHandle));
	s2 = UTF8ToUTF16(name);
	if (IsInGame())
	{
		GetPlayerColor();
		text.Format(_T("|C%x%s|R 选中不可见单位 %s"),  playerColor,  s1, s2);
		TextOutInGame(text);
	}
	if (XWindowExist())
	{
		text.Format(_T("%s 选中不可见单位 %s"), s1, s2);
		int len = (wcslen(text) + 1) * 2;
		CopyDataSend(1, len, (void*)text.GetBuffer(len));
		text.ReleaseBuffer();
	}
}

void __declspec(naked) IsUnitInvisible()
{
	__asm
	{
		MOV ECX, DWORD PTR [ESP + 4];
		PUSH ESI;
		JMP IsUnitInvisibleEntry;
	}
}

bool IsInvisibleClick()
{
	result = 0;
	__try
	{
		playerHandle = GetPlayerHandle(playerId);
		__asm
		{
			PUSH playerHandle;
			PUSH 0;
			MOV ESI, IsUnitInvisibleOffset;
			MOV EAX, unit;
			CALL IsUnitInvisible;
			POP ECX;
			POP EDX;
			MOV result, EAX;
		}
	}
	__except(EXCEPTION_EXECUTE_HANDLER)
	{
		OutputDebugString(_T("Crashed.\r\n"));
		return 0;
	}
	return result;
}

void __declspec(naked) ClickHook()
{
	__asm
	{
		PUSHAD;
	}
	EnterCriticalSection(&ClickSection);
	__asm
	{
		MOV EAX, DWORD PTR DS:[ESP + 0x20];
		MOV name, EAX;
		MOV EAX, DWORD PTR DS:[ESP + 0x2C];
		MOV playerId, EAX;
		MOV EAX, DWORD PTR DS:[ESP + 0x38];
		MOV unit, EAX;
		MOV EAX, DWORD PTR DS:[ESP + 0x44];
		MOV address, EAX;
	}
	if (address == ClickHookOffset1 || address == ClickHookOffset2)
	{
		if (IsInvisibleClick())
		{
			PrintAndSend();
		}
	}
	LeaveCriticalSection(&ClickSection);
	__asm
	{
		POPAD;
		PUSH ClickHookOffset3;
		JMP ClickBack;
	}
}

void SetupClickDetect()
{
	PlantDetourJMP((BYTE*)ClickEntry, (BYTE*)ClickHook, 5);
}