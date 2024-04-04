using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchiffeVersenken {
    class MehrspielerSpiel : Spiel {
        public override void Start ()
        {
            Console.WriteLine ("Du spielst gegen einen zweiten Spieler!");
            Console.WriteLine ("Spieler 1, platziere deine Schiffe:");
            InitialisiereSpielfeld (spielfeldSpieler);
            InitialisiereSpielfeld (spielfeldGegner);
            PlatziereSchiffe (schiffeSpieler, spielfeldSpieler);
            PlatziereSchiffe (schiffeGegner, spielfeldGegner);
            Spielablauf ();
        }

        protected override void Spielablauf ()
        {
            bool spieler1AmZug = true;

            while (true) {
                bool isPlayerOne = true;
                int x = 0;
                int y = 0;
                if (spieler1AmZug) {
                    // Spieler 1 schießt
                    Console.WriteLine ("Spieler 1:");
                    ZeigeGegnerSpielfeld (spielfeldGegner, isPlayerOne);
                    Console.WriteLine ("Spieler 1, geben Sie die Koordinaten für Ihren Schuss ein (z.B. A3):");
                    string eingabe = Console.ReadLine().ToUpper();
                    x = eingabe[0] - 'A';
                    y = int.Parse (eingabe.Substring (1)) - 1;

                    if (x < 0 || x >= SpielfeldGroesse || y < 0 || y >= SpielfeldGroesse) {
                        Console.WriteLine ("Ungültige Koordinaten! Bitte erneut eingeben.");
                        continue;
                    }

                    if (spielfeldGegner[x, y] == ZellenStatus.Treffer || spielfeldGegner[x, y] == ZellenStatus.Versenkt) {
                        Console.WriteLine ("Bereits geschossen! Bitte erneut eingeben.");
                        continue;
                    }

                    if (spielfeldGegner[x, y] == ZellenStatus.Schiff) {
                        Console.WriteLine ("Treffer!");
                        spielfeldGegner[x, y] = ZellenStatus.Treffer;
                        Schiff getroffenesSchiff = FindeGetroffenesSchiff(x, y, schiffeGegner);
                        if (getroffenesSchiff.IstVersenkt (spielfeldGegner)) {
                            Console.WriteLine ("Schiff versenkt!");
                            MarkiereVersenkt (getroffenesSchiff, spielfeldGegner);
                        }
                        continue;
                    } else {
                        Console.WriteLine ("Kein Treffer.");
                        spielfeldGegner[x, y] = ZellenStatus.Verfehlt;
                    }
                    spieler1AmZug = false;
                } else if (!spieler1AmZug) {
                    // Überprüfen, ob alle Schiffe des Spielers 2 versenkt wurden
                    if (schiffeGegner.TrueForAll (schiff => SchiffIstVersenkt (schiff, spielfeldGegner))) {
                        Console.WriteLine ("Herzlichen Glückwunsch! Spieler 1 hat alle Schiffe von Spieler 2 versenkt! Spieler 1 gewinnt!");
                        break;
                    }

                    // Spieler 2 schießt
                    isPlayerOne = false;
                    Console.WriteLine ("Spieler 2:");
                    ZeigeGegnerSpielfeld (spielfeldSpieler, isPlayerOne);
                    Console.WriteLine ("Spieler 2, geben Sie die Koordinaten für Ihren Schuss ein (z.B. A3):");
                    string eingabe2 = Console.ReadLine().ToUpper();
                    x = eingabe2[0] - 'A';
                    y = int.Parse (eingabe2.Substring (1)) - 1;

                    if (x < 0 || x >= SpielfeldGroesse || y < 0 || y >= SpielfeldGroesse) {
                        Console.WriteLine ("Ungültige Koordinaten! Bitte erneut eingeben.");
                        continue;
                    }

                    if (spielfeldSpieler[x, y] == ZellenStatus.Treffer || spielfeldSpieler[x, y] == ZellenStatus.Versenkt) {
                        Console.WriteLine ("Bereits geschossen! Bitte erneut eingeben.");
                        continue;
                    }

                    if (spielfeldSpieler[x, y] == ZellenStatus.Schiff) {
                        Console.WriteLine ("Treffer!");
                        spielfeldSpieler[x, y] = ZellenStatus.Treffer;
                        Schiff getroffenesSchiff = FindeGetroffenesSchiff(x, y, schiffeSpieler);
                        if (getroffenesSchiff.IstVersenkt (spielfeldSpieler)) {
                            Console.WriteLine ("Schiff versenkt!");
                            MarkiereVersenkt (getroffenesSchiff, spielfeldSpieler);
                        }
                        continue;
                    } else {
                        Console.WriteLine ("Kein Treffer.");
                        spielfeldSpieler[x, y] = ZellenStatus.Verfehlt;
                        spieler1AmZug = true;
                    }

                    // Überprüfen, ob alle Schiffe des Spielers 1 versenkt wurden
                    if (schiffeSpieler.TrueForAll (schiff => SchiffIstVersenkt (schiff, spielfeldSpieler))) {
                        Console.WriteLine ("Herzlichen Glückwunsch! Spieler 2 hat alle Schiffe von Spieler 1 versenkt! Spieler 2 gewinnt!");
                        break;
                    }
                }
            }
        }

        private bool SchiffIstVersenkt (Schiff schiff, ZellenStatus[,] spielfeld)
        {
            foreach (var position in schiff.Positionen) {
                if (spielfeld[position[0], position[1]] != ZellenStatus.Treffer) {
                    return false;
                }
            }
            return true;
        }

        private Schiff FindeGetroffenesSchiff (int x, int y, List<Schiff> schiffe)
        {
            foreach (var schiff in schiffe) {
                foreach (var position in schiff.Positionen) {
                    if (position[0] == x && position[1] == y) {
                        return schiff;
                    }
                }
            }
            return null;
        }
    }
}
