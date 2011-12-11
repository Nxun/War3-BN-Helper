#include "Global.h"

extern DWORD GameState;

void OnGameStateChange(DWORD gameState)
{
	//if (gameState == 4)
	//{
	//	StartClickDetect();
	//}
	if (!XWindowExist())
		return;
	CopyDataSend(4, sizeof(DWORD), (PVOID)&GameState);
}