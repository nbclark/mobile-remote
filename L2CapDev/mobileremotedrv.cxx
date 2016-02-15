#include <windows.h>

#include <svsutil.hxx>

#include <bt_debug.h>
#include <bt_os.h>
#include <bt_buffer.h>
#include <bt_ddi.h>
#include <bt_api.h>

#if defined (UNDER_CE)
#include <pkfuncs.h>
#endif

#include <bt_tdbg.h>

#include "../l2capapi/mobileremoteapi.hxx"
#include "mobileremotedrv.hxx"

#define DEBUG_LAYER_TRACE			0x00000040

DECLARE_DEBUG_VARS();

#define CALL_L2CAP_LINK_SETUP		0x11
#define CALL_L2CAP_ACCEPT			0x12
#define CALL_L2CAP_DATA_WRITE		0x13
#define CALL_L2CAP_DATA_READ		0x14
#define CALL_L2CAP_DISCONNECT		0x15
#define CALL_L2CAP_PING				0x16
#define CALL_L2CAP_CONFIGREQ        0x17

#define	NONE			0x00
#define CONNECTED		0x01
#define CONFIG_REQ_DONE 0x02
#define CONFIG_IND_DONE 0x04
#define UP              0x07
#define LINK_ERROR		0x80

struct Link {
	Link			*pNext;
	BD_ADDR			b;
	unsigned short	psm;
	unsigned short	cid;

	unsigned int	fStage;

	unsigned int	fIncoming : 1;

	unsigned short	inMTU;
	unsigned short	outMTU;

	HANDLE			hProcOwner;
};

struct Port {
	Port			*pNext;
	unsigned short	psm;
	unsigned short	mtu;

	HANDLE			hProcOwner;
};

struct SCall {
	SCall			*pNext;
	Link			*pLink;
	HANDLE			hProcOwner;

	HANDLE			hEvent;

	int				iResult;

	unsigned short	psm;

	unsigned int	fWhat			: 8;
	unsigned int	fComplete		: 1;
	unsigned int	fAutoClean		: 1;

	BD_BUFFER		*pBuffer;
};

int l2capdev_CloseDriverInstance (void);

static int l2capdev_ConfigInd (void *pUserContext, unsigned char id, unsigned short cid, unsigned short usOutMTU, unsigned short usInFlushTO, struct btFLOWSPEC *pInFlow, int cOptNum, struct btCONFIGEXTENSION **pExtendedOptions);
static int l2capdev_ConnectInd (void *pUserContext, BD_ADDR *pba, unsigned short cid, unsigned char id, unsigned short psm);
static int l2capdev_DataUpInd (void *pUserContext, unsigned short cid, BD_BUFFER *pBuffer);
static int l2capdev_DisconnectInd (void *pUserContext, unsigned short cid, int iError);
static int l2capdev_lStackEvent (void *pUserContext, int iEvent, void *pEventContext);

static int l2capdev_lCallAborted (void *pCallContext, int iError);
static int l2capdev_ConfigReq_Out (void *pCallContext, unsigned short usResult, unsigned short usInMTU, unsigned short usOutFlushTO, struct btFLOWSPEC *pOutFlow, int cOptNum, struct btCONFIGEXTENSION **pExtendedOptions);
static int l2capdev_ConfigResponse_Out (void *pCallContext, unsigned short result);
static int l2capdev_ConnectReq_Out (void *pCallContext, unsigned short cid, unsigned short result, unsigned short status);
static int l2capdev_ConnectResponse_Out (void *pCallContext, unsigned short result);
static int l2capdev_DataDown_Out (void *pCallContext, unsigned short result);
static int l2capdev_Ping_Out (void *pCallContext, BD_ADDR *pba, unsigned char *pOutBuffer, unsigned short size);
static int l2capdev_Disconnect_Out (void *pCallContext, unsigned short result);

class L2CAPDEV : public SVSSynch, public SVSRefObj {
public:
	Link			*pLinks;
	Port			*pPorts;
	SCall			*pCalls;

	unsigned int	fIsRunning : 1;
	unsigned int	fConnected : 1;

	HANDLE			hL2CAP;
	L2CAP_INTERFACE	l2cap_if;

	int				cHeaders;
	int				cTrailers;

	FixedMemDescr	*pfmdLinks;
	FixedMemDescr	*pfmdPorts;
	FixedMemDescr	*pfmdCalls;

	L2CAPDEV (void) {
		IFDBG(DebugOut (DEBUG_LAYER_TRACE, L"Shell: new L2CAPDEV\n"));
		pLinks = NULL;
		pPorts = NULL;
		pCalls = NULL;

		cHeaders = 0;
		cTrailers = 0;

		fIsRunning = FALSE;
		fConnected = FALSE;

		hL2CAP = NULL;

		memset (&l2cap_if, 0, sizeof(l2cap_if));

		pfmdLinks = pfmdPorts = pfmdCalls = NULL;

		if (! (pfmdLinks = svsutil_AllocFixedMemDescr (sizeof(Link), 10)))
			return;

		if (! (pfmdPorts = svsutil_AllocFixedMemDescr (sizeof(Port), 10)))
			return;

		if (! (pfmdCalls = svsutil_AllocFixedMemDescr (sizeof(SCall), 10)))
			return;

		L2CAP_EVENT_INDICATION lei;
		memset (&lei, 0, sizeof(lei));

		lei.l2ca_ConfigInd = l2capdev_ConfigInd;
		lei.l2ca_ConnectInd = l2capdev_ConnectInd;
		lei.l2ca_DataUpInd = l2capdev_DataUpInd;
		lei.l2ca_DisconnectInd = l2capdev_DisconnectInd;
		lei.l2ca_StackEvent = l2capdev_lStackEvent;

		L2CAP_CALLBACKS lc;
		memset (&lc, 0, sizeof(lc));

		lc.l2ca_CallAborted = l2capdev_lCallAborted;
		lc.l2ca_ConfigReq_Out = l2capdev_ConfigReq_Out;
		lc.l2ca_ConfigResponse_Out = l2capdev_ConfigResponse_Out;
		lc.l2ca_ConnectReq_Out = l2capdev_ConnectReq_Out;
		lc.l2ca_ConnectResponse_Out = l2capdev_ConnectResponse_Out;
		lc.l2ca_DataDown_Out = l2capdev_DataDown_Out;
		lc.l2ca_Ping_Out = l2capdev_Ping_Out;
		lc.l2ca_Disconnect_Out = l2capdev_Disconnect_Out;

		if (ERROR_SUCCESS != L2CAP_EstablishDeviceContext (this, L2CAP_PSM_MULTIPLE, &lei, &lc, &l2cap_if, &cHeaders, &cTrailers, &hL2CAP))
			return;

		fIsRunning = TRUE;

		IFDBG(DebugOut (DEBUG_LAYER_TRACE, L"Shell: new L2CAPDEV successful\n"));
	}

	~L2CAPDEV (void) {
		IFDBG(DebugOut (DEBUG_LAYER_TRACE, L"Shell: delete L2CAPDEV\n"));

		if (hL2CAP)
			L2CAP_CloseDeviceContext (hL2CAP);

		if (pfmdLinks)
			svsutil_ReleaseFixedNonEmpty (pfmdLinks);

		if (pfmdPorts)
			svsutil_ReleaseFixedNonEmpty (pfmdPorts);

		if (pfmdCalls)
			svsutil_ReleaseFixedNonEmpty (pfmdCalls);
	}
};

static L2CAPDEV *gpState = NULL;
//
//	Auxiliary code
//
static L2CAPDEV *CreateNewState (void) {
	return new L2CAPDEV;
}

static SCall *AllocCall (int fWhat, Link *pLink, HANDLE hProcOwner) {
	SCall *pCall = (SCall *)svsutil_GetFixed (gpState->pfmdCalls);
	if (! pCall)
		return NULL;
	
	memset (pCall, 0, sizeof(*pCall));

	pCall->pLink = pLink;
	pCall->hProcOwner = hProcOwner;
	pCall->fWhat = fWhat;

	pCall->hEvent = CreateEvent (NULL, FALSE, FALSE, NULL);
	if (NULL == pCall->hEvent) {
		svsutil_FreeFixed (pCall, gpState->pfmdCalls);
		return NULL;
	}

	if (! gpState->pCalls)
		gpState->pCalls = pCall;
	else {
		SCall *pLast = gpState->pCalls;
		while (pLast->pNext)
			pLast = pLast->pNext;

		pLast->pNext = pCall;
	}

	IFDBG(DebugOut (DEBUG_LAYER_TRACE, L"Shell: Allocated call 0x%08x what = 0x%02x\n", pCall, fWhat));

	return pCall;
}

static void DeleteCall (SCall *pCall) {
	if (pCall == gpState->pCalls)
		gpState->pCalls = pCall->pNext;
	else {
		SCall *pParent = gpState->pCalls;
		while (pParent && (pParent->pNext != pCall))
			pParent = pParent->pNext;

		if (! pParent) {
			IFDBG(DebugOut (DEBUG_ERROR, L"Shell: call not in list in DeleteCall!\n"));

			return;
		}

		pParent->pNext = pCall->pNext;
	}

	CloseHandle (pCall->hEvent);

	if (pCall->pBuffer)
		pCall->pBuffer->pFree (pCall->pBuffer);

	svsutil_FreeFixed (pCall, gpState->pfmdCalls);
}

