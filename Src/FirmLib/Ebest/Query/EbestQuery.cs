using FirmLib.Ebest.Block;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FirmLib.Ebest.Query
{
    public class EbestQuery<TInBlock> : QueryBase<TInBlock> where TInBlock : BlockBase
    {
        private static readonly NLog.Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        /// <summary>
        /// OutBlock의 갯수를 조회한다.
        /// </summary>
        /// <returns></returns>
        public int GetOutBlockCount<TOutBlock>() where TOutBlock : BlockBase
        {
            return Query.GetBlockCount(typeof(TOutBlock).Name);
        }

        /// <summary>
        /// Index에 맞는 OutBock을 반환한다.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public TOutBlock GetOutBlock<TOutBlock>(int index = 0) where TOutBlock : BlockBase
        {
            if (Query.GetFieldData(out TOutBlock block, index) == true)
                return block;

            return default(TOutBlock);
        }

        /// <summary>
        /// 모든 OutBlock을 반환한다.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<TOutBlock> GetAllOutBlocks<TOutBlock>() where TOutBlock : BlockBase
        {
            var blocks = new List<TOutBlock>();

            try
            {
                var count = GetOutBlockCount<TOutBlock>();
                for (int i = 0; i < count; i++)
                {
                    var block = GetOutBlock<TOutBlock>(i);
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
