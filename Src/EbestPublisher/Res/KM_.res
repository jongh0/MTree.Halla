BEGIN_FUNCTION_MAP
.Feed, KOSDAQ���α׷��Ÿ���ü����(KM), KM_, attr, key=1, group=1;
    BEGIN_DATA_MAP
    InBlock,�Է�,input;
    begin
        ���а�,                     gubun,          gubun,      char,   1;
    end
    OutBlock,���,output;
    begin
        ���Žð�,                   time,           time,       char,   6;
        ���͸ŵ�ȣ�� �ܷ�,          cdhrem,         cdhrem,     long,   6;
        ���͸ż�ȣ�� �ܷ�,          cshrem,         cshrem,     long,   6;
        �����͸ŵ�ȣ�� �ܷ�,        bdhrem,         bdhrem,     long,   6;
        �����͸ż�ȣ�� �ܷ�,        bshrem,         bshrem,     long,   6;

        ���͸ŵ�ȣ�� ����,          cdhvolume,      cdhvolume,  long,   6;
        ���͸ż�ȣ�� ����,          cshvolume,      cshvolume,  long,   6;
        �����͸ŵ�ȣ�� ����,        bdhvolume,      bdhvolume,  long,   6;
        �����͸ż�ȣ�� ����,        bshvolume,      bshvolume,  long,   6;

        ���͸ŵ���Źü�����,       cdwvolume,      cdwvolume,  long,   6;
        ���͸ŵ��ڱ�ü�����,       cdjvolume,      cdjvolume,  long,   6;
        ���͸ż���Źü�����,       cswvolume,      cswvolume,  long,   6;
        ���͸ż��ڱ�ü�����,       csjvolume,      csjvolume,  long,   6;
        ������Ź���ż� ����,        cwvol,          cwvol,      long,   6;
        �����ڱ���ż� ����,        cjvol,          cjvol,      long,   6;

        �����͸ŵ���Źü�����,     bdwvolume,      bdwvolume,  long,   6;
        �����͸ŵ��ڱ�ü�����,     bdjvolume,      bdjvolume,  long,   6;
        �����͸ż���Źü�����,     bswvolume,      bswvolume,  long,   6;
        �����͸ż��ڱ�ü�����,     bsjvolume,      bsjvolume,  long,   6;
        ��������Ź���ż� ����,      bwvol,          bwvol,      long,   6;
        �������ڱ���ż� ����,      bjvol,          bjvol,      long,   6;

        ��ü�ŵ���Źü�����,       dwvolume,       dwvolume,   long,   6;
        ��ü�ż���Źü�����,       swvolume,       swvolume,   long,   6;
        ��ü��Ź���ż� ����,        wvol,           wvol,       long,   6;
        ��ü�ŵ��ڱ�ü�����,       djvolume,       djvolume,   long,   6;
        ��ü�ż��ڱ�ü�����,       sjvolume,       sjvolume,   long,   6;
        ��ü�ڱ���ż� ����,        jvol,           jvol,       long,   6;

        ���͸ŵ���Źü��ݾ�,       cdwvalue,       cdwvalue,   long,   8;
        ���͸ŵ��ڱ�ü��ݾ�,       cdjvalue,       cdjvalue,   long,   8;
        ���͸ż���Źü��ݾ�,       cswvalue,       cswvalue,   long,   8;
        ���͸ż��ڱ�ü��ݾ�,       csjvalue,       csjvalue,   long,   8;
        ������Ź���ż� �ݾ�,        cwval,          cwval,      long,   8;
        �����ڱ���ż� �ݾ�,        cjval,          cjval,      long,   8;

        �����͸ŵ���Źü��ݾ�,     bdwvalue,       bdwvalue,   long,   8;
        �����͸ŵ��ڱ�ü��ݾ�,     bdjvalue,       bdjvalue,   long,   8;
        �����͸ż���Źü��ݾ�,     bswvalue,       bswvalue,   long,   8;
        �����͸ż��ڱ�ü��ݾ�,     bsjvalue,       bsjvalue,   long,   8;
        ��������Ź���ż� �ݾ�,      bwval,          bwval,      long,   8;
        �������ڱ���ż� �ݾ�,      bjval,          bjval,      long,   8;

        ��ü�ŵ���Źü��ݾ�,       dwvalue,        dwvalue,    long,   8;
        ��ü�ż���Źü��ݾ�,       swvalue,        swvalue,    long,   8;
        ��ü��Ź���ż� �ݾ�,        wval,           wval,       long,   8;
        ��ü�ŵ��ڱ�ü��ݾ�,       djvalue,        djvalue,    long,   8;
        ��ü�ż��ڱ�ü��ݾ�,       sjvalue,        sjvalue,    long,   8;
        ��ü�ڱ���ż� �ݾ�,        jval,           jval,       long,   8;

        KOSDAQ50 ����,              k50jisu,        k50jisu,    float,  6.2;
        KOSDAQ50 ���ϴ�񱸺�,      k50sign,        k50sign,    char,   1;
        KOSDAQ50 ���ϴ��,          change,         change,     float,  6.2;
        KOSDAQ50 ���̽ý�,          k50basis,       k50basis,   float,  4.2;

        ���͸ŵ�ü������հ�,       cdvolume,       cdvolume,   long,   6;
        ���͸ż�ü������հ�,       csvolume,       csvolume,   long,   6;
        ���ͼ��ż� �����հ�,        cvol,           cvol,       long,   6;

        �����͸ŵ�ü������հ�,     bdvolume,       bdvolume,   long,   6;
        �����͸ż�ü������հ�,     bsvolume,       bsvolume,   long,   6;
        �����ͼ��ż� �����հ�,      bvol,           bvol,       long,   6;

        ��ü�ŵ�ü������հ�,       tdvolume,       tdvolume,   long,   6;
        ��ü�ż�ü������հ�,       tsvolume,       tsvolume,   long,   6;
        ��ü���ż� �����հ�,        tvol,           tvol,       long,   6;

        ���͸ŵ�ü��ݾ��հ�,       cdvalue,        cdvalue,    long,   8;
        ���͸ż�ü��ݾ��հ�,       csvalue,        csvalue,    long,   8;
        ���ͼ��ż� �ݾ��հ�,        cval,           cval,       long,   8;

        �����͸ŵ�ü��ݾ��հ�,     bdvalue,        bdvalue,    long,   8;
        �����͸ż�ü��ݾ��հ�,     bsvalue,        bsvalue,    long,   8;
        �����ͼ��ż� �ݾ��հ�,      bval,           bval,       long,   8;

        ��ü�ŵ�ü��ݾ��հ�,       tdvalue,        tdvalue,    long,   8;
        ��ü�ż�ü��ݾ��հ�,       tsvalue,        tsvalue,    long,   8;
        ��ü���ż� �ݾ��հ�,        tval,           tval,       long,   8;

        ���͸ŵ�ü������������,   p_cdvolcha,     p_cdvolcha, long,   6;
        ���͸ż�ü������������,   p_csvolcha,     p_csvolcha, long,   6;
        ���ͼ��ż� �����������,    p_cvolcha,      p_cvolcha,  long,   6;

        �����͸ŵ�ü������������, p_bdvolcha,     p_bdvolcha, long,   6;
        �����͸ż�ü������������, p_bsvolcha,     p_bsvolcha, long,   6;
        �����ͼ��ż� �����������,  p_bvolcha,      p_bvolcha,  long,   6;

        ��ü�ŵ�ü������������,   p_tdvolcha,     p_tdvolcha, long,   6;
        ��ü�ż�ü������������,   p_tsvolcha,     p_tsvolcha, long,   6;
        ��ü���ż� �����������,    p_tvolcha,      p_tvolcha,  long,   6;

        ���͸ŵ�ü��ݾ��������,   p_cdvalcha,     p_cdvalcha, long,   8;
        ���͸ż�ü��ݾ��������,   p_csvalcha,     p_csvalcha, long,   8;
        ���ͼ��ż� �ݾ��������,    p_cvalcha,      p_cvalcha,  long,   8;

        �����͸ŵ�ü��ݾ��������, p_bdvalcha,     p_bdvalcha, long,   8;
        �����͸ż�ü��ݾ��������, p_bsvalcha,     p_bsvalcha, long,   8;
        �����ͼ��ż� �ݾ��������,  p_bvalcha,      p_bvalcha,  long,   8;

        ��ü�ŵ�ü��ݾ��������,   p_tdvalcha,     p_tdvalcha, long,   8;
        ��ü�ż�ü��ݾ��������,   p_tsvalcha,     p_tsvalcha, long,   8;
        ��ü���ż� �ݾ��������,    p_tvalcha,      p_tvalcha,  long,   8;
        
		���а�,                     gubun,          gubun,      char,   1;
    end
    END_DATA_MAP
END_FUNCTION_MAP
