using System;
using System.IO;
using System.Runtime.InteropServices;
using MSCLoader;

namespace RealisticMotorSound.Audio
{
    /// <summary>
    /// PInvoke bindings for EngineSimNative.dll
    /// </summary>
    public static class EngineSimNative
    {
        public const string DllName = "EngineSimNative";

        private static bool _loadAttempted;
        private static bool _available;
        private static string _nativeDir = string.Empty;

        [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern IntPtr LoadLibrary(string lpFileName);

        [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern bool SetDllDirectory(string lpPathName);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr esm_create(string scriptPath);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void esm_destroy(IntPtr handle);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void esm_set_targets(IntPtr handle, float rpm, float throttle01, int ignitionOn);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int esm_tick(IntPtr handle, float dtSeconds);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int esm_read_pcm(IntPtr handle, short[] dst, int maxSamples);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void esm_set_quality(IntPtr handle, int simHz);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void esm_set_fluid_steps(IntPtr handle, int steps);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void esm_set_mix(
            IntPtr handle,
            float volume,
            float noise,
            float jitter,
            float hfGain,
            float convolution);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void esm_set_highpass(IntPtr handle, float cutoffHz);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr esm_last_error();

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int esm_queued_samples(IntPtr handle);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl)]
        public static extern float esm_last_tick_ms(IntPtr handle);

        /// <summary>
        /// Loads native DLL and deps from a folder outside Mods root
        /// </summary>
        /// <param name="nativeDirectory">Folder with EngineSimNative.dll and Boost DLLs</param>
        /// <returns>True when LoadLibrary succeeds</returns>
        public static bool TryBootstrap(string nativeDirectory)
        {
            if (_loadAttempted)
                return _available;

            _loadAttempted = true;
            if (string.IsNullOrEmpty(nativeDirectory) || !Directory.Exists(nativeDirectory))
            {
                ModConsole.Error("[RealisticMotorSound] Native folder missing: " + nativeDirectory);
                return false;
            }

            string dllPath = Path.Combine(nativeDirectory, DllName + ".dll");
            if (!File.Exists(dllPath))
            {
                ModConsole.Error("[RealisticMotorSound] EngineSimNative.dll not found in " + nativeDirectory);
                return false;
            }

            // MSCLoader loads every Mods/*.dll as a managed mod.
            // Keep native binaries only under Assets/.../engine_sim.
            SetDllDirectory(nativeDirectory);
            _nativeDir = nativeDirectory;

            try
            {
                string[] deps = Directory.GetFiles(nativeDirectory, "boost_*.dll");
                for (int i = 0; i < deps.Length; i++)
                    LoadLibrary(deps[i]);
            }
            catch
            {
            }

            IntPtr module = LoadLibrary(dllPath);
            if (module != IntPtr.Zero)
            {
                _available = true;
                ModConsole.Print("[RealisticMotorSound] Loaded native " + dllPath);
                return true;
            }

            int err = Marshal.GetLastWin32Error();
            ModConsole.Error("[RealisticMotorSound] LoadLibrary failed for EngineSimNative.dll Win32=" + err);
            return false;
        }

        /// <summary>
        /// Returns true when the native library is loaded
        /// </summary>
        public static bool IsAvailable
        {
            get { return _available; }
        }

        /// <summary>
        /// Returns folder used for native DLL search
        /// </summary>
        public static string NativeDirectory
        {
            get { return _nativeDir; }
        }

        /// <summary>
        /// Reads last native error string
        /// </summary>
        /// <returns>Error text</returns>
        public static string LastError()
        {
            try
            {
                IntPtr ptr = esm_last_error();
                if (ptr == IntPtr.Zero)
                    return "unknown";
                return Marshal.PtrToStringAnsi(ptr) ?? "unknown";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
