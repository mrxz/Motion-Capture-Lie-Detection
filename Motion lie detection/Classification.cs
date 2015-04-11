using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Motion_lie_detection
{
    public static class Classification
    {          
        public static List<float> Classify(ClassificationConfiguration model, LieResult result, int frameId){
            List<float> res = null;
            List<float> movements = result[frameId];
            if (movements != null)
                res = Classification.classify(model, movements);
            return res;
        }

        public static List<float> ClassifyParts(ClassificationConfiguration model, LieResult result, int frameId)
        {
            List<float> res = null;
            List<float> movements = result[frameId];
            if (movements != null)
                res = Classification.classify(model, movements, model.NormalBodyconfiguration.Size + 1);
            return res;
        }

        public static List<int> ClassifyDiscrete(ClassificationConfiguration model, LieResult result, int frameId)
        {
            List<float> prob = Classify(model, result, frameId);
            List<int> res = new List<int>();
            foreach (float p in prob)
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

        public static List<float> ClassifyMeans(ClassificationConfiguration model, LieResult result)
        {
            List<float> res = null;
            List<float> movements = result.Means;
            if (movements != null)
                res = Classification.classify(model, movements);
            return res;
        }

        private static float GaussianNaiveBayes(float average, float variance, float movement)
        {
            float diff = movement - average;
            float var2 = variance * 2;
            return (float)((1 / Math.Sqrt(Math.PI * var2)) * Math.Pow(Math.E, -(diff * diff) / var2));
        }

        private static List<float> classify(ClassificationConfiguration model, List<float> movements, int min = 0, int max = -1)
        {
            List<float> res = new List<float>();
            float[] param;
            max = (max == -1 || max > movements.Count) ? movements.Count : max;

            for (int i = min; i < max; i++)
            {
                param = model[i];
                if (param != null)
                {
                    float ptruth = Classification.GaussianNaiveBayes(param[0], param[1], movements[i] * 1000);
                    float plie = Classification.GaussianNaiveBayes(param[2], param[3], movements[i] * 1000);

                    res.Add(ptruth / (ptruth + plie));
                }
                else
                {
                    res.Add(0.5f);
                }
            }
            return res;
        }
    }
}
