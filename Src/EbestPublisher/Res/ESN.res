BEGIN_FUNCTION_MAP
.Feed, ��ELW������ǥ�ΰ���(ESN), ESN, attr, key=6, group=1;
    BEGIN_DATA_MAP
    InBlock,�Է�,input;
    begin
		�����ڵ�,			shcode,			shcode,			char,	6;
    end
    OutBlock,���,output;
    begin
		�ð�,				time,			time,			char,	6;
		�����̷а�,			theoryprice,	theoryprice,	float,	10.2;
		��Ÿ,				delt,			delt,			float,	7.6;
		����,				gama,			gama,			float,	7.6;
		��Ÿ,				ceta,			ceta,			float,	12.6;
		����,				vega,			vega,			float,	12.6;
		�ο�,				rhox,			rhox,			float,	12.6;
		���纯����,			impv,			impv,			float,	5.2;
		E.��,			egearing,		egearing,		float,	8.2;
		�����ڵ�,			shcode,			shcode,			char,	6;
		ELW���簡,          elwclose,       elwclose,       long,   8;
        ELW���ϴ�񱸺�,    sign,           sign,           char,   1;
        ELW���ϴ��,        change,         change,         long,   8;
		����,               date,           date,           char,   8;
		ƽȯ��,             tickvalue,      tickvalue,      float,  10.2;
		LP���纯����,		lp_impv,		lp_impv,		float,	5.2;
    end
    END_DATA_MAP
END_FUNCTION_MAP
ESN