static SCall *VerifyCall (SCall *pCall) {
	SCall *p = gpState->pCalls;
	while (p && (p != pCall))
		p = p->pNext;

#if defined (DEBUG) || defined (_DEBUG) || defined (RETAILLOG)
	if (! p)
		IFDBG(DebugOut (DEBUG_ERROR, L"Shell: call verify failed!\n"));
#endif

	return p;
}

static SCall *FindCall (unsigned int fOp) {
	SCall *p = gpState->pCalls;
	while (p && (p->fWhat != fOp))
		p = p->pNext;

#if defined (DEBUG) || defined (_DEBUG) || defined (RETAILLOG)
	if (! p)
		IFDBG(DebugOut (DEBUG_ERROR, L"Shell: call find failed!\n"));
#endif

	return p;
}

static SCall *FindCall (Link *pLink, unsigned int fOp) {
	SCall *p = gpState->pCalls;
	while (p && ((p->pLink != pLink) || (p->fWhat != fOp)))
		p = p->pNext;

#if defined (DEBUG) || defined (_DEBUG) || defined (RETAILLOG)
	if (! p)
		IFDBG(DebugOut (DEBUG_ERROR, L"Shell: call verify failed!\n"));
#endif

	return p;
}
	
static Link *VerifyLink (Link *pLink) {
	Link *p = gpState->pLinks;
	while (p && (p != pLink))
		p = p->pNext;

#if defined (DEBUG) || defined (_DEBUG) || defined (RETAILLOG)
	if (! p)
		IFDBG(DebugOut (DEBUG_ERROR, L"Shell: link verify failed!\n"));
#endif

	return p;
}

static void DeleteLink (Link *pLink) {
	IFDBG(DebugOut (DEBUG_LAYER_TRACE, L"Shell: delete link for bd_addr %04x%08x cid 0x%04x\n", pLink->b.NAP, pLink->b.SAP, pLink->cid));

	if (pLink == gpState->pLinks)
		gpState->pLinks = pLink->pNext;
	else {
		Link *pParent = gpState->pLinks;
		while (pParent && (pParent->pNext != pLink))
			pParent = pParent->pNext;

		if (! pParent) {
			IFDBG(DebugOut (DEBUG_ERROR, L"Shell: link to be deleted not in list\n"));
			return;
		}

		pParent->pNext = pLink->pNext;
	}

	SCall *pC = gpState->pCalls;
	while (pC) {
		if (pC->pLink == pLink) {
			if (pC->fAutoClean) {
				DeleteCall (pC);
				pC = gpState->pCalls;
				continue;
			} else if (! pC->fComplete) {
				pC->pLink = NULL;
				pC->fComplete = TRUE;
				pC->iResult = ERROR_CONNECTION_UNAVAIL;
				SetEvent (pC->hEvent);
			} else {
				pC->pLink = NULL;
				if (pC->iResult == ERROR_SUCCESS)
					pC->iResult = ERROR_CONNECTION_UNAVAIL;

			}
		}

		pC = pC->pNext;
	}

	svsutil_FreeFixed (pLink, gpState->pfmdLinks);
}

static Link *FindLink (unsigned short cid) {
	Link *p = gpState->pLinks;
	while (p && (p->cid != cid))
		p = p->pNext;

#if defined (DEBUG) || defined (_DEBUG) || defined (RETAILLOG)
	if (! p)
		IFDBG(DebugOut (DEBUG_ERROR, L"Shell: Link not found for cid 0x%04x\n", cid));
#endif

	return p;
}

static void L2CAP_BufferFree (BD_BUFFER *pBuf) {
	if (! pBuf->fMustCopy)
		g_funcFree (pBuf, g_pvFreeData);
}

static BD_BUFFER *L2CAP_BufferAlloc (int cSize) {
	SVSUTIL_ASSERT (cSize > 0);

	BD_BUFFER *pRes = (BD_BUFFER *)g_funcAlloc (cSize + sizeof (BD_BUFFER), g_pvAllocData);
	pRes->cSize = cSize;

	pRes->cEnd = pRes->cSize;
	pRes->cStart = 0;

	pRes->fMustCopy = FALSE;
	pRes->pFree = L2CAP_BufferFree;
	pRes->pBuffer = (unsigned char *)(pRes + 1);

	return pRes;
}

static BD_BUFFER *L2CAP_BufferCopy (BD_BUFFER *pBuffer) {
	BD_BUFFER *pRes = L2CAP_BufferAlloc (pBuffer->cSize);
	pRes->cSize = pBuffer->cSize;
	pRes->cStart = pBuffer->cStart;
	pRes->cEnd = pBuffer->cEnd;
	pRes->fMustCopy = FALSE;
	pRes->pFree = L2CAP_BufferFree;
	pRes->pBuffer = (unsigned char *)(pRes + 1);

	memcpy (pRes->pBuffer, pBuffer->pBuffer, pRes->cSize);

	return pRes;
}

static DWORD WINAPI StackDown (LPVOID lpVoid) {		// Attention - must increment ref count before calling this!
	if (! gpState)
		return ERROR_SERVICE_NOT_ACTIVE;

	gpState->Lock ();
	gpState->DelRef ();

	if ((! gpState->fIsRunning) || (! gpState->fConnected)) {
		gpState->Unlock ();

		return ERROR_SERVICE_NOT_ACTIVE;
	}

	gpState->fConnected = FALSE;

    Link *pLink = gpState->pLinks;

	while (gpState->pLinks)
		DeleteLink (gpState->pLinks);

	SCall *pC = gpState->pCalls;
	while (pC) {
		if (pC->fWhat != CALL_L2CAP_ACCEPT) {
			if (pC->fAutoClean) {
				DeleteCall (pC);
				pC = gpState->pCalls;
				continue;
			} else if (! pC->fComplete) {
				pC->pLink = NULL;
				pC->fComplete = TRUE;
				pC->iResult = ERROR_CONNECTION_UNAVAIL;
				SetEvent (pC->hEvent);
			} else {
				pC->pLink = NULL;
				if (pC->iResult == ERROR_SUCCESS)
					pC->iResult = ERROR_CONNECTION_UNAVAIL;

			}
		}

		pC = pC->pNext;
	}

	gpState->Unlock ();
	return ERROR_SUCCESS;
}

static DWORD WINAPI StackUp (LPVOID lpVoid) {	// Attention - must increment ref count before calling this!
	if (! gpState)
		return ERROR_SERVICE_NOT_ACTIVE;

	gpState->Lock ();
	gpState->DelRef ();

	if ((! gpState->fIsRunning) || gpState->fConnected) {
		gpState->Unlock ();

		return ERROR_SERVICE_NOT_ACTIVE;
	}

	gpState->fConnected = TRUE;
	gpState->Unlock ();

	return ERROR_SUCCESS;
}

static void ProcessExited (HANDLE hProc) {
	IFDBG(DebugOut (DEBUG_LAYER_TRACE, L"Shell: ProcessExited 0x%08x\n", hProc));

	if (! gpState)
		return;

	gpState->Lock ();

	if (! gpState->fIsRunning) {
		gpState->Unlock ();
		return;
	}


    Port *pPort = gpState->pPorts;

	while (pPort) {
		if (pPort->hProcOwner == hProc) {
			unsigned short psm = pPort->psm;

			gpState->Unlock ();

			L2CAPClosePSM (psm);

			if (! gpState)
				return;

			gpState->Lock ();

			if (! gpState->fIsRunning) {
				gpState->Unlock ();
				return;
			}

			pPort = gpState->pPorts;
			continue;
		}
		pPort = pPort->pNext;
	}
    
    Link *pLink = gpState->pLinks;

	while (pLink) {
		if (pLink->hProcOwner == hProc) {
			unsigned short cid = pLink->cid;
			gpState->Unlock ();

			L2CAPCloseCID (cid);

			if (! gpState)
				return;

			gpState->Lock ();

			if (! gpState->fIsRunning) {
				gpState->Unlock ();
				return;
			}
			pLink = gpState->pLinks;
			continue;
		}

		pLink = pLink->pNext;
	}

	SCall *pCall = gpState->pCalls;
	while (pCall) {
		if ((pCall->hProcOwner == hProc) && (! pCall->fComplete)) {
			if (pCall->fAutoClean) {
				SCall *pNext = pCall->pNext;
				DeleteCall (pCall);
				pCall = pNext;
				continue;
			}

			pCall->fComplete = TRUE;
			pCall->iResult = ERROR_SHUTDOWN_IN_PROGRESS;
			SetEvent (pCall->hEvent);
		}

		pCall = pCall->pNext;
	}

	gpState->Unlock ();
}

