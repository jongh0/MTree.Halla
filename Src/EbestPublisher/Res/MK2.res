BEGIN_FUNCTION_MAP
.Feed, US����(MK2), MK2, attr, key=16, group=1;
    BEGIN_DATA_MAP
    InBlock,�Է�,input;
    begin
		�ɺ��ڵ�,			symbol,			symbol,			char,	16;
    end
    OutBlock,���,output;
    begin
		����,			    date,		    date,		    char,	8;
		�ð�,			    time,		    time,		    char,	6;
		�ѱ�����,		    kodate,		    kodate,		    char,	8;
		�ѱ��ð�,		    kotime,		    kotime,		    char,	6;
		�ð�,			    open,		    open,		    float,	9.2;
		��,			    high,		    high,		    float,	9.2;
		����,			    low,		    low,		    float,	9.2;
		���簡,			    price,		    price,		    float,	9.2;
		���ϴ�񱸺�,	    sign,		    sign,		    char,	1;
		���ϴ��,			change,		    change,		    float,	9.2;
		�����,			    uprate,		    uprate,		    float,	9.2;
		�ż�ȣ��,		    bidho,		    bidho,		    float,	9.2;
		�ż��ܷ�,		    bidrem,		    bidrem,		    long,	9;
		�ŵ�ȣ��,		    offerho,		offerho,		float,	9.2;
		�ŵ��ܷ�,		    offerrem,		offerrem,		long,	9;
		�����ŷ���,		    volume,		    volume,		    float,	12.0;
		�ɹ�,		        xsymbol,	    xsymbol,        char,	16;
		ü��ŷ���,		    cvolume,	    cvolume,	float,	8.0;
    end
    END_DATA_MAP
END_FUNCTION_MAP
