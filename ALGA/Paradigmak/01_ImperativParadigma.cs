using System;
using System.Collections;
using System.Collections.Generic;

namespace OE.ALGA.Paradigmak
{
    // 1. heti labor feladat - Tesztek: 01_ImperativParadigmaTesztek.cs

    /// <summary>
    /// Olyan objektum, amely végrehajtható. A design lényege:
    /// bármi, ami "feladatként" szerepel, egyetlen metódussal futtatható legyen.
    /// </summary>
    public interface IVegrehajthato
    {
        void Vegrehajtas();
    }

    /// <summary>
    /// Olyan entitás, amely valamilyen függőségtől teszi függővé a végrehajtását.
    /// A FuggosegTeljesul tulajdonság jelzi, hogy szabad-e futni.
    /// </summary>
    public interface IFuggo
    {
        public bool FuggosegTeljesul { get; }
    }

    /// <summary>
    /// Bejárható objektum létrehozását támogató interface.
    /// A cél: ha egy adatszerkezet bejárót akar gyártani, ez lesz a közös kapu.
    /// </summary>
    public interface IBejarhato
    {
        public IBejarhato BejaroLetrehozas();
    }

    /// <summary>
    /// Egyszerű tároló, amely IVegrehajthato típusú elemeket vesz fel.
    /// A klasszikus "imperatív" kollekció: tömbbel működik és kézzel menedzselt mérettel.
    /// </summary>
    public class FeladatTarolo<T> : IEnumerable where T : IVegrehajthato
    {
        protected T[] tarolo; // A backing array, tényleges tárolóhely
        protected int n = 0;  // Jelenleg tárolt elemek száma

        public FeladatTarolo(int meret)
        {
            tarolo = new T[meret];
        }

        /// <summary>
        /// Új elem felvétele a tárolóba.
        /// Ha megtelt, saját exceptiont dobunk (TaroloMegteltKivetel).
        /// </summary>
        public void Felvesz(T elem)
        {
            if (n + 1 > tarolo.Length)
                throw new TaroloMegteltKivetel();

            n++;
            tarolo[n - 1] = elem;
        }

        /// <summary>
        /// A tárolóban lévő összes feladat végrehajtása balról jobbra.
        /// Imperatív stílus: explicit for ciklus és metódushívás.
        /// </summary>
        public virtual void MindentVegrehajt()
        {
            for (int i = 0; i < n; i++)
            {
                tarolo[i].Vegrehajtas();
            }
        }

        /// <summary>
        /// IEnumerable megvalósítása: visszaad egy saját bejárót.
        /// A bejáró csak a ténylegesen tárolt (n darab) elemet járja be.
        /// </summary>
        public IEnumerator GetEnumerator() =>
            new FeladatTaroloBejaro<T>(tarolo, n);
    }

    /// <summary>
    /// Olyan tároló, amely csak akkor hajtja végre egy eleme feladatát,
    /// ha az IFuggo interfészen keresztül engedélyezett.
    /// Ez lényegében egy "feltételes végrehajtó" konténer.
    /// </summary>
    public class FuggoFeladatTarolo<T> : FeladatTarolo<T>
        where T : IFuggo, IVegrehajthato
    {
        public FuggoFeladatTarolo(int meret) : base(meret)
        {
        }

        /// <summary>
        /// Csak azok a feladatok futnak le, amelyek függősége teljesül.
        /// Nem változtatja meg az elemek sorrendjét, csak szűrve végrehajt.
        /// </summary>
        public override void MindentVegrehajt()
        {
            for (int i = 0; i < n; i++)
            {
                // A T típus biztosan IFuggo is (generikus constraint),
                // így a típusellenőrzés igazából redundáns, de nem árt.
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

    /// <summary>
    /// A FeladatTarolo bejárója: IEnumerator-t valósít meg generikus formában.
    /// Lényege: a tömb elemeit sorra adja vissza, amíg el nem éri az n-et.
    /// </summary>
    public class FeladatTaroloBejaro<T> : IEnumerator<T> where T : IVegrehajthato
    {
        T[] _tarolo;
        int _n = 0;
        int _aktualisIndex = -1; // A bejárás kezdetén az enumerátor a "nulladik" elem előtt áll.

        public FeladatTaroloBejaro(T[] tarolo, int n)
        {
            _tarolo = tarolo;
            _n = n;
        }

        /// <summary>
        /// Tovább lép a következő elemre. Ha túlfut, false-al jelzi a végét.
        /// </summary>
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

        /// <summary>
        /// A jelenlegi elem visszaadása.
        /// Feltételezi, hogy a MoveNext() hívások már pozícióba állították.
        /// </summary>
        public T Current =>
            _tarolo[_aktualisIndex];

        public void Dispose()
        {
        }
    }

    /// <summary>
    /// Saját kivétel, amely jelzi, hogy a FeladatTarolo megtelt.
    /// Logikailag az InvalidOperationException rokona, csak specifikusabb.
    /// </summary>
    public class TaroloMegteltKivetel : Exception
    {
    }
}