//
//	L2CAP stuff
//
static int l2capdev_DataUpInd (void *pUserContext, unsigned short cid, BD_BUFFER *pBuffer) {
	IFDBG(DebugOut (DEBUG_LAYER_TRACE, L"Shell: Data up on channel 0x%04x %d bytes\n", cid, BufferTotal (pBuffer)));

	if (! gpState)
		return ERROR_SERVICE_NOT_ACTIVE;

	gpState->Lock ();

	if (! gpState->fIsRunning) {
		gpState->Unlock ();
		return ERROR_SERVICE_NOT_ACTIVE;
	}

	Link *pLink = FindLink (cid);

	if (pLink) {
		SCall *pCall = gpState->pCalls;
		while (pCall && ((pCall->pLink != pLink) || (pCall->fWhat != CALL_L2CAP_DATA_READ) || pCall->fComplete))
			pCall = pCall->pNext;

		if (! pCall) {
			pCall = AllocCall (CALL_L2CAP_DATA_READ, pLink, pLink->hProcOwner);
			if (! pCall) {
				gpState->Unlock ();
				return ERROR_OUTOFMEMORY;
			}
	
			pCall->fAutoClean = TRUE;
		}

		pCall->pBuffer = pBuffer->fMustCopy ? L2CAP_BufferCopy (pBuffer) : pBuffer;

		pCall->fComplete = TRUE;
		pCall->iResult = ERROR_SUCCESS;
		SetEvent (pCall->hEvent);
	}

	gpState->Unlock ();
	return ERROR_SUCCESS;
}

static int l2capdev_DisconnectInd (void *pUserContext, unsigned short cid, int iError) {
	IFDBG(DebugOut (DEBUG_LAYER_TRACE, L"Shell: Disconnect indicator on channel 0x%04x\n", cid));

	if (! gpState)
		return ERROR_SERVICE_NOT_ACTIVE;

	gpState->Lock ();

	if (! gpState->fIsRunning) {
		gpState->Unlock ();
		return ERROR_SERVICE_NOT_ACTIVE;
	}

	Link *pLink = FindLink (cid);
	if (! pLink) {
		gpState->Unlock ();
		return ERROR_NOT_FOUND;
	}

	if (pLink->fStage == UP) {
		SCall *pC = gpState->pCalls;
		while (pC) {
			if (pC->pLink == pLink) {
				if (pC->fAutoClean) {
					DeleteCall (pC);
					pC = gpState->pCalls;
					continue;
				} else if (! pC->fComplete) {
					pC->fComplete = TRUE;
					pC->iResult = ERROR_CONNECTION_UNAVAIL;
					SetEvent (pC->hEvent);
				}
			}

			pC = pC->pNext;
		}

		pLink->fStage |= LINK_ERROR;
	} else
		DeleteLink (pLink);

	gpState->Unlock ();
	return ERROR_SUCCESS;
}

static int l2capdev_lStackEvent (void *pUserContext, int iEvent, void *pEventContext) {
	IFDBG(DebugOut (DEBUG_LAYER_TRACE, L"Shell: Stack event (L2CAP) %d\n", iEvent));

	if (! gpState)
		return ERROR_SUCCESS;

	gpState->Lock ();

	if (! gpState->fIsRunning) {
		gpState->Unlock ();
		return ERROR_SUCCESS;
	}

	LPTHREAD_START_ROUTINE p = NULL;

	if (iEvent == BTH_STACK_DISCONNECT)
		l2capdev_CloseDriverInstance ();
	else if (iEvent == BTH_STACK_DOWN)
		p = StackDown;
	else if (iEvent == BTH_STACK_UP)
		p = StackUp;

	HANDLE h = p ? CreateThread (NULL, 0, p, NULL, 0, NULL) : NULL;

	if (h) {
		CloseHandle (h);
		gpState->AddRef ();
	}

	gpState->Unlock ();

	return ERROR_SUCCESS;
}

static int l2capdev_lCallAborted (void *pCallContext, int iError) {
	IFDBG(DebugOut (DEBUG_LAYER_TRACE, L"Shell: Aborted call 0x%08x error %d\n", pCallContext, iError));

	WCHAR szError[500];
	wsprintf(szError, L"Shell: Aborted call 0x%08x error %d\n", pCallContext, iError);
	if (! gpState)
		return ERROR_SERVICE_NOT_ACTIVE;

	gpState->Lock ();

	if (! gpState->fIsRunning) {
		gpState->Unlock ();
		return ERROR_SERVICE_NOT_ACTIVE;
	}

	SCall *pCall = VerifyCall ((SCall *)pCallContext);
	if (! pCall) {
		gpState->Unlock ();
		return -1;
	}

	SVSUTIL_ASSERT (! pCall->fComplete);
	SVSUTIL_ASSERT (! pCall->pBuffer);

	Link *pLink = pCall->pLink;

	if (pCall->fAutoClean)
		DeleteCall (pCall);
	else {
		pCall->iResult = iError;
		pCall->fComplete = TRUE;
		pCall->pLink = NULL;
		SetEvent (pCall->hEvent);
	}

	unsigned short disconnect_cid = 0;

	if (pLink) {
		if ((pLink->fStage & UP) != UP) {
			if (pLink->fStage & CONNECTED)
				disconnect_cid = pLink->cid;
			else 
				DeleteLink (pLink);
		} else
			pLink->fStage |= LINK_ERROR;
	}

	gpState->Unlock ();

	if (disconnect_cid)
		L2CAPCloseCID (disconnect_cid);

	return ERROR_SUCCESS;
}

static int l2capdev_ConfigInd (void *pUserContext, unsigned char id, unsigned short cid, unsigned short usOutMTU, unsigned short usInFlushTO, struct btFLOWSPEC *pInFlow, int cOptNum, struct btCONFIGEXTENSION **pExtendedOptions) {
	IFDBG(DebugOut (DEBUG_LAYER_TRACE, L"Shell: Config request indicator on ch 0x%04x id %d MTU %d flush 0x%04x, flow: %s\n", cid, id, usOutMTU, usInFlushTO, pInFlow ? L"yes" : L"no"));

	if (! gpState)
		return ERROR_SERVICE_NOT_ACTIVE;

	gpState->Lock ();

	if (! gpState->fIsRunning) {
		gpState->Unlock ();
		return ERROR_SERVICE_NOT_ACTIVE;
	}

	Link *pLink = FindLink (cid);
	SCall *pCall = pLink ? FindCall (pLink, CALL_L2CAP_LINK_SETUP) : NULL;

	if ((! pCall) || pCall->fComplete)  {
		gpState->Unlock ();
		return ERROR_NOT_FOUND;
	}

	SVSUTIL_ASSERT (pCall->fWhat == CALL_L2CAP_LINK_SETUP);
	SVSUTIL_ASSERT (! pCall->fComplete);
	SVSUTIL_ASSERT (VerifyLink (pCall->pLink));
	SVSUTIL_ASSERT (! pCall->pBuffer);

	int fAccept = FALSE;

	if ((usInFlushTO == 0xffff) && (! pInFlow)) {
		pCall->iResult = ERROR_SUCCESS;
		pCall->pLink->fStage |= CONFIG_IND_DONE;
		pCall->pLink->outMTU = usOutMTU;
		fAccept = TRUE;

		if (pLink->fStage == UP) {
			if (pLink->fIncoming) {
				SVSUTIL_ASSERT (pCall->fAutoClean);
				pCall->fComplete = TRUE;

				SCall *pCall2 = gpState->pCalls;
				while (pCall2 && (pCall2->fComplete || (pCall2->fWhat != CALL_L2CAP_ACCEPT) || (pCall2->psm != pLink->psm)))
					pCall2 = pCall2->pNext;

				if (pCall2) {
					DeleteCall (pCall);
					pCall2->fComplete = TRUE;
					pCall2->iResult = ERROR_SUCCESS;
					pCall2->pLink   = pLink;
					SetEvent (pCall2->hEvent);
				}
			} else {
				pCall->fComplete = TRUE;
				pCall->iResult = ERROR_SUCCESS;
				SetEvent (pCall->hEvent);
			}
		}
	}

	HANDLE hL2CAP = gpState->hL2CAP;
	L2CA_ConfigResponse_In pCallback = gpState->l2cap_if.l2ca_ConfigResponse_In;
	gpState->Unlock ();

	__try {
		pCallback (hL2CAP, NULL, id, cid, fAccept ? 0 : 2, 0, 0xffff, NULL, 0, NULL);
	} __except (1) {
	}

	if (! fAccept)
		l2capdev_lCallAborted (pCall, ERROR_CONNECTION_ABORTED);

	return ERROR_SUCCESS;
}

