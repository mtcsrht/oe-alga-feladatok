using System;
using System.Collections.Generic;

namespace OE.ALGA.Adatszerkezetek
{
    // 11. heti labor feladat - Tesztek: 11_KupacTesztek.cs


    public class Kupac<T> 
    {
        protected T[] E;
        protected int n;
        protected Func<T, T, bool> nagyobbPrioritas;

        public Kupac(T[] E, int n, Func<T, T, bool> nagyobbPrioritas)
        {
            this.E = E;
            this.n = n;
            this.nagyobbPrioritas = nagyobbPrioritas;
            KupacotEpit();
        }

        public static int Bal(int i) => (i+1) * 2-1;
        public static int Jobb(int i) => (i + 1) * 2;
        
        public static int Szulo(int i) => (int)Math.Floor((double)(i+1) / 2)-1;


        public void Kupacol(int i)
        {
            int b = Bal(i);
            int j = Jobb(i);
            int max = i;
            
            if (b < n && nagyobbPrioritas.Invoke(E[b], E[max]))
                max = b;

            if (j < n && nagyobbPrioritas.Invoke(E[j], E[max]))
                max = j;

            if (max != i)
            {
                (E[i], E[max]) = (E[max], E[i]);
                Kupacol(max);
            }
        }

        public void KupacotEpit()
        {
            for (int i = n/2; i >= 1; i--)
            {
                Kupacol(i-1);
            }
        }
    }
    public class KupacRendezes<T> : Kupac<T> where T : IComparable<T>
    {
        //lehet meg kell fordítani a relációs jelet!!
        //(ez, ennel) => ez.Length > ennel.Length
        public KupacRendezes(T[] A) : base(A, A.Length, (ez, ennel) => ez.CompareTo(ennel) == 1)
        {
        }

        public void Rendezes()
        {
            for (int i = n - 1; i >= 0; i--)
            {
                (E[i], E[0]) = (E[0], E[i]);
                n--;
                Kupacol(0);
            }
        }
    }

    public class KupacPrioritasosSor<T> : Kupac<T>, PrioritasosSor<T>
    {
        public KupacPrioritasosSor(int n, Func<T, T, bool> nagyobbPrioritas)
            : base(new T[n],0, nagyobbPrioritas)
        {
        }

        public bool Ures => n == 0;

        private void KulcsotFelvisz(int i)
        {
            int sz = Szulo(i);
            if (sz >= 0 && nagyobbPrioritas(E[i], E[sz]))
            {
                (E[i], E[sz]) = (E[sz], E[i]);
                KulcsotFelvisz(sz);
            }
        }
        public void Sorba(T ertek)
        {
            if (n < E.Length)
            {
                E[n] = ertek;
                n++;
                KulcsotFelvisz(n-1);
            }
            else throw new NincsHelyKivetel();
        }

        public T Sorbol()
        {
            if (!Ures)
            {
                T max = E[0];
                E[0] = E[n - 1];
                n -= 1;
                Kupacol(0);
                return max;
            }

            throw new NincsElemKivetel();

        }

        public T Elso()
        {
            if (!Ures)
                return  E[0];
            
            throw new NincsElemKivetel();
        }

        public void Frissit(T elem)
        {
            int i = 0;
            for(i = 0; i < n && !E[i].Equals(elem);)
            { i++; }
            
            if (n <= i) throw new NincsElemKivetel();
            KulcsotFelvisz(i);
            Kupacol(i);
        }
    }
}