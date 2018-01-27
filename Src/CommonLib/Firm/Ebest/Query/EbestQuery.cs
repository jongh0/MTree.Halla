using CommonLib.Firm.Ebest.Block;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonLib.Firm.Ebest.Query
{
    public class EbestQuery<TInBlock, TOutBlock> : QueryBase<TInBlock> 
        where TInBlock : BlockBase
        where TOutBlock : BlockBase
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public event Action<TOutBlock> OutBlockReceived;

        public TOutBlock OutBlock { get; private set; }

        public override bool ExecuteQuery(TInBlock block)
        {
            OutBlock = default(TOutBlock);
            return base.ExecuteQuery(block);
        }

        public override bool ExecuteQueryAndWait(TInBlock block, int timeout = QUERY_TIMEOUT)
        {
            OutBlock = default(TOutBlock);
            return base.ExecuteQueryAndWait(block, timeout);
        }

        protected override void OnReceiveData(string trCode)
        {
            if (Query.GetFieldData(out TOutBlock block) == true)
            {
                _logger.Info($"OnReceiveData: {block}");

                OutBlock = block;
                OutBlockReceived?.Invoke(block);
            }

            base.OnReceiveData(trCode);
        }

        public int GetOutBlockCount()
        {
            return Query.GetBlockCount(typeof(TOutBlock).Name);
        }

        public TOutBlock GetOutBlock(int index)
        {
            if (Query.GetFieldData(out TOutBlock block, index) == true)
                return block;

            return null;
        }

        public IEnumerable<TOutBlock> GetAllOutBlocks()
        {
            var blocks = new List<TOutBlock>();

            try
            {
                var count = GetOutBlockCount();
                for (int i = 0; i < count; i++)
                {
                    var block = GetOutBlock(i);
                    if (block != null)
                        blocks.Add(block);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return blocks;
        }
    }

    public class EbestQuery<TInBlock, TOutBlock, TOutBlock1> : QueryBase<TInBlock>
        where TInBlock : BlockBase 
        where TOutBlock : BlockBase 
        where TOutBlock1 : BlockBase
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        public event Action<TOutBlock> OutBlockReceived;
        public event Action<TOutBlock1> OutBlock1Received;

        public TOutBlock OutBlock { get; private set; }
        public TOutBlock1 OutBlock1 { get; private set; }

        public override bool ExecuteQuery(TInBlock block)
        {
            OutBlock = default(TOutBlock);
            OutBlock1 = default(TOutBlock1);
            return base.ExecuteQuery(block);
        }

        public override bool ExecuteQueryAndWait(TInBlock block, int timeout = QUERY_TIMEOUT)
        {
            OutBlock = default(TOutBlock);
            OutBlock1 = default(TOutBlock1);
            return base.ExecuteQueryAndWait(block, timeout);
        }

        protected override void OnReceiveData(string trCode)
        {
            if (Query.GetFieldData(out TOutBlock block) == true)
            {
                _logger.Info($"OnReceiveData: {block}");

                OutBlock = block;
                OutBlockReceived?.Invoke(block);
            }

            if (Query.GetFieldData(out TOutBlock1 block1) == true)
            {
                _logger.Info($"OnReceiveData: {block1}");

                OutBlock1 = block1;
                OutBlock1Received?.Invoke(block1);
            }

            base.OnReceiveData(trCode);
        }

        public int GetOutBlockCount()
        {
            return Query.GetBlockCount(typeof(TOutBlock).Name);
        }

        public TOutBlock GetOutBlock(int index)
        {
            if (Query.GetFieldData(out TOutBlock block, index) == true)
                return block;

            return null;
        }

        public IEnumerable<TOutBlock> GetAllOutBlock()
        {
            var blocks = new List<TOutBlock>();

            try
            {
                var count = GetOutBlockCount();
                for (int i = 0; i < count; i++)
                {
                    var block = GetOutBlock(i);
                    if (block != null)
                        blocks.Add(block);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return blocks;
        }

        public int GetOutBlock1Count()
        {
            return Query.GetBlockCount(typeof(TOutBlock1).Name);
        }

        public TOutBlock1 GetOutBlock1(int index)
        {
            if (Query.GetFieldData(out TOutBlock1 block, index) == true)
                return block;

            return null;
        }

        public IEnumerable<TOutBlock1> GetAllOutBlock1()
        {
            var blocks = new List<TOutBlock1>();

            try
            {
                var count = GetOutBlock1Count();
                for (int i = 0; i < count; i++)
                {
                    var block = GetOutBlock1(i);
                    if (block != null)
                        blocks.Add(block);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }

            return blocks;
        }
    }
}
