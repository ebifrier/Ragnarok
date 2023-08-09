#if !MONO
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Diagnostics.Runtime;

namespace Ragnarok.Utility
{
    /// <summary>
    /// スレッドのスタックトレース情報を保持します。
    /// </summary>
    public sealed class PdbThread
    {
        /// <summary>
        /// スレッドIDを取得します。
        /// </summary>
        public int ThreadID { get; set; }

        /// <summary>
        /// スレッドのスタックトレースを取得します。
        /// </summary>
        public List<string> StackTrace { get; set; }
    }

    /// <summary>
    /// スタックトレースなどを表示してからロックします。
    /// </summary>
    public static class PdbUtility
    {
        /// <summary>
        /// 全スレッドのスタックトレースを取得します。
        /// </summary>
        public static List<PdbThread> GetThreadList()
        {
            try
            {
                var pid = Environment.ProcessId;
                using var dataTarget = DataTarget.CreateSnapshotAndAttach(pid);
 
                var clrInfo = dataTarget.ClrVersions[0];
                using var runtime = clrInfo.CreateRuntime();

                return runtime.Threads
                    .Where(_ => _?.IsAlive == true)
                    .Select(_ => new PdbThread
                    {
                        ThreadID = _.ManagedThreadId,
                        StackTrace = MakeStackTrace(_),
                    })
                    .Where(_ => _.StackTrace.Any())
                    .ToList();
            }
            catch (Exception ex)
            {
                Log.ErrorException(ex,
                    "全スレッドのスタックトレースの取得に失敗しました。");

                return new List<PdbThread>();
            }
        }

        private static List<string> MakeStackTrace(ClrThread thread)
        {
            return thread.EnumerateStackTrace()
                .Select(_ => $"  at {_.InstructionPointer,10:x} {_.StackPointer,10:x} {_}")
                .ToList();
        }
    }
}
#endif
