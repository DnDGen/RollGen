using System;
using System.Security.Cryptography;

namespace D20Dice.RandomWrappers
{
    public class RandomFactory
    {
        public static IRandomWrapper Create(Random random)
        {
            IRandomWrapper randomWrapper = new RandomWrapper(random);

            return randomWrapper;
        }
        
        public static IRandomWrapper CreateCryptographicallyStrong()
        {
            var crypto = new RNGCryptoServiceProvider();
            var cryptoByteBuffer = new Byte[4];
 
            crypto.GetBytes(cryptoByteBuffer);
            var random = new Random(BitConverter.ToInt32(cryptoByteBuffer, 0));

            return Create(random);
        }
    }
}