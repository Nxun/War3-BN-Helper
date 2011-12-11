#include "Global.h"
#include <map>

SOCKET SocketWithHost;
byte MyId;

extern DWORD GameState;
extern int BlockUntil;

extern int (WINAPI *pSend)(SOCKET socket, const char* buffer, int length, int flags);

extern int (WINAPI *pWSARecv)(SOCKET socket, LPWSABUF lpBuffers, DWORD dwBufferCount,
    LPDWORD lpNumberOfBytesRecvd, LPDWORD lpFlags,LPWSAOVERLAPPED lpOverlapped,
    LPWSAOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine);

extern BOOL (WINAPI *pGetQueuedCompletionStatus)(HANDLE CompletionPort, LPDWORD lpNumberOfBytes,
		PULONG_PTR lpCompletionKey, LPOVERLAPPED *lpOverlapped, DWORD dwMilliseconds);

struct ROOM Room;
struct ADVLIST AdvList;

CStringW NewPlayerName;
CStringW HostIp;

CRITICAL_SECTION OverlapSection;

std::map<void *, char *> OverlapList;

CStringW IpConvert(DWORD ip)
{
	in_addr addr;
	addr.S_un.S_addr = ip;
	return CStringW(inet_ntoa(addr));
}

void SendRoomData()
{
	if (!XWindowExist())
		return;
	CopyDataSend(2, sizeof(Room), (PVOID)&Room);
}

void SendAdvListData()
{
	if (!XWindowExist())
		return;
	CopyDataSend(3, sizeof(ADVLIST), (PVOID)&AdvList);
}


unsigned int decode_map_data(const char * encoded, char * decoded)
{
	unsigned int i = 0, j;
	unsigned int len = 0;
	char d;
	
	while (*encoded) {
		if (i % 8) {
			decoded[len++] = *encoded & ((d >> ++j) | ~1);
		} else {
			j = 0;
			d = *encoded;
		}		
		i++;
		encoded++;
	}
	
	return len;
}

int WINAPI OnWSARecv(SOCKET socket, LPWSABUF lpBuffers, DWORD dwBufferCount,
    LPDWORD lpNumberOfBytesRecvd, LPDWORD lpFlags,LPWSAOVERLAPPED lpOverlapped,
    LPWSAOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine)
{
	int nResult = pWSARecv(socket, lpBuffers, dwBufferCount, lpNumberOfBytesRecvd,
			lpFlags, lpOverlapped, lpCompletionRoutine);

	EnterCriticalSection(&OverlapSection);
	OverlapList[lpOverlapped] = lpBuffers->buf;
	LeaveCriticalSection(&OverlapSection);


	//CStringA s;
	//s.Format("WSARecv   nResult = %d   RecvdByte = %u   buf = %d   Overlapped = %d\r\n", nResult, *lpNumberOfBytesRecvd, lpBuffers->buf, lpOverlapped);
	//OutputDebugStringA(s);


	return nResult;
}

void ReceiveData(char *bp, int len);

BOOL WINAPI OnGetQueuedCompletionStatus(HANDLE CompletionPort, LPDWORD lpNumberOfBytes,
	PULONG_PTR lpCompletionKey, LPOVERLAPPED *lpOverlapped, DWORD dwMilliseconds)
{
	bool bResult = pGetQueuedCompletionStatus(CompletionPort, lpNumberOfBytes, lpCompletionKey, lpOverlapped, dwMilliseconds);

	int len = *lpNumberOfBytes;
	char *bp = NULL;
	EnterCriticalSection(&OverlapSection);
	if (OverlapList.count(*lpOverlapped))
	{
		bp = OverlapList[*lpOverlapped];
		OverlapList.erase(*lpOverlapped);
	}
	LeaveCriticalSection(&OverlapSection);

	if (len > 0 && bp)
	{
		ReceiveData(bp, len);
	}


	//CStringA s;
	//s.Format("GetComplete   Result = %d   Bytes = %d   Overlapped = %d\r\n", bResult, *lpNumberOfBytes, *lpOverlapped);
	//OutputDebugStringA(s);

	return bResult;
}

