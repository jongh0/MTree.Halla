﻿using MTree.DataStructure;
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
        private void StartStockMastering()
        {
            logger.Info("Stock mastering started");

            Task.Run(() =>
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();

                try
                {
                    foreach (var code in StockCodeList)
                    {
                        var mastering = new StockMastering();
                        mastering.Stock = new StockMaster();
                        mastering.Stock.Code = code;

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
                    logger.Info($"Stock mastering done, Elapsed time: {sw.Elapsed.ToString("hh:mm:ss")}");
                    Debugger.Break(); // 테스트 용도

                    DistributeSubscribeCode();
                }
            });
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
                    if (DaishinContracts.Count >= 3) break;
                    Thread.Sleep(1000);
                }

                logger.Info("Daishin stock mastering, contract count enough");

                while (true)
                {
                    lock (StockMasteringList)
                    {
                        if (StockMasteringList.Count(m => m.DaishinState != MasteringStateType.Finished) == 0)
                            break;
                    }

                    StockMastering mastering = null;

                    lock (StockMasteringList)
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
                            lock (DaishinContractForMastering)
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
                logger.Info($"Daishin stock mastering done, Elapsed time: {sw.Elapsed.ToString("hh:mm:ss")}");
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
                    if (EbestContracts.Count >= 1) break;
                    Thread.Sleep(1000);
                }

                logger.Info("Ebest stock mastering, contract count enough");

                while (true)
                {
                    lock (StockMasteringList)
                    {
                        if (StockMasteringList.Count(m => m.EbestState != MasteringStateType.Finished) == 0)
                            break;
                    }

                    StockMastering mastering = null;

                    lock (StockMasteringList)
                    {
                        mastering = StockMasteringList.FirstOrDefault(m => m.EbestState == MasteringStateType.Ready);
                        if (mastering != null)
                            mastering.DaishinState = MasteringStateType.Running;
                    }

                    if (mastering != null)
                    {
                        PublishContract contract = null;

                        while (true)
                        {
                            lock (EbestContractForMastering)
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
                logger.Info($"Ebest stock mastering done, Elapsed time: {sw.Elapsed.ToString("hh:mm:ss")}");
            }
        }

        private void GetStockMaster(StockMastering mastering, PublishContract contract)
        {
            try
            {
                var master = contract.Callback.GetStockMaster(mastering.Stock.Code);

                if (contract.Type == PublisherType.Daishin)
                    CopyStockMasterFromDaishin(mastering, master);
                else if (contract.Type == PublisherType.Ebest)
                    CopyStockMasterFromEbest(mastering, master);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            finally
            {
                if (contract.Type == PublisherType.Daishin)
                {
                    lock (DaishinContractForMastering)
                        contract.NowOperating = false;
                }
                else if (contract.Type == PublisherType.Ebest)
                {
                    lock (EbestContractForMastering)
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
                        logger.Warn($"Daishin stock mastering, Code not matched, {source.Code} != {dest.Code}");
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
                lock (StockMasteringList)
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
                        logger.Warn($"Ebest stock mastering, Code not matched, {source.Code} != {dest.Code}");
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
                lock (StockMasteringList)
                    mastering.EbestState = state;
            }
        }
    }
}