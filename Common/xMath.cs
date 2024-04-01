using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xLibV100.Common
{
    public class xMath
    {
        public class AddVirtualPointsOptions
        {
            public double[] Convolution;
            public int NumberOfPasses = 1;
        }

        public class FilteringOptions
        {
            public double[] Convolution;
            public int NumberOfPasses = 1;
        }

        public class ReducingPointsOptions
        {
            public double[] Convolution;
            public int NumberOfPasses = 1;
        }

        public static double[] AddVirtualPoints(double[] points, AddVirtualPointsOptions options)
        {
            List<double> virtualPoints = new List<double>();

            for (int pass = 0; pass < options.NumberOfPasses; pass++)
            {
                virtualPoints.Clear();

                for (int i = 0; i < points.Length; i++)
                {
                    double average = 0;
                    double numberOfSamples = 0;
                    int offset = i - options.Convolution.Length / 2;

                    for (int j = 0; j < options.Convolution.Length; j++)
                    {
                        int step = offset + j;

                        if (step >= 0 && step < points.Length)
                        {
                            average += points[step] * options.Convolution[j];
                            numberOfSamples += options.Convolution[j];
                        }
                    }

                    average /= numberOfSamples;

                    virtualPoints.Add(points[i]);
                    virtualPoints.Add(average);
                }

                points = virtualPoints.ToArray();
            }

            return points;
        }

        public static double[] Filtering(double[] points, FilteringOptions options)
        {
            List<double> virtualPoints = new List<double>();

            for (int i = 0; i < points.Length; i++)
            {
                double average = 0;
                double numberOfSamples = 0;

                int offset = i - options.Convolution.Length / 2;

                for (int j = 0; j < options.Convolution.Length; j++)
                {
                    int step = offset + j;

                    if (step >= 0 && step < points.Length)
                    {
                        average += points[step] * options.Convolution[j];
                        numberOfSamples += options.Convolution[j];
                    }
                }

                average /= numberOfSamples;
                virtualPoints.Add(average);
            }

            return virtualPoints.ToArray();
        }

        public static double[] ReducingPoints(double[] points, ReducingPointsOptions options)
        {
            List<double> virtualPoints = new List<double>();

            for (int pass = 0; pass < options.NumberOfPasses; pass++)
            {
                virtualPoints.Clear();

                for (int i = 0; i < points.Length; i += 2)
                {
                    double average = 0;
                    double numberOfSamples = 0;

                    int offset = i - options.Convolution.Length / 2;

                    for (int j = 0; j < options.Convolution.Length; j++)
                    {
                        int step = offset + j;

                        if (step >= 0 && step < points.Length)
                        {
                            average += points[step] * options.Convolution[j];
                            numberOfSamples += options.Convolution[j];
                        }
                    }

                    average /= numberOfSamples;
                    virtualPoints.Add(average);
                }

                points = virtualPoints.ToArray();
            }

            return virtualPoints.ToArray();
        }
    }
}
