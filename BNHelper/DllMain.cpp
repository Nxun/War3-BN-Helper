#include "Global.h"
#include <Winver.h>
#include "Offset.h"

extern DWORD GameBase;
extern DWORD GameVersion;

extern CRITICAL_SECTION OverlapSection;
extern CRITICAL_SECTION ClickSection;


DWORD  GetFileVer( 
				  __in  LPTSTR FileName, 
				  __out         LPTSTR lpVersion, 
				  __in          DWORD nSize) 
{ 
	TCHAR  SubBlock[64]; 
	DWORD  InfoSize; 
	//首先获得版本信息资源的长度 
	InfoSize = GetFileVersionInfoSize(FileName,NULL); 
	//将版本信息资源读入缓冲区 
	if(InfoSize==0) return 0; 
	TCHAR *InfoBuf = new TCHAR[InfoSize]; 
	GetFileVersionInfo(FileName,0,InfoSize,InfoBuf); 
	//获得生成文件使用的代码页及文件版本 
	unsigned int  cbTranslate = 0; 
	struct LANGANDCODEPAGE { 
		WORD wLanguage; 
		WORD wCodePage; 
	} *lpTranslate; 
	VerQueryValue(InfoBuf, TEXT("\\VarFileInfo\\Translation"), 
		(LPVOID*)&lpTranslate,&cbTranslate); 
	// Read the file description for each language and code page. 
	wsprintf( SubBlock,  
		TEXT("\\StringFileInfo\\%04x%04x\\FileVersion"), 
		lpTranslate[0].wLanguage, 
		lpTranslate[0].wCodePage); 
	void *lpBuffer=NULL; 
	unsigned int dwBytes=0; 
	VerQueryValue(InfoBuf, SubBlock, &lpBuffer, &dwBytes);  
	lstrcpyn(lpVersion,(LPTSTR)lpBuffer,nSize); 
	delete[] InfoBuf; 
	return dwBytes; 
}

void GetWar3Ver()
{
	TCHAR tcTmp[MAX_PATH];
	GetModuleFileName((HMODULE)GameBase,tcTmp,MAX_PATH);
	TCHAR FileVer[64];
	GetFileVer(tcTmp,FileVer,64);
	if(lstrcmpi(FileVer,TEXT("1, 24, 1, 6374")) == 0)
	{
		GameVersion = V124B;
	}
	else if(lstrcmpi(FileVer,TEXT("1, 24, 4, 6387")) == 0)
	{
		GameVersion = V124E;
	}
 	else if(lstrcmpi(FileVer,TEXT("1, 26, 0, 6401")) ==0)
 	{
 		GameVersion = V126A;
 	}
	else
	{
		GameVersion = 0;
	}
}


BOOL WINAPI DllMain(HINSTANCE hinstDLL, DWORD fdwReason, LPVOID lpvReserved)
{
	if (fdwReason == DLL_PROCESS_ATTACH)
	{
		DisableThreadLibraryCalls(hinstDLL);

		GameBase = (DWORD)LoadLibrary(TEXT("game.dll"));
		FreeLibrary((HMODULE)GameBase);

		GetWar3Ver();
		if (!GameVersion)
			return 1;
		
		char ch[256];
		GetModuleFileNameA(hinstDLL, ch, 256);
		InitIpDatabase(ch);

		InitializeOffset();

		InitializeCriticalSection(&OverlapSection);
		InitializeCriticalSection(&ClickSection);

		SetupDetour();
		SetupClickDetect();
	}
	else if (fdwReason == DLL_PROCESS_DETACH)
	{
		UnloadDetour();
		DeleteCriticalSection(&OverlapSection);
		DeleteCriticalSection(&ClickSection);
	}
	return TRUE;
}