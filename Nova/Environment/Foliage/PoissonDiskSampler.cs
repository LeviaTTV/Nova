using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Nova.Environment.Foliage
{
    public class PoissonDiscSampler
    {
        private const int k = 30;  // Maximum number of attempts before marking a sample as inactive.

        private readonly Rectangle rect;
        private readonly float radius2;  // radius squared
        private readonly float cellSize;
        private Vector2[,] grid;
        private List<Vector2> activeSamples = new List<Vector2>();

        private Random _random;

        /// Create a sampler with the following parameters:
        ///
        /// width:  each sample's x coordinate will be between [0, width]
        /// height: each sample's y coordinate will be between [0, height]
        /// radius: each sample will be at least `radius` units away from any other sample, and at most 2 * `radius`.
        public PoissonDiscSampler(float width, float height, float radius)
        {
            rect = new Rectangle(0, 0, (int)width, (int)height);
            radius2 = radius * radius;
            cellSize = (float)(radius / Math.Sqrt(2));
            /*grid = new Vector2[Mathf.CeilToInt(width / cellSize),
                               Mathf.CeilToInt(height / cellSize)];*/

            grid = new Vector2[(int) Math.Ceiling(width / cellSize), (int)Math.Ceiling(height / cellSize)];

            _random = new Random(Guid.NewGuid().GetHashCode());
        }

        /// Return a lazy sequence of samples. You typically want to call this in a foreach loop, like so:
        ///   foreach (Vector2 sample in sampler.Samples()) { ... }
        public IEnumerable<Vector2> Samples()
        {
            // First sample is choosen randomly
            yield return AddSample(new Vector2((float)(_random.NextDouble() * rect.Width), (float)(_random.NextDouble() * rect.Height)));

            while (activeSamples.Count > 0)
            {

                // Pick a random active sample
                int i = (int)_random.NextDouble() * activeSamples.Count;
                Vector2 sample = activeSamples[i];

                // Try `k` random candidates between [radius, 2 * radius] from that sample.
                bool found = false;
                for (int j = 0; j < k; ++j)
                {

                    float angle = (float)(2 * Math.PI * ((float)_random.NextDouble()));
                    float r = (float)(Math.Sqrt((float)_random.NextDouble() * 3 * radius2 + radius2)); // See: http://stackoverflow.com/questions/9048095/create-random-number-within-an-annulus/9048443#9048443
                    Vector2 candidate = sample + r * new Vector2((float)Math.Cos(angle), (float)Math.Sin(angle));

                    // Accept candidates if it's inside the rect and farther than 2 * radius to any existing sample.
                    if (rect.Contains(candidate) && IsFarEnough(candidate))
                    {
                        found = true;
                        yield return AddSample(candidate);
                        break;
                    }
                }

                // If we couldn't find a valid candidate after k attempts, remove this sample from the active samples queue
                if (!found)
                {
                    activeSamples[i] = activeSamples[activeSamples.Count - 1];
                    activeSamples.RemoveAt(activeSamples.Count - 1);
                }
            }
        }

        private bool IsFarEnough(Vector2 sample)
        {
            GridPos pos = new GridPos(sample, cellSize);

            int xmin = Math.Max(pos.x - 2, 0);
            int ymin = Math.Max(pos.y - 2, 0);
            int xmax = Math.Min(pos.x + 2, grid.GetLength(0) - 1);
            int ymax = Math.Min(pos.y + 2, grid.GetLength(1) - 1);

            for (int y = ymin; y <= ymax; y++)
            {
                for (int x = xmin; x <= xmax; x++)
                {
                    Vector2 s = grid[x, y];
                    if (s != Vector2.Zero)
                    {
                        Vector2 d = s - sample;
                        if (d.X * d.X + d.Y * d.Y < radius2) return false;
                    }
                }
            }

            return true;

            // Note: we use the zero vector to denote an unfilled cell in the grid. This means that if we were
            // to randomly pick (0, 0) as a sample, it would be ignored for the purposes of proximity-testing
            // and we might end up with another sample too close from (0, 0). This is a very minor issue.
        }

        /// Adds the sample to the active samples queue and the grid before returning it
        private Vector2 AddSample(Vector2 sample)
        {
            activeSamples.Add(sample);
            GridPos pos = new GridPos(sample, cellSize);
            grid[pos.x, pos.y] = sample;
            return sample;
        }

        /// Helper struct to calculate the x and y indices of a sample in the grid
        private struct GridPos
        {
            public int x;
            public int y;

            public GridPos(Vector2 sample, float cellSize)
            {
                x = (int)(sample.X / cellSize);
                y = (int)(sample.Y / cellSize);
            }
        }
    }
}