static int l2capdev_ConfigReq_Out (void *pCallContext, unsigned short usResult, unsigned short usInMTU, unsigned short usOutFlushTO, struct btFLOWSPEC *pOutFlow, int cOptNum, struct btCONFIGEXTENSION **pExtendedOptions) {
	IFDBG(DebugOut (DEBUG_LAYER_TRACE, L"Shell: Config req out for call 0x%08x result 0x%04x mtu %d flush 0x%04x, flow %s\n", pCallContext, usResult, usInMTU, usOutFlushTO, pOutFlow ? L"yes" : L"no" ));

	if (! gpState)
		return ERROR_SERVICE_NOT_ACTIVE;

	gpState->Lock ();

	if (! gpState->fIsRunning) {
		gpState->Unlock ();
		return ERROR_SERVICE_NOT_ACTIVE;
	}

	SCall *pCall = VerifyCall ((SCall *)pCallContext);
	if ((! pCall) || pCall->fComplete)  {
		gpState->Unlock ();
		return ERROR_NOT_FOUND;
	}

	SVSUTIL_ASSERT (pCall->fWhat == CALL_L2CAP_LINK_SETUP);
	SVSUTIL_ASSERT (! pCall->fComplete);
	SVSUTIL_ASSERT (VerifyLink (pCall->pLink));
	SVSUTIL_ASSERT (! pCall->pBuffer);

	if (usResult == 0) {
		Link *pLink = pCall->pLink;
		SVSUTIL_ASSERT (! (pLink->fStage & CONFIG_REQ_DONE));
		SVSUTIL_ASSERT (pLink->fStage & CONNECTED);
		SVSUTIL_ASSERT (pLink->cid);
		SVSUTIL_ASSERT (pLink->psm);
		SVSUTIL_ASSERT (pLink->hProcOwner);

		pLink->fStage |= CONFIG_REQ_DONE;

		if (pLink->fStage == UP) {
			if (pLink->fIncoming) {
				SVSUTIL_ASSERT (pCall->fAutoClean);
				pCall->fComplete = TRUE;

				SCall *pCall2 = gpState->pCalls;
				while (pCall2 && (pCall2->fComplete || (pCall2->fWhat != CALL_L2CAP_ACCEPT) || (pCall2->psm != pLink->psm)))
					pCall2 = pCall2->pNext;

				if (pCall2) {
					DeleteCall (pCall);
					pCall2->fComplete = TRUE;
					pCall2->iResult = ERROR_SUCCESS;
					pCall2->pLink   = pLink;
					SetEvent (pCall2->hEvent);
				}
			} else {
				pCall->fComplete = TRUE;
				pCall->iResult = ERROR_SUCCESS;
				SetEvent (pCall->hEvent);
			}
		}

		gpState->Unlock ();
		return ERROR_SUCCESS;
	}
	gpState->Unlock ();

	l2capdev_lCallAborted (pCallContext, ERROR_CONNECTION_ABORTED);

	return ERROR_SUCCESS;
}

static int l2capdev_ConnectInd (void *pUserContext, BD_ADDR *pba, unsigned short cid, unsigned char id, unsigned short psm) {
	IFDBG(DebugOut (DEBUG_LAYER_TRACE, L"Shell: Connect indicator from %04x%08x ch 0x%04x id %d psm 0x%04x\n", pba->NAP, pba->SAP, cid, id, psm));

	if (! gpState)
		return ERROR_SERVICE_NOT_ACTIVE;

	gpState->Lock ();

	if (! gpState->fIsRunning) {
		gpState->Unlock ();
		return ERROR_SERVICE_NOT_ACTIVE;
	}

	Port *pPort = gpState->pPorts;
	while (pPort && (pPort->psm != psm))
		pPort = pPort->pNext;

	unsigned short	result;
	unsigned short	status = 0;
	unsigned short	mtu = 0;
	SCall *pCall = NULL;

	if (pPort) {
		Link *pLink = (Link *)svsutil_GetFixed (gpState->pfmdLinks);
		pCall = pLink ? AllocCall (CALL_L2CAP_LINK_SETUP, pLink, pPort->hProcOwner) : NULL;
		if (pCall) {
			pCall->fAutoClean = TRUE;

			pLink->b = *pba;
			pLink->cid = cid;
			pLink->fStage = CONNECTED;
			pLink->hProcOwner = pPort->hProcOwner;
			pLink->inMTU = pPort->mtu;
			pLink->outMTU = 0;
			pLink->psm = psm;
			pLink->fIncoming = TRUE;

			pLink->pNext = gpState->pLinks;
			gpState->pLinks = pLink;

			result = 0;
			mtu = pPort->mtu;
		} else {
			if (pLink)
				svsutil_FreeFixed (pLink, gpState->pfmdLinks);
			result = 4;
		}
	} else
		result = 2;

	HANDLE hL2CAP = gpState->hL2CAP;
	L2CA_ConnectResponse_In pCallbackConnect = gpState->l2cap_if.l2ca_ConnectResponse_In;
	L2CA_ConfigReq_In pCallbackConfig = gpState->l2cap_if.l2ca_ConfigReq_In;
	gpState->Unlock ();

	__try {
		pCallbackConnect (hL2CAP, NULL, pba, id, cid, result, status);
	} __except (1) {
	}

	if (result == 0) {
		int iRes = ERROR_INTERNAL_ERROR+1;
		__try {
			iRes = pCallbackConfig (hL2CAP, pCall, cid, mtu, 0xffff, NULL, 0, NULL);
		} __except (1) {
		}

		if (iRes != ERROR_SUCCESS)
			l2capdev_lCallAborted (pCall, -1);
	}

	return ERROR_SUCCESS;
}

static int l2capdev_ConnectReq_Out (void *pCallContext, unsigned short cid, unsigned short result, unsigned short status) {
	IFDBG(DebugOut (DEBUG_LAYER_TRACE, L"Shell: Connect out for call 0x%08x ch 0x%04x result = 0x%04x status 0x%04x\n", pCallContext, cid, result, status));

	if (! gpState)
		return ERROR_SERVICE_NOT_ACTIVE;

	gpState->Lock ();

	if (! gpState->fIsRunning) {
		gpState->Unlock ();
		return ERROR_SERVICE_NOT_ACTIVE;
	}

	SCall *pCall = VerifyCall ((SCall *)pCallContext);
	if ((! pCall) || pCall->fComplete) {
		gpState->Unlock ();
		return ERROR_NOT_FOUND;
	}

	SVSUTIL_ASSERT (pCall->fWhat == CALL_L2CAP_LINK_SETUP);
	SVSUTIL_ASSERT (! pCall->fComplete);
	SVSUTIL_ASSERT (VerifyLink (pCall->pLink));
	SVSUTIL_ASSERT (! pCall->pBuffer);

	if (result) {
		if (result != 1) {
			pCall->fComplete = TRUE;
			pCall->iResult = ERROR_CONNECTION_REFUSED;
			SetEvent (pCall->hEvent);
		}
		gpState->Unlock ();
		return ERROR_SUCCESS;
	}

	Link *pLink = pCall->pLink;

	SVSUTIL_ASSERT (pLink->fStage == NONE);
	SVSUTIL_ASSERT (! pLink->cid);
	SVSUTIL_ASSERT (pLink->psm);
	SVSUTIL_ASSERT (pLink->hProcOwner);

	pLink->fStage = CONNECTED;
	pLink->cid = cid;

	unsigned short mtu = pLink->inMTU;

	HANDLE hL2CAP = gpState->hL2CAP;
	L2CA_ConfigReq_In pCallback = gpState->l2cap_if.l2ca_ConfigReq_In;
	gpState->Unlock ();

	int iRes = ERROR_INTERNAL_ERROR+2;
	__try {
		iRes = pCallback (hL2CAP, pCallContext, cid, mtu, 0xffff, NULL, 0, NULL);
	} __except (1) {
	}

	if (iRes != ERROR_SUCCESS)
		l2capdev_lCallAborted (pCallContext, -2);

	return ERROR_SUCCESS;
}

static int l2capdev_DataDown_Out (void *pCallContext, unsigned short result) {
	IFDBG(DebugOut (DEBUG_LAYER_TRACE, L"Shell: Data down call 0x%08x result %d\n", pCallContext, result));
	if (! gpState)
		return ERROR_SERVICE_NOT_ACTIVE;

	gpState->Lock ();

	if (! gpState->fIsRunning) {
		gpState->Unlock ();
		return ERROR_SERVICE_NOT_ACTIVE;
	}

	SCall *pCall = VerifyCall ((SCall *)pCallContext);
	if ((! pCall) || pCall->fComplete) {
		gpState->Unlock ();
		return ERROR_NOT_FOUND;
	}

	SVSUTIL_ASSERT (pCall->fWhat == CALL_L2CAP_DATA_WRITE);
	SVSUTIL_ASSERT (! pCall->fComplete);
	SVSUTIL_ASSERT (VerifyLink (pCall->pLink));
	SVSUTIL_ASSERT (! pCall->pBuffer);
	SVSUTIL_ASSERT (! pCall->fAutoClean);

	pCall->iResult = ERROR_SUCCESS;
	pCall->fComplete = TRUE;
	SetEvent (pCall->hEvent);

	gpState->Unlock ();
	return ERROR_SUCCESS;
}

static int l2capdev_Ping_Out (void *pCallContext, BD_ADDR *pba, unsigned char *pOutBuffer, unsigned short size) {
	IFDBG(DebugOut (DEBUG_LAYER_TRACE, L"Shell: l2capdev_Ping_Out call 0x%08x result %d\n", pCallContext));
	if (! gpState)
		return ERROR_SERVICE_NOT_ACTIVE;

	gpState->Lock ();

	if (! gpState->fIsRunning) {
		gpState->Unlock ();
		return ERROR_SERVICE_NOT_ACTIVE;
	}

	SCall *pCall = VerifyCall ((SCall *)pCallContext);
	if ((! pCall) || pCall->fComplete) {
		gpState->Unlock ();
		return ERROR_NOT_FOUND;
	}

	SVSUTIL_ASSERT (pCall->fWhat == CALL_L2CAP_PING);
	SVSUTIL_ASSERT (! pCall->fComplete);
	SVSUTIL_ASSERT (! pCall->pBuffer);
	SVSUTIL_ASSERT (! pCall->fAutoClean);

	pCall->iResult = ERROR_SUCCESS;
	pCall->fComplete = TRUE;

	BD_BUFFER *pBuffer = L2CAP_BufferAlloc (size);
	if (pBuffer)
		memcpy (pBuffer->pBuffer, pOutBuffer, size);

	pCall->pBuffer = pBuffer;
	SetEvent (pCall->hEvent);

	gpState->Unlock ();
	return ERROR_SUCCESS;
}

