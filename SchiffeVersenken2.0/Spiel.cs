using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchiffeVersenken {
    abstract class Spiel {
        protected const int SpielfeldGroesse = 12;
        protected const int AnzahlSchiffe = 10;

        protected ZellenStatus[,] spielfeldSpieler;
        protected ZellenStatus[,] spielfeldGegner;
        protected List<Schiff> schiffeSpieler;
        protected List<Schiff> schiffeGegner;
        protected Random zufallsgenerator;

        public Spiel ()
        {
            spielfeldSpieler = new ZellenStatus[SpielfeldGroesse, SpielfeldGroesse];
            spielfeldGegner = new ZellenStatus[SpielfeldGroesse, SpielfeldGroesse];
            schiffeSpieler = new List<Schiff> ();
            schiffeGegner = new List<Schiff> ();
            zufallsgenerator = new Random ();
        }
        public abstract void Start ();

        protected void InitialisiereSpielfeld (ZellenStatus[,] spielfeld)
        {
            for (int i = 0; i < SpielfeldGroesse; i++) {
                for (int j = 0; j < SpielfeldGroesse; j++) {
                    spielfeld[i, j] = ZellenStatus.Unbekannt;
                }
            }
        }

        protected void PlatziereSchiffe (List<Schiff> schiffe, ZellenStatus[,] spielfeld)
        {
            int[] schiffsGroessen = { 4, 3, 3, 2, 2, 2, 1, 1, 1, 1 };
            //int[] schiffsGroessen = { 4, 4, 4, 4, 4, 4, 4, 4};

            for (int i = 0; i < schiffsGroessen.Length; i++) {
                PlatzierenNeuesSchiff (schiffsGroessen[i], schiffe, spielfeld);
            }
        }

        protected void PlatzierenNeuesSchiff (int laenge, List<Schiff> schiffe, ZellenStatus[,] spielfeld)
        {
            Schiff neuesSchiff = new Schiff(laenge);

            bool platziert = false;
            while (!platziert) {
                int x = zufallsgenerator.Next(0, SpielfeldGroesse);
                int y = zufallsgenerator.Next(0, SpielfeldGroesse);
                bool horizontal = zufallsgenerator.Next(2) == 0;

                if (IstPlatzVerfuegbar (x, y, laenge, horizontal, schiffe, spielfeld)) {
                    neuesSchiff.Platzieren (x, y, horizontal);
                    schiffe.Add (neuesSchiff);

                    // Setze Zellenstatus auf "Schiff" für jedes Feld des platzierten Schiffs
                    foreach (var position in neuesSchiff.Positionen) {
                        spielfeld[position[0], position[1]] = ZellenStatus.Schiff;
                    }

                    platziert = true;
                }
            }
        }

        protected bool IstPlatzVerfuegbar (int startX, int startY, int laenge, bool horizontal, List<Schiff> schiffe, ZellenStatus[,] spielfeld)
        {
            // Überprüfe, ob das Schiff außerhalb des Spielfelds platziert wird
            if (horizontal && startX + laenge > SpielfeldGroesse)
                return false;
            if (!horizontal && startY + laenge > SpielfeldGroesse)
                return false;

            // Überprüfe, ob das Schiff überlappende Felder hat oder sich zu nahe an anderen Schiffen befindet
            for (int i = -1; i <= laenge; i++) {
                for (int j = -1; j <= 1; j++) {
                    int x = startX + (horizontal ? i : j);
                    int y = startY + (horizontal ? j : i);

                    if (x >= 0 && x < SpielfeldGroesse && y >= 0 && y < SpielfeldGroesse) {
                        if (spielfeld[x, y] != ZellenStatus.Unbekannt)
                            return false;
                    }
                }
            }
            return true;
        }

        protected abstract void Spielablauf ();

        protected void ZeigeEigenesSpielfeld ()
        {
            Console.WriteLine ("Dein Spielfeld:");
            Console.Write ("  ");
            for (int i = 1; i <= SpielfeldGroesse; i++) {
                Console.Write ($"{i} ");
            }
            Console.WriteLine ();

            for (int i = 0; i < SpielfeldGroesse; i++) {
                Console.Write ($"{(char) ('A' + i)} ");
                for (int j = 0; j < SpielfeldGroesse; j++) {
                    switch (spielfeldSpieler[i, j]) {
                        case ZellenStatus.Leer:
                            Console.Write (".  ");
                            break;
                        case ZellenStatus.Treffer:
                            Console.Write ("X  ");
                            break;
                        case ZellenStatus.Versenkt:
                            Console.Write ("#  ");
                            break;
                        case ZellenStatus.Verfehlt:
                            Console.Write ("*  ");
                            break;
                        default:
                            Console.Write (".  ");
                            break;
                    }
                }
                Console.WriteLine ();
            }
        }

        public void MarkiereVersenkt (Schiff versenktesSchiff, ZellenStatus[,] spielfeld)
        {
            foreach (var position in versenktesSchiff.Positionen) {
                spielfeld[position[0], position[1]] = ZellenStatus.Versenkt;
            }
        }

        protected void ZeigeGegnerSpielfeld (ZellenStatus[,] spielfeldGegner, bool isPlayer)
        {
            if (isPlayer)
                Console.WriteLine ("Gegnerisches Spielfeld:");
            else
                Console.WriteLine ("Eigenes Spielfeld:");
            Console.Write ("  ");
            for (int i = 1; i <= SpielfeldGroesse; i++) {
                if (i < 10)
                    Console.Write ($"0{i} ");
                else
                    Console.Write ($"{i} ");
            }
            Console.WriteLine ();

            for (int i = 0; i < SpielfeldGroesse; i++) {
                Console.Write ($"{(char) ('A' + i)} ");
                for (int j = 0; j < SpielfeldGroesse; j++) {
                    switch (spielfeldGegner[i, j]) {
                        case ZellenStatus.Leer:
                            Console.Write (".  ");
                            break;
                        case ZellenStatus.Treffer:
                            Console.Write ("X  ");
                            break;
                        case ZellenStatus.Versenkt:
                            Console.Write ("#  ");
                            break;
                        case ZellenStatus.Verfehlt:
                            Console.Write ("*  ");
                            break;
                        default:
                            Console.Write (".  ");
                            break;
                    }
                }
                Console.WriteLine ();
            }
        }

        protected void ClearConsole ()
        {
            Console.Clear ();
            Console.SetCursorPosition (0, 0);
        }
    }
}
