using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using OpenTK;

using Ragnarok;
using Ragnarok.Extra.Effect;
using Ragnarok.Forms.Input;
using Ragnarok.ObjectModel;
using Ragnarok.Shogi;
using Ragnarok.Utility;

namespace Ragnarok.Forms.Shogi.View
{
    /// <summary>
    /// 評価値モードの一覧です。
    /// </summary>
    public enum EvaluationMode
    {
        /// <summary>
        /// 評価値としてプログラム的に設定された値を使います。
        /// </summary>
        Programmable,
        /// <summary>
        /// 評価値としてユーザーが手入力した値を使います。
        /// </summary>
        User,
        /// <summary>
        /// 評価値として評価値サーバーから得られた値を使います。
        /// </summary>
        Server,
    }

    /// <summary>
    /// 評価値表示用のOpenGL用エレメントクラスです。
    /// </summary>
    public class GLEvaluationElement : GLElement
    {
        #region Command
        /// <summary>
        /// 評価値コントロール用のダイアログを表示します。
        /// </summary>
        public static readonly ICommand OpenEvaluationSettingDialog =
            new RelayCommand<GLEvaluationElement>(
                ExecuteOpenDialog);

        private static void ExecuteOpenDialog(GLEvaluationElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            try
            {
                var dialog = new GLEvaluationElementSettingDialog(element);
                dialog.SetCenterMouse();

                // 親コントロールを取得します。
                var glcontainer = element.GLContainer;
                var parent = (glcontainer != null ? glcontainer.ParentForm : null);

                // 親を指定して評価値用のダイアログを開きます。
                dialog.ShowDialog(parent);
            }
            catch(Exception ex)
            {
                Util.ThrowIfFatal(ex);
                Log.ErrorException(ex,
                    "評価値ダイアログの表示に失敗しました；；");
            }
        }
        #endregion

        private readonly GLEvaluationElementManager manager =
            new GLEvaluationElementManager();
        private ContextMenuStrip contextMenu;
        private Score currentScore;
        private float valueHeight;
        private float valueTop;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public GLEvaluationElement()
        {
            this.valueHeight = 0.12f;
            this.valueTop = 0.5f - this.valueHeight + 0.04f;
            this.contextMenu = CreateContextMenu();

            // 評価値モードはデフォルトでは何でも許可します。
            EvaluationMode = EvaluationMode.Programmable;
            IsEnableUserScore = true;
            IsEnableServerScore = true;
            IsVisibleValue = true;
            IsValueFullWidth = false;
            MaxValue = 9999;

            ValueFont = new GLUtil.TextTextureFont
            {
                Color = Color.White,
                EdgeColor = Color.LightGray,
                EdgeLength = 0.5,
            };

            // 内部描画オブジェクトの初期化
            AddInternalType("image", typeof(GLEvaluationElementImage));

            this.AddPropertyChangedHandler(
                "ImageSetList",
                (_, __) => this.manager.SetImageSetList(ImageSetList));
            this.AddPropertyChangedHandler(
                "ImageSet",
                (_, __) => ImageSetUpdated());
            this.AddPropertyChangedHandler(
                "EvaluationMode",
                (_, __) => EvaluationModeUpdated());

            // CurrentScoreの場合はDependOnProperty属性で指定している
            // プロパティが変わった時に、これが呼ばれます。
            this.AddPropertyChangedHandler(
                "CurrentScore",
                (_, __) => UpdateCurrentScore());
        }

        /// <summary>
        /// コンテキストメニューを作成します。
        /// </summary>
        private ContextMenuStrip CreateContextMenu()
        {
            var contextMenu = new ContextMenuStrip();

            // 評価値の設定ダイアログ
            var item1 = new ToolStripMenuItem
            {
                Text = "設定",
            };
            item1.Click += (_, __) =>
                OpenEvaluationSettingDialog.Execute(this);
            contextMenu.Items.Add(item1);

            // セパレータ
            contextMenu.Items.Add(new ToolStripSeparator());

            // 各項目の設定
            var item2 = new ToolStripMenuItem
            {
                Text = "値を表示する",
                CheckOnClick = true,
                Checked = true,
            };
            item2.Click += (_, __) =>
                IsVisibleValue = item2.Checked;
            this.AddPropertyChangedHandler(
                "IsVisibleValue",
                (_, __) => item2.Checked = IsVisibleValue);
            contextMenu.Items.Add(item2);

            // 各項目の設定
            var item3 = new ToolStripMenuItem
            {
                Text = "値を全角文字で表示する",
                CheckOnClick = true,
                Checked = IsValueFullWidth,
            };
            item3.Click += (_, __) =>
                IsValueFullWidth = item3.Checked;
            this.AddPropertyChangedHandler(
                "IsValueFullWidth",
                (_, __) => item3.Checked = IsValueFullWidth);
            contextMenu.Items.Add(item3);

            return contextMenu;
        }

