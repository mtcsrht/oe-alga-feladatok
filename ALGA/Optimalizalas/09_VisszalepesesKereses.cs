using System;

namespace OE.ALGA.Optimalizalas
{
    public class VisszalepesesOptimalizacio<T>
    {
        protected int n;
        protected int[] M;
        protected T[,] R;
        protected Func<int, T, bool> ft;
        protected Func<int, T, T[], bool> fk;
        protected Func<T[], double> josag;
        
        public int LepesSzam { get; protected set; }
        
        public VisszalepesesOptimalizacio(
            int n,
            int[] M,
            T[,] R,
            Func<int, T, bool> ft,
            Func<int, T, T[], bool> fk,
            Func<T[], double> josag)
        {
            this.n = n;
            this.M = M;
            this.R = R;
            this.ft = ft;
            this.fk = fk;
            this.josag = josag;
            this.LepesSzam = 0;
        }
        
        protected void Backtrack(int szint, T[] E, ref bool van, ref T[] O)
        {
            if (szint == n)
            {
                LepesSzam++;
                double ertEk = josag(E);
                if (!van || ertEk > josag(O))
                {
                    van = true;
                    Array.Copy(E, O, n);
                }
            }
            else
            {
                for (int i = 0; i < M[szint]; i++)
                {
                    LepesSzam++;
                    T r = R[szint, i];
                    if (ft(szint, r) && fk(szint, r, E))
                    {
                        E[szint] = r;
                        Backtrack(szint + 1, E, ref van, ref O);
                    }
                }
            }
        }
        
        public T[] OptimalisMegoldas()
        {
            T[] E = new T[n];
            T[] O = new T[n];
            bool van = false;
            
            Backtrack(0, E, ref van, ref O);
            
            return van ? O : null;
        }
    }
    
    public class VisszalepesesHatizsakPakolas
    {
        protected HatizsakProblema problema;
        
        public int LepesSzam { get; protected set; }
        
        public VisszalepesesHatizsakPakolas(HatizsakProblema problema)
        {
            this.problema = problema;
        }
        
        public virtual bool[] OptimalisMegoldas()
        {
            int n = problema.n;
            
            int[] M = new int[n];
            for (int i = 0; i < n; i++)
            {
                M[i] = 2;
            }
            
            bool[,] R = new bool[n, 2];
            for (int i = 0; i < n; i++)
            {
                R[i, 0] = true;
                R[i, 1] = false;
            }
            
            Func<int, bool, bool> ft = (szint, r) => true;
            
            Func<int, bool, bool[], bool> fk = (szint, r, E) =>
            {
                
                int osszSuly = 0;
                for (int i = 0; i < szint; i++)
                {
                    if (E[i])
                    {
                        osszSuly += problema.w[i];
                    }
                }
                osszSuly += problema.w[szint];
                
                return osszSuly <= problema.Wmax;
            };
            
            Func<bool[], double> josag = (E) =>
            {
                double osszErtek = 0;
                for (int i = 0; i < n; i++)
                {
                    if (E[i])
                    {
                        osszErtek += problema.p[i];
                    }
                }
                return osszErtek;
            };
            
            var optimalizalo = new VisszalepesesOptimalizacio<bool>(
                n, M, R, ft, fk, josag);
            
            bool[] eredmeny = optimalizalo.OptimalisMegoldas();
            this.LepesSzam = optimalizalo.LepesSzam;
            
            return eredmeny;
        }
        
        public double OptimalisErtek()
        {
            bool[] megoldas = OptimalisMegoldas();
            if (megoldas == null) return 0;
            
            double osszErtek = 0;
            for (int i = 0; i < megoldas.Length; i++)
            {
                if (megoldas[i])
                {
                    osszErtek += problema.p[i];
                }
            }
            return osszErtek;
        }
    }
    
    public class SzetvalasztasEsKorlatozasOptimalizacio<T> : VisszalepesesOptimalizacio<T>
    {
        protected Func<int, T[], double> fb;
        
