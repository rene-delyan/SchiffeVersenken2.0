using System;
using System.Collections.Generic;

namespace SchiffeVersenken
{
    class Program
    {
        static void Main (string[] args)
        {
            Console.WriteLine("Willkommen bei Schiffe versenken!");
            Console.WriteLine("Wollen Sie gegen einen Computer (C) oder einen zweiten Spieler (S) spielen?");
            char auswahl = Console.ReadLine().ToUpper()[0];

            Spiel spiel;
            if (auswahl == 'C')
            {
                spiel = new EinzelspielerSpiel();
            }
            else if (auswahl == 'S')
            {
                spiel = new MehrspielerSpiel();
            }
            else
            {
                Console.WriteLine("Ungültige Auswahl. Das Spiel wird beendet.");
                return;
            }

            spiel.Start();

            Console.WriteLine("Vielen Dank fürs Spielen!");
        }
    }

    enum ZellenStatus
    {
        Leer,
        Schiff,
        Treffer,
        Versenkt,
        Verfehlt
    }

    abstract class Spiel
    {
        protected const int SpielfeldGroesse = 10;
        protected const int AnzahlSchiffe = 5;

        protected ZellenStatus[,] spielfeldSpieler;
        protected ZellenStatus[,] spielfeldGegner;
        protected List<Schiff> schiffeSpieler;
        protected List<Schiff> schiffeGegner;
        protected Random zufallsgenerator;

        public Spiel ()
        {
            spielfeldSpieler = new ZellenStatus[SpielfeldGroesse, SpielfeldGroesse];
            spielfeldGegner = new ZellenStatus[SpielfeldGroesse, SpielfeldGroesse];
            schiffeSpieler = new List<Schiff>();
            schiffeGegner = new List<Schiff>();
            zufallsgenerator = new Random();
        }
        public abstract void Start ();

        protected void InitialisiereSpielfeld (ZellenStatus[,] spielfeld)
        {
            for (int i = 0; i < SpielfeldGroesse; i++)
            {
                for (int j = 0; j < SpielfeldGroesse; j++)
                {
                    spielfeld[i, j] = ZellenStatus.Leer;
                }
            }
        }

        protected void PlatziereSchiffe (List<Schiff> schiffe, ZellenStatus[,] spielfeld)
        {
            for (int i = 0; i < AnzahlSchiffe; i++)
            {
                PlatzierenNeuesSchiff(i + 1, schiffe, spielfeld);
            }
        }

        protected void PlatzierenNeuesSchiff (int laenge, List<Schiff> schiffe, ZellenStatus[,] spielfeld)
        {
            Schiff neuesSchiff = new Schiff(laenge);

            bool platziert = false;
            while (!platziert)
            {
                int x = zufallsgenerator.Next(0, SpielfeldGroesse);
                int y = zufallsgenerator.Next(0, SpielfeldGroesse);
                bool horizontal = zufallsgenerator.Next(2) == 0;

                if (IstPlatzVerfuegbar(x, y, laenge, horizontal, schiffe))
                {
                    neuesSchiff.Platzieren(x, y, horizontal);
                    schiffe.Add(neuesSchiff);

                    // Setze Zellenstatus auf "Schiff" für jedes Feld des platzierten Schiffs
                    foreach (var position in neuesSchiff.Positionen)
                    {
                        spielfeld[position[0], position[1]] = ZellenStatus.Schiff;
                    }

                    platziert = true;
                }
            }
        }

        protected bool IstPlatzVerfuegbar (int startX, int startY, int laenge, bool horizontal, List<Schiff> schiffe)
        {
            // Überprüfe, ob das Schiff außerhalb des Spielfelds platziert wird
            if (horizontal && startX + laenge > SpielfeldGroesse)
                return false;
            if (!horizontal && startY + laenge > SpielfeldGroesse)
                return false;

            // Überprüfe, ob das Schiff überlappende Felder hat oder sich zu nahe an anderen Schiffen befindet
            for (int i = -1; i <= laenge; i++)
            {
                for (int j = -1; j <= 1; j++)
                {
                    int x = startX + (horizontal ? i : j);
                    int y = startY + (horizontal ? j : i);

                    if (x >= 0 && x < SpielfeldGroesse && y >= 0 && y < SpielfeldGroesse)
                    {
                        if (spielfeldSpieler[x, y] != ZellenStatus.Leer)
                            return false;
                    }
                }
            }
            return true;
        }

        protected abstract void Spielablauf ();

