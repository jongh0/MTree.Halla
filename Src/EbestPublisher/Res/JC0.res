BEGIN_FUNCTION_MAP
.Feed, �ֽļ���ü��(JC0), JC0, attr, key=8, group=1 ;
    BEGIN_DATA_MAP
    InBlock,�Է�,input;
    begin
		�����ڵ�,			futcode,			futcode,		char,	8;
    end
    OutBlock,���,output;
    begin
        �����ڵ�,           futcode,            futcode,        char,       8;
        ü��ð�,           chetime,            chetime,        char,       6;
        ����ȣ,           sign,               sign,           char,       1;
        ���ϴ��,           change,             change,         long,       10;
        �����,             drate,              drate,          double,      6.2;
        ���簡,             price,              price,          long,       10;
        �ð�,               open,               open,           long,       10;
        ��,               high,               high,           long,       10;
        ����,               low,                low,            long,       10;
        ü�ᱸ��,           cgubun,             cgubun,         char,       1;
        ü�ᷮ,             cvolume,            cvolume,        long,       6;
        �����ŷ���,         volume,             volume,         long,       12;
        �����ŷ����,       value,              value,          long,       15;
        �ŵ�����ü�ᷮ,     mdvolume,           mdvolume,       long,       12;
        �ŵ�����ü��Ǽ�,   mdchecnt,           mdchecnt,       long,       8;
        �ż�����ü�ᷮ,     msvolume,           msvolume,       long,       12;
        �ż�����ü��Ǽ�,   mschecnt,           mschecnt,       long,       8;
        ü�ᰭ��,           cpower,             cpower,         double,      9.2;
        �ŵ�ȣ��1,          offerho1,           offerho1,       long,       10;
        �ż�ȣ��1,          bidho1,             bidho1,         long,       10;
        �̰�����������,     openyak,            openyak,        long,       8;
        KOSPI200����,       k200jisu,           k200jisu,       double,      6.2;
        �̷а�,             theoryprice,        theoryprice,    long,       8;
        ������,             kasis,              kasis,          double,      6.3;
        ����BASIS,          sbasis,             sbasis,         long,       6;
        �̷�BASIS,          ibasis,             ibasis,         long,       6;
        �̰�����������,     openyakcha,         openyakcha,     long,       8;
        ������,         jgubun,             jgubun,         char,       2;
        ���ϵ��ð���ŷ���, jnilvolume,         jnilvolume,     long,       12;
		�����ڻ����簡,     basprice,           basprice,       long,       8;
    end
    END_DATA_MAP
END_FUNCTION_MAP
JC0