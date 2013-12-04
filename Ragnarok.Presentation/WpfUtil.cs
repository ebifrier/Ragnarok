using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Runtime.InteropServices;

namespace Ragnarok.Presentation
{
    using Ragnarok.Utility;

    /// <summary>
    /// ユーティリティクラスです。
    /// </summary>
    public static class WPFUtil
    {
        #region PInvoke
        [StructLayout(LayoutKind.Sequential)]
        private struct Win32Point
        {
            public Int32 X;
            public Int32 Y;
        };

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetCursorPos(ref Win32Point pt);
        #endregion

        /// <summary>
        /// WPFを使うための初期化処理を行います。
        /// </summary>
        public static void Init()
        {
            Initializer.Initialize();

            Util.SetPropertyChangedCaller(CallPropertyChanged);
            Util.SetColletionChangedCaller(CallCollectionChanged);
            Util.SetEventCaller(UIProcess);
        }

        /// <summary>
        /// デザインモードかどうかを取得します。
        /// (running in Blend or Visual Studio).
        /// </summary>
        public static bool IsInDesignMode
        {
            get
            {
                var prop = DesignerProperties.IsInDesignModeProperty;
                var desc = DependencyPropertyDescriptor.FromProperty(
                    prop, typeof(FrameworkElement));

                return (bool)desc.Metadata.DefaultValue;
            }
        }

        /// <summary>
        /// UIスレッドに関連づけられたディスパッチャーを取得します。
        /// </summary>
        public static Dispatcher UIDispatcher
        {
            get
            {
                if (Application.Current == null)
                {
                    return null;
                }

                return Application.Current.Dispatcher;
            }
        }

        /// <summary>
        /// 与えられた手続きをUIスレッド上で実行します。
        /// </summary>
        public static void UIProcess(Action func)
        {
            var dispatcher = WPFUtil.UIDispatcher;

            if (dispatcher == null || dispatcher.CheckAccess())
            {
                func();
            }
            else
            {
                dispatcher.BeginInvoke(func);
            }
        }

        /// <summary>
        /// コマンドバインダをすべて更新します。
        /// </summary>
        public static void InvalidateCommand()
        {
            UIProcess(CommandManager.InvalidateRequerySuggested);
        }

        /// <summary>
        /// <paramref name="type"/>が持つ全コマンドをバインディングします。
        /// </summary>
        public static void BindCommands(Type type, CommandBindingCollection commands)
        {
            var fieldFlags =
                BindingFlags.Static |
                BindingFlags.GetField |
                BindingFlags.Public;
            var fieldCommands = type
                .GetFields(fieldFlags)
                .Select(_ => _.GetValue(null));

            var propertyFlags =
                BindingFlags.Static |
                BindingFlags.GetProperty |
                BindingFlags.Public;
            var propertyCommands = type
                .GetProperties(propertyFlags)
                .Select(_ => _.GetValue(null, null));

            fieldCommands.Concat(propertyCommands)
                .OfType<ICommand>()
                .Where(_ => _ != null)
                .Select(_ => new CommandBinding(_))
                .ForEach(_ => commands.Add(_));
        }

        /// <summary>
        /// Mouse.GetPositionにはバグがあるので、P/Invokeでマウス座標を取ります。
        /// </summary>
        /// <remarks>
        /// マウスのDragAndDrop中やキャプチャ中に座標を取ろうとすると
        /// 間違った値を返してきます。
        /// 
        /// 参考: http://www.switchonthecode.com/tutorials/wpf-snippet-reliably-getting-the-mouse-position
        /// </remarks>
        public static Point GetMousePosition(Visual relativeTo)
        {
            var w32Mouse = new Win32Point();
            GetCursorPos(ref w32Mouse);

            return relativeTo.PointFromScreen(new Point(w32Mouse.X, w32Mouse.Y));
        }

        /// <summary>
        /// 透明度だけを変えた色を作成します。
        /// </summary>
        public static Color MakeColor(byte a, Color baseColor)
        {
            return Color.FromArgb(
                a,
                baseColor.R,
                baseColor.G,
                baseColor.B);
        }

