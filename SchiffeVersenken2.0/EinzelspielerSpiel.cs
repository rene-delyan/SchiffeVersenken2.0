using SchiffeVersenken;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Media;
using SchiffeVersenken2._0;

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
            int letzterTrefferX = -1;
            int letzterTrefferY = -1;
            Sounds sounds = new Sounds ();

            List<(int, int)> getroffeneSchuesse  = new List<(int, int)>();

            while (true) {
                int x = 0;
                int y = 0;

                bool isPlayerOne = true;
                if (spieler1AmZug) {
                    // Spieler schießt
                    ZeigeGegnerSpielfeld (spielfeldGegner, isPlayerOne);
                    Console.WriteLine ("Geben Sie die Koordinaten für Ihren Schuss ein (z.B. A3):");
                    bool test = true;
                    while (test) {
                        try {
                            string eingabe = Console.ReadLine ().ToUpper ();
                            x = eingabe[0] - 'A';
                            y = int.Parse (eingabe.Substring (1)) - 1;
                            test = false;
                        } catch {
                            Console.WriteLine ("Ungültige Eingabe.");
                            Console.WriteLine ("Geben Sie die Koordinaten für Ihren Schuss ein (z.B. A3):");                           
                        }
                    }

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
                        sounds.PlayKaboom ();
                        spielfeldGegner[x, y] = ZellenStatus.Treffer;
                        Schiff getroffenesSchiff = FindeGetroffenesSchiff(x, y, schiffeGegner);
                        if (getroffenesSchiff.IstVersenkt (spielfeldGegner)) {
                            Console.WriteLine ("Schiff versenkt!");
                            MarkiereVersenkt (getroffenesSchiff, spielfeldGegner);
                        }
                        ClearConsole ();
                        continue;
                    } else {
                        Console.WriteLine ("Kein Treffer.");
                        sounds.PlaySploosh ();
                        spielfeldGegner[x, y] = ZellenStatus.Verfehlt;
                    }
                    ClearConsole ();
                    spieler1AmZug = false;
                } else if (!spieler1AmZug) {
                    Console.ForegroundColor = ConsoleColor.Red;
                    // Überprüfen, ob alle Schiffe des Gegners versenkt wurden
                    if (schiffeGegner.TrueForAll (schiff => SchiffIstVersenkt (schiff, spielfeldGegner))) {
                        Console.WriteLine ("Herzlichen Glückwunsch! Du hast alle Schiffe des Gegners versenkt!");
                        break;
                    }
                    isPlayerOne = false;

                    ZeigeGegnerSpielfeld (spielfeldSpieler, isPlayerOne);
                    // Computer schießt
                    if (letzterTrefferX != -1 && letzterTrefferY != -1 || getroffeneSchuesse.Count > 1) {
                        // Schieße in der Nähe des letzten Treffers
                        (x, y) = IntelligenterSchuss (letzterTrefferX, letzterTrefferY, spielfeldSpieler, getroffeneSchuesse);

                    } else {
                        // Zufälliger Schuss
                        (x, y) = ZufaelligerSchuss (spielfeldSpieler);
                    }
                    Console.WriteLine ($"Der Computer schießt auf {Convert.ToChar ('A' + x)}{y + 1}...");
                    if (spielfeldSpieler[x, y] == ZellenStatus.Schiff) {
                        Console.WriteLine ("Treffer!");
                        getroffeneSchuesse.Add ((x,y));
                        spielfeldSpieler[x, y] = ZellenStatus.Treffer;
                        Schiff getroffenesSchiff = FindeGetroffenesSchiff(x, y, schiffeSpieler);
                        if (getroffenesSchiff.IstVersenkt (spielfeldSpieler)) {
                            Console.WriteLine ("Schiff versenkt!");
                            getroffeneSchuesse.Clear ();
                            MarkiereVersenkt (getroffenesSchiff, spielfeldSpieler);
                            letzterTrefferX = -1;
                            letzterTrefferY = -1;
                        } else {
                            letzterTrefferX = x;
                            letzterTrefferY = y;
                        }
                        ClearConsole ();
                        continue;
                    } else {
                        Console.WriteLine ("Kein Treffer.");
                        spielfeldSpieler[x, y] = ZellenStatus.Verfehlt;
                        spieler1AmZug = true;
                        //letzterTrefferX = -1;
                        //letzterTrefferY = -1;
                    }

                    // Überprüfen, ob alle eigenen Schiffe versenkt wurden
                    if (schiffeSpieler.TrueForAll (schiff => SchiffIstVersenkt (schiff, spielfeldSpieler))) {
                        Console.WriteLine ("Der Computer hat alle deine Schiffe versenkt! Du hast verloren.");
                        break;
                    }
                    Console.WriteLine ("Es wird auf eine Aktion gewartet.");
                    Console.ReadKey ();
                    ClearConsole ();
                    Console.ForegroundColor = ConsoleColor.White;
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

        private (int, int) IntelligenterSchuss (int x, int y, ZellenStatus[,] spielfeld, List<(int, int)> getroffeneSchuesse)
        {
            // Schieße in der Nähe des letzten Treffers
            List<(int, int)> moeglicheSchuesse  = new List<(int, int)>();

            if (getroffeneSchuesse.Count == 1)
            {
                // Schieße horizontal
                if (x - 1 >= 0 && spielfeld[x - 1, y] == ZellenStatus.Unbekannt || x - 1 >= 0 && spielfeld[x - 1, y] == ZellenStatus.Schiff)
                    moeglicheSchuesse.Add ((x - 1, y));
                if (x + 1 < SpielfeldGroesse && spielfeld[x + 1, y] == ZellenStatus.Unbekannt || x + 1 < SpielfeldGroesse && spielfeld[x + 1, y] == ZellenStatus.Schiff)
                    moeglicheSchuesse.Add ((x + 1, y));

                // Schieße vertikal
                if (y - 1 >= 0 && spielfeld[x, y - 1] == ZellenStatus.Unbekannt || y - 1 >= 0 && spielfeld[x, y - 1] == ZellenStatus.Schiff)
                    moeglicheSchuesse.Add ((x, y - 1));
                if (y + 1 < SpielfeldGroesse && spielfeld[x, y + 1] == ZellenStatus.Unbekannt || y + 1 < SpielfeldGroesse && spielfeld[x, y + 1] == ZellenStatus.Schiff)
                    moeglicheSchuesse.Add ((x, y + 1));
            }
            else if (getroffeneSchuesse.Count >= 2) {
                bool horizontal = false;
                bool vertikal = false;
    
                // Überprüfe, ob die getroffenen Schüsse horizontal oder vertikal angeordnet sind
                if (getroffeneSchuesse[0].Item1 == getroffeneSchuesse[1].Item1)
                {
                    horizontal = true;
                }
                else if (getroffeneSchuesse[0].Item2 == getroffeneSchuesse[1].Item2)
                {
                    vertikal = true;
                }
    
                if (horizontal)
                {
                    // Sortiere die getroffenen Schüsse nach ihrer y-Koordinate
                    getroffeneSchuesse.Sort((a, b) => a.Item2.CompareTo(b.Item2));
        
                    // Bestimme die Richtung und erweitere die möglichen Schüsse entsprechend
                    if (y - 1 >= 0 && spielfeld[x, y - 1] == ZellenStatus.Unbekannt || y - 1 >= 0 && spielfeld[x, y - 1] == ZellenStatus.Schiff)
                        moeglicheSchuesse.Add((x, y - 1));
                    if (y + 1 < SpielfeldGroesse && spielfeld[x, y + 1] == ZellenStatus.Unbekannt || y + 1 < SpielfeldGroesse && spielfeld[x, y + 1] == ZellenStatus.Schiff)
                        moeglicheSchuesse.Add((x, y + 1));
                }
                else if (vertikal)
                {
                    // Sortiere die getroffenen Schüsse nach ihrer x-Koordinate
                    getroffeneSchuesse.Sort((a, b) => a.Item1.CompareTo(b.Item1));
        
                    // Bestimme die Richtung und erweitere die möglichen Schüsse entsprechend
                    if (x - 1 >= 0 && spielfeld[x - 1, y] == ZellenStatus.Unbekannt || x - 1 >= 0 && spielfeld[x - 1, y] == ZellenStatus.Schiff)
                        moeglicheSchuesse.Add((x - 1, y));
                    if (x + 1 < SpielfeldGroesse && spielfeld[x + 1, y] == ZellenStatus.Unbekannt || x + 1 < SpielfeldGroesse && spielfeld[x + 1, y] == ZellenStatus.Schiff)
                        moeglicheSchuesse.Add((x + 1, y));
                }
            }

            // Entferne Felder, die direkt neben einem bereits versenkten Schiff liegen
            moeglicheSchuesse.RemoveAll ((coord) =>
            {
                int i = coord.Item1;
                int j = coord.Item2;
                return (i > 0 && spielfeld[i - 1, j] == ZellenStatus.Versenkt) ||
                       (i < SpielfeldGroesse - 1 && spielfeld[i + 1, j] == ZellenStatus.Versenkt) ||
                       (j > 0 && spielfeld[i, j - 1] == ZellenStatus.Versenkt) ||
                       (j < SpielfeldGroesse - 1 && spielfeld[i, j + 1] == ZellenStatus.Versenkt);
            });

            // Zufällig einen der verbleibenden möglichen Schüsse auswählen
            if (moeglicheSchuesse.Count > 0) {
                int index = zufallsgenerator.Next(moeglicheSchuesse.Count);
                return moeglicheSchuesse[index];
            } else {
                // Falls keine gültigen Schüsse möglich sind, schieße zufällig
                return ZufaelligerSchuss (spielfeld);
            }
        }

        private (int, int) ZufaelligerSchuss (ZellenStatus[,] spielfeld)
        {
            // Zufälliger Schuss auf ein unbekanntes Feld
            int x, y;
            do {
                x = zufallsgenerator.Next (0, SpielfeldGroesse);
                y = zufallsgenerator.Next (0, SpielfeldGroesse);
            } while (spielfeld[x, y] != ZellenStatus.Unbekannt && spielfeld[x, y] != ZellenStatus.Schiff);

            return (x, y);
        }
    }
}
