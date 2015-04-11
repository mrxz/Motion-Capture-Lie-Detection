using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Motion_lie_detection
{
    public static class Classification
    {          
        public static List<float> ClassifyTruth(ClassificationConfiguration model, LieResult result, int frameId){
            List<float> res = null;
            List<float> movements = result[frameId];
            if (movements != null)
            {
                res = new List<float>();
                float[] param;

                for (int i = 0; i < movements.Count; i++)
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
            }
            return res;
        }

        public static List<float> ClassifyPartsTruth(ClassificationConfiguration model, LieResult result, int frameId)
        {
            List<float> res = null;
            List<float> movements = result[frameId];
            if (movements != null)
            {
                res = new List<float>();
                float[] param;

                for (int i = model.NormalBodyconfiguration.Size + 1; i < movements.Count; i++)
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
            }
            return res;
        }

        public static List<int> ClassifyDiscreteParts(ClassificationConfiguration model, LieResult result, int frameId)
        {
            List<float> prob = ClassifyPartsTruth(model, result, frameId);
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

        private static float GaussianNaiveBayes(float average, float variance, float movement)
        {
            float diff = movement - average;
            float var2 = variance * 2;
            return (float)((1 / Math.Sqrt(Math.PI * var2)) * Math.Pow(Math.E, -(diff * diff) / var2));
        }        
    }
}
