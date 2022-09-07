using System.Collections;
using System.Collections.Generic;
using System;
namespace TrueSync
{
    public static class CollisionLayer
    {
        public static int[] layerCollisionMatrix = new int[32];


        public static bool GetLayer(byte layer, byte otherLayer)
        {
            return GetBitValue(layerCollisionMatrix[layer], otherLayer);
        }

        public static bool GetLayerByMatrix(int layerCollisionMatrix, byte otherLayer)
        {
            return GetBitValue(layerCollisionMatrix, otherLayer);
        }

        public static void SetLayer(byte layer, byte otherLayer, bool testing)
        {
            if (layer > 32 - 1)
                throw new ArgumentOutOfRangeException("0 <= layer < 32");
            layerCollisionMatrix[layer] = SetBitValue(layerCollisionMatrix[layer], otherLayer, testing);
        }


        public static bool GetBitValue(int v, byte index)
        {
            if (index > 32 - 1)
                throw new ArgumentOutOfRangeException("0 <= index < 32");
            var tempBit = 1 << index;
            return (v & (1 << index)) != 0;
        }

        public static int SetBitValue(int v, byte index, bool bitValue)
        {
            if (index > 32 - 1)
                throw new ArgumentOutOfRangeException("0 <= index < 32");
            var tempBit = 1 << index;
            return ((bitValue ? (int)(v | tempBit) : (int)(v & ~tempBit)));
        }

    }
}