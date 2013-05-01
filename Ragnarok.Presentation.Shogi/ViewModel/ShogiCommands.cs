using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;

using Ragnarok.Shogi;

namespace Ragnarok.Presentation.Shogi.ViewModel
{
    using View;

    /// <summary>
    /// コマンドを保持します。
    /// </summary>
    public static class ShogiCommands
    {
        /// <summary>
        /// 棋譜ファイルを読み込みます。
        /// </summary>
        public static readonly ICommand LoadKifFile =
            new RoutedUICommand(
                "棋譜ファイルを読み込みます。",
                "LoadKifFile",
                typeof(ShogiControl));
        /// <summary>
        /// 棋譜ファイルを保存します。
        /// </summary>
        public static readonly ICommand SaveKifFile =
            new RoutedUICommand(
                "棋譜ファイルを保存します。",
                "SaveKifFile",
                typeof(ShogiControl));
        /// <summary>
        /// 棋譜ファイルの貼り付けを行います。
        /// </summary>
        public static readonly ICommand PasteKifFile =
            new RoutedUICommand(
                "棋譜ファイルの貼り付けを行います。",
                "PasteKifFile",
                typeof(ShogiControl));
        /// <summary>
        /// 棋譜ファイルのコピーを行います。
        /// </summary>
        public static readonly ICommand CopyKifFile =
            new RoutedUICommand(
                "棋譜ファイルのコピーを行います。",
                "CopyKifFile",
                typeof(ShogiControl));
        /// <summary>
        /// 盤面を反転します。
        /// </summary>
        public static readonly ICommand SetReverseBoard =
            new RoutedUICommand(
                "盤面を反転します。",
                "SetReverseBoard",
                typeof(ShogiControl));

        /// <summary>
        /// 開始局面へ。
        /// </summary>
        public static readonly ICommand GotoFirstState =
            new RoutedUICommand(
                "開始局面へ。",
                "GotoFirstState",
                typeof(ShogiControl));
        /// <summary>
        /// 最終局面へ。
        /// </summary>
        public static readonly ICommand GotoLastState =
            new RoutedUICommand(
                "最終局面へ。",
                "GotoLastState",
                typeof(ShogiControl));
        /// <summary>
        /// 手を一つ戻します。
        /// </summary>
        public static readonly ICommand MoveUndo =
            new RoutedUICommand(
                "手を一つ戻します。",
                "MoveUndo",
                typeof(ShogiControl));
        /// <summary>
        /// 手を一つ進めます。
        /// </summary>
        public static readonly ICommand MoveRedo =
            new RoutedUICommand(
                "手を一つ進めます。",
                "MoveRedo",
                typeof(ShogiControl));
        /// <summary>
        /// 連続して手を戻します。
        /// </summary>
        public static readonly ICommand MoveUndoContinue =
            new RoutedUICommand(
                "連続して手を戻します。",
                "MoveUndoContinue",
                typeof(ShogiControl));
        /// <summary>
        /// 連続して手を進めます。
        /// </summary>
        public static readonly ICommand MoveRedoContinue =
            new RoutedUICommand(
                "連続して手を進めます。",
                "MoveRedoContinue",
                typeof(ShogiControl));
        /// <summary>
        /// 再生中の手を停止します。
        /// </summary>
        public static readonly ICommand MoveStop =
            new RoutedUICommand(
                "再生中の手を停止します。",
                "MoveStop",
                typeof(ShogiControl));

        /// <summary>
        /// コマンドをバインディングします。
        /// </summary>
        public static void Binding(ShogiControl control,
                                   CommandBindingCollection bindings)
        {
            bindings.Add(
                new CommandBinding(
                    LoadKifFile,
                    (_, e) => ExecuteLoadKifFile(control),
                    (_, e) => CanExecute(control, e)));
            bindings.Add(
                new CommandBinding(
                    SaveKifFile,
                    (_, e) => ExecuteSaveKifFile(control),
                    (_, e) => CanExecute(control, e)));
            bindings.Add(
                new CommandBinding(
                    PasteKifFile,
                    (_, e) => ExecutePasteKifFile(control),
                    (_, e) => CanExecute(control, e)));
            bindings.Add(
                new CommandBinding(
                    CopyKifFile,
                    (_, e) => ExecuteCopyKifFile(control),
                    (_, e) => CanExecute(control, e)));
            bindings.Add(
                new CommandBinding(
                    SetReverseBoard,
                    (_, e) => ExecuteSetReverseBoard(control, e),
                    (_, e) => CanExecute(control, e)));

            bindings.Add(
                new CommandBinding(
                    GotoFirstState,
                    (_, e) => ExecuteGotoFirstState(control),
                    (_, e) => CanExecute(control, e)));
            bindings.Add(
                new CommandBinding(
                    GotoLastState,
                    (_, e) => ExecuteGotoLastState(control),
                    (_, e) => CanExecute(control, e)));
            bindings.Add(
                new CommandBinding(
                    MoveUndo,
                    (_, e) => ExecuteMoveUndo(control),
                    (_, e) => CanExecute(control, e)));
            bindings.Add(
                new CommandBinding(
                    MoveRedo,
                    (_, e) => ExecuteMoveRedo(control),
                    (_, e) => CanExecute(control, e)));
            bindings.Add(
                new CommandBinding(
                    MoveUndoContinue,
                    (_, e) => ExecuteMoveUndoContinue(control),
                    (_, e) => CanExecute(control, e)));
            bindings.Add(
                new CommandBinding(
                    MoveRedoContinue,
                    (_, e) => ExecuteMoveRedoContinue(control),
                    (_, e) => CanExecute(control, e)));
            bindings.Add(
                new CommandBinding(
                    MoveStop,
                    (_, e) => ExecuteMoveStop(control),
                    (_, e) => CanExecute(control, e)));
        }

