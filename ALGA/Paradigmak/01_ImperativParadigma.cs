using System;
using System.Collections;
using System.Collections.Generic;

namespace OE.ALGA.Paradigmak
{
    // 1. heti labor feladat - Tesztek: 01_ImperativParadigmaTesztek.cs
    public interface IVegrehajthato
    {
        void Vegrehajtas();
    }
    public interface IFuggo
    {
        public bool FuggosegTeljesul { get; }
    }

    public interface IBejarhato
    {
        public IBejarhato BejaroLetrehozas();
    }

    public class FeladatTarolo<T> : IEnumerable where T : IVegrehajthato
    {
        protected T[] tarolo;
        protected int n = 0;
        public FeladatTarolo(int meret)
        {
            tarolo = new T[meret];
        }

        public void Felvesz(T elem)
        {
            if (n + 1 > tarolo.Length) throw new TaroloMegteltKivetel();
            n++;
            tarolo[n - 1] = elem;
        }

        public virtual void MindentVegrehajt()
        {
            for (int i = 0; i < n; i++)
            {
                tarolo[i].Vegrehajtas();
            }
        }
        public IEnumerator GetEnumerator() =>
            new FeladatTaroloBejaro<T>(tarolo, n);
    }

    public class FuggoFeladatTarolo<T> : FeladatTarolo<T> where T : IFuggo, IVegrehajthato
    {
        public FuggoFeladatTarolo(int meret) : base(meret)
        {
        }

        public override void MindentVegrehajt()
        {
            for (int i = 0; i < n; i++)
            {
                if (tarolo[i] is IFuggo)
                {
                    if (tarolo[i].FuggosegTeljesul)
                    {
                        tarolo[i].Vegrehajtas();
                    }
                }
            }
        }

    }

    public class FeladatTaroloBejaro<T> : IEnumerator<T> where T : IVegrehajthato
    {
        T[] _tarolo;
        int _n = 0;
        int _aktualisIndex = -1;
        public FeladatTaroloBejaro(T[] tarolo, int n)
        {
            _tarolo = tarolo;
            _n = n;
        }

        public bool MoveNext()
        {
            if (++_aktualisIndex >= _n)
                return false;
            return true;
        }

        public void Reset() =>
            _aktualisIndex = -1;
        object? IEnumerator.Current
        {
            get { return Current; }
        }

        public T Current =>
            _tarolo[_aktualisIndex];

        public void Dispose()
        {
        }
    }


    public class TaroloMegteltKivetel : Exception
    {

    }

}

