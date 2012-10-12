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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Ragnarok.Shogi.View
{
    using ViewModel;

    /// <summary>
    /// PromoteDialog.xaml の相互作用ロジック
    /// </summary>
    public partial class PromoteDialog : Window
    {
        /// <summary>
        /// 表示する駒の種類を扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty PieceTypeProperty =
            DependencyProperty.Register(
                "PieceType", typeof(PieceType), typeof(PromoteDialog),
                new FrameworkPropertyMetadata(PieceType.None, OnPieceTypeChanged));

        /// <summary>
        /// 表示する駒の種類を取得または設定します。
        /// </summary>
        public PieceType PieceType
        {
            get { return (PieceType)GetValue(PieceTypeProperty); }
            set { SetValue(PieceTypeProperty, value); }
        }

        /// <summary>
        /// 駒の表示サイズを扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty CellSizeProperty =
            DependencyProperty.Register(
                "CellSize", typeof(Size), typeof(PromoteDialog),
                new FrameworkPropertyMetadata(default(Size)));

        /// <summary>
        /// 駒の表示サイズを取得または設定します。
        /// </summary>
        public Size CellSize
        {
            get { return (Size)GetValue(CellSizeProperty); }
            set { SetValue(CellSizeProperty, value); }
        }

        /// <summary>
        /// 成り駒を示す画像を扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty PromotedImageProperty =
            DependencyProperty.Register(
                "PromotedImage", typeof(ImageBrush), typeof(PromoteDialog),
                new FrameworkPropertyMetadata(null));

        /// <summary>
        /// 成り駒を示す画像を取得または設定します。
        /// </summary>
        public ImageBrush PromotedImage
        {
            get { return (ImageBrush)GetValue(PromotedImageProperty); }
            set { SetValue(PromotedImageProperty, value); }
        }

        /// <summary>
        /// 不成り駒を示す画像を扱う依存プロパティです。
        /// </summary>
        public static readonly DependencyProperty UnpromotedImageProperty =
            DependencyProperty.Register(
                "UnpromotedImage", typeof(ImageBrush), typeof(PromoteDialog),
                new FrameworkPropertyMetadata(null));

        /// <summary>
        /// 不成り駒を示す画像を取得または設定します。
        /// </summary>
        public ImageBrush UnpromotedImage
        {
            get { return (ImageBrush)GetValue(UnpromotedImageProperty); }
            set { SetValue(UnpromotedImageProperty, value); }
        }

        /// <summary>
        /// 駒の種類が変わったらその画像を一緒に変えます。
        /// </summary>
        static void OnPieceTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var self = d as PromoteDialog;

            if (self != null && self.LayoutRoot != null)
            {
                var pieceType = (PieceType)e.NewValue;
 
                self.PromotedImage = PieceObject.CreateBrush(
                    self.shogi,
                    new BoardPiece(BWType.Black, pieceType, true));
                self.UnpromotedImage = PieceObject.CreateBrush(
                    self.shogi,
                    new BoardPiece(BWType.Black, pieceType, false));

                self.Height = self.CellSize.Height;
                self.Width = self.CellSize.Width * 2;
            }
        }

        private readonly ShogiControl shogi;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public PromoteDialog(ShogiControl shogi, PieceType pieceType)
        {
            InitializeComponent();

            this.shogi = shogi;
            CellSize = shogi.CellSize;
            PieceType = pieceType;
        }

        private void ImageP_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DialogResult = true;
        }

        private void ImageU_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DialogResult = false;
        }
    }
}
