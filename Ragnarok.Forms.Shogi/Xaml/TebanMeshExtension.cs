using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Markup;

using Ragnarok.Shogi;
using Ragnarok.Extra.Effect;

namespace Ragnarok.Forms.Shogi.Xaml
{
    /// <summary>
    /// 手番表示のメッシュを作成する拡張構文です。
    /// </summary>
    [MarkupExtensionReturnType(typeof(Mesh))]
    public sealed class TebanMeshExtension : MarkupExtension
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TebanMeshExtension()
        {
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TebanMeshExtension(BWType turn)
        {
            Turn = turn;
        }

        /// <summary>
        /// 今の手番を取得または設定します。
        /// </summary>
        public BWType Turn
        {
            get;
            set;
        }

        /// <summary>
        /// 手番表示用のメッシュを作成します。
        /// </summary>
        public override object ProvideValue(IServiceProvider service)
        {
            return MeshUtil.CreateTeban(Turn);
        }
    }
}
