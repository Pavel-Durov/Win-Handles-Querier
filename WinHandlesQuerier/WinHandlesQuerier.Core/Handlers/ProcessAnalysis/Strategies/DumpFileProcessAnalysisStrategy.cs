﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WinHandlesQuerier.Core.Model.Unified;
using WinHandlesQuerier.Core.Model.Unified.Thread;
using WinHandlesQuerier.Core.msos;
using Microsoft.Diagnostics.Runtime;

namespace WinHandlesQuerier.Core.Handlers.StackAnalysis.Strategies
{
    public class DumpFileProcessAnalysisStrategy : ProcessAnalysisStrategy
    {

        ClrRuntime _runtime;

        public DumpFileProcessAnalysisStrategy(string dumpFilePath, ClrRuntime runtime)
        {
            _miniDump = new MiniDump.MiniDumpHandler(dumpFilePath);
            _runtime = runtime;
        }

        MiniDump.MiniDumpHandler _miniDump;

        public override List<UnifiedBlockingObject> GetUnmanagedBlockingObjects(ThreadInfo thread, List<UnifiedStackFrame> unmanagedStack, ClrRuntime runtime)
        {
            var miniDumpHandles = _miniDump.GetHandles();

            List<UnifiedBlockingObject> result = null;

            result.AddRange(base.GetUnmanagedBlockingObjects(unmanagedStack));

            foreach (var item in miniDumpHandles)
            {
                result.Add(new UnifiedBlockingObject(item));
            }

            CheckForCriticalSections(result, unmanagedStack, runtime);

            return result;
        }


    }
}
