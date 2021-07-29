using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;

namespace Ragnarok.Presentation.Utility
{
    /// <summary>
    /// ShortCutKey�ł��B
    /// </summary>
    public sealed class ShortCutKey : IEquatable<ShortCutKey>
    {
        /// <summary>
        /// �L�[���擾�܂��͐ݒ肵�܂��B
        /// </summary>
        public Key Key
        {
            get;
            set;
        }

        /// <summary>
        /// �V�t�g�L�[��������Ă��邩�擾�܂��͐ݒ肵�܂��B
        /// </summary>
        public bool IsShift
        {
            get;
            set;
        }

        /// <summary>
        /// �A���g�L�[��������Ă��邩�擾�܂��͐ݒ肵�܂��B
        /// </summary>
        public bool IsAlt
        {
            get;
            set;
        }

        /// <summary>
        /// �R���g���[���L�[��������Ă��邩�擾�܂��͐ݒ肵�܂��B
        /// </summary>
        public bool IsCtrl
        {
            get;
            set;
        }

        /// <summary>
        /// �I�u�W�F�N�g���r���܂��B
        /// </summary>
        public override bool Equals(object obj)
        {
            var result = this.PreEquals(obj);
            if (result.HasValue)
            {
                return result.Value;
            }

            return Equals((ShortCutKey)obj);
        }

        /// <summary>
        /// �I�u�W�F�N�g���r���܂��B
        /// </summary>
        public bool Equals(ShortCutKey obj)
        {
            if ((object)obj == null)
            {
                return false;
            }

            if (Key != obj.Key)
            {
                return false;
            }

            if (IsShift != obj.IsShift || IsAlt != obj.IsAlt ||
                IsCtrl != obj.IsCtrl)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// �n�b�V���l���v�Z���܂��B
        /// </summary>
        public override int GetHashCode()
        {
            return (
                Key.GetHashCode() ^
                IsShift.GetHashCode() ^
                IsAlt.GetHashCode() ^
                IsCtrl.GetHashCode());
        }

        /// <summary>
        /// �����񉻂��܂��B
        /// </summary>
        public override string ToString()
        {
            return string.Format(
                "[{0}{1}{2}{3}]",
                Key.ToString(),
                (IsShift ? "+Shift" : ""),
                (IsAlt ? "+Alt" : ""),
                (IsCtrl ? "+Ctrl" : ""));
        }
    }

    /// <summary>
    /// KeyEvent�̈����ł��B
    /// </summary>
    public class KeyHookEventArgs : EventArgs
    {
        /// <summary>
        /// �L�[���擾�܂��͐ݒ肵�܂��B
        /// </summary>
        public ShortCutKey ShortCutKey
        {
            get;
            private set;
        }

        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
        public KeyHookEventArgs(ShortCutKey shortKey)
        {
            ShortCutKey = shortKey;
        }
    }

    /// <summary>
    /// �O���[�o���L�[�t�b�N���s�����߂̃N���X�ł��B
    /// dll ���Ŏg��Ȃ��ƃv���Z�X���z�����L�[�t�b�N���s���܂���B
    /// 
    /// �Q��: http://www.shise.net/wiki/wiki.cgi?page=C%23%2FClass%2FLowLevelKeyHook
    /// </summary>
    public class KeyHook : IDisposable
    {
        #region PInvoke
        private const int WM_CHAR = 0x102;
        private const int WM_HOTKEY = 0x312;
        private const int WM_KEYDOWN = 0x100;
        private const int WM_KEYUP = 0x101;
        private const int WM_IME_KEYDOWN = 0x290;
        private const int WM_IME_KEYUP = 0x291;
        private const int WM_SYSKEYDOWN = 0x104;
        private const int WM_SYSKEYUP = 0x105;

        private enum HookType
        {
            WH_JOURNALRECORD = 0,
            WH_JOURNALPLAYBACK = 1,
            WH_KEYBOARD = 2,
            WH_GETMESSAGE = 3,
            WH_CALLWNDPROC = 4,
            WH_CBT = 5,
            WH_SYSMSGFILTER = 6,
            WH_MOUSE = 7,
            WH_HARDWARE = 8,
            WH_DEBUG = 9,
            WH_SHELL = 10,
            WH_FOREGROUNDIDLE = 11,
            WH_CALLWNDPROCRET = 12,
            WH_KEYBOARD_LL = 13,
            WH_MOUSE_LL = 14
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct KBDLLHOOKSTRUCT
        {
            public int vkCode;
            public int scanCode;
            public int flags;
            public int time;
            public IntPtr dwExtraInfo;
        }

        private delegate int HookProc(int code, IntPtr wParam, IntPtr lParam);
        private delegate int LowLevelKeyboardProc(int code, IntPtr wParam, ref KBDLLHOOKSTRUCT lParam);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(HookType hook, HookProc callback, IntPtr hMod, int dwThreadId);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(HookType hook, LowLevelKeyboardProc callback, IntPtr hMod, int dwThreadId);
        [DllImport("user32.dll")]
        private static extern int CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, ref KBDLLHOOKSTRUCT lParam);
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool UnhookWindowsHookEx(IntPtr idHook);
        #endregion

        /// <summary>
        /// �f���Q�[�g�I�u�W�F�N�g�������o�Ŏ����Ȃ��ƃK�x�R������܂�(;;)
        /// </summary>
        private readonly LowLevelKeyboardProc keyboardProc;
        private IntPtr hookId = IntPtr.Zero;
        private int shift;
        private int ctrl;
        private int alt;
        private bool disposed = false;

