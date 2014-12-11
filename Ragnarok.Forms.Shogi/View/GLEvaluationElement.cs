using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SharpGL;

using Ragnarok;
using Ragnarok.Extra.Effect;
using Ragnarok.Forms.Input;
using Ragnarok.ObjectModel;
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

        private ContextMenuStrip contextMenu;
        private float imageHeight;
        private float valueTop;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public GLEvaluationElement()
        {
            this.imageHeight = 135.0f / 145.0f;
            this.valueTop = this.imageHeight - 0.04f;
            this.contextMenu = CreateContextMenu();

            // 評価値モードはデフォルトでは何でも許可します。
            EvaluationMode = EvaluationMode.Programmable;
            IsEnableUserValue = true;
            IsEnableServerValue = true;
            IsVisibleValue = true;
            IsValueFullWidth = true;

            LocalTransform = new Matrix44d();
            LocalTransform.Translate(-0.5, -0.5, 0.0);

            ValueFont = new GL.TextTextureFont
            {
                Color = Color.White,
                EdgeColor = Color.LightGray,
                EdgeLength = 0.5,
            };

            this.AddPropertyChangedHandler(
                "EvaluationMode",
                (_, __) => EvaluationModeUpdated());

            // CurrentValueの場合はDependOnProperty属性で指定している
            // プロパティが変わった時に、これが呼ばれます。
            this.AddPropertyChangedHandler(
                "CurrentValue",
                (_, __) => UpdateCurrentValue());
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
                Text = "評価値の設定",
            };
            item1.Click += (_, __) =>
                OpenEvaluationSettingDialog.Execute(this);
            contextMenu.Items.Add(item1);

            // セパレータ
            contextMenu.Items.Add(new ToolStripSeparator());

            // 各項目の設定
            var item2 = new ToolStripMenuItem
            {
                Text = "数字を表示する",
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
                Text = "数字を全角文字で表示する",
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
        /// プログラムから設定可能な評価値を取得または設定します。
        /// </summary>
        public int ProgrammableValue
        {
            get { return GetValue<int>("ProgrammableValue"); }
            set { SetValue("ProgrammableValue", value); }
        }

        /// <summary>
        /// ユーザーからの手入力値を有効にするかどうかを取得または設定します。
        /// </summary>
        /// <remarks>
        /// 主に設定ダイアログで使われます。
        /// </remarks>
        public bool IsEnableUserValue
        {
            get { return GetValue<bool>("IsEnableUserValue"); }
            set { SetValue("IsEnableUserValue", value); }
        }

        /// <summary>
        /// ユーザーからの手入力値を取得または設定します。
        /// </summary>
        public int UserValue
        {
            get { return GetValue<int>("UserValue"); }
            set { SetValue("UserValue", value); }
        }

        /// <summary>
        /// 評価値サーバーからの入力値を有効にするかどうかを取得または設定します。
        /// </summary>
        /// <remarks>
        /// 主に設定ダイアログで使われます。
        /// </remarks>
        public bool IsEnableServerValue
        {
            get { return GetValue<bool>("IsEnableServerValue"); }
            set { SetValue("isEnableServerValue", value); }
        }

        /// <summary>
        /// 評価値サーバーの評価値を取得または設定します。
        /// </summary>
        public int ServerValue
        {
            get { return GetValue<int>("ServerValue"); }
            set { SetValue("ServerValue", value); }
        }

        /// <summary>
        /// 実際に使われる評価値を取得します。
        /// </summary>
        [DependOnProperty("ProgrammableValue")]
        [DependOnProperty("UserValue")]
        [DependOnProperty("ServerValue")]
        [DependOnProperty("EvaluationMode")]
        public int CurrentValue
        {
            get { return GetValue<int>("CurrentValue"); }
            set { SetValue("CurrentValue", value); }
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
        public GL.TextTextureFont ValueFont
        {
            get { return GetValue<GL.TextTextureFont>("ValueFont"); }
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
        /// 評価値モードが変わった時に呼ばれます。
        /// </summary>
        public void EvaluationModeUpdated()
        {
            // 設定された評価値モードが正しいかどうかを確認します。
            if (EvaluationMode == EvaluationMode.User && !IsEnableUserValue)
            {
                EvaluationMode = EvaluationMode.Programmable;

                throw new InvalidOperationException(
                    "EvaluationMode=Userは許可されていない値です。");
            }

            if (EvaluationMode == EvaluationMode.Server && !IsEnableServerValue)
            {
                EvaluationMode = EvaluationMode.Programmable;

                throw new InvalidOperationException(
                    "EvaluationMode=Serverは許可されていない値です。");
            }
        }

        /// <summary>
        /// 現在の評価値を評価値モードに合わせた値に更新します。
        /// </summary>
        private void UpdateCurrentValue()
        {
            switch (EvaluationMode)
            {
                case EvaluationMode.Programmable:
                    CurrentValue = ProgrammableValue;
                    break;
                case EvaluationMode.User:
                    CurrentValue = UserValue;
                    break;
                case EvaluationMode.Server:
                    CurrentValue = ServerValue;
                    break;
            }
        }

        /// <summary>
        /// 毎フレーム呼ばれる更新用メソッドです。
        /// </summary>
        protected override void OnEnterFrame(EnterFrameEventArgs e)
        {
            base.OnEnterFrame(e);
            var renderBuffer = (GL.RenderBuffer)e.StateObject;

            // 評価値画像の描画を行います。
            AddRenderImage(renderBuffer);

            // 評価値を表示する場合は、数字の描画を行います。
            if (IsVisibleValue)
            {
                AddRenderValue(renderBuffer, CurrentValue);
            }
        }

        /// <summary>
        /// 評価値画像を描画リストに加えます。
        /// </summary>
        private void AddRenderImage(GL.RenderBuffer renderBuffer)
        {
            var gl = OpenGL;

            var imageSet = ImageSet;
            if (imageSet == null)
            {
                // 評価値画像のセットがない場合は、何も表示しません。
                return;
            }

            var imagePath = imageSet.GetSelectedImagePath(CurrentValue);
            if (string.IsNullOrEmpty(imagePath))
            {
                // 評価値画像がない場合も、何もしません。
                return;
            }

            // 描画領域はこのクラスの外側で指定します。
            var bounds = new RectangleF(0.0f, 0.0f, 1.0f, this.imageHeight);

            // 描画領域を設定します。
            var texture = GL.TextureCache.GetTexture(gl, new Uri(imagePath));
            if (texture != null && texture.IsAvailable)
            {
                renderBuffer.AddRender(
                    texture, BlendType.Diffuse, bounds, Transform, 0.0);
            }
        }

        /// <summary>
        /// 評価値を数字として描画リストに加えます。
        /// </summary>
        private void AddRenderValue(GL.RenderBuffer renderBuffer, int value)
        {
            var gl = OpenGL;

            var text = (
                IsValueFullWidth ?
                Ragnarok.Utility.StringConverter.ConvertInt(NumberType.Big, value) :
                value.ToString());
            var textTexture = GL.TextureCache.GetTextTexture(
                gl, text, ValueFont);
            var texture = textTexture.Texture;

            if (texture != null && texture.IsAvailable)
            {                
                var Margin = 0.02f;

                // 描画領域はテクスチャサイズの割合から決定します。
                // 評価値は画像の下部or上部の
                // 横幅が全体の 3/4 以上になるか、
                // 縦幅が全体の 1/5 以上になるまで
                // 拡大します。拡大は縦横比を保存した状態で行います。

                // フォントの描画サイズを全体の高さからの割合で指定。
                var eh = 1.0f - this.valueTop;
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
                    0.0f, this.valueTop, 1.0f, 1.0f - this.valueTop);
                renderBuffer.AddRender(
                    BlendType.Diffuse, Color.FromArgb(128, Color.Black),
                    bounds, Transform, 1.0);

                // 評価値の描画
                bounds = new RectangleF(
                    1.0f - Margin - ew, this.valueTop, ew, eh);
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
                var r = new RectangleF(0, 0, 1, 1);
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
