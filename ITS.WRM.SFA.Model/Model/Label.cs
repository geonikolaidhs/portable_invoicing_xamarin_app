using ITS.WRM.SFA.Model.Model.NonPersistant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITS.WRM.SFA.Model.Model
{
    public class Label: LookUp2Fields
    {
        public string _LabelFileName;
        public byte[] _LabelFile;
        public bool _UseDirectSQL;
        public string _DirectSQL;
        public int _PrinterEncoding;
    }
}
