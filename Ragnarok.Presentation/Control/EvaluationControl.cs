using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using Ragnarok.Utility;

namespace Ragnarok.Presentation.Control
{
    using Utility;

    /// <summary>
    /// 評価値の取得元を識別します。
    /// </summary>
    public enum EvaluationPointType
    {
        /// <summary>
        /// 手入力値を使います。
        /// </summary>
        ManualInput,
        /// <summary>
        /// ユーザーの評価値の平均値を使います。
        /// </summary>
        User,
        /// <summary>
        /// 各モード固有の値を使います。
        /// </summary>
        ModeCustom,
    }

    /// <summary>
    /// 評価値ウィンドウです。
    /// </summary>
    public class EvaluationControl : UserControl
    {
        #region 評価値関係
        /// <summary>
        /// 評価値の取得元を扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty PointTypeProperty =
            DependencyProperty.Register(
                "PointType",
                typeof(EvaluationPointType),
                typeof(EvaluationControl),
                new FrameworkPropertyMetadata(EvaluationPointType.User,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnPointChanged));

        /// <summary>
        /// 評価値の取得元を取得または設定します。
        /// </summary>
        public EvaluationPointType PointType
        {
            get { return (EvaluationPointType)GetValue(PointTypeProperty); }
            set { SetValue(PointTypeProperty, value); }
        }

        /// <summary>
        /// 手入力による評価値を扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty ManualInputPointProperty =
            DependencyProperty.Register(
                "ManualInputPoint",
                typeof(double),
                typeof(EvaluationControl),
                new FrameworkPropertyMetadata(0.0,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnPointChanged, CoercePoint));

        /// <summary>
        /// 手入力による評価値を取得または設定します。
        /// </summary>
        public double ManualInputPoint
        {
            get { return (double)GetValue(ManualInputPointProperty); }
            set { SetValue(ManualInputPointProperty, value); }
        }

        /// <summary>
        /// リスナーによる評価値を扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty UserPointProperty =
            DependencyProperty.Register(
                "UserPoint",
                typeof(double),
                typeof(EvaluationControl),
                new FrameworkPropertyMetadata(0.0, OnPointChanged, CoercePoint));

        /// <summary>
        /// リスナーによる評価値を取得または設定します。
        /// </summary>
        public double UserPoint
        {
            get { return (double)GetValue(UserPointProperty); }
            set { SetValue(UserPointProperty, value); }
        }

        /// <summary>
        /// 各モードに固有の評価値を扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty ModeCustomPointProperty =
            DependencyProperty.Register(
                "ModeCustomPoint",
                typeof(double),
                typeof(EvaluationControl),
                new FrameworkPropertyMetadata(0.0, OnPointChanged, CoercePoint));

        /// <summary>
        /// 各モードに固有の評価値を取得または設定します。
        /// </summary>
        public double ModeCustomPoint
        {
            get { return (double)GetValue(ModeCustomPointProperty); }
            set { SetValue(ModeCustomPointProperty, value); }
        }

        /// <summary>
        /// 表示される評価値を扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty EvaluationPointProperty =
            DependencyProperty.Register(
                "EvaluationPoint",
                typeof(double),
                typeof(EvaluationControl),
                new FrameworkPropertyMetadata(0.0));

        /// <summary>
        /// 表示される評価値を取得します。
        /// </summary>
        public double EvaluationPoint
        {
            get { return (double)GetValue(EvaluationPointProperty); }
            private set { SetValue(EvaluationPointProperty, value); }
        }

        static void OnPointChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = (EvaluationControl)d;

            self.UpdatePoint();
        }

        static object CoercePoint(DependencyObject d, object v)
        {
            var value = (double)v;

            return MathEx.Between(-9999.0, 9999.0, value);
        }
        #endregion

        #region その他のプロパティ
        /// <summary>
        /// 画像セットのリストを扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty ImageSetListProperty =
            DependencyProperty.Register(
                "ImageSetList",
                typeof(List<ImageSetInfo>),
                typeof(EvaluationControl),
                new FrameworkPropertyMetadata(new List<ImageSetInfo>(),
                    OnImageSetListChanged));

        static void OnImageSetListChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = (EvaluationControl)d;

