﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MTree.Publisher
{
    public class BasePublisher
    {
        protected object LockObeject { get; } = new object();

        protected CancellationTokenSource QueueTaskCancelSource { get; } = new CancellationTokenSource();
        protected CancellationToken QueueTaskCancelToken { get; set; }

        public BasePublisher()
        {
            QueueTaskCancelToken = QueueTaskCancelSource.Token;
        }
    }
}
