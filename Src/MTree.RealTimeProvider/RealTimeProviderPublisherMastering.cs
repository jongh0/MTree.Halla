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

        private void StartStockMastering()
        {
            logger.Info("Stock mastering started");

            Stopwatch sw = new Stopwatch();
            sw.Start();

            try
            {
                LaunchPublisherAll();

                foreach (var code in StockCodeList)
                {
                    var mastering = new StockMastering();
                    mastering.Stock = new StockMaster();
                    mastering.Stock.Code = code.Key;
                    mastering.Stock.Name = code.Value;

                    StockMasteringList.Add(mastering);
                }

                var masteringTask = new List<Task>();
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

                Task.Run(() => DistributeSubscribeCode());
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

        private void GetStockMaster(StockMastering mastering, PublishContract contract)
        {
            try
            {
                var master = contract.Callback.GetStockMaster(mastering.Stock.Code);

                if (contract.Type == ProcessType.Daishin)
                    CopyStockMasterFromDaishin(mastering, master);
                else if (contract.Type == ProcessType.Ebest)
                    CopyStockMasterFromEbest(mastering, master);
            }
            catch (Exception ex)
            {
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

                    dest.PreviousVolume = source.PreviousVolume;
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
    }
}
