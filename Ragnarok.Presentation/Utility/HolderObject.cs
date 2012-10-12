using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media.Animation;

namespace Ragnarok.Presentation.Utility
{
    /// <summary>
    /// intやdoubleなどの値を保持します。
    /// </summary>
    /// <remarks>
    /// obj.Valueで値の取得・設定がしたいことがあるため作りました。
    /// </remarks>
    public class HolderObject<T> : Animatable
    {
        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register(
                "Value", typeof(T), typeof(HolderObject<T>),
                new UIPropertyMetadata(default(T)));

        /// <summary>
        /// 値を取得または設定します。
        /// </summary>
        public T Value
        {
            get { return (T)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        /// <summary>
        /// 新しいインスタンスを作ります。
        /// </summary>
        protected override Freezable CreateInstanceCore()
        {
            return new HolderObject<T>();
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public HolderObject(T value)
        {
            Value = value;
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public HolderObject()
            : this(default(T))
        {
        }

        /// <summary>
        /// T型に変換します。
        /// </summary>
        public static explicit operator T(HolderObject<T> self)
        {
            return self.Value;
        }

        /// <summary>
        /// このオブジェクトに変換します。
        /// </summary>
        public static explicit operator HolderObject<T>(T value)
        {
            return new HolderObject<T>(value);
        }
    }

    /// <summary>
    /// doubleを扱うホルダーオブジェクトです。
    /// </summary>
    public class DoubleObject : HolderObject<double>
    {
    }
}
