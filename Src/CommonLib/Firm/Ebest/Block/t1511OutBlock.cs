using CommonLib.Attribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Firm.Ebest.Block
{
    /// <summary>
    /// 업종현재가
    /// </summary>
    public class t1511OutBlock : BlockBase
    {
        /// <summary>
        /// 업종구분 [1]
        /// </summary>
        public string gubun { get; set; }

        /// <summary>
        /// 업종명 [20]
        /// </summary>
        public string hname { get; set; }

        /// <summary>
        /// 현재지수 [7.2]
        /// </summary>
        public float pricejisu { get; set; }

        /// <summary>
        /// 전일지수 [7.2]
        /// </summary>
        public float jniljisu { get; set; }

        /// <summary>
        /// 전일대비구분 [1]
        /// </summary>
        public string sign { get; set; }

        /// <summary>
        /// 전일대비 [7.2]
        /// </summary>
        public float change { get; set; }

        /// <summary>
        /// 지수등락율 [6.2]
        /// </summary>
        public float diffjisu { get; set; }

        /// <summary>
        /// 전일거래량 [12]
        /// </summary>
        public long jnilvolume { get; set; }

        /// <summary>
        /// 당일거래량 [12]
        /// </summary>
        public long volume { get; set; }

        /// <summary>
        /// 거래량전일대비 [12]
        /// </summary>
        public long volumechange { get; set; }

        /// <summary>
        /// 거래량비율 [6.2]
        /// </summary>
        public float volumerate { get; set; }

        /// <summary>
        /// 전일거래대금 [12]
        /// </summary>
        public long jnilvalue { get; set; }

        /// <summary>
        /// 당일거래대금 [12]
        /// </summary>
        public long value { get; set; }

        /// <summary>
        /// 거래대금전일대비 [12]
        /// </summary>
        public long valuechange { get; set; }

        /// <summary>
        /// 거래대금비율 [6.2]
        /// </summary>
        public float valuerate { get; set; }

        /// <summary>
        /// 시가지수 [7.2]
        /// </summary>
        public float openjisu { get; set; }

        /// <summary>
        /// 시가등락율 [6.2]
        /// </summary>
        public float opendiff { get; set; }

        /// <summary>
        /// 시가시간 [6]
        /// </summary>
        public string opentime { get; set; }

        /// <summary>
        /// 고가지수 [7.2]
        /// </summary>
        public float highjisu { get; set; }

        /// <summary>
        /// 고가등락율 [6.2]
        /// </summary>
        public float highdiff { get; set; }

        /// <summary>
        /// 고가시간 [6]
        /// </summary>
        public string hightime { get; set; }

        /// <summary>
        /// 저가지수 [7.2]
        /// </summary>
        public float lowjisu { get; set; }

        /// <summary>
        /// 저가등락율 [6.2]
        /// </summary>
        public float lowdiff { get; set; }

        /// <summary>
        /// 저가시간 [6]
        /// </summary>
        public string lowtime { get; set; }

        /// <summary>
        /// 52주최고지수 [7.2]
        /// </summary>
        public float whjisu { get; set; }

        /// <summary>
        /// 52주최고현재가대비 [7.2]
        /// </summary>
        public float whchange { get; set; }

        /// <summary>
        /// 52주최고지수일자 [8]
        /// </summary>
        public string whjday { get; set; }

        /// <summary>
        /// 52주최저지수 [7.2]
        /// </summary>
        public float wljisu { get; set; }

        /// <summary>
        /// 52주최저현재가대비 [7.2]
        /// </summary>
        public float wlchange { get; set; }

        /// <summary>
        /// 52주최저지수일자 [8]
        /// </summary>
        public string wljday { get; set; }

        /// <summary>
        /// 연중최고지수 [7.2]
        /// </summary>
        public float yhjisu { get; set; }

        /// <summary>
        /// 연중최고현재가대비 [7.2]
        /// </summary>
        public float yhchange { get; set; }

        /// <summary>
        /// 연중최고지수일자 [8]
        /// </summary>
        public string yhjday { get; set; }

        /// <summary>
        /// 연중최저지수 [7.2]
        /// </summary>
        public float yljisu { get; set; }

        /// <summary>
        /// 연중최저현재가대비 [7.2]
        /// </summary>
        public float ylchange { get; set; }

        /// <summary>
        /// 연중최저지수일자 [8]
        /// </summary>
        public string yljday { get; set; }

        /// <summary>
        /// 첫번째지수코드 [3]
        /// </summary>
        public string firstjcode { get; set; }

        /// <summary>
        /// 첫번째지수명 [20]
        /// </summary>
        public string firstjname { get; set; }

        /// <summary>
        /// 첫번째지수 [7.2]
        /// </summary>
        public float firstjisu { get; set; }

        /// <summary>
        /// 첫번째대비구분 [1]
        /// </summary>
        public string firsign { get; set; }

        /// <summary>
        /// 첫번째전일대비 [7.2]
        /// </summary>
        public float firchange { get; set; }

        /// <summary>
        /// 첫번째등락율 [6.2]
        /// </summary>
        public float firdiff { get; set; }

        /// <summary>
        /// 두번째지수코드 [3]
        /// </summary>
        public string secondjcode { get; set; }

        /// <summary>
        /// 두번째지수명 [20]
        /// </summary>
        public string secondjname { get; set; }

        /// <summary>
        /// 두번째지수 [7.2]
        /// </summary>
        public float secondjisu { get; set; }

        /// <summary>
        /// 두번째대비구분 [1]
        /// </summary>
        public string secsign { get; set; }

        /// <summary>
        /// 두번째전일대비 [7.2]
        /// </summary>
        public float secchange { get; set; }

        /// <summary>
        /// 두번째등락율 [6.2]
        /// </summary>
        public float secdiff { get; set; }

        /// <summary>
        /// 세번째지수코드 [3]
        /// </summary>
        public string thirdjcode { get; set; }

        /// <summary>
        /// 세번째지수명 [20]
        /// </summary>
        public string thirdjname { get; set; }

        /// <summary>
        /// 세번째지수 [7.2]
        /// </summary>
        public float thirdjisu { get; set; }

        /// <summary>
        /// 세번째대비구분 [1]
        /// </summary>
        public string thrsign { get; set; }

        /// <summary>
        /// 세번째전일대비 [7.2]
        /// </summary>
        public float thrchange { get; set; }

        /// <summary>
        /// 세번째등락율 [6.2]
        /// </summary>
        public float thrdiff { get; set; }

        /// <summary>
        /// 네번째지수코드 [3]
        /// </summary>
        public string fourthjcode { get; set; }

        /// <summary>
        /// 네번째지수명 [20]
        /// </summary>
        public string fourthjname { get; set; }

        /// <summary>
        /// 네번째지수 [7.2]
        /// </summary>
        public float fourthjisu { get; set; }

        /// <summary>
        /// 네번째대비구분 [1]
        /// </summary>
        public string forsign { get; set; }

        /// <summary>
        /// 네번째전일대비 [7.2]
        /// </summary>
        public float forchange { get; set; }

        /// <summary>
        /// 네번째등락율 [6.2]
        /// </summary>
        public float fordiff { get; set; }

        /// <summary>
        /// 상승종목수 [4]
        /// </summary>
        public long highjo { get; set; }

        /// <summary>
        /// 상한종목수 [4]
        /// </summary>
        public long upjo { get; set; }

        /// <summary>
        /// 보합종목수 [4]
        /// </summary>
        public long unchgjo { get; set; }

        /// <summary>
        /// 하락종목수 [4]
        /// </summary>
        public long lowjo { get; set; }

        /// <summary>
        /// 하한종목수 [4]
        /// </summary>
        public long downjo { get; set; }
    }
}
