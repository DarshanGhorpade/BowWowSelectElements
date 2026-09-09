using System;

namespace Selection.Revit.Utils
{
    // NATIVE WINDOWS API
    public static class NativeMethods
    {
        [System.Runtime.InteropServices.DllImport(
            "user32.dll")]
        public static extern bool GetWindowRect(
            IntPtr hWnd,
            out RECT lpRect);


        public struct RECT
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;
        }
    }
}
