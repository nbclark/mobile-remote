//
// Copyright (c) Microsoft Corporation.  All rights reserved.
//
//
// Use of this source code is subject to the terms of the Microsoft end-user
// license agreement (EULA) under which you licensed this SOFTWARE PRODUCT.
// If you did not accept the terms of the EULA, you are not authorized to use
// this source code. For a copy of the EULA, please see the LICENSE.RTF on your
// install media.
//
/**
THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.


Abstract:
	Windows CE Bluetooth stack layer sample

**/
#include <windows.h>
#include <winerror.h>
#include <bthapi.h>
#include <bthutil.h>

#include <svsutil.hxx>
#include <winsock2.h>
#include <winsock.h>
#include <ws2bth.h>
#include <service.h>

#include "l2capapi.hxx"
#include "hid.h"

typedef int (FAR WINAPI *BthPerformInquiryProc)(
unsigned int	 LAP,
unsigned char	 length,
unsigned char	 num_responses,
unsigned int	 cBuffer,
unsigned int	 *pcDiscoveredDevices,
__out_ecount(cBuffer) BthInquiryResult *InquiryList);

typedef int (FAR WINAPI *BthRemoteNameQueryProc)(
BT_ADDR			*pba,
unsigned int	cBuffer,
unsigned int	*pcRequired,
__out_ecount(cBuffer) WCHAR			*szString);

int WINAPI WinMain(HINSTANCE hInstance, HINSTANCE, LPTSTR, int nCmdShow)
{
	int iErr;
	
	WORD wVersionRequested = 0x202;
	WSADATA m_data;

	if (0 == ::WSAStartup(wVersionRequested, &m_data))
	{
		SOCKADDR_BTH sab;
		BT_ADDR addr;
		USHORT usCid = 0, usMTU = 0;

		HMODULE hBTRD = LoadLibrary(L"btdrt.dll");
		BthRemoteNameQueryProc BthRemoteNameQuery = (BthRemoteNameQueryProc)GetProcAddress(hBTRD, L"BthRemoteNameQuery");
		BthPerformInquiryProc BthPerformInquiry = (BthPerformInquiryProc)GetProcAddress(hBTRD, L"BthPerformInquiry");

		WCHAR sz[_MAX_PATH];
		unsigned int cLen;
		--iErr = BthRemoteNameQuery (&addr, _MAX_PATH, &cLen, sz);

		BthInquiryResult ir[256];
		unsigned int cGot = 0;
		int iError = BthPerformInquiry (0x9e8b33, 0x10, 1, 256, &cGot, ir);
		if ((iError != ERROR_SUCCESS) || (! cGot))
		{
			// got nothing
			goto Exit;
		}

		for (int i = 0 ; i < (int)cGot ; ++i)
		{
			iError = BthRemoteNameQuery (&ir[i].ba, _MAX_PATH, &cLen, sz);
		}

		FreeLibrary(hBTRD);

		addr = ir[0].ba;

		GUID serviceID1 = { 0x00001101, 0x0000, 0x1000, { 0x80, 0x00, 0x00, 0x80, 0x5F, 0x9B, 0x34, 0xFB } };
		SOCKET s = socket (AF_BTH, SOCK_STREAM, BTHPROTO_RFCOMM);
		SOCKADDR_BTH sab1;
		memset (&sab1, 0, sizeof(sab));
		sab1.addressFamily = AF_BTH;
		sab1.serviceClassId = serviceID1;
		sab1.port = 0;
		sab1.btAddr = addr;

		if (0 != connect(s, (SOCKADDR*)&sab1, sizeof(sab1)))
		{
			goto Exit;
		}

		// here we can launch our listener


Exit :
		WSACleanup();
	}

	return 0;
}