using System;
using System.Security.Cryptography.X509Certificates;

namespace OE.ALGA.Adatszerkezetek
{
    // 12. heti labor feladat - Tesztek: 12_SulyozottGrafTesztek.cs
    public class SulyozottEgeszGrafEl : EgeszGrafEl, SulyozottGrafEl<int>, IComparable
    {
        public int CompareTo(object? other)
        {
            if (other == null) return 1;
            var masikEl = other as SulyozottEgeszGrafEl;
            if (masikEl == null) return 1;
            
            int weightComparison = this.Suly.CompareTo(masikEl.Suly);
            
            if (weightComparison != 0) return weightComparison;
            
            int honnanComparison = this.Honnan.CompareTo(masikEl.Honnan);
            if (honnanComparison != 0) return honnanComparison;
            
            return masikEl.Hova.CompareTo(masikEl.Hova);
        }

        public SulyozottEgeszGrafEl(int honnan, int hova, float suly) : base(honnan, hova)
        {
            Suly = suly;
        }

        public float Suly { get; }
    }

    public class CsucsmatrixSulyozottEgeszGraf : SulyozottGraf<int, SulyozottEgeszGrafEl>
    {
        private int n;
        private float[,] M;
        public int CsucsokSzama => n;

        public int ElekSzama
        {
            get
            {
                int szamlalo = 0;
                
                for (int i = 0; i < M.GetLength(0); i++)
                    for (int j = 0; j < M.GetLength(1); j++)
                        if (!float.IsNaN(M[i, j]))
                            szamlalo++;
                
                return szamlalo;
            }
        }

        public Halmaz<int> Csucsok
        {
            get
            {
                Halmaz<int> halmaz = new FaHalmaz<int>();
                
                for (int i = 0; i < n; i++) 
                    halmaz.Beszur(i);
                
                return halmaz;
            }
        }

        public Halmaz<SulyozottEgeszGrafEl> Elek
        {
            get
            {
                Halmaz<SulyozottEgeszGrafEl> halmaz = new FaHalmaz<SulyozottEgeszGrafEl>();
                for (int i = 0; i < M.GetLength(0); i++)
                {
                    for (int j = 0; j < M.GetLength(1); j++)
                    {
                        if (!float.IsNaN(M[i, j]))
                        {
                            SulyozottEgeszGrafEl s = new(i, j, M[i, j]);
                            halmaz.Beszur(s);
                        }
                    }
                }
                return halmaz;
            }
        }

        public CsucsmatrixSulyozottEgeszGraf(int n)
        {
            this.n = n;
            M = new float[n, n];
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                    M[i, j] = float.NaN;
            }
        }
        
        public bool VezetEl(int honnan, int hova)
        {
            return !float.IsNaN(M[honnan, hova]);
        }

        public Halmaz<int> Szomszedai(int csucs)
        {
            Halmaz<int> szomszedok = new FaHalmaz<int>();
            for (int i = 0; i < n; i++)
                if (!float.IsNaN(M[csucs, i]))
                    szomszedok.Beszur(i);
            
            return szomszedok;
        }

        public void UjEl(int honnan, int hova, float suly)
        {
            M[honnan, hova] = suly;
        }

