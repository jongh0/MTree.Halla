BEGIN_FUNCTION_MAP
.Feed, KOSPI200�ɼ�ȣ��(H0), OH0, attr, key=8, group=1 ;
    BEGIN_DATA_MAP
    InBlock,�Է�,input;
    begin
		�����ڵ�,			optcode,			optcode,		char,	8;
    end
    OutBlock,���,output;
    begin
		ȣ���ð�,			hotime,				hotime,			char,	6;
		�ŵ�ȣ��1,			offerho1,			offerho1,		double,	6.2;
		�ż�ȣ��1,			bidho1,				bidho1,			double,	6.2;
		�ŵ�ȣ������1,		offerrem1,			offerrem1,		long,	7;
		�ż�ȣ������1,		bidrem1,			bidrem1,		long,	7;
		�ŵ�ȣ���Ǽ�1,		offercnt1,			offercnt1,		long,	5;
		�ż�ȣ���Ǽ�1,		bidcnt1,			bidcnt1,		long,	5;
		�ŵ�ȣ��2,			offerho2,			offerho2,		double,	6.2;
		�ż�ȣ��2,			bidho2,				bidho2,			double,	6.2;
		�ŵ�ȣ������2,		offerrem2,			offerrem2,		long,	7;
		�ż�ȣ������2,		bidrem2,			bidrem2,		long,	7;
		�ŵ�ȣ���Ǽ�2,		offercnt2,			offercnt2,		long,	5;
		�ż�ȣ���Ǽ�2,		bidcnt2,			bidcnt2,		long,	5;
		�ŵ�ȣ��3,			offerho3,			offerho3,		double,	6.2;
		�ż�ȣ��3,			bidho3,				bidho3,			double,	6.2;
		�ŵ�ȣ������3,		offerrem3,			offerrem3,		long,	7;
		�ż�ȣ������3,		bidrem3,			bidrem3,		long,	7;
		�ŵ�ȣ���Ǽ�3,		offercnt3,			offercnt3,		long,	5;
		�ż�ȣ���Ǽ�3,		bidcnt3,			bidcnt3,		long,	5;
		�ŵ�ȣ��4,			offerho4,			offerho4,		double,	6.2;
		�ż�ȣ��4,			bidho4,				bidho4,			double,	6.2;
		�ŵ�ȣ������4,		offerrem4,			offerrem4,		long,	7;
		�ż�ȣ������4,		bidrem4,			bidrem4,		long,	7;
		�ŵ�ȣ���Ǽ�4,		offercnt4,			offercnt4,		long,	5;
		�ż�ȣ���Ǽ�4,		bidcnt4,			bidcnt4,		long,	5;
		�ŵ�ȣ��5,			offerho5,			offerho5,		double,	6.2;
		�ż�ȣ��5,			bidho5,				bidho5,			double,	6.2;
		�ŵ�ȣ������5,		offerrem5,			offerrem5,		long,	7;
		�ż�ȣ������5,		bidrem5,			bidrem5,		long,	7;
		�ŵ�ȣ���Ǽ�5,		offercnt5,			offercnt5,		long,	5;
		�ż�ȣ���Ǽ�5,		bidcnt5,			bidcnt5,		long,	5;
		�ŵ�ȣ���Ѽ���,		totofferrem,		totofferrem,	long,	7;
		�ż�ȣ���Ѽ���,		totbidrem,			totbidrem,		long,	7;
		�ŵ�ȣ���ѰǼ�,		totoffercnt,		totoffercnt,	long,	5;
		�ż�ȣ���ѰǼ�,		totbidcnt,			totbidcnt,		long,	5;
		�����ڵ�,			optcode,			optcode,		char,	8;
		���ϰ�ȣ������,		danhochk,			danhochk,		char,	1;
		������뱸��,		alloc_gubun,		alloc_gubun,	char,	1;
    end
    END_DATA_MAP
END_FUNCTION_MAP
OH0