static int l2capdev_ConfigResponse_Out (void *pCallContext, unsigned short result) {
	IFDBG(DebugOut (DEBUG_LAYER_TRACE, L"Shell: ConfigResponse out call 0x%08x, result %d\n", pCallContext, result));
	if (! gpState)
		return ERROR_SERVICE_NOT_ACTIVE;

	gpState->Lock ();

	if (! gpState->fIsRunning) {
		gpState->Unlock ();
		return ERROR_SERVICE_NOT_ACTIVE;
	}

	SCall *pCall = VerifyCall ((SCall *)pCallContext);
	if ((! pCall) || pCall->fComplete) {
		gpState->Unlock ();
		return ERROR_NOT_FOUND;
	}

	SVSUTIL_ASSERT (pCall->fWhat == CALL_L2CAP_CONFIGREQ);
	//SVSUTIL_ASSERT (! pCall->fComplete);
	//SVSUTIL_ASSERT (! pCall->pBuffer);
	//SVSUTIL_ASSERT (! pCall->fAutoClean);

	pCall->iResult = ERROR_SUCCESS;
	pCall->fComplete = TRUE;

	SetEvent (pCall->hEvent);

	gpState->Unlock ();
	return ERROR_SUCCESS;
}

// These are just stubs - they do nothing
static int l2capdev_ConnectResponse_Out (void *pCallContext, unsigned short result) {
	IFDBG(DebugOut (DEBUG_LAYER_TRACE, L"Shell: ConnectResponse out call 0x%08x, result %d\n", pCallContext, result));
	return ERROR_SUCCESS;
}

static int l2capdev_Disconnect_Out (void *pCallContext, unsigned short result) {
	IFDBG(DebugOut (DEBUG_LAYER_TRACE, L"Shell: disconnect out call 0x%08x, result %d\n", pCallContext, result));
	return ERROR_SUCCESS;
}

//
//	Init stuff
//
int l2capdev_CreateDriverInstance (void) {
	IFDBG(DebugOut (DEBUG_LAYER_TRACE, L"+l2capdev_CreateDriverInstance\n"));

	if (gpState) {
		IFDBG(DebugOut (DEBUG_LAYER_TRACE, L"-l2capdev_CreateDriverInstance : ERROR_ALREADY_INITIALIZED\n"));
		return ERROR_ALREADY_INITIALIZED;
	}

	gpState = CreateNewState ();

	if ((! gpState) || (! gpState->fIsRunning)) {
		IFDBG(DebugOut (DEBUG_LAYER_TRACE, L"-l2capdev_CreateDriverInstance : ERROR_OUTOFMEMORY\n"));
		if (gpState)
			delete gpState;
		gpState = NULL;
		return ERROR_OUTOFMEMORY;
	}

	IFDBG(DebugOut (DEBUG_LAYER_TRACE, L"-l2capdev_CreateDriverInstance : ERROR_SUCCESS\n"));
	return ERROR_SUCCESS;
}


int l2capdev_CloseDriverInstance (void) {
	IFDBG(DebugOut (DEBUG_LAYER_TRACE, L"+l2capdev_CloseDriverInstance\n"));

	if (! gpState) {
		IFDBG(DebugOut (DEBUG_LAYER_TRACE, L"-l2capdev_CloseDriverInstance : ERROR_SERVICE_NOT_ACTIVE\n"));
		return ERROR_SERVICE_NOT_ACTIVE;
	}

	gpState->Lock ();
	if (! gpState->fIsRunning) {
		IFDBG(DebugOut (DEBUG_LAYER_TRACE, L"-l2capdev_CloseDriverInstance : ERROR_SERVICE_NOT_ACTIVE\n"));
		gpState->Unlock ();

		return ERROR_SERVICE_NOT_ACTIVE;
	}

	gpState->fIsRunning = FALSE;

	while (gpState->GetRefCount () > 1) {
		IFDBG(DebugOut (DEBUG_LAYER_TRACE, L"Waiting for ref count in l2capdev_CloseDriverInstance\n"));
		gpState->Unlock ();
		Sleep (200);
		gpState->Lock ();
	}

	while (gpState->pCalls) {
		SetEvent (gpState->pCalls->hEvent);
		gpState->pCalls->iResult = ERROR_CANCELLED;
		gpState->pCalls = gpState->pCalls->pNext;
	}

	while (gpState->pLinks) {
		unsigned short cid = gpState->pLinks->cid;

		gpState->Unlock ();
		gpState->l2cap_if.l2ca_Disconnect_In (gpState->hL2CAP, NULL, cid);
		gpState->Lock ();

		gpState->pLinks = gpState->pLinks->pNext;
	}

	IFDBG(DebugOut (DEBUG_LAYER_TRACE, L"-l2capdev_CloseDriverInstance : ERROR_SUCCESS\n"));

	gpState->Unlock ();
	delete gpState;
	gpState = NULL;

	return ERROR_SUCCESS;
}

//
//	Main API section
//
int L2CAPConnect
(
BT_ADDR			*pbt,
unsigned short	usPSM,
unsigned short	usInMTU,
unsigned short	*pusCID,
unsigned short	*pusOutMTU
) {
	BD_ADDR *pba = (BD_ADDR *)pbt;
	if (usPSM == 0)
		return ERROR_INVALID_PARAMETER;

	if (! gpState)
		return ERROR_SERVICE_NOT_ACTIVE;

	gpState->Lock ();
	if (! gpState->fIsRunning) {
		gpState->Unlock ();

		return ERROR_SERVICE_NOT_ACTIVE;
	}

	Link *pLink = (Link *)svsutil_GetFixed (gpState->pfmdLinks);
	memset (pLink, 0, sizeof (*pLink));
	pLink->cid = 0;
	pLink->b = *pba;
	pLink->fStage = NONE;
	pLink->hProcOwner = GetOwnerProcess ();
	pLink->inMTU = usInMTU;
	pLink->fIncoming = FALSE;
	pLink->psm = usPSM;

	pLink->pNext = gpState->pLinks;
	gpState->pLinks = pLink;

	SCall *pCall = AllocCall (CALL_L2CAP_LINK_SETUP, pLink, GetOwnerProcess());
	if (! pCall) {
		gpState->Unlock ();
		return ERROR_OUTOFMEMORY;
	}
	
	HANDLE hEvent = pCall->hEvent;
	HANDLE hL2CAP = gpState->hL2CAP;
	L2CA_ConnectReq_In pCallbackConnect = gpState->l2cap_if.l2ca_ConnectReq_In;
	L2CA_ConfigReq_In pCallbackConfig = gpState->l2cap_if.l2ca_ConfigReq_In;
	int iRes = ERROR_INTERNAL_ERROR+3;
	gpState->Unlock ();
	__try {
		iRes = pCallbackConnect (hL2CAP, pCall, usPSM, pba);
	} __except (1) {
	}

	if (iRes == ERROR_SUCCESS)
	{
		WaitForSingleObject (hEvent, INFINITE);
	}

	if (! gpState)
		return ERROR_SERVICE_NOT_ACTIVE;

	gpState->Lock ();

	if (gpState->fIsRunning) {
		pCall = VerifyCall (pCall);
		pLink = VerifyLink (pLink);

		gpState->Unlock ();
	} else {
		pCall = NULL;
		pLink = NULL;
		iRes = ERROR_SERVICE_NOT_ACTIVE;
	}

	if (iRes == ERROR_SUCCESS)
	{
		if (pCall && pCall->fComplete)
		{
			iRes = pCall->iResult;
		}
		else
		{
			iRes = ERROR_TIMEOUT;
		}
	}

	if (pCall)
	{
		DeleteCall (pCall);
	}
	else
	{
		iRes = ERROR_INTERNAL_ERROR+4;
	}

	if ((! pLink) || (pLink->fStage & LINK_ERROR) || (iRes != ERROR_SUCCESS) || (pLink->fStage != UP)) {
		unsigned short cid_disconnect = 0;

		if (pLink) {
			if ((iRes == ERROR_SUCCESS) && (pLink->fStage != UP))
			{
				iRes = ERROR_INTERNAL_ERROR+5;
			}

			cid_disconnect = pLink->cid;
		}
		else
		{
			iRes = ERROR_INTERNAL_ERROR+6;
		}

		gpState->Unlock ();

		if (cid_disconnect)
			L2CAPCloseCID (cid_disconnect);

		return iRes;
	}

	// We might need to set up our config here

	*pusCID = pLink->cid;
	*pusOutMTU = pLink->outMTU;

	gpState->Unlock ();

	return ERROR_SUCCESS;
}

