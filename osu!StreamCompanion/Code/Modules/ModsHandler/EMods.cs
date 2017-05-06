using System;

namespace osu_StreamCompanion.Code.Modules.ModsHandler
{
    [Flags]
    public enum EMods
    {
        Omod = 0,
        Nf = 1 << 0,
        Ez = 1 << 1,
        //Nv = 1 << 2, //No video
        Hd = 1 << 3,
        Hr = 1 << 4,
        Sd = 1 << 5, //suddenDeath
        Dt = 1 << 6,
        RX = 1 << 7, //Relax
        Ht = 1 << 8,
        Nc = 1 << 9,
        Fl = 1 << 10,
        Ap = 1 << 11,//autoplay
        So = 1 << 12,
        Rx2= 1 << 13,
        Pf= 1 << 14,
        K4= 1 << 15,
        K5= 1 << 16,
        K6= 1 << 17,
        K7= 1 << 18,
        K8= 1 << 19,
        Fi= 1 << 20,
        Rn= 1 << 21,
        Lm= 1 << 22,
        //= 1 << 23,
        K9= 1 << 24,
        Coop= 1 << 25,
        K1= 1 << 26,
        K3= 1 << 27,
        K2= 1 << 28,
        SpeedChanging = Dt | Ht | Nc,
        MapChanging = Hr | Ez | SpeedChanging
    }
}