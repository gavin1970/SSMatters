using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

// Code was pulled and modified from https://swharden.com/blog/2023-03-07-treemapping/
namespace SSMatters.src.services
{
    public static class TreeMapRecs
    {
        #region Public Methods
        public static RectangleF[] GetRectangles(double[] values, int width, int height)
        {
            for (int i = 1; i < values.Length; i++)
                if (values[i] > values[i - 1])
                    throw new ArgumentException("values must be ordered large to small");

            var slice = GetSlice(values, 1, 0.35);
            var rectangles = GetRectangles(slice, width, height);
            return rectangles.Select(x => x.ToRectF()).ToArray();
        }
        #endregion

        #region Private Methods
        private class Slice
        {
            public double Size { get; }
            public IEnumerable<double> Values { get; }
            public Slice[] Children { get; }
            public Slice(double size, IEnumerable<double> values, Slice sub1, Slice sub2)
            {
                Size = size;
                Values = values;
                Children = new Slice[] { sub1, sub2 };
            }
            public Slice(double size, double finalValue)
            {
                Size = size;
                Values = new double[] { finalValue };
                Children = Array.Empty<Slice>();
            }
        }

        private class SliceResult
        {
            public double ElementsSize { get; }
            public IEnumerable<double> Elements { get; }
            public IEnumerable<double> RemainingElements { get; }
            public SliceResult(double elementsSize, IEnumerable<double> elements, IEnumerable<double> remainingElements)
            {
                ElementsSize = elementsSize;
                Elements = elements;
                RemainingElements = remainingElements;
            }
        }

        private class SliceRectangle
        {
            public Slice Slice { get; set; }
            public float X { get; set; }
            public float Y { get; set; }
            public float Width { get; set; }
            public float Height { get; set; }
            public SliceRectangle(Slice slice) => Slice = slice;
            public RectangleF ToRectF() => new RectangleF(X, Y, Width, Height);
        }

        private static Slice GetSlice(IEnumerable<double> elements, double totalSize, double sliceWidth)
        {
            if (elements.Count() == 1)
                return new Slice(totalSize, elements.Single());

            SliceResult sr = GetElementsForSlice(elements, sliceWidth);
            Slice child1 = GetSlice(sr.Elements, sr.ElementsSize, sliceWidth);
            Slice child2 = GetSlice(sr.RemainingElements, 1 - sr.ElementsSize, sliceWidth);
            return new Slice(totalSize, elements, child1, child2);
        }

        private static SliceResult GetElementsForSlice(IEnumerable<double> elements, double sliceWidth)
        {
            var elementsInSlice = new List<double>();
            var remainingElements = new List<double>();
            double current = 0;
            double total = elements.Sum();

            foreach (var element in elements)
            {
                if (current > sliceWidth)
                    remainingElements.Add(element);
                else
                {
                    elementsInSlice.Add(element);
                    current += element / total;
                }
            }

            return new SliceResult(current, elementsInSlice, remainingElements);
        }

        private static IEnumerable<SliceRectangle> GetRectangles(Slice slice, int width, int height)
        {
            SliceRectangle area = new SliceRectangle(slice) { Width = width, Height = height };

            foreach (var rect in GetRectangles(area))
            {
                if (rect.X + rect.Width > area.Width)
                    rect.Width = area.Width - rect.X;

                if (rect.Y + rect.Height > area.Height)
                    rect.Height = area.Height - rect.Y;

                yield return rect;
            }
        }

        private static IEnumerable<SliceRectangle> GetRectangles(SliceRectangle sliceRectangle)
        {
            var isHorizontalSplit = sliceRectangle.Width >= sliceRectangle.Height;
            var currentPos = 0;
            foreach (var subSlice in sliceRectangle.Slice.Children)
            {
                var subRect = new SliceRectangle(subSlice);
                int rectSize;

                if (isHorizontalSplit)
                {
                    rectSize = (int)Math.Round(sliceRectangle.Width * subSlice.Size);
                    subRect.X = sliceRectangle.X + currentPos;
                    subRect.Y = sliceRectangle.Y;
                    subRect.Width = rectSize;
                    subRect.Height = sliceRectangle.Height;
                }
                else
                {
                    rectSize = (int)Math.Round(sliceRectangle.Height * subSlice.Size);
                    subRect.X = sliceRectangle.X;
                    subRect.Y = sliceRectangle.Y + currentPos;
                    subRect.Width = sliceRectangle.Width;
                    subRect.Height = rectSize;
                }

                currentPos += rectSize;

                if (subSlice.Values.Count() > 1)
                {
                    foreach (var sr in GetRectangles(subRect))
                    {
                        yield return sr;
                    }
                }
                else if (subSlice.Values.Count() == 1)
                {
                    yield return subRect;
                }
            }
        }
        #endregion
    }
}
