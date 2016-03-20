using MTree.DataStructure;
using System;

namespace MTree.DaishinPublisher
{
    public partial class DaishinPublisher
    {
        public override bool SubscribeIndex(string code)
        {
            return false;
        }

        public override bool UnsubscribeIndex(string code)
        {
            return false;
        }

        private void IndexConclusionReceived()
        {
            try
            {
                var now = DateTime.Now;
                var conclusion = new IndexConclusion();



                IndexConclusionQueue.Enqueue(conclusion);
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
        }
    }
}
