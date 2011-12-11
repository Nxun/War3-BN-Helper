#include "Global.h"
#include "Offset.h"


extern DWORD TextInRoomFunction;
extern DWORD TextInRoomOffset1;
extern DWORD TextInRoomOffset2;
extern DWORD TextInRoomOffset3;

extern DWORD TextInGameFunction;
extern DWORD TextInGameWriteData;

extern DWORD GameBase;
extern bool ToSend;

const DWORD ColorWhite = 0xFFFFFFFF;


CStringA UTF16ToUTF8(const CStringW& utf16)
{
	CStringA utf8;
	int len = WideCharToMultiByte(CP_UTF8, 0, utf16, -1, NULL, 0, 0, 0);
	if (len > 1)
	{ 
		char *ptr = utf8.GetBuffer(len - 1);
		if (ptr)
		{
			WideCharToMultiByte(CP_UTF8, 0, utf16, -1, ptr, len, 0, 0);
		}
		utf8.ReleaseBuffer();
	}
	return utf8;
}


CStringW UTF8ToUTF16(const CStringA& utf8)
{
   CStringW utf16;
   int len = MultiByteToWideChar(CP_UTF8, 0, utf8, -1, NULL, 0);
   if (len > 1)
   {
      wchar_t *ptr = utf16.GetBuffer(len - 1);
      if (ptr)
	  {
		  MultiByteToWideChar(CP_UTF8, 0, utf8, -1, ptr, len);
	  }
      utf16.ReleaseBuffer();
   }
   return utf16;
}



void TextOutInRoom(const CStringW& text)
{
	DWORD func = TextInRoomFunction;
	DWORD p = *(DWORD*)(*(DWORD*)(*(DWORD*)(GameBase + TextInRoomOffset1) + TextInRoomOffset2) + TextInRoomOffset3);
	LPCSTR str = UTF16ToUTF8(text);
	ToSend = false; // 这条文字不发送给前端

	__asm
	{
		MOV ECX, str;
		PUSH ECX;
		MOV ECX, p;
		CALL func;
	}
}

void TextOutInGame(const CStringW& text, float duration)
{
	DWORD func = TextInGameFunction;
	DWORD writeData = TextInGameWriteData;
	LPCSTR str = UTF16ToUTF8(text);

	__asm
	{
		PUSH 0;
		PUSH duration; //0x41A00000; // 持续时间
		LEA EAX, ColorWhite;
		PUSH EAX;
		MOV EAX, str;
		PUSH EAX;
		MOV EDI, writeData;
		MOV EDI, DWORD PTR DS:[EDI];
		MOV ECX, DWORD PTR DS:[EDI + 0x3E8]; // 0x3EC 为对话型字样 0x3E8 为系统字样
		CALL func;
	}
}

void TextOutInGame(const CStringW& text)
{
	TextOutInGame(text, 10);
}