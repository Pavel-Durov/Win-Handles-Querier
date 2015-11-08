﻿using Microsoft.Diagnostics.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Diagnostics.Runtime.Interop;
using System.Runtime.InteropServices;
using System.IO;
using Assignment_3.msos;

class ThreadStackHandler
{
    public static void Handle(ClrThread thread)
    {
    } 

    IDebugClient _debugClient;
    private ClrRuntime _runtime;

    public IEnumerable<ThreadInfo> Threads { get; private set; }

    internal void Handle(IDebugClient debugClient, ClrThread thread, ClrRuntime runtime)
    {
        _debugClient = debugClient;
        _runtime = runtime;

        uint _numThreads = 0;
        Util.VerifyHr(((IDebugSystemObjects)_debugClient).GetNumberThreads(out _numThreads));

        var threads = new List<ThreadInfo>();

        ThreadInfo specific_info = null;

        for (uint threadIdx = 0; threadIdx < _numThreads; ++threadIdx)
        {
            specific_info = GetThreadInfo(threadIdx);
            threads.Add(specific_info);
        }
        Threads = threads;


        var managedStack = GetManagedStackTrace(thread);
        var unmanagedStack = GetNativeStackTrace(specific_info.EngineThreadId);
        Init(thread, managedStack, unmanagedStack);
    }

    private void Init(ClrThread thread, List<UnifiedStackFrame> managedStack, List<UnifiedStackFrame> unmanagedStack)
    {
        Assignment_3.PrintHandles.ThreadStackAnalyzer.PrintStackTrace(managedStack, thread, _runtime);
        Assignment_3.PrintHandles.ThreadStackAnalyzer.PrintStackTrace(unmanagedStack, thread, _runtime, true);
  

    }

    private ThreadInfo GetThreadInfo(uint threadIndex)
    {
        uint[] engineThreadIds = new uint[1];
        uint[] osThreadIds = new uint[1];
        Util.VerifyHr(((IDebugSystemObjects)_debugClient).GetThreadIdsByIndex(threadIndex, 1, engineThreadIds, osThreadIds));
        ClrThread managedThread = _runtime.Threads.FirstOrDefault(thread => thread.OSThreadId == osThreadIds[0]);
        return new ThreadInfo
        {
            Index = threadIndex,
            EngineThreadId = engineThreadIds[0],
            OSThreadId = osThreadIds[0],
            ManagedThread = managedThread
        };
    }


    public List<UnifiedStackFrame> GetStackTrace(uint threadIndex)
    {
        ThreadInfo threadInfo = GetThreadInfo(threadIndex);
        List<UnifiedStackFrame> unifiedStackTrace = new List<UnifiedStackFrame>();
        List<UnifiedStackFrame> nativeStackTrace = GetNativeStackTrace(threadInfo.EngineThreadId);
        if (threadInfo.IsManagedThread)
        {
            List<UnifiedStackFrame> managedStackTrace = GetManagedStackTrace(threadInfo.ManagedThread);
            int managedFrame = 0;
            for (int nativeFrame = 0; nativeFrame < nativeStackTrace.Count; ++nativeFrame)
            {
                bool found = false;
                for (int temp = managedFrame; temp < managedStackTrace.Count; ++temp)
                {
                    if (nativeStackTrace[nativeFrame].InstructionPointer == managedStackTrace[temp].InstructionPointer)
                    {
                        managedStackTrace[temp].LinkedStackFrame = nativeStackTrace[nativeFrame];
                        unifiedStackTrace.Add(managedStackTrace[temp]);
                        managedFrame = temp + 1;
                        found = true;
                        break;
                    }
                    else if (managedFrame > 0)
                    {
                        // We have already seen at least one managed frame, and we're about
                        // to skip a managed frame because we didn't find a matching native
                        // frame. In this case, add the managed frame into the stack anyway.
                        unifiedStackTrace.Add(managedStackTrace[temp]);
                        managedFrame = temp + 1;
                        found = true;
                        break;
                    }
                }
                // We didn't find a matching managed frame, so add the native frame directly.
                if (!found)
                    unifiedStackTrace.Add(nativeStackTrace[nativeFrame]);
            }
        }
        else
        {
            return nativeStackTrace;
        }
        return unifiedStackTrace;
    }

    private List<UnifiedStackFrame> GetManagedStackTrace(ClrThread thread)
    {
        return (from frame in thread.StackTrace
                let sourceLocation = SymbolCache.GetFileAndLineNumberSafe(frame)
                select new UnifiedStackFrame(frame, sourceLocation)
                ).ToList();
    }



    private List<UnifiedStackFrame> GetNativeStackTrace(uint engineThreadId)
    {
        Util.VerifyHr(((IDebugSystemObjects)_debugClient).SetCurrentThreadId(engineThreadId));

        DEBUG_STACK_FRAME[] stackFrames = new DEBUG_STACK_FRAME[200];
        uint framesFilled;
        Util.VerifyHr(((IDebugControl)_debugClient).GetStackTrace(0, 0, 0, stackFrames, stackFrames.Length, out framesFilled));

        List<UnifiedStackFrame> stackTrace = new List<UnifiedStackFrame>();
        for (uint i = 0; i < framesFilled; ++i)
        {
            stackTrace.Add(new UnifiedStackFrame(stackFrames[i], (IDebugSymbols2)_debugClient));
        }
        return stackTrace;
    }





}

