BEGIN_FUNCTION_MAP
.Feed, KOSPI���α׷��Ÿ�����(PH), PH_, attr, key=6, group=1;
    BEGIN_DATA_MAP
    InBlock,�Է�,input;
    begin
        �����ڵ�,                   shcode,         shcode,     char,   6;
    end
    OutBlock,���,output;
    begin
        ���Žð�,                   time,           time,       char,   6;

        ���簡,                     price,          price,      long,   8;
        ���ϴ�񱸺�,               sign,           sign,       long,   1;
        ���ϴ��,                   change,         change,     long,   8;
        �����ŷ���,                 volume,         volume,     long,   10;
        �����,                     drate,          drate,      float,  6.2;

        ���͸ŵ�ȣ�� �ܷ�,          cdhrem,         cdhrem,     long,   9;
        ���͸ż�ȣ�� �ܷ�,          cshrem,         cshrem,     long,   9;
        �����͸ŵ�ȣ�� �ܷ�,        bdhrem,         bdhrem,     long,   9;
        �����͸ż�ȣ�� �ܷ�,        bshrem,         bshrem,     long,   9;

        ���͸ŵ�ȣ�� ����,          cdhvolume,      cdhvolume,  long,   9;
        ���͸ż�ȣ�� ����,          cshvolume,      cshvolume,  long,   9;
        �����͸ŵ�ȣ�� ����,        bdhvolume,      bdhvolume,  long,   9;
        �����͸ż�ȣ�� ����,        bshvolume,      bshvolume,  long,   9;

        ��ü�ŵ���Źü�����,       dwcvolume,      dwcvolume,  long,   9;
        ��ü�ż���Źü�����,       swcvolume,      swcvolume,  long,   9;
        ��ü�ŵ��ڱ�ü�����,       djcvolume,      djcvolume,  long,   9;
        ��ü�ż��ڱ�ü�����,       sjcvolume,      sjcvolume,  long,   9;

        ��ü�ŵ�ü�����,           tdvolume,       tdvolume,   long,   9;
        ��ü�ż�ü�����,           tsvolume,       tsvolume,   long,   9;
        ��ü���ż� ����,            tvol,           tvol,       long,   9;

        ��ü�ŵ���Źü��ݾ�,       dwcvalue,       dwcvalue,   long,   12;
        ��ü�ż���Źü��ݾ�,       swcvalue,       swcvalue,   long,   12;
        ��ü�ŵ��ڱ�ü��ݾ�,       djcvalue,       djcvalue,   long,   12;
        ��ü�ż��ڱ�ü��ݾ�,       sjcvalue,       sjcvalue,   long,   12;

        ��ü�ŵ�ü��ݾ�,           tdvalue,        tdvalue,    long,   12;
        ��ü�ż�ü��ݾ�,           tsvalue,        tsvalue,    long,   12;
        ��ü���ż� �ݾ�,            tval,           tval,       long,   12;

        �ŵ� �������ü���,          pdgvolume,      pdgvolume,  long,   9;
        �ż� �������ü���,          psgvolume,      psgvolume,  long,   9;

        �����ڵ�,                   shcode,         shcode,     char,   6;
    end
    END_DATA_MAP
END_FUNCTION_MAP
