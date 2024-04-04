using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*namespace SchiffeVersenken
{
    internal class Spiel
    {
        private const int SpielfeldGroesse = 12;
        private const int AnzahlSchiffe = 10;

        private ZellenStatus[,] spielfeld;
        private List<Schiff> schiffe;
        private Random zufallsgenerator;

        public void Start ()
        {
            spielfeld = new ZellenStatus[SpielfeldGroesse, SpielfeldGroesse];
            schiffe = new List<Schiff>();
            zufallsgenerator = new Random();

            InitialisiereSpielfeld();
            PlatziereSchiffe();
            Spielablauf();
        }

        private void InitialisiereSpielfeld ()
        {
            for (int i = 0; i < SpielfeldGroesse; i++)
            {
                for (int j = 0; j < SpielfeldGroesse; j++)
                {
                    spielfeld[i, j] = ZellenStatus.Leer;
                }
            }
        }

        private void PlatziereSchiffe ()
        {
            for (int i = 4; i >= 1; i--)
            {
                for (int j = 0; j < 5 - i; j++)
                {
                    PlatzierenNeuesSchiff(i);
                }
            }
        }

        private void PlatzierenNeuesSchiff (int laenge)
        {
            Schiff neuesSchiff = new Schiff(laenge);

            bool platziert = false;
            while (!platziert)
            {
                int x = zufallsgenerator.Next(0, SpielfeldGroesse);
                int y = zufallsgenerator.Next(0, SpielfeldGroesse);
                bool horizontal = zufallsgenerator.Next(2) == 0;

                if (IstPlatzVerfuegbar(x, y, laenge, horizontal))
                {
                    neuesSchiff.Platzieren(x, y, horizontal);
                    schiffe.Add(neuesSchiff);
                    platziert = true;

                    // Markieren Sie das Spielfeld als Schiff
                    foreach (var position in neuesSchiff.Positionen)
                    {
                        spielfeld[position[0], position[1]] = ZellenStatus.Schiff;
                    }
                }
            }
        }

        private bool IstPlatzVerfuegbar (int startX, int startY, int laenge, bool horizontal)
        {
            if (horizontal)
            {
                for (int i = 0; i < laenge; i++)
                {
                    if (startX + i >= SpielfeldGroesse || spielfeld[startX + i, startY] != ZellenStatus.Leer)
                    {
                        return false;
                    }
                }
            }
            else
            {
                for (int i = 0; i < laenge; i++)
                {
                    if (startY + i >= SpielfeldGroesse || spielfeld[startX, startY + i] != ZellenStatus.Leer)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private void Spielablauf ()
        {
            while (true)
            {
                // Spieler schießt
                Console.WriteLine("Spielfeld:");
                ZeigeSpielfeld();
                Console.WriteLine("Geben Sie die Koordinaten für Ihren Schuss ein (z.B. A3):");
                string eingabe = Console.ReadLine().ToUpper();
                int x = eingabe[0] - 'A';
                int y = int.Parse(eingabe.Substring(1)) - 1;

                if (x < 0 || x >= SpielfeldGroesse || y < 0 || y >= SpielfeldGroesse)
                {
                    Console.WriteLine("Ungültige Koordinaten! Bitte erneut eingeben.");
                    continue;
                }

                ZellenStatus status = spielfeld[x, y];
                if (status == ZellenStatus.Treffer || status == ZellenStatus.Versenkt)
                {
                    Console.WriteLine("Bereits geschossen! Bitte erneut eingeben.");
                    continue;
                }

                if (status == ZellenStatus.Schiff)
                {
                    Console.WriteLine("Treffer!");
                    spielfeld[x, y] = ZellenStatus.Treffer;
                    Schiff getroffenesSchiff = FindeGetroffenesSchiff(x, y);
                    if (getroffenesSchiff.IstVersenkt(spielfeld))
                    {
                        Console.WriteLine("Schiff versenkt!");
                    }
                }
                else
                {
                    Console.WriteLine("Kein Treffer.");
                    spielfeld[x, y] = ZellenStatus.Leer;
                }

                // Überprüfen, ob alle Schiffe versenkt wurden
                if (schiffe.TrueForAll(schiff => schiff.IstVersenkt(spielfeld)))
                {
                    Console.WriteLine("Herzlichen Glückwunsch! Alle Schiffe versenkt!");
                    break;
                }
            }
        }

        private Schiff FindeGetroffenesSchiff (int x, int y)
        {
            foreach (var schiff in schiffe)
            {
                foreach (var position in schiff.Positionen)
                {
                    if (position[0] == x && position[1] == y)
                    {
                        return schiff;
                    }
                }
            }
            return null;
        }

        private void ZeigeSpielfeld ()
        {
            Console.Write("  ");
            for (int i = 1; i <= SpielfeldGroesse; i++)
            {
                Console.Write($"{i} ");
            }
            Console.WriteLine();

            for (int i = 0; i < SpielfeldGroesse; i++)
            {
                Console.Write($"{(char)('A' + i)} ");
                for (int j = 0; j < SpielfeldGroesse; j++)
                {
                    switch (spielfeld[i, j])
                    {
                        case ZellenStatus.Leer:
                            Console.Write(". ");
                            break;
                        case ZellenStatus.Schiff:
                            Console.Write("O ");
                            break;
                        case ZellenStatus.Treffer:
                            Console.Write("X ");
                            break;
                        case ZellenStatus.Versenkt:
                            Console.Write("# ");
                            break;
                    }
                }
                Console.WriteLine();
            }
        }
    }
}
    */