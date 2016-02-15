#define REG_MOBILEREMOTE_REGKEY           TEXT("Drivers\\BuiltIn\\L2Cap")
#define REG_MOBILEREMOTE_DLL              TEXT("Dll")
#define REG_MOBILEREMOTE_INDEX            TEXT("Index")
#define REG_MOBILEREMOTE_ORDER            TEXT("Order")
#define REG_MOBILEREMOTE_PREFIX           TEXT("Prefix")

void LedOn(int id);
void LedOff(int id);
BOOL LaunchProgram(LPCTSTR lpFile, LPCTSTR lpParameters);
HRESULT GetFromRegistry(HKEY rootKey, TCHAR* szRegistryKey, TCHAR* szRegValueName, WCHAR wzValue[MAX_PATH]);
HRESULT GetFromRegistry(TCHAR* szRegValueName, WCHAR wzValue[MAX_PATH]);
HRESULT GetFromRegistry(HKEY rootKey, TCHAR* szRegistryKey, TCHAR* szRegValueName, DWORD *pdwValue);
HRESULT GetFromRegistry(TCHAR* szRegValueName, DWORD *pdwValue);
HRESULT SetToRegistry(TCHAR* szRegValueName, DWORD dwValue);
HRESULT SetStringToRegistry(TCHAR* szRegValueName, WCHAR* wzValue);
HICON GetApplicationIcon(WCHAR* szFilteredPath, bool bSmall, WCHAR* wzExeFile, WCHAR* wzClassName);
WCHAR* TrimEndSpaces(WCHAR* szEXEPath2);