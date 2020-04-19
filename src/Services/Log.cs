using System;
using System.Globalization;
using System.Drawing;
using Pastel;

namespace sunshine.Services
{
    public class LogService
    {
        private void log (string s) {
            var _ = s.Split('\n');
            var __ = DateTime.Now.ToString("o", CultureInfo.InvariantCulture);
            Console.WriteLine($"{__} | {_[0]}");
            foreach (var statement in new ArraySegment<string>(_, 1, _.Length - 1))
                Console.WriteLine($"{new String(' ', __.Length)} | {statement}");
        }

        public void success (string s) {
            this.log($"{"[Success]".Pastel(Color.Black).PastelBg(Color.LightGreen)} " + s);
        }

        public void error (string s) {
            this.log($"{"[Error]".PastelBg(Color.Red)} " + s);
        }

        public void warning (string s) {
            this.log($"{"[Warning]".PastelBg(Color.Yellow)} " + s);
        }

        public void info (string s) {
            this.log($"{"[Info]".PastelBg(Color.Blue)} " + s);
        }
    }
}