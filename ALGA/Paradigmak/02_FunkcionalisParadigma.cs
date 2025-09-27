using System;
using System.Collections;
using System.Collections.Generic;

namespace OE.ALGA.Paradigmak
{
    // 2. heti labor feladat - Tesztek: 02_FunkcionálisParadigmaTesztek.cs


    public class FeltetelesFeladatTarolo<T> : FeladatTarolo<T> where T : IVegrehajthato
    {
        public Func<T, bool> BejaroFeltetel { get; set; }
        public FeltetelesFeladatTarolo(int meret) : base(meret)
        {
        }

        public void FeltetelesVegrehajtas(Func<T,bool> feltetel)
        {
            for (int i = 0; i < n; i++)
            {
                if (feltetel(tarolo[i]))
                {
                    tarolo[i].Vegrehajtas();
                }

            }
        }
        
        public new IEnumerator GetEnumerator() =>
            new FeltetelesFeladatTaroloBejaro<T>(tarolo, n, BejaroFeltetel);

    }

    public class FeltetelesFeladatTaroloBejaro<T> : IEnumerator<T> where T : IVegrehajthato
    {
        T[] tarolo;
        int n;
        int aktualisIndex = -1;
        public Func<T, bool> BejaroFeltetel { get; set; } 
        public T Current
        {
            get
            {
                return tarolo[aktualisIndex];
            }
        }
        public FeltetelesFeladatTaroloBejaro(T[] tarolo, int n, Func<T, bool> bejaroFeltetel)
        {
            this.tarolo = tarolo;
            this.n = n;
            BejaroFeltetel = bejaroFeltetel ?? (_ => true);
            
        }

        public bool MoveNext()
        {
            while (++aktualisIndex < n)
            {
                if (BejaroFeltetel(tarolo[aktualisIndex]))
                    return true;
            }
            return false;
        }
        public void Reset() => aktualisIndex = -1;
       
        object? IEnumerator.Current
        {
            get { return Current; }
        }
        public void Dispose()
        {
        }
    }

}