int L2CAPListen
(
unsigned short usPSM,
unsigned short	usInMTU	// =>
) {
	if (usPSM == 0)
		return ERROR_INVALID_PARAMETER;

	if (! gpState)
		return ERROR_SERVICE_ALREADY_RUNNING;

	gpState->Lock ();
	if (! gpState->fIsRunning) {
		gpState->Unlock ();

		return ERROR_SERIAL_NO_DEVICE;
	}

	HANDLE h = gpState->hL2CAP;
	BT_LAYER_IO_CONTROL pCallback = gpState->l2cap_if.l2ca_ioctl;

	int iRes = ERROR_INTERNAL_ERROR+7;

	gpState->AddRef ();
	gpState->Unlock ();

	__try {
		int iRet = 0;
		unsigned short usPSMin = usPSM;
		iRes = pCallback (h, BTH_STACK_IOCTL_RESERVE_PORT, sizeof(usPSMin), (char *)&usPSMin, sizeof (usPSM), (char *)&usPSM, &iRet);
	} __except (1) {
	}

	gpState->Lock ();
	gpState->DelRef ();

	if (iRes != ERROR_SUCCESS) {
		gpState->Unlock ();
		return ERROR_INTERNAL_ERROR+8;
	}

	Port *p = gpState->pPorts;
	while (p && (p->psm != usPSM))
		p = p->pNext;

	if (p) {
		gpState->Unlock ();
		return ERROR_ADDRESS_ALREADY_ASSOCIATED;
	}

	p = (Port *)svsutil_GetFixed (gpState->pfmdPorts);
	if (! p) {
		gpState->Unlock ();
		return ERROR_OUTOFMEMORY;
	}

	p->hProcOwner = GetOwnerProcess ();
	p->mtu = usInMTU;
	p->psm = usPSM;
	p->pNext = gpState->pPorts;
	gpState->pPorts = p;

	gpState->Unlock ();
	return ERROR_SUCCESS;
}

int L2CAPAccept
(
unsigned short	usPSM,		// =>
BT_ADDR			*pbt,		// <=
unsigned short	*pusCID,	// <=
unsigned short	*pusOutMTU	// <=
) {
	if (usPSM == 0)
		return ERROR_INVALID_PARAMETER;

	if (! gpState)
		return ERROR_SERVICE_NOT_ACTIVE;

	gpState->Lock ();
	if (! gpState->fIsRunning) {
		gpState->Unlock ();

		return ERROR_SERVICE_NOT_ACTIVE;
	}

	SCall *pCall = gpState->pCalls;
	while (pCall && ((pCall->fWhat != CALL_L2CAP_LINK_SETUP) || (! pCall->fComplete) || (! pCall->pLink) || (pCall->pLink->psm != usPSM) || (! pCall->pLink->fIncoming)))
		pCall = pCall->pNext;

	if (pCall) {
		SVSUTIL_ASSERT (pCall->fAutoClean);
		Link *pLink = pCall->pLink;
		DeleteCall (pCall);
		*pusCID = pLink->cid;
		*pusOutMTU = pLink->outMTU;
		*pbt = SET_NAP_SAP(pLink->b.NAP, pLink->b.SAP);

		gpState->Unlock ();

		return ERROR_SUCCESS;
	}

	Port *p = gpState->pPorts;
	while (p && (p->psm != usPSM))
		p = p->pNext;

	if (! p) {
		gpState->Unlock ();
		return ERROR_NOT_FOUND;
	}

	pCall = AllocCall (CALL_L2CAP_ACCEPT, NULL, GetOwnerProcess());
	if (! pCall) {
		gpState->Unlock ();
		return ERROR_OUTOFMEMORY;
	}
	
	pCall->psm = usPSM;
	HANDLE hEvent = pCall->hEvent;

	gpState->Unlock ();

	WaitForSingleObject (hEvent, INFINITE);

	if (! gpState)
		return ERROR_SERVICE_NOT_ACTIVE;

	gpState->Lock ();

	if (pCall != VerifyCall (pCall)) {
		gpState->Unlock ();
		return ERROR_CANCELLED;
	}

	Link *pLink = pCall->pLink;
	int iRes = pCall->fComplete ? pCall->iResult : ERROR_TIMEOUT;

	DeleteCall (pCall);

	if (pLink) {
		*pusCID = pLink->cid;
		*pusOutMTU = pLink->outMTU;		
		*pbt = SET_NAP_SAP(pLink->b.NAP, pLink->b.SAP);

		// Set up config here too
	}

	gpState->Unlock ();
	return iRes;
}

int L2CAPClosePSM (unsigned short usPSM) {
	if (usPSM == 0)
		return ERROR_INVALID_PARAMETER;

	if (! gpState)
		return ERROR_SERVICE_NOT_ACTIVE;

	gpState->Lock ();
	if (! gpState->fIsRunning) {
		gpState->Unlock ();

		return ERROR_SERVICE_NOT_ACTIVE;
	}

	Port *pParent = NULL;
	Port *p = gpState->pPorts;
	while (p && (p->psm != usPSM)) {
		pParent = p;
		p = p->pNext;
	}

	if (! p) {
		gpState->Unlock ();
		return ERROR_NOT_FOUND;
	}

	if (pParent)
		pParent->pNext = p->pNext;
	else
		gpState->pPorts = p->pNext;

	svsutil_FreeFixed (p, gpState->pfmdPorts);

	SCall *pCall = gpState->pCalls;

	while (pCall) {
		if (pCall->psm == usPSM) {
			SVSUTIL_ASSERT (! pCall->fAutoClean);
			SVSUTIL_ASSERT (pCall->fWhat == CALL_L2CAP_ACCEPT);
			SVSUTIL_ASSERT (pCall->hEvent);
			SVSUTIL_ASSERT (pCall->hProcOwner);
			SVSUTIL_ASSERT (! pCall->pBuffer);

			if (! pCall->fComplete) {
				SVSUTIL_ASSERT (! pCall->pLink);
				pCall->fComplete = TRUE;
				pCall->iResult = ERROR_CANCELLED;
				SetEvent (pCall->hEvent);
			}
		}

		pCall = pCall->pNext;
	}

	HANDLE h = gpState->hL2CAP;
	BT_LAYER_IO_CONTROL pCallback = gpState->l2cap_if.l2ca_ioctl;

	int iRes = ERROR_INTERNAL_ERROR+9;
	gpState->Unlock ();

	__try {
		iRes = pCallback (h, BTH_STACK_IOCTL_FREE_PORT, sizeof(usPSM), (char *)&usPSM, 0, NULL, NULL);
	} __except (1) {
	}

	return iRes;
}

int L2CAPCloseCID (unsigned short usCID) {
	if (! gpState)
		return ERROR_SERVICE_NOT_ACTIVE;

	gpState->Lock ();
	if (! gpState->fIsRunning) {
		gpState->Unlock ();

		return ERROR_SERVICE_NOT_ACTIVE;
	}

	Link *pLink = FindLink (usCID);
	if (! pLink) {
		gpState->Unlock ();

		return ERROR_NOT_FOUND;
	}

	unsigned short cid = pLink->cid;
	DeleteLink (pLink);
	HANDLE hL2CAP = gpState->hL2CAP;
	L2CA_Disconnect_In pCallback = gpState->l2cap_if.l2ca_Disconnect_In;

	gpState->Unlock ();

	__try {
		pCallback (hL2CAP, NULL, cid);
	} __except (1) {
	}

	return ERROR_SUCCESS;
}

int L2CAPRead
(
unsigned short usCID,
unsigned int cBuffer,
unsigned int *pRequired,
unsigned char *pBuffer
) {
	if (! gpState)
		return ERROR_SERVICE_NOT_ACTIVE;

	gpState->Lock ();
	if (! gpState->fIsRunning) {
		gpState->Unlock ();

		return ERROR_SERVICE_NOT_ACTIVE;
	}

	Link *pLink = FindLink (usCID);
	if (! pLink) {
		gpState->Unlock ();

		return ERROR_NOT_FOUND;
	}

	if (pLink->fStage & LINK_ERROR) {
		gpState->Unlock ();

		return ERROR_CONNECTION_UNAVAIL;
	}

	SCall *pCall = gpState->pCalls;
	while (pCall && ((pCall->pLink != pLink) || (pCall->fWhat != CALL_L2CAP_DATA_READ) || (! pCall->fAutoClean)))
		pCall = pCall->pNext;

	for (int i = 0 ; ; ++i) {
		if (pCall) {
			SVSUTIL_ASSERT (pCall->fComplete);
			int iRes = pCall->iResult;
			if (iRes == ERROR_SUCCESS) {
				SVSUTIL_ASSERT (pCall->pBuffer);

				unsigned int cSize = BufferTotal (pCall->pBuffer);
				*pRequired = cSize;
				if (cSize <= cBuffer) {
					BufferGetChunk (pCall->pBuffer, cSize, pBuffer);
					DeleteCall (pCall);
				}
			}
			gpState->Unlock ();
			return iRes;
		}

		if (i == 1) {
			gpState->Unlock ();
			return ERROR_CANCELLED;
		}

		if (pLink->fStage != UP) {
			gpState->Unlock ();
			return ERROR_CONNECTION_UNAVAIL;
		}

		pCall = AllocCall (CALL_L2CAP_DATA_READ, pLink, GetOwnerProcess ());
		if (! pCall) {
			gpState->Unlock ();
			return ERROR_OUTOFMEMORY;
		}
		
		HANDLE hEvent = pCall->hEvent;
		gpState->Unlock ();
		WaitForSingleObject (hEvent, INFINITE);
		if (! gpState)
			return ERROR_CANCELLED;

		gpState->Lock ();
		if (! gpState->fIsRunning) {
			gpState->Unlock ();
			return ERROR_CANCELLED;
		}

		pCall = VerifyCall (pCall);
	}
}