        protected void ZeigeEigenesSpielfeld ()
        {
            Console.WriteLine("Dein Spielfeld:");
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
                    switch (spielfeldSpieler[i, j])
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
                        case ZellenStatus.Verfehlt:
                            Console.Write("* ");
                            break;
                        default:
                            Console.Write(". ");
                            break;
                    }
                }
                Console.WriteLine();
            }
        }

        protected void ZeigeGegnerSpielfeld (ZellenStatus[,] spielfeldGegner, bool isPlayer)
        {
            if (isPlayer)
                Console.WriteLine("Gegnerisches Spielfeld:");
            else
                Console.WriteLine("Eigenes Spielfeld:");
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
                    switch (spielfeldGegner[i, j])
                    {
                        case ZellenStatus.Leer:
                            Console.Write(". ");
                            break;
                        case ZellenStatus.Treffer:
                            Console.Write("X ");
                            break;
                        case ZellenStatus.Versenkt:
                            Console.Write("# ");
                            break;
                        case ZellenStatus.Verfehlt:
                            Console.Write("* ");
                            break;
                        default:
                            Console.Write(". ");
                            break;
                    }
                }
                Console.WriteLine();
            }
        }
    }

    class EinzelspielerSpiel : Spiel
    {
        public override void Start ()
        {
            Console.WriteLine("Du spielst gegen den Computer!");
            InitialisiereSpielfeld(spielfeldSpieler);
            InitialisiereSpielfeld(spielfeldGegner);
            PlatziereSchiffe(schiffeSpieler, spielfeldSpieler);
            PlatziereSchiffe(schiffeGegner, spielfeldGegner);
            Spielablauf();
        }

        protected override void Spielablauf ()
        {
            while (true)
            {
                // Spieler schießt
                bool isPlayerOne = true;
                ZeigeGegnerSpielfeld(spielfeldGegner, isPlayerOne);
                Console.WriteLine("Geben Sie die Koordinaten für Ihren Schuss ein (z.B. A3):");
                string eingabe = Console.ReadLine().ToUpper();
                int x = eingabe[0] - 'A';
                int y = int.Parse(eingabe.Substring(1)) - 1;

                if (x < 0 || x >= SpielfeldGroesse || y < 0 || y >= SpielfeldGroesse)
                {
                    Console.WriteLine("Ungültige Koordinaten! Bitte erneut eingeben.");
                    continue;
                }

                if (spielfeldGegner[x, y] == ZellenStatus.Treffer || spielfeldGegner[x, y] == ZellenStatus.Versenkt)
                {
                    Console.WriteLine("Bereits geschossen! Bitte erneut eingeben.");
                    continue;
                }

                if (spielfeldGegner[x, y] == ZellenStatus.Schiff)
                {
                    Console.WriteLine("Treffer!");
                    spielfeldGegner[x, y] = ZellenStatus.Treffer;
                    Schiff getroffenesSchiff = FindeGetroffenesSchiff(x, y, schiffeGegner);
                    if (getroffenesSchiff.IstVersenkt(spielfeldGegner))
                    {
                        Console.WriteLine("Schiff versenkt!");
                    }
                }
                else
                {
                    Console.WriteLine("Kein Treffer.");
                    spielfeldGegner[x, y] = ZellenStatus.Verfehlt;
                }

                // Überprüfen, ob alle Schiffe des Gegners versenkt wurden
                if (schiffeGegner.TrueForAll(schiff => SchiffIstVersenkt(schiff, spielfeldGegner)))
                {
                    Console.WriteLine("Herzlichen Glückwunsch! Du hast alle Schiffe des Gegners versenkt!");
                    break;
                }
                isPlayerOne = false;

                ZeigeGegnerSpielfeld(spielfeldSpieler, isPlayerOne);
                // Computer schießt
                x = zufallsgenerator.Next(0, SpielfeldGroesse);
                y = zufallsgenerator.Next(0, SpielfeldGroesse);
                Console.WriteLine($"Der Computer schießt auf {Convert.ToChar('A' + x)}{y + 1}...");
                if (spielfeldSpieler[x, y] == ZellenStatus.Schiff)
                {
                    Console.WriteLine("Treffer!");
                    spielfeldSpieler[x, y] = ZellenStatus.Treffer;
                }
                else
                {
                    Console.WriteLine("Kein Treffer.");
                    spielfeldSpieler[x, y] = ZellenStatus.Verfehlt;
                }

                // Überprüfen, ob alle eigenen Schiffe versenkt wurden
                if (schiffeSpieler.TrueForAll(schiff => SchiffIstVersenkt(schiff, spielfeldSpieler)))
                {
                    Console.WriteLine("Der Computer hat alle deine Schiffe versenkt! Du hast verloren.");
                    break;
                }
            }
        }

        private bool SchiffIstVersenkt (Schiff schiff, ZellenStatus[,] spielfeld)
        {
            foreach (var position in schiff.Positionen)
            {
                if (spielfeld[position[0], position[1]] != ZellenStatus.Treffer)
                {
                    return false;
                }
            }
            return true;
        }

        private Schiff FindeGetroffenesSchiff (int x, int y, List<Schiff> schiffe)
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
    }

    class MehrspielerSpiel : Spiel
    {
        public override void Start ()
        {
            Console.WriteLine("Du spielst gegen einen zweiten Spieler!");
            Console.WriteLine("Spieler 1, platziere deine Schiffe:");
            InitialisiereSpielfeld(spielfeldSpieler);
            InitialisiereSpielfeld(spielfeldGegner);
            PlatziereSchiffe(schiffeSpieler, spielfeldSpieler);
            PlatziereSchiffe(schiffeGegner, spielfeldGegner);
            Spielablauf();
        }

        protected override void Spielablauf ()
        {
            while (true)
            {
                // Spieler 1 schießt
                Console.WriteLine("Spieler 1:");
                bool isPlayerOne = true;
                ZeigeGegnerSpielfeld(spielfeldGegner, isPlayerOne);
                Console.WriteLine("Spieler 1, geben Sie die Koordinaten für Ihren Schuss ein (z.B. A3):");
                string eingabe = Console.ReadLine().ToUpper();
                int x = eingabe[0] - 'A';
                int y = int.Parse(eingabe.Substring(1)) - 1;

                if (x < 0 || x >= SpielfeldGroesse || y < 0 || y >= SpielfeldGroesse)
                {
                    Console.WriteLine("Ungültige Koordinaten! Bitte erneut eingeben.");
                    continue;
                }

                if (spielfeldGegner[x, y] == ZellenStatus.Treffer || spielfeldGegner[x, y] == ZellenStatus.Versenkt)
                {
                    Console.WriteLine("Bereits geschossen! Bitte erneut eingeben.");
                    continue;
                }

                if (spielfeldGegner[x, y] == ZellenStatus.Schiff)
                {
                    Console.WriteLine("Treffer!");
                    spielfeldGegner[x, y] = ZellenStatus.Treffer;
                    Schiff getroffenesSchiff = FindeGetroffenesSchiff(x, y, schiffeGegner);
                    if (getroffenesSchiff.IstVersenkt(spielfeldGegner))
                    {
                        Console.WriteLine("Schiff versenkt!");
                    }
                }
                else
                {
                    Console.WriteLine("Kein Treffer.");
                    spielfeldGegner[x, y] = ZellenStatus.Verfehlt;
                }

                // Überprüfen, ob alle Schiffe des Spielers 2 versenkt wurden
                if (schiffeGegner.TrueForAll(schiff => SchiffIstVersenkt(schiff, spielfeldGegner)))
                {
                    Console.WriteLine("Herzlichen Glückwunsch! Spieler 1 hat alle Schiffe von Spieler 2 versenkt! Spieler 1 gewinnt!");
                    break;
                }

                // Spieler 2 schießt
                isPlayerOne = false;
                Console.WriteLine("Spieler 2:");
                ZeigeGegnerSpielfeld(spielfeldSpieler, isPlayerOne);
                Console.WriteLine("Spieler 2, geben Sie die Koordinaten für Ihren Schuss ein (z.B. A3):");
                eingabe = Console.ReadLine().ToUpper();
                x = eingabe[0] - 'A';
                y = int.Parse(eingabe.Substring(1)) - 1;

                if (x < 0 || x >= SpielfeldGroesse || y < 0 || y >= SpielfeldGroesse)
                {
                    Console.WriteLine("Ungültige Koordinaten! Bitte erneut eingeben.");
                    continue;
                }

                if (spielfeldSpieler[x, y] == ZellenStatus.Treffer || spielfeldSpieler[x, y] == ZellenStatus.Versenkt)
                {
                    Console.WriteLine("Bereits geschossen! Bitte erneut eingeben.");
                    continue;
                }

                if (spielfeldSpieler[x, y] == ZellenStatus.Schiff)
                {
                    Console.WriteLine("Treffer!");
                    spielfeldSpieler[x, y] = ZellenStatus.Treffer;
                    Schiff getroffenesSchiff = FindeGetroffenesSchiff(x, y, schiffeSpieler);
                    if (getroffenesSchiff.IstVersenkt(spielfeldSpieler))
                    {
                        Console.WriteLine("Schiff versenkt!");
                    }
                }
                else
                {
                    Console.WriteLine("Kein Treffer.");
                    spielfeldSpieler[x, y] = ZellenStatus.Verfehlt;
                }

                // Überprüfen, ob alle Schiffe des Spielers 1 versenkt wurden
                if (schiffeSpieler.TrueForAll(schiff => SchiffIstVersenkt(schiff, spielfeldSpieler)))
                {
                    Console.WriteLine("Herzlichen Glückwunsch! Spieler 2 hat alle Schiffe von Spieler 1 versenkt! Spieler 2 gewinnt!");
                    break;
                }
            }
        }

        private bool SchiffIstVersenkt (Schiff schiff, ZellenStatus[,] spielfeld)
        {
            foreach (var position in schiff.Positionen)
            {
                if (spielfeld[position[0], position[1]] != ZellenStatus.Treffer)
                {
                    return false;
                }
            }
            return true;
        }

        private Schiff FindeGetroffenesSchiff (int x, int y, List<Schiff> schiffe)
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
    }

    class Schiff
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
