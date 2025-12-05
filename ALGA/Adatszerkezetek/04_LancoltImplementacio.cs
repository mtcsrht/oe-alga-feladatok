using System;
using System.Collections;
using System.Collections.Generic;
using OE.ALGA.Paradigmak;

namespace OE.ALGA.Adatszerkezetek
{
    // 4. heti labor feladat - Tesztek: 04_LancoltImplementacioTesztek.cs

    // ─────────────────────────────────────────────────────────────
    // ALAP EGYSÉG: LÁNCOLT LISTA EGY ELEME
    // ─────────────────────────────────────────────────────────────
    public class LancElem<T>
    {
        public T tart;              // Az elem tényleges értéke
        public LancElem<T>? kov;    // Pointer a következő láncelemre (vagy null)

        public LancElem(T tart, LancElem<T>? kov)
        {
            this.tart = tart;
            this.kov = kov;
        }
    }

    // ─────────────────────────────────────────────────────────────
    // LÁNCOLT VEREM – stack megvalósítása láncolt listával
    // ─────────────────────────────────────────────────────────────
    public class LancoltVerem<T> : Verem<T>
    {
        LancElem<T>? fej;   // A verem teteje (top elem)

        /// <summary>
        /// Üres-e a verem? Fej == null ⇒ nincs egyetlen elem sem.
        /// </summary>
        public bool Ures => fej is null;

        public LancoltVerem()
        {
            fej = null;
        }

        /// <summary>
        /// Push: új elem beszúrása a lista elejére.
        /// </summary>
        public void Verembe(T ertek)
        {
            // Új elem, amelynek "kov"-ja az eddigi top
            fej = new LancElem<T>(ertek, fej);
        }

        /// <summary>
        /// Pop: felső elem kivétele és visszaadása.
        /// </summary>
        public T Verembol()
        {
            if (Ures) throw new NincsElemKivetel();

            T ertek = fej.tart;
            fej = fej.kov;   // A következő elem lesz a top
            return ertek;
        }

        /// <summary>
        /// Peek: a tetején lévő elem lekérdezése.
        /// </summary>
        public T Felso()
        {
            if (Ures) throw new NincsElemKivetel();
            return fej.tart;
        }
    }

    // ─────────────────────────────────────────────────────────────
    // LÁNCOLT SOR – queue láncolt listával (fej + vég pointer)
    // ─────────────────────────────────────────────────────────────
    public class LancoltSor<T> : Sor<T>
    {
        LancElem<T>? fej;   // Queue első eleme
        LancElem<T>? vege;  // Queue utolsó eleme (O(1) hozzáfűzés miatt)

        public bool Ures => fej is null;

        public LancoltSor()
        {
            fej = null;
            vege = null;
        }

        /// <summary>
        /// A teljes sor felszabadítása (értsd: mindent „elengedünk” GC-nek).
        /// </summary>
        public void Felszabadit()
        {
            fej = null;
            vege = null;
        }

        /// <summary>
        /// Enqueue: új elem hozzáadása a sor végére.
        /// </summary>
        public void Sorba(T ertek)
        {
            LancElem<T> uj = new LancElem<T>(ertek, null);

            // Ha a sor üres, az új elem egyszerre fej és vég
            if (vege is null)
            {
                fej = vege = uj;
            }
            else
            {
                vege.kov = uj;
                vege = uj;   // A vég pointer mostantól az uj
            }
        }

        /// <summary>
        /// Dequeue: az első elem kivétele.
        /// </summary>
        public T Sorbol()
        {
            if (Ures) throw new NincsElemKivetel();

            T ertek = fej.tart;
            fej = fej.kov;

            // Ha kivettük az utolsó elemet, akkor a vege is nullá válik
            if (Ures) vege = null;

            return ertek;
        }

        /// <summary>
        /// A sor első elemének lekérdezése.
        /// </summary>
        public T Elso()
        {
            if (Ures) throw new NincsElemKivetel();
            return fej.tart;
        }
    }

    // ─────────────────────────────────────────────────────────────
    // LÁNCOLT LISTA BEJÁRÓ – foreach támogatása
    // ─────────────────────────────────────────────────────────────
    public class LancoltListaBejaro<T> : IEnumerator<T>
    {
        readonly LancElem<T>? fej;
        LancElem<T>? aktualisElem;   // A jelenleg "aktív" elem (gyakorlatilag iterator pointer)

