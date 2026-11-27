
using System.Numerics;

namespace FramesLab.Analysis;

public readonly record struct FrameMetricResult(
    float LengthX,
    float LengthY,
    float LengthZ,
    float DotXY,
    float DotXZ,
    float DotYZ,
    float NormalizedDotXY,
    float NormalizedDotXZ,
    float NormalizedDotYZ,
    float OrthogonalityError,
    float NormalizedOrthogonalityError,
    float Handedness,
    float NormalizedHandedness,
    bool IsFinite,
    bool IsDegenerate);

public static class FrameMetrics
{
    private const float DegenerateTolerance = 1e-8f;

    public static FrameMetricResult Evaluate(
        Vector3 x,
        Vector3 y,
        Vector3 z)
    {
        bool finiteInputs =
            IsFinite(x) &&
            IsFinite(y) &&
            IsFinite(z);

        float lengthX = x.Length();
        float lengthY = y.Length();
        float lengthZ = z.Length();

        bool degenerate =
            !finiteInputs ||
            !float.IsFinite(lengthX) ||
            !float.IsFinite(lengthY) ||
            !float.IsFinite(lengthZ) ||
            lengthX <= DegenerateTolerance ||
            lengthY <= DegenerateTolerance ||
            lengthZ <= DegenerateTolerance;

        float dotXY = Vector3.Dot(x, y);
        float dotXZ = Vector3.Dot(x, z);
        float dotYZ = Vector3.Dot(y, z);

        float orthogonalityError = MathF.Max(
            MathF.Abs(dotXY),
            MathF.Max(
                MathF.Abs(dotXZ),
                MathF.Abs(dotYZ)));

        float handedness = Vector3.Dot(
            Vector3.Cross(x, y),
            z);

        float normalizedDotXY = float.NaN;
        float normalizedDotXZ = float.NaN;
        float normalizedDotYZ = float.NaN;
        float normalizedOrthogonalityError = float.NaN;
        float normalizedHandedness = float.NaN;

        if (!degenerate)
        {
            Vector3 nx = x / lengthX;
            Vector3 ny = y / lengthY;
            Vector3 nz = z / lengthZ;

            normalizedDotXY = Vector3.Dot(nx, ny);
            normalizedDotXZ = Vector3.Dot(nx, nz);
            normalizedDotYZ = Vector3.Dot(ny, nz);

            normalizedOrthogonalityError = MathF.Max(
                MathF.Abs(normalizedDotXY),
                MathF.Max(
                    MathF.Abs(normalizedDotXZ),
                    MathF.Abs(normalizedDotYZ)));

            normalizedHandedness = Vector3.Dot(
                Vector3.Cross(nx, ny),
                nz);
        }

        bool finiteMetrics =
            float.IsFinite(lengthX) &&
            float.IsFinite(lengthY) &&
            float.IsFinite(lengthZ) &&
            float.IsFinite(dotXY) &&
            float.IsFinite(dotXZ) &&
            float.IsFinite(dotYZ) &&
            float.IsFinite(orthogonalityError) &&
            float.IsFinite(handedness);

        if (!degenerate)
        {
            finiteMetrics =
                finiteMetrics &&
                float.IsFinite(normalizedDotXY) &&
                float.IsFinite(normalizedDotXZ) &&
                float.IsFinite(normalizedDotYZ) &&
                float.IsFinite(normalizedOrthogonalityError) &&
                float.IsFinite(normalizedHandedness);
        }

        return new FrameMetricResult(
            lengthX,
            lengthY,
            lengthZ,
            dotXY,
            dotXZ,
            dotYZ,
            normalizedDotXY,
            normalizedDotXZ,
            normalizedDotYZ,
            orthogonalityError,
            normalizedOrthogonalityError,
            handedness,
            normalizedHandedness,
            finiteMetrics,
            degenerate);
    }

    private static bool IsFinite(Vector3 v)
    {
        return
            float.IsFinite(v.X) &&
            float.IsFinite(v.Y) &&
            float.IsFinite(v.Z);
    }
}

