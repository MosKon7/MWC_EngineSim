namespace RealisticMotorSound.Audio
{
    /// <summary>
    /// Per-frame mix snapshot for HUD and logging
    /// </summary>
    public sealed class MixDebugInfo
    {
        public string VehicleName = "-";
        public string ProfileId = "-";
        public float Rpm;
        public float Throttle01;
        public bool EngineOn;
        public bool Starter;
        public float LowRpm;
        public float HighRpm;
        public float CoastThr;
        public float LoadThr;
        public float RpmBlend;
        public float ThrBlend;
        public float LowCoastVol;
        public float LowLoadVol;
        public float HighCoastVol;
        public float HighLoadVol;
        public int MutedSources;
        public int ForeignSources;
        public int PlayingForeign;
        public float VanillaThrottleVolField;
        public float VanillaNoThrottleVolField;
        public string ActiveClips = "-";
        public string ActiveBank = "-";
        public float SimTickMs;
        public float SimTickAvgMs;
        public float BufferFill01;
        public float UnderrunRate01;
        public int UnderrunFrames;
    }
}
