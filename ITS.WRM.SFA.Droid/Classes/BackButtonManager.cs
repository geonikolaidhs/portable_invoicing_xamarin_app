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

namespace ITS.WRM.SFA.Droid.Classes
{
    public class BackButtonManager
    {
        public static readonly BackButtonManager Instance = new BackButtonManager();

        public delegate bool OnBackButtonPressedDelegate();

        private OnBackButtonPressedDelegate onBackButtonPressedListener;

        private BackButtonManager()
        {

        }

        public bool NotifyBackButtonPressed()
        {

            if (onBackButtonPressedListener != null)
            {
                return onBackButtonPressedListener.Invoke();
            }

            return true;
        }

        public void SetBackButtonListener(OnBackButtonPressedDelegate onBackButtonPressedListener)
        {
            this.onBackButtonPressedListener = onBackButtonPressedListener;
        }

        public void RemoveBackButtonListener()
        {
            onBackButtonPressedListener = null;
        }
    }
}