int WINAPI OnSend(SOCKET socket, const char* buffer, int length, int flags)
{
	const char *bp = buffer;
	if (GameState != 4) // 游戏外
	{
		for (int offset = 0; offset < length && *(BYTE *)(bp + offset) == 0xF7; offset += *(WORD *)(bp + offset + 2))
		{
			if (*(BYTE *)(bp + offset + 1) == 0x1E)
			{
				SocketWithHost = socket; // new added

				CStringA name((char *)(bp + offset + 19));
				NewPlayerName = UTF8ToUTF16(name);

				struct sockaddr_in addr;
				int len = sizeof(addr);
				getpeername(socket, (sockaddr *)&addr, &len);
				HostIp = inet_ntoa(addr.sin_addr);
			}
			else if (*(BYTE *)(bp + offset + 1) == 0x04)
			{
				struct sockaddr_in addr;
				int len = sizeof(addr);
				getsockname(socket, (sockaddr *)&addr, &len);
				HostIp = inet_ntoa(addr.sin_addr);

				Room.host = true;
				Room.count = *(BYTE *)(bp + offset + 6);
				BYTE id = *(BYTE *)(bp + *(WORD *)(bp + offset + 4) + offset + 6);
				DWORD ip = *(DWORD *)(bp + *(WORD *)(bp + offset + 4) + offset + 11);

				wcscpy(Room.player[id].ip, (LPCWSTR)IpConvert(ip));
				wcscpy(Room.player[id].location, (LPCWSTR)GetLocationByIp(Room.player[id].ip));
				wcscpy(Room.player[id].name, (LPCWSTR)NewPlayerName);

				for (int i = 0, j = offset + 7; i < Room.count && j < length; ++i, j+= 9)
				{
					Room.line[i] = *(LINE *)((bp) + j);
				}
				SendRoomData();
			}
			else if (*(BYTE *)(bp + offset + 1) == 0x09)
			{
				Room.host = true;
				Room.count = *(BYTE *)(bp + offset + 6);
				for (int i = 0, j = offset + 7; i < Room.count && j < length; ++i, j+= 9)
				{
					Room.line[i] = *(LINE *)((bp) + j);
				}
				SendRoomData();
			}
			else if (*(BYTE *)(bp + offset + 1) == 0x06)
			{
				BYTE id = *(BYTE *)(bp + offset + 8);
				CStringA name((char *)(bp + offset + 9));
				wcscpy(Room.player[id].name, (LPCWSTR)UTF8ToUTF16(name));
				DWORD ip = *(DWORD *)(bp + offset + name.GetLength() + 16);
				if (ip)
				{
					wcscpy(Room.player[id].ip, (LPCWSTR)IpConvert(ip));
					wcscpy(Room.player[id].location, (LPCWSTR)GetLocationByIp(Room.player[id].ip));
				}
				else
				{
					wcscpy(Room.player[id].ip, (LPCWSTR)HostIp);
					wcscpy(Room.player[id].location, (LPCWSTR)GetLocationByIp(Room.player[id].ip));
				}
			}
		}
	}
	else // 游戏中
	{
		if (GetTickCount() < BlockUntil)
		{
			if (*(BYTE *)(bp + 0) == 0xF7 && (*(BYTE *)(bp + 1) == 0x27 || *(BYTE *)(bp + 1) == 0x0C))
			{
				return length;
			}
		}
	}

	int nResult = pSend(socket, buffer, length, flags);

	return nResult;
}

void ReceiveData(char *bp, int len)
{
	if (GameState != 4)
	{
		for (int offset = 0; offset < len && *(BYTE *)(bp + offset) == 0xFF; offset += *(WORD *)(bp + offset + 2))
		{
			if (*(BYTE *)(bp + offset + 1) == 0x09)
			{
				AdvList.count = *(DWORD *)(bp + offset + 4);
				for (int i = 0, j = 8; i < AdvList.count && j < *(WORD *)(bp + offset + 2); ++i)
				{
					DWORD ip = *(DWORD *)(bp + offset + j + 12);
					wcscpy(AdvList.adv[i].ip, (LPCWSTR)IpConvert(ip));
					wcscpy(AdvList.adv[i].location, (LPCWSTR)GetLocationByIp(AdvList.adv[i].ip));
					CStringA s((char *)(bp + offset + j + 32));
					wcscpy(AdvList.adv[i].title, (LPCWSTR)UTF8ToUTF16(s));
					j += s.GetLength() + 32 + 2 + 9;
					s = (char *)(bp + offset + j);
					char mapInfo[100];
					decode_map_data((char *)(bp + offset + j), mapInfo);
					CStringA map = (char *)(mapInfo + 13);
					wcscpy(AdvList.adv[i].map, CStringW(map));
					wcscpy(AdvList.adv[i].creator, (LPCWSTR)UTF8ToUTF16(mapInfo + 14 + map.GetLength()));
					j += s.GetLength() + 1;
				}
				SendAdvListData();
			}
		}

		int count = 0;
		for (int offset = 0; offset < len && *(BYTE *)(bp + offset) == 0xF7; offset += *(WORD *)(bp + offset + 2))
		{
			if (*(BYTE *)(bp + offset + 1) == 0x1E)
			{
				CStringA name((char *)(bp + offset + 19));
				NewPlayerName = UTF8ToUTF16(name);
			}
			else if (*(BYTE *)(bp + offset + 1) == 0x04)
			{
				Room.host = false;
				Room.count = *(BYTE *)(bp + offset + 6);
				BYTE id = *(BYTE *)(bp + *(WORD *)(bp + offset + 4) + offset + 6);
				MyId = id; // new added
				DWORD ip = *(DWORD *)(bp + *(WORD *)(bp + offset + 4) + offset + 11);

				wcscpy(Room.player[id].ip, (LPCWSTR)IpConvert(ip));
				wcscpy(Room.player[id].location, (LPCWSTR)GetLocationByIp(Room.player[id].ip));
				wcscpy(Room.player[id].name, (LPCWSTR)NewPlayerName);

				for (int i = 0, j = offset + 7; i < Room.count && j < len; ++i, j+= 9)
				{
					Room.line[i] = *(LINE *)((bp) + j);
				}
				SendRoomData();
			}
			if (*(BYTE *)(bp + offset + 1) == 0x09)
			{
				Room.host = false;
				Room.count = *(BYTE *)(bp + offset + 6);
				for (int i = 0, j = offset + 7; i < Room.count && j < len; ++i, j+= 9)
				{
					Room.line[i] = *(LINE *)((bp) + j);
				}
				SendRoomData();
			}
			else if (*(BYTE *)(bp + offset + 1) == 0x06)
			{
				BYTE id = *(BYTE *)(bp + offset + 8);
				CStringA name((char *)(bp + offset + 9));
				wcscpy(Room.player[id].name, (LPCWSTR)UTF8ToUTF16(name));
				DWORD ip = *(DWORD *)(bp + offset + name.GetLength() + 16);
				if (ip)
				{
					wcscpy(Room.player[id].ip, (LPCWSTR)IpConvert(ip));
					wcscpy(Room.player[id].location, (LPCWSTR)GetLocationByIp(Room.player[id].ip));
				}
				else
				{
					wcscpy(Room.player[id].ip, (LPCWSTR)HostIp);
					wcscpy(Room.player[id].location, (LPCWSTR)GetLocationByIp(Room.player[id].ip));
				}
			}
		}
		
	}
}