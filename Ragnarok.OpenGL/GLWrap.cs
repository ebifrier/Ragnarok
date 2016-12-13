using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK;
using OpenTK.Graphics.OpenGL;

namespace Ragnarok.OpenGL
{
    /// <summary>
    /// OpenGLでエラーを扱うためのクラスです。
    /// </summary>
    public static class GLWrap
    {
        public static void Wrap(Action action)
        {
            action();

            var err = GL.GetError();
            if (err != ErrorCode.NoError)
            {
                var trace = Environment.StackTrace;
                var n = Environment.NewLine;
                Log.Error($"{err} {(int)err}: OpenGL error{n}{trace}");
            }
        }

        public static T Wrap<T>(Func<T> action)
        {
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
    }
}