int L2CAPWrite
(
unsigned short usCID,
unsigned int cBuffer,
unsigned char *pBuffer
) {
	if (! gpState)
		return ERROR_SERVICE_NOT_ACTIVE;

	gpState->Lock ();
	if (! gpState->fIsRunning) {
		gpState->Unlock ();

		return ERROR_SERVICE_NOT_ACTIVE;
	}

	Link *pLink = FindLink (usCID);
	if ((! pLink) || (pLink->fStage != UP)) {
		gpState->Unlock ();

		return ERROR_NOT_FOUND;
	}

	BD_BUFFER *pB = L2CAP_BufferAlloc (cBuffer + gpState->cHeaders + gpState->cTrailers);
	pB->cStart = gpState->cHeaders;
	pB->cEnd = pB->cSize - gpState->cTrailers;
	memcpy (pB->pBuffer + pB->cStart, pBuffer, cBuffer);

	SCall *pCall = AllocCall (CALL_L2CAP_DATA_WRITE, pLink, GetOwnerProcess ());
	if (! pCall) {
		gpState->Unlock ();
		return ERROR_OUTOFMEMORY;
	}

	HANDLE hEvent = pCall->hEvent;
	HANDLE hL2CAP = gpState->hL2CAP;
	L2CA_DataDown_In pCallback = gpState->l2cap_if.l2ca_DataDown_In;

	gpState->Unlock ();

	int iRes = ERROR_INTERNAL_ERROR+10;
	__try {
		iRes = pCallback (hL2CAP, pCall, usCID, pB);
	} __except (1) {
	}

	if (iRes == ERROR_SUCCESS)
		WaitForSingleObject (hEvent, INFINITE);

	if (! gpState)
		return ERROR_SERVICE_NOT_ACTIVE;

	gpState->Lock ();

	SVSUTIL_ASSERT ((iRes != ERROR_SUCCESS) || pCall->fComplete);
	if (iRes == ERROR_SUCCESS)
		iRes = pCall->iResult;
	else
		L2CAP_BufferFree (pB);

	DeleteCall (pCall);

	gpState->Unlock ();
	return iRes;
}

int L2CAPConfigReq
(
unsigned short usCID,
unsigned short usInMTU,
unsigned short usOutFlushTO,
struct btFLOWSPEC *pOutFlow,
int cOptNum,
struct btCONFIGEXTENSION **ppExtendedOptions
)
{
	if (! gpState)
		return ERROR_SERVICE_NOT_ACTIVE;

	gpState->Lock ();
	if (! gpState->fIsRunning) {
		gpState->Unlock ();

		return ERROR_SERVICE_NOT_ACTIVE;
	}

	Link *pLink = FindLink (usCID);
	if ((! pLink) || (pLink->fStage != UP)) {
		gpState->Unlock ();

		return ERROR_NOT_FOUND;
	}

	SCall *pCall = AllocCall (CALL_L2CAP_CONFIGREQ, pLink, GetOwnerProcess ());
	if (! pCall) {
		gpState->Unlock ();
		return ERROR_OUTOFMEMORY;
	}

	HANDLE hEvent = pCall->hEvent;
	HANDLE hL2CAP = gpState->hL2CAP;
	L2CA_ConfigReq_In pCallback = gpState->l2cap_if.l2ca_ConfigReq_In;

	gpState->Unlock ();

	int iRes = ERROR_INTERNAL_ERROR+11;
	__try {
		iRes = pCallback (hL2CAP, pCall, usCID, usInMTU, usOutFlushTO, pOutFlow, cOptNum, ppExtendedOptions);
	} __except (1) {
	}

	if (iRes == ERROR_SUCCESS)
	{
		WaitForSingleObject (hEvent, INFINITE);
	}

	if (! gpState)
	{
		return ERROR_SERVICE_NOT_ACTIVE;
	}

	gpState->Lock ();

	SVSUTIL_ASSERT ((iRes != ERROR_SUCCESS) || pCall->fComplete);
	if (iRes == ERROR_SUCCESS)
	{
		iRes = pCall->iResult;
	}
	else
	{
	}

	DeleteCall (pCall);

	gpState->Unlock ();
	return iRes;
}


int L2CAPPing
(
BT_ADDR	*pbt,
unsigned int cBufferIn,
unsigned char *pBufferIn,
unsigned int *pcBufferOut,
unsigned char *pBufferOut
) {
	BD_ADDR ba = *(BD_ADDR *)pbt;
	unsigned int cBuffer = *pcBufferOut;
	*pcBufferOut = 0;

	if (! gpState)
		return ERROR_SERVICE_NOT_ACTIVE;

	gpState->Lock ();
	if (! gpState->fIsRunning) {
		gpState->Unlock ();

		return ERROR_SERVICE_NOT_ACTIVE;
	}

	SCall *pCall = AllocCall (CALL_L2CAP_PING, NULL, GetOwnerProcess ());
	if (! pCall) {
		gpState->Unlock ();
		return ERROR_OUTOFMEMORY;
	}

	HANDLE hEvent = pCall->hEvent;
	HANDLE hL2CAP = gpState->hL2CAP;
	L2CA_Ping_In pCallback = gpState->l2cap_if.l2ca_Ping_In;

	gpState->Unlock ();

	int iRes = ERROR_INTERNAL_ERROR+12;
	__try {
		iRes = pCallback (hL2CAP, pCall, &ba, pBufferIn, cBufferIn);
	} __except (1) {
	}

	if (iRes == ERROR_SUCCESS)
		WaitForSingleObject (hEvent, INFINITE);

	if (! gpState)
		return ERROR_SERVICE_NOT_ACTIVE;

	gpState->Lock ();

	SVSUTIL_ASSERT ((iRes != ERROR_SUCCESS) || pCall->fComplete);
	if (iRes == ERROR_SUCCESS) {
		iRes = pCall->iResult;
		if ((iRes == ERROR_SUCCESS) && (pCall->pBuffer)) {
			unsigned int cSize = BufferTotal (pCall->pBuffer);
			*pcBufferOut = cSize;
			if (cSize <= cBuffer)
				BufferGetChunk (pCall->pBuffer, cSize, pBufferOut);
		}
	}

	DeleteCall (pCall);

	gpState->Unlock ();
	return iRes;
}

#if defined (UNDER_CE)
//
//	Driver service funcs...
//
extern "C" BOOL WINAPI DllMain( HANDLE hInstDll, DWORD fdwReason, LPVOID lpvReserved) {
	switch(fdwReason) {
		case DLL_PROCESS_ATTACH:
			DisableThreadLibraryCalls ((HMODULE)hInstDll);
			svsutil_Initialize ();
			break;

		case DLL_PROCESS_DETACH:
			svsutil_DeInitialize ();
			break;
	}
	return TRUE;
}

//-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
//		EXECUTION THREAD: Client-application!
//			These functions are only executed on the caller's thread
// 			i.e. the thread belongs to the client application
//-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+-+
//	@func PVOID | L2C_Init | Device initialization routine
//  @parm DWORD | dwInfo | Info passed to RegisterDevice
//  @rdesc	Returns a DWORD which will be passed to Open & Deinit or NULL if
//			unable to initialize the device.
//	@remark	Routine exported by a device driver.  "PRF" is the string passed
//			in as lpszType in RegisterDevice
extern "C" DWORD L2C_Init (DWORD Index) {
	DebugInit();

	return (DWORD)(l2capdev_CreateDriverInstance () == ERROR_SUCCESS);
}

//	@func PVOID | L2C_Deinit | Device deinitialization routine
//  @parm DWORD | dwData | value returned from L2C_Init call
//  @rdesc	Returns TRUE for success, FALSE for failure.
//	@remark	Routine exported by a device driver.  "PRF" is the string
//			passed in as lpszType in RegisterDevice
extern "C" BOOL L2C_Deinit(DWORD dwData) {
	l2capdev_CloseDriverInstance ();

	DebugDeInit();

	return TRUE;
}

//	@func PVOID | L2C_Open		| Device open routine
//  @parm DWORD | dwData		| value returned from L2C_Init call
//  @parm DWORD | dwAccess		| requested access (combination of GENERIC_READ
//								  and GENERIC_WRITE)
//  @parm DWORD | dwShareMode	| requested share mode (combination of
//								  FILE_SHARE_READ and FILE_SHARE_WRITE)
//  @rdesc	Returns a DWORD which will be passed to Read, Write, etc or NULL if
//			unable to open device.
//	@remark	Routine exported by a device driver.  "PRF" is the string passed
//			in as lpszType in RegisterDevice
extern "C" DWORD L2C_Open (DWORD dwData, DWORD dwAccess, DWORD dwShareMode) {
	HANDLE *ph = (HANDLE *)g_funcAlloc (sizeof(HANDLE), g_pvAllocData);

	ph[0] = NULL;

	return (DWORD)ph;
}

