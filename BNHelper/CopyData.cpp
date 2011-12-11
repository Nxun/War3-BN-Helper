#include "Global.h"

#define XWindowName "War3 BN Helper"

bool XWindowExist()
{
	return FindWindow(NULL, _T(XWindowName));
}

void CopyDataSend(ULONG_PTR dwData, DWORD cbData, PVOID lpData)
{
	if (HWND handle = FindWindow(NULL, _T(XWindowName)))
	{
		COPYDATASTRUCT data;
		data.dwData = dwData;
		data.cbData = cbData;
		data.lpData = lpData;
		SendMessage(handle, WM_COPYDATA, 0, (LPARAM)&data);
	}
}