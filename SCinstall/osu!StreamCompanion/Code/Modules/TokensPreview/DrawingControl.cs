using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using osu_StreamCompanion.Code.Misc;

namespace osu_StreamCompanion.Code.Modules.TokensPreview
{
    class DrawingControl
    {
        public static void SuspendDrawing(Control parent)
        {
            NativeMethods.SendMessage(parent.Handle, NativeMethods.WM_SETREDRAW, false, 0);
        }

        public static void ResumeDrawing(Control parent)
        {
            NativeMethods.SendMessage(parent.Handle, NativeMethods.WM_SETREDRAW, true, 0);
            parent.Refresh();
        }

    }
}