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

#endif // ANGLE_MATH_H