using MTree.Configuration;
using MTree.DataStructure;
using MTree.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTree.RealTimeProvider
{
    public partial class RealTimeProvider
    {
        private void StartCodeMapBuilding(PublisherContract contract)
        {
            Task.Run(() =>
            {
                try
                {
                    CodeMapBuilder codeMapBuilder = new CodeMapBuilder();

                    var codemapBuildingTask = new List<Task>();

                    codemapBuildingTask.Add(Task.Run(() =>
                    {
                        _logger.Info(RealTimeState = "Build MarketType Map");
                        if (DaishinContracts[0] != null)
                            codeMapBuilder.AddMarketTypeMap(DaishinContracts[0]);
                        _logger.Info(RealTimeState = "Build MarketType Map Done");
                    }));

                    codemapBuildingTask.Add(Task.Run(() =>
                    {
                        _logger.Info(RealTimeState = "Build Group Map");
                        if (DaishinContracts[0] != null)
                            codeMapBuilder.AddGroupMap(DaishinContracts[0]);
                        _logger.Info(RealTimeState = "Build Group Map Done");
                    }));

                    codemapBuildingTask.Add(Task.Run(() =>
                    {
                        _logger.Info(RealTimeState = "Build BizType Map");
                        foreach (PublisherContract daishinContract in DaishinContracts)
                        {
                            codeMapBuilder.AddBizTypeMap(daishinContract);
                        }
                        _logger.Info(RealTimeState = "Build BizType Map Done");
                    }));

                    codemapBuildingTask.Add(Task.Run(() =>
                    {
                        _logger.Info(RealTimeState = "Build CapitalScale Map");
                        foreach (PublisherContract daishinContract in DaishinContracts)
                        {
                            codeMapBuilder.AddCapitalScaleMap(daishinContract);
                        }
                        _logger.Info(RealTimeState = "Build CapitalScale Map Done");
                    }));

                    codemapBuildingTask.Add(Task.Run(() =>
                    {
                        _logger.Info(RealTimeState = "Build Daishin Theme Map");
                        if (DaishinContracts[1] != null)
                            codeMapBuilder.AddThemeMap(DaishinContracts[1], "DaishinTheme");
                        _logger.Info(RealTimeState = "Build Daishin Theme Map Done");
                    }));

                    if (Config.General.ExcludeEbest == false)
                    {
                        codemapBuildingTask.Add(Task.Run(() =>
                        {
                            _logger.Info(RealTimeState = $"Build Ebest Theme Map");
                            if (EbestContracts[0] != null)
                                codeMapBuilder.AddThemeMap(EbestContracts[0], "EbestTheme");
                            _logger.Info(RealTimeState = $"Build Ebest Theme Map Done");
                        }));
                    }
                    if (Config.General.ExcludeKiwoom == false)
                    {
                        codemapBuildingTask.Add(Task.Run(() =>
                        {
                            _logger.Info(RealTimeState = $"Build Kiwoom Theme Map");
                            if (KiwoomContracts.Count > 1 && KiwoomContracts[0] != null)
                                codeMapBuilder.AddThemeMap(KiwoomContracts[0], "KiwoomTheme");
                            _logger.Info(RealTimeState = $"Build Kiwoom Theme Map Done");
                        }));
                    }

                    bool masteringRet = Task.WaitAll(codemapBuildingTask.ToArray(), TimeSpan.FromMinutes(30));
                    //var jsonString = codeMapBuilder.GetCodeMapAsJsonString();
                    
                    StartCodeMapPublishing(codeMapBuilder.GetCodeMap());
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                }
            }).Wait();
        }
    }
}
