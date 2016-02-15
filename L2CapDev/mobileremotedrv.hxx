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
#if ! defined (__l2capdev_HXX__)
#define __l2capdev_HXX__		1

#define L2CAPDEV_IOCTL_L2CAPConnect					1
#define L2CAPDEV_IOCTL_L2CAPAccept					2
#define L2CAPDEV_IOCTL_L2CAPListen					3
#define L2CAPDEV_IOCTL_L2CAPWrite					4
#define L2CAPDEV_IOCTL_L2CAPRead					5
#define L2CAPDEV_IOCTL_L2CAPCloseCID				6
#define L2CAPDEV_IOCTL_L2CAPClosePSM				7
#define L2CAPDEV_IOCTL_L2CAPPing					8
#define L2CAPDEV_IOCTL_L2CAPConfigReq				9

typedef union {
	struct {
		// in-parameters
		BT_ADDR				ba;
		unsigned short		usPSM;
		unsigned short		usInMTU;
		// out-parameters
		unsigned short		usCID;
		unsigned short		usOutMTU;
	} L2CAPConnect_p;

	struct  {
		// in-parameters
		unsigned short		usPSM;
		unsigned short		usInMTU;
	} L2CAPListen_p;

	struct  {
		// in-parameters
		unsigned short		usPSM;
		// out-parameters
		BT_ADDR				ba;
		unsigned short		usCID;
		unsigned short		usOutMTU;
	} L2CAPAccept_p;

	struct  {
		// in-parameters
		unsigned short		usCID;
		unsigned int		cBuffer;
		// out-parameters
		unsigned int		cRequired;
		unsigned char		*pBuffer;
	} L2CAPReadWrite_p;

	struct  {
		// in-parameters
		BT_ADDR				ba;
		unsigned int		cBufferIn;
		unsigned char		*pBufferIn;
		// in-out
		unsigned int		cBufferOut;
		// out-parameters
		unsigned char		*pBufferOut;
	} L2CAPPing_p;

	struct  {
		unsigned short usCID;
		unsigned short usInMTU;
		unsigned short usOutFlushTO;
		struct btFLOWSPEC *pOutFlow;
		int cOptNum;
		struct btCONFIGEXTENSION **ppExtendedOptions;
	} L2CAPConfigReq_p;

	struct  {
		// in-parameters
		unsigned short		us;
	} L2CAPClose_p;

} L2CAPDEVAPICALL;

#endif		/* __l2capdev_HXX__ */
