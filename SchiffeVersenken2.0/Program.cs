using System;
using System.Collections.Generic;

namespace SchiffeVersenken {
    class Program {
        static void Main (string[] args)
        {
            Console.WriteLine ("Willkommen bei Schiffe versenken!");
            Console.WriteLine ("Wollen Sie gegen einen Computer (C) oder einen zweiten Spieler (S) spielen?");
            char auswahl = Console.ReadLine().ToUpper()[0];

            Spiel spiel;
            if (auswahl == 'C') {
                spiel = new EinzelspielerSpiel ();
            } else if (auswahl == 'S') {
                spiel = new MehrspielerSpiel ();
            } else {
                Console.WriteLine ("Ungültige Auswahl. Das Spiel wird beendet.");
                return;
            }

            spiel.Start ();

            Console.WriteLine ("Vielen Dank fürs Spielen!");
        }
    }

    enum ZellenStatus {
        Leer,
        Schiff,
        Treffer,
        Versenkt,
        Verfehlt
    }
}
