using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Media;
using System.Text.RegularExpressions;

namespace SchiffeVersenken2._0 {
    internal class Sounds {
        private System.Media.SoundPlayer sploosh = new(Properties.Resources.sploosh1);
        private System.Media.SoundPlayer kaboooom = new (Properties.Resources.kaboooom1);

        public void PlaySploosh ()
        {
            sploosh.Play ();
        }

        public void PlayKaboom ()
        {
            kaboooom.Play ();
        }
    }
}
