using System.Numerics;

namespace FramesLab.Geometry;

public static class HelixPath
{
    public static List<Vector3> Generate(
        float radius,
        float axialLength,
        int turns,
        int samples)
    {
        if (!float.IsFinite(radius) || radius <= 0)
        throw new ArgumentOutOfRangeException(nameof(radius));

        if (!float.IsFinite(axialLength) || axialLength <= 0)
        throw new ArgumentOutOfRangeException(nameof(axialLength));

        if (turns <= 0)
        throw new ArgumentOutOfRangeException(nameof(turns));

        if (samples < 2)
        throw new ArgumentOutOfRangeException(nameof(samples));

        var points = new List<Vector3>(samples);

        for (int i = 0; i < samples; i++)
        {
            float t = (float)i / (samples - 1);
            float phi = turns * 2f * MathF.PI * t;
            float z = axialLength * t;

            points.Add(new Vector3(
                radius * MathF.Cos(phi),
                radius * MathF.Sin(phi),
                z));
        }

        return points;
    }
}
