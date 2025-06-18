using UnityEngine;

[CreateAssetMenu(fileName = "ColorPalette", menuName = "Scriptable Objects/ColorPalette")]
public class ColorPalette : ScriptableObject
{
    public Color PosYaw;
    public Color NegYaw;
    public Color PosPitch;
    public Color NegPitch;
    public Color PosRoll;
    public Color NegRoll;
    
    private static ColorPalette _rotationPalette;
    public static ColorPalette RotationPalette
    {
        get
        {
            if (_rotationPalette == null)
            {
                _rotationPalette = Resources.Load<ColorPalette>("RotationPalette");
                if (_rotationPalette == null)
                {
                    Debug.LogError("Could not find RotationColorsData.asset in Resources folder.");
                }
            }
            return _rotationPalette;
        }
    }

    public Color InterpColorForAxisAndSign(Vector3 RotationVector, bool positive)
    {
        RotationVector = RotationVector.normalized; 
        Color InterpedColor =
            InterpolateColor(RotationVector, Vector3.right, 
                positive ? PosRoll : NegRoll) +
            InterpolateColor(RotationVector, Vector3.up, 
                positive ? PosYaw : NegYaw) + 
            InterpolateColor(RotationVector, Vector3.forward, 
                positive ? PosPitch : NegPitch); 
        
        Color InterpolateColor(Vector3 RotVector, Vector3 Axis, Color Color)
        {
            float dot = Vector3.Dot(RotVector, Axis);
            return dot * dot * Color;
        }
        return InterpedColor;
    }
}
