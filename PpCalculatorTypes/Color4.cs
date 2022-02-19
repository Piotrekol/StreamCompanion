using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Xml.Serialization;

namespace PpCalculatorTypes
{
    /// <summary>
    /// Represents a color with 4 floating-point components (R, G, B, A).
    /// </summary>
    [Serializable]
    [JsonConverter(typeof(ColorHexConverter))]
    public struct Color4 : IEquatable<Color4>
    {
        /// <summary>
        /// The red component of this Color4 structure.
        /// </summary>
        public float R;

        /// <summary>
        /// The green component of this Color4 structure.
        /// </summary>
        public float G;

        /// <summary>
        /// The blue component of this Color4 structure.
        /// </summary>
        public float B;

        /// <summary>
        /// The alpha component of this Color4 structure.
        /// </summary>
        public float A;

        /// <summary>
        /// Constructs a new Color4 structure from the specified components.
        /// </summary>
        /// <param name="r">The red component of the new Color4 structure.</param>
        /// <param name="g">The green component of the new Color4 structure.</param>
        /// <param name="b">The blue component of the new Color4 structure.</param>
        /// <param name="a">The alpha component of the new Color4 structure.</param>
        public Color4(float r, float g, float b, float a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        /// <summary>
        /// Constructs a new Color4 structure from the specified components.
        /// </summary>
        /// <param name="r">The red component of the new Color4 structure.</param>
        /// <param name="g">The green component of the new Color4 structure.</param>
        /// <param name="b">The blue component of the new Color4 structure.</param>
        /// <param name="a">The alpha component of the new Color4 structure.</param>
        public Color4(byte r, byte g, byte b, byte a)
        {
            R = r / (float)Byte.MaxValue;
            G = g / (float)Byte.MaxValue;
            B = b / (float)Byte.MaxValue;
            A = a / (float)Byte.MaxValue;
        }

        /// <summary>
        /// Compares whether this Color4 structure is equal to the specified object.
        /// </summary>
        /// <param name="obj">An object to compare to.</param>
        /// <returns>True obj is a Color4 structure with the same components as this Color4; false otherwise.</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is Color4))
                return false;

            return Equals((Color4)obj);
        }

        /// <summary>
        /// Converts the specified Color4 to a System.Drawing.Color structure.
        /// </summary>
        /// <param name="color">The Color4 to convert.</param>
        /// <returns>A new System.Drawing.Color structure containing the converted components.</returns>
        public static explicit operator Color(Color4 color)
        {
            return Color.FromArgb(
                (int)(color.A * Byte.MaxValue),
                (int)(color.R * Byte.MaxValue),
                (int)(color.G * Byte.MaxValue),
                (int)(color.B * Byte.MaxValue));
        }

        /// <summary>
        /// Compares whether this Color4 structure is equal to the specified Color4.
        /// </summary>
        /// <param name="other">The Color4 structure to compare to.</param>
        /// <returns>True if both Color4 structures contain the same components; false otherwise.</returns>
        public bool Equals(Color4 other)
        {
            return
                this.R == other.R &&
                this.G == other.G &&
                this.B == other.B &&
                this.A == other.A;
        }

        /// <summary>
        /// Returns a lightened version of the colour.
        /// </summary>
        /// <param name="colour">Original colour</param>
        /// <param name="amount">Decimal light addition</param>
        public Color4 Lighten(float amount) => Multiply(1 + amount);

        /// <summary>
        /// Multiply the RGB coordinates by a scalar.
        /// </summary>
        /// <param name="colour">Original colour</param>
        /// <param name="scalar">A scalar to multiply with</param>
        public Color4 Multiply(float scalar)
        {
            if (scalar < 0)
                throw new ArgumentOutOfRangeException(nameof(scalar), scalar, "Can not multiply colours by negative values.");

            return new Color4(
                Math.Min(1, R * scalar),
                Math.Min(1, G * scalar),
                Math.Min(1, B * scalar),
                A);
        }

        /// <summary>
        /// Converts this color to an integer representation with 8 bits per channel.
        /// </summary>
        /// <returns>A <see cref="System.Int32"/> that represents this instance.</returns>
        /// <remarks>This method is intended only for compatibility with System.Drawing. It compresses the color into 8 bits per channel, which means color information is lost.</remarks>
        public int ToArgb()
        {
            uint value =
                (uint)(A * Byte.MaxValue) << 24 |
                (uint)(R * Byte.MaxValue) << 16 |
                (uint)(G * Byte.MaxValue) << 8 |
                (uint)(B * Byte.MaxValue);

            return unchecked((int)value);
        }

        /// <summary>
        /// Converts a <see cref="Color4"/> into a hex colour code.
        /// </summary>
        /// <param name="colour">The <see cref="Color4"/> to convert.</param>
        /// <param name="alwaysOutputAlpha">Whether the alpha channel should always be output. If <c>false</c>, the alpha channel is only output if <paramref name="colour"/> is translucent.</param>
        /// <returns>The hex code representing the colour.</returns>
        public string ToHex(bool alwaysOutputAlpha = false)
        {
            int argb = this.ToArgb();
            byte a = (byte)(argb >> 24);
            byte r = (byte)(argb >> 16);
            byte g = (byte)(argb >> 8);
            byte b = (byte)argb;

            if (!alwaysOutputAlpha && a == 255)
                return $"#{r:X2}{g:X2}{b:X2}";

            return $"#{r:X2}{g:X2}{b:X2}{a:X2}";
        }
        public override string ToString()
        {
            return ToHex();
        }
    }
}
