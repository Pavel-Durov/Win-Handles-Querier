﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Diagnostics.Runtime;
using WHQ.Core.Model.Unified.Thread;
using WHQ.Core.msos;

namespace WHQ.Core.Model.Unified.Thread
{
    public class UnifiedThread
    {
        public UnifiedThread(ThreadInfo info)
        {
            this.IsManagedThread = info.IsManagedThread;
            this.Index = info.Index;
            this.EngineThreadId = info.EngineThreadId;
            this.OSThreadId = info.OSThreadId;
            this.Detail = info.Detail;
        }

        public UnifiedThread(uint owningThreadId)
        {
            this.OSThreadId = owningThreadId;
        }

        public List<UnifiedStackFrame> StackTrace { get; protected set; }
        public List<UnifiedBlockingObject> BlockingObjects { get; protected set; }

        public bool IsManagedThread { get; protected set; }
        public uint Index { get; set; }
        public uint EngineThreadId { get; set; }
        public uint OSThreadId { get; set; }
        public string Detail { get; set; }
    }
}
