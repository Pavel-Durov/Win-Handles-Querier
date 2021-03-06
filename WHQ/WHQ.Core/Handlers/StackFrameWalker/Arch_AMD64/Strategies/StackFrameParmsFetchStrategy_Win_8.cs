﻿using Microsoft.Diagnostics.Runtime;
using System;
using WHQ.Core.Model.Unified;

namespace WHQ.Core.Handlers.UnmanagedStackFrameWalker.AMD64
{
    internal class StackFrameParmsFetchStrategy_Win_8 : StackFrameParmsFetchStrategy
    {
        public StackFrameParmsFetchStrategy_Win_8(ClrRuntime runtime) : base(runtime)
        {

        }

        /* WaitForSingleObject
        * 
        * 1st: RCX (handle) 
        * 000007fe`fd24102b 488bf9          mov     rdi,rcx
        * 2nd - RDX (Timeout)
        * 000007fe`fd241029 8bf2            mov     esi,edx
        */

        /* EnterCriticalSection 
        * 
        * 1st: RCX (CRITICAL_SECTION ptr) 
        * 00007ffd`b57310cb 488bf9          mov     rdi,rcx
        */

        internal override Params GetWaitForMultipleObjectsParams(UnifiedStackFrame frame)
        {
            Params result = new Params();
            //RCX, RDX, R8, and R9

            //RCX
            //00007ffd`b57312e5 8bd9            mov     ebx,ecx
            var handlesCount = frame.ThreadContext.Context_amd64.Rbx;

            if (handlesCount > Kernel32.Const.MAXIMUM_WAIT_OBJECTS)
            {
                throw new ArgumentOutOfRangeException($"Cannot await for more then : {Kernel32.Const.MAXIMUM_WAIT_OBJECTS}, given value :{handlesCount}");
            }

            result.First = handlesCount;

            //RDX
            //00007ffd`b57312e2 4c8bea          mov     r13,rdx
            var hArrayPtr = frame.ThreadContext.Context_amd64.R13;
            result.Second = hArrayPtr;


            //3rd - R8: WaitAll (BOOLEAN)
            //00007ffd`b57312ca 4489442418      mov dword ptr[rsp + 18h],r8d
            var rspPtr = frame.StackPointer + 24;
            result.Third = base.ReadBoolean(rspPtr);


            //R9
            ////00007ffd`b57312df 458bf1          mov     r14d,r9d
            var waitTime = frame.ThreadContext.Context_amd64.R14;
            result.Fourth = waitTime;

            return result;
        }
    }
}
