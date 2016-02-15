//
// Copyright (c) Microsoft Corporation.  All rights reserved.
//
//
// Use of this sample source code is subject to the terms of the Microsoft
// license agreement under which you licensed this sample source code. If
// you did not accept the terms of the license agreement, you are not
// authorized to use this sample source code. For the terms of the license,
// please see the license agreement between you and Microsoft or, if applicable,
// see the LICENSE.RTF on your install media or the root of your tools installation.
// THE SAMPLE SOURCE CODE IS PROVIDED "AS IS", WITH NO WARRANTIES.
//
/**
THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND,
EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.


Abstract:
	Windows CE Bluetooth stack layer sample

**/
#if ! defined (__l2capapi_HXX__)
#define __l2capapi_HXX__		1

#include <ws2bth.h>

#if defined (__cplusplus)
extern "C" {
#endif

//
//	L2CAP Apis
//
int L2CAPConnect
(
BT_ADDR			*pba,		// =>
unsigned short	usPSM,		// =>
unsigned short	usInMTU,	// =>
unsigned short	*pCID,		// <=
unsigned short	*pusOutMTU	// <=
);

int L2CAPListen
(
unsigned short	usPSM,		// =>
unsigned short	usInMTU		// =>
);

int L2CAPAccept
(
unsigned short	usPSM,		// =>
BT_ADDR			*pba,		// <=
unsigned short	*pusCID,	// <=
unsigned short	*pusOutMTU	// <=
);

int L2CAPWrite
(
unsigned short	cid,
unsigned int	cBuffer,
unsigned char	*pBuffer
);

int L2CAPRead
(
unsigned short	cid,
unsigned int	cBuffer,
unsigned int	*pRequired,
unsigned char	*pBuffer
);

int L2CAPCloseCID
(
unsigned short cid
);

int L2CAPClosePSM
(
unsigned short psm
);

int L2CAPPing
(
BT_ADDR	*pbt,
unsigned int cBufferIn,
unsigned char *pBufferIn,
unsigned int *pcBufferOut,
unsigned char *pBufferOut
);

int L2CAPLoad (void);

int L2CAPUnload (void);

#if defined (__cplusplus)
};			// __cplusplus
#endif

#endif		// __l2capapi_HXX__

