#include "stdafx.h"
#include <windows.h>
#include <ce_setup.h>
#include <Tlhelp32.h>
#include "utils.h"

#define TH32CS_SNAPNOHEAPS   0x40000000   // optimization for text shell to not snapshot heaps 

DWORD FindProcess(TCHAR *szName) 
{ 
    HINSTANCE         hProcessSnap   = NULL; 
    PROCESSENTRY32    pe32           = {0}; 
    DWORD             dwTaskCount    = 0; 

    hProcessSnap = (HINSTANCE)CreateToolhelp32Snapshot(TH32CS_SNAPPROCESS | TH32CS_SNAPNOHEAPS, 0); 
    if (hProcessSnap == (HANDLE)-1) return 0; 

    dwTaskCount = 0; 
    pe32.dwSize = sizeof(PROCESSENTRY32);   // must be filled out before use 
    if (Process32First(hProcessSnap, &pe32)) { 
        do { 
            if (_wcsicmp(pe32.szExeFile,szName)==0) 
            { 
                CloseToolhelp32Snapshot(hProcessSnap); 
                return pe32.th32ProcessID; 
            } 
        } 
        while (Process32Next(hProcessSnap, &pe32)); 
    } 
    CloseToolhelp32Snapshot(hProcessSnap); 
    return 0; 
} 


void KillProcess(TCHAR *szName) 
{ 
    DWORD dwPID = FindProcess(szName); 
    HANDLE hProcess; 

    if (dwPID) 
    { 
        hProcess = OpenProcess(PROCESS_ALL_ACCESS,false,dwPID); 
        TerminateProcess(hProcess,0); 
        CloseHandle(hProcess); 
    } 
}

codeINSTALL_INIT Install_Init(
                              HWND        hwndParent,
                              BOOL        fFirstCall, 
                              BOOL        fPreviouslyInstalled,
                              LPCTSTR     pszInstallDir
                              )
{
    return codeINSTALL_INIT_CONTINUE;
}

codeINSTALL_EXIT Install_Exit(
                              HWND    hwndParent,
                              LPCTSTR pszInstallDir,
                              WORD    cFailedDirs,
                              WORD    cFailedFiles,
                              WORD    cFailedRegKeys,
                              WORD    cFailedRegVals,
                              WORD    cFailedShortcuts
                              )
{
    SetStringToRegistry(REG_MOBILEREMOTE_DLL, L"MobileRemoteDRV.dll");
    SetToRegistry(REG_MOBILEREMOTE_INDEX, 1);
    SetToRegistry(REG_MOBILEREMOTE_ORDER, 0);
    SetStringToRegistry(REG_MOBILEREMOTE_PREFIX, L"L2C");

    return codeINSTALL_EXIT_DONE;
}



codeUNINSTALL_INIT Uninstall_Init(
                                  HWND        hwndParent,
                                  LPCTSTR     pszInstallDir
                                  )
{
    // Kill process here
    KillProcess(TEXT("MobileRemote.exe"));
    return codeUNINSTALL_INIT_CONTINUE;
}



codeUNINSTALL_EXIT Uninstall_Exit(
                                  HWND    hwndParent
                                  )
{
    return codeUNINSTALL_EXIT_DONE;
}

