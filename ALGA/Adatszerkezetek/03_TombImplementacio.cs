using System;
using System.Collections;
using System.Collections.Generic;
using OE.ALGA.Paradigmak;

namespace OE.ALGA.Adatszerkezetek
{
    // 3. heti labor feladat - Tesztek: 03_TombImplementacioTesztek.cs
    public class TombVerem<T> : Verem<T>
    {
        T[] E;
        int n;
        public TombVerem(int meret)
        {
            E =  new T[meret];
            n = 0;

        }

        public bool Ures
        {
            get
            {
                return n == 0;
            }
        }
        public void Verembe(T ertek)
        {
            if (n == E.Length) throw new NincsHelyKivetel();
            E[n++] = ertek;
        }
        public T Verembol()
        {
            if (Ures) throw new NincsElemKivetel();
            n--;
            T top = E[n];
            E[n] = default(T)!;
            return top;
        }
        public T Felso()
        {
            if (Ures) throw new NincsElemKivetel();
            return E[n - 1];
        }
    }
    public class TombSor<T> : Sor<T>
    {
        T[] E;
        int e;
        int u;
        int n;

        public TombSor(int meret)
        {
            E = new T[meret];
            e = 0;
            u = 0;
            n = 0;
        }
        public bool Ures
        {
            get
            {
                return n == 0;
            }
        }
        private int NextIdx(int i) => (i + 1) % E.Length;
        public void Sorba(T ertek)
        {
           if (n == E.Length) throw new NincsHelyKivetel();
           E[u] = ertek;
           u = NextIdx(u);
           n++;
        }
        public T Sorbol()
        {
            if (Ures)  throw new NincsElemKivetel();
            T elso = E[e];
            E[e] = default(T)!;
            e = NextIdx(e);
            n--;
            return elso;
        }
        public T Elso()
        {
            if (Ures) throw new NincsElemKivetel();
            return E[e];
        }
    }
    
    public class TombLista<T>: Lista<T>, IEnumerable<T>
    {
        T[] E;
        int n;
        public int Elemszam
        {
            get
            {
                return n;
            }
        }
        public TombLista(int meret)
        {
            n = meret;
            E = new T[meret];
        }
        public TombLista()
        {
            n = 0;
            E = new T[n];
        }

        void CheckIndex(int index)
        {
            if (index >= n || index < 0)  throw new NincsHelyKivetel();
        }
        void ProvideSpace(int req)
        {
            if (req <= E.Length) return;
            int newSize = Math.Max(E.Length * 2, req);
            T[] _new = new T[newSize];
            Array.Copy(E, _new, E.Length);
            E = _new;
        }
        
        public T Kiolvas(int index)
        {
            CheckIndex(index);
            return E[index];
        }
        public void Modosit(int index, T ertek)
        {
            CheckIndex(index);
            E[index] = ertek;
        }
        public void Hozzafuz(T ertek)
        {
            ProvideSpace(n + 1);
            E[n++] = ertek;
        }
        public void Beszur(int index, T ertek)
        {
           if (index > n || index < 0) throw new HibasIndexKivetel();
           ProvideSpace(n + 1);
           
           Array.Copy(E, index, E, index + 1,n - index);
           E[index] = ertek;
           n++;
        }
        public void Torol(T ertek)
        {
            var cmp = EqualityComparer<T>.Default;
            int ujN = 0;
            for (int i = 0; i < n; i++)
            {
                if (!cmp.Equals(E[i], ertek))
                {
                    E[ujN++] = E[i];
                }
            }
            for (int i = ujN; i < n; i++)
            {
                E[i] = default!;
            }

            n = ujN;
        }
        public void Bejar(Action<T> muvelet)
        {
            for (int i = 0; i < n; i++)
            {
                muvelet(E[i]);
            }
        }
        public IEnumerator<T> GetEnumerator()
        {
            return new TombListaBejaro<T>(E, n);
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
    public class TombListaBejaro<T> : IEnumerator<T>
    {
        T[] E;
        int n;
        int aktualisIndex = -1;
        public T Current
        {
            get
            {
                return E[aktualisIndex];
            }
        }
        object? IEnumerator.Current
        {
            get { return Current; }
        }
        public TombListaBejaro(T[] E, int n)
        {
            this.E = E;
            this.n = n; 
        }
        public bool MoveNext()
        {
            if (aktualisIndex + 1 >=  n) return false;
            aktualisIndex++;
            return true;
        }
        public void Reset() => aktualisIndex = -1;
        public void Dispose()
        {
        }
    }
}
