using SchiffeVersenken;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchiffeVersenken
{
    internal class Schiff
    {
        public int Laenge { get; }
        public List<int[]> Positionen { get; }

        public Schiff (int laenge)
        {
            Laenge = laenge;
            Positionen = new List<int[]>();
        }

        public void Platzieren (int startX, int startY, bool horizontal)
        {
            for (int i = 0; i < Laenge; i++)
            {
                if (horizontal)
                {
                    Positionen.Add(new int[] { startX + i, startY });
                }
                else
                {
                    Positionen.Add(new int[] { startX, startY + i });
                }
            }
        }

        public bool IstVersenkt (ZellenStatus[,] spielfeld)
        {
            foreach (var position in Positionen)
            {
                if (spielfeld[position[0], position[1]] != ZellenStatus.Treffer)
                {
                    return false;
                }
            }
            return true;
        }
    }
}
