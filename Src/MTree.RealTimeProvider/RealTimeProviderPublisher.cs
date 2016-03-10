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
        private ConcurrentDictionary<Guid, PublishContract> PublishContracts { get; set; } = new ConcurrentDictionary<Guid, PublishContract>();

        #region Contract Property
        private List<PublishContract> DaishinContracts
        {
            get { return PublishContracts.Values.Where(c => c.Type == PublisherType.Daishin || c.Type == PublisherType.DaishinMaster).ToList(); }
        }

        private PublishContract DaishinContractForMastering
        {
            get { return PublishContracts.Values.FirstOrDefault(c => (c.Type == PublisherType.Daishin || c.Type == PublisherType.DaishinMaster) && c.NowOperating == false); }
        }

        private List<PublishContract> EbestContracts
        {
            get { return PublishContracts.Values.Where(c => c.Type == PublisherType.Ebest).ToList(); }
        }

        private PublishContract EbestContractForMastering
        {
            get { return PublishContracts.Values.FirstOrDefault(c => c.Type == PublisherType.Ebest && c.NowOperating == false); }
        }

        private List<PublishContract> KrxContracts
        {
            get { return PublishContracts.Values.Where(c => c.Type == PublisherType.Krx).ToList(); }
        }

        private List<PublishContract> NaverContract
        {
            get { return PublishContracts.Values.Where(c => c.Type == PublisherType.Naver).ToList(); }
        }
        #endregion

        public void LaunchPublisher(PublisherType type, bool mastering = false)
        {
            try
            {
                var windowStyle = ProcessWindowStyle.Minimized;

                switch (type)
                {
                    case PublisherType.Daishin:
                    case PublisherType.DaishinMaster:
                        ProcessUtility.Start("MTree.DaishinPublisher.exe", type.ToString(), windowStyle);
                        break;

                    case PublisherType.Ebest:
                        ProcessUtility.Start("MTree.EbestPublisher.exe", type.ToString(), windowStyle);
                        break;

                    case PublisherType.Krx:
                        ProcessUtility.Start("MTree.KrxPublisher.exe", type.ToString(), windowStyle);
                        break;
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public void RegisterPublishContract(Guid clientId, PublishContract contract)
        {
            try
            {
                if (PublishContracts.ContainsKey(clientId) == true)
                {
                    logger.Error($"{contract.Type} contract exist / {clientId}");
                }
                else
                {
                    if (contract.Type == PublisherType.None)
                    {
                        logger.Info($"{contract.Type} wrong contract type / {clientId}");
                    }
                    else
                    {
                        contract.Callback = OperationContext.Current.GetCallbackChannel<IRealTimePublisherCallback>();
                        PublishContracts.TryAdd(clientId, contract);

                        logger.Info($"{contract.Type} contract registered / {clientId}");

                        if (contract.Type == PublisherType.DaishinMaster)
                        {
                            StockCodeList = contract.Callback.GetStockCodeList();

                            if (StockCodeList != null)
                            {
                                logger.Info($"Stock code list count: {StockCodeList.Count}");
                                StartStockMastering();
                            }
                            else
                            {
                                logger.Error("Stock code list gathering failed");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        public void UnregisterPublishContract(Guid clientId)
        {
            try
            {
                if (PublishContracts.ContainsKey(clientId) == true)
                {
                    PublishContract temp;
                    PublishContracts.TryRemove(clientId, out temp);

                    logger.Info($"{clientId} / {temp.Type} contract unregistered");
                }
                else
                {
                    logger.Warn($"{clientId} contract not exist");
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }

        private void StartStockMastering()
        {
            Task.Run(() =>
            {
                foreach (var code in StockCodeList)
                {
                    var mastering = new StockMastering();
                    mastering.Stock = new StockMaster();
                    mastering.Stock.Code = code;

                    StockMasteringList.Add(mastering);
                }

                var masteringTasks = new List<Task>();

                masteringTasks.Add(Task.Run(() =>
                {
                    while (true)
                    {
                        if (DaishinContracts.Count >= 3)
                            break;

                        Thread.Sleep(1000);
                    }

                    var daishinTasks = new List<Task>();

                    foreach (var mastering in StockMasteringList)
                    {
                        PublishContract contract = null;

                        while (true)
                        {
                            contract = DaishinContractForMastering;
                            if (contract != null)
                            {
                                contract.NowOperating = true;
                                break;
                            }
                            else
                            {
                                Thread.Sleep(100);
                            }
                        }

                        daishinTasks.Add(Task.Run(() => GetStockMaster(mastering, contract)));
                    }

                    Task.WaitAll(daishinTasks.ToArray());
                }));

                Task.WaitAll(masteringTasks.ToArray());

                logger.Info("Stock mastering done");
                Debugger.Break();
            });
        }

        private void GetStockMaster(StockMastering mastering, PublishContract contract)
        {
            try
            {
                var stockMaster = contract.Callback.GetStockMaster(mastering.Stock.Code);

                mastering.Stock.Name = stockMaster.Name;
                mastering.DaishinDone = true;
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            finally
            {
                contract.NowOperating = false;
            }
        }

        private void StartStockSubscribing()
        {

        }

        public void PublishBiddingPrice(BiddingPrice biddingPrice)
        {
            BiddingPriceQueue.Enqueue(biddingPrice);
        }

        public void PublishCircuitBreak(CircuitBreak circuitBreak)
        {
        }

        public void PublishIndexConclusion(IndexConclusion conclusion)
        {
            IndexConclusionQueue.Enqueue(conclusion);
        }

        public void PublishStockConclusion(StockConclusion conclusion)
        {
            StockConclusionQueue.Enqueue(conclusion);
        }
    }
}
