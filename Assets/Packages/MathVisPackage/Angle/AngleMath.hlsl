#ifndef ANGLE_MATH_H
#define ANGLE_MATH_H

#define PI 3.14159265
#define TWO_PI 6.2831853
#define THREE_PI 9.424777961

float WrapAngleToPlusMinusPI(float angle)
{
    return fmod(angle + THREE_PI, TWO_PI)-PI; 
}

void IsValueInModRange_float(float val, float alpha, float beta, bool excludeFullCircle, out float result)
{
    float wVal = WrapAngleToPlusMinusPI(val);
    float wAlpha = WrapAngleToPlusMinusPI(alpha);
    float wBeta = WrapAngleToPlusMinusPI(beta);

    if (wAlpha == wBeta && excludeFullCircle)
    {
        result = 0.0f;
        return; 
    }
    if (wAlpha < wBeta)
    {
        result = (wAlpha < wVal && wVal < wBeta) ? 1.0f : 0.0f;
    } 
    else
    {
        result = (wBeta < wVal && wVal < wAlpha) ? 1.0f : 0.0f; 
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
    float remappedAngle = (angle+TWO_PI)%TWO_PI/TWO_PI;
    float lerpAlpha = spiralCount > 0 ? 1-remappedAngle : remappedAngle; 
    float firstRadius = lerp(outerRadius, outerRadius*radiusRatio, lerpAlpha); 
    MultiCircle_float(distanceTo0, angle, min(abs(spiralCount), 5), firstRadius, radiusRatio, isInSpiral);  
}

#endif // ANGLE_MATH_H