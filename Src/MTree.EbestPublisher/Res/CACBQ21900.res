BEGIN_FUNCTION_MAP
	.Func,������������ ��ȸ,CACBQ21900,SERVICE=CACBQ21900,headtype=B,CREATOR=�ݹ̼�,CREDATE=2012/04/19 14:46:14;
	BEGIN_DATA_MAP
	CACBQ21900InBlock1,In(*EMPTY*),input;
	begin
		���ڵ尹��, RecCnt, RecCnt, long, 5
		���¹�ȣ, AcntNo, AcntNo, char, 20;
		�Էº�й�ȣ, InptPwd, InptPwd, char, 8;
	end
	CACBQ21900OutBlock1,In(*EMPTY*),output;
	begin
		���ڵ尹��, RecCnt, RecCnt, long, 5
		���¹�ȣ, AcntNo, AcntNo, char, 20;
		�Էº�й�ȣ, InptPwd, InptPwd, char, 8;
	end
	CACBQ21900OutBlock2,Out(*EMPTY*),output;
	begin
		���ڵ尹��, RecCnt, RecCnt, long, 5
		��ǰ���ڵ�, PrdtDtlCode, PrdtDtlCode, char, 3;
		��ǰ�󼼸�, PrdtDtlNm, PrdtDtlNm, char, 40;
		�������¹�ȣ, HirnkAcntNo, HirnkAcntNo, char, 20;
		���»��¸�, AcntStatNm, AcntStatNm, char, 40;
		���¸�, AcntNm, AcntNm, char, 40;
		�����ѱۺα��, AcntHanglAlias, AcntHanglAlias, char, 40;
		���������ȣ, MgempNo, MgempNo, char, 9;
		���������, MgempNm, MgempNm, char, 40;
		���������ȣ, CnvsEmpNo, CnvsEmpNo, char, 9;
		���������, CnvsEmpNm, CnvsEmpNm, char, 40;
		����������, MgmtBrnNm, MgmtBrnNm, char, 40;
		����������, AgrgtBrnNm, AgrgtBrnNm, char, 40;
		�Ǹ�Ȯ�ι���ڵ��, RmnCnfMthdCodeNm, RmnCnfMthdCodeNm, char, 18;
		���ͳݰ��������ڵ��, InetOpnTpCodeNm, InetOpnTpCodeNm, char, 20;
		�����ڸ�, RcptPsnNm, RcptPsnNm, char, 40;
		���°�����, AcntOpnDt, AcntOpnDt, char, 8;
		�����ŷ���, LastTrdDt, LastTrdDt, char, 8;
		������, TrsfrDt, TrsfrDt, char, 8;
		������, BfOutDt, BfOutDt, char, 8;
		���԰��¹�ȣ, TrsfrAcntNo, TrsfrAcntNo, char, 20;
		������¹�ȣ, BfOutAcntNo, BfOutAcntNo, char, 20;
		����������ȣ, TrsfrBrnNo, TrsfrBrnNo, char, 3;
		����������ȣ, TrnsfBrnNo, TrnsfBrnNo, char, 3;
		������������, AcntCncDt, AcntCncDt, char, 8;
		����������������, AcntCncAbndDt, AcntCncAbndDt, char, 8;
		������������, AcntAbndDt, AcntAbndDt, char, 8;
		�����ó����, MisincTrxDt, MisincTrxDt, char, 8;
		HTS�����, HtsRegDt, HtsRegDt, char, 8;
		ä�����������, DebtUnMigrtStatNm, DebtUnMigrtStatNm, char, 40;
		���尳����, PsbOpnDt, PsbOpnDt, char, 8;
		���ű�¡��������, MgnLevyPtnNm, MgnLevyPtnNm, char, 40;
		��Ź�����ںз��ڵ�, CsgnInvstrClssCode, CsgnInvstrClssCode, char, 4;
		��Ź�����ںз�, CsgnInvstrClssCodeNm, CsgnInvstrClssCodeNm, char, 40;
		�ŷ�������, TrtaxTaxchrYnNm, TrtaxTaxchrYnNm, char, 40;
		Ư�̱���, UnuslAcntClssNm, UnuslAcntClssNm, char, 40;
		�ֽ�ä�Ǳ���, TrdSecPtnNm, TrdSecPtnNm, char, 40;
		���������ڵ��, BnsCmsnAmtGrdCodeNm, BnsCmsnAmtGrdCodeNm, char, 40;
		���������ڵ��, SettCmsnAmtGrdCodeNm, SettCmsnAmtGrdCodeNm, char, 40;
		���ոŸž��Ȯ�ο���, SyntrdStplCnfYn, SyntrdStplCnfYn, char, 1;
		�ŷ����и�, TrdTpNm, TrdTpNm, char, 20;
		�������ް��¹�ȣ, MktTransAcntNo, MktTransAcntNo, char, 20;
		��Ź�ڱⱸ�и�, CsgnSfaccTpNm, CsgnSfaccTpNm, char, 40;
		��Ͻ����ڵ�, RegMktCode, RegMktCode, char, 2;
		��Ͻ����, RegMktNm, RegMktNm, char, 40;
		�ɼǼ��������ڵ��, OptCmsnGrdCodeNm, OptCmsnGrdCodeNm, char, 40;
		�������������ڵ��, FutsCmsnGrdCodeNm, FutsCmsnGrdCodeNm, char, 40;
		FCM���¹�ȣ, FcmAcntNo, FcmAcntNo, char, 20;
		�ٰŰ��¹�ȣ, GrndAcntNo, GrndAcntNo, char, 20;
		��������, DueDt, DueDt, char, 8;
		��ü��������, BkeepCtrctYn, BkeepCtrctYn, char, 1;
		�ڵ���ü��������, AbkpCtrctYn, AbkpCtrctYn, char, 1;
		����������������, FutsLnkDsgnYn, FutsLnkDsgnYn, char, 1;
		û���ڱݴ����������, AplLoanCtrctYn, AplLoanCtrctYn, char, 1;
		�ڵ��Ա��԰������������, AutoInAcntDsgnYn, AutoInAcntDsgnYn, char, 1;
		��Ÿ�ڱݴ����������, EmLoanCtrctYn, EmLoanCtrctYn, char, 1;
		�ŵ���ݴ㺸�����������, CslLoanCtrctYn, CslLoanCtrctYn, char, 1;
		�����ڱݴ����������, PmLoanCtrctYn, PmLoanCtrctYn, char, 1;
		�ſ���¿���, CrdtAcntYn, CrdtAcntYn, char, 1;
		����������������ڵ�, LoanAcntKindTpCode, LoanAcntKindTpCode, char, 1;
		���ͳݰ��������ڵ�, InetOpnTpCode, InetOpnTpCode, char, 1;
		���̹����������ڵ�, CybBrnTpCode, CybBrnTpCode, char, 1;
	end
	END_DATA_MAP
END_FUNCTION_MAP