        /// <summary>
        /// 評価値の選択モードを取得または設定します。
        /// </summary>
        /// <remarks>
        /// EvaluationModeの値により、使用される評価値が変わります。
        /// 
        /// 1, Programmableの場合 ProgrammableValueが使われます。
        /// 2, Userの場合         UserValueが使われます。
        /// 3, Serverの場合       指定の評価値サーバーから得られた値を使います。
        /// </remarks>
        public EvaluationMode EvaluationMode
        {
            get { return GetValue<EvaluationMode>("EvaluationMode"); }
            set { SetValue("EvaluationMode", value); }
        }

        /// <summary>
        /// 表示する値の最大値を取得または設定します。
        /// </summary>
        public int MaxValue
        {
            get { return GetValue<int>("MaxValue"); }
            set { SetValue("MaxValue", value); }
        }

        /// <summary>
        /// プログラムから設定可能な評価値を取得または設定します。
        /// </summary>
        public Score ProgrammableScore
        {
            get { return GetValue<Score>("ProgrammableScore"); }
            set { SetValue("ProgrammableScore", value); }
        }

        /// <summary>
        /// ユーザーからの手入力値を有効にするかどうかを取得または設定します。
        /// </summary>
        /// <remarks>
        /// 主に設定ダイアログで使われます。
        /// </remarks>
        public bool IsEnableUserScore
        {
            get { return GetValue<bool>("IsEnableUserScore"); }
            set { SetValue("IsEnableUserScore", value); }
        }

        /// <summary>
        /// ユーザーからの手入力値を取得または設定します。
        /// </summary>
        public Score UserScore
        {
            get { return GetValue<Score>("UserScore"); }
            set { SetValue("UserScore", value); }
        }

        /// <summary>
        /// 評価値サーバーからの入力値を有効にするかどうかを取得または設定します。
        /// </summary>
        /// <remarks>
        /// 主に設定ダイアログで使われます。
        /// </remarks>
        public bool IsEnableServerScore
        {
            get { return GetValue<bool>("IsEnableServerScore"); }
            set { SetValue("isEnableServerValue", value); }
        }

        /// <summary>
        /// 評価値サーバーの評価値を取得または設定します。
        /// </summary>
        public Score ServerScore
        {
            get { return GetValue<Score>("ServerScore"); }
            set { SetValue("ServerScore", value); }
        }

        /// <summary>
        /// 実際に使われる評価値を取得します。
        /// </summary>
        /// <remarks>
        /// CurrentScoreのPropertyChangedが呼ばれるのは
        /// 関連プロパティが変更されたときのみです。
        /// 
        /// CurrentScoreへの直接的な代入では
        /// CurrentScoreのPropertyChangedが呼ばれないようにします。
        /// そうしないと、PropertyChangedでCurrentScoreを再設定
        /// することによる無限ループが発生します。
        /// </remarks>
        [DependOnProperty("MaxValue")]
        [DependOnProperty("ProgrammableScore")]
        [DependOnProperty("UserScore")]
        [DependOnProperty("ServerScore")]
        [DependOnProperty("EvaluationMode")]
        public Score CurrentScore
        {
            get { return this.currentScore; }
            private set { this.currentScore = value; }
        }

        /// <summary>
        /// 評価値の数字を表示するかどうかを取得または設定します。
        /// </summary>
        public bool IsVisibleValue
        {
            get { return GetValue<bool>("IsVisibleValue"); }
            set { SetValue("IsVisibleValue", value); }
        }

        /// <summary>
        /// 評価値を全角文字で表示するかどうかを取得または設定します。
        /// </summary>
        public bool IsValueFullWidth
        {
            get { return GetValue<bool>("IsValueFullWidth"); }
            set { SetValue("IsValueFullWidth", value); }
        }

        /// <summary>
        /// 評価値の数字を描画するためのフォントを取得または設定します。
        /// </summary>
        public GLUtil.TextTextureFont ValueFont
        {
            get { return GetValue<GLUtil.TextTextureFont>("ValueFont"); }
            set { SetValue("ValueFont", value); }
        }

        /// <summary>
        /// 表示する評価値画像のセットを取得または設定します。
        /// </summary>
        public ImageSetInfo ImageSet
        {
            get { return GetValue<ImageSetInfo>("ImageSet"); }
            set { SetValue("ImageSet", value); }
        }

        /// <summary>
        /// 表示する評価値画像セットのリストを取得または設定します。
        /// </summary>
        /// <remarks>
        /// 付属の設定ダイアログから評価値画像セットを選択することができます。
        /// </remarks>
        public List<ImageSetInfo> ImageSetList
        {
            get { return GetValue<List<ImageSetInfo>>("ImageSetList"); }
            set { SetValue("ImageSetList", value); }
        }

        /// <summary>
        /// 評価値とリンクする絵を描画する内部型の設定を行います。
        /// </summary>
        public void AddInternalType(string name, Type internalType)
        {
            this.manager.AddInternalType(name, internalType);
        }

