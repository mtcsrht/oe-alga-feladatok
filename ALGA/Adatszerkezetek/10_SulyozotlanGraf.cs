using System;

namespace OE.ALGA.Adatszerkezetek
{
    // 10. heti labor feladat - Tesztek: 10_SulyozatlanGrafTesztek.cs

    /// <summary>
    /// Gráf élének típusa egész csúcsokkal.
    /// A GrafEl<V> interfésznek megfelelően két végpontot tart fenn:
    ///   - Honnan: start vertex
    ///   - Hova:    end vertex
    ///
    /// CompareTo: rendezhetőség érdekében (halmazokban használjuk).
    /// </summary>
    public class EgeszGrafEl : GrafEl<int>, IComparable
    {
        public EgeszGrafEl(int honnan, int hova)
        {
            Honnan = honnan;
            Hova = hova;
        }

        public int Honnan { get; }
        public int Hova { get; }

        public int CompareTo(object? obj)
        {
            // Null mindig "kisebb rangú"
            if (obj is null)
                return 1;

            if (obj is not EgeszGrafEl other)
                throw new ArgumentException("Object nem EgeszGrafEl", nameof(obj));

            // Először a Honnan mező dönt
            int honnanComparison = Honnan.CompareTo(other.Honnan);
            if (honnanComparison != 0)
                return honnanComparison;

            // Ha ugyanonnan indulnak → Hova dönt
            int hovaComparison = Hova.CompareTo(other.Hova);
            if (hovaComparison != 0)
                return hovaComparison;

            return 0; // teljesen azonos él
        }
    }

    // ───────────────────────────────────────────────────────────────
    // GRÁF CSÚCSMÁTRIXOS REPREZENTÁCIÓJA (irányított, súlyozatlan)
    // ───────────────────────────────────────────────────────────────
    public class CsucsmatrixSulyozatlanEgeszGraf : SulyozatlanGraf<int, EgeszGrafEl>
    {
        private int n;          // csúcsok száma
        private bool[,] M;      // M[i, j] == true → él megy i → j között

        /// <summary>
        /// Létrehozza az NxN mátrixot, minden él alapértelmezésben false.
        /// </summary>
        public CsucsmatrixSulyozatlanEgeszGraf(int n)
        {
            this.n = n;
            this.M = new bool[n, n];
        }

        public int CsucsokSzama => n;

        /// <summary>
        /// Minden M[i,j] pozíciót átnézünk és számoljuk, ahol true van.
        /// A reprezentáció irányított → minden él külön cella!
        /// </summary>
        public int ElekSzama
        {
            get
            {
                int db = 0;
                for (int i = 0; i < n; i++)
                    for (int j = 0; j < n; j++)
                        if (M[i, j])
                            db++;
                return db;
            }
        }

        /// <summary>
        /// A csúcsok halmaza mindig {0, 1, 2, ..., n-1}.
        /// Bináris keresőfában adjuk vissza.
        /// </summary>
        public Halmaz<int> Csucsok
        {
            get
            {
                FaHalmaz<int> h = new FaHalmaz<int>();
                for (int i = 0; i < n; i++)
                    h.Beszur(i);
                return h;
            }
        }

        /// <summary>
        /// Az élek halmaza.
        /// Végigmegyünk a mátrixon és minden true esetében beszúrunk egy EgeszGrafEl-t.
        /// </summary>
        public Halmaz<EgeszGrafEl> Elek
        {
            get
            {
                FaHalmaz<EgeszGrafEl> h = new FaHalmaz<EgeszGrafEl>();

                for (int i = 0; i < n; i++)
                    for (int j = 0; j < n; j++)
                        if (M[i, j])
                            h.Beszur(new EgeszGrafEl(i, j));

                return h;
            }
        }

        /// <summary>
        /// Vezet-e él honnan → hova irányban?
        /// Biztonsági határellenőrzés után a mátrixból olvasunk.
        /// </summary>
        public bool VezetEl(int honnan, int hova)
        {
            if (honnan >= 0 && honnan < n && hova >= 0 && hova < n)
                return M[honnan, hova];
            return false;
        }

