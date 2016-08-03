using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;

using Ragnarok.Extra.Effect;
using Ragnarok.OpenGL;

namespace Ragnarok.Forms.Shogi.Effect
{
    /// <summary>
    /// テキスト表示用のオブジェクトです。
    /// </summary>
    public class ShogiTextObject : VisualEffect
    {
        private bool needToUpdateTexture;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public ShogiTextObject()
        {
            Font = new TextTextureFont();
            Color = Color.White;
            this.needToUpdateTexture = false;

            PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var name = e.PropertyName;
            if (name == "Font" || name == "Text")
            {
                this.needToUpdateTexture = true;
            }
        }

        #region 基本プロパティ
        /// <summary>
        /// 頂点カラーを取得または設定します。
        /// </summary>
        public TextTextureFont Font
        {
            get { return GetValue<TextTextureFont>("Font"); }
            set { SetValue("Font", value); }
        }

        /*/// <summary>
        /// 文字列の色を取得または設定します。
        /// </summary>
        [DependOnProperty("Font")]
        public Color FontColor
        {
            get { return Font.Color; }
            set { Font.Color = value; }
        }

        /// <summary>
        /// 縁取りの色を取得または設定します。
        /// </summary>
        [DependOnProperty("Font")]
        public Color EdgeColor
        {
            get { return Font.EdgeColor; }
            set { Font.EdgeColor = value; }
        }

        /// <summary>
        /// 縁取りの色を取得または設定します。
        /// </summary>
        [DependOnProperty("Font")]
        public double EdgeLength
        {
            get { return Font.EdgeLength; }
            set { Font.EdgeLength = value; }
        }

        /// <summary>
        /// 外側の空白を削除し、ビットマップいっぱいに文字を
        /// 描画するかどうかを取得または設定します。
        /// </summary>
        [DependOnProperty("Font")]
        public bool IsStretchSize
        {
            get { return Font.IsStretchSize; }
            set { Font.IsStretchSize = value; }
        }*/

        /// <summary>
        /// 描画時に使うブレンド色を取得または設定します。
        /// </summary>
        public Color Color
        {
            get { return GetValue<Color>("Color"); }
            set { SetValue("Color", value); }
        }

        /// <summary>
        /// 描画するテキストを取得または設定します。
        /// </summary>
        public string Text
        {
            get { return GetValue<string>("Text"); }
            set { SetValue("Text", value); }
        }

        /// <summary>
        /// 描画するテクスチャを取得または設定します。
        /// </summary>
        public TextTexture TextTexture
        {
            get { return GetValue<TextTexture>("TextTexture"); }
            set { SetValue("TextTexture", value); }
        }
        #endregion

        /// <summary>
        /// テキスト画像を更新します。
        /// </summary>
        private void UpdateTexture()
        {
            var texture = TextureCache.GetTextTexture(Text, Font);
            if (texture == null)
            {
                // 取得できない場合は諦める
                TextTexture = null;
                return;
            }

            // メッシュがない場合はデフォルトのメッシュで初期化します。
            if (Mesh == null)
            {
                Mesh = Mesh.CreateDefault(1, 1, 0, 0);
            }

            TextTexture = texture;
        }

        /// <summary>
        /// フレーム毎の処理を行います。
        /// </summary>
        protected override void OnEnterFrame(EnterFrameEventArgs e)
        {
            var renderBuffer = (RenderBuffer)e.StateObject;

            base.OnEnterFrame(e);

            // 必要ならテクスチャの更新を行います。
            if (this.needToUpdateTexture)
            {
                UpdateTexture();
                this.needToUpdateTexture = false;
            }

            if (Mesh != null)
            {
                // 頂点カラーを算出します。
                var a = (byte)(Color.A * InheritedOpacity);
                var color = Color.FromArgb(a, Color);

                // 描画用オブジェクトとして登録します。
                renderBuffer.AddRender(
                    TextTexture.Texture, Blend, color,
                    Mesh, Transform, InheritedZOrder);
            }
        }

        /// <summary>
        /// 描画処理を行います。
        /// </summary>
        protected override void OnRender(EventArgs e)
        {
        }
    }
}
