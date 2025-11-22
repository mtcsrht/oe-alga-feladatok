using System;

namespace OE.ALGA.Adatszerkezetek
{
    // 10. heti labor feladat - Tesztek: 10_SulyozatlanGrafTesztek.cs

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
            if (obj is null)
                return 1;

            if (obj is not EgeszGrafEl other)
                throw new ArgumentException("Object nem EgeszGrafEl", nameof(obj));
            
            int honnanComparison = Honnan.CompareTo(other.Honnan);
    
            if (honnanComparison != 0)
            {
                return honnanComparison;
            }
            
            int hovaComparison = Hova.CompareTo(other.Hova);
    
            if (hovaComparison != 0)
            {
                return hovaComparison;
            }
            return 0;
        }
    }
    public class CsucsmatrixSulyozatlanEgeszGraf : SulyozatlanGraf<int, EgeszGrafEl>
    {
        private int n;

        private bool[,] M;

        public CsucsmatrixSulyozatlanEgeszGraf(int n)
        {
            this.n = n;
            this.M = new bool[n, n];
        }

        public int CsucsokSzama => n;

        public int ElekSzama
        {
            get
            {
                int db = 0;
                for (int i = 0; i < n; i++)
                    for (int j =  0; j < n; j++)
                        if (M[i, j])
                            db++;
                
                return db;
            }
        }

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

        public bool VezetEl(int honnan, int hova)
        {
            if (honnan >= 0 && honnan < n && hova >= 0 && hova < n)
                return M[honnan, hova];
            return false;
        }

        public Halmaz<int> Szomszedai(int csucs)
        {
            FaHalmaz<int> szomszedok = new FaHalmaz<int>();
            
            if (csucs >= 0 && csucs < n)
                for (int j = 0; j < n; j++)
                    if (M[csucs, j])
                        szomszedok.Beszur(j);
            
            return szomszedok;
        }

        public void UjEl(int honnan, int hova)
        {
            if (honnan >= 0 && honnan < n && hova >= 0 && hova < n)
                M[honnan, hova] = true;
        }
    }

    public static class GrafBejarasok
    {
        public static Halmaz<V> SzelessegiBejaras<V, E>(Graf<V, E> g, V start, Action<V> muvelet)
            where V : IComparable 
            where E : GrafEl<V>
        {
            FaHalmaz<V> F = new FaHalmaz<V>();

            Sor<V> S = new LancoltSor<V>();
            
            S.Sorba(start);
            
            F.Beszur(start);

            while (!S.Ures)
            {
                V k = S.Sorbol();
                muvelet(k);
                
                Halmaz<V> szomszedok = g.Szomszedai(k);
                
                szomszedok.Bejar(x => 
                {
                    if (!F.Eleme(x)) 
                    {
                        S.Sorba(x);
                        F.Beszur(x);
                    }
                });
            }

            return F;
        }

        public static Halmaz<V> MelysegiBejaras<V, E>(Graf<V, E> g, V start, Action<V> muvelet)
         where V : IComparable
            where E : GrafEl<V>
        {
            FaHalmaz<V> F = new FaHalmaz<V>();
            MelysegBejarasRekurzio(g, start, F, muvelet);
            return F;
        }
        private static void MelysegBejarasRekurzio<V, E>(Graf<V, E> g, V k, Halmaz<V> F, Action<V> muvelet)
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