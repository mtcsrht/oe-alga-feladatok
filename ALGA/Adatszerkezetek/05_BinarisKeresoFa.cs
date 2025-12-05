using System;
using OE.ALGA;

namespace OE.ALGA.Adatszerkezetek
{
    // 5. heti labor feladat - Tesztek: 05_BinarisKeresoFaTesztek.cs

    // ─────────────────────────────────────────────────────────────
    // FA CSÚCSELEM: egy bináris keresőfa 1 csomópontja
    // ─────────────────────────────────────────────────────────────
    public class FaElem<T> where T : IComparable
    {
        public T tart;              // A csomópont tárolt értéke
        public FaElem<T>? bal;      // Bal részfa gyökere (kisebb elemek)
        public FaElem<T>? jobb;     // Jobb részfa gyökere (nagyobb elemek)

        public FaElem(T tart, FaElem<T>? bal, FaElem<T>? jobb)
        {
            this.tart = tart;
            this.bal = bal;
            this.jobb = jobb;
        }
    }

    // ─────────────────────────────────────────────────────────────
    // HALMAZ BINÁRIS KERESŐFÁVAL – beszúrás, keresés, törlés
    // ─────────────────────────────────────────────────────────────
    public class FaHalmaz<T> : Halmaz<T> where T : IComparable
    {
        FaElem<T>? gyoker;    // A fa gyökere (null esetben üres halmaz)

        // ─────────────────────────────────────────────
        // BESZÚRÁS (rekurzív): standard BST insert
        // ─────────────────────────────────────────────
        protected static FaElem<T> ReszfabaBeszur(FaElem<T>? p, T ertek)
        {
            // Ha üres helyre értünk, létrehozzuk az új csomópontot
            if (p == null)
                return new FaElem<T>(ertek, null, null);

            int cmp = ertek.CompareTo(p.tart);

            if (cmp < 0)
            {
                // Az új érték kisebb → bal részfába szúrunk
                p.bal = ReszfabaBeszur(p.bal, ertek);
            }
            else if (cmp > 0)
            {
                // Nagyobb → jobb részfába
                p.jobb = ReszfabaBeszur(p.jobb, ertek);
            }
            // Egyenlőség esetén: nem csinálunk semmit (halmaz, nincsenek duplikátumok)

            return p; // minden módosítás után vissza kell adni a részfa gyökerét
        }

        // ─────────────────────────────────────────────
        // KERESÉS (rekurzív)
        // ─────────────────────────────────────────────
        protected static bool ReszfaEleme(FaElem<T>? p, T ertek)
        {
            if (p == null) return false;

            int cmp = ertek.CompareTo(p.tart);

            if (cmp == 0) return true;
            if (cmp < 0) return ReszfaEleme(p.bal, ertek);

            return ReszfaEleme(p.jobb, ertek);
        }

        // ─────────────────────────────────────────────
        // TÖRLÉS (rekurzív): a BST három klasszikus esete
        // ─────────────────────────────────────────────
        protected static FaElem<T>? ReszfabolTorol(FaElem<T>? p, T ertek)
        {
            if (p == null)
                throw new NincsElemKivetel(); // Nem találtuk → nem törölhető

            int cmp = ertek.CompareTo(p.tart);

            if (cmp < 0)
            {
                // Törlendő elem a bal részfában van
                p.bal = ReszfabolTorol(p.bal, ertek);
                return p;
            }

            if (cmp > 0)
            {
                // Jobb részfában van
                p.jobb = ReszfabolTorol(p.jobb, ertek);
                return p;
            }

            // Itt cmp == 0 → megtaláltuk a törlendő csomópontot

            // 1) Ha az egyik gyerek null, a másikat visszaadjuk
            if (p.bal == null)
                return p.jobb;

            if (p.jobb == null)
                return p.bal;

            // 2) Két gyerekes eset:
            // A klasszikus megoldás: a bal részfa legnagyobb elemét felvisszük
            p.bal = KetGyerekesTorles(p, p.bal);
            return p;
        }

        /// <summary>
        /// A kettős gyerekű törlés: megtaláljuk a bal részfa legnagyobb elemét
        /// (ez a p.bal jobbra haladó lehulló ágainak legmélye),
        /// felcseréljük a tartalmát a törlendő csomópontéval,
        /// majd eltávolítjuk onnan.
        ///
        /// Rekurzív jobbra haladás, amíg p.jobb null nem lesz.
        /// </summary>
        protected static FaElem<T> KetGyerekesTorles(FaElem<T> e, FaElem<T> p)
        {
            if (p.jobb == null)
            {
                // p a bal részfa legnagyobb eleme.
                e.tart = p.tart;   // az értéket „felhozzuk”
                return p.bal;      // p helyére p bal gyerekét rakjuk (ha volt)
            }

            // Különben jobbra megyünk tovább
            p.jobb = KetGyerekesTorles(e, p.jobb);
            return p;
        }

        // ─────────────────────────────────────────────
        // KÜLSŐ FÜGGVÉNYEK (halmaz interfész)
        // ─────────────────────────────────────────────
        public void Beszur(T ertek)
        {
            gyoker = ReszfabaBeszur(gyoker, ertek);
        }

        public bool Eleme(T ertek)
        {
            return ReszfaEleme(gyoker, ertek);
        }

        public void Torol(T ertek)
        {
            gyoker = ReszfabolTorol(gyoker, ertek);
        }

        // ─────────────────────────────────────────────
        // FA BEJÁRÁSAI (traverzálások)
        // ─────────────────────────────────────────────

        public void Bejar(Action<T> muvelet)
        {
            // A default bejárás Preorder (gyök → bal → jobb)
            ReszfaBejarPreorder(gyoker, muvelet);
        }

        /// <summary>
        /// PREORDER bejárás: gyök → bal → jobb
        /// </summary>
        protected static void ReszfaBejarPreorder(FaElem<T>? p, Action<T> muvelet)
        {
            if (p == null) return;

            muvelet(p.tart);
            ReszfaBejarPreorder(p.bal, muvelet);
            ReszfaBejarPreorder(p.jobb, muvelet);
        }

        /// <summary>
        /// INORDER bejárás: bal → gyök → jobb
        /// Bináris keresőfán rendezetten járja be az elemeket.
        /// </summary>
        protected static void ReszfaBejarInorder(FaElem<T>? p, Action<T> muvelet)
        {
            if (p == null) return;

            ReszfaBejarInorder(p.bal, muvelet);
            muvelet(p.tart);
            ReszfaBejarInorder(p.jobb, muvelet);
        }

        /// <summary>
        /// POSTORDER: bal → jobb → gyök
        /// Leginkább törléshez, felszabadításhoz hasznos.
        /// </summary>
        protected static void ReszfaBejarPostorder(FaElem<T>? p, Action<T> muvelet)
        {
            if (p == null) return;

            ReszfaBejarPostorder(p.bal, muvelet);
            ReszfaBejarPostorder(p.jobb, muvelet);
            muvelet(p.tart);
        }
    }
}
