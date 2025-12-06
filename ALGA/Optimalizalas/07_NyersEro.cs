using System;

namespace OE.ALGA.Optimalizalas
{
    // 7. heti labor feladat - Tesztek: 07_NyersEroTesztek.cs

    public class HatizsakProblema
    {
        public int n { get; private set; }
        public int Wmax { get; private set; }
        public int[] w { get; }
        public float[] p { get; }
        
        public HatizsakProblema(int n, int Wmax, int[] w, float[] p)
        {
            this.n = n;
            this.Wmax = Wmax;
            this.w = w;
            this.p = p;
        }
        public int OsszSuly(bool[] pakolas)
        {
            int osszSuly = 0;
            for (int i = 0; i < n; i++)
            {
                if (pakolas[i])
                {
                    osszSuly += w[i];
                }
            }
            return osszSuly;
        }
        public float OsszErtek(bool[] pakolas)
        {
            float osszErtek = 0;
            for (int i = 0; i < n; i++)
            {
                if (pakolas[i])
                {
                    osszErtek += p[i];
                }
            }
            return osszErtek;
        }
        public bool Ervenyes(bool[] pakolas)
        {
            return OsszSuly(pakolas) <= Wmax;
        }
    }

    public class NyersEro<T>
    {
        private int m;
        private Func<int, T> generator;
        private Func<T, double> josag;
        public int LepesSzam { get; private set; }
        
        public NyersEro(int m, Func<int, T> generator, Func<T, double> josag)
        {
            this.m = m;
            this.generator = generator;
            this.josag = josag;
            this.LepesSzam = 0;
        }

        public T OptimalisMegoldas()
        {
            LepesSzam = 0;

            T optimalisMegoldas = generator(1);
            double optimalisJosag = josag(optimalisMegoldas);

            for (int i = 2; i <= m; i++)
            {
                T aktualisMegoldas = generator(i);
                double aktualisJosag = josag(aktualisMegoldas);

                LepesSzam++;
                if (aktualisJosag > optimalisJosag)
                {
                    optimalisJosag = aktualisJosag;
                    optimalisMegoldas = aktualisMegoldas;
                }
            }
            return optimalisMegoldas;
        }
    }

    public class NyersEroHatizsakPakolas
    {
        HatizsakProblema problema;
        bool[]? optimalisMegoldasCache = null;
        
        public int LepesSzam { get; private set; }

        public NyersEroHatizsakPakolas(HatizsakProblema problema)
        {
            this.problema = problema;
        }

        public bool[] Generator(int i)
        {
            bool[] pakolas = new bool[problema.n];

            int szam = i - 1;


            for (int j = 0; j < problema.n; j++)
            {
                pakolas[j] = (szam % 2 == 1);
                szam /= 2;
            }
            return pakolas;
        }
        public double Josag(bool[] pakolas)
        {
            if (problema.Ervenyes(pakolas))
            {
                return problema.OsszErtek((pakolas));
            }
            else
            {
                return -1.0;
            }
        }

        public bool[] OptimalisMegoldas()
        {
            if (optimalisMegoldasCache != null)
            {
                return optimalisMegoldasCache;
            }

            int m = (int)Math.Pow(2, problema.n);

            NyersEro<bool[]> megoldomotor = new NyersEro<bool[]>(m, Generator, Josag);

            bool[] optimalisPakolas = megoldomotor.OptimalisMegoldas();


            this.LepesSzam = megoldomotor.LepesSzam;

            this.optimalisMegoldasCache = optimalisPakolas;
            return optimalisPakolas;
        }
        public double OptimalisErtek()
        {
            bool[] optimalisPakolas = OptimalisMegoldas();

            return Josag(optimalisPakolas);
        }
    }
    
}