//	@func BOOL | L2C_Close | Device close routine
//  @parm DWORD | dwOpenData | value returned from L2C_Open call
//  @rdesc	Returns TRUE for success, FALSE for failure
//	@remark	Routine exported by a device driver.  "PRF" is the string passed
//			in as lpszType in RegisterDevice
extern "C" BOOL L2C_Close (DWORD dwData)  {
	HANDLE *ph = (HANDLE *)dwData;

	if (ph && ph[0])
		ProcessExited (ph[0]);

	if (ph)
		g_funcFree (ph, g_pvFreeData);

	return TRUE;
}

//	@func DWORD | L2C_Write | Device write routine
//  @parm DWORD | dwOpenData | value returned from L2C_Open call
//  @parm LPCVOID | pBuf | buffer containing data
//  @parm DWORD | len | maximum length to write [IN BYTES, NOT WORDS!!!]
//  @rdesc	Returns -1 for error, otherwise the number of bytes written.  The
//			length returned is guaranteed to be the length requested unless an
//			error condition occurs.
//	@remark	Routine exported by a device driver.  "PRF" is the string passed
//			in as lpszType in RegisterDevice
//
extern "C" DWORD L2C_Write (DWORD dwData, LPCVOID pInBuf, DWORD dwInLen) {
	return -1;
}

//	@func DWORD | L2C_Read | Device read routine
//  @parm DWORD | dwOpenData | value returned from L2C_Open call
//  @parm LPVOID | pBuf | buffer to receive data
//  @parm DWORD | len | maximum length to read [IN BYTES, not WORDS!!]
//  @rdesc	Returns 0 for end of file, -1 for error, otherwise the number of
//			bytes read.  The length returned is guaranteed to be the length
//			requested unless end of file or an error condition occurs.
//	@remark	Routine exported by a device driver.  "PRF" is the string passed
//			in as lpszType in RegisterDevice
//
extern "C" DWORD L2C_Read (DWORD dwData, LPVOID pBuf, DWORD Len) {
	return -1;
}

//	@func DWORD | L2C_Seek | Device seek routine
//  @parm DWORD | dwOpenData | value returned from L2C_Open call
//  @parm long | pos | position to seek to (relative to type)
//  @parm DWORD | type | FILE_BEGIN, FILE_CURRENT, or FILE_END
//  @rdesc	Returns current position relative to start of file, or -1 on error
//	@remark	Routine exported by a device driver.  "PRF" is the string passed
//		 in as lpszType in RegisterDevice

extern "C" DWORD L2C_Seek (DWORD dwData, long pos, DWORD type) {
	return (DWORD)-1;
}

//	@func void | L2C_PowerUp | Device powerup routine
//	@comm	Called to restore device from suspend mode.  You cannot call any
//			routines aside from those in your dll in this call.
extern "C" void L2C_PowerUp (void) {
	return;
}
//	@func void | L2C_PowerDown | Device powerdown routine
//	@comm	Called to suspend device.  You cannot call any routines aside from
//			those in your dll in this call.
extern "C" void L2C_PowerDown (void) {
	return;
}

//	@func BOOL | L2C_IOControl | Device IO control routine
//  @parm DWORD | dwOpenData | value returned from L2C_Open call
//  @parm DWORD | dwCode | io control code to be performed
//  @parm PBYTE | pBufIn | input data to the device
//  @parm DWORD | dwLenIn | number of bytes being passed in
//  @parm PBYTE | pBufOut | output data from the device
//  @parm DWORD | dwLenOut |maximum number of bytes to receive from device
//  @parm PDWORD | pdwActualOut | actual number of bytes received from device
//  @rdesc	Returns TRUE for success, FALSE for failure
//	@remark	Routine exported by a device driver.  "PRF" is the string passed
//		in as lpszType in RegisterDevice
extern "C" BOOL L2C_IOControl(DWORD dwData, DWORD dwCode, PBYTE pBufIn,
			  DWORD dwLenIn, PBYTE pBufOut, DWORD dwLenOut,
			  PDWORD pdwActualOut)
{
#if ! defined (SDK_BUILD)
	if (dwCode == IOCTL_PSL_NOTIFY) {
		PDEVICE_PSL_NOTIFY pPslPacket = (PDEVICE_PSL_NOTIFY)pBufIn;
		if ((pPslPacket->dwSize == sizeof(DEVICE_PSL_NOTIFY)) && (pPslPacket->dwFlags == DLL_PROCESS_EXITING)){
			SVSUTIL_ASSERT (*(HANDLE *)dwData == pPslPacket->hProc);
			ProcessExited ((HANDLE)pPslPacket->hProc);
		}

		return TRUE;
	}
#endif

	HANDLE *ph = (HANDLE *)dwData;
	SVSUTIL_ASSERT (ph);

	if (! ph[0])
		ph[0] = GetCallerProcess ();

	SVSUTIL_ASSERT (ph[0] == GetCallerProcess());

	int iError = ERROR_SUCCESS;

	L2CAPDEVAPICALL *pbapi = (L2CAPDEVAPICALL *)pBufIn;
	if (dwLenIn != sizeof(L2CAPDEVAPICALL)) {
		SetLastError(ERROR_INVALID_PARAMETER);
		return FALSE;
	}

	switch (dwCode) {
	case L2CAPDEV_IOCTL_L2CAPConnect:
		iError = L2CAPConnect (&pbapi->L2CAPConnect_p.ba,
			pbapi->L2CAPConnect_p.usPSM,
			pbapi->L2CAPConnect_p.usInMTU,
			&pbapi->L2CAPConnect_p.usCID,
			&pbapi->L2CAPConnect_p.usOutMTU);
		break;

	case L2CAPDEV_IOCTL_L2CAPListen:
		iError = L2CAPListen (pbapi->L2CAPListen_p.usPSM, pbapi->L2CAPListen_p.usInMTU);
		break;

	case L2CAPDEV_IOCTL_L2CAPAccept:
		iError = L2CAPAccept (pbapi->L2CAPAccept_p.usPSM,
			&pbapi->L2CAPAccept_p.ba,
			&pbapi->L2CAPAccept_p.usCID,
			&pbapi->L2CAPAccept_p.usOutMTU);
		break;

	case L2CAPDEV_IOCTL_L2CAPWrite:
		iError = L2CAPWrite (pbapi->L2CAPReadWrite_p.usCID,
			pbapi->L2CAPReadWrite_p.cBuffer,
			(unsigned char *)MapCallerPtr (pbapi->L2CAPReadWrite_p.pBuffer, pbapi->L2CAPReadWrite_p.cBuffer));
		break;

	case L2CAPDEV_IOCTL_L2CAPRead:
		iError = L2CAPRead (pbapi->L2CAPReadWrite_p.usCID,
			pbapi->L2CAPReadWrite_p.cBuffer,
			&pbapi->L2CAPReadWrite_p.cRequired,
			(unsigned char *)MapCallerPtr (pbapi->L2CAPReadWrite_p.pBuffer, pbapi->L2CAPReadWrite_p.cBuffer));
		break;

	case L2CAPDEV_IOCTL_L2CAPCloseCID:
		iError = L2CAPCloseCID (pbapi->L2CAPClose_p.us);
		break;

	case L2CAPDEV_IOCTL_L2CAPClosePSM:
		iError = L2CAPClosePSM (pbapi->L2CAPClose_p.us);
		break;

	case L2CAPDEV_IOCTL_L2CAPConfigReq:
		iError = L2CAPConfigReq(pbapi->L2CAPConfigReq_p.usCID, pbapi->L2CAPConfigReq_p.usInMTU, pbapi->L2CAPConfigReq_p.usOutFlushTO, pbapi->L2CAPConfigReq_p.pOutFlow, pbapi->L2CAPConfigReq_p.cOptNum, pbapi->L2CAPConfigReq_p.ppExtendedOptions);
		break;

	case L2CAPDEV_IOCTL_L2CAPPing:
		iError = L2CAPPing (&pbapi->L2CAPPing_p.ba, pbapi->L2CAPPing_p.cBufferIn,
				(unsigned char *)MapCallerPtr (pbapi->L2CAPPing_p.pBufferIn, pbapi->L2CAPPing_p.cBufferIn),
				&pbapi->L2CAPPing_p.cBufferOut, 
				(unsigned char *)MapCallerPtr (pbapi->L2CAPPing_p.pBufferOut, pbapi->L2CAPPing_p.cBufferOut));
		break;

	default:
		IFDBG(DebugOut (DEBUG_WARN, L"Unknown control code %d\n", dwCode));
		iError = ERROR_CALL_NOT_IMPLEMENTED;
	}

	if (iError != ERROR_SUCCESS) {
		SetLastError(iError);
		return FALSE;
	}

	return TRUE;
}

#if defined(UNDER_CE) && CE_MAJOR_VER < 0x0003
extern "C" int _isctype(int c, int mask) {
	if ((c < 0) || (c > 0xff))
		return 0;
	return iswctype((wchar_t)c,(wctype_t)mask);
}
#endif //defined(UNDER_CE) && CE_MAJOR_VER < 0x0003

#endif	// UNDER_CE

