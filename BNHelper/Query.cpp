#include "Global.h"

extern ROOM Room;
extern SOCKET SocketWithHost;
extern byte MyId;

typedef struct QueryItem
{
	QueryItem *next;
	int id;
} QueryItem;

QueryItem *QueryHead1, *QueryTail1;
QueryItem *QueryHead2, *QueryTail2;

void AddQuery1(int i)
{

}

void Statsdota(int i)
{
	char data[50] = "\xf7\x28\x13\x00\x01\x01\x09\x10";
	sprintf(data + 8, "@statsdota %ls", Room.player[Room.line[i - 1].playerId].name);
	byte len = strlen(data + 8) + 9;
	data[6] = MyId;
	data[2] = len;
	send(SocketWithHost, data, len, 0);
}