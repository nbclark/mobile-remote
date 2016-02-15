#include "stdafx.h"
#include "utils.h"
#include <nled.h>

void LedOn(int id)
{ 
    NLED_SETTINGS_INFO settings; 
    settings.LedNum= id; 
    settings.OffOnBlink= 1; 
    NLedSetDevice(NLED_SETTINGS_INFO_ID, &settings); 
}

void LedOff(int id)
{
    NLED_SETTINGS_INFO settings; 
    settings.LedNum= id; 
    settings.OffOnBlink= 0; 
    NLedSetDevice(NLED_SETTINGS_INFO_ID, &settings); 
}

BOOL LaunchProgram(LPCTSTR lpFile, LPCTSTR lpParameters)
{
    SHELLEXECUTEINFO shInfo;

    shInfo.cbSize = sizeof(SHELLEXECUTEINFO);
    shInfo.dwHotKey = 0;
    shInfo.fMask = 0;
    shInfo.hIcon = NULL;
    shInfo.hInstApp = NULL;
    shInfo.hProcess = NULL;
    shInfo.lpDirectory = NULL;
    shInfo.lpIDList = NULL;
    shInfo.lpParameters = lpParameters;
    shInfo.lpVerb = NULL;
    shInfo.nShow = SW_SHOW;
    shInfo.lpFile = lpFile;

    return ShellExecuteEx(&shInfo);
}

HRESULT GetFromRegistry(HKEY rootKey, TCHAR* szRegistryKey, TCHAR* szRegValueName, WCHAR wzValue[MAX_PATH])
{
    HKEY    hKey;
    DWORD    dwSize;
    DWORD    dwType;
    HRESULT    hRes;

    hRes = RegOpenKeyExW(rootKey,szRegistryKey,0,KEY_READ,&hKey);
    if (hRes == ERROR_SUCCESS)
    {
        if (wzValue)
        {
            if (hRes == ERROR_SUCCESS)
            {
                dwSize = sizeof(WCHAR) * MAX_PATH;
                dwType = REG_SZ;

                hRes = RegQueryValueExW(hKey,szRegValueName,0,&dwType,(LPBYTE)wzValue,&dwSize);

                if (hRes != ERROR_SUCCESS)
                {
                    wzValue[0] = 0;
                }
            }
        }
        RegCloseKey(hKey);
    }

    return hRes;
}

HRESULT GetFromRegistry(TCHAR* szRegValueName, WCHAR wzValue[MAX_PATH])
{
    return GetFromRegistry(HKEY_LOCAL_MACHINE, REG_MOBILEREMOTE_REGKEY, szRegValueName, wzValue);
}

HRESULT GetFromRegistry(HKEY rootKey, TCHAR* szRegistryKey, TCHAR* szRegValueName, DWORD *pdwValue)
{
    HKEY    hKey;
    DWORD    dwSize;
    DWORD    dwType;
    HRESULT    hRes;

    if (pdwValue)
    {
        *pdwValue = 0;
    }

    hRes = RegOpenKeyExW(rootKey,szRegistryKey,0,KEY_READ,&hKey);
    if (hRes == ERROR_SUCCESS)
    {
        if (pdwValue)
        {
            dwSize = sizeof(DWORD);

            if (hRes == ERROR_SUCCESS)
            {
                hRes = RegQueryValueExW(hKey,szRegValueName,0,&dwType,(LPBYTE)pdwValue,&dwSize);
            }
        }
        RegCloseKey(hKey);
    }

    return hRes;
}

HRESULT GetFromRegistry(TCHAR* szRegValueName, DWORD *pdwValue)
{
    return GetFromRegistry(HKEY_LOCAL_MACHINE, REG_MOBILEREMOTE_REGKEY, szRegValueName, pdwValue);
}

HRESULT SetToRegistry(TCHAR* szRegValueName, DWORD dwValue)
{
    HKEY    hKey;
    DWORD    dwDisp;
    HRESULT    hRes;

    TCHAR    *szRegistryKey;

    szRegistryKey    = REG_MOBILEREMOTE_REGKEY;
    
    hRes = RegCreateKeyExW(HKEY_LOCAL_MACHINE, szRegistryKey, 0, NULL, 0, KEY_ALL_ACCESS, NULL, &hKey, &dwDisp);
    if (hRes != ERROR_SUCCESS)
    {
        return hRes;
    }
    
    hRes = RegSetValueExW(hKey,szRegValueName,0,REG_DWORD,(LPBYTE)&dwValue,sizeof(DWORD));

    RegCloseKey(hKey);                    
    
    return hRes;
}

HRESULT SetStringToRegistry(TCHAR* szRegValueName, WCHAR* wzValue)
{
    HKEY    hKey;
    DWORD    dwDisp;
    HRESULT    hRes;

    TCHAR    *szRegistryKey;

    szRegistryKey    = REG_MOBILEREMOTE_REGKEY;
    
    hRes = RegCreateKeyExW(HKEY_LOCAL_MACHINE, szRegistryKey, 0, NULL, 0, KEY_ALL_ACCESS, NULL, &hKey, &dwDisp);
    if (hRes != ERROR_SUCCESS)
    {
        return hRes;
    }

    int cCharacters =  wcslen(wzValue)+1;

    if (!wzValue)
    {
        wzValue = L"";
    }
    hRes = RegSetValueExW(hKey,szRegValueName,0,REG_SZ,(LPBYTE)wzValue, cCharacters * sizeof(WCHAR));

    RegCloseKey(hKey);                    
    
    return hRes;
}

WCHAR* TrimEndSpaces(WCHAR* szEXEPath2)
{
    if (szEXEPath2)
    {
        int iLen = wcslen(szEXEPath2) - 1;

        while (iLen >=0 && szEXEPath2[iLen] == L' ')
        {
            szEXEPath2[iLen] = L'\0';
        }
    }
    return szEXEPath2;
}

WCHAR* TrimSpaces(WCHAR* szEXEPath2)
{
    if (szEXEPath2)
    {
        if (szEXEPath2[0] == L'"')
        {
            int len = wcslen(szEXEPath2);
            szEXEPath2[len-1] = L'\0';
            szEXEPath2++;
        }
        else
        {
            WCHAR* szSpace = wcschr(szEXEPath2, L' ');

            if (szSpace)
            {
                szSpace[0] = L'\0';
            }
        }
    }
    return szEXEPath2;
}