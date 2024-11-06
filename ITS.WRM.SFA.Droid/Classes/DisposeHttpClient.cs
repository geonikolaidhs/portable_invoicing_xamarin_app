using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ITS.WRM.SFA.Droid.Classes
{
    public class DisposeHttpClient:HttpClient
    {
        public DisposeHttpClient(HttpMessageHandler handler) : base(handler)
        {
        }

        public DisposeHttpClient() : base()
        {
        }

        public bool IsDisposed { get; set; }
        protected override void Dispose(bool disposing)
        {
            IsDisposed = true;
            base.Dispose(disposing);
        }
    }
}
