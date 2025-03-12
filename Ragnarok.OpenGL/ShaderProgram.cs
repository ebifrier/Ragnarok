using System;
using System.Collections.Generic;
using System.Linq;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;

namespace Ragnarok.OpenGL
{
    /// <summary>
    /// OpenGL用のシェーダーを管理します。
    /// </summary>
    public class ShaderProgram : GLObject
    {
        public sealed class UniformData
        {
            public int Index;
            public string Name;
            public ActiveUniformType Type;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ShaderProgram(IGraphicsContext context)
            : base(context)
        {
        }

        /// <summary>
        /// テクスチャを削除します。
        /// </summary>
        public override void Destroy()
        {
            if (ProgramName != 0)
            {
                var program = ProgramName;
                GLDisposer.AddTarget(
                    Context,
                    () => GL.DeleteProgram(program));
                ProgramName = 0;
            }
        }

        /// <summary>
        /// プログラム名(ID)を取得します。
        /// </summary>
        public int ProgramName
        {
            get;
            private set;
        }

        /// <summary>
        /// シェーダーが使用可能か調べます。
        /// </summary>
        public bool IsAvailable
        {
            get { return ProgramName != 0; }
        }

        /// <summary>
        /// 設定可能な変数一覧を取得します。
        /// </summary>
        public List<UniformData> Uniforms
        {
            get;
            private set;
        } = new();

        /// <summary>
        /// 変数のバインド番号を取得します。見つからない場合は-1を返します。
        /// </summary>
        public int GetUniformLocation(string name)
        {
            var uniform = Uniforms.Find(_ => _.Name == name);
            return uniform?.Index ?? -1;
        }

        /// <summary>
        /// シェーダーをバインドします。
        /// </summary>
        public void Use()
        {
            if (ProgramName == 0)
            {
                throw new InvalidOperationException(
                    "シェーダーの作成が完了していません。");
            }

            ValidateContext();
            GLw.C(() => GL.UseProgram(ProgramName));
        }

        /// <summary>
        /// シェーダーのバインドを解除します。
        /// </summary>
        public static void Unuse()
        {
            GLw.C(() => GL.UseProgram(0));
        }

        /// <summary>
        /// ファイルからテクスチャを作成します。
        /// </summary>
        public bool Create(string vertexShaderSource,
                           string fragmentShaderSource)
        {
            ValidateContext();

            int programName = 0;
            int vertexName = 0;
            int fragmentName = 0;
            try
            {
                vertexName = CreateShader(
                    ShaderType.VertexShader,
                    vertexShaderSource);

                fragmentName = CreateShader(
                    ShaderType.FragmentShader,
                    fragmentShaderSource);

                programName = GLw.C(GL.CreateProgram);
                GLw.C(() => GL.AttachShader(programName, vertexName));
                GLw.C(() => GL.AttachShader(programName, fragmentName));
                GLw.C(() => GL.LinkProgram(programName));
                GL.GetProgram(programName, GetProgramParameterName.LinkStatus, out int status);
                if (status == 0)
                {
                    var info = GL.GetProgramInfoLog(programName);
                    throw new GLException(
                        $"Failed to link program: {info}");
                }

                // 先にDeleteShaderを呼んでおくと、
                // DeleteProrgram時にシェーダーも同時に削除されるようになります。
                GLw.C(() => GL.DeleteShader(vertexName));
                GLw.C(() => GL.DeleteShader(fragmentName));

                Destroy();
                ProgramName = programName;
                Uniforms = ListUniforms(programName).ToList();
                return true;
            }
            catch (Exception ex)
            {
                Log.ErrorException(ex, "シェーダーの作成に失敗しました。");

                if (fragmentName != 0)
                {
                    GLw.C(() => GL.DeleteShader(fragmentName));
                }

                if (vertexName != 0)
                {
                    GLw.C(() => GL.DeleteShader(vertexName));
                }

                if (programName != 0)
                {
                    GLw.C(() => GL.DeleteProgram(programName));
                }

                throw;
            }
        }

        private static int CreateShader(ShaderType shaderType, string source)
        {
            var name = GLw.C(() => GL.CreateShader(shaderType));
            GLw.C(() => GL.ShaderSource(name, source));
            GLw.C(() => GL.CompileShader(name));

            GL.GetShader(name, ShaderParameter.CompileStatus, out int status);
            if (status == 0)
            {
                var info = GL.GetShaderInfoLog(name);
                throw new GLException(
                    $"Failed to compile {shaderType}: {info}");
            }

            return name;
        }

        private static IEnumerable<UniformData> ListUniforms(int programName)
        {
            int uniformCount = 0;
            GLw.C(() => GL.GetProgram(
                programName,
                GetProgramParameterName.ActiveUniforms,
                out uniformCount));

            for (var i = 0; i < uniformCount; i++)
            {
                ActiveUniformType type = ActiveUniformType.Bool;
                string name = string.Empty;

                GLw.C(() => GL.GetActiveUniform(
                    programName, i, 256,
                    out _, out _, out type, out name));

                yield return new UniformData
                {
                    Index = i,
                    Name = name,
                    Type = type,
                };
            }
        }
    }
}
