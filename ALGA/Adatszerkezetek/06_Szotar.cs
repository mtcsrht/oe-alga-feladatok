using System;

namespace OE.ALGA.Adatszerkezetek
{
    // 6. heti labor feladat - Tesztek: 06_SzotarTesztek.cs

    /// <summary>
    /// Egy szótár bejegyzése: kulcs–érték pár.
    /// Egyszerű kis adatkonténer, amit a főtáblában (E[]) és
    /// a túltöltési listában (U) egyaránt használunk.
    /// </summary>
    public class SzotarElem<K, T>
    {
        public K kulcs;
        public T tart;

        public SzotarElem(K kulcs, T tart)
        {
            this.kulcs = kulcs;
            this.tart = tart;
        }
    }

    /// <summary>
    /// Hash-táblás szótár *túlcsordulási területtel*.
    /// Ez egy klasszikus ütközéskezelési stratégia:
    ///   - A hash-tábla fő részébe (E[]) csak az első elem kerül.
    ///   - Ütközés esetén *nem* próbálkozunk tovább a táblán,
    ///     hanem az elemet behelyezzük egy külön láncolt listába (U).
    ///
    /// A lookup kétlépcsős:
    ///   1) Megnézzük a hash indexet.
    ///   2) Ha ott nem egyezik a kulcs → megnézzük az overflow listát.
    ///
    /// Ez egyszerű, jól tesztelhető, de nagy terhelésnél nem optimális,
    /// mert MINDEN ütközött elem egyetlen listába kerül.
    /// </summary>
    public class HasitoSzotarTulcsordulasiTerulettel<K, T> : Szotar<K,T>
    {
        SzotarElem<K, T>[] E;                     // Főtábla rögzített mérettel
        Func<K, int> h;                           // Hash függvény → index
        LancoltLista<SzotarElem<K, T>> U;         // Túlcsordulási (overflow) lista

        /// <summary>
        /// A konstruktor megkapja a tábla méretét és egy hash-függvényt.
        /// A megadott hash értéket modoljuk E.Length-el.
        /// </summary>
        public HasitoSzotarTulcsordulasiTerulettel(int meret, Func<K, int> hasitoFuggveny)
        {
            E = new SzotarElem<K, T>[meret];
            U = new LancoltLista<SzotarElem<K, T>>();

            // Hash függvény: abs → mod → táblába illesztett index
            h = (kulcs) => Math.Abs(hasitoFuggveny(kulcs)) % E.Length;
        }

        /// <summary>
        /// Másik konstruktor: a kulcs saját GetHashCode értékét használjuk.
        /// </summary>
        public HasitoSzotarTulcsordulasiTerulettel(int meret)
            : this(meret, (kulcs) => kulcs.GetHashCode())
        { }

        /// <summary>
        /// Kulcs keresése a szótárban.
        /// Lépések:
        ///   1) Megnézzük a hash index pozíciót a táblában.
        ///   2) Ha ott nincs találat → átmegyünk az overflow listára.
        /// </summary>
        private SzotarElem<K, T> KulcsKeres(K kulcs)
        {
            int index = h(kulcs);
            SzotarElem<K, T> elem = E[index];

            // 1) Megtalálható-e a főtáblában?
            if (elem != null && elem.kulcs.Equals(kulcs))
            {
                return elem;
            }

            // 2) Ha nem → megnézzük az overflow listát
            foreach (SzotarElem<K, T> tulcsordultElem in U)
            {
                if (tulcsordultElem.kulcs.Equals(kulcs))
                {
                    return tulcsordultElem;
                }
            }

            // Nincs találat
            return null;
        }

        /// <summary>
        /// Beírás (insert vagy update).
        /// Ha a kulcs már létezik → módosítjuk a hozzá tartozó értéket.
        /// Ha nem létezik:
        ///   - Ha a hash slot üres → odarakjuk.
        ///   - Ha foglalt → az elem az overflow listába kerül.
        ///
        /// Tehát az overflow listát csak ütközések esetén használjuk.
        /// </summary>
        public void Beir(K kulcs, T ertek)
        {
            SzotarElem<K, T> letezoElem = KulcsKeres(kulcs);

            // A kulcs már létezett → frissítjük
            if (letezoElem != null)
            {
                letezoElem.tart = ertek;
            }
            else
            {
                // Új elem beszúrása
                SzotarElem<K, T> ujElem = new SzotarElem<K, T>(kulcs, ertek);
                int index = h(kulcs);

                // Ütközés nélkül → a táblába kerül
                if (E[index] == null)
                {
                    E[index] = ujElem;
                }
                else
                {
                    // Ütközés! → Overflow területbe kerül.
                    U.Hozzafuz(ujElem);
                }
            }
        }

        /// <summary>
        /// Kiolvasás kulcs alapján.
        /// Ha megtaláljuk → visszaadjuk az értéket.
        /// Ha nem → kivételt dobunk.
        /// </summary>
        public T Kiolvas(K kulcs)
        {
            SzotarElem<K, T> elem = KulcsKeres(kulcs);
            if (elem != null)
            {
                return elem.tart;
            }
            throw new HibasKulcsKivetel();
        }

        /// <summary>
        /// Törlés kulcs alapján.
        /// Kísérlet sorrendje:
        ///   1) Főtábla megfelelő indexének vizsgálata.
        ///   2) Overflow lista bejárása.
        ///   3) Ha sehol nincs → HibasKulcsKivetel.
        /// </summary>
        public void Torol(K kulcs)
        {
            int index = h(kulcs);
            SzotarElem<K, T> elemE = E[index];

            // 1) Főtáblában van?
            if (elemE != null && elemE.kulcs.Equals(kulcs))
            {
                E[index] = null;
                return;
            }

            // 2) Overflow terület vizsgálata
            SzotarElem<K, T> torlendoElem = null;

            foreach (SzotarElem<K, T> elemU in U)
            {
                if (elemU.kulcs.Equals(kulcs))
                {
                    torlendoElem = elemU;
                    break;
                }
            }

            if (torlendoElem != null)
            {
                U.Torol(torlendoElem);
                return;
            }

            // 3) Sehol nincs → hiba
            throw new HibasKulcsKivetel();
        }
    }
}
