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
            List<float> res = new List<float>();
            List<float> movements = result[frameId];
            
            float[] param;

            for (int i = 0; i < movements.Count; i++)
            {
                param = model[i];
                if (param != null)
                {
                    res.Add(Classification.GaussianNaiveBayes(param[0], param[1], movements[i]) - Classification.GaussianNaiveBayes(param[2], param[3], movements[i]));
                }
                else
                {
                    res.Add(0);
                }
            }
            return res;
        }

        public static List<float> ClassifyParts(ClassificationConfiguration model, LieResult result, int frameId)
        {
            List<float> res = new List<float>();
            List<float> movements = result[frameId];

            float[] param;

            for (int i = model.NormalBodyconfiguration.Size + 1; i < movements.Count; i++)
            {
                param = model[i];
                if (param != null)
                {
                    res.Add(Classification.GaussianNaiveBayes(param[0], param[1], movements[i]) - Classification.GaussianNaiveBayes(param[2], param[3], movements[i]));
                }
                else
                {
                    res.Add(0);
                }
            }
            return res;
        }   

        private static float GaussianNaiveBayes(float average, float variance, float movement)
        {
            movement *= 10000;
            float diff = movement - average;
            float var2 = variance * 2;
            return (float)((1 / Math.Sqrt(Math.PI * var2)) * Math.Pow(Math.E, -(diff * diff) / var2));
        }        
    }
}
