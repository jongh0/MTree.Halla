BEGIN_FUNCTION_MAP
.Feed, KOSPI200�ɼǹΰ���(MG), OMG, attr, key=8, group=1 ;
    BEGIN_DATA_MAP
    InBlock,�Է�,input;
    begin
		�ɼ��ڵ�,			optcode,			optcode,		char,	8;
    end
    OutBlock,���,output;
    begin
        ü��ð�,           chetime,            chetime,        char,   6;
		��簡,				actprice,			actprice,		float,	6.2;
		KOSPI200����,		k200jisu,			k200jisu,		float,	6.2;
		��������,			fut200jisu,			fut200jisu,		float,	6.2;
		���簡,				price,				price,			float,	6.2;
		��ǥ���纯����,		capimpv,			capimpv,		float,	6.2;
		���纯����,			impv,				impv,			float,	6.2;
		��Ÿ(������),		delt,				delt,			float,	7.4;
		����(������),		gama,				gama,			float,	7.4;
		��Ÿ(������),		ceta,				ceta,			float,	7.4;
		����(������),		vega,				vega,			float,	7.4;
		�ο�(������),		rhox,				rhox,			float,	7.4;
		�̷а�(������),	theoryprice,		theoryprice,	float,	6.2;
		���ϰ����纯����,	bimpv,				bimpv,			float,	6.2;
		�ŵ������纯����,	offerimpv,			offerimpv,		float,	6.2;
		�ż������纯����,	bidimpv,			bidimpv,		float,	6.2;
		�ɼ��ڵ�,			optcode,			optcode,		char,	8;
    end
    END_DATA_MAP
END_FUNCTION_MAP
OMG