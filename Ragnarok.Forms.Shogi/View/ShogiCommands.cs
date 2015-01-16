using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Ragnarok.Forms;
using Ragnarok.Forms.Input;

namespace Ragnarok.Forms.Shogi.View
{
    /// <summary>
    /// 将棋用のコマンドを定義します。
    /// </summary>
    public static class ShogiCommands
    {
        #region CanExecute
        /// <summary>
        /// Undo操作が可能か調べます。
        /// </summary>
        private static void CanExecuteUndo(object sender, CanExecuteRelayEventArgs e)
        {
            var element = e.Parameter as GLShogiElement;
            if (element == null)
            {
                throw new ArgumentNullException("parameter");
            }

            if (element.Board == null)
            {
                throw new InvalidOperationException(
                    "element.Boardがnullです。");
            }

            e.CanExecute = element.Board.CanUndo;
        }

        /// <summary>
        /// Redo操作が可能か調べます。
        /// </summary>
        private static void CanExecuteRedo(object sender, CanExecuteRelayEventArgs e)
        {
            var element = e.Parameter as GLShogiElement;
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            if (element.Board == null)
            {
                throw new InvalidOperationException(
                    "element.Boardがnullです。");
            }

            e.CanExecute = element.Board.CanRedo;
        }
        #endregion

        #region Undo
        /// <summary>
        /// Undo操作を行います。
        /// </summary>
        public static readonly ICommand Undo =
            new RelayCommand<GLShogiElement>(
                ExecuteUndo, CanExecuteUndo);

        private static void ExecuteUndo(GLShogiElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            try
            {
                element.Board.Undo();
            }
            catch (Exception ex)
            {
                Util.ThrowIfFatal(ex);
                DialogUtil.ShowError(ex,
                    "指し手をⅰ手戻すことができませんでした (￣ω￣;)");
            }
        }
        #endregion

        #region UndoContinue
        /// <summary>
        /// 指し手を連続して戻します。
        /// </summary>
        public static void ExecuteUndoContinue(GLShogiElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            try
            {
                var autoPlay = new AutoPlay(
                    element.Board, true, AutoPlayType.Undo)
                {
                    IsUseEffect = false,
                };
                element.StartAutoPlay(autoPlay);
            }
            catch (Exception ex)
            {
                Util.ThrowIfFatal(ex);
                DialogUtil.ShowError(
                    "指し手を連続して戻すことに失敗しました (￣ω￣;)");
            }
        }
        #endregion

        #region GotoFirstState
        /// <summary>
        /// 局面を初期局面に設定します。
        /// </summary>
        public static readonly ICommand GotoFirstState =
            new RelayCommand<GLShogiElement>(
                ExecuteGotoFirstState, CanExecuteUndo);

        private static void ExecuteGotoFirstState(GLShogiElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            try
            {
                element.Board.UndoAll();
            }
            catch (Exception ex)
            {
                Util.ThrowIfFatal(ex);
                DialogUtil.ShowError(
                    "局面を初期局面にすることができませんでした (￣ω￣;)");
            }
        }
        #endregion

        #region Redo
        /// <summary>
        /// Redo操作を実行します。
        /// </summary>
        public static readonly ICommand Redo =
            new RelayCommand<GLShogiElement>(
                ExecuteRedo, CanExecuteRedo);

        private static void ExecuteRedo(GLShogiElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            try
            {
                element.Board.Redo();
            }
            catch(Exception ex)
            {
                Util.ThrowIfFatal(ex);
                DialogUtil.ShowError(
                    "局面を１手進めることができませんでした (￣ω￣;)");
            }
        }
        #endregion

        #region RedoContinue
        /// <summary>
        /// 局面を連続して進めます。
        /// </summary>
        public static readonly ICommand RedoContinue =
            new RelayCommand<GLShogiElement>(
                ExecuteRedoContinue, CanExecuteRedo);

        private static void ExecuteRedoContinue(GLShogiElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            try
            {
                var autoPlay = new AutoPlay(
                    element.Board, true, AutoPlayType.Redo)
                {
                    IsUseEffect = false,
                };

                element.StartAutoPlay(autoPlay);
            }
            catch (Exception ex)
            {
                Util.ThrowIfFatal(ex);
                DialogUtil.ShowError(
                    "局面を連続して進めることに失敗しました (￣ω￣;)");
            }
        }
        #endregion

        #region GotoLastState
        /// <summary>
        /// 局面を最終局面に設定します。
        /// </summary>
        public static readonly ICommand GotoLastState =
            new RelayCommand<GLShogiElement>(
                ExecuteGotoLastState, CanExecuteRedo);

        private static void ExecuteGotoLastState(GLShogiElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            try
            {
                element.Board.RedoAll();
            }
            catch(Exception ex)
            {
                Util.ThrowIfFatal(ex);
                DialogUtil.ShowError(
                    "局面を最終局面にできませんでした (￣ω￣;)");
            }
        }
        #endregion
    }
}
