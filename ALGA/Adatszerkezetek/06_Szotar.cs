using System;

namespace OE.ALGA.Adatszerkezetek
{
    // 6. heti labor feladat - Tesztek: 06_SzotarTesztek.cs

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
    
    public class HasitoSzotarTulcsordulasiTerulettel<K, T> : Szotar<K,T>
    {
        SzotarElem<K, T>[] E;

        Func<K, int> h;
        
        LancoltLista<SzotarElem<K, T>> U;

        public HasitoSzotarTulcsordulasiTerulettel(int meret, Func<K, int> hasitoFuggveny)
        {
            E = new SzotarElem<K, T>[meret];
            U = new LancoltLista<SzotarElem<K, T>>();
            h = (kulcs) => Math.Abs(hasitoFuggveny(kulcs)) % E.Length;
        }

        public HasitoSzotarTulcsordulasiTerulettel(int meret) : this(meret, (kulcs) => kulcs.GetHashCode())
        {
            
        }

        private SzotarElem<K, T> KulcsKeres(K kulcs)
        {
            int index = h(kulcs);
            SzotarElem<K, T> elem = E[index];

            if (elem != null && elem.kulcs.Equals(kulcs))
            {
                return elem;
            }

            foreach (SzotarElem<K,T> tulcsordultElem in U)
            {
                if (tulcsordultElem.kulcs.Equals(kulcs))
                {
                    return tulcsordultElem;
                }
            }
            return null;
        }
        public void Beir(K kulcs, T ertek)
        {
            SzotarElem<K, T> letezoElem = KulcsKeres(kulcs);

            if (letezoElem != null)
            {
                letezoElem.tart = ertek;
            }
            else
            {
                SzotarElem<K, T> ujElem = new SzotarElem<K, T>(kulcs, ertek);
                int index = h(kulcs);

                if (E[index] == null)
                {
                    E[index] = ujElem;
                }
                else
                {
                    U.Hozzafuz(ujElem);
                }
            }
        }
        public T Kiolvas(K kulcs)
        {
            SzotarElem<K, T> elem = KulcsKeres(kulcs);
            if (elem != null)
            {
                return elem.tart;
            }
            throw new HibasKulcsKivetel();
        }
        public void Torol(K kulcs)
        {
            int index = h(kulcs);
            SzotarElem<K, T> elemE = E[index];
            
            if (elemE != null && elemE.kulcs.Equals(kulcs))
            {
                E[index] = null;
                return;
            }
            
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
            
            throw new HibasKulcsKivetel();
        }
    }
}
