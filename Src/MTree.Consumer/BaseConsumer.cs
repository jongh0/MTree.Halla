﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MTree.Consumer
{
    public class BaseConsumer
    {
        protected object LockObject { get; } = new object();

        protected CancellationTokenSource QueueTaskCancelSource { get; } = new CancellationTokenSource();
        protected CancellationToken QueueTaskCancelToken { get; set; }

        public BaseConsumer()
        {
            QueueTaskCancelToken = QueueTaskCancelSource.Token;
        }
    }
}
