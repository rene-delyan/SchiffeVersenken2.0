using SchiffeVersenken;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchiffeVersenken {
    internal class EinzelspielerSpiel : Spiel {
        public override void Start ()
        {
            Console.WriteLine ("Du spielst gegen den Computer!");
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
                int x = 0;
                int y = 0;
                bool isPlayerOne = true;
                if (spieler1AmZug) {
                    // Spieler schießt
                    ZeigeGegnerSpielfeld (spielfeldGegner, isPlayerOne);
                    Console.WriteLine ("Geben Sie die Koordinaten für Ihren Schuss ein (z.B. A3):");
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
                    // Überprüfen, ob alle Schiffe des Gegners versenkt wurden
                    if (schiffeGegner.TrueForAll (schiff => SchiffIstVersenkt (schiff, spielfeldGegner))) {
                        Console.WriteLine ("Herzlichen Glückwunsch! Du hast alle Schiffe des Gegners versenkt!");
                        break;
                    }
                    isPlayerOne = false;

                    ZeigeGegnerSpielfeld (spielfeldSpieler, isPlayerOne);
                    // Computer schießt
                    x = zufallsgenerator.Next (0, SpielfeldGroesse);
                    y = zufallsgenerator.Next (0, SpielfeldGroesse);
                    Console.WriteLine ($"Der Computer schießt auf {Convert.ToChar ('A' + x)}{y + 1}...");
                    if (spielfeldSpieler[x, y] == ZellenStatus.Schiff) {
                        Console.WriteLine ("Treffer!");
                        spielfeldSpieler[x, y] = ZellenStatus.Treffer;
                        Schiff getroffenesSchiff = FindeGetroffenesSchiff(x, y, schiffeGegner);
                        if (getroffenesSchiff.IstVersenkt (spielfeldGegner)) {
                            Console.WriteLine ("Schiff versenkt!");
                            MarkiereVersenkt (getroffenesSchiff, spielfeldSpieler);
                        }
                        continue;
                    } else {
                        Console.WriteLine ("Kein Treffer.");
                        spielfeldSpieler[x, y] = ZellenStatus.Verfehlt;
                        spieler1AmZug = true;
                    }

                    // Überprüfen, ob alle eigenen Schiffe versenkt wurden
                    if (schiffeSpieler.TrueForAll (schiff => SchiffIstVersenkt (schiff, spielfeldSpieler))) {
                        Console.WriteLine ("Der Computer hat alle deine Schiffe versenkt! Du hast verloren.");
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
