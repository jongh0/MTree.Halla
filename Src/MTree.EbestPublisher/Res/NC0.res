BEGIN_FUNCTION_MAP
.Feed, CME����KP200��������ü��(NC0), NC0, attr, key=8, group=1 ;
    BEGIN_DATA_MAP
    InBlock,�Է�,input;
    begin
		�����ڵ�,			futcode,			futcode,		char,	8;
    end
    OutBlock,���,output;
    begin
		ü��ð�(24�ð�),	chetime,			chetime,		char,	6;
		ü��ð�(36�ð�),	chetime1,			chetime1,		char,	6;
		���ϴ�񱸺�,		sign,				sign,			char,	1;
		���ϴ��,			change,				change,			float,	6.2;
		�����,				drate,				drate,			float,	6.2;
		���簡,				price,				price,			float,	6.2;
		�ð�,				open,				open,			float,	6.2;
		��,				high,				high,			float,	6.2;
		����,				low,				low,			float,	6.2;
		ü�ᱸ��,			cgubun,				cgubun,			char,	1;
		ü�ᷮ,				cvolume,			cvolume,		long,	6;
		�����ŷ���,			volume,				volume,			long,	12;
		�����ŷ����,		value,				value,			long,	12;
		�ŵ�����ü�ᷮ,		mdvolume,			mdvolume,		long,	12;
		�ŵ�����ü��Ǽ�,	mdchecnt,			mdchecnt,		long,	8;
		�ż�����ü�ᷮ,		msvolume,			msvolume,		long,	12;
		�ż�����ü��Ǽ�,	mschecnt,			mschecnt,		long,	8;
		ü�ᰭ��,			cpower,				cpower,			float,	9.2;
		�ŵ�ȣ��1,			offerho1,			offerho1,		float,	6.2;
		�ż�ȣ��1,			bidho1,				bidho1,			float,	6.2;
		�̰�����������,		openyak,			openyak,		long,	8;
		KOSPI200����,		k200jisu,			k200jisu,		float,	6.2;
		�̷а�,				theoryprice,		theoryprice,	float,	6.2;
		������,				kasis,				kasis,			float,	6.2;
		����BASIS,			sbasis,				sbasis,			float,	6.2;
		�̷�BASIS,			ibasis,				ibasis,			float,	6.2;
		�̰�����������,		openyakcha,			openyakcha,		long,	8;
		������,			jgubun,				jgubun,			char,	2;
		�̻��filler,		jnilvolume,			jnilvolume,		long,	12;
		�����ڵ�,			futcode,			futcode,		char,	8;
    end
    END_DATA_MAP
END_FUNCTION_MAP
NC0