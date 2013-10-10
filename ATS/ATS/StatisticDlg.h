#pragma once
#include "stdafx.h"
#include "Station.h"
#include "afxwin.h"

// ���������� ���� CStatisticDlg

class CStatisticDlg : public CDialogEx
{
	DECLARE_DYNAMIC(CStatisticDlg)
	Station *station;
public:
	CStatisticDlg(Station *station, CWnd* pParent = NULL);   // ����������� �����������
	virtual ~CStatisticDlg();
	virtual BOOL OnInitDialog() override;

// ������ ����������� ����
	enum { IDD = IDD_DIALOG4 };

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // ��������� DDX/DDV

	DECLARE_MESSAGE_MAP()
public:
	CListBox m_list;
};
