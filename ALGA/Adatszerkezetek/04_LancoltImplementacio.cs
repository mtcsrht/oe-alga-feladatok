using System;
using System.Collections;
using System.Collections.Generic;
using OE.ALGA.Paradigmak;

namespace OE.ALGA.Adatszerkezetek
{
    // 4. heti labor feladat - Tesztek: 04_LancoltImplementacioTesztek.cs
    public class LancElem<T>
    {
        public T tart;
        public LancElem<T>? kov;
        public LancElem(T tart, LancElem<T>? kov)
        {
            this.tart = tart;
            this.kov = kov;
        }
    }
    public class LancoltVerem<T> : Verem<T>
    {
        LancElem<T>? fej;
        public bool Ures
        {
            get
            {
                return fej is null;
            }
        }

        public LancoltVerem()
        {
            fej = null;
        }
        public void Verembe(T ertek)
        {
            fej = new LancElem<T>(ertek, fej);
        }
        public T Verembol()
        {
            if (Ures) throw new NincsElemKivetel();
            T ertek = fej.tart;
            fej = fej.kov;
            return ertek;
        }
        public T Felso()
        {
            if (Ures) throw new NincsElemKivetel();
            return fej.tart;
        }
    }

    public class LancoltSor<T> : Sor<T>
    {
        LancElem<T>? fej;
        LancElem<T>? vege;

        public bool Ures
        {
            get
            {
                return fej is null;
            }
        }

        public LancoltSor()
        {
            fej = null;
            vege = null;
        }

        public void Felszabadit()
        {
            fej = null;
            vege = null;
        }

        public void Sorba(T ertek)
        {
            LancElem<T> uj = new LancElem<T>(ertek, null);
            if (vege is null)
            {
                fej = vege = uj;
            }
            else
            {
                vege.kov = uj;
                vege = uj;
            }
        }

        public T Sorbol()
        {
            if (Ures) throw new NincsElemKivetel();
            T ertek = fej.tart;
            fej = fej.kov;
            if (Ures) vege = null;
            return ertek;
        }

        public T Elso()
        {
            if (Ures) throw new NincsElemKivetel();
            return fej.tart;
        }
    }

    public class LancoltListaBejaro<T> : IEnumerator<T>
    {

        readonly LancElem<T>? fej;
        LancElem<T>? aktualisElem;

        public LancoltListaBejaro(LancElem<T>? fej)
        {
            this.fej = fej;
            aktualisElem = null;
        }
        public bool MoveNext()
        {
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
        public T Current
        {
            get
            {
                return aktualisElem!.tart;
            }
        }
        object? IEnumerator.Current
        {
            get
            {
                return Current!;
            }
        }
        public void Dispose()
        {
        }
    }
    public class LancoltLista<T> : Lista<T>, IEnumerable<T>
    {
        LancElem<T>? fej;
        int elemszam;
        public int Elemszam
        {
            get
            {
                return elemszam;
            }
        }

        public LancoltLista()
        {
            fej = null;
            elemszam = 0;
        }
        LancElem<T> KeresIndex(int index)
        {
            if (index < 0 || index >= elemszam) throw new HibasIndexKivetel();

            LancElem<T>? akt = fej!;
            for (int i = 0; i < index; i++)
            {
                akt = akt.kov!;
            }
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
        public void Hozzafuz(T ertek)
        {
            LancElem<T> uj = new LancElem<T>(ertek, null);
            if (fej is null)
            {
                fej = uj;
            }
            else
            {
                LancElem<T>? utolso = fej;
                while (utolso!.kov is not null)
                {
                    utolso = utolso.kov;
                }
                utolso.kov = uj;
            }
            elemszam++;
        }
        public void Beszur(int index, T ertek)
        {
            if (index < 0 || index > elemszam) throw new HibasIndexKivetel();

            LancElem<T> uj = new LancElem<T>(ertek, null);

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
        public void Torol(T ertek)
        {
            if (fej is null) return;

            EqualityComparer<T> eq = EqualityComparer<T>.Default;
            int removed = 0;

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
                    prev.kov = curr.kov;
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
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}