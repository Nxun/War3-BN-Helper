#pragma once;
#include <string>

const int INDEX_LENGTH = 7;// 一个索引包含4字节的起始IP和3字节的IP记录偏移，共7字节
const int IP_LENGTH = 4;
const int OFFSET_LENGTH = 3;

enum {
	REDIRECT_MODE_1 = 0x01,    // 重定向模式1 偏移量后无地区名
	REDIRECT_MODE_2 = 0x02,    // 重定向模式2 偏移量后有地区名
};

class CIpFinder
{
public:
	CIpFinder();
	CIpFinder(const char* pszFileName);
	~CIpFinder();

	// 获取ip国家名、地区名
	void GetAddressByIp(unsigned long ipValue, std::string& strCountry, std::string& strLocation) const;
	void GetAddressByIp(const char* pszIp, std::string& strCountry, std::string& strLocation) const;
	void GetAddressByOffset(unsigned long ulOffset, std::string& strCountry, std::string& strLocation) const;

	unsigned long GetString(std::string& str, unsigned long indexStart) const;
	unsigned long GetValue3(unsigned long indexStart) const;
	unsigned long GetValue4(unsigned long indexStart) const;

	// 转换
	unsigned long IpString2IpValue(const char *pszIp) const;
	void IpValue2IpString(unsigned long ipValue, char* pszIpAddress, int nMaxCount) const;
	bool IsRightIpString(const char* pszIp) const;

	// 输出数据
	unsigned long OutputData(const char* pszFileName, unsigned long ulIndexStart = 0, unsigned long ulIndexEnd = 0) const;
	unsigned long OutputDataByIp(const char* pszFileName, unsigned long ipValueStart, unsigned long ipValueEnd) const;
	unsigned long OutputDataByIp(const char* pszFileName, const char* pszStartIp, const char* pszEndIp) const;

	unsigned long SearchIp(unsigned long ipValue, unsigned long indexStart = 0, unsigned long indexEnd = 0) const;
	unsigned long SearchIp(const char* pszIp, unsigned long indexStart = 0, unsigned long indexEnd = 0) const;

	bool Open(const char* pszFileName);
private:
	FILE *m_fpIpDataFile; // IP数据库文件
	unsigned long m_indexStart; // 起始索引偏移
	unsigned long m_indexEnd;  // 结束索引偏移
}; 