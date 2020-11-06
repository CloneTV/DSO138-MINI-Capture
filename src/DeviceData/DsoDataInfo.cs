namespace DSO138Device
{
    public class DsoDataInfoIndex
    {
        public int Id;
        public string Name;
        public string Value;
    }

    public class DsoDataInfo : INotify
    {
        public enum Ids : int
        {
            ID_VSen,
            ID_Couple,
            ID_VPos,
            ID_Timebase,
            ID_HPos,
            ID_TriggerMode,
            ID_TriggerSlope,
            ID_TriggerLevel,
            ID_RecordLength,
            ID_Vmax,
            ID_Vmin,
            ID_Vavr,
            ID_Vpp,
            ID_Vrms,
            ID_Freq,
            ID_Cycl,
            ID_PW,
            ID_Duty,
            ID_SampleInterval,
            ID_End
        };

        public static readonly Ids[] dataAnnotationIdx = new Ids[]
        {
            Ids.ID_VPos, Ids.ID_HPos, Ids.ID_Vmax, Ids.ID_Vmin, Ids.ID_Vavr, Ids.ID_Vpp,
            Ids.ID_Vrms, Ids.ID_Freq, Ids.ID_Cycl, Ids.ID_PW, Ids.ID_Duty
            // Ids.ID_RecordLength, Ids.ID_SampleInterval
        };
        public static readonly string[] Names = new string[]  {
            "VSen",
            "Couple",
            "VPos",
            "Timebase",
            "HPos",
            "TriggerMode",
            "TriggerSlope",
            "TriggerLevel",
            "RecordLength",
            "Vmax",
            "Vmin",
            "Vavr",
            "Vpp",
            "Vrms",
            "Freq",
            "Cycl",
            "PW",
            "Duty",
            "SampleInterval",
            ""
        };
        private string[] Values = new string[(int)Ids.ID_End];
        
        public string get(int id)
        {
            return (id < (int)Ids.ID_End) ? Values[id] : "";
        }
        public string get(Ids ids)
        {
            return (ids < Ids.ID_End) ? Values[(int)ids] : "";
        }
        public void set(int id, string s)
        {
            if (id >= (int)Ids.ID_End)
                return;
            Values[id] = s;
        }
        public void set(Ids ids, string s)
        {
            if (ids >= Ids.ID_End)
                return;
            Values[(int)ids] = s;
        }
        public bool update(int id, ref string valName, ref string valData)
        {
            if ((id >= (int)Ids.ID_End) || (!Names[id].Equals(valName)))
                return false;
            Values[id] = valData;
            return true;
        }

        /// 

        public string VSen
        {
            get { return Values[(int)Ids.ID_VSen]; }
            set { Values[(int)Ids.ID_VSen] = value; OnPropertyChanged(Names[(int)Ids.ID_VSen]);  }
        }
        public string Couple
        {
            get { return Values[(int)Ids.ID_Couple]; }
            set { Values[(int)Ids.ID_Couple] = value; OnPropertyChanged(Names[(int)Ids.ID_Couple]); }
        }
        public string VPos
        {
            get { return Values[(int)Ids.ID_VPos]; }
            set { Values[(int)Ids.ID_VPos] = value; OnPropertyChanged(Names[(int)Ids.ID_VPos]); }
        }
        public string Timebase
        {
            get { return Values[(int)Ids.ID_Timebase]; }
            set { Values[(int)Ids.ID_Timebase] = value; OnPropertyChanged(Names[(int)Ids.ID_Timebase]); }
        }
        public string HPos
        {
            get { return Values[(int)Ids.ID_HPos]; }
            set { Values[(int)Ids.ID_HPos] = value; OnPropertyChanged(Names[(int)Ids.ID_HPos]); }
        }
        public string TriggerMode
        {
            get { return Values[(int)Ids.ID_TriggerMode]; }
            set { Values[(int)Ids.ID_TriggerMode] = value; OnPropertyChanged(Names[(int)Ids.ID_TriggerMode]); }
        }
        public string TriggerSlope
        {
            get { return Values[(int)Ids.ID_TriggerSlope]; }
            set { Values[(int)Ids.ID_TriggerSlope] = value; OnPropertyChanged(Names[(int)Ids.ID_TriggerSlope]); }
        }
        public string TriggerLevel
        {
            get { return Values[(int)Ids.ID_TriggerLevel]; }
            set { Values[(int)Ids.ID_TriggerLevel] = value; OnPropertyChanged(Names[(int)Ids.ID_TriggerLevel]); }
        }
        public string RecordLength
        {
            get { return Values[(int)Ids.ID_RecordLength]; }
            set { Values[(int)Ids.ID_RecordLength] = value; OnPropertyChanged(Names[(int)Ids.ID_RecordLength]); }
        }
        public string Vmax
        {
            get { return Values[(int)Ids.ID_Vmax]; }
            set { Values[(int)Ids.ID_Vmax] = value; OnPropertyChanged(Names[(int)Ids.ID_Vmax]); }
        }
        public string Vmin
        {
            get { return Values[(int)Ids.ID_Vmin]; }
            set { Values[(int)Ids.ID_Vmin] = value; OnPropertyChanged(Names[(int)Ids.ID_Vmin]); }
        }
        public string Vavr
        {
            get { return Values[(int)Ids.ID_Vavr]; }
            set { Values[(int)Ids.ID_Vavr] = value; OnPropertyChanged(Names[(int)Ids.ID_Vavr]); }
        }
        public string Vpp
        {
            get { return Values[(int)Ids.ID_Vpp]; }
            set { Values[(int)Ids.ID_Vpp] = value; OnPropertyChanged(Names[(int)Ids.ID_Vpp]); }
        }
        public string Vrms
        {
            get { return Values[(int)Ids.ID_Vrms]; }
            set { Values[(int)Ids.ID_Vrms] = value; OnPropertyChanged(Names[(int)Ids.ID_Vrms]); }
        }
        public string Freq
        {
            get { return Values[(int)Ids.ID_Freq]; }
            set { Values[(int)Ids.ID_Freq] = value; OnPropertyChanged(Names[(int)Ids.ID_Freq]); }
        }
        public string Cycl
        {
            get { return Values[(int)Ids.ID_Cycl]; }
            set { Values[(int)Ids.ID_Cycl] = value; OnPropertyChanged(Names[(int)Ids.ID_Cycl]); }
        }
        public string PW
        {
            get { return Values[(int)Ids.ID_PW]; }
            set { Values[(int)Ids.ID_PW] = value; OnPropertyChanged(Names[(int)Ids.ID_PW]); }
        }
        public string Duty
        {
            get { return Values[(int)Ids.ID_Duty]; }
            set { Values[(int)Ids.ID_Duty] = value; OnPropertyChanged(Names[(int)Ids.ID_Duty]); }
        }
        public string SampleInterval
        {
            get { return Values[(int)Ids.ID_SampleInterval]; }
            set { Values[(int)Ids.ID_SampleInterval] = value; OnPropertyChanged(Names[(int)Ids.ID_SampleInterval]); }
        }
    }
}
