BEGIN_FUNCTION_MAP
	.Func,���º��ſ��ѵ���ȸ,CSPAQ00600,SERVICE=CSPAQ00600,headtype=B,CREATOR=������,CREDATE=2011/12/01 15:40:38;
	BEGIN_DATA_MAP
	CSPAQ00600InBlock1,In(*EMPTY*),input;
	begin
		���ڵ尹��, RecCnt, RecCnt, long, 5
		���¹�ȣ, AcntNo, AcntNo, char, 20;
		�Էº�й�ȣ, InptPwd, InptPwd, char, 8;
		����󼼺з��ڵ�, LoanDtlClssCode, LoanDtlClssCode, char, 2;
		�����ȣ, IsuNo, IsuNo, char, 12;
		�ֹ���, OrdPrc, OrdPrc, double, 13.2;
	end
	CSPAQ00600OutBlock1,In(*EMPTY*),output;
	begin
		���ڵ尹��, RecCnt, RecCnt, long, 5
		���¹�ȣ, AcntNo, AcntNo, char, 20;
		�Էº�й�ȣ, InptPwd, InptPwd, char, 8;
		����󼼺з��ڵ�, LoanDtlClssCode, LoanDtlClssCode, char, 2;
		�����ȣ, IsuNo, IsuNo, char, 12;
		�ֹ���, OrdPrc, OrdPrc, double, 13.2;
	end
	CSPAQ00600OutBlock2,Out(*EMPTY*),output;
	begin
		���ڵ尹��, RecCnt, RecCnt, long, 5
		���¸�, AcntNm, AcntNm, char, 40;
		�ֹ���, OrdPrc, OrdPrc, double, 13.2;
		�����ѵ�, SloanLmtAmt, SloanLmtAmt, long, 16;
		���ֱݾ��հ�, SloanAmtSum, SloanAmtSum, long, 16;
		���ֽűԱݾ�, SloanNewAmt, SloanNewAmt, long, 16;
		���ֻ�ȯ�ݾ�, SloanRfundAmt, SloanRfundAmt, long, 16;
		���������ѵ��ݾ�, MktcplMloanLmtAmt, MktcplMloanLmtAmt, long, 16;
		�������ڱݾ��հ�, MktcplMloanAmtSum, MktcplMloanAmtSum, long, 16;
		�������ڽűԱݾ�, MktcplMloanNewAmt, MktcplMloanNewAmt, long, 16;
		�������ڻ�ȯ�ݾ�, MktcplMloanRfundAmt, MktcplMloanRfundAmt, long, 16;
		�ڱ������ѵ��ݾ�, SfaccMloanLmtAmt, SfaccMloanLmtAmt, long, 16;
		�ڱ����ڱݾ��հ�, SfaccMloanAmtSum, SfaccMloanAmtSum, long, 16;
		�ڱ����ڽűԱݾ�, SfaccMloanNewAmt, SfaccMloanNewAmt, long, 16;
		�ڱ����ڻ�ȯ�ݾ�, SfaccMloanRfundAmt, SfaccMloanRfundAmt, long, 16;
		�������������ѵ��ݾ�, BrnMktcplMloanLmtAmt, BrnMktcplMloanLmtAmt, long, 16;
		�����������ڽűԱݾ�, BrnMktcplMloanNewAmt, BrnMktcplMloanNewAmt, long, 16;
		�����������ڻ�ȯ�ݾ�, BrnMktcplMloanRfundAmt, BrnMktcplMloanRfundAmt, long, 16;
		�����������ڻ��ݾ�, BrnMktcplMloanUseAmt, BrnMktcplMloanUseAmt, long, 16;
		�����ڱ������ѵ��ݾ�, BrnSfaccMloanLmtAmt, BrnSfaccMloanLmtAmt, long, 16;
		�����ڱ����ڽűԱݾ�, BrnSfaccMloanNewAmt, BrnSfaccMloanNewAmt, long, 16;
		�����ڱ����ڻ�ȯ�ݾ�, BrnSfaccMloanRfundAmt, BrnSfaccMloanRfundAmt, long, 16;
		�����ڱ����ڻ��ݾ�, BrnSfaccMloanUseAmt, BrnSfaccMloanUseAmt, long, 16;
		�̿�������ѵ���������, FirmMloanLmtMgmtYn, FirmMloanLmtMgmtYn, char, 1;
		�̿��ſ��������ѱ���, FirmCrdtIsuRestrcTp, FirmCrdtIsuRestrcTp, char, 1;
		�㺸��������, PldgMaintRat, PldgMaintRat, double, 7.4;
		�̿���, FirmNm, FirmNm, char, 50;
		�㺸����, PldgRat, PldgRat, double, 7.4;
		��Ź�ڻ��հ�, DpsastSum, DpsastSum, long, 17;
		�ѵ����氡�ɱݾ�, LmtChgAbleAmt, LmtChgAbleAmt, long, 16;
		�ֹ����ɱݾ�, OrdAbleAmt, OrdAbleAmt, long, 16;
		�ֹ����ɼ���, OrdAbleQty, OrdAbleQty, long, 16;
		�̼��Ұ��ֹ����ɼ���, RcvblUablOrdAbleQty, RcvblUablOrdAbleQty, long, 16;
	end
	END_DATA_MAP
END_FUNCTION_MAP
