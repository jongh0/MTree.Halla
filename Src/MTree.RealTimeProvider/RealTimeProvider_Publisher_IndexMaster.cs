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
        private void StartIndexMastering()
        {
            logger.Info("Index mastering started");

            bool masteringRet = false;

            Stopwatch sw = new Stopwatch();
            sw.Start();

            try
            {
                var now = DateTime.Now;

                foreach (var codeEntity in IndexCodeList.Values)
                {
                    var mastering = new IndexMastering();
                    mastering.Index.Code = codeEntity.Code;
                    mastering.Index.Name = codeEntity.Name;
                    mastering.Index.Time = new DateTime(now.Year, now.Month, now.Day); // IndexMaster는 날짜까지만 사용한다

                    IndexMasteringList.Add(mastering);
                }

                var masteringTask = new List<Task>();
                masteringTask.Add(Task.Run(() => StartDaishinIndexMastering()));

                masteringRet = Task.WaitAll(masteringTask.ToArray(), TimeSpan.FromMinutes(30));
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            finally
            {
                sw.Stop();

                if (masteringRet == true)
                {
                    logger.Info($"Index mastering done, Elapsed time: {sw.Elapsed.ToString()}");
                    PushUtility.NotifyMessage("Index mastering success");

                    Task.Run(() => StartIndexMasterPublishing());
                }
                else
                {
                    logger.Info("Index mastering failed");
                    PushUtility.NotifyMessage("Index mastering fail");
                }
            }
        }

        private void StartIndexMasterPublishing()
        {
            logger.Info("Index master publishing started");

            Stopwatch sw = new Stopwatch();
            sw.Start();

            try
            {
                foreach (var mastering in IndexMasteringList)
                {
                    foreach (var contract in MasteringContracts)
                    {
                        try
                        {
                            contract.Value.Callback.ConsumeIndexMaster(mastering.Index);
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
                logger.Info($"Index master publishing done, Elapsed time: {sw.Elapsed.ToString()}");
            }
        }

        private void StartDaishinIndexMastering()
        {
            logger.Info("Daishin index mastering started");

            Stopwatch sw = new Stopwatch();
            sw.Start();

            try
            {
                while (true)
                {
                    lock (masteringLock)
                    {
                        if (IndexMasteringList.Count(m => m.DaishinState != MasteringStates.Finished) == 0)
                            break;
                    }

                    IndexMastering mastering = null;

                    lock (masteringLock)
                    {
                        mastering = IndexMasteringList.FirstOrDefault(m => m.DaishinState == MasteringStates.Ready);
                        if (mastering != null)
                            mastering.DaishinState = MasteringStates.Running;
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
                                    contract.IsOperating = true;
                                    break;
                                }
                            }

                            Thread.Sleep(100);
                        }

                        Task.Run(() => GetIndexMaster(mastering, contract));
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
                logger.Info($"Daishin index mastering done, Elapsed time: {sw.Elapsed.ToString()}");
            }
        }

        private void GetIndexMaster(IndexMastering mastering, PublishContract contract)
        {
            try
            {
                var codeEntity = IndexCodeList[mastering.Index.Code];
                var code = codeEntity.Code;

                if (contract.Type == ProcessTypes.Daishin)
                    code = CodeEntity.ConvertToDaishinCode(codeEntity);

                IndexMaster master = contract.Callback.GetIndexMaster(code);

                if (contract.Type == ProcessTypes.Daishin)
                    CopyIndexMasterFromDaishin(mastering, master);
                else
                    logger.Warn("Wrong contract type for index mastering");
            }
            catch (Exception ex)
            {
                logger.Error($"Index mastering error, {contract.ToString()}");
                logger.Error(ex);
            }
            finally
            {
                if (contract.Type == ProcessTypes.Daishin)
                {
                    lock (daishinMasteringLock)
                        contract.IsOperating = false;
                }
            }
        }

        private void CopyIndexMasterFromDaishin(IndexMastering mastering, IndexMaster source)
        {
            var state = MasteringStates.Ready;

            try
            {
                if (source != null)
                {
                    IndexMaster dest = mastering.Index;

                    if (source.Code != dest.Code)
                    {
                        logger.Error($"Daishin index mastering, Code not matched, {source.Code} != {dest.Code}");
                        return;
                    }

                    //dest.Name = source.Name;
                    dest.BasisPrice = source.BasisPrice;
                    dest.PreviousClosedPrice = source.PreviousClosedPrice;
                    dest.PreviousVolume = source.PreviousVolume;

                    state = MasteringStates.Finished;
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
    }
}
