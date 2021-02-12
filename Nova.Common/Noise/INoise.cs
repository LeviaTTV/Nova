namespace Nova.Common.Noise
{
    public interface INoise
    {
        float[,] Generate(int seed, int width, int height);
    }
}
