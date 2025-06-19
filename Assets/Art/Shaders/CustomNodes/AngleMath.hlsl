#ifndef ANGLE_MATH_H
#define ANGLE_MATH_H

#define PI 3.14159265
#define TWO_PI 6.2831853

float WrapAngle(float angle)
{
    angle = fmod(angle + PI, TWO_PI);
    if (angle < 0.0)
    {
        angle += TWO_PI;
    }
    return angle-PI; 
}

void IsValueInModRange_float(float val, float alpha, float beta, 
    out float result)
{
    float wVal = WrapAngle(val);
    float wAlpha = WrapAngle(alpha);
    float wBeta = WrapAngle(beta);
        
    if (wAlpha < wBeta)
    {
        result = wAlpha < wVal && wVal < wBeta ? 1.0f : 0.0f;
    } 
    else
    {
        result = wBeta < wVal && wVal < wAlpha ? 1.0f : 0.0f; 
    }
    
    if (sign(alpha - beta) != sign(wAlpha - wBeta))
    {
        result = 1.0f - result;
    }
}

void FullCircle_float(float distanceTo0, float angle, float circleRadius, out float isInCircle)
{
    isInCircle = distanceTo0 < circleRadius ? 1.0f : 0.0f;
}

void MultiCircle_float(float distanceTo0, float angle, float circleCount, float firstRadius, float radiusRatio, out float inCirclesCount)
{
    float radius = firstRadius;
    int count = 0;

    [loop]
    for (int i = 0; i < circleCount; i++)
    {
        if (distanceTo0 < radius)
        {
            count++; 
        }
        radius *= radiusRatio; 
    }

    inCirclesCount = count;  
}

void ToPolarCoordinates_float(float2 pos, out float radius, out float angle)
{
    radius = length(pos);          
    angle = atan2(pos.y, pos.x) / (TWO_PI); 
}

void Length_float(float2 pos, out float radius)
{
    radius = distance(pos, float2(0, 0)); 
}

void MultiSpiral_float(float distanceTo0, float angle, float spiralCount, float outerRadius, float radiusRatio, out float isInSpiral)
{
    float firstRadius = lerp(outerRadius, outerRadius*radiusRatio, 1-(((angle+TWO_PI)%(TWO_PI))/(TWO_PI))); 
    MultiCircle_float(distanceTo0, angle, spiralCount, firstRadius, radiusRatio, isInSpiral);  
}

#endif // ANGLE_MATH_H