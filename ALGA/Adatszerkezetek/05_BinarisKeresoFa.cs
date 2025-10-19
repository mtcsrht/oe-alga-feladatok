using System;
using OE.ALGA;

namespace OE.ALGA.Adatszerkezetek
{
    // 5. heti labor feladat - Tesztek: 05_BinarisKeresoFaTesztek.cs

    public class FaElem<T> where T : IComparable
    {
        public T tart;
        public FaElem<T>? bal;
        public FaElem<T>? jobb;

        public FaElem(T tart, FaElem<T>? bal, FaElem<T>? jobb)
        {
            this.tart = tart;
            this.bal = bal;
            this.jobb = jobb;
        }
    }

    public class FaHalmaz<T> : Halmaz<T> where T : IComparable
    {
        FaElem<T>? gyoker;

        protected static FaElem<T> ReszfabaBeszur(FaElem<T>? p, T ertek)
        {
            if (p == null)
            {
                return new FaElem<T>(ertek, null, null);
            }

            int cmp = ertek.CompareTo(p.tart);
            if (cmp < 0)
            {
                p.bal = ReszfabaBeszur(p.bal, ertek);
            }
            else if (cmp > 0)
            {
                p.jobb = ReszfabaBeszur(p.jobb, ertek);
            }

            return p;
        }

        protected static bool ReszfaEleme(FaElem<T>? p, T ertek)
        {
            if (p == null) return false;
            int cmp = ertek.CompareTo(p.tart);
            if (cmp == 0) return true;
            if (cmp < 0) return ReszfaEleme(p.bal, ertek);
            return ReszfaEleme(p.jobb, ertek);
        }

        protected static FaElem<T>? ReszfabolTorol(FaElem<T>? p, T ertek)
        {
            if (p == null)
            {
                throw new NincsElemKivetel();
            }

            int cmp = ertek.CompareTo(p.tart);
            if (cmp < 0)
            {
                p.bal = ReszfabolTorol(p.bal, ertek);
                return p;
            }
            if (cmp > 0)
            {
                p.jobb = ReszfabolTorol(p.jobb, ertek);
                return p;
            }

            if (p.bal == null)
            {
                return p.jobb;
            }

            if (p.jobb == null)
            {
                return p.bal;
            }

            p.bal = KetGyerekesTorles(p, p.bal);
            return p;
        }

        protected static FaElem<T> KetGyerekesTorles(FaElem<T> e, FaElem<T> p)
        {
            if (p.jobb == null)
            {
                e.tart = p.tart;
                return p.bal;
            }
            p.jobb = KetGyerekesTorles(e, p.jobb);
            return p;
        }

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
        public void Bejar(Action<T> muvelet)
        {
            ReszfaBejarPreorder(gyoker, muvelet);
        }

        protected static void ReszfaBejarPreorder(FaElem<T>? p, Action<T> muvelet)
        {
            if (p == null) return;
            muvelet(p.tart);
            ReszfaBejarPreorder(p.bal, muvelet);
            ReszfaBejarPreorder(p.jobb, muvelet);
        }

        protected static void ReszfaBejarInorder(FaElem<T>? p, Action<T> muvelet)
        {
            if (p == null) return;
            ReszfaBejarInorder(p.bal, muvelet);
            muvelet(p.tart);
            ReszfaBejarInorder(p.jobb, muvelet);
        }

        protected static void ReszfaBejarPostorder(FaElem<T>? p, Action<T> muvelet)
        {
            if (p == null) return;
            ReszfaBejarPostorder(p.bal, muvelet);
            ReszfaBejarPostorder(p.jobb, muvelet);
            muvelet(p.tart);
        }
    }
}
