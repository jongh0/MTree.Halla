BEGIN_FUNCTION_MAP
	.Func,개별계좌정보 조회,CACBQ21900,SERVICE=CACBQ21900,headtype=B,CREATOR=금미선,CREDATE=2012/04/19 14:46:14;
	BEGIN_DATA_MAP
	CACBQ21900InBlock1,In(*EMPTY*),input;
	begin
		레코드갯수, RecCnt, RecCnt, long, 5
		계좌번호, AcntNo, AcntNo, char, 20;
		입력비밀번호, InptPwd, InptPwd, char, 8;
	end
	CACBQ21900OutBlock1,In(*EMPTY*),output;
	begin
		레코드갯수, RecCnt, RecCnt, long, 5
		계좌번호, AcntNo, AcntNo, char, 20;
		입력비밀번호, InptPwd, InptPwd, char, 8;
	end
	CACBQ21900OutBlock2,Out(*EMPTY*),output;
	begin
		레코드갯수, RecCnt, RecCnt, long, 5
		상품상세코드, PrdtDtlCode, PrdtDtlCode, char, 3;
		상품상세명, PrdtDtlNm, PrdtDtlNm, char, 40;
		상위계좌번호, HirnkAcntNo, HirnkAcntNo, char, 20;
		계좌상태명, AcntStatNm, AcntStatNm, char, 40;
		계좌명, AcntNm, AcntNm, char, 40;
		계좌한글부기명, AcntHanglAlias, AcntHanglAlias, char, 40;
		관리사원번호, MgempNo, MgempNo, char, 9;
		관리사원명, MgempNm, MgempNm, char, 40;
		권유사원번호, CnvsEmpNo, CnvsEmpNo, char, 9;
		권유사원명, CnvsEmpNm, CnvsEmpNm, char, 40;
		관리지점명, MgmtBrnNm, MgmtBrnNm, char, 40;
		집계지점명, AgrgtBrnNm, AgrgtBrnNm, char, 40;
		실명확인방법코드명, RmnCnfMthdCodeNm, RmnCnfMthdCodeNm, char, 18;
		인터넷개설구분코드명, InetOpnTpCodeNm, InetOpnTpCodeNm, char, 20;
		접수자명, RcptPsnNm, RcptPsnNm, char, 40;
		계좌개설일, AcntOpnDt, AcntOpnDt, char, 8;
		최종거래일, LastTrdDt, LastTrdDt, char, 8;
		전입일, TrsfrDt, TrsfrDt, char, 8;
		전출일, BfOutDt, BfOutDt, char, 8;
		전입계좌번호, TrsfrAcntNo, TrsfrAcntNo, char, 20;
		전출계좌번호, BfOutAcntNo, BfOutAcntNo, char, 20;
		전입지점번호, TrsfrBrnNo, TrsfrBrnNo, char, 3;
		전출지점번호, TrnsfBrnNo, TrnsfBrnNo, char, 3;
		계좌통폐합일, AcntCncDt, AcntCncDt, char, 8;
		계좌통폐합해지일, AcntCncAbndDt, AcntCncAbndDt, char, 8;
		계좌해지일자, AcntAbndDt, AcntAbndDt, char, 8;
		잡수익처리일, MisincTrxDt, MisincTrxDt, char, 8;
		HTS등록일, HtsRegDt, HtsRegDt, char, 8;
		채무불이행상태, DebtUnMigrtStatNm, DebtUnMigrtStatNm, char, 40;
		통장개설일, PsbOpnDt, PsbOpnDt, char, 8;
		증거금징수유형명, MgnLevyPtnNm, MgnLevyPtnNm, char, 40;
		위탁투자자분류코드, CsgnInvstrClssCode, CsgnInvstrClssCode, char, 4;
		위탁투자자분류, CsgnInvstrClssCodeNm, CsgnInvstrClssCodeNm, char, 40;
		거래세과세, TrtaxTaxchrYnNm, TrtaxTaxchrYnNm, char, 40;
		특이구분, UnuslAcntClssNm, UnuslAcntClssNm, char, 40;
		주식채권구분, TrdSecPtnNm, TrdSecPtnNm, char, 40;
		수수료등급코드명, BnsCmsnAmtGrdCodeNm, BnsCmsnAmtGrdCodeNm, char, 40;
		수수료등급코드명, SettCmsnAmtGrdCodeNm, SettCmsnAmtGrdCodeNm, char, 40;
		종합매매약관확인여부, SyntrdStplCnfYn, SyntrdStplCnfYn, char, 1;
		거래구분명, TrdTpNm, TrdTpNm, char, 20;
		시장전달계좌번호, MktTransAcntNo, MktTransAcntNo, char, 20;
		위탁자기구분명, CsgnSfaccTpNm, CsgnSfaccTpNm, char, 40;
		등록시장코드, RegMktCode, RegMktCode, char, 2;
		등록시장명, RegMktNm, RegMktNm, char, 40;
		옵션수수료등급코드명, OptCmsnGrdCodeNm, OptCmsnGrdCodeNm, char, 40;
		선물수수료등급코드명, FutsCmsnGrdCodeNm, FutsCmsnGrdCodeNm, char, 40;
		FCM계좌번호, FcmAcntNo, FcmAcntNo, char, 20;
		근거계좌번호, GrndAcntNo, GrndAcntNo, char, 20;
		만기일자, DueDt, DueDt, char, 8;
		대체약정여부, BkeepCtrctYn, BkeepCtrctYn, char, 1;
		자동대체약정여부, AbkpCtrctYn, AbkpCtrctYn, char, 1;
		선물연계지정여부, FutsLnkDsgnYn, FutsLnkDsgnYn, char, 1;
		청약자금대출약정여부, AplLoanCtrctYn, AplLoanCtrctYn, char, 1;
		자동입금입고계좌지정여부, AutoInAcntDsgnYn, AutoInAcntDsgnYn, char, 1;
		기타자금대출약정여부, EmLoanCtrctYn, EmLoanCtrctYn, char, 1;
		매도대금담보대출약정여부, CslLoanCtrctYn, CslLoanCtrctYn, char, 1;
		매입자금대출약정여부, PmLoanCtrctYn, PmLoanCtrctYn, char, 1;
		신용계좌여부, CrdtAcntYn, CrdtAcntYn, char, 1;
		대출계좌종류구분코드, LoanAcntKindTpCode, LoanAcntKindTpCode, char, 1;
		인터넷개설구분코드, InetOpnTpCode, InetOpnTpCode, char, 1;
		사이버지점구분코드, CybBrnTpCode, CybBrnTpCode, char, 1;
	end
	END_DATA_MAP
END_FUNCTION_MAP
