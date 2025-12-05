using System;
using System.Collections;
using System.Collections.Generic;
using OE.ALGA.Paradigmak;

namespace OE.ALGA.Adatszerkezetek
{
    // 3. heti labor feladat - Tesztek: 03_TombImplementacioTesztek.cs

    // 
    // VEREM (STACK) TÖMBBEL MEGVALÓSÍTVA
    // 
    public class TombVerem<T> : Verem<T>
    {
        T[] E;  // A verem elemeit tároló tömb
        int n;  // A veremben lévő elemek aktuális száma (top index = n-1)

        public TombVerem(int meret)
        {
            E = new T[meret];
            n = 0;
        }

        /// <summary>
        /// Igaz, ha a verem üres. (n == 0)
        /// </summary>
        public bool Ures => n == 0;

        /// <summary>
        /// Push: új elem betétele a verem tetejére.
        /// </summary>
        public void Verembe(T ertek)
        {
            if (n == E.Length) throw new NincsHelyKivetel();  // Ha megtelt, dobunk.
            E[n++] = ertek;  // Betesszük az n-edik helyre, majd növeljük n-t.
        }

        /// <summary>
        /// Pop: a verem tetejéről levesszük és visszaadjuk az elemet.
        /// </summary>
        public T Verembol()
        {
            if (Ures) throw new NincsElemKivetel();
            n--;  // A tetejét veszítjük el
            T top = E[n];  // A régi felső elem
            E[n] = default(T)!;  // Memóriaszivárgás elkerülése (régi elem kiütése)
            return top;
        }

        /// <summary>
        /// Peek: a verem tetején lévő elem megtekintése törlés nélkül.
        /// </summary>
        public T Felso()
        {
            if (Ures) throw new NincsElemKivetel();
            return E[n - 1];
        }
    }

    // 
    // SOR (QUEUE) TÖMBBEL, KÖRBE ÉRTELMEZETT INDEXEKKEL
    // 
    public class TombSor<T> : Sor<T>
    {
        T[] E;
        int e;  // első elem indexe (head)
        int u;  // következő szabad hely indexe (tail)
        int n;  // elemek száma

        public TombSor(int meret)
        {
            E = new T[meret];
            e = 0;
            u = 0;
            n = 0;
        }

        /// <summary>
        /// Nincs elem a sorban.
        /// </summary>
        public bool Ures => n == 0;

        /// <summary>
        /// Következő index körkörösen.
        /// </summary>
        private int NextIdx(int i) => (i + 1) % E.Length;

        /// <summary>
        /// Enqueue: új elem betétele a sor végére.
        /// </summary>
        public void Sorba(T ertek)
        {
            if (n == E.Length) throw new NincsHelyKivetel();
            E[u] = ertek;
            u = NextIdx(u); // Körkörös léptetés
            n++;
        }

        /// <summary>
        /// Dequeue: az első elem kivétele.
        /// </summary>
        public T Sorbol()
        {
            if (Ures) throw new NincsElemKivetel();
            T elso = E[e];
            E[e] = default(T)!; // régi elem törlése
            e = NextIdx(e);
            n--;
            return elso;
        }

        /// <summary>
        /// A sor elejének megtekintése törlés nélkül.
        /// </summary>
        public T Elso()
        {
            if (Ures) throw new NincsElemKivetel();
            return E[e];
        }
    }

    // 
    // LISTA TÖMBBEL (DINAMIKUS TÖMB)
    // 
    public class TombLista<T> : Lista<T>, IEnumerable<T>
    {
        T[] E;  // háttértömb
        int n;  // tényleges elemszám (nem feltétlen egyenlő capacity-vel)

        /// <summary>
        /// A lista aktuális elemszáma.
        /// </summary>
        public int Elemszam => n;

        public TombLista(int meret)
        {
            n = meret;
            E = new T[meret];
        }

        public TombLista()
        {
            n = 0;
            E = new T[n];  // induló üres tömb
        }

        /// <summary>
        /// Indexellenőrzés. Ha a hivatkozás érvénytelen, hibát dob.
        /// </summary>
        void CheckIndex(int index)
        {
            if (index >= n || index < 0) throw new NincsHelyKivetel();
        }

        /// <summary>
        /// Gondoskodik róla, hogy legyen elég hely req darab elemnek.
        /// Szükség esetén kétszerezzük a tömb méretét (amortizált O(1) append).
        /// </summary>
        void ProvideSpace(int req)
        {
            if (req <= E.Length) return;

            int newSize = Math.Max(E.Length * 2, req);
            T[] _new = new T[newSize];
            Array.Copy(E, _new, E.Length);
            E = _new;
        }

        /// <summary>
        /// Érték kiolvasása index alapján.
        /// </summary>
        public T Kiolvas(int index)
        {
            CheckIndex(index);
            return E[index];
        }

        /// <summary>
        /// Érték módosítása index alapján.
        /// </summary>
        public void Modosit(int index, T ertek)
        {
            CheckIndex(index);
            E[index] = ertek;
        }

        /// <summary>
        /// Új elem hozzáfűzése a lista végéhez.
        /// </summary>
        public void Hozzafuz(T ertek)
        {
            ProvideSpace(n + 1);
            E[n++] = ertek;
        }

        /// <summary>
        /// Beszúrás index alapján.
        /// A beszúrás után a többi elem jobbra tolódik.
        /// </summary>
        public void Beszur(int index, T ertek)
        {
            if (index > n || index < 0) throw new HibasIndexKivetel();
            ProvideSpace(n + 1);

            // Tömb részének jobbra tolása
            Array.Copy(E, index, E, index + 1, n - index);

            E[index] = ertek;
            n++;
        }

        /// <summary>
        /// Töröl minden olyan elemet, amely == ertek (egy meghatározott EqualityComparer szerint).
        /// Az összes nem egyező elem előre kerül, a maradék törlődik.
        /// </summary>
        public void Torol(T ertek)
        {
            var cmp = EqualityComparer<T>.Default;
            int ujN = 0;  // új elemszám

            // Csak a nem egyező elemeket írjuk előre
            for (int i = 0; i < n; i++)
            {
                if (!cmp.Equals(E[i], ertek))
                {
                    E[ujN++] = E[i];
                }
            }

            // A maradék helyek nullázása
            for (int i = ujN; i < n; i++)
            {
                E[i] = default!;
            }

            n = ujN;
        }

        /// <summary>
        /// A lista bejárása és művelet végrehajtása minden elemen.
        /// </summary>
        public void Bejar(Action<T> muvelet)
        {
            for (int i = 0; i < n; i++)
            {
                muvelet(E[i]);
            }
        }

        /// <summary>
        /// Generikus enumerátor.
        /// </summary>
        public IEnumerator<T> GetEnumerator()
        {
            return new TombListaBejaro<T>(E, n);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    // 
    // LISTA BEJÁRÓ
    // 
    public class TombListaBejaro<T> : IEnumerator<T>
    {
        T[] E;
        int n;
        int aktualisIndex = -1;

        public T Current => E[aktualisIndex];

        object? IEnumerator.Current => Current;

        public TombListaBejaro(T[] E, int n)
        {
            this.E = E;
            this.n = n;
        }

        /// <summary>
        /// Következő elemre lép. Ha nincs több, false.
        /// </summary>
        public bool MoveNext()
        {
            if (aktualisIndex + 1 >= n) return false;
            aktualisIndex++;
            return true;
        }

        public void Reset() => aktualisIndex = -1;

        public void Dispose() { }
    }
}
