BEGIN_FUNCTION_MAP
.Feed, �ð��ܴ��ϰ�VI�ߵ�����(DVI), DVI, attr, key=6, group=1;
    BEGIN_DATA_MAP
    InBlock,�Է�,input;
    begin
		�����ڵ�(KEY),			shcode,		shcode,			char,	6;
    end
    OutBlock,���,output;
    begin
		����(0:���� 1:�����ߵ� 2:�����ߵ� 3:����&����)	,	vi_gubun	,	vi_gubun	,	char,	1;
		����VI�ߵ����ذ���								,	svi_recprice,	svi_recprice,	long,	8;
		����VI�ߵ����ذ���								,	dvi_recprice,	dvi_recprice,	long,	8;
		VI�ߵ�����										,	vi_trgprice	,	vi_trgprice	,	long,	8;
		�����ڵ�(KEY)									,	shcode		,	shcode		,	char,	6;
		�����ڵ�(�̻��)								,	ref_shcode,		ref_shcode,		char,	6;
		�ð�											,	time,			time,			char,	6;
    end
    END_DATA_MAP
END_FUNCTION_MAP
