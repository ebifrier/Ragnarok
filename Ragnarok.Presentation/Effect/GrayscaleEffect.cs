using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace Ragnarok.Presentation.Effect
{
    /// <summary>
    /// グレースケールに変換するためのエフェクトです。
    /// </summary>
    public class GrayscaleEffect : ShaderEffect
    {
        private static readonly PixelShader pixelShader;

        /// <summary>
        /// 入力画像を扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty InputProperty =
            ShaderEffect.RegisterPixelShaderSamplerProperty(
                "Input", typeof(GrayscaleEffect), 0);

        /// <summary>
        /// 入力画像を取得または設定します。
        /// </summary>
        public Brush Input
        {
            get { return (Brush)GetValue(InputProperty); }
            set { SetValue(InputProperty, value); }
        }

        /// <summary>
        /// 係数を扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty DesaturationFactorProperty =
            DependencyProperty.Register(
                "DesaturationFactor", typeof(double), typeof(GrayscaleEffect),
                new UIPropertyMetadata(0.0,
                    PixelShaderConstantCallback(0),
                    CoerceDesaturationFactor));

        /// <summary>
        /// 係数を取得または設定します。
        /// </summary>
        public double DesaturationFactor
        {
            get { return (double)GetValue(DesaturationFactorProperty); }
            set { SetValue(DesaturationFactorProperty, value); }
        }

        private static object CoerceDesaturationFactor(DependencyObject d, object value)
        {
            GrayscaleEffect effect = (GrayscaleEffect)d;
            double newFactor = (double)value;

            if (newFactor < 0.0 || newFactor > 1.0)
            {
                return effect.DesaturationFactor;
            }

            return newFactor;
        }

        /// <summary>
        /// 静的コンストラクタ
        /// </summary>
        static GrayscaleEffect()
        {
            var uri =
                @"pack://application:,,,/Ragnarok.Presentation;" +
                @"component/Effect/GrayscaleEffect.ps";

            try
            {
                pixelShader = new PixelShader()
                {
                    UriSource = new Uri(uri)
                };
            }
            catch (Exception ex)
            {
                Log.ErrorException(ex,
                    "PixelShaderの初期化に失敗しました。({0}を開くことができません)",
                    uri);
            }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public GrayscaleEffect()
        {
            PixelShader = pixelShader;

            if (PixelShader != null)
            {
                UpdateShaderValue(InputProperty);
                UpdateShaderValue(DesaturationFactorProperty);
            }
        }
    }
}
