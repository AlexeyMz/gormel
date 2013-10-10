#pragma once

#include "stdafx.h"
#include "Station.h"
#include "Call.h"
#include "afxwin.h"
#include "AddCallDlg.h"
// ���������� ���� EditDlg

class EditDlg : public CDialogEx
{
	DECLARE_DYNAMIC(EditDlg)
	Station *station;
	void UpdateCallsList();
public:
	EditDlg(Station *station, CWnd* pParent = NULL);   // ����������� �����������
	virtual ~EditDlg();

	virtual BOOL OnInitDialog() override;

// ������ ����������� ����
	enum { IDD = IDD_DIALOG1 };

protected:
	virtual void DoDataExchange(CDataExchange* pDX);    // ��������� DDX/DDV

	DECLARE_MESSAGE_MAP()
public:
	CListBox m_callsList;
	afx_msg void OnBnClickedButton1();
	afx_msg void OnBnClickedButton4();
	afx_msg void OnBnClickedButton2();
	afx_msg void OnBnClickedButton3();
	afx_msg void OnEnChangeEdit1();
	CEdit m_search;
};
