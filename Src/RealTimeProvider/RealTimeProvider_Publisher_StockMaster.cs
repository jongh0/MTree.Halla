using Configuration;
using DataStructure;
using CommonLib;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RealTimeProvider
{
    public partial class RealTimeProvider_
    {
        private readonly object _masteringLock = new object();
        private readonly object _daishinMasteringLock = new object();
        private readonly object _ebestMasteringLock = new object();
        private readonly object _kiwoomMasteringLock = new object();

        private void StartStockMastering()
        {
            RealTimeState = "Stock mastering started";
            _logger.Info(RealTimeState);

            bool masteringRet = false;

            Stopwatch sw = new Stopwatch();
            sw.Start();

            try
            {
                var now = DateTime.Now;

                foreach (var codeEntity in StockCodeList.Values)
                {
                    var mastering = new StockMastering();
                    mastering.Stock.Code = codeEntity.Code;
                    mastering.Stock.Name = codeEntity.Name;
                    mastering.Stock.MarketType = codeEntity.MarketType;
                    mastering.Stock.Time = new DateTime(now.Year, now.Month, now.Day); // 날짜까지만 사용한다

                    StockMasteringList.Add(mastering);
                }

                var masteringTask = new List<Task>();
                masteringTask.Add(Task.Run(() => StartDaishinStockMastering()));
                if (Config.General.ExcludeEbest == false)
                    masteringTask.Add(Task.Run(() => StartEbestStockMatering()));
                if (Config.General.ExcludeKiwoom == false)
                    masteringTask.Add(Task.Run(() => StartKiwoomStockMastering()));

                masteringRet = Task.WaitAll(masteringTask.ToArray(), TimeSpan.FromMinutes(30));
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            finally
            {
                sw.Stop();

                if (masteringRet == true)
                {
                    RealTimeState = "Stock mastering success";
                    _logger.Info($"{RealTimeState}, Elapsed time: {sw.Elapsed.ToString()}");    
                }
                else
                {
                    RealTimeState = "Stock mastering failed";
                    _logger.Info(RealTimeState);
                }

                Task.Run(() => StartStockMasterPublishing()).ContinueWith((x) => MasteringDone = true);
            }
        }

        private void StartStockMasterPublishing()
        {
            RealTimeState = "Stock master publishing started";
            _logger.Info(RealTimeState);

            Stopwatch sw = new Stopwatch();
            sw.Start();

            var stockMasterList = new List<StockMaster>();

            try
            {
                foreach (var mastering in StockMasteringList)
                {
                    stockMasterList.Add(mastering.Stock);
                }

                Counter.StockMasterCount = stockMasterList.Count;

                foreach (var contract in MasteringContracts.Values)
                {
                    try
                    {
                        contract.Callback.ConsumeStockMaster(stockMasterList);
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            finally
            {
                stockMasterList.Clear();
                //StockMasteringList.Clear();

                sw.Stop();

                RealTimeState = "Stock master publishing done";
                _logger.Info($"{RealTimeState}, Elapsed time: {sw.Elapsed.ToString()}");
            }
        }
        private void StartCodeMapPublishing(Dictionary<string, object> codemap)
        {
            RealTimeState = "Codemap publishing started";
            _logger.Info(RealTimeState);

            try
            {
                foreach (var contract in MasteringContracts.Values)
                {
                    try
                    {
                        contract.Callback.ConsumeCodeMap(codemap);
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            finally
            {
                RealTimeState = "Codemap publishing done";
                _logger.Info(RealTimeState);
            }
        }
        private void StartDaishinStockMastering()
        {
            RealTimeState = "Daishin stock mastering started";
            _logger.Info(RealTimeState);

            Stopwatch sw = new Stopwatch();
            sw.Start();

            try
            {
                while (true)
                {
                    lock (_daishinMasteringLock)
                    {
                        var totalCount = StockMasteringList.Count;
                        var notFinishedCount = StockMasteringList.Count(m => m.DaishinState != MasteringStates.Finished);
                        RealTimeState = $"Daishin stock mastering ({totalCount - notFinishedCount}/{totalCount})";

                        if (notFinishedCount == 0)
                            break;
                    }

                    StockMastering mastering = null;

                    lock (_daishinMasteringLock)
                    {
                        mastering = StockMasteringList.FirstOrDefault(m => m.DaishinState == MasteringStates.Ready);
                        if (mastering != null)
                            mastering.DaishinState = MasteringStates.Running;
                    }
                    
                    if (mastering != null)
                    {
                        PublisherContract contract = null;

                        while (true)
                        {
                            lock (_daishinMasteringLock)
                            {
                                contract = DaishinContractForMastering;
                                if (contract != null)
                                {
                                    contract.IsOperating = true;
                                    break;
                                }
                            }
                            
                            Thread.Sleep(100);
                        }

                        Task.Run(() => GetStockMaster(mastering, contract));
                    }
                    else
                    {
                        Thread.Sleep(100);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            finally
            {
                sw.Stop();

                RealTimeState = "Daishin stock mastering done";
                _logger.Info($"{RealTimeState}, Elapsed time: {sw.Elapsed.ToString()}");
            }
        }

        private void StartEbestStockMatering()
        {
            RealTimeState = "Ebest stock mastering started";
            _logger.Info(RealTimeState);

            Stopwatch sw = new Stopwatch();
            sw.Start();

            try
            {
                while (true)
                {
                    lock (_ebestMasteringLock)
                    {
                        var totalCount = StockMasteringList.Count;
                        var notFinishedCount = StockMasteringList.Count(m => m.EbestState != MasteringStates.Finished);
                        RealTimeState = $"Ebest stock mastering ({totalCount - notFinishedCount}/{totalCount})";

                        if (notFinishedCount == 0)
                            break;
                    }

                    StockMastering mastering = null;

                    lock (_ebestMasteringLock)
                    {
                        mastering = StockMasteringList.FirstOrDefault(m => m.EbestState == MasteringStates.Ready);
                        if (mastering != null)
                            mastering.EbestState = MasteringStates.Running;
                    }

                    if (mastering != null)
                    {
                        PublisherContract contract = null;

                        while (true)
                        {
                            lock (_ebestMasteringLock)
                            {
                                contract = EbestContractForMastering;
                                if (contract != null)
                                {
                                    contract.IsOperating = true;
                                    break;
                                }
                            }

                            Thread.Sleep(100);
                        }

                        Task.Run(() => GetStockMaster(mastering, contract));
                    }
                    else
                    {
                        Thread.Sleep(100);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            finally
            {
                sw.Stop();

                RealTimeState = "Ebest stock mastering done";
                _logger.Info($"{RealTimeState}, Elapsed time: {sw.Elapsed.ToString()}");
            }
        }

        private void StartKiwoomStockMastering()
        {
            RealTimeState = "Kiwoom stock mastering started";
            _logger.Info(RealTimeState);

            Stopwatch sw = new Stopwatch();
            sw.Start();

            try
            {
                while (true)
                {
                    lock (_kiwoomMasteringLock)
                    {
                        var totalCount = StockMasteringList.Count;
                        var notFinishedCount = StockMasteringList.Count(m => m.KiwoomState != MasteringStates.Finished);
                        RealTimeState = $"Kiwoom stock mastering ({totalCount - notFinishedCount}/{totalCount})";

                        if (notFinishedCount == 0)
                            break;
                    }

                    StockMastering mastering = null;

                    lock (_kiwoomMasteringLock)
                    {
                        mastering = StockMasteringList.FirstOrDefault(m => m.KiwoomState == MasteringStates.Ready);
                        if (mastering != null)
                            mastering.KiwoomState = MasteringStates.Running;
                    }

                    if (mastering != null)
                    {
                        PublisherContract contract = null;

                        while (true)
                        {
                            lock (_kiwoomMasteringLock)
                            {
                                contract = KiwoomContractForMastering;
                                if (contract != null)
                                {
                                    contract.IsOperating = true;
                                    break;
                                }
                            }

                            Thread.Sleep(10);
                        }

                        Task.Run(() => GetStockMaster(mastering, contract));
                    }
                    else
                    {
                        Thread.Sleep(100);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            finally
            {
                sw.Stop();

                RealTimeState = "Kiwoom stock mastering done";
                _logger.Info($"{RealTimeState}, Elapsed time: {sw.Elapsed.ToString()}");
            }
        }

        private void GetStockMaster(StockMastering mastering, PublisherContract contract)
        {
            try
            {
                var codeEntity = StockCodeList[mastering.Stock.Code];
                var code = codeEntity.Code;

                if (contract.Type == ProcessTypes.DaishinPublisher)
                    code = CodeEntity.ConvertToDaishinCode(codeEntity);
                
                StockMaster master = contract.Callback.GetStockMaster(code);

                if (contract.Type == ProcessTypes.DaishinPublisher)
                    CopyStockMasterFromDaishin(mastering, master);
                else if (contract.Type == ProcessTypes.EbestPublisher)
                    CopyStockMasterFromEbest(mastering, master);
                else if (contract.Type == ProcessTypes.KiwoomPublisher)
                    CopyStockMasterFromKiwoom(mastering, master);
                else
                    _logger.Warn("Wrong contract type for stock mastering");
            }
            catch (Exception ex)
            {
                _logger.Error($"Stock mastering error, {contract.ToString()}");
                _logger.Error(ex);
            }
            finally
            {
                if (contract.Type == ProcessTypes.DaishinPublisher)
                {
                    lock (_daishinMasteringLock)
                        contract.IsOperating = false;
                }
                else if (contract.Type == ProcessTypes.EbestPublisher)
                {
                    lock (_ebestMasteringLock)
                        contract.IsOperating = false;
                }
                else if (contract.Type == ProcessTypes.KiwoomPublisher)
                {
                    lock (_kiwoomMasteringLock)
                        contract.IsOperating = false;
                }
            }
        }

        private void CopyStockMasterFromDaishin(StockMastering mastering, StockMaster source)
        {
            var state = MasteringStates.Ready;

            try
            {
                if (source != null)
                {
                    StockMaster dest = mastering.Stock;

                    if (source.Code != dest.Code)
                    {
                        _logger.Warn($"Daishin stock mastering, Code not matched, {source.Code} != {dest.Code}");
                        return;
                    }

                    //dest.Name = source.Name;
                    dest.UpperLimit = source.UpperLimit;
                    dest.LowerLimit = source.LowerLimit;
                    dest.PreviousClosingPrice = source.PreviousClosingPrice;
                    dest.SettlementMonth = source.SettlementMonth;
                    dest.BasisPrice = source.BasisPrice;
                    dest.ShareVolume = source.ShareVolume;
                    dest.ListedCapital = source.ListedCapital;
                    dest.ForeigneLimit = source.ForeigneLimit;
                    dest.ForeigneAvailableRemain = source.ForeigneAvailableRemain;
                    dest.QuantityUnit = source.QuantityUnit;
                    dest.PreviousVolume = source.PreviousVolume;
                    dest.FaceValue = source.FaceValue;

                    if (dest.BasisPrice != 0 && dest.PBR != 0 && dest.ShareVolume != 0)
                        dest.Asset = (dest.BasisPrice / dest.PBR) * dest.ShareVolume;

                    if (dest.BasisPrice != 0 && dest.PER != 0 && dest.ShareVolume != 0)
                        dest.NetIncome = (dest.BasisPrice / dest.PER) * dest.ShareVolume;

                    state = MasteringStates.Finished;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            finally
            {
                lock (_daishinMasteringLock)
                    mastering.DaishinState = state;
            }
        }

        private void CopyStockMasterFromEbest(StockMastering mastering, StockMaster source)
        {
            var state = MasteringStates.Ready;

            try
            {
                if (source != null)
                {
                    StockMaster dest = mastering.Stock;

                    if (source.Code != dest.Code)
                    {
                        _logger.Warn($"Ebest stock mastering, Code not matched, {source.Code} != {dest.Code}");
                        return;
                    }

                    dest.TradingSuspend = source.TradingHalt;
                    dest.AdministrativeIssue = source.AdministrativeIssue;
                    dest.UnfairAnnouncement = source.UnfairAnnouncement;
                    dest.InvestAttention = source.InvestAttention;
                    dest.CallingAttention = source.CallingAttention;
                    dest.InvestWarning = source.InvestWarning;
                    dest.TradingHalt = source.TradingHalt;
                    dest.CleaningTrade = source.CleaningTrade;
                    dest.InvestCaution = source.InvestCaution;
                    dest.InvestRisk = source.InvestRisk;
                    dest.InvestRiskNoticed = source.InvestRiskNoticed;
                    dest.Overheated = source.Overheated;
                    dest.OverheatNoticed = source.OverheatNoticed;

                    dest.ListedDate = source.ListedDate;
                    
                    dest.CirculatingVolume = source.CirculatingVolume;
                    dest.ValueAlteredType = source.ValueAlteredType;

                    state = MasteringStates.Finished;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            finally
            {
                lock (_ebestMasteringLock)
                    mastering.EbestState = state;
            }
        }

        private void CopyStockMasterFromKiwoom(StockMastering mastering, StockMaster source)
        {
            var state = MasteringStates.Ready;

            try
            {
                if (source != null)
                {
                    StockMaster dest = mastering.Stock;

                    if (source.Code != dest.Code)
                    {
                        _logger.Warn($"Kiwoom stock mastering, Code not matched, {source.Code} != {dest.Code}");
                        return;
                    }

                    dest.PER = source.PER;
                    dest.EPS = source.EPS;
                    dest.PBR = source.PBR;
                    dest.BPS = source.BPS;
                    dest.ROE = source.ROE;
                    dest.EV = source.EV;

                    if (dest.BasisPrice != 0 && dest.PBR != 0 && dest.ShareVolume != 0)
                        dest.Asset = (dest.BasisPrice / dest.PBR) * dest.ShareVolume;

                    if (dest.BasisPrice != 0 && dest.PER != 0 && dest.ShareVolume != 0)
                        dest.NetIncome = (dest.BasisPrice / dest.PER) * dest.ShareVolume;

                    state = MasteringStates.Finished;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            finally
            {
                lock (_kiwoomMasteringLock)
                    mastering.KiwoomState = state;
            }
        }
    }
}
