using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using NLog;
using NLog.LayoutRenderers;

namespace Ragnarok.NLog
{
    /// <summary>
    /// InnerExceptionを扱う例外レイアウトクラスです。
    /// </summary>
    [LayoutRenderer("repeated-exception", UsingLogEventInfo = true)]
    public class RepeatedExceptionLayoutRenderer : ExceptionLayoutRenderer
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public RepeatedExceptionLayoutRenderer()
        {
            InnerExceptionSeparator = Environment.NewLine;
        }

        /// <summary>
        /// 内部例外を区切るセパレーターを取得または設定します。
        /// </summary>
        /// <remarks>
        /// デフォルトでは改行文字を使います。
        /// </remarks>
        public string InnerExceptionSeparator
        {
            get;
            set;
        }

        /// <summary>
        /// 出力文字列を書き出します。
        /// </summary>
        protected override void Append(StringBuilder builder,
                                       LogEventInfo logEvent)
        {
            var original = logEvent.Exception;
            var separator = string.Empty;

            try
            {
                // 全内部例外を書き出します。
                for (var ex = original; ex != null; ex = ex.InnerException)
                {
                    builder.Append(separator);
                    separator = this.InnerExceptionSeparator;

                    logEvent.Exception = ex;
                    base.Append(builder, logEvent);
                }
            }
            finally
            {
                logEvent.Exception = original;
            }
        }
    }
}