        public float Suly(int honnan, int hova)
        {
            if (float.IsNaN(M[honnan, hova])) throw new NincsElKivetel();
            return M[honnan, hova];
        }
    }

    public class Utkereses
    {
        public static Szotar<V, float> Dijkstra<V, E>(SulyozottGraf<V, E> g, V start)
        {
            Szotar<V, V> PPP = new HasitoSzotarTulcsordulasiTerulettel<V, V>(g.CsucsokSzama);
            Szotar<V, float> LLL = new HasitoSzotarTulcsordulasiTerulettel<V, float>(g.CsucsokSzama);
            PrioritasosSor<V> SSS =
                new KupacPrioritasosSor<V>(g.CsucsokSzama, (v1, v2) => LLL.Kiolvas(v1) < LLL.Kiolvas(v2));
            
            g.Csucsok.Bejar(cs =>
            {
                LLL.Beir(cs, float.MaxValue);
                SSS.Sorba(cs);
            });
            
            LLL.Beir(start, 0);
            SSS.Frissit(start);

            while (!SSS.Ures)
            {
                V u = SSS.Sorbol();
                
                g.Szomszedai(u).Bejar(cs =>
                {
                    if (LLL.Kiolvas(u) + g.Suly(u, cs) < LLL.Kiolvas(cs))
                    {
                        LLL.Beir(cs, LLL.Kiolvas(u) + g.Suly(u, cs));
                    }
                });
                
            }

            return LLL;
        }
        
        
    }
    
    public class FeszitofaKereses
    {
        public static Halmaz<E> Kruskal<V, E>(SulyozottGraf<V, E> g) where E : SulyozottGrafEl<V>, IComparable where V : IComparable
        {
            Szotar<V, int> vhalmaz = new HasitoSzotarTulcsordulasiTerulettel<V, int>(g.CsucsokSzama);
            PrioritasosSor<E> SSS = new KupacPrioritasosSor<E>(g.ElekSzama, (e1, e2) => e1.CompareTo(e2) == -1);
            Halmaz<E> AAA = new FaHalmaz<E>();
            
            

            int i = 0;

            g.Csucsok.Bejar(cs => { vhalmaz.Beir(cs,i++); });
            g.Elek.Bejar(e => SSS.Sorba(e));
            
            while (!SSS.Ures)
            {
                
                E e = SSS.Sorbol();
                i = 0;

                int eFromTemp = vhalmaz.Kiolvas(e.Honnan);
                int eToTemp = vhalmaz.Kiolvas(e.Hova);
                
                if (eFromTemp != eToTemp)
                {
                    g.Csucsok.Bejar(cs =>
                    {
                        int csTemp = vhalmaz.Kiolvas(cs);
                        
                        if(csTemp == eToTemp)
                        {
                            
                            vhalmaz.Beir(cs, eFromTemp);
                        }
                    });
                    AAA.Beszur(e);
                }

            }


            return AAA;
        }
        
        public static Szotar<V,V> Prim<V,E>(SulyozottGraf<V,E> g, V start) where V : IComparable
        {
            Szotar<V,V> PPP = new HasitoSzotarTulcsordulasiTerulettel<V,V>(g.CsucsokSzama);
            Halmaz<V> WWW = new FaHalmaz<V>();
            Szotar<V,float> KKK = new HasitoSzotarTulcsordulasiTerulettel<V,float>(g.CsucsokSzama);
            
            V c = start;
            KupacPrioritasosSor<V> S = new KupacPrioritasosSor<V>(g.CsucsokSzama, (g1, g2) =>
            {
                if (g1.CompareTo(c) == 0)
                    return true;
                if (g2.CompareTo(c) == 0)
                    return false;

                if (g.VezetEl(c, g1))
                    if (g.VezetEl(c, g2))
                        return g.Suly(c, g1) <= g.Suly(c, g2);
                    else
                        return true;
                else
                if (g.VezetEl(c, g2))
                    return false;
                return true;
            });

            g.Csucsok.Bejar(cs =>
            {
                KKK.Beir(cs, float.MaxValue);
                S.Sorba(cs);
                WWW.Beszur(cs);
            });

            S.Frissit(start);
            KKK.Beir(start,0);
            PPP.Beir(start, start);
            while (!S.Ures)
            {
                c = S.Sorbol();
                WWW.Torol(c);

                g.Szomszedai(c).Bejar(cs =>
                {
                    if(WWW.Eleme(cs) && g.Suly(c,cs) < KKK.Kiolvas(cs)){
                        KKK.Beir(cs, g.Suly(c, cs));
                        PPP.Beir(cs, c);
                    }
                });
            }

            return PPP;
        }

    }
}
