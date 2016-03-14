using MTree.DataStructure;
using MTree.Utility;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MTree.RealTimeProvider
{
    public partial class RealTimeProvider
    {
        private object masteringLock = new object();
        private object daishinMasteringLock = new object();
        private object ebestMasteringLock = new object();
        private object kiwoomMasteringLock = new object();

        private void StartStockMastering()
        {
            logger.Info("Stock mastering started");

            Stopwatch sw = new Stopwatch();
            sw.Start();

            try
            {
                LaunchClientProcess();

                foreach (var codeEntity in StockCodeList.Values)
                {
                    var mastering = new StockMastering();
                    mastering.Stock.Code = codeEntity.Code;
                    mastering.Stock.Name = codeEntity.Name;
                    mastering.Stock.Time = DateTime.Now;

                    StockMasteringList.Add(mastering);
                }

                var masteringTask = new List<Task>();
                masteringTask.Add(Task.Run(() => StartKiwoomStockMastering()));
                masteringTask.Add(Task.Run(() => StartDaishinStockMastering()));
                masteringTask.Add(Task.Run(() => StartEbestStockMatering()));

                Task.WaitAll(masteringTask.ToArray());
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            finally
            {
                sw.Stop();
                logger.Info($"Stock mastering done, Elapsed time: {sw.Elapsed.ToString()}");

                Task.Run(() => StartStockMasterPublishing());
                Task.Run(() => StartCodeDistributing());
            }
        }

        private void StartStockMasterPublishing()
        {
            logger.Info("Stock master publishing started");

            Stopwatch sw = new Stopwatch();
            sw.Start();

            try
            {
                foreach (var mastering in StockMasteringList)
                {
                    foreach (var contract in StockMasterContracts)
                    {
                        try
                        {
                            contract.Value.Callback.ConsumeStockMaster(mastering.Stock);
                        }
                        catch (Exception ex)
                        {
                            logger.Error(ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            finally
            {
                sw.Stop();
                logger.Info($"Stock master publishing done, Elapsed time: {sw.Elapsed.ToString()}");
            }
        }

        private void StartDaishinStockMastering()
        {
            logger.Info("Daishin stock mastering started");

            Stopwatch sw = new Stopwatch();
            sw.Start();

            try
            {
                while (true)
                {
                    lock (masteringLock)
                    {
                        if (StockMasteringList.Count(m => m.DaishinState != MasteringStateType.Finished) == 0)
                            break;
                    }

                    StockMastering mastering = null;

                    lock (masteringLock)
                    {
                        mastering = StockMasteringList.FirstOrDefault(m => m.DaishinState == MasteringStateType.Ready);
                        if (mastering != null)
                            mastering.DaishinState = MasteringStateType.Running;
                    }
                    
                    if (mastering != null)
                    {
                        PublishContract contract = null;

                        while (true)
                        {
                            lock (daishinMasteringLock)
                            {
                                contract = DaishinContractForMastering;
                                if (contract != null)
                                {
                                    contract.NowOperating = true;
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
                logger.Error(ex);
            }
            finally
            {
                sw.Stop();
                logger.Info($"Daishin stock mastering done, Elapsed time: {sw.Elapsed.ToString()}");
            }
        }

        private void StartEbestStockMatering()
        {
            logger.Info("Ebest stock mastering started");

            Stopwatch sw = new Stopwatch();
            sw.Start();

            try
            {
                while (true)
                {
                    lock (masteringLock)
                    {
                        if (StockMasteringList.Count(m => m.EbestState != MasteringStateType.Finished) == 0)
                            break;
                    }

                    StockMastering mastering = null;

                    lock (masteringLock)
                    {
                        mastering = StockMasteringList.FirstOrDefault(m => m.EbestState == MasteringStateType.Ready);
                        if (mastering != null)
                            mastering.EbestState = MasteringStateType.Running;
                    }

                    if (mastering != null)
                    {
                        PublishContract contract = null;

                        while (true)
                        {
                            lock (ebestMasteringLock)
                            {
                                contract = EbestContractForMastering;
                                if (contract != null)
                                {
                                    contract.NowOperating = true;
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
                logger.Error(ex);
            }
            finally
            {
                sw.Stop();
                logger.Info($"Ebest stock mastering done, Elapsed time: {sw.Elapsed.ToString()}");
            }
        }

        private void StartKiwoomStockMastering()
        {
            logger.Info("Kiwoom stock mastering started");

            Stopwatch sw = new Stopwatch();
            sw.Start();

            try
            {
                while (true)
                {
                    lock (masteringLock)
                    {
                        if (StockMasteringList.Count(m => m.KiwoomState != MasteringStateType.Finished) == 0)
                            break;
                    }

                    StockMastering mastering = null;

                    lock (masteringLock)
                    {
                        mastering = StockMasteringList.FirstOrDefault(m => m.KiwoomState == MasteringStateType.Ready);
                        if (mastering != null)
                            mastering.KiwoomState = MasteringStateType.Running;
                    }

                    if (mastering != null)
                    {
                        PublishContract contract = null;

                        while (true)
                        {
                            lock (kiwoomMasteringLock)
                            {
                                contract = KiwoomContractForMastering;
                                if (contract != null)
                                {
                                    contract.NowOperating = true;
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
                logger.Error(ex);
            }
            finally
            {
                sw.Stop();
                logger.Info($"Kiwoom stock mastering done, Elapsed time: {sw.Elapsed.ToString()}");
            }
        }

        private void GetStockMaster(StockMastering mastering, PublishContract contract)
        {
            try
            {
                var codeEntity = StockCodeList[mastering.Stock.Code];
                var code = codeEntity.Code;

                if (contract.Type == ProcessType.Daishin)
                    code = CodeEntity.ConvertToDaishinCode(codeEntity);

                StockMaster master = contract.Callback.GetStockMaster(code);

                if (contract.Type == ProcessType.Daishin)
                    CopyStockMasterFromDaishin(mastering, master);
                else if (contract.Type == ProcessType.Ebest)
                    CopyStockMasterFromEbest(mastering, master);
                else if (contract.Type == ProcessType.Kiwoon)
                    CopyStockMasterFromKiwoom(mastering, master);
                else
                    logger.Warn("Wrong contract type for stock mastering");

#if false
                if (mastering.KiwoomState == MasteringStateType.Finished && 
                    mastering.EbestState == MasteringStateType.Finished && 
                    mastering.DaishinState == MasteringStateType.Finished)
                {
                    logger.Info(mastering.Stock.ToString());
                }
#endif
            }
            catch (Exception ex)
            {
                logger.Error($"Stock mastering error, {contract.ToString()}");
                logger.Error(ex);
            }
            finally
            {
                if (contract.Type == ProcessType.Daishin)
                {
                    lock (daishinMasteringLock)
                        contract.NowOperating = false;
                }
                else if (contract.Type == ProcessType.Ebest)
                {
                    lock (ebestMasteringLock)
                        contract.NowOperating = false;
                }
                else if (contract.Type == ProcessType.Kiwoon)
                {
                    lock (kiwoomMasteringLock)
                        contract.NowOperating = false;
                }
            }
        }

        private void CopyStockMasterFromDaishin(StockMastering mastering, StockMaster source)
        {
            var state = MasteringStateType.Ready;

            try
            {
                if (source != null)
                {
                    StockMaster dest = mastering.Stock;

                    if (source.Code != dest.Code)
                    {
                        logger.Error($"Daishin stock mastering, Code not matched, {source.Code} != {dest.Code}");
                        return;
                    }

                    dest.Name = source.Name;
                    dest.UpperLimit = source.UpperLimit;
                    dest.LowerLimit = source.LowerLimit;
                    dest.PreviousClosedPrice = source.PreviousClosedPrice;
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

                    state = MasteringStateType.Finished;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            finally
            {
                lock (masteringLock)
                    mastering.DaishinState = state;
            }
        }

        private void CopyStockMasterFromEbest(StockMastering mastering, StockMaster source)
        {
            var state = MasteringStateType.Ready;

            try
            {
                if (source != null)
                {
                    StockMaster dest = mastering.Stock;

                    if (source.Code != dest.Code)
                    {
                        logger.Error($"Ebest stock mastering, Code not matched, {source.Code} != {dest.Code}");
                        return;
                    }

                    //dest.PreviousVolume = source.PreviousVolume; // => Daishin에서 조회
                    dest.CirculatingVolume = source.CirculatingVolume;
                    dest.ValueAltered = source.ValueAltered;

                    state = MasteringStateType.Finished;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            finally
            {
                lock (masteringLock)
                    mastering.EbestState = state;
            }
        }

        private void CopyStockMasterFromKiwoom(StockMastering mastering, StockMaster source)
        {
            var state = MasteringStateType.Ready;

            try
            {
                if (source != null)
                {
                    StockMaster dest = mastering.Stock;

                    if (source.Code != dest.Code)
                    {
                        logger.Error($"Kiwoom stock mastering, Code not matched, {source.Code} != {dest.Code}");
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

                    state = MasteringStateType.Finished;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            finally
            {
                lock (masteringLock)
                    mastering.KiwoomState = state;
            }
        }
    }
}
