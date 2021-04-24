#if !MONO && false
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using Microsoft.Diagnostics.Runtime;
using Microsoft.Diagnostics.Runtime.Utilities;
using Microsoft.Diagnostics.Runtime.Utilities.Pdb;

namespace Ragnarok.Utility
{
    /// <summary>
    /// スタックトレースなどを表示してからロックします。
    /// </summary>
    public static class PdbUtility
    {
        /// <summary>
        /// 全スレッドのスタックトレースを取得します。
        /// </summary>
        public static List<string> GetAllThreadStackTrace()
        {
            try
            {
                var pid = Process.GetCurrentProcess().Id;

                using (var dataTarget = DataTarget.AttachToProcess(pid, 5000, AttachFlag.Passive))
                {
                    var clrInfo = dataTarget.ClrVersions[0];
                    var runtime = clrInfo.CreateRuntime();

                    var list = new List<string>();

                    foreach (var thread in runtime.Threads)
                    {
                        var trace = MakeStackTrace(thread);
                        if (!trace.Any())
                        {
                            continue;
                        }

                        list.Add($"  thread {thread.ManagedThreadId}:");
                        trace.ForEach(_ => list.Add(_));
                    }

                    return list;
                }
            }
            catch
            {
                return new List<string>();
            }
        }

        private static List<string> MakeStackTrace(ClrThread thread)
        {
            return thread.StackTrace
                .Select(_ => $"{_.InstructionPointer,10:x} {_.StackPointer,10:x} {_.DisplayString}")
                .ToList();
        }
    }
}
#endif
