using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using ITS.WRM.SFA.Droid.Classes.Helpers;
namespace ITS.WRM.SFA.Droid.Classes
{
    /// <summary>
    /// A listing of ASCII control characters for readability.
    /// </summary>
    public static class AsciiControlChars
    {
        /// <summary>
        /// Usually indicates the end of a string.
        /// </summary>
        public const byte Nul = (byte)0x00;
        /// <summary>
        /// Meant to be used for printers. When receiving this code the 
        /// printer moves to the next sheet of paper.
        /// </summary>
        public const byte FormFeed = (byte)0x0C;
        /// <summary>
        /// Starts an extended sequence of control codes.
        /// </summary>
        public const byte Escape = (byte)0x1B;
        /// <summary>
        /// Advances to the next line.
        /// </summary>
        public static byte[] Newline = new byte[] { Escape, (byte)0x64, 1 };
        /// <summary>
        /// Defined to separate tables or different sets of data in a serial
        /// data storage system.
        /// </summary>
        public const byte GroupSeparator = (byte)0x1D;
        /// <summary>
        /// A horizontal tab.
        /// </summary>
        public const byte HorizontalTab = (byte)0x09;
        /// <summary>
        /// Vertical Tab
        /// </summary>
        public const byte VerticalTab = (byte)0x11;
        /// <summary>
        /// Returns the carriage to the start of the line.
        /// </summary>
        public const byte CarriageReturn = (byte)0x0D;
        /// <summary>
        /// Cancels the operation.
        /// </summary>
        public const byte Cancel = (byte)0x18;
        /// <summary>
        /// Indicates that control characters present in the stream should
        /// be passed through as transmitted and not interpreted as control
        /// characters.
        /// </summary>
        public const byte DataLinkEscape = (byte)0x10;
        /// <summary>
        /// Signals the end of a transmission.
        /// </summary>
        public const byte EndOfTransmission = (byte)0x04;
        /// <summary>
        /// In serial storage, signals the separation of two files.
        /// </summary>
        public const byte FileSeparator = (byte)0x1C;
        /// <summary>
        /// Sets justification to center
        /// </summary>
        //public static byte[] CenterJustification = new byte[] { (byte)0x1B, (byte)0x61, (byte)0x01 };
        //public static byte[] LeftJustification = new byte[] { (byte)0x1B, (byte)0x61, (byte)0x00 };
        //public static byte[] RightJustification = new byte[] { (byte)0x1B, (byte)0x61, (byte)0x02 };
        //public static void Enlarged(this List<byte> bw, string text, bool zpl = true, string justification = null)
        //{
        //    if (zpl)
        //    {
        //        bw.AddRange(Encoding.UTF8.GetBytes(string.Format("^FO0,{0},0^CI28^A@N,40,30,E:SWI000.TTF^FB{1},4,,{2}", PrintDocumentHelper.length, PrintDocumentHelper.width, justification)));
        //        bw.AddRange(Encoding.UTF8.GetBytes("^FD" + text + "^FS"));
        //        PrintDocumentHelper.length += 40;
        //    }
        //    else
        //    {
        //        bw.Add(AsciiControlChars.Escape);
        //        bw.Add((byte)33);
        //        bw.Add((byte)32);
        //        bw.AddRange(EncodedTextBytes(text));
        //        bw.AddRange(AsciiControlChars.Newline);
        //    }
        //}
        //public static void High(this List<byte> bw, string text, bool line = true, bool zpl = true)
        //{
        //    if (zpl)
        //    {
        //        bw.AddRange(Encoding.UTF8.GetBytes("^A@N,35,25,E:SWI000.TTF"));
        //        bw.AddRange(Encoding.UTF8.GetBytes("^FD" + text + "^FS"));
        //        if (line)
        //            bw.FeedLines(1);
        //    }
        //    else
        //    {
        //        bw.Add(AsciiControlChars.Escape);
        //        bw.Add((byte)33);
        //        bw.Add((byte)16);
        //        bw.AddRange(EncodedTextBytes(text)); //Width,enlarged
        //        if (line)
        //            bw.AddRange(AsciiControlChars.Newline);
        //    }
        //}
        //public static void LargeText(this List<byte> bw, string text, bool zpl = true, string justification = null)
        //{
        //    if (zpl)
        //    {
        //        bw.AddRange(Encoding.UTF8.GetBytes(string.Format("^FO0,{0},0^CI28^A@N,35,25,E:SWI000.TTF^FB{1},4,,{2}", PrintDocumentHelper.length, PrintDocumentHelper.width, justification)));
        //        bw.AddRange(Encoding.UTF8.GetBytes("^FD" + text + "^FS"));
        //        PrintDocumentHelper.length += 35;
        //    }
        //    else
        //    {
        //        bw.Add(AsciiControlChars.Escape);
        //        bw.Add((byte)0x21);
        //        bw.Add((byte)0x8);
        //        bw.AddRange(EncodedTextBytes(text));
        //        bw.AddRange(AsciiControlChars.Newline);
        //    }
        //}
        //public static void FeedLines(this List<byte> bw, int lines, bool zpl = true)
        //{
        //    for (int i = 0; i < lines; i++)
        //    {
        //        if (zpl)
        //        {
        //            bw.AddRange(Encoding.UTF8.GetBytes(string.Format("^FO0,{0},0^CI28^A@N,25,25,E:SWI000.TTF^FB{1},4,,C", PrintDocumentHelper.length, PrintDocumentHelper.width)));
        //            bw.AddRange(Encoding.UTF8.GetBytes("^FD" + "   " + "^FS"));
        //            PrintDocumentHelper.length += 25;
        //        }
        //        else
        //        {
        //            bw.AddRange(AsciiControlChars.Newline);
        //        }
        //    }
        //}
        //public static void Finish(this List<byte> bw, bool zpl = true, string justification = null)
        //{
        //    if (zpl)
        //    {
        //        bw.AddRange(Encoding.UTF8.GetBytes(string.Format("^FO0,{0},0^CI28^A@N,25,25,E:SWI000.TTF^FB{1},4,,{2}", PrintDocumentHelper.length, PrintDocumentHelper.width, justification)));
        //        bw.AddRange(Encoding.UTF8.GetBytes("^FD-----------------------------^FS"));
        //        PrintDocumentHelper.length += 25;
        //        bw.AddRange(Encoding.UTF8.GetBytes(string.Format("^FO0,{0},0^CI28^A@N,25,25,E:SWI000.TTF^FB{1},4,,{2}", PrintDocumentHelper.length, PrintDocumentHelper.width, justification)));
        //        bw.AddRange(Encoding.UTF8.GetBytes("^FDΕΥΧΑΡΙΣΤΟΥΜΕ ΓΙΑ ΤΗΝ ΠΡΟΤΙΜΗΣΗ^FS"));
        //        PrintDocumentHelper.length += 25;
        //        bw.AddRange(Encoding.UTF8.GetBytes(string.Format("^FO0,{0},0^CI28^A@N,25,25,E:SWI000.TTF^FB{1},4,,{2}", PrintDocumentHelper.length, PrintDocumentHelper.width, justification)));
        //        bw.AddRange(Encoding.UTF8.GetBytes("^FD-----------------------------^FS"));
        //        PrintDocumentHelper.length += 25;
        //        bw.AddRange(Encoding.UTF8.GetBytes("^XZ"));
        //    }
        //    else
        //    {
        //        bw.FeedLines(1, false);
        //        bw.NormalFont("-----------------------------", false);
        //        bw.AddRange(AsciiControlChars.CenterJustification);
        //        bw.NormalFont("ΕΥΧΑΡΙΣΤΟΥΜΕ ΓΙΑ ΤΗΝ ΠΡΟΤΙΜΗΣΗ", false);
        //        bw.NormalFont("-----------------------------", false);
        //        bw.FeedLines(2, false);
        //        bw.Add(0x0A);
        //    }
        //}
        //public static void NormalFont(this List<byte> bw, string text, bool zpl = true, bool line = true, string justification = null)
        //{
        //    if (zpl)
        //    {
        //        bw.AddRange(Encoding.UTF8.GetBytes(string.Format("^FO0,{0},0^CI28^A@N,25,15,E:SWI000.TTF^FB{1},4,,{2}", PrintDocumentHelper.length, PrintDocumentHelper.width, justification)));
        //        bw.AddRange(Encoding.UTF8.GetBytes("^FD" + text + "^FS"));
        //        PrintDocumentHelper.length += 25;
        //        if (line)
        //            bw.FeedLines(1);
        //    }
        //    else
        //    {
        //        bw.Add(AsciiControlChars.Escape);
        //        bw.Add((byte)33);
        //        bw.Add((byte)8);
        //        bw.AddRange(EncodedTextBytes(" " + text));
        //        if (line)
        //            bw.AddRange(AsciiControlChars.Newline);
        //    }
        //}
        //public static void GreekEncoding(this List<byte> bw)
        //{
        //    bw.Add(AsciiControlChars.Escape);
        //    bw.Add((byte)0x74);
        //    if (App.SFASettings.BlueToothPrinter.ToLower().Contains("printer"))
        //    {
        //        bw.Add(7);
        //    }
        //    else
        //    {
        //        bw.Add(24);
        //    }
        //}

        public static void GreekEncoding2(this List<byte> bw)
        {
            bw.Add(AsciiControlChars.Escape);
            bw.Add((byte)0x74);
            bw.Add(24);
        }

        //public static byte[] EncodedTextBytes(string text)
        //{
        //    byte[] bytes;
        //    if (!App.SFASettings.BlueToothPrinter.ToLower().Contains("printer"))
        //    {
        //        var isoEncoding = Encoding.GetEncoding(1253);
        //        bytes = Encoding.Convert(Encoding.Unicode, isoEncoding, Encoding.Unicode.GetBytes(text));
        //    }
        //    else
        //    {
        //        bytes = Encoding.UTF8.GetBytes(text);
        //    }
        //    return bytes;
        //}
    }
}