        /// <summary>
        /// Egy csúcs szomszédai:
        ///   Az összes j csúcs, amelyre M[csucs, j] == true.
        /// </summary>
        public Halmaz<int> Szomszedai(int csucs)
        {
            FaHalmaz<int> szomszedok = new FaHalmaz<int>();

            if (csucs >= 0 && csucs < n)
                for (int j = 0; j < n; j++)
                    if (M[csucs, j])
                        szomszedok.Beszur(j);

            return szomszedok;
        }

        /// <summary>
        /// Új él hozzáadása: M[honnan, hova] = true.
        /// Nem vizsgálja, hogy korábban már volt-e ilyen él.
        /// </summary>
        public void UjEl(int honnan, int hova)
        {
            if (honnan >= 0 && honnan < n && hova >= 0 && hova < n)
                M[honnan, hova] = true;
        }
    }

    // ───────────────────────────────────────────────────────────────
    // GRÁF BEJÁRÁSOK: BFS (szélességi) és DFS (mélységi)
    // ───────────────────────────────────────────────────────────────
    public static class GrafBejarasok
    {
        /// <summary>
        /// Szélességi bejárás (Breadth-First Search).
        ///
        /// Lényeg:
        ///   - Kezdő csúcsot betesszük a sorba.
        ///   - Végigvesszük szintenként a csúcsokat.
        ///   - F visszatartja, hogy mely csúcsokat láttunk már.
        /// </summary>
        public static Halmaz<V> SzelessegiBejaras<V, E>(
            Graf<V, E> g,
            V start,
            Action<V> muvelet)
            where V : IComparable
            where E : GrafEl<V>
        {
            FaHalmaz<V> F = new FaHalmaz<V>();      // Látogatott csúcsok halmaza
            Sor<V> S = new LancoltSor<V>();         // A BFS klasszikus sora

            S.Sorba(start);
            F.Beszur(start);

            while (!S.Ures)
            {
                V k = S.Sorbol();
                muvelet(k);                         // Feldolgozás

                Halmaz<V> szomszedok = g.Szomszedai(k);

                // Minden szomszédot megnézünk
                szomszedok.Bejar(x =>
                {
                    if (!F.Eleme(x))                // Ha még nem jártunk ott
                    {
                        S.Sorba(x);                 // Bepakoljuk a sorba
                        F.Beszur(x);                // Megjegyezzük, hogy már láttuk
                    }
                });
            }

            return F;
        }

        /// <summary>
        /// Mélységi bejárás (Depth-First Search). Külső függvény.
        /// Az igazi munka a rekurzív segédfüggvényben történik.
        /// </summary>
        public static Halmaz<V> MelysegiBejaras<V, E>(
            Graf<V, E> g,
            V start,
            Action<V> muvelet)
            where V : IComparable
            where E : GrafEl<V>
        {
            FaHalmaz<V> F = new FaHalmaz<V>();  // Látogatott csúcsok
            MelysegBejarasRekurzio(g, start, F, muvelet);
            return F;
        }

        /// <summary>
        /// A DFS rekurzív magja.
        ///
        /// Lépések:
        ///   - Megjelöljük a csúcsot látogatottnak.
        ///   - Végrehajtjuk rajta a műveletet.
        ///   - Minden szomszédot rekurzívan bejárunk, ha még nem jártunk ott.
        /// </summary>
        private static void MelysegBejarasRekurzio<V, E>(
            Graf<V, E> g,
            V k,
            Halmaz<V> F,
            Action<V> muvelet)
            where V : IComparable
            where E : GrafEl<V>
        {
            F.Beszur(k);
            muvelet(k);

            Halmaz<V> szomszedok = g.Szomszedai(k);

            szomszedok.Bejar(szomszed =>
            {
                if (!F.Eleme(szomszed))
                {
                    MelysegBejarasRekurzio<V, E>(g, szomszed, F, muvelet);
                }
            });
        }
    }
}
