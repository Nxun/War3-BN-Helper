#include "Global.h"
#include <string.h>

#include "CIpFinder.h"


CIpFinder IpFinder;
bool IpDataExist;

void InitIpDatabase(char *path)
{
	std::string str(path);
	str = str.substr(0, str.find_last_of('\\') + 1).append("ip.dat");
	IpDataExist = IpFinder.Open(str.c_str());
}

CStringW GetLocationByIp(LPCWSTR ip)
{
	if (!IpDataExist)
	{
		return _T("");
	}
	std::string strCountry, strLocation;
	CStringA astr(ip);
	IpFinder.GetAddressByIp((LPCSTR)astr, strCountry, strLocation);
	CStringW wstr((strCountry + " " + strLocation).c_str());
	return wstr;
}