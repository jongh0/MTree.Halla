BEGIN_FUNCTION_MAP
.Feed, �ǽð� ���� ���� ��Ŷ(NWS), NWS, key=6, group=1;
    BEGIN_DATA_MAP
    InBlock,�Է�,input;
    begin
		�����ڵ�,			nwcode,			nwcode,			char,	6;
    end
    OutBlock,���,output;
    begin
		��¥,               date,           date,           char,   8;
		�ð�,               time,           time,           char,   6;
		����������,         id,             id,             char,   2;
		Ű��,	    		realkey,		realkey,		char,	24;
		����,       		title,		    title,		    char,	300;
		���������ڵ�,  		code,		    code,		    char,	240;
		BODY����,   		bodysize,		bodysize,		long,	8;
    end
    END_DATA_MAP
END_FUNCTION_MAP
