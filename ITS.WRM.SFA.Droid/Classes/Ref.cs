using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using ITS.WRM.SFA.Model.NonPersistant;

namespace ITS.WRM.SFA.Droid.Classes
{
    public class Ref<T> where T : BasicObj
    {
        public Ref() { }

        public Ref(T value) { Value = value; }

        public T Value { get; set; }

        public override string ToString()
        {
            T value = Value;
            return value == null ? "" : value.ToString();
        }

        public static implicit operator T(Ref<T> r) { return r.Value; }

        public static implicit operator Ref<T>(T value) { return new Ref<T>(value); }
    }
}