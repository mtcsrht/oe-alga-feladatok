using System;
using System.Collections;
using System.Collections.Generic;

namespace OE.ALGA.Paradigmak
{
    // 2. heti labor feladat - Tesztek: 02_FunkcionálisParadigmaTesztek.cs

    /// <summary>
    /// Egy olyan feladattároló, amely képes:
    /// 1) tetszőleges feltétel alapján végrehajtani elemeket
    /// 2) tetszőleges feltétel alapján bejárni a tárolót (a GetEnumerator felülírásával)
    ///
    /// Itt jelenik meg a funkcionális paradigma szelleme:
    /// a "mit csináljunk?" helyét átveszi a "milyen szabály szerint csináljuk?" (→ függvényparaméter).
    /// </summary>
    public class FeltetelesFeladatTarolo<T> : FeladatTarolo<T> where T : IVegrehajthato
    {
        /// <summary>
        /// A bejáró működését meghatározó feltétel.
        /// Ha null, akkor a bejáró minden elemet beenged — ezért később kap egy "default true" lambdát.
        /// </summary>
        public Func<T, bool> BejaroFeltetel { get; set; }

        public FeltetelesFeladatTarolo(int meret) : base(meret)
        {
        }

        /// <summary>
        /// A tárolóban lévő elemek végrehajtása *csak akkor*, ha a hívó által megadott feltétel igaz rájuk.
        /// Ez egy direkt funkcionális vezérlőminta: a logika kívülről érkezik.
        /// </summary>
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

        /// <summary>
        /// A GetEnumerator felüldefiniálása:
        /// nem a sima FeladatTaroloBejaro-t adjuk vissza, hanem egy olyan bejárót,
        /// amely a BejaroFeltetel alapján szűri be az elemeket.
        /// 
        /// new kulcsszó: itt *elrejtjük* az ős GetEnumerator-ját, nem override-oljuk.
        /// </summary>
        public new IEnumerator GetEnumerator() =>
            new FeltetelesFeladatTaroloBejaro<T>(tarolo, n, BejaroFeltetel);
    }

    /// <summary>
    /// Olyan enumerátor, amely csak azokat az elemeket adja vissza,
    /// amelyekre a bejárási feltétel (predicate) igaz.
    ///
    /// Ez már egy tiszta funkcionális iterátor:
    /// nem változtatja meg az adatot, csak szűrt nézetet ad róla.
    /// </summary>
    public class FeltetelesFeladatTaroloBejaro<T> : IEnumerator<T> where T : IVegrehajthato
    {
        T[] tarolo;
        int n;
        int aktualisIndex = -1;

        /// <summary>
        /// A bejáró szűrőfeltétele:
        /// csak az az elem tekinthető "Current"-nek vagy MoveNext által érvényesnek,
        /// amelyre ez a lambda igaz.
        /// </summary>
        public Func<T, bool> BejaroFeltetel { get; set; }

        /// <summary>
        /// A jelenlegi elem.
        /// Feltételezés: MoveNext már olyan indexre állította az aktuálisIndexet,
        /// amely megfelel a feltételnek.
        /// </summary>
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

            // Ha a felhasználó nem adott meg feltételt, akkor a bejáró minden elemet engedjen át
            BejaroFeltetel = bejaroFeltetel ?? (_ => true);
        }

        /// <summary>
        /// Lényegében egy "szűrt MoveNext":
        /// végiglépked a tömbön addig, amíg nem talál egy olyan elemet,
        /// amelyre BejaroFeltetel igaz.
        /// Ha talál: visszatér true-val.
        /// Ha eléri a végét: false, tehát vége az enumerálásnak.
        /// </summary>
        public bool MoveNext()
        {
            while (++aktualisIndex < n)
            {
                if (BejaroFeltetel(tarolo[aktualisIndex]))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// A bejárást visszaállítja a kezdő pozícióba (az első érvényes elem előtt).
        /// </summary>
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