        /// <summary>
        /// コマンドを操作にバインディングします。
        /// </summary>
        public static void Binding(InputBindingCollection inputs)
        {
            inputs.Add(
                new KeyBinding(MoveUndo,
                    new KeyGesture(Key.Left)));
            inputs.Add(
                new KeyBinding(MoveRedo,
                    new KeyGesture(Key.Right)));

            inputs.Add(
                new KeyBinding(LoadKifFile,
                    new KeyGesture(Key.O, ModifierKeys.Control)));
            inputs.Add(
                new KeyBinding(SaveKifFile,
                    new KeyGesture(Key.A, ModifierKeys.Control)));

            inputs.Add(
                new KeyBinding(PasteKifFile,
                    new KeyGesture(Key.V, ModifierKeys.Control)));
            inputs.Add(
                new KeyBinding(CopyKifFile,
                    new KeyGesture(Key.C, ModifierKeys.Control)));
        }

        private static Board GetBoard(ShogiControl control)
        {
            var board = control.Board;

            if (board == null || !board.Validate())
            {
                //DialogUtil.ShowError(
                //    "局面が正しくありません (ー_ー)");
                return null;
            }

            return board;
        }

        /// <summary>
        /// コマンドが実行できるか調べます。
        /// </summary>
        private static void CanExecute(ShogiControl control, CanExecuteRoutedEventArgs e)
        {
            var canEdit = (control.EditMode == EditMode.Normal);

            var board = GetBoard(control);
            if (board == null)
            {
                return;
            }

            if (e.Command == GotoFirstState)
            {
                e.CanExecute = board.CanUndo && canEdit;
            }
            else if (e.Command == GotoLastState)
            {
                e.CanExecute = board.CanRedo && canEdit;
            }

            else if (e.Command == MoveUndo)
            {
                e.CanExecute = board.CanUndo && canEdit;
            }
            else if (e.Command == MoveRedo)
            {
                e.CanExecute = board.CanRedo && canEdit;
            }
            else if (e.Command == MoveUndoContinue)
            {
                e.CanExecute = board.CanUndo && canEdit;
            }
            else if (e.Command == MoveRedoContinue)
            {
                e.CanExecute = board.CanRedo && canEdit;
            }

            else if (e.Command == MoveStop)
            {
                e.CanExecute = (control.AutoPlayState == AutoPlayState.Playing);
            }
            else
            {
                e.CanExecute = true;
            }

            e.Handled = true;
        }

        /// <summary>
        /// 棋譜ファイルを読み込みます。
        /// </summary>
        private static void ExecuteLoadKifFile(ShogiControl control)
        {
            try
            {
                var dialog = new OpenFileDialog()
                {
                    DefaultExt = ".kif",
                    Title = "棋譜ファイルの選択",
                    Multiselect = false,
                    RestoreDirectory = false,
                    Filter = "Kif Files(*.kif,*.ki2)|*.kif;*.ki2|All files (*.*)|*.*",
                    FilterIndex = 0,
                };
                var result = dialog.ShowDialog();
                if (result == null || !result.Value)
                {
                    return;
                }

                using (var reader = new StreamReader(dialog.FileName,
                                                     KifuObject.DefaultEncoding))
                {
                    LoadKif(control, reader);
                }
            }
            catch (Exception ex)
            {
                DialogUtil.ShowError(ex,
                    "棋譜ファイルの読み込みに失敗しました。(￣ω￣;)");
            }
        }

        /// <summary>
        /// 棋譜ファイルの貼り付けを行います。
        /// </summary>
        private static void ExecutePasteKifFile(ShogiControl control)
        {
            var text = Clipboard.GetText(TextDataFormat.Text);
            using (var reader = new StringReader(text))
            {
                LoadKif(control, reader);
            }
        }

        /// <summary>
        /// 棋譜ファイルの読み込みを行います。
        /// </summary>
        public static void LoadKif(ShogiControl control, TextReader reader)
        {
            try
            {
                if (reader == null)
                {
                    return;
                }

                // ファイルを読み込み局面を作成します。
                var file = KifuReader.Load(reader);
                var board = file.CreateBoard();

                control.Board = board;
            }
            catch (Exception ex)
            {
                DialogUtil.ShowError(ex,
                    "棋譜ファイルの読み込みに失敗しました(￣ω￣;)");
            }
        }

