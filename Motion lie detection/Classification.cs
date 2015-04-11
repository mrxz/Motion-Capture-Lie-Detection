using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Motion_lie_detection
{
    public static class Classification
    {          
        public static List<double> Classify(ClassificationConfiguration model, LieResult result, int frameId){
            List<double> res = null;
            List<double> movements = result[frameId];
            if (movements != null)
                res = Classification.classify(model, movements);
            return res;
        }

        public static List<double> ClassifyParts(ClassificationConfiguration model, LieResult result, int frameId)
        {
            List<double> res = null;
            List<double> movements = result[frameId];
            if (movements != null)
                res = Classification.classify(model, movements, model.NormalBodyconfiguration.Size + 1);
            return res;
        }

        public static List<int> ClassifyDiscrete(ClassificationConfiguration model, LieResult result, int frameId)
        {
            List<double> prob = Classify(model, result, frameId);
            List<int> res = new List<int>();
            foreach (double p in prob)
            {
                int discr = 0;
                if (p > model.UpTreshold)
                    discr = 1;
                else if (p < model.LowTreshold)
                    discr = -1;
                res.Add(discr);
            }
            return res;
        }

        public static List<double> ClassifyMeans(ClassificationConfiguration model, LieResult result)
        {
            List<double> res = null;
            List<double> movements = result.Means;
            if (movements != null)
                res = Classification.classify(model, movements);
            return res;
        }

        private static double GaussianNaiveBayes(double average, double variance, double movement)
        {
            double diff = movement - average;
            double var2 = variance * 2;
            return (1.0 / Math.Sqrt(Math.PI * var2)) * Math.Pow(Math.E, -(diff * diff) / var2);
        }

        private static List<double> classify(ClassificationConfiguration model, List<double> movements, int min = 0, int max = -1)
        {
            List<double> res = new List<double>();
            double[] param;
            max = (max == -1 || max > movements.Count) ? movements.Count : max;

            for (int i = min; i < max; i++)
            {
                param = model[i];
                if (param != null)
                {
                    double ptruth = Classification.GaussianNaiveBayes(param[0], param[1], movements[i] * 500);
                    double plie = Classification.GaussianNaiveBayes(param[2], param[3], movements[i] * 500);

                    res.Add(ptruth / (ptruth + plie));
                }
                else
                {
                    res.Add(0.5);
                }
            }
            return res;
        }
    }
}