        /// <summary>
        /// Vector3Dを作成します。
        /// </summary>
        public static Vector3D MakeVector3D(Point v, double z)
        {
            return new Vector3D(v.X, v.Y, z);
        }

        /// <summary>
        /// Vector3Dを作成します。
        /// </summary>
        public static Vector3D MakeVector3D(Vector v, double z)
        {
            return new Vector3D(v.X, v.Y, z);
        }

        /// <summary>
        /// Sizeを作成します。
        /// </summary>
        public static Size MakeSizeXY(Size3D s)
        {
            return new Size(s.X, s.Y);
        }

        /// <summary>
        /// Rectを作成します。
        /// </summary>
        public static Rect MakeRectXY(Rect3D r)
        {
            return new Rect(r.X, r.Y, r.SizeX, r.SizeY);
        }

        /// <summary>
        /// プロパティ名から依存プロパティを検索します。
        /// </summary>
        private static DependencyProperty GetDepPropertyImpl(Type sourceType,
                                                             string propertyName)
        {
            const BindingFlags flags =
                BindingFlags.Static |
                BindingFlags.GetField |
                BindingFlags.Public;

            var classes = MethodUtil.GetThisAndInheritClasses(sourceType);

            var propertyList = classes
                .SelectMany(_ => _.GetFields(flags))
                .Select(_ => _.GetValue(null) as DependencyProperty)
                .Where(_ => _ != null);

            return propertyList.FirstOrDefault(_ => _.Name == propertyName);
        }

        /// <summary>
        /// プロパティ名から依存プロパティを検索します。
        /// </summary>
        /// <example>
        /// Foo.Bar.X のような指定からX依存プロパティを返します。
        /// </example>
        public static DependencyProperty GetDependencyProperty(Type sourceType,
                                                               string propertyPath)
        {
            if (string.IsNullOrEmpty(propertyPath))
            {
                return null;
            }

            var Splitter = new[] { "." };
            var sourceProperties = propertyPath.Split(Splitter, StringSplitOptions.None);
            var propertyType = sourceType;
            DependencyProperty property = null;

            foreach (var name in sourceProperties)
            {
                property = GetDepPropertyImpl(propertyType, name);
                if (property == null)
                {
                    throw new InvalidOperationException(
                        string.Format(
                            "{0}プロパティが見つかりませんでした。", name));
                }

                propertyType = property.PropertyType;
            }

            return property;
        }

        /// <summary>
        /// 単純な四角形のジオメトリを作成します。
        /// </summary>
        public static MeshGeometry3D CreateDefaultMesh(double width, double height,
                                                       double imageWidth,
                                                       double imageHeight)
        {
            var halfPixelW = (imageWidth != 0.0 ? 0.5 / imageWidth : 0.0);
            var halfPixelH = (imageHeight != 0.0 ? 0.5 / imageHeight : 0.0);

            return new MeshGeometry3D
            {
                Positions =
                {
                    new Point3D(-width / 2, -height / 2, 0),
                    new Point3D(width / 2, -height / 2, 0),
                    new Point3D(-width / 2, height / 2, 0),
                    new Point3D(width / 2, height / 2, 0),
                },
                TextureCoordinates =
                {
                    new Point(0.0,              0.0),
                    new Point(1.0 - halfPixelW, 0.0),
                    new Point(0.0,              1.0 - halfPixelH),
                    new Point(1.0 - halfPixelW, 1.0 - halfPixelH),
                },
                TriangleIndices =
                {
                    0, 2, 1,
                    1, 2, 3,
                },
            };
        }

