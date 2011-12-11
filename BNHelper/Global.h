#pragma once

#include <WinSock2.h>
#include <Windows.h>
#include <atlstr.h>

// Location.cpp
void InitIpDatabase(char *path);
CStringW GetLocationByIp(LPCWSTR ip);

// CopdyData.cpp
bool XWindowExist();
void CopyDataSend(ULONG_PTR dwData, DWORD cbData, PVOID lpData);

// SendAndReceive.cpp
int WINAPI OnSend(SOCKET socket, const char* buffer, int length, int flags);
int WINAPI OnWSARecv(SOCKET socket, LPWSABUF lpBuffers, DWORD dwBufferCount,
    LPDWORD lpNumberOfBytesRecvd, LPDWORD lpFlags,LPWSAOVERLAPPED lpOverlapped,
    LPWSAOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine);

BOOL WINAPI OnGetQueuedCompletionStatus(HANDLE CompletionPort, LPDWORD lpNumberOfBytes,
	PULONG_PTR lpCompletionKey, LPOVERLAPPED *lpOverlapped, DWORD dwMilliseconds);


// TextOut.cpp
CStringA UTF16ToUTF8(const CStringW& utf16);
CStringW UTF8ToUTF16(const CStringA& utf8);
void TextOutInRoom(const CStringW& text);
void TextOutInGame(const CStringW& text);
void TextOutInGame(const CStringW& text, float dur);

void Statsdota(int i);

// Detoured functions
void PlantDetourJMP(BYTE* source, const BYTE* destination, const int length);

bool OnSayInRoom(const char *text);
bool OnSayInGame(const char *text);
void OnDisplayInRoom(const char *text);
void OnGameStateChange(DWORD gameState);

void SetupDetour();
void UnloadDetour();

void SetupClickDetect();

void InitializeOffset();

// structures
struct LINE
{
	BYTE playerId;
	BYTE mapProcess;
	BYTE emptyOrClosed;
	BYTE humanOrAi;
	BYTE team;
	BYTE color;
	BYTE race;
	BYTE aiLevel;
	BYTE hp;
};

struct RECORD
{
	int win;
	int lose;
	int kill;
	int dead;
	int assist;
};

struct PLAYER
{
	RECORD record;
	wchar_t name[20];
	wchar_t ip[20];
	wchar_t location[40];
};

struct ROOM
{
	BYTE count;
	BOOL host;
	LINE line[13];
	PLAYER player[13];
};


struct ADV
{
	wchar_t title[35];
	wchar_t creator[20];
	wchar_t ip[20];
	wchar_t location[40];
	wchar_t map[40];
};

struct ADVLIST
{
	int count;
	ADV adv[20];
};