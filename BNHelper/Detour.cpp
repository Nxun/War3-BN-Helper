#include "Global.h"
#include "Detours\detours.h"
#include "Offset.h"


int (WINAPI *pSend)(SOCKET socket, const char* buffer, int length, int flags) = NULL;

int (WINAPI *pWSARecv)(SOCKET socket, LPWSABUF lpBuffers, DWORD dwBufferCount,
    LPDWORD lpNumberOfBytesRecvd, LPDWORD lpFlags,LPWSAOVERLAPPED lpOverlapped,
    LPWSAOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine) = NULL;

BOOL (WINAPI *pGetQueuedCompletionStatus)(HANDLE CompletionPort, LPDWORD lpNumberOfBytes,
	PULONG_PTR lpCompletionKey, LPOVERLAPPED *lpOverlapped, DWORD dwMilliseconds) = NULL;


extern DWORD GameBase;

extern DWORD SayInRoomEntry;
extern DWORD SayInRoomBack;
extern DWORD SayInRoomSkip;

extern DWORD SayInGameEntry;
extern DWORD SayInGameBack;
extern DWORD SayInGameSkip;

extern DWORD DisplayInRoomEntry;
extern DWORD DisplayInRoomBack;

extern DWORD GameStateChangeEntry;
extern DWORD GameStateChangeBack;



char *SubmittedTextInRoom;
char *SubmittedTextInGame;
char *DisplayedTextInRoom;
DWORD GameState;


void PlantDetourJMP(BYTE* source, const BYTE* destination, const int length)
{
	BYTE* jump = (BYTE*) malloc(length + 5);

	DWORD oldProtection;
	VirtualProtect(source, length, PAGE_EXECUTE_READWRITE, &oldProtection);
	memcpy(jump, source, length);

	jump[length] = 0xE9;
	*(DWORD*)(jump + length) = (DWORD)((source + length) - (jump + length)) - 5;

	source[0] = 0xE9;
	*(DWORD*)(source + 1) = (DWORD)(destination - source) - 5;

	for(int i = 5; i < length; i++)
		source[i] = 0x90;

	VirtualProtect(source, length, oldProtection, &oldProtection);
}


void __declspec(naked) SayInRoomHook()
{
	__asm
	{
		MOV ESI, EAX;
		TEST ESI, ESI;
		JE roomSkip;
		PUSHAD;
		MOV SubmittedTextInRoom, EAX;
	}

	if (OnSayInRoom(SubmittedTextInRoom))
	{
		__asm
		{
			POPAD;
			JMP SayInRoomSkip;
		}
	}
	else
	{
		__asm
		{
			POPAD;
			JMP SayInRoomBack;
		}
	}
roomSkip:
	__asm
	{
		JMP SayInRoomSkip;
	}
}


void __declspec(naked) SayInGameHook()
{
	__asm
	{
		MOV ESI, EAX;
		TEST ESI, ESI;
		JE gameSkip;
		PUSHAD;
		MOV SubmittedTextInGame, EAX;
	}

	if (OnSayInGame(SubmittedTextInGame))
	{
		__asm
		{
			POPAD;
			JMP SayInGameSkip;
		}
	}
	else
	{
		__asm
		{
			POPAD;
			JMP SayInGameBack;
		}
	}
gameSkip:
	__asm
	{
		JMP SayInGameSkip;
	}
}


void __declspec(naked) DisplayInRoomHook()
{
	__asm
	{
		MOV DisplayedTextInRoom, EBX;
		PUSHAD;
	}
	
	OnDisplayInRoom(DisplayedTextInRoom);

	__asm
	{
		POPAD;
		TEST EBX, EBX;
		PUSH ESI;
		MOV ESI, ECX;
		JMP DisplayInRoomBack;
	}
}

void __declspec(naked) GameStateHook()
{
	__asm
	{
		MOV DWORD PTR DS:[ESI + 0x270], ECX;
		MOV GameState, ECX;
		PUSHAD;
	}
	OnGameStateChange(GameState);
	__asm
	{
		POPAD;
		JMP GameStateChangeBack;
	}
}



void SetupDetour()
{
	DetourRestoreAfterWith();

	// send function
	DetourTransactionBegin();
	DetourUpdateThread(GetCurrentThread());
	pSend = (int (WINAPI *)(SOCKET, const char*, int, int))DetourFindFunction("wsock32.dll", "send");
	DetourAttach(&(PVOID&)pSend, OnSend);
	if (DetourTransactionCommit() == NO_ERROR)
		OutputDebugString(TEXT("send() detoured successfully"));
	else
		OutputDebugString(TEXT("send() detoured failed"));

	// WSARecv function
	DetourTransactionBegin();
	DetourUpdateThread(GetCurrentThread());
	pWSARecv = (int (WINAPI *)(SOCKET, LPWSABUF, DWORD,
		LPDWORD, LPDWORD, LPWSAOVERLAPPED, LPWSAOVERLAPPED_COMPLETION_ROUTINE))
		DetourFindFunction("Ws2_32.dll", "WSARecv");
	DetourAttach(&(PVOID&)pWSARecv, OnWSARecv);
	if (DetourTransactionCommit() == NO_ERROR)
		OutputDebugString(TEXT("WSARecv() detoured successfully"));
	else
		OutputDebugString(TEXT("WSARecv() detoured failed"));


	DetourTransactionBegin();
	DetourUpdateThread(GetCurrentThread());
	pGetQueuedCompletionStatus = (BOOL (WINAPI *)(HANDLE, LPDWORD, PULONG_PTR, LPOVERLAPPED *, DWORD))DetourFindFunction("Kernel32.dll", "GetQueuedCompletionStatus");
	DetourAttach(&(PVOID&)pGetQueuedCompletionStatus, OnGetQueuedCompletionStatus);
	long l = DetourTransactionCommit();


	PlantDetourJMP((BYTE*)SayInRoomEntry, (BYTE*)SayInRoomHook, 6);
	PlantDetourJMP((BYTE*)SayInGameEntry, (BYTE*)SayInGameHook, 10);
	PlantDetourJMP((BYTE*)DisplayInRoomEntry, (BYTE*)DisplayInRoomHook, 5);
	PlantDetourJMP((BYTE*)GameStateChangeEntry, (BYTE*)GameStateHook, 6);
}

void UnloadDetour()
{
	DetourTransactionBegin();
	DetourUpdateThread(GetCurrentThread());
	DetourDetach(&(PVOID&)pSend, OnSend);
	DetourTransactionCommit();

	DetourTransactionBegin();
	DetourUpdateThread(GetCurrentThread());
	DetourDetach(&(PVOID&)pWSARecv, OnWSARecv);
	DetourTransactionCommit();
}