        /// <summary>
        /// 表示セットが更新されたときに呼ばれます。
        /// </summary>
        private void ImageSetUpdated()
        {
            var prev = this.manager.InternalObj;
            this.manager.SetImageSet(ImageSet);
            var next = this.manager.InternalObj;

            if (!ReferenceEquals(prev, next))
            {
                if (prev != null)
                {
                    Children.Remove(prev);
                }

                if (next != null)
                {
                    Children.Add(next);
                }
            }
        }

        /// <summary>
        /// 評価値モードが変わった時に呼ばれます。
        /// </summary>
        private void EvaluationModeUpdated()
        {
            // 設定された評価値モードが正しいかどうかを確認します。
            if (EvaluationMode == EvaluationMode.User && !IsEnableUserScore)
            {
                EvaluationMode = EvaluationMode.Programmable;

                throw new InvalidOperationException(
                    "EvaluationMode=Userは許可されていない値です。");
            }

            if (EvaluationMode == EvaluationMode.Server && !IsEnableServerScore)
            {
                EvaluationMode = EvaluationMode.Programmable;

                throw new InvalidOperationException(
                    "EvaluationMode=Serverは許可されていない値です。");
            }
        }

        /// <summary>
        /// 現在の評価値を評価値モードに合わせた値に更新します。
        /// </summary>
        private void UpdateCurrentScore()
        {
            Score score = null;

            switch (EvaluationMode)
            {
                case EvaluationMode.Programmable:
                    score = ProgrammableScore;
                    break;
                case EvaluationMode.User:
                    score = UserScore;
                    break;
                case EvaluationMode.Server:
                    score = ServerScore;
                    break;
                default:
                    
                    break;
            }

            score = score ?? new Score();
            CurrentScore = score; // MathEx.Between(-MaxValue, MaxValue, value);
        }
        
        /// <summary>
        /// 毎フレーム呼ばれる更新用メソッドです。
        /// </summary>
        protected override void OnEnterFrame(EnterFrameEventArgs e)
        {
            base.OnEnterFrame(e);
            var renderBuffer = (GLUtil.RenderBuffer)e.StateObject;

            // 評価値の更新を行います。
            if (this.manager.InternalObj != null)
            {
                this.manager.InternalObj.CurrentScore = CurrentScore;
            }

            // 評価値を表示する場合は、数字の描画を行います。
            if (IsVisible && IsVisibleValue)
            {
                AddRenderValue(renderBuffer, CurrentScore);
            }
        }

        /// <summary>
        /// 評価値を数字として描画リストに加えます。
        /// </summary>
        private void AddRenderValue(GLUtil.RenderBuffer renderBuffer, Score score)
        {
            var textTexture = GLUtil.TextureCache.GetTextTexture(
                score.Value.ToString(),
                ValueFont);
            var texture = textTexture.Texture;

            var textTexture2 = GLUtil.TextureCache.GetTextTexture(
                score.Turn == BWType.Black ? "先手" :
                score.Turn == BWType.White ? "後手" : "",
                ValueFont);
            var texture2 = textTexture2.Texture;

            if (texture != null && texture.IsAvailable)
            {
                var Margin = 0.02f;

                // 描画領域はテクスチャサイズの割合から決定します。
                // 評価値は画像の下部or上部の
                // 横幅が全体の 3/4 以上になるか、
                // 縦幅が全体の 1/5 以上になるまで
                // 拡大します。拡大は縦横比を保存した状態で行います。

                // フォントの描画サイズを全体の高さからの割合で指定。
                var eh = this.valueHeight;
                var ew = eh * texture.OriginalWidth / texture.OriginalHeight; 

                // 文字幅がエレメントサイズを超えていたら、
                // 文字列サイズの調整を行います。
                if (ew > 0.9f)
                {
                    ew = 0.9f;
                    eh = ew * texture.OriginalHeight / texture.OriginalWidth;
                }

                // 評価値の背景描画
                var bounds = new RectangleF(
                    -0.5f, this.valueTop, 1.0f, this.valueHeight);
                renderBuffer.AddRender(
                    BlendType.Diffuse, Color.FromArgb(128, Color.Black),
                    bounds, Transform, 1.0);

                // 先手後手の描画
                bounds = new RectangleF(
                    -0.5f + Margin, this.valueTop, eh * texture2.OriginalWidth / texture2.OriginalHeight, eh);
                renderBuffer.AddRender(
                    texture2, BlendType.Diffuse,
                    bounds, Transform, 1.0);

                // 評価値の描画
                bounds = new RectangleF(
                    0.5f - Margin - ew, this.valueTop, ew, eh);
                renderBuffer.AddRender(
                    texture, BlendType.Diffuse,
                    bounds, Transform, 1.0);
            }
        }

        public override void OnMouseDown(MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                var p = ClientToLocal(e.Location);
                var r = new RectangleF(-0.5f, -0.5f, 1.0f, 1.0f);
                if (r.Contains(p))
                {
                    // 右クリックメニューを開きます。
                    this.contextMenu.Show(Cursor.Position);
                }
            }

            base.OnMouseDown(e);
        }
    }
}
