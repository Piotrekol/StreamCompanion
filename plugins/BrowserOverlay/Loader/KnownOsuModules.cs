﻿using System.Collections.Generic;

namespace BrowserOverlay.Loader
{
    internal static class KnownOsuModules
    {
        private static List<string> _modules = new List<string>
        {
            "acgenral.dll",
            "advapi32.dll",
            "amsi.dll",
            "apphelp.dll",
            "appxdeploymentclient.dll",
            "audioses.dll",
            "avrt.dll",
            "bass.dll",
            "bcrypt.dll",
            "bcryptprimitives.dll",
            "cfgmgr32.dll",
            "chrome_elf.dll",
            "clbcatq.dll",
            "clr.dll",
            "clrjit.dll",
            "coloradapterclient.dll",
            "combase.dll",
            "comctl32.dll",
            "comdlg32.dll",
            "coremessaging.dll",
            "coreuicomponents.dll",
            "credui.dll",
            "crypt32.dll",
            "cryptbase.dll",
            "cryptnet.dll",
            "cryptsp.dll",
            "cryptui.dll",
            "d3d11.dll",
            "d3d9.dll",
            "d3dcompiler_47.dll",
            "dataexchange.dll",
            "dbgcore.dll",
            "dbghelp.dll",
            "dciman32.dll",
            "dcomp.dll",
            "ddraw.dll",
            "devobj.dll",
            "dhcpcsvc.dll",
            "dhcpcsvc6.dll",
            "diasymreader.dll",
            "dnsapi.dll",
            "dpapi.dll",
            "dwmapi.dll",
            "dwrite.dll",
            "dxcore.dll",
            "dxgi.dll",
            "dxva2.dll",
            "esent.dll",
            "fastprox.dll",
            "fwpuclnt.dll",
            "gdi32.dll",
            "gdi32full.dll",
            "gdiplus.dll",
            "glu32.dll",
            "gpapi.dll",
            "hid.dll",
            "icm32.dll",
            "iertutil.dll",
            "imagehlp.dll",
            "imm32.dll",
            "inputhost.dll",
            "iphlpapi.dll",
            "kernel.appcore.dll",
            "kernel32.dll",
            "kernelbase.dll",
            "libcef.dll",
            "libegl.dll",
            "libglesv2.dll",
            "mmdevapi.dll",
            "mpclient.dll",
            "mpoav.dll",
            "mpr.dll",
            "msacm32.dll",
            "msasn1.dll",
            "mscms.dll",
            "mscoree.dll",
            "mscoreei.dll",
            "mscorlib.ni.dll",
            "msctf.dll",
            "mskeyprotect.dll",
            "msvcp_win.dll",
            "msvcrt.dll",
            "mswsock.dll",
            "ncrypt.dll",
            "ncryptsslp.dll",
            "netapi32.dll",
            "netutils.dll",
            "nlaapi.dll",
            "nsi.dll",
            "ntasn1.dll",
            "ntdll.dll",
            "ntmarta.dll",
            "nvapi.dll",
            "nvoglv32.dll",
            "ole32.dll",
            "oleacc.dll",
            "oleaut32.dll",
            "opengl32.dll",
            "osu!.exe",
            "osu!auth.dll",
            "osubrowseroverlay.dll",
            "powrprof.dll",
            "profapi.dll",
            "propsys.dll",
            "psapi.dll",
            "rasadhlp.dll",
            "rasapi32.dll",
            "rasman.dll",
            "resourcepolicyclient.dll",
            "rpcrt4.dll",
            "rsaenh.dll",
            "rtutils.dll",
            "samcli.dll",
            "schannel.dll",
            "sechost.dll",
            "secur32.dll",
            "setupapi.dll",
            "shcore.dll",
            "shell32.dll",
            "shlwapi.dll",
            "sspicli.dll",
            "system.configuration.ni.dll",
            "system.core.ni.dll",
            "system.data.dll",
            "system.data.ni.dll",
            "system.drawing.ni.dll",
            "system.management.ni.dll",
            "system.ni.dll",
            "system.numerics.ni.dll",
            "system.runtime.remoting.ni.dll",
            "system.runtime.serialization.ni.dll",
            "system.web.ni.dll",
            "system.windows.forms.ni.dll",
            "system.xml.ni.dll",
            "textinputframework.dll",
            "textshaping.dll",
            "twinapi.appcore.dll",
            "ucrtbase_clr0400.dll",
            "ucrtbase.dll",
            "uiautomationcore.dll",
            "umpdc.dll",
            "urlmon.dll",
            "user32.dll",
            "userenv.dll",
            "usp10.dll",
            "uxtheme.dll",
            "vcruntime140_clr0400.dll",
            "version.dll",
            "wbemcomn.dll",
            "wbemprox.dll",
            "wbemsvc.dll",
            "wevtapi.dll",
            "win32u.dll",
            "windows.storage.dll",
            "windowscodecs.dll",
            "winhttp.dll",
            "wininet.dll",
            "winmm.dll",
            "winmmbase.dll",
            "winnsi.dll",
            "winspool.drv",
            "winsta.dll",
            "wintrust.dll",
            "wintypes.dll",
            "wkscli.dll",
            "wldp.dll",
            "wminet_utils.dll",
            "wmiutils.dll",
            "ws2_32.dll",
            "wtsapi32.dll",
            "xinput1_4.dll",
        };

        public static IReadOnlyList<string> Modules { get; } = _modules.AsReadOnly();
    }
}