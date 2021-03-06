﻿using System;
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

        public static List<Tuple<double, double>> ClassifyMeansBoth(ClassificationConfiguration model, LieResult result)
        {
            List<Tuple<double, double>> res = null;
            List<double> movements = result.Means;
            if (movements != null)
                res = Classification.classifyBoth(model, movements);
            return res;
        }

        public static List<Tuple<double, double>> ClassifyBoth(ClassificationConfiguration model, List<double> movements)
        {
            List<Tuple<double, double>> res = null;
            if (movements != null)
                res = Classification.classifyBoth(model, movements);
            return res;
        }

        public static double GaussianNaiveBayes(double average, double variance, double movement)
        {
            double diff = movement - average;
            double var2 = variance * 2;
            return (1.0 / Math.Sqrt(Math.PI * var2)) * Math.Exp(-(diff * diff) / var2);
        }

        private static List<double> classify(ClassificationConfiguration model, List<double> movements, int min = 0, int max = -1, double factor = -1)
        {
            List<double> res = new List<double>();
            double[] param;
            max = (max == -1 || max > movements.Count) ? movements.Count : max;
            double fac = (factor == -1) ? model.Factor : factor;

            for (int i = min; i < max; i++)
            {
                param = model[i];
                if (param != null)
                {
                    double ptruth = Classification.GaussianNaiveBayes(param[0], param[1], movements[i] * fac);
                    double plie = Classification.GaussianNaiveBayes(param[2], param[3], movements[i] * fac);

                    res.Add(ptruth / (ptruth + plie));
                }
                else
                {
                    res.Add(0.5);
                }
            }
            return res;
        }

        private static List<Tuple<double, double>> classifyBoth(ClassificationConfiguration model, List<double> movements, int min = 0, int max = -1, double factor = -1)
        {
            List<Tuple<double, double>> res = new List<Tuple<double, double>>();
            double[] param;
            max = (max == -1 || max > movements.Count) ? movements.Count : max;
            double fac = (factor == -1) ? model.Factor : factor;

            for (int i = min; i < max; i++)
            {
                param = model[i];
                if (param != null)
                {
                    double ptruth = Classification.GaussianNaiveBayes(param[0], param[1], movements[i] * fac);
                    double plie = Classification.GaussianNaiveBayes(param[2], param[3], movements[i] * fac);

                    res.Add(Tuple.Create(ptruth, plie));
                }
                else
                {
                    res.Add(Tuple.Create(0.5, 0.5));
                }
            }
            return res;
        }
    }
}