        /// <summary>
        /// 棋譜ファイルを保存します。
        /// </summary>
        private static void ExecuteSaveKifFile(ShogiControl control)
        {
            try
            {
                var dialog = new SaveFileDialog()
                {
                    AddExtension = true,
                    CheckFileExists = false,
                    OverwritePrompt = true,
                    CreatePrompt = false,
                    Title = "棋譜ファイルの選択",
                    RestoreDirectory = false,
                    Filter = "Kif Files(*.kif)|*.kif|All files (*.*)|*.*",
                    FilterIndex = 0,
                };
                var result = dialog.ShowDialog();
                if (result == null || !result.Value)
                {
                    return;
                }

                using (var writer = new StreamWriter(dialog.FileName, false,
                                                     KifuObject.DefaultEncoding))
                {
                    SaveKif(control, writer);
                }
            }
            catch (Exception ex)
            {
                DialogUtil.ShowError(ex,
                    "棋譜ファイルの保存に失敗しました(￣ω￣;)");
            }
        }

        /// <summary>
        /// 棋譜ファイルのコピーを行います。
        /// </summary>
        private static void ExecuteCopyKifFile(ShogiControl control)
        {
            using (var writer = new StringWriter())
            {
                SaveKif(control, writer);

                Clipboard.SetText(writer.ToString());
            }
        }

        /// <summary>
        /// 棋譜ファイルの書き込みを行います。
        /// </summary>
        public static void SaveKif(ShogiControl control, TextWriter writer)
        {
            try
            {
                if (control == null || writer == null)
                {
                    return;
                }

                //var manager = model.MoveManager;
                //var root = manager.CreateVariationNode(control.Board);

                var headers = new Dictionary<string, string>();
                headers["先手"] = "あなた";
                headers["後手"] = "あなた２";

                //var kifu = new KifuObject(headers, null);
                //KifuWriter.Save(writer, kifu);
            }
            catch (Exception ex)
            {
                DialogUtil.ShowError(ex,
                    "棋譜ファイルの出力に失敗しました(￣ω￣;)");
            }
        }

        /// <summary>
        /// 盤面を反転します。
        /// </summary>
        private static void ExecuteSetReverseBoard(ShogiControl control, ExecutedRoutedEventArgs e)
        {
            try
            {
                var isWhite = (bool)e.Parameter;
                var side = (isWhite ? BWType.White : BWType.Black);

                control.ViewSide = side;
            }
            catch (Exception ex)
            {
                DialogUtil.ShowError(ex,
                    "盤面の反転に失敗しました(￣ω￣;)");
            }
        }

        /// <summary>
        /// 開始局面へ。
        /// </summary>
        private static void ExecuteGotoFirstState(ShogiControl control)
        {
            if (control.EditMode != EditMode.Normal)
            {
                return;
            }

            // 局面をUndoします。
            var cloned = control.Board.Clone();
            cloned.UndoAll();
            control.Board = cloned;
        }

        /// <summary>
        /// 最終局面へ。
        /// </summary>
        private static void ExecuteGotoLastState(ShogiControl control)
        {
            if (control.EditMode != EditMode.Normal)
            {
                return;
            }

            // 局面をRedoします。
            var cloned = control.Board.Clone();
            cloned.RedoAll();
            control.Board = cloned;
        }

        /// <summary>
        /// １手戻します。
        /// </summary>
        private static void ExecuteMoveUndo(ShogiControl control)
        {
            if (control.EditMode != EditMode.Normal)
            {
                return;
            }

            var board = GetBoard(control);
            if (board == null)
            {
                return;
            }

            board.Undo();
        }

        /// <summary>
        /// １手進めます。
        /// </summary>
        private static void ExecuteMoveRedo(ShogiControl control)
        {
            if (control.EditMode != EditMode.Normal)
            {
                return;
            }

            var board = GetBoard(control);
            if (board == null)
            {
                return;
            }

            board.Redo();
        }

        /// <summary>
        /// 連続して手を戻します。
        /// </summary>
        private static void ExecuteMoveUndoContinue(ShogiControl control)
        {
            if (control.EditMode != EditMode.Normal)
            {
                return;
            }

            var board = GetBoard(control);
            if (board == null)
            {
                return;
            }

            var autoPlay = new AutoPlay(board, AutoPlayType.Undo)
            {
                //Interval = ,
            };

            control.StartAutoPlay(autoPlay);
        }

        /// <summary>
        /// 連続して手を進めます。
        /// </summary>
        private static void ExecuteMoveRedoContinue(ShogiControl control)
        {
            if (control.EditMode != EditMode.Normal)
            {
                return;
            }

            var board = GetBoard(control);
            if (board == null)
            {
                return;
            }

            var autoPlay = new AutoPlay(board, AutoPlayType.Redo)
            {
                //Interval = ,
            };

            control.StartAutoPlay(autoPlay);
        }

        /// <summary>
        /// 再生中の手を停止します。
        /// </summary>
        private static void ExecuteMoveStop(ShogiControl control)
        {
            control.StopAutoPlay();
        }
    }
}
