﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.DataExtractingConsumer
{
    public enum ExtractTypes
    {
        Stock,
        Index,
    }

    public enum StockConclusionField
    {
        Time,
        Amount,
        MarketTimeType,
        Price,
        ConclusionType,
    }

    public enum StockMasterField
    {
        MarketType,
        SettlementMonth,
        FaceValue,
        ListedDate,
        CirculatingVolume,
        ShareVolume,
        ListedCapital,
        BasisPrice,
        UpperLimit,
        LowerLimit,
        PreviousClosingPrice,
        PreviousVolume,
        QuantityUnit,
        ForeigneLimit,
        ForeigneAvailableRemain,
        ForeigneLimitRate,
        ForeigneHold,
        ForeigneExhaustingRate,
        ForeigneAvailableRemainRate,
        AdministrativeIssue,
        InvestCaution,
        InvestWarning,
        InvestRiskNoticed,
        InvestRisk,
        TradingHalt,
        TradingSuspend,
        UnfairAnnouncement,
        Overheated,
        OverheatNoticed,
        CleaningTrade,
        InvestAttention,
        CallingAttention,
        Asset,
        BPS,
        PBR,
        NetIncome,
        EPS,
        PER,
        ROE,
        EV,
        ValueAlteredType,
    }

    public enum StockTAField
    {
        MovingAverage_Tick_5_Sma,
        MovingAverage_Tick_5_Ema,
        MovingAverage_Tick_5_Wma,
        MovingAverage_Tick_5_Dema,
        MovingAverage_Tick_5_Tema,
        MovingAverage_Tick_5_Trima,
        MovingAverage_Tick_5_Kama,
        MovingAverage_Tick_5_Mama,
        MovingAverage_Tick_5_T3,
        MovingAverage_Tick_10_Sma,
        MovingAverage_Tick_10_Ema,
        MovingAverage_Tick_10_Wma,
        MovingAverage_Tick_10_Dema,
        MovingAverage_Tick_10_Tema,
        MovingAverage_Tick_10_Trima,
        MovingAverage_Tick_10_Kama,
        MovingAverage_Tick_10_Mama,
        MovingAverage_Tick_10_T3,
        MovingAverage_Tick_20_Sma,
        MovingAverage_Tick_20_Ema,
        MovingAverage_Tick_20_Wma,
        MovingAverage_Tick_20_Dema,
        MovingAverage_Tick_20_Tema,
        MovingAverage_Tick_20_Trima,
        MovingAverage_Tick_20_Kama,
        MovingAverage_Tick_20_Mama,
        MovingAverage_Tick_20_T3,
        MovingAverage_Day_5_Sma,
        MovingAverage_Day_5_Ema,
        MovingAverage_Day_5_Wma,
        MovingAverage_Day_5_Dema,
        MovingAverage_Day_5_Tema,
        MovingAverage_Day_5_Trima,
        MovingAverage_Day_5_Kama,
        MovingAverage_Day_5_Mama,
        MovingAverage_Day_5_T3,
        MovingAverage_Day_10_Sma,
        MovingAverage_Day_10_Ema,
        MovingAverage_Day_10_Wma,
        MovingAverage_Day_10_Dema,
        MovingAverage_Day_10_Tema,
        MovingAverage_Day_10_Trima,
        MovingAverage_Day_10_Kama,
        MovingAverage_Day_10_Mama,
        MovingAverage_Day_10_T3,
        MovingAverage_Day_20_Sma,
        MovingAverage_Day_20_Ema,
        MovingAverage_Day_20_Wma,
        MovingAverage_Day_20_Dema,
        MovingAverage_Day_20_Tema,
        MovingAverage_Day_20_Trima,
        MovingAverage_Day_20_Kama,
        MovingAverage_Day_20_Mama,
        MovingAverage_Day_20_T3,
        AccumulationDistributionLine_Day,
        AccumulationDistributionOscillator_Day_3_10,
        AccumulationDistributionOscillator_Day_6_20,
        AbsolutePriceOscillator_Day_3_10_Sma,
        AbsolutePriceOscillator_Day_3_10_Ema,
        AbsolutePriceOscillator_Day_3_10_Wma,
        AbsolutePriceOscillator_Day_3_10_Dema,
        AbsolutePriceOscillator_Day_3_10_Tema,
        AbsolutePriceOscillator_Day_3_10_Trima,
        AbsolutePriceOscillator_Day_3_10_Kama,
        AbsolutePriceOscillator_Day_3_10_Mama,
        AbsolutePriceOscillator_Day_3_10_T3,
        AbsolutePriceOscillator_Day_6_20_Sma,
        AbsolutePriceOscillator_Day_6_20_Ema,
        AbsolutePriceOscillator_Day_6_20_Wma,
        AbsolutePriceOscillator_Day_6_20_Dema,
        AbsolutePriceOscillator_Day_6_20_Tema,
        AbsolutePriceOscillator_Day_6_20_Trima,
        AbsolutePriceOscillator_Day_6_20_Kama,
        AbsolutePriceOscillator_Day_6_20_Mama,
        AbsolutePriceOscillator_Day_6_20_T3,
        Aroon_Day_25,
        Aroon_Day_50,
        Aroon_Day_100,
        AroonOsc_Day_25,
        AroonOsc_Day_50,
        AroonOsc_Day_100,
        AverageTrueRange_Day_8,
        AverageTrueRange_Day_14,
        AverageTrueRange_Day_21,
        AverageTrueRange_Day_50,
        AverageDirectionalIndexRating_Day_8,
        AverageDirectionalIndexRating_Day_14,
        AverageDirectionalIndexRating_Day_21,
        AverageDirectionalIndexRating_Day_50,
        AverageDirectionalIndex_Day_8,
        MinusDI_Day_8,
        PlusDI_Day_8,
        MinusDM_Day_8,
        PlusDM_Day_8,
        AverageDirectionalIndex_Day_14,
        MinusDI_Day_14,
        PlusDI_Day_14,
        MinusDM_Day_14,
        PlusDM_Day_14,
        AverageDirectionalIndex_Day_21,
        MinusDI_Day_21,
        PlusDI_Day_21,
        MinusDM_Day_21,
        PlusDM_Day_21,
        AverageDirectionalIndex_Day_50,
        MinusDI_Day_50,
        PlusDI_Day_50,
        MinusDM_Day_50,
        PlusDM_Day_50,
        BollingerBands_Day_10_2_Sma,
        BollingerBands_Day_10_2_Ema,
        BollingerBands_Day_10_2_Wma,
        BollingerBands_Day_10_2_Dema,
        BollingerBands_Day_10_2_Tema,
        BollingerBands_Day_10_2_Trima,
        BollingerBands_Day_10_2_Kama,
        BollingerBands_Day_10_2_Mama,
        BollingerBands_Day_10_2_T3,
        BollingerBands_Day_10_3_Sma,
        BollingerBands_Day_10_3_Ema,
        BollingerBands_Day_10_3_Wma,
        BollingerBands_Day_10_3_Dema,
        BollingerBands_Day_10_3_Tema,
        BollingerBands_Day_10_3_Trima,
        BollingerBands_Day_10_3_Kama,
        BollingerBands_Day_10_3_Mama,
        BollingerBands_Day_10_3_T3,
        BollingerBands_Day_20_2_Sma,
        BollingerBands_Day_20_2_Ema,
        BollingerBands_Day_20_2_Wma,
        BollingerBands_Day_20_2_Dema,
        BollingerBands_Day_20_2_Tema,
        BollingerBands_Day_20_2_Trima,
        BollingerBands_Day_20_2_Kama,
        BollingerBands_Day_20_2_Mama,
        BollingerBands_Day_20_2_T3,
        BollingerBands_Day_20_3_Sma,
        BollingerBands_Day_20_3_Ema,
        BollingerBands_Day_20_3_Wma,
        BollingerBands_Day_20_3_Dema,
        BollingerBands_Day_20_3_Tema,
        BollingerBands_Day_20_3_Trima,
        BollingerBands_Day_20_3_Kama,
        BollingerBands_Day_20_3_Mama,
        BollingerBands_Day_20_3_T3,
        BollingerBands_Day_50_2_Sma,
        BollingerBands_Day_50_2_Ema,
        BollingerBands_Day_50_2_Wma,
        BollingerBands_Day_50_2_Dema,
        BollingerBands_Day_50_2_Tema,
        BollingerBands_Day_50_2_Trima,
        BollingerBands_Day_50_2_Kama,
        BollingerBands_Day_50_2_Mama,
        BollingerBands_Day_50_3_Sma,
        BollingerBands_Day_50_3_Ema,
        BollingerBands_Day_50_3_Wma,
        BollingerBands_Day_50_3_Dema,
        BollingerBands_Day_50_3_Tema,
        BollingerBands_Day_50_3_Trima,
        BollingerBands_Day_50_3_Kama,
        BollingerBands_Day_50_3_Mama,
        BalanceOfPower_Day,
        CommodityChannelIndex_Day_20,
        CommodityChannelIndex_Day_50,
        CommodityChannelIndex_Day_100,
        TwoCrows_Day,
        ThreeBlackCrows_Day,
        ThreeInsideUpDown_Day,
        ThreeLineStrike_Day,
        ThreeOutsideUpDown_Day,
        ThreeStarsInTheSouth_Day,
        ThreeAdvancingWhiteSoldiers_Day,
        AbandonedBaby_Day_30,
        AdvanceBlock_Day,
        BeltHold_Day,
        Breakaway_Day,
        ClosingMarubozu_Day,
        ConcealingBabySwallow_Day,
        Counterattack_Day,
        DarkCloudCover_Day_30,
        Doji_Day,
        DojiStar_Day,
        DragonflyDoji_Day,
        EngulfingPattern_Day,
        EveningDojiStar_Day_30,
        EveningStar_Day_30,
        UpDownGapSideBySideWhiteLines_Day,
        GravestoneDoji_Day,
        Hammer_Day,
        HangingMan_Day,
        HaramiPattern_Day,
        HaramiCrossPattern_Day,
        HighWaveCandle_Day,
        HikkakePattern_Day,
        ModifiedHikkakePattern_Day,
        HomingPigeon_Day,
        IdenticalThreeCrows_Day,
        InNeck_Day,
        InvertedHammer_Day,
        Kicking_Day,
        KickingByLenghth_Day,
        LadderBottom_Day,
        LongLeggedDoji_Day,
        LongLineCandle_Day,
        Marubozu_Day,
        MatchingLow_Day,
        MatHold_Day_30,
        MorningDojiStar_Day,
        MorningStar_Day,
        OnNeckPattern_Day,
        PiercingPattern_Day,
        RickshawMan_Day,
        RisingFallingThreeMethods_Day,
        SeparatingLines_Day,
        ShootingStar_Day,
        ShortLineCandle_Day,
        SpinningTop_Day,
        StalledPattern_Day,
        StickSandwich_Day,
        Takuri_Day,
        TasukiGap_Day,
        ThrustingPattern_Day,
        TristarPattern_Day,
        Unique3River_Day,
        UpsideGapTwoCrows_Day,
        UpsideDownsideGapThreeMethods_Day,
        ChandeMomentumOscillator_Day_10,
        ChandeMomentumOscillator_Day_20,
        ChandeMomentumOscillator_Day_40,
        HilbertTransformDominantCyclePeriod_Day,
        HilbertTransformDominantCyclePhase_Day,
        HilbertTransformPhasorComponents_Day,
        HilbertTransformSineWave_Day,
        HilbertTransformInstantaneousTrendline_Day,
        HilbertTransformTrendVsCycleMode_Day,
        LinearRegression_Day_20,
        LinearRegression_Day_60,
        LinearRegression_Day_100,
        LinearRegressionAngle_Day_20,
        LinearRegressionAngle_Day_60,
        LinearRegressionAngle_Day_100,
        LinearRegressionIntercept_Day_20,
        LinearRegressionIntercept_Day_60,
        LinearRegressionIntercept_Day_100,
        LinearRegressionSlope_Day_20,
        LinearRegressionSlope_Day_60,
        LinearRegressionSlope_Day_100,
        MovingAverageConvergenceDivergence_Day_12_26_9,
        MovingAverageConvergenceDivergence_Day_21_55_9,
        MacdWithMAType_Day_12_26_9_Sma,
        MacdWithMAType_Day_12_26_9_Ema,
        MacdWithMAType_Day_12_26_9_Wma,
        MacdWithMAType_Day_12_26_9_Dema,
        MacdWithMAType_Day_12_26_9_Tema,
        MacdWithMAType_Day_12_26_9_Trima,
        MacdWithMAType_Day_12_26_9_Kama,
        MacdWithMAType_Day_12_26_9_T3,
        MacdWithMAType_Day_21_55_9_Sma,
        MacdWithMAType_Day_21_55_9_Ema,
        MacdWithMAType_Day_21_55_9_Wma,
        MacdWithMAType_Day_21_55_9_Dema,
        MacdWithMAType_Day_21_55_9_Tema,
        MacdWithMAType_Day_21_55_9_Trima,
        MacdWithMAType_Day_21_55_9_Kama,
        Max_Day_5,
        Max_Day_10,
        Max_Day_20,
        Max_Day_50,
        Max_Day_100,
        Max_Day_200,
        MaxIndex_Day_5,
        MaxIndex_Day_10,
        MaxIndex_Day_20,
        MaxIndex_Day_50,
        MaxIndex_Day_100,
        MaxIndex_Day_200,
        MedPrice_Day,
        MoneyFlowIndex_Day_5,
        MoneyFlowIndex_Day_10,
        MoneyFlowIndex_Day_14,
        MoneyFlowIndex_Day_21,
        MoneyFlowIndex_Day_30,
        MidPoint_Day_5,
        MidPoint_Day_10,
        MidPoint_Day_20,
        MidPoint_Day_50,
        MidPoint_Day_100,
        MidPoint_Day_200,
        MidPrice_Day_5,
        MidPrice_Day_10,
        MidPrice_Day_20,
        MidPrice_Day_50,
        MidPrice_Day_100,
        MidPrice_Day_200,
        Min_Day_5,
        Min_Day_10,
        Min_Day_20,
        Min_Day_50,
        Min_Day_100,
        Min_Day_200,
        MinIndex_Day_5,
        MinIndex_Day_10,
        MinIndex_Day_20,
        MinIndex_Day_50,
        MinIndex_Day_100,
        MinIndex_Day_200,
        Momentum_Day_5,
        Momentum_Day_10,
        Momentum_Day_20,
        Momentum_Day_50,
        NormalizedAverageTrueRange_Day_8,
        NormalizedAverageTrueRange_Day_14,
        NormalizedAverageTrueRange_Day_21,
        NormalizedAverageTrueRange_Day_50,
        OnBalanceVolume_Day,
        PercentagePriceOscillator_Day_12_26_Sma,
        PercentagePriceOscillator_Day_12_26_Ema,
        PercentagePriceOscillator_Day_12_26_Wma,
        PercentagePriceOscillator_Day_12_26_Dema,
        PercentagePriceOscillator_Day_12_26_Tema,
        PercentagePriceOscillator_Day_12_26_Trima,
        PercentagePriceOscillator_Day_12_26_Kama,
        PercentagePriceOscillator_Day_12_26_T3,
        PercentagePriceOscillator_Day_21_55_Sma,
        PercentagePriceOscillator_Day_21_55_Ema,
        PercentagePriceOscillator_Day_21_55_Wma,
        PercentagePriceOscillator_Day_21_55_Dema,
        PercentagePriceOscillator_Day_21_55_Tema,
        PercentagePriceOscillator_Day_21_55_Trima,
        PercentagePriceOscillator_Day_21_55_Kama,
        RelativeStrengthIndex_Day_5,
        RelativeStrengthIndex_Day_10,
        RelativeStrengthIndex_Day_14,
        RelativeStrengthIndex_Day_20,
        RelativeStrengthIndex_Day_50,
        ParabolicSAR_Day_01_10,
        ParabolicSAR_Day_01_20,
        ParabolicSAR_Day_01_30,
        ParabolicSAR_Day_03_10,
        ParabolicSAR_Day_03_20,
        ParabolicSAR_Day_03_30,
        ParabolicSAR_Day_05_10,
        ParabolicSAR_Day_05_20,
        ParabolicSAR_Day_05_30,
        ParabolicSARExt_Day_0_0_20_20_200_20_20_200,
        ParabolicSARExt_Day_0_0_20_20_200_5_5_50,
        ParabolicSARExt_Day_0_0_5_5_50_20_20_200,
        StandardDeviation_Day_10_2,
        StandardDeviation_Day_10_3,
        StandardDeviation_Day_10_4,
        StandardDeviation_Day_21_2,
        StandardDeviation_Day_21_3,
        StandardDeviation_Day_21_4,
        StandardDeviation_Day_63_2,
        StandardDeviation_Day_63_3,
        StandardDeviation_Day_63_4,
        Stochastic_Day_14_3_Sma_3_Sma,
        Stochastic_Day_21_3_Sma_3_Sma,
        Stochastic_Day_14_9_Sma_9_Sma,
        Stochastic_Day_21_9_Sma_9_Sma,
        Stochastic_Day_14_3_Ema_3_Ema,
        Stochastic_Day_21_3_Ema_3_Ema,
        Stochastic_Day_14_9_Ema_9_Ema,
        Stochastic_Day_21_9_Ema_9_Ema,
        Stochastic_Day_14_3_Wma_3_Wma,
        Stochastic_Day_21_3_Wma_3_Wma,
        Stochastic_Day_14_9_Wma_9_Wma,
        Stochastic_Day_21_9_Wma_9_Wma,
        Stochastic_Day_14_3_Dema_3_Dema,
        Stochastic_Day_21_3_Dema_3_Dema,
        Stochastic_Day_14_9_Dema_9_Dema,
        Stochastic_Day_21_9_Dema_9_Dema,
        Stochastic_Day_14_3_Tema_3_Tema,
        Stochastic_Day_21_3_Tema_3_Tema,
        Stochastic_Day_14_9_Tema_9_Tema,
        Stochastic_Day_21_9_Tema_9_Tema,
        StochasticFast_Day_14_3_Sma,
        StochasticFast_Day_21_3_Sma,
        StochasticFast_Day_14_9_Sma,
        StochasticFast_Day_21_9_Sma,
        StochasticFast_Day_14_3_Ema,
        StochasticFast_Day_21_3_Ema,
        StochasticFast_Day_14_9_Ema,
        StochasticFast_Day_21_9_Ema,
        StochasticFast_Day_14_3_Wma,
        StochasticFast_Day_21_3_Wma,
        StochasticFast_Day_14_9_Wma,
        StochasticFast_Day_21_9_Wma,
        StochasticFast_Day_14_3_Dema,
        StochasticFast_Day_21_3_Dema,
        StochasticFast_Day_14_9_Dema,
        StochasticFast_Day_21_9_Dema,
        StochasticFast_Day_14_3_Tema,
        StochasticFast_Day_21_3_Tema,
        StochasticFast_Day_14_9_Tema,
        StochasticFast_Day_21_9_Tema,
        StochasticRelativeStrengthIndex_Day_14_14_3_Sma,
        StochasticRelativeStrengthIndex_Day_21_21_3_Sma,
        StochasticRelativeStrengthIndex_Day_14_14_9_Sma,
        StochasticRelativeStrengthIndex_Day_21_21_9_Sma,
        StochasticRelativeStrengthIndex_Day_14_14_3_Ema,
        StochasticRelativeStrengthIndex_Day_21_21_3_Ema,
        StochasticRelativeStrengthIndex_Day_14_14_9_Ema,
        StochasticRelativeStrengthIndex_Day_21_21_9_Ema,
        StochasticRelativeStrengthIndex_Day_14_14_3_Wma,
        StochasticRelativeStrengthIndex_Day_21_21_3_Wma,
        StochasticRelativeStrengthIndex_Day_14_14_9_Wma,
        StochasticRelativeStrengthIndex_Day_21_21_9_Wma,
        StochasticRelativeStrengthIndex_Day_14_14_3_Dema,
        StochasticRelativeStrengthIndex_Day_21_21_3_Dema,
        StochasticRelativeStrengthIndex_Day_14_14_9_Dema,
        StochasticRelativeStrengthIndex_Day_21_21_9_Dema,
        StochasticRelativeStrengthIndex_Day_14_14_3_Tema,
        StochasticRelativeStrengthIndex_Day_21_21_3_Tema,
        StochasticRelativeStrengthIndex_Day_14_14_9_Tema,
        StochasticRelativeStrengthIndex_Day_21_21_9_Tema,
        TrueRange_Day,
        TripleSmoothEma_Day_9,
        TripleSmoothEma_Day_15,
        TripleSmoothEma_Day_21,
        TimeSeriesForecast_Day_9,
        TimeSeriesForecast_Day_15,
        TimeSeriesForecast_Day_21,
        TimeSeriesForecast_Day_63,
        UltimateOscillator_Day_4_8_16,
        UltimateOscillator_Day_7_14_28,
        Variance_Day_10_2,
        Variance_Day_10_3,
        Variance_Day_10_4,
        Variance_Day_21_2,
        Variance_Day_21_3,
        Variance_Day_21_4,
        Variance_Day_63_2,
        Variance_Day_63_3,
        Variance_Day_63_4,
        WeightedClosePrice_Day,
        WilliamsR_Day_9,
        WilliamsR_Day_14,
        WilliamsR_Day_28,
        WilliamsR_Day_56,
    }

    public enum IndexConclusionField
    {
        Time,
        Amount,
        MarketTimeType,
        Price,
        MarketCapitalization,
    }

    public enum IndexMasterField
    {
        BasisPrice,
    }

    public enum IndexTAField
    {
    }
}
