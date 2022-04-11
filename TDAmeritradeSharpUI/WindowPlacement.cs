using System.Runtime.InteropServices;
using System.Text.Json;

// ReSharper disable CommentTypo
// ReSharper disable InconsistentNaming
// ReSharper disable IdentifierTypo
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable ArrangeModifiersOrder

namespace TDAmeritradeSharpUI
{
    // RECT structure required by WINDOWPLACEMENT structure
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;

        public RECT(int left, int top, int right, int bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }
    }

    // POINT structure required by WINDOWPLACEMENT structure
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int X;
        public int Y;

        public POINT(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    // WINDOWPLACEMENT stores the position, size, and state of a window
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct WINDOWPLACEMENT
    {
        public int length;
        public int flags;
        public int showCmd;
        public POINT minPosition;
        public POINT maxPosition;
        public RECT normalPosition;
    }

    /// <summary>
    ///     Class for using interop GetWindowPlacement and SetWindowPlacement
    ///     see http://www.pinvoke.net/default.aspx/user32.getwindowplacement
    ///     and modified using https://blogs.msdn.microsoft.com/davidrickard/2010/03/08/saving-window-size-and-location-in-wpf-and-winforms/
    /// </summary>
    public static class WindowPlacement
    {
        private static readonly JsonSerializerOptions _jsonOptions;

        static WindowPlacement()
        {
            _jsonOptions = new JsonSerializerOptions {
                IncludeFields = true
            };
        }

        [DllImport("user32.dll")]
        public static extern bool SetWindowPlacement(IntPtr hWnd, [In] ref WINDOWPLACEMENT lpwndpl);

        [DllImport("user32.dll")]
        public static extern bool GetWindowPlacement(IntPtr hWnd, out WINDOWPLACEMENT lpwndpl);

        public static void RestoreWindow(IntPtr windowHandle, string placementJson)
        {
            if (string.IsNullOrEmpty(placementJson))
            {
                return;
            }
            var placement = JsonSerializer.Deserialize<WINDOWPLACEMENT>(placementJson, _jsonOptions);
            var success = SetWindowPlacement(windowHandle, ref placement);
            if (!success)
            {
                throw new ApplicationException("Unexpected failure to set window placement");
            }
        }

        public static string SaveWindow(IntPtr windowHandle)
        {
            var success = GetWindowPlacement(windowHandle, out var placement);
            if (!success)
            {
                throw new ApplicationException("Unexpected failure to get window placement");
            }
            var str = JsonSerializer.Serialize(placement, _jsonOptions);
            return str;
        }
    }
}