        public SzetvalasztasEsKorlatozasOptimalizacio(
            int n,
            int[] M,
            T[,] R,
            Func<int, T, bool> ft,
            Func<int, T, T[], bool> fk,
            Func<T[], double> josag,
            Func<int, T[], double> fb)
            : base(n, M, R, ft, fk, josag)
        {
            this.fb = fb;
        }
        
        protected new void Backtrack(int szint, T[] E, ref bool van, ref T[] O)
        {
            LepesSzam++;
            
            if (szint == n)
            {
                double ertEk = josag(E);
                if (!van || ertEk > josag(O))
                {
                    van = true;
                    Array.Copy(E, O, n);
                }
            }
            else
            {
                for (int i = 0; i < M[szint]; i++)
                {
                    T r = R[szint, i];
                    if (ft(szint, r) && fk(szint, r, E))
                    {
                        E[szint] = r;
                        
                        double aktualisErtek = josag(E);
                        double becsles = aktualisErtek + fb(szint + 1, E);
                        
                        if (!van || becsles > josag(O))
                        {
                            Backtrack(szint + 1, E, ref van, ref O);
                        }
                    }
                }
            }
        }
        
        public new T[] OptimalisMegoldas()
        {
            T[] E = new T[n];
            T[] O = new T[n];
            bool van = false;
            
            Backtrack(0, E, ref van, ref O);
            
            return van ? O : null;
        }
    }
    
    public class SzetvalasztasEsKorlatozasHatizsakPakolas : VisszalepesesHatizsakPakolas
    {
        public SzetvalasztasEsKorlatozasHatizsakPakolas(HatizsakProblema problema)
            : base(problema)
        {
        }
        
        public override bool[] OptimalisMegoldas()
        {
            int n = problema.n;
            
            int[] M = new int[n];
            for (int i = 0; i < n; i++)
            {
                M[i] = 2;
            }
            
            bool[,] R = new bool[n, 2];
            for (int i = 0; i < n; i++)
            {
                R[i, 0] = true;
                R[i, 1] = false;
            }
            

            Func<int, bool, bool> ft = (szint, r) => true;
            

            Func<int, bool, bool[], bool> fk = (szint, r, E) =>
            {
                if (!r) return true;
                
                int osszSuly = 0;
                for (int i = 0; i < szint; i++)
                {
                    if (E[i])
                    {
                        osszSuly += problema.w[i];
                    }
                }
                osszSuly += problema.w[szint];
                
                return osszSuly <= problema.Wmax;
            };

            
            Func<bool[], double> josag = (E) =>
            {
                double osszErtek = 0;
                for (int i = 0; i < E.Length; i++)
                {
                    if (E[i])
                    {
                        osszErtek += problema.p[i];
                    }
                }
                return osszErtek;
            };
            

            Func<int, bool[], double> fb = (szint, E) =>
            {
                if (szint >= n) return 0;
                
                int hAtralevoKapacitas = problema.Wmax;
                for (int i = 0; i < szint; i++)
                {
                    if (E[i])
                    {
                        hAtralevoKapacitas -= problema.w[i];
                    }
                }
                
                double becsles = 0;
                for (int i = szint; i < n; i++)
                {
                    if (problema.w[i] <= hAtralevoKapacitas)
                    {
                        becsles += problema.p[i];
                        hAtralevoKapacitas -= problema.w[i];
                    }
                    else
                    {
                        becsles += ((double)hAtralevoKapacitas / problema.w[i]) * problema.p[i];
                        break;
                    }
                }
                
                return becsles;
            };
            
            var optimalizalo = new SzetvalasztasEsKorlatozasOptimalizacio<bool>(
                n, M, R, ft, fk, josag, fb);
            
            bool[] eredmeny = optimalizalo.OptimalisMegoldas();
            this.LepesSzam = optimalizalo.LepesSzam;
            
            return eredmeny;
        }
    }
}