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
        protected Func<T[], float> josag;
        public int LepesSzam { get; private set; }
        
        public VisszalepesesOptimalizacio(int n, int[] M, T[,] R, Func<int, T, bool> ft, Func<int, T, T[], bool> fk, Func<T[], float> josag)
        {
            this.n = n;
            this.M = M;
            this.R = R;
            this.ft = ft;
            this.fk = fk;
            this.josag = josag;
            LepesSzam = 0;
        }
        
        protected virtual void Backtrack(int szint, ref T[] E, ref bool van, ref T[] O)
        {
            int i = 0;
            while (i < M[szint])
            {
                LepesSzam++;
                i++;
                if (ft.Invoke(szint, R[szint, i - 1]))
                {
                    if (fk.Invoke(szint, R[szint, i - 1], E))
                    {
                        E[szint] = R[szint, i - 1];
                        if (szint == n - 1)
                        {
                            if (!van || josag.Invoke(E) > josag.Invoke(O)) E.CopyTo(O, 0);
                            van = true;
                        }
                        else
                        {
                            Backtrack(szint + 1, ref E, ref van, ref O);
                        }
                    }
                }
            }
        }
        
        public T[] OptimalisMegoldas()
        {
            T[] E = new T[n];
            T[] O = new T[n];
            bool van = false;
            Backtrack(0,ref E, ref van, ref O);
            return O;
        }
    }

    public class VisszalepesesHatizsakPakolas
    {
        protected HatizsakProblema problema;
        public int LepesSzam { get; private set; }
        
        public VisszalepesesHatizsakPakolas(HatizsakProblema problema)
        {
            this.problema = problema;
        }
        protected bool ft(int szint, bool r) => true;
        protected bool fk(int szint, bool r, bool[] E)
        {
            if (!r) return true;
            int s = problema.w[szint];
            int i = 0;
            while (i < szint && s <= problema.Wmax)
            {
                if (E[i]) s += problema.w[i];
                i++;
            }
            return (s <= problema.Wmax);
        }

        public virtual bool[] OptimalisMegoldas()
        {
            int[] M = new int[problema.n];
            bool[,] R = new bool[problema.n, 2];
            for (int i = 0; i < problema.n; i++) 
            {
                M[i] = 2;
                R[i,0] = true;
                R[i,1] = false;
            }

            VisszalepesesOptimalizacio<bool> opt = new VisszalepesesOptimalizacio<bool>(problema.n, M, R, ft, fk, problema.OsszErtek);
            bool[] megoldas = opt.OptimalisMegoldas();
            LepesSzam = opt.LepesSzam;
            return megoldas;
        }
        public float OptimalisErtek()
        {
            bool[] E = new bool[problema.n];
            bool[] O = new bool[problema.n];
            return (float)problema.OsszErtek(this.OptimalisMegoldas());
        }
    }

    public class SzetvalasztasEsKorlatozasOptimalizacio<T> : VisszalepesesOptimalizacio<T>
    {
        Func<int, T[], float> fb;
        public new int LepesSzam { get; private set; }
        public SzetvalasztasEsKorlatozasOptimalizacio(int n, int[] M, T[,] R, Func<int, T, bool> ft, Func<int, T, T[], bool> fk, Func<T[], float> josag, Func<int, T[], float> fb) : base(n, M, R, ft, fk, josag)
        {
            this.fb = fb;
            LepesSzam = 0;
        }

        protected override void Backtrack(int szint, ref T[] E, ref bool van, ref T[] O)
        {
            int i = 0;
            while (i < M[szint])
            {
                LepesSzam++;
                i++;
                if (ft.Invoke(szint, R[szint, i - 1]))
                {
                    if (fk.Invoke(szint, R[szint, i - 1], E))
                    {
                        E[szint] = R[szint, i - 1];
                        if (szint == n - 1)
                        {
                            if (!van || josag.Invoke(E) > josag.Invoke(O)) E.CopyTo(O, 0);
                            van = true;
                        }
                        else
                        {
                            if (josag.Invoke(E) + fb.Invoke(szint, E) > josag.Invoke(O)) Backtrack(szint + 1, ref E, ref van, ref O);
                        }
                    }
                }
            }
        }
    }
    public class SzetvalasztasEsKorlatozasHatizsakPakolas : VisszalepesesHatizsakPakolas
    {
        public new int LepesSzam { get; private set; }
        public SzetvalasztasEsKorlatozasHatizsakPakolas(HatizsakProblema problema) : base(problema)
        {
            LepesSzam = 0;
        }
        float fb(int szint, bool[] E)
        {
            float b = 0;
            for (int i = szint; i < problema.n; i++)
            {
                if (problema.OsszSuly(E) + problema.w[i] <= problema.Wmax) b += problema.p[i];
            }
            return b;
        }
        public override bool[] OptimalisMegoldas()
        {
            int[] M = new int[base.problema.n];
            bool[,] R = new bool[problema.n, 2];
            for (int i = 0; i < problema.n; i++)
            {
                M[i] = 2;
                R[i, 0] = true;
                R[i, 1] = false;
            }

            SzetvalasztasEsKorlatozasOptimalizacio<bool> opt = new SzetvalasztasEsKorlatozasOptimalizacio<bool>(problema.n, M, R, ft, fk, problema.OsszErtek, fb);
            bool[] megoldas = opt.OptimalisMegoldas();
            LepesSzam = opt.LepesSzam;
            return megoldas;
        }
    }
}