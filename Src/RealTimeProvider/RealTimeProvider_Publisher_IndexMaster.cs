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
        private void StartIndexMastering()
        {
            RealTimeState = "Index mastering started";
            _logger.Info(RealTimeState);

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
                    mastering.Index.Time = new DateTime(now.Year, now.Month, now.Day); // 날짜까지만 사용한다

                    IndexMasteringList.Add(mastering);
                }

                var masteringTask = new List<Task>();
                masteringTask.Add(Task.Run(() => StartDaishinIndexMastering()));

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
                    RealTimeState = "Index mastering success";
                    _logger.Info($"{RealTimeState}, Elapsed time: {sw.Elapsed.ToString()}");

                    Task.Run(() => StartIndexMasterPublishing());
                }
                else
                {
                    RealTimeState = "Index mastering failed";
                    _logger.Info(RealTimeState);
                }
            }
        }

        private void StartIndexMasterPublishing()
        {
            RealTimeState = "Index master publishing started";
            _logger.Info(RealTimeState);

            Stopwatch sw = new Stopwatch();
            sw.Start();

            var indexMasterList = new List<IndexMaster>();

            try
            {
                foreach (var mastering in IndexMasteringList)
                {
                    indexMasterList.Add(mastering.Index);
                }

                Counter.IndexMasterCount = indexMasterList.Count;

                foreach (var contract in MasteringContracts)
                {
                    try
                    {
                        contract.Value.Callback.ConsumeIndexMaster(indexMasterList);
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
                indexMasterList.Clear();
                //IndexMasteringList.Clear();

                sw.Stop();

                RealTimeState = "Index master publishing done";
                _logger.Info($"{RealTimeState}, Elapsed time: {sw.Elapsed.ToString()}");
            }
        }

        private void StartDaishinIndexMastering()
        {
            RealTimeState = "Daishin index mastering started";
            _logger.Info(RealTimeState);

            Stopwatch sw = new Stopwatch();
            sw.Start();

            try
            {
                while (true)
                {
                    lock (masteringLock)
                    {
                        var totalCount = IndexMasteringList.Count;
                        var notFinishedCount = IndexMasteringList.Count(m => m.DaishinState != MasteringStates.Finished);
                        RealTimeState = $"Daishin index mastering ({totalCount - notFinishedCount}/{totalCount})";

                        if (notFinishedCount == 0)
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
                        PublisherContract contract = null;

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
                _logger.Error(ex);
            }
            finally
            {
                sw.Stop();

                RealTimeState = "Daishin index mastering done";
                _logger.Info($"{RealTimeState}, Elapsed time: {sw.Elapsed.ToString()}");
            }
        }

        private void GetIndexMaster(IndexMastering mastering, PublisherContract contract)
        {
            try
            {
                var codeEntity = IndexCodeList[mastering.Index.Code];
                var code = codeEntity.Code;

                if (contract.Type == ProcessTypes.DaishinPublisher)
                    code = CodeEntity.ConvertToDaishinCode(codeEntity);

                IndexMaster master = contract.Callback.GetIndexMaster(code);

                if (contract.Type == ProcessTypes.DaishinPublisher)
                    CopyIndexMasterFromDaishin(mastering, master);
                else
                    _logger.Warn("Wrong contract type for index mastering");
            }
            catch (Exception ex)
            {
                _logger.Error($"Index mastering error, {contract.ToString()}");
                _logger.Error(ex);
            }
            finally
            {
                if (contract.Type == ProcessTypes.DaishinPublisher)
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
                        _logger.Error($"Daishin index mastering, Code not matched, {source.Code} != {dest.Code}");
                        return;
                    }

                    //dest.Name = source.Name;
                    dest.BasisPrice = source.BasisPrice;
                    
                    state = MasteringStates.Finished;
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            finally
            {
                lock (masteringLock)
                    mastering.DaishinState = state;
            }
        }
    }
}
