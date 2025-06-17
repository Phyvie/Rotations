#ifndef GRID_H
#define GRID_H

#define PI 3.14159265
#define TWO_PI 6.2831853

void SignedFraction_float(float input, out float output)
{
    output = input - trunc(input); 
}

void Line_float(float pos, float aim, float maxDist, out float result)
{
    result = abs(pos - aim) < maxDist ? 1.0 : 0.0;
}

void MakeGridLines_float(float pos, float scale, float lineThickness, out float isGridLine)
{
    float posScaled = pos * scale;
    float posFraction;
    SignedFraction_float(posScaled, posFraction);
    float adjustedThickness = scale * lineThickness;
    Line_float(posFraction, 0, adjustedThickness, isGridLine);
}

void MakeGrid_float(float2 pos, float2 gridScale, float lineThickness, out float isGridLine)
{
    float isGridLineX, isGridLineY; 
    MakeGridLines_float(pos.x, gridScale.x, lineThickness, isGridLineX);
    MakeGridLines_float(pos.y, gridScale.y, lineThickness, isGridLineY);
    isGridLine = max(isGridLineX, isGridLineY);
}

#endif // GGRID_H