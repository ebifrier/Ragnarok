using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using OpenTK.Graphics.OpenGL;

namespace Ragnarok.OpenGL
{
    /// <summary>
    /// OpenGLでエラーを扱うためのクラスです。
    /// </summary>
    public static class GLw
    {
        public static void C(Action action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            action();

            var err = GL.GetError();
            if (err != ErrorCode.NoError)
            {
                var trace = Environment.StackTrace;
                var n = Environment.NewLine;
                Log.Error($"{err} {(int)err}: OpenGL error{n}{trace}");
            }
        }

        public static T C<T>(Func<T> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            var result = action();

            var err = GL.GetError();
            if (err != ErrorCode.NoError)
            {
                var trace = Environment.StackTrace;
                var n = Environment.NewLine;
                Log.Error($"{err} {(int)err}: OpenGL error{n}{trace}");
            }

            return result;
        }

        /// <summary>
        /// OpenGLの内部デバッグ出力を開始します。
        /// </summary>
        /// <param name="syncronous">
        /// リアルタイムデバッグを行いたい場合は真に設定してください。
        /// </param>
        /// <remarks>
        /// Windowsに OpenTK.Windowing.Common.ContextFlags.Debug を設定してください。
        /// </remarks>
        public static void StartDebug(bool syncronous = false)
        {
            GL.DebugMessageCallback(OnDebugMessage, IntPtr.Zero);
            GL.Enable(EnableCap.DebugOutput);

            if (syncronous)
            {
                GL.Enable(EnableCap.DebugOutputSynchronous);
            }
        }

        private static void OnDebugMessage(
            DebugSource source,     // Source of the debugging message.
            DebugType type,         // Type of the debugging message.
            int id,                 // ID associated with the message.
            DebugSeverity severity, // Severity of the message.
            int length,             // Length of the string in pMessage.
            IntPtr pMessage,        // Pointer to message string.
            IntPtr pUserParam)      // The pointer you gave to OpenGL, explained later.
        {
            var message = Marshal.PtrToStringAnsi(pMessage, length);

            Log.Error("[{0} source={1} type={2} id={3}] {4}",
                severity, source, type, id, message);
        }
    }
}