        /// <summary>
        /// �L�[�������ꂽ�Ƃ��ɌĂ΂��C�x���g�ł��B
        /// </summary>
        public event EventHandler<KeyHookEventArgs> KeyDown;

        /// <summary>
        /// �L�[�������ꂽ�Ƃ��ɌĂ΂��C�x���g�ł��B
        /// </summary>
        public event EventHandler<KeyHookEventArgs> KeyUp;

        /// <summary>
        /// �L�[�t�b�N���J�n���܂��B
        /// </summary>
        public void BeginHook()
        {
            // �R�[���o�b�N�p�̃f���Q�[�g���쐬�B
            if (this.hookId != IntPtr.Zero)
            {
                return;
            }

            // ���s�A�Z���u���̃��W���[���n���h�����擾�B
            var modules = Assembly.GetEntryAssembly().GetModules();
            var hMod = Marshal.GetHINSTANCE(modules[0]);

            // �჌�x���L�[�t�b�N�ɓo�^�B
            // �R�[���o�b�N�̓f���Q�[�ƃI�u�W�F�N�g���w�肵�Ȃ��ƁA
            // �K�x�R������܂��B
            var hookId = SetWindowsHookEx(
                HookType.WH_KEYBOARD_LL,
                this.keyboardProc, hMod, 0);
            if (hookId == IntPtr.Zero)
            {
                throw new ApplicationException(
                    Marshal.GetLastWin32Error().ToString() +
                    ": �L�[�t�b�N�̓o�^�Ɏ��s���܂����B");
            }

            this.hookId = hookId;
        }

        /// <summary>
        /// �L�[�t�b�N���I�����܂��B
        /// </summary>
        public void EndHook()
        {
            if (this.hookId == IntPtr.Zero)
            {
                return;
            }

            if (!UnhookWindowsHookEx(this.hookId))
            {
                /* ���s�͂Ƃ肠���������B*/
            }

            this.hookId = IntPtr.Zero;
        }

        /// <summary>
        /// �L�[�t�b�N���ɌĂ΂��R�[���o�b�N���\�b�h�ł��B
        /// </summary>
        private int KeyboardCallback(int code, IntPtr wParam, ref KBDLLHOOKSTRUCT lParam)
        {
            if (code < 0)
            {
                return CallNextHookEx(IntPtr.Zero, code, wParam, ref lParam);
            }

            var key = KeyInterop.KeyFromVirtualKey(lParam.vkCode);
            switch ((int)wParam)
            {
                case WM_IME_KEYDOWN:
                case WM_KEYDOWN:
                case WM_SYSKEYDOWN:
                    UpdateModifiers(key, true);
                    OnKeyEvent(KeyDown, key);
                    break;
                case WM_IME_KEYUP:
                case WM_KEYUP:
                case WM_SYSKEYUP:
                    UpdateModifiers(key, false);
                    OnKeyEvent(KeyUp, key);
                    break;
            }

            // �L�[�̏��������ɉ񂵂Ċ����B
            return CallNextHookEx(IntPtr.Zero, code, wParam, ref lParam);
        }

        /// <summary>
        /// �L�[�C�x���g���Ăяo���܂��B
        /// </summary>
        private void OnKeyEvent(EventHandler<KeyHookEventArgs> handler, Key key)
        {
            var shortKey = new ShortCutKey
            {
                Key = key,
                IsShift = (this.shift != 0),
                IsAlt = (this.alt != 0),
                IsCtrl = (this.ctrl != 0),
            };

            handler.SafeRaiseEvent(this, new KeyHookEventArgs(shortKey));
        }

        /// <summary>
        /// �L�[�C���q���C�����܂��B
        /// </summary>
        private static void ChangeModifier(ref int modifier, int index, bool isDown)
        {
            if (isDown)
            {
                modifier |= (1 << index);
            }
            else
            {
                modifier &= ~(1 << index);
            }
        }

        /// <summary>
        /// �C���L�[�̏�Ԃ��X�V���܂��B
        /// </summary>
        private void UpdateModifiers(Key key, bool isDown)
        {
            switch (key)
            {
                case Key.LeftShift:
                    ChangeModifier(ref this.shift, 0, isDown);
                    break;
                case Key.RightShift:
                    ChangeModifier(ref this.shift, 1, isDown);
                    break;
                case Key.LeftAlt:
                    ChangeModifier(ref this.alt, 0, isDown);
                    break;
                case Key.RightAlt:
                    ChangeModifier(ref this.alt, 1, isDown);
                    break;
                case Key.LeftCtrl:
                    ChangeModifier(ref this.ctrl, 0, isDown);
                    break;
                case Key.RightCtrl:
                    ChangeModifier(ref this.ctrl, 1, isDown);
                    break;
            }
        }

        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
        public KeyHook()
        {
            this.keyboardProc = KeyboardCallback;
        }

        /// <summary>
        /// �f�X�g���N�^
        /// </summary>
        ~KeyHook()
        {
            Dispose(false);
        }

        /// <summary>
        /// �I�u�W�F�N�g��j�����܂��B
        /// </summary>
        public void Dispose()
        {
            GC.SuppressFinalize(this);
            Dispose(true);
        }

        /// <summary>
        /// �I�u�W�F�N�g��j�����܂��B
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                EndHook();
                this.disposed = true;
            }
        }
    }
}
