using System.Numerics;
using Leap71.ShapeKernel;
using FramesLab.Analysis;
using FramesLab.Geometry;

Console.WriteLine("Kimi GeometryLab - FramesLab");
Console.WriteLine("ShapeKernel frame comparison");
Console.WriteLine();

var points = HelixPath.Generate(
    radius: 20f,
    axialLength: 100f,
    turns: 5,
    samples: 1000);

Console.WriteLine($"Input points: {points.Count}");
Console.WriteLine("Helix radius: 20 mm");
Console.WriteLine("Axial length: 100 mm");
Console.WriteLine("Turns: 5");
Console.WriteLine();

const int evaluationSamples = 1001;
const float spacing = 1f;

// CYLINDRICAL
var cylindrical = new Frames(
    points,
    Frames.EFrameType.CYLINDRICAL,
    spacing);

CompareFrames(
    "CYLINDRICAL",
    cylindrical,
    evaluationSamples);

// MIN_ROTATION
var minRotation = new Frames(
    points,
    Frames.EFrameType.MIN_ROTATION,
    spacing);

CompareFrames(
    "MIN_ROTATION",
    minRotation,
    evaluationSamples);

// TARGET X
var targetX = new Frames(
    points,
    Vector3.UnitZ,
    spacing);

CompareFrames(
    "TARGET_X (global Z)",
    targetX,
    evaluationSamples);

static void CompareFrames(
    string name,
    Frames frames,
    int sampleCount)
{
    float maxOrthogonality = 0f;
    float sumOrthogonality = 0f;
    float maxUnitLengthError = 0f;

    float minDotX = 1f;
    float minDotY = 1f;
    float minDotZ = 1f;

    int degenerateCount = 0;
    int nonFiniteCount = 0;
    int negativeHandednessCount = 0;
    int validCount = 0;

    Vector3 previousX = Vector3.Zero;
    Vector3 previousY = Vector3.Zero;
    Vector3 previousZ = Vector3.Zero;

    bool hasPrevious = false;

    for (int i = 0; i < sampleCount; i++)
    {
        float ratio = (float)i / (sampleCount - 1);

        Vector3 x = frames.vecGetLocalXAlongLength(ratio);
        Vector3 y = frames.vecGetLocalYAlongLength(ratio);
        Vector3 z = frames.vecGetLocalZAlongLength(ratio);

        FrameMetricResult metric =
            FrameMetrics.Evaluate(x, y, z);

        if (!metric.IsFinite)
            nonFiniteCount++;

        if (metric.IsDegenerate)
        {
            degenerateCount++;
            hasPrevious = false;
            continue;
        }

        validCount++;

        float error = metric.NormalizedOrthogonalityError;

        maxOrthogonality = MathF.Max(
            maxOrthogonality,
            error);

        sumOrthogonality += error;

        maxUnitLengthError = MathF.Max(
            maxUnitLengthError,
            MathF.Max(
                MathF.Abs(metric.LengthX - 1f),
                MathF.Max(
                    MathF.Abs(metric.LengthY - 1f),
                    MathF.Abs(metric.LengthZ - 1f))));

        if (metric.NormalizedHandedness < 0f)
            negativeHandednessCount++;

        Vector3 currentX = Vector3.Normalize(x);
        Vector3 currentY = Vector3.Normalize(y);
        Vector3 currentZ = Vector3.Normalize(z);

        if (hasPrevious)
        {
            minDotX = MathF.Min(
                minDotX,
                Vector3.Dot(previousX, currentX));

            minDotY = MathF.Min(
                minDotY,
                Vector3.Dot(previousY, currentY));

            minDotZ = MathF.Min(
                minDotZ,
                Vector3.Dot(previousZ, currentZ));
        }

        previousX = currentX;
        previousY = currentY;
        previousZ = currentZ;

        hasPrevious = true;
    }

    Console.WriteLine($"===== {name} =====");
    Console.WriteLine($"Evaluation samples: {sampleCount}");
    Console.WriteLine($"Valid frames: {validCount}");
    Console.WriteLine($"Degenerate frames: {degenerateCount}");
    Console.WriteLine($"Non-finite frames: {nonFiniteCount}");
    Console.WriteLine($"Negative handedness: {negativeHandednessCount}");
    Console.WriteLine($"Maximum orthogonality error: {maxOrthogonality:F8}");
    Console.WriteLine($"Mean orthogonality error: {(validCount > 0 ? sumOrthogonality / validCount : float.NaN):F8}");
    Console.WriteLine($"Maximum unit length error: {maxUnitLengthError:F8}");
    Console.WriteLine($"Minimum consecutive X dot: {minDotX:F8}");
    Console.WriteLine($"Minimum consecutive Y dot: {minDotY:F8}");
    Console.WriteLine($"Minimum consecutive Z dot: {minDotZ:F8}");
    Console.WriteLine();
}

