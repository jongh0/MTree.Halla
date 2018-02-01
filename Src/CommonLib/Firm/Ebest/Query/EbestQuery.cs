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

        /// <summary>
        /// Query를 수행한다.
        /// </summary>
        /// <param name="block"></param>
        /// <returns></returns>
        public override bool ExecuteQuery(TInBlock block)
        {
            OutBlock = default(TOutBlock);
            return base.ExecuteQuery(block);
        }

        /// <summary>
        /// Query 수행 후 결과 반환될 때 까지 대기한다.
        /// </summary>
        /// <param name="block"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public override bool ExecuteQueryAndWait(TInBlock block, int timeout = QUERY_TIMEOUT)
        {
            OutBlock = default(TOutBlock);
            return base.ExecuteQueryAndWait(block, timeout);
        }

        protected override void OnReceiveData(string trCode)
        {
            if (Query.GetFieldData(out TOutBlock block) == true)
            {
                if (block.BlockName != nameof(t1102OutBlock))
                    _logger.Info($"OnReceiveData: {block}");

                OutBlock = block;
                OutBlockReceived?.Invoke(block);
            }

            base.OnReceiveData(trCode);
        }

        /// <summary>
        /// OutBlock의 갯수를 조회한다.
        /// </summary>
        /// <returns></returns>
        public int GetOutBlockCount()
        {
            return Query.GetBlockCount(typeof(TOutBlock).Name);
        }

        /// <summary>
        /// Index에 맞는 OutBock을 반환한다.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public TOutBlock GetOutBlock(int index)
        {
            if (Query.GetFieldData(out TOutBlock block, index) == true)
                return block;

            return null;
        }

        /// <summary>
        /// 모든 OutBlock을 반환한다.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Query를 수행한다.
        /// </summary>
        /// <param name="block"></param>
        /// <returns></returns>
        public override bool ExecuteQuery(TInBlock block)
        {
            OutBlock = default(TOutBlock);
            OutBlock1 = default(TOutBlock1);
            return base.ExecuteQuery(block);
        }

        /// <summary>
        /// Query 수행 후 결과 반환될 때 까지 대기한다.
        /// </summary>
        /// <param name="block"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
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

        /// <summary>
        /// OutBlock의 갯수를 조회한다.
        /// </summary>
        /// <returns></returns>
        public int GetOutBlockCount()
        {
            return Query.GetBlockCount(typeof(TOutBlock).Name);
        }

        /// <summary>
        /// Index에 맞는 OutBlock을 반환한다.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public TOutBlock GetOutBlock(int index)
        {
            if (Query.GetFieldData(out TOutBlock block, index) == true)
                return block;

            return null;
        }

        /// <summary>
        /// 모든 OutBlock을 반환한다.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// OutBlock1의 갯수를 조회한다.
        /// </summary>
        /// <returns></returns>
        public int GetOutBlock1Count()
        {
            return Query.GetBlockCount(typeof(TOutBlock1).Name);
        }

        /// <summary>
        /// Index에 맞는 OutBlock1을 반환한다.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public TOutBlock1 GetOutBlock1(int index)
        {
            if (Query.GetFieldData(out TOutBlock1 block, index) == true)
                return block;

            return null;
        }

        /// <summary>
        /// 모든 OutBlock1을 반환한다.
        /// </summary>
        /// <returns></returns>
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
