﻿using MTree.RealTimeProvider;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MTree.Publisher
{
    public class PublisherBase : RealTimeBase, IRealTimePublisherCallback
    {
        public PublisherBase() : base()
        {
        }

        public virtual void NoOperation()
        {
        }

        public virtual List<string> GetStockCodeList()
        {
            return new List<string>();
        }
    }
}
