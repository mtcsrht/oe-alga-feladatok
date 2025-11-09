using System;

namespace OE.ALGA.Optimalizalas
{
    // 8. heti labor feladat - Tesztek: 08_DinamikusProgramozasTesztek.cs
    public class DinamikusHatizsakPakolas
    {
        private HatizsakProblema problema;
        private double[,] F_cache = null;
        public int LepesSzam { get; private set; }
        
        public DinamikusHatizsakPakolas(HatizsakProblema problema)
        {
            this.problema = problema; 
            this.LepesSzam = 0;
        }
        public bool[] OptimalisMegoldas()
        {
            double[,] F = TablazatFeltoltes();

            bool[] pakolas = new bool[problema.n]; 
            int n = problema.n;
            int Wmax = problema.Wmax;
            
            int t = n;
            int h = Wmax;

            while (t > 0 && h > 0)
            {
                int itemIndex = t - 1;
                
                if (F[t, h] > F[t - 1, h])
                {
                    pakolas[itemIndex] = true;
                    h -= problema.w[itemIndex];
                }
                else
                {
                    pakolas[itemIndex] = false;
                }
                
                t--;
            }
            
            return pakolas;
        }
        public double OptimalisErtek()
        {
            double[,] F = TablazatFeltoltes();
            
            return F[problema.n, problema.Wmax];
        }
        public double[,] TablazatFeltoltes()
        {
            
            if (F_cache != null)
            {
                return F_cache;
            }

            int n = problema.n;
            int Wmax = problema.Wmax;

            
            double[,] F = new double[n + 1, Wmax + 1];
            
            LepesSzam = 0;

            
            for (int t = 0; t <= n; t++)
            {
                
                for (int h = 0; h <= Wmax; h++)
                {
                    
                    if (t == 0 || h == 0)
                    {
                        F[t, h] = 0;
                    }
                    else
                    {
                        
                        int itemIndex = t - 1;
                        int itemWeight = problema.w[itemIndex];
                        double itemValue = problema.p[itemIndex];

                        
                        LepesSzam++; 

                        
                        if (h < itemWeight)
                        {
                            
                            F[t, h] = F[t - 1, h];
                        }
                        
                        else
                        {
                            
                            double ertek_nem_visszuk = F[t - 1, h];
                            double ertek_visszuk = F[t - 1, h - itemWeight] + itemValue;
                            
                            F[t, h] = Math.Max(ertek_nem_visszuk, ertek_visszuk);
                        }
                    }
                }
            }

            F_cache = F;
            return F_cache;
        }
    }
}
