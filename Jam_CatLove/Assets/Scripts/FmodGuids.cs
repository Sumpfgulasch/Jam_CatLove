/*
    FmodGuids.cs - FMOD Studio API

    Generated GUIDs for project 'CatLove.fspro'
*/

using System;
using System.Collections.Generic;

namespace Audio
{
    public class AudioEvent
    {
        public static readonly FMOD.GUID CatMeows = new FMOD.GUID { Data1 = 1369531543, Data2 = 1224165700, Data3 = -1045484356, Data4 = 377330144 };
        public static readonly FMOD.GUID CatPetting = new FMOD.GUID { Data1 = -1245905491, Data2 = 1267082880, Data3 = 339970457, Data4 = -588250560 };
        public static readonly FMOD.GUID CatPurr = new FMOD.GUID { Data1 = 449229274, Data2 = 1122905702, Data3 = 1781778086, Data4 = 1294659576 };


        public static readonly Dictionary<string, FMOD.GUID> AudioEventNameToGuid = new Dictionary<string, FMOD.GUID>()
        {
                {"CatMeows", CatMeows}, {"CatPetting", CatPetting}, {"CatPurr", CatPurr}, 
        };
    }

    public class AudioBus
    {
        public static readonly FMOD.GUID MasterBus = new FMOD.GUID { Data1 = -323976756, Data2 = 1138677162, Data3 = -583288656, Data4 = -142521793 };
        public static readonly FMOD.GUID Reverb = new FMOD.GUID { Data1 = -38252101, Data2 = 1102538477, Data3 = -819948353, Data4 = -1790549757 };


        public static readonly Dictionary<string, FMOD.GUID> AudioBusNameToGuid = new Dictionary<string, FMOD.GUID>()
        {
                {"MasterBus", MasterBus}, {"Reverb", Reverb}, 
        };
    }

    public class AudioBank
    {
        public static readonly FMOD.GUID Master = new FMOD.GUID { Data1 = 598590988, Data2 = 1190612160, Data3 = 553388216, Data4 = 653845446 };


        public static readonly Dictionary<string, FMOD.GUID> AudioBankNameToGuid = new Dictionary<string, FMOD.GUID>()
        {
                {"Master", Master}, 
        };
    }

}

