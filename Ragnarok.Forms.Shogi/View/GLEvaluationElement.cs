using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using SharpGL;

using Ragnarok;
using Ragnarok.Extra.Effect;
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
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public GLEvaluationElement()
        {
            // 評価値モードはデフォルトでは何でも許可します。
            EvaluationMode = EvaluationMode.Programmable;
            IsEnableUserValue = true;
            IsEnableServerValue = true;
            IsVisibleValue = true;
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

        /*/// <summary>
        /// 評価値画像を表示するかどうかを取得または設定します。
        /// </summary>
        public bool IsVisibleImage
        {
            get { return GetValue<bool>("IsVisibleImage"); }
            set { SetValue("IsVisibleImage", value); }
        }*/

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
            var bounds = new RectangleF(-0.5f, -0.5f, 1.0f, 1.0f);

            // 描画領域を設定します。
            var texture = GL.TextureCache.GetTexture(gl, new Uri(imagePath));
            if (texture != null && texture.TextureName != 0)
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

            var text = value.ToString();
            var textTexture = GL.TextureCache.GetTextTexture(
                gl, text, ValueFont);

            if (textTexture != null)
            {
                // 描画するテクスチャを取得します。
                var texture = textTexture.Texture;

                // 描画領域はテクスチャサイズの割合から決定します。
                // 評価値は画像の下部or上部の
                // 横幅が全体の 3/4 以上になるか、
                // 縦幅が全体の 1/5 以上になるまで
                // 拡大します。拡大は縦横比を保存した状態で行います。

                // フォントの描画高さを全体の高さからの割合で指定。
                var eh = 1.0f / 8.0f;

                // エレメント上での描画座標を上記の高さの割合と、テクスチャのサイズから取得。
                var ew = eh * texture.OriginalWidth / texture.OriginalHeight; 

                // エレメント上での幅がエレメントサイズを超えていたら、
                // 文字列サイズの調整を行います。
                if (ew > 3.0f / 4.0f)
                {
                    ew = 3.0f / 4.0f;
                    eh = ew * texture.OriginalHeight / texture.OriginalWidth;
                }

                float Margin = 0.01f;

                // 文字背景
                var bounds = new RectangleF(
                    -0.5f + 1.0f / 4.0f, -0.5f, 1.0f, eh + 2 * Margin);
                renderBuffer.AddRender(
                    BlendType.Diffuse, Color.FromArgb(128, Color.Black),
                    bounds, Transform, 1.0);

                // 描画処理を追加します。
                bounds = new RectangleF(
                    0.5f - Margin - ew, -0.5f + Margin, ew, eh);
                renderBuffer.AddRender(
                    texture, BlendType.Diffuse,
                    bounds, Transform, 1.0);
            }
        }
    }
}