        public LancoltListaBejaro(LancElem<T>? fej)
        {
            this.fej = fej;
            aktualisElem = null;  // Még nem álltunk elemre
        }

        public bool MoveNext()
        {
            // Első lépéskor fejre ugrunk
            if (aktualisElem is null)
                aktualisElem = fej;
            else
                aktualisElem = aktualisElem.kov;

            return aktualisElem is not null;
        }

        public void Reset()
        {
            aktualisElem = null;
        }

        public T Current => aktualisElem!.tart;

        object? IEnumerator.Current => Current;

        public void Dispose() { }
    }

    // ─────────────────────────────────────────────────────────────
    // LÁNCOLT LISTA – teljes értékű lista implementáció láncolt elemekkel
    // ─────────────────────────────────────────────────────────────
    public class LancoltLista<T> : Lista<T>, IEnumerable<T>
    {
        LancElem<T>? fej;  // A lista első eleme
        int elemszam;      // Az aktuális elemszám

        public int Elemszam => elemszam;

        public LancoltLista()
        {
            fej = null;
            elemszam = 0;
        }

        /// <summary>
        /// Egy adott indexen lévő láncelem visszakeresése.
        /// </summary>
        LancElem<T> KeresIndex(int index)
        {
            if (index < 0 || index >= elemszam) throw new HibasIndexKivetel();

            LancElem<T>? akt = fej!;

            // lineáris előrelépés index-szer
            for (int i = 0; i < index; i++)
                akt = akt.kov!;

            return akt;
        }

        public T Kiolvas(int index)
        {
            return KeresIndex(index).tart;
        }

        public void Modosit(int index, T ertek)
        {
            KeresIndex(index).tart = ertek;
        }

        /// <summary>
        /// Új elem hozzáfűzése a lista végéhez.
        /// </summary>
        public void Hozzafuz(T ertek)
        {
            LancElem<T> uj = new LancElem<T>(ertek, null);

            if (fej is null)
            {
                fej = uj;   // Ha üres volt a lista, az új elem a fej
            }
            else
            {
                // Keressük a legutolsó elemet
                LancElem<T>? utolso = fej;
                while (utolso!.kov is not null)
                    utolso = utolso.kov;

                utolso.kov = uj;
            }
            elemszam++;
        }

        /// <summary>
        /// Beszúrás egy adott indexre (a meglévő elemek jobbra tolódnak).
        /// </summary>
        public void Beszur(int index, T ertek)
        {
            if (index < 0 || index > elemszam) throw new HibasIndexKivetel();

            LancElem<T> uj = new LancElem<T>(ertek, null);

            // 0. index → fej elé beszúrunk
            if (index == 0)
            {
                uj.kov = fej;
                fej = uj;
            }
            else
            {
                LancElem<T> elozo = KeresIndex(index - 1);
                uj.kov = elozo.kov;
                elozo.kov = uj;
            }

            elemszam++;
        }

        /// <summary>
        /// Egy adott érték előfordulásainak törlése.
        /// - külön kezeljük a fej elemet (ha egyező),
        /// - majd végigmegyünk prev-curr párral.
        /// </summary>
        public void Torol(T ertek)
        {
            if (fej is null) return;

            var eq = EqualityComparer<T>.Default;
            int removed = 0;

            // Fejelemek törlése, amíg egyezőek
            while (fej is not null && eq.Equals(fej.tart, ertek))
            {
                fej = fej.kov;
                removed++;
            }

            if (fej is null)
            {
                elemszam -= removed;
                return;
            }

            LancElem<T>? prev = fej;
            LancElem<T>? curr = fej.kov;

            while (curr is not null)
            {
                if (eq.Equals(curr.tart, ertek))
                {
                    prev.kov = curr.kov; // curr átugrása
                    curr = prev.kov;
                    removed++;
                }
                else
                {
                    prev = curr;
                    curr = curr.kov;
                }
            }

            elemszam -= removed;
        }

        /// <summary>
        /// A lista bejárása és művelet végrehajtása minden elemre.
        /// </summary>
        public void Bejar(Action<T> muvelet)
        {
            LancElem<T>? akt = fej;

            while (akt is not null)
            {
                muvelet(akt.tart);
                akt = akt.kov;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new LancoltListaBejaro<T>(fej);
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