        #region コリジョン
        /// <summary>
        /// p0とp1で結ばれる直線と点cの距離を計算します。
        /// </summary>
        /// <remarks>
        /// ベクトルを用いて距離を計算します。
        /// ベクトルL = P1 - P0
        /// 
        /// 線分上で点Cと直交する点をP = P0 + t * Lとすると、
        /// (P - C)・L = 0
        ///   (P0 - C + t * L)・L = 0
        ///   t * |L|^2 = - (P0 - C)・L
        /// 
        /// また、
        /// 距離d = |P - C|
        ///       = |(P0 - C) + t * L|
        /// 
        /// 参考：http://homepage2.nifty.com/mathfin/distance/vector.htm
        /// </remarks>
        public static double LineCircleDistance(Vector3D p0, Vector3D p1,
                                                Vector3D c)
        {
            var cp = p0 - c;
            var l = p1 - p0;
            var length2 = l.LengthSquared;

            if (length2 < double.Epsilon)
            {
                return cp.Length;
            }

            var t = -Vector3D.DotProduct(cp, l) / length2;

            // 線分と点の距離なので点Pは端点の外には出れません。
            t = MathEx.Between(0.0, 1.0, t);

            return (cp + t * l).Length;
        }
        #endregion

        /// <summary>
        /// コントロールから指定の型の子要素をすべて検索します。
        /// </summary>
        public static IEnumerable<TChild> GetChildren<TChild>(this DependencyObject parent)
            where TChild : DependencyObject
        {
            if ((object)parent == null)
            {
                yield return null;
            }

            var count = VisualTreeHelper.GetChildrenCount(parent);
            for (var i = 0; i < count; ++i)
            {
                var childDep = VisualTreeHelper.GetChild(parent, i);
                var child = childDep as TChild;

                if ((object)child != null)
                {
                    yield return child;
                }
                else
                {
                    // 子コントロールのさらに子コントロールを検索します。
                    foreach (var c in GetChildren<TChild>(childDep))
                    {
                        yield return c;
                    }
                }
            }
        }

        /// <summary>
        /// コントロールから指定の型の子要素を検索します。
        /// </summary>
        public static TChild GetChild<TChild>(this DependencyObject parent)
            where TChild : DependencyObject
        {
            return GetChildren<TChild>(parent).FirstOrDefault();
        }

        /// <summary>
        /// 必要ならGUIスレッド上でPropertyChangedを呼び出します。
        /// </summary>
        public static void CallPropertyChanged(PropertyChangedEventHandler handler,
                                               object sender,
                                               PropertyChangedEventArgs e)
        {
            if (handler == null)
            {
                return;
            }

            // 個々のDelegate単位で呼び出すスレッドを変更します。
            foreach (PropertyChangedEventHandler child in
                     handler.GetInvocationList())
            {
                var target = child.Target as DispatcherObject;

                try
                {
                    // 必要があれば指定のスレッド上で実行します。
                    if (target != null && !target.Dispatcher.CheckAccess())
                    {
                        target.Dispatcher.BeginInvoke(
                            child,
                            sender, e);
                    }
                    else
                    {
                        child(sender, e);
                    }
                }
                catch (Exception ex)
                {
                    Log.ErrorException(ex,
                        "PropertyChangedの呼び出しに失敗しました。");
                }
            }
        }

        /// <summary>
        /// 必要ならGUIスレッド上でCollectionChangedを呼び出します。
        /// </summary>
        public static void CallCollectionChanged(NotifyCollectionChangedEventHandler handler,
                                                 object sender,
                                                 NotifyCollectionChangedEventArgs e)
        {
            if (handler == null)
            {
                return;
            }

            // 個々のDelegate単位で呼び出すスレッドを変更します。
            foreach (NotifyCollectionChangedEventHandler child in
                     handler.GetInvocationList())
            {
                var target = child.Target as DispatcherObject;

                try
                {
                    // 必要があれば指定のスレッド上で実行します。
                    if (target != null && !target.Dispatcher.CheckAccess())
                    {
                        // コレクションの状態が変わる前に変更通知を出す
                        // 必要があるため、Invokeを使っています。
                        target.Dispatcher.Invoke(
                            child,
                            sender, e);
                    }
                    else
                    {
                        child(sender, e);
                    }
                }
                catch (Exception ex)
                {
                    Log.ErrorException(ex,
                        "CollectionChangedの呼び出しに失敗しました。");
                }
            }
        }
    }
}