            self.UpdateImageSetList();
        }

        /// <summary>
        /// 画像セットのリストを取得または設定します。
        /// </summary>
        public List<ImageSetInfo> ImageSetList
        {
            get { return (List<ImageSetInfo>)GetValue(ImageSetListProperty); }
            set { SetValue(ImageSetListProperty, value); }
        }

        /// <summary>
        /// 選択された画像セットを扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty SelectedImageSetProperty =
            DependencyProperty.Register(
                "SelectedImageSet",
                typeof(ImageSetInfo),
                typeof(EvaluationControl),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnImageSetChanged));

        static void OnImageSetChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = (EvaluationControl)d;

            self.UpdateImageSet();
        }

        /// <summary>
        /// 選択された画像セットを取得または設定します。
        /// </summary>
        public ImageSetInfo SelectedImageSet
        {
            get { return (ImageSetInfo)GetValue(SelectedImageSetProperty); }
            set { SetValue(SelectedImageSetProperty, value); }
        }

        /// <summary>
        /// 選択された画像セット名を扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty SelectedImageSetTitleProperty =
            DependencyProperty.Register(
                "SelectedImageSetTitle",
                typeof(string),
                typeof(EvaluationControl),
                new FrameworkPropertyMetadata(string.Empty,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnImageSetTitleChanged));

        static void OnImageSetTitleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = (EvaluationControl)d;

            self.UpdateImageSetTitle();
        }

        /// <summary>
        /// 選択された画像セット名を取得または設定します。
        /// </summary>
        public string SelectedImageSetTitle
        {
            get { return (string)GetValue(SelectedImageSetTitleProperty); }
            set { SetValue(SelectedImageSetTitleProperty, value); }
        }

        /// <summary>
        /// 選択された画像のパスを扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty SelectedImagePathProperty =
            DependencyProperty.Register(
                "SelectedImagePath",
                typeof(string),
                typeof(EvaluationControl),
                new FrameworkPropertyMetadata(string.Empty));

        /// <summary>
        /// 選択された画像のパスを取得します。
        /// </summary>
        public string SelectedImagePath
        {
            get { return (string)GetValue(SelectedImagePathProperty); }
            private set { SetValue(SelectedImagePathProperty, value); }
        }

        /// <summary>
        /// 評価値を表示するかどうかを扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty IsShowEvaluationPointProperty =
            DependencyProperty.Register(
                "IsShowEvaluationPoint",
                typeof(bool),
                typeof(EvaluationControl),
                new FrameworkPropertyMetadata(true,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        /// <summary>
        /// 評価値を表示するかどうかを取得または設定します。
        /// </summary>
        public bool IsShowEvaluationPoint
        {
            get { return (bool)GetValue(IsShowEvaluationPointProperty); }
            set { SetValue(IsShowEvaluationPointProperty, value); }
        }
        #endregion

        /// <summary>
        /// 設定が更新されたときに呼ばれるイベントです。
        /// </summary>
        public static readonly RoutedEvent SettingUpdatedEvent =
            EventManager.RegisterRoutedEvent(
                "SettingUpdated",
                RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                typeof(EvaluationControl));

        /// <summary>
        /// 設定が更新されたときに呼ばれるイベントです。
        /// </summary>
        public event RoutedEventHandler SettingUpdated
        {
            add { AddHandler(SettingUpdatedEvent, value); }
            remove { RemoveHandler(SettingUpdatedEvent, value); }
        }

        /// <summary>
        /// 設定ダイアログを開きます。
        /// </summary>
        public static readonly ICommand OpenSettingDialog =
            new RoutedUICommand(
                "設定ダイアログを新たに開きます。",
                "OpenSettingDialog",
                typeof(EvaluationControl));

        /// <summary>
        /// 設定ダイアログを開きます。
        /// </summary>
        private void ExecuteOpenSettingDialog(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                var dialog = new EvaluationSettingDialog(this);

                if (dialog.ShowDialog() == true)
                {
                    RaiseEvent(new RoutedEventArgs(SettingUpdatedEvent));
                }
            }
            catch (Exception ex)
            {
                Util.ThrowIfFatal(ex);
            }
        }

        private ReentrancyLock imageLock = new ReentrancyLock();

        /// <summary>
        /// 画像セットリストを更新します。
        /// </summary>
        private void UpdateImageSetList()
        {
            if (ImageSetList == null)
            {
                return;
            }

            // 画像セット名から画像セットを探します。
            UpdateImageSetTitle();
        }

        /// <summary>
        /// 画像セットを更新します。
        /// </summary>
        private void UpdateImageSet()
        {
            using (var result = imageLock.Lock())
            {
                if (result == null) return;

                SelectedImageSetTitle = (SelectedImageSet != null ?
                    SelectedImageSet.Title :
                    string.Empty);
            }

            // 画像セットが更新された可能性があるため。
            UpdateImage();
        }

        /// <summary>
        /// 画像セット名を更新します。
        /// </summary>
        private void UpdateImageSetTitle()
        {
            using (var result = imageLock.Lock())
            {
                if (result == null || ImageSetList == null)
                {
                    return;
                }

                // 同じ名前の画像セットを探し、なければ最初のものを使います。
                var imageSet = ImageSetList.FirstOrDefault(
                    _ => _.Title == SelectedImageSetTitle);
                imageSet = imageSet ?? ImageSetList.FirstOrDefault();

                if (imageSet != null)
                {
                    SelectedImageSet = imageSet;
                }
                else
                {
                    SelectedImageSetTitle = string.Empty;
                    SelectedImageSet = null;
                }
            }

            // 画像セットが更新された可能性があるため。
            UpdateImage();
        }

        /// <summary>
        /// 評価値表示用の画像を更新します。
        /// </summary>
        private void UpdateImage()
        {
            if (SelectedImageSet == null)
            {
                return;
            }

            // 評価値から画像を取得します。
            SelectedImagePath =
                SelectedImageSet.GetSelectedImagePath(EvaluationPoint);
        }

        /// <summary>
        /// 評価値を更新します。
        /// </summary>
        private void UpdatePoint()
        {
            switch (PointType)
            {
                case EvaluationPointType.ManualInput:
                    EvaluationPoint = ManualInputPoint;
                    break;
                case EvaluationPointType.User:
                    EvaluationPoint = UserPoint;
                    break;
                case EvaluationPointType.ModeCustom:
                    EvaluationPoint = ModeCustomPoint;
                    break;
            }

            UpdateImage();
        }

        /// <summary>
        /// 静的コンストラクタ
        /// </summary>
        static EvaluationControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(EvaluationControl),
                new FrameworkPropertyMetadata(typeof(EvaluationControl)));
        }

        /// <summary>
        /// コマンドのバインディングを行います。
        /// </summary>
        public void InitializeBindings(UIElement elem)
        {
            elem.CommandBindings.Add(
                new CommandBinding(
                    OpenSettingDialog,
                    ExecuteOpenSettingDialog));
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public EvaluationControl()
        {
            InitializeBindings(this);
        